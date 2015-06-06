using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;

namespace PCITC.MES.MM.Tapper.Engine.Producer
{
    public class Producer
    {
        private readonly object _lockObject;
        private readonly ConcurrentDictionary<string, IList<int>> _topicQueueIdsDict;
        private readonly ITaskQueueService _queueService;
        private readonly ITaskService _taskService;
        private readonly IScheduleService _scheduleService;
        private readonly IModelService _modelService;
        private readonly IQueueSelector _queueSelector;
        private readonly ILogger _logger;
        private readonly INotifyService _notifyService;
        private readonly List<int> _taskIds;

        public string Id { get; private set; }

        public ProducerSettings Setting { get; private set; }

        public Producer(string id) : this(id, null) { }
        public Producer(string id, ProducerSettings setting)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            Id = id;
            Setting = setting ?? new ProducerSettings();
            _lockObject = new object();
            _taskIds = new List<int>();
            _topicQueueIdsDict = new ConcurrentDictionary<string, IList<int>>();
            _queueService = ObjectContainer.Resolve<ITaskQueueService>();
            _taskService = ObjectContainer.Resolve<ITaskService>();
            _scheduleService = ObjectContainer.Resolve<IScheduleService>();
            _modelService = ObjectContainer.Resolve<IModelService>();
            _queueSelector = ObjectContainer.Resolve<IQueueSelector>();
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
            _notifyService = ObjectContainer.Resolve<INotifyService>();
        }

        public Producer Start()
        {
            _modelService.Start();
            StartBackgroundJobs();
            _logger.Info("Producer start:Id={0}", Id);
            _notifyService.AddInfoNotify(string.Format("任务生成器启动,Id={0}",Id),null,null);
            return this;
        }

        public Producer Shutdown()
        {
            StopBackgroundJobs();
            _logger.Info("Producer shutdown:Id={0}", Id);
            _notifyService.AddInfoNotify(string.Format("任务生成器关闭,Id={0}", Id), null, null);
            return this;
        }

        private void StartBackgroundJobs()
        {
            lock (_lockObject)      //启动任务必须为原子操作
            {
                StopBackgroundJobsInternal();
                StartBackgroundJobsInternal();
            }
        }

        private void StopBackgroundJobs()
        {
            lock (_lockObject)      //中止操作必须为原子操作
            {
                StopBackgroundJobsInternal();
            }
        }

