using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;
using PCITC.MES.MM.Tapper.Framework.Serializing;

namespace PCITC.MES.MM.Tapper.Engine.Consumer
{
    public class Consumer
    {
        private readonly object _lockObject;
        private readonly List<string> _subscribedTopics;
        private readonly List<int> _taskIds;
        private readonly TaskFactory _taskFactory;
        private readonly ConcurrentDictionary<string, IList<int>> _topicQueueIdsDict;  //subscribed
        private readonly BlockingCollection<TaskEntity> _consumingTaskQueue;        //request tasks from broker cache
        private readonly IScheduleService _scheduleService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ITopicService _topicService;
        private readonly ITaskQueueService _queueService;
        private readonly ITaskService _taskService;
        // 2015-06-21 Edit by JiaK----
        // change the singleton taskHandler to named taskHandlers ,canceling the taskHandler injection;
        // solve the horilizing concurrent bugs; 
        // and initialize the handler dics in the subscribe event;
        //private readonly ITaskHandler _taskHandler;
        private ConcurrentDictionary<string, ITaskHandler> _taskHandlers; 
        private readonly IQueueSelector _queueSelector;
        private readonly IConsumerManager _consumerManager;
        private readonly BackWorker _processTaskWorker;
        private readonly ILogger _logger;
        private readonly INotifyService _notifyService;
        private bool _stopped;

        public string Id { get; private set; }
        public ConsumerSettings Setting { get; private set; }
        public IEnumerable<string> SubscriptedTopics
        {
            get { return _subscribedTopics; }
        }

        public Consumer(string id) : this(id, null) { }

        public Consumer(string id, ConsumerSettings setting)
        {
            if(id==null)throw new ArgumentNullException("id");
            Id = id;
            Setting = setting ?? new ConsumerSettings();
            _lockObject=new object();
            _subscribedTopics = new List<string>();
            _taskIds=new List<int>();
            _taskFactory= new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(Setting.ConsumeThreadMaxCount));
            _topicQueueIdsDict=new ConcurrentDictionary<string, IList<int>>();
            _consumingTaskQueue = new BlockingCollection<TaskEntity>();
            _topicService = ObjectContainer.Resolve<ITopicService>();
            _jsonSerializer = ObjectContainer.Resolve<IJsonSerializer>();
            _queueService = ObjectContainer.Resolve<ITaskQueueService>();
            _taskService = ObjectContainer.Resolve<ITaskService>();
            //2015-06-21 canceling the injection
            //_taskHandler = ObjectContainer.Resolve<ITaskHandler>();
            _scheduleService = ObjectContainer.Resolve<IScheduleService>();
            _queueSelector = ObjectContainer.Resolve<IQueueSelector>();
            _consumerManager = ObjectContainer.Resolve<IConsumerManager>();
            _processTaskWorker = new BackWorker("Consumer.ProcessTask", ProcessTask);
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
            _notifyService = ObjectContainer.Resolve<INotifyService>();
        }

        public Consumer Start()
        {
            _stopped = false;
            StartBackgroundJobs();
            TaskScheduler.UnobservedTaskException +=
                (_, ev) => LogUnobservedException(ev.Exception);
            _logger.Info("Consumer start:Id={0}",Id);
            _notifyService.AddInfoNotify(string.Format("任务处理器启动,Id={0}",Id),null,null);
            return this;
        }

        public Consumer Shutdown()
        {
            _stopped = true;
            StopBackgroundJobs();
            _logger.Info("Consumer shutdown:Id={0}", Id);
            _notifyService.AddInfoNotify(string.Format("任务处理器关闭,Id={0}", Id), null, null);
            return this;
        }

        public Consumer Subscribe(string topic)
        {
            if(!_subscribedTopics.Contains(topic))
                _subscribedTopics.Add(topic);
            if (!_taskHandlers.ContainsKey(topic))
            {
                if (!_taskHandlers.TryAdd(topic, new MvTaskHandler()))
                    _logger.Fatal("Task Handler initialized failed, Topic={0}", topic);
            }    
            return this;
        }

        private void StartBackgroundJobs()
        {
            lock (_lockObject)      //need to be atomic oprate
            {
                StopBackgroundJobsInternal();
                StartBackgroundJobsInternal();
            }
        }

