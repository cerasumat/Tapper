using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Threading;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Dapper;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class NotifyInOracleService : INotifyService
    {
        private readonly ConcurrentDictionary<string, NotifyEntity> _notifyEntityDict;
        private readonly IScheduleService _scheduleService;
        private readonly List<int> _taskIds;
        private readonly ILogger _logger;
        private int _isSavingNotification;        //持久化通知消息基元线程同步信号量

        public NotifySettings Setting { get; private set; }

        public NotifyInOracleService()
        {
            _taskIds = new List<int>();
            Setting = new NotifySettings();
            _notifyEntityDict = new ConcurrentDictionary<string, NotifyEntity>();
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
            _scheduleService = ObjectContainer.Resolve<IScheduleService>();
        }

        public void Start()
        {
            _notifyEntityDict.Clear();
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
            _taskIds.Add(_scheduleService.ScheduleTask("NotifyInOracleService.StoreNotifyInOracle", StoreNotifyInOracle,
               Setting.NotifyInfoStoreInterval, Setting.NotifyInfoStoreInterval));
        }

        public void Shutdown()
        {
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
        }

        public void AddInfoNotify(string message, TaskEntity task, Exception exp)
        {
            var notification = GetNotification("INFO", message, task, exp);
            _notifyEntityDict.TryAdd(notification.NotifyCode, notification);
        }

        public void AddDebugNotify(string message, TaskEntity task, Exception exp)
        {
            var notification = GetNotification("DEBUG", message, task, exp);
            _notifyEntityDict.TryAdd(notification.NotifyCode, notification);
        }

        public void AddWarnNotify(string message, TaskEntity task, Exception exp)
        {
            var notification = GetNotification("WARN", message, task, exp);
            _notifyEntityDict.TryAdd(notification.NotifyCode, notification);
        }

        public void AddErrorNotify(string message, TaskEntity task, Exception exp)
        {
            var notification = GetNotification("ERROR", message, task, exp);
            _notifyEntityDict.TryAdd(notification.NotifyCode, notification);
        }

        public void AddFatalNotify(string message, TaskEntity task, Exception exp)
        {
            var notification = GetNotification("FATAL", message, task, exp);
            _notifyEntityDict.TryAdd(notification.NotifyCode, notification);
        }

        private void StoreNotifyInOracle()
        {
            if (_notifyEntityDict.IsEmpty) return;
            if (Interlocked.CompareExchange(ref _isSavingNotification, 1, 0) != 0) return;
            _logger.Info("开始持久化通知信息至Oracle...");
            try
            {
                using (var connection = new OracleConnection(Setting.ConnectionStr))
                {
                    connection.Open();
                    foreach (var key in _notifyEntityDict.Keys)
                    {
                        NotifyEntity notify;
                        if (_notifyEntityDict.TryRemove(key, out notify))
                        {
                            connection.Insert(notify, Setting.NotifyInfoTable);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                _logger.Error("持久化通知信息至Oracle失败.", exp);
            }
            finally
            {
                Interlocked.Exchange(ref _isSavingNotification, 0);
            }
        }

        private static NotifyEntity GetNotification(string level, string message, TaskEntity task, Exception exp)
        {
            return new NotifyEntity
            {
                NotifyLevel = level,
                NotifyContent = message,
                NotifyTime = DateTime.Now,
                NotifyCode = Guid.NewGuid().ToString("N"),
                Topic = task == null ? string.Empty : task.Topic,
                TaskCode = task == null ? string.Empty : task.TaskCode,
                NotifyTarget = task == null ? string.Empty : task.NodeName,
                StackInfo = exp == null ? string.Empty : exp.Message
            };
        }
    }
}