        private void StartBackgroundJobsInternal()
        {
            _taskIds.Add(_scheduleService.ScheduleTask("Producer.RefreshTopicQueues", RefreshTopicQueues,
                5, Setting.UpdateTopicQueueCountInterval));
            _taskIds.Add(_scheduleService.ScheduleTask("Producer.GenerateTaskFromRules", GenerateTaskFromRules,
                Setting.GenerateTaskFromRulesInterval, Setting.GenerateTaskFromRulesInterval));
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
            var topics = _queueService.GetAllTopics();
            foreach (var topic in topics)
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
                    _logger.Info("任务队列变更,已进行同步:ProducerId={0},Topic={1},Old QueueIds=[{2}],New QueueIds=[{3}]", Id,
                        topic, string.Join(",", topicQueueIdsOfLocal), string.Join(",", topicQueueIdsFromServer));
                }
            }
            catch (Exception exp)
            {
                _logger.Error(string.Format("任务队列更新失败: ProducerId={0}, Topic={1}", Id, topic), exp);
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

        #region 通过规则及任务模型生成任务实体
        private void GenerateTaskFromRules()
        {
            var rules =
                _modelService.GetRuleModels()
                    .Values.Where(r => r.RuleBegDate <= DateTime.Today && r.RuleEndDate > DateTime.Today).ToList();
            var tasks = _modelService.GetTaskModels();
            var filtedRules = GetActiveRules(rules);
            //生成任务实体
            foreach (var rule in filtedRules)
            {
                var task = tasks.Values.FirstOrDefault(t => t.TaskId == rule.TaskId);
                if (task == null)
                {
                    _logger.Error("规则模型没有对应的任务模型.TaskId={0}",rule.TaskId);
                    continue;
                }
                var entity = GetEntityFromModel(rule, task);
                if (entity != null)
                {
                    IList<int> queueIds;         //指定topic下queue id列表
                    _topicQueueIdsDict.TryGetValue(rule.Topic, out queueIds);
                    if (queueIds == null)
                    {
                        _logger.Warn("未找到对应的任务队列.Topic={0}",rule.Topic);
                        break;
                    }
                    var selectedQueueId = _queueSelector.SelectQueueId(queueIds, null, null);
                    _taskService.AddTaskToQueue(entity, selectedQueueId);
                }
            }
        }

        private TaskEntity GetEntityFromModel(RuleModel rule, TaskModel task)
        {
            var activeAt = JoinTodayAndTime(rule.RuleActiveTime);
            var expireAt = JoinTodayAndTime(rule.RuleExpireTime);
            if (activeAt > expireAt)
                activeAt = activeAt.AddDays(-1);
            if (_taskService.IsTaskExisted(rule.Topic, rule.RuleId, activeAt)) return null;
            if (!IsParamValidate(rule.TaskParams))
            {
                _logger.Error("规则参数不是合法的JSON字符串.Rule={0}",rule.ToString());
                return null;
            }
            return new TaskEntity
            {
                Topic = rule.Topic,
                TaskCode = Guid.NewGuid().ToString("N"),
                RuleId = rule.RuleId,
                RuleName = rule.RuleName,
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                NodeId = rule.NodeId,
                NodeName = rule.NodeName,
                NodeTypeId = rule.NodeTypeId,
                NodeTypeName = rule.NodeTypeName,
                TaskAction = task.TaskAction,
                TaskParameters = rule.TaskParams,
                TaskActiveAt = activeAt,
                TaskExpireAt = expireAt,
                TaskMaxDispatchTime = task.TaskMaxDispatchTime,
                TaskDispatchAt = new DateTime(2099, 12, 31),
                TaskDispatchCount = 0,
                TaskState = TaskState.Waiting
            };
        }

        private IList<RuleModel> GetActiveRules(IList<RuleModel> rules)
        {
            //过滤生效日期
            var dateFiltedRules=rules.Where(r => r.RuleBegDate <= DateTime.Today && r.RuleEndDate > DateTime.Today);
            var timeFiltedRules = new List<RuleModel>();
            foreach (var rule in dateFiltedRules)
            {
                //过滤生效周期
                var dayOfMonth = GetIntsFromStr(rule.RuleDaysInMonth);
                var dayOfWeek = GetIntsFromStr(rule.RuleDaysInWeek);
                if (dayOfMonth.Count > 0)
                {
                    if (dayOfMonth.All(d => d != DateTime.Today.Day)) continue;
                }
                if (dayOfWeek.Count > 0)
                {
                    if (dayOfWeek.All(d => d != (int)DateTime.Today.DayOfWeek)) continue;
                }
                //过滤激活时间
                var activeAt = JoinTodayAndTime(rule.RuleActiveTime);
                var expireAt = JoinTodayAndTime(rule.RuleExpireTime);
                if (activeAt > expireAt)
                    activeAt = activeAt.AddDays(-1);
                if (activeAt <= DateTime.Now && expireAt > DateTime.Now)
                    timeFiltedRules.Add(rule);
            }
            return timeFiltedRules;
        } 

        private IList<int> GetIntsFromStr(string str)
        {
            try
            {
                return string.IsNullOrEmpty(str) ? new List<int>() : str.Split(',').Select(int.Parse).ToList();
            }
            catch(Exception)
            {
                _logger.Error("周期字符串配置错误：{0}",str);
                return new List<int>();
            }
        }

        private static DateTime JoinTodayAndTime(DateTime time)
        {
            DateTime joinTime;
            DateTime.TryParse(string.Join(DateTime.Today.ToString("yyyy-MM-dd "), time.ToLongTimeString()), out joinTime);
            return joinTime;
        }

        private static bool IsParamValidate(string paramJson)
        {
            var jsonRegex=new Regex(@"^\{((\S,)*\S)*\}$");
            return jsonRegex.IsMatch(paramJson);
        }
        #endregion
    }
}