        private void StopBackgroundJobs()
        {
            lock (_lockObject)      //need to be atomic oprate
            {
                StopBackgroundJobsInternal();
            }
        }

        private void StartBackgroundJobsInternal()
        {
            _taskIds.Add(_scheduleService.ScheduleTask("Consumer.RefreshTopicQueues", RefreshTopicQueues,
                5, Setting.UpdateTopicQueueCountInterval));
            _taskIds.Add(_scheduleService.ScheduleTask("Consumer.PullTaskFromBroker", PullTaskFromBroker,
                Setting.PullTaskFromBrokerInterval, Setting.PullMessageBatchSize));
            _taskIds.Add(_scheduleService.ScheduleTask("Consumer.HeartbeatBroker", HeartbeatBroker,
                Setting.HeartbeatBrokerInterval, Setting.HeartbeatBrokerInterval));
            _processTaskWorker.Start();
        }

        private void StopBackgroundJobsInternal()
        {
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
            _taskIds.Clear();
            _topicQueueIdsDict.Clear();
        }

        #region 本地任务队列信息与broker任务队列信息同步
        private void RefreshTopicQueues()
        {
            foreach (var topic in _subscribedTopics)
            {
                UpdateTopicQueue(topic);
            }
        }

        private void UpdateTopicQueue(string topic)
        {
            try
            {
                var topicQueueIdsFromServer = _queueService.QueryQueueIds(topic);
                IList<int> currentQueueIds;
                var topicQueueIdsOfLocal = _topicQueueIdsDict.TryGetValue(topic, out currentQueueIds)
                    ? currentQueueIds
                    : new List<int>();
                if (IsIntCollectionChanged(topicQueueIdsFromServer, topicQueueIdsOfLocal))
                {
                    _topicQueueIdsDict[topic] = topicQueueIdsFromServer;
                    _logger.Info("任务队列变更,已进行同步:ConsumerId={0},Topic={1},Old QueueIds=[{2}],New QueueIds=[{3}]", Id,
                        topic, string.Join(",", topicQueueIdsOfLocal), string.Join(",", topicQueueIdsFromServer));
                }
            }
            catch (Exception exp)
            {
                _logger.Error(string.Format("任务队列更新失败: ConsumerId={0}, Topic={1}", Id, topic), exp);
            }
        }

        private bool IsIntCollectionChanged(IList<int> first, IList<int> second)
        {
            if (first.Count != second.Count)
            {
                return true;
            }
            return first.Where((x, index) => x != second[index]).Any();
        }
        #endregion

        #region 从Broker pull任务
        private void PullTaskFromBroker()
        {
            try
            {
                if (_stopped) return;
                var taskCount = GetTaskCount();
                if (taskCount >= Setting.PullThresholdForQueue)
                {
                    _logger.Warn("Consumer had too many tasks waiting for process, task pulling suspended. Consumer Id={0}",Id);
                    return;
                }
                var selectedTopic = _queueSelector.SelectTopic(_topicQueueIdsDict.Keys.ToList());
                var selectedQueueId = _queueSelector.SelectQueueIdInTopic(_topicQueueIdsDict[selectedTopic].ToList());
                var tasks = _taskService.GetWaitingTaskFromQueue(selectedTopic, selectedQueueId,
                    Setting.PullMessageBatchSize).ToList();
                if (!tasks.Any()) return;
                var sucCount = 0;
                lock (_consumingTaskQueue)
                {
                    sucCount += tasks.Count(task => _consumingTaskQueue.TryAdd(task));
                }
                _logger.Info("Pull tasks: Topic:{2},QueueId:{3}[Success:{0}条, Fail:{1}条].", sucCount,
                    (tasks.Count - sucCount), selectedTopic, selectedQueueId);
            }
            catch (Exception exp)
            {
                _logger.Error(string.Format("Pull tasks exception. ConsumerId:{0}", Id), exp);
            }
        }

        private int GetTaskCount()
        {
            lock (_consumingTaskQueue)
            {
                return _consumingTaskQueue.Count;
            }
        }
        #endregion

