using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Dapper;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;

namespace PCITC.MES.MM.Tapper.Engine.Producer
{
    public class ModelService : IModelService
    {
        private readonly ConcurrentDictionary<int, RuleModel> _ruleModelDict;
        private readonly ConcurrentDictionary<int, TaskModel> _taskModelDict;
        private readonly IScheduleService _scheduleService;
        private readonly ILogger _logger;
        private readonly List<int> _taskIds;
        private int _isLoadingModelTask;        //加载规则及任务模型的基元线程同步信号量
        public ModelSettings Setting { get; private set; }

        public ModelService(IScheduleService scheduleService, ILog4NetLoggerFactory loggerFactory)
        {
            _taskIds = new List<int>();
            Setting = new ModelSettings();
            _ruleModelDict = new ConcurrentDictionary<int, RuleModel>();
            _taskModelDict = new ConcurrentDictionary<int, TaskModel>();
            _scheduleService = scheduleService;
            _logger = loggerFactory.Create(GetType().FullName);
        }

        public void Start()
        {
            _ruleModelDict.Clear();
            _taskModelDict.Clear();
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
            _taskIds.Add(_scheduleService.ScheduleTask("ModelService.RefreshModelInMemory", RefreshModelInMemory,
                3, Setting.GetModelsFromDatabaseInterval));
        }

        public void Shutdown()
        {
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
        }

        public ConcurrentDictionary<int, RuleModel> GetRuleModels()
        {
            return _ruleModelDict;
        }

        public ConcurrentDictionary<int, TaskModel> GetTaskModels()
        {
            return _taskModelDict;
        }

        private void RefreshModelInMemory()
        {
            if (Interlocked.CompareExchange(ref _isLoadingModelTask, 1, 0) != 0) return;
            _logger.Info("开始同步规则模型及任务模型...");
            try
            {
                using (var connection = new OracleConnection(Setting.ConnectionStr))
                {
                    connection.Open();
                    var rules = connection.QueryList<RuleModel>(null, Setting.RuleModelTable,"*");
                    var loadRuleCount = 0;
                    if (rules != null)
                    {
                        //清理过期规则
                        var expireRules = rules.Where(r => r.RuleEndDate <= DateTime.Today).ToList();
                        if (expireRules.Count > 0)
                        {
                            foreach (var expireRule in expireRules)
                            {
                                var count = connection.Delete(new { RuleId = expireRule.RuleId }, Setting.RuleModelTable);
                                if (count > 0)
                                {
                                    _logger.Info("已删除过期规则模型:{0}", expireRule.ToString());
                                }
                                else
                                {
                                    _logger.Error("删除过期规则模型失败:{0}", expireRule.ToString());
                                }
                            }
                        }
                        var activeRules = rules.Where(r => r.RuleEndDate > DateTime.Today);
                        _ruleModelDict.Clear();
                        foreach (var rule in activeRules)
                        {
                            if (_ruleModelDict.TryAdd(rule.RuleId, rule))
                                loadRuleCount++;
                        }
                    }
                    var tasks = connection.QueryList<TaskModel>(null, Setting.TaskModelTable,"*");
                    if (tasks != null)
                    {
                        _taskModelDict.Clear();
                        foreach (var task in tasks)
                        {
                            _taskModelDict.TryAdd(task.TaskId, task);
                        }
                    }
                    connection.Close();
                    _logger.Info("加载规则模型{0}条，任务模型{1}条.", loadRuleCount, tasks.Count());
                }
            }
            catch (Exception exp)
            {
                _logger.Error("数据库加载模型数据失败.", exp);
            }
            finally
            {
                Interlocked.Exchange(ref _isLoadingModelTask, 0);
            }
        }
    }
}
