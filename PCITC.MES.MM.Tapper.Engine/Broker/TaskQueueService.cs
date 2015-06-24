using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class TaskQueueService : ITaskQueueService
    {
        private readonly ConcurrentDictionary<string, TaskQueue> _queueDict;
        private readonly ITopicService _topicService;
        private readonly IScheduleService _scheduleService;
        private readonly INotifyService _notifyService;
        private readonly ILogger _logger;
        private readonly IList<int> _taskIds;
        private int _isCheckingFailedTask;              //检查失败(包含过期，超时，N次失败等检查机制)任务的基元线程同步信号量
        private int _isRemovingConsumedTask;            //移除任务（completed,failed,expired,canceled)的基元线程同步信号量
                                                        //参考：《基元线程同步——Interlocked Anything模式》

        // queue count in the queue dictionary
        public int QueueCount => _queueDict.Count;
        // task count in all of the queues
        public long TaskCount => _queueDict.Values.Sum(x => x.TaskCount);
        public long ActiveTaskCount => _queueDict.Values.Sum(x => x.ActiveTaskCount);

        public TaskQueueService(IScheduleService scheduleService, ILog4NetLoggerFactory loggerFactory)
        {
            _taskIds=new List<int>();
            _queueDict = new ConcurrentDictionary<string, TaskQueue>();
            _topicService = ObjectContainer.Resolve<ITopicService>();
            _notifyService = ObjectContainer.Resolve<INotifyService>();
            _scheduleService = scheduleService;
            _logger = loggerFactory.Create(GetType().FullName);
        }

        public void Start()
        {
            _queueDict.Clear();
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
            var topicNames = _topicService.AllTopics;
            if (topicNames == null || topicNames.Count == 0)
            {
                _logger.Fatal("加载Topic模型失败，请检查系统配置.");
                return;
            }
            var queues = topicNames.Select(topicName => new TaskQueue(topicName, 1)).ToList();
            foreach (var queue in queues)
            {
                var key = CreateQueueKey(queue.Topic, queue.QueueId);
                _queueDict.TryAdd(key, queue);
            }
            _taskIds.Add(_scheduleService.ScheduleTask("TaskQueueService.RemoveConsumedTask", RemoveConsumedTask,
                queues[0].Setting.RemoveTaskFromQueueInterval, queues[0].Setting.RemoveTaskFromQueueInterval));
            _taskIds.Add(_scheduleService.ScheduleTask("TaskQueueService.CheckFaildTask", CheckFailedTask,
                queues[0].Setting.CheckFailedTaskInterval,queues[0].Setting.CheckFailedTaskInterval));
        }

        public void Shutdown()
        {
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
        }

        public IEnumerable<string> GetAllTopics()
        {
            return _queueDict.Values.Select(x => x.Topic).Distinct();
        }

        public bool IsQueueExist(string topic, int queueId)
        {
            var key = CreateQueueKey(topic, queueId);
            return _queueDict.ContainsKey(key);
        }

        public TaskQueue GetQueue(string topic, int queueId)
        {
            var key = CreateQueueKey(topic, queueId);
            TaskQueue queue;
            return _queueDict.TryGetValue(key, out queue) ? queue : null;
        }

        public IList<int> QueryQueueIds(string topic)
        {
            return _queueDict.Values.Where(x => x.Topic.Equals(topic)).Select(x => x.QueueId).ToList();
        }

        public IEnumerable<TaskQueue> QueryQueues(string topic)
        {
            return _queueDict.Values.Where(x => x.Topic.Equals(topic));
        }

        private static string CreateQueueKey(string topic, int queueId)
        {
            return string.Format("{0}-{1}", topic, queueId);
        }

        private void CheckFailedTask()
        {
            if (Interlocked.CompareExchange(ref _isCheckingFailedTask, 1, 0) != 0) return;
            try
            {
                TaskEntity updateEntity; // safe update taskentity
                foreach (var taskQueue in _queueDict.Values)
                {
                    foreach (var taskEntity in taskQueue.TaskEntities.Values)
                    {
                        switch (taskEntity.TaskState)
                        {
                            case TaskState.Running:
                                if (taskEntity.TaskTimeOutAt <= DateTime.Now)
                                {
                                    if (!taskQueue.TaskEntities.TryGetValue(taskEntity.TaskCode, out updateEntity))
                                    {
                                        _logger.Error("未获取到需要更新状态的任务:{0}", taskEntity.ToString());
                                        continue;
                                    }
                                    if (updateEntity.TaskDispatchCount >= taskQueue.Setting.TaskMaxRetryTimes)
                                    {
                                        updateEntity.TaskState = TaskState.Failed;
                                        _logger.Error("任务失败:{0}", updateEntity.ToString());
                                        _notifyService.AddErrorNotify(string.Format("任务失败:{0}",updateEntity.ToString()),updateEntity,null);
                                    }
                                    else
                                    {
                                        updateEntity.TaskState = TaskState.Waiting;
                                        _logger.Warn("任务第{0}次调度超时,准备重新调度:{1}", updateEntity.TaskDispatchCount,
                                            updateEntity.ToString());
                                        _notifyService.AddErrorNotify(
                                            string.Format("任务第{0}次调度超时,准备重新调度:{1}", updateEntity.TaskDispatchCount,
                                                updateEntity.ToString()), updateEntity, null);
                                    }
                                    taskQueue.TaskEntities.TryUpdate(updateEntity.TaskCode, updateEntity, taskEntity);
                                }
                                break;
                            case TaskState.Waiting:
                                if (taskEntity.TaskDispatchCount >= taskQueue.Setting.TaskMaxRetryTimes)
                                {
                                    if (!taskQueue.TaskEntities.TryGetValue(taskEntity.TaskCode, out updateEntity))
                                    {
                                        _logger.Error("未获取到需要更新状态的任务:{0}", taskEntity.ToString());
                                        continue;
                                    }
                                    updateEntity.TaskState = TaskState.Failed;
                                    _logger.Error("任务失败:{0}", updateEntity.ToString());
                                    taskQueue.TaskEntities.TryUpdate(updateEntity.TaskCode, updateEntity, taskEntity);
                                    _notifyService.AddErrorNotify(string.Format("任务失败:{0}", updateEntity.ToString()), updateEntity, null);
                                }
                                break;
                            case TaskState.Failed:
                                if (taskEntity.TaskDispatchCount < taskQueue.Setting.TaskMaxRetryTimes)
                                {
                                    if (!taskQueue.TaskEntities.TryGetValue(taskEntity.TaskCode, out updateEntity))
                                    {
                                        _logger.Error("未获取到需要更新状态的任务:{0}", taskEntity.ToString());
                                        continue;
                                    }
                                    updateEntity.TaskState = TaskState.Waiting;
                                    _logger.Error("任务第{0}次调度失败,准备重新调度:{1}", updateEntity.TaskDispatchCount,
                                            updateEntity.ToString());
                                    taskQueue.TaskEntities.TryUpdate(updateEntity.TaskCode, updateEntity, taskEntity);
                                    _notifyService.AddErrorNotify(
                                            string.Format("任务第{0}次调度失败,准备重新调度:{1}", updateEntity.TaskDispatchCount,
                                                updateEntity.ToString()), updateEntity, null);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _logger.Error("任务失败检查异常.",exp);
            }
            finally
            {
                Interlocked.Exchange(ref _isCheckingFailedTask, 0);
            }
        }

        private void RemoveConsumedTask()
        {
            if (Interlocked.CompareExchange(ref _isRemovingConsumedTask, 1, 0) != 0) return;
            try
            {
                TaskEntity removedEntity; // safe removed taskentity
                foreach (var taskQueue in _queueDict.Values)
                {
                    foreach (var taskEntity in taskQueue.TaskEntities.Values)
                    {
                        // business handled logic modified by JiaK. 2015-05-14 22:42
                        // only the expired task can be removed avoiding the duplicated task generated by the rules
                        if (taskEntity.IsExpired)
                        {
                            taskQueue.TaskEntities.TryRemove(taskEntity.TaskCode, out removedEntity);
                            _logger.Info("任务移除:{0}", removedEntity.ToString());
                            _notifyService.AddInfoNotify(string.Format("任务结束移除:{0}", removedEntity.ToString()),
                                removedEntity, null);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _logger.Error("任务移除失败.", exp);
            }
            finally
            {
                Interlocked.Exchange(ref _isRemovingConsumedTask, 0);
            }
        }
    }
}