        #region 从consumingQueue获取待处理任务，处理后放入processedQueue
        private void ProcessTask()
        {
            TaskEntity consumingTask;
            if (!_consumingTaskQueue.TryTake(out consumingTask)) return;
            if (_stopped) return;
            var processAction = new Action(() =>
            {
                _logger.Info("Task process begin:{0}", consumingTask.ToString());
                _notifyService.AddInfoNotify(string.Format("开始处理任务:{0}", consumingTask.ToString()), consumingTask, null);
                ProcessTask(consumingTask);
            });
            if (Setting.TaskHandleMode == TaskHandleMode.Sequential)
            {
                processAction();
            }
            else if (Setting.TaskHandleMode == TaskHandleMode.Parallel)
            {
                _taskFactory.StartNew(processAction);
            }
        }

        private void ProcessTask(TaskEntity task)
        {
            if (_stopped) return;
            if (task == null) return;
            try
            {
                // task handling...............
                var taskTopic = _topicService.GetTopicModelByName(task.Topic);
                var taskParams = _jsonSerializer.Deserialize<Dictionary<string, string>>(task.TaskParameters);
                var newParams = new Dictionary<string, string>();
                // time param reload
                foreach (var param in taskParams)
                {
                    if (IsTimeWithDayBias(param.Value))
                    {
                        var timeSplit = param.Value.Split('(');
                        var dayBias = int.Parse(timeSplit[1].TrimEnd(')'));
                        var newTime = DateTime.Parse(task.TaskActiveAt.ToString("yyyy-MM-dd ") + timeSplit[0])
                            .AddDays(dayBias)
                            .ToString("yyyy-MM-dd HH:mm:ss");
                        newParams.Add(param.Key, newTime);
                    }
                    else
                    {
                        newParams.Add(param.Key,param.Value);
                    }
                }
                dynamic result;
                //-- 2015-6-21 Edit By JiaK
                //lock (_taskHandler)
                //{
                //    _taskHandler.ServiceUrl = taskTopic.ServiceUrl;
                //    _taskHandler.ServiceBinding = taskTopic.ServiceBinding;
                //    result = _taskHandler.TaskExcute(task.Topic, task.TaskAction, newParams);
                //}
                if (string.IsNullOrWhiteSpace(_taskHandlers[task.Topic].ServiceUrl))
                {
                    _taskHandlers[task.Topic].ServiceUrl = taskTopic.ServiceUrl;
                    _taskHandlers[task.Topic].ServiceBinding = taskTopic.ServiceBinding;
                }
                result = _taskHandlers[task.Topic].TaskExcute(task.Topic, task.TaskAction, newParams);

                //dynamic result = _taskHandler.TaskExcute(task.Topic, task.TaskAction, newParams);
                if (result)
                {
                    task.TaskState = TaskState.Complete;
                    // task state update
                    _taskService.UpdateTaskInQueue(task);
                    _logger.Info("Process task successful:{0}", task.ToString());
                    _notifyService.AddInfoNotify(string.Format("任务处理成功:{0}", task.ToString()), task, null);
                }
                else
                {
                    task.TaskState = TaskState.Failed;
                    // task state update
                    _taskService.UpdateTaskInQueue(task);
                    _logger.Error(string.Format("Process task failed:{0}", task.ToString()));
                    _notifyService.AddErrorNotify(string.Format("任务处理失败:{0}", task.ToString()), task, null);
                }
            }
            catch (Exception exp)
            {
                task.TaskState = TaskState.Failed;
                // task state update
                _taskService.UpdateTaskInQueue(task);
                _logger.Error(string.Format("Process task failed:{0}", task.ToString()), exp);
                _notifyService.AddErrorNotify(string.Format("任务处理失败:{0}", task.ToString()),task,exp);
            }
        }

        private static bool IsTimeWithDayBias(string paramStr)
        {
            var regex = new Regex(@"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d\([-]?[0-1]+\))$");
            return regex.IsMatch(paramStr);
        }
        #endregion

        private void HeartbeatBroker()
        {
            _consumerManager.UpdateConsumer(Id, _subscribedTopics);
        }

        private void LogUnobservedException(Exception exp)
        {
            _logger.Fatal("Task schedule unobserved exception:", exp);
        }
    }
}
