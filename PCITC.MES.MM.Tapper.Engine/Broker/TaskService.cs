using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class TaskService : ITaskService
    {
        private readonly ITaskQueueService _queueService;
        private readonly ILogger _logger;
        private readonly object _syncObj = new object();

        public TaskService(ITaskQueueService queueService)
        {
            _queueService = queueService;
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
        }

        public bool IsTaskExisted(string topic,int ruleId,DateTime activeTime)
        {
            var queues = _queueService.QueryQueues(topic);
            foreach (var queue in queues)
            {
                if (queue.TaskEntities.Values.Any(t => t.RuleId == ruleId && t.TaskActiveAt == activeTime))
                    return true;
            }
            return false;
        }

        public bool AddTaskToQueue(TaskEntity task, int queueId)
        {
            var queue = _queueService.GetQueue(task.Topic, queueId);
            if (queue == null)
            {
                _logger.Error("尝试向不存在的队列添加任务:[Topic={0},QueueId={1}]", task.Topic, queueId);
                return false;
            }
            lock (_syncObj)
            {
                if (queue.TaskEntities.Values.All(t => t.RuleId != task.RuleId))
                {
                    if (queue.TaskEntities.TryAdd(task.TaskCode, task))
                    {
                        _logger.Info("任务加入队列:Topic={0},QueueId={1},Task={2}", task.Topic, queueId, task.ToString());
                        return true;
                    }
                    else
                    {
                        _logger.Error("任务入队失败:Topic={0},QueueId={1},Task={2}", task.Topic, queueId, task.ToString());
                        return false;
                    }
                }
            }
            return false;
        }

        public IEnumerable<TaskEntity> GetWaitingTaskFromQueue(string topic, int queueId, int batchSize)
        {
            var queue = _queueService.GetQueue(topic, queueId);
            if (queue == null)
            {
                _logger.Error("尝试从不存在的队列获取任务:[Topic={0},QueueId={1}]", topic, queueId);
                return new TaskEntity[0];
            }
            var tasks = new List<TaskEntity>();
            //2015-05-16  batch pull tasks from topic queue    --edited by JiaK
            lock (_syncObj)
            {
                var entities = queue.TaskEntities.Values.Where(t => t.IsWaiting).ToList();
                if (entities.Count == 0) return new TaskEntity[0];
                var taskCount = Math.Min(batchSize, entities.Count);
                var dispatchInfo = new StringBuilder();
                for (var i = 0; i < taskCount; i++)
                {
                    TaskEntity oldEntity;
                    queue.TaskEntities.TryGetValue(entities[i].TaskCode, out oldEntity);
                    if (oldEntity == null) continue;
                    entities[i].TaskDispatchAt = DateTime.Now;
                    entities[i].TaskDispatchCount++;
                    entities[i].TaskState = TaskState.Running;
                    if (queue.TaskEntities.TryUpdate(oldEntity.TaskCode, entities[i], oldEntity))
                    {
                        tasks.Add(entities[i]);
                        dispatchInfo.Append(string.Format("任务进行第{0}次调度：{1}", entities[i].TaskDispatchCount, entities[i].ToString()));
                    }
                    else
                    {
                        dispatchInfo.Append(string.Format("任务第{0}次调度失败：{1}", entities[i].TaskDispatchCount, entities[i].ToString()));
                    }
                }
                _logger.Info(dispatchInfo.ToString());
                return tasks;
            }
        }

        public bool UpdateTaskInQueue(TaskEntity task)
        {
            var queues = _queueService.QueryQueues(task.Topic);
            foreach (var queue in queues)
            {
                var oldTask = queue.TryGetTaskEntity(task.TaskCode);
                if (oldTask != null)
                {
                    queue.TaskEntities.TryUpdate(task.TaskCode, task, oldTask);
                    _logger.Info("任务状态更新:{0}",task.ToString());
                    //--------active child task when father task complete------------------
                    //--------do not active child task when father task failed------------------
                    //-------- V1.1.0 Content   -By JiaK   2015-05-25
                    if (task.RuleIdNext > 0 && task.TaskState == TaskState.Complete)
                    {
                        var childTask = queue.TryGetTaskEntity(task.RuleIdNext, TaskState.Created);
                        if (childTask == null)
                        {
                            _logger.Error("未找到子任务对应任务实体:RuleId={0}", task.RuleIdNext);
                            return false;
                        }
                        var newChildTask = childTask;
                        newChildTask.TaskState = TaskState.Waiting;
                        queue.TaskEntities.TryUpdate(childTask.TaskCode, newChildTask, childTask);
                        _logger.Info("任务链子任务激活:{0}", newChildTask.ToString());
                    }
                    return true;
                }
            }
            _logger.Error("没有在Broker调度队列中找到对应的任务进行状态更新:{0}",task.ToString());
            return false;
        }
    }
}
