using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;


namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class ConsumerManager : IConsumerManager
    {
        private readonly ConcurrentDictionary<string, IList<string>> _consumerSubScribedTopics;
        private readonly ConcurrentDictionary<string, DateTime> _consumerHeartbeatTimes; 
        private readonly IScheduleService _scheduleService;
        private readonly ILogger _logger;
        private readonly IList<int> _taskIds;

        public ConsumerManagerSettings Setting { get; private set; }

        public ConsumerManager() : this(null) { }
        public ConsumerManager(ConsumerManagerSettings setting)
        {
            Setting = setting ?? new ConsumerManagerSettings();
            _consumerSubScribedTopics = new ConcurrentDictionary<string, IList<string>>();
            _consumerHeartbeatTimes = new ConcurrentDictionary<string, DateTime>();
            _scheduleService = ObjectContainer.Resolve<IScheduleService>();
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
            _taskIds = new List<int>();
        }

        public void Start()
        {
            _consumerSubScribedTopics.Clear();
            _taskIds.Add(_scheduleService.ScheduleTask("ConsumerManager.CheckHeartbeatTimes", CheckHeartbeatTimes,
                Setting.ScanConsumerInterval, Setting.ScanConsumerInterval));
        }

        public void Shutdown()
        {
            foreach (var taskId in _taskIds)
            {
                _scheduleService.ShutdownTask(taskId);
            }
        }

        public IList<string> GetConsumerSubscribedTopics(string consumerId)
        {
            IList<string> subscribedTopics;
            lock (_consumerSubScribedTopics)
            {
                _consumerSubScribedTopics.TryGetValue(consumerId, out subscribedTopics);
            }
            return subscribedTopics;
        }

        public IList<string> GetActiveConsumers()
        {
            ICollection<string> activeConsumers;
            lock (_consumerHeartbeatTimes)
            {
                activeConsumers = _consumerHeartbeatTimes.Keys;
            }
            return activeConsumers.ToList();
        }

        public int GetActiveConsumersCount()
        {
            lock (_consumerHeartbeatTimes)
            {
                return _consumerHeartbeatTimes.Keys.Count;
            }
        }

        public void UpdateConsumer(string consumerId, IList<string> subscribedTopics)
        {
            IList<string> oldSubscribedTopics;
            if (_consumerSubScribedTopics.TryGetValue(consumerId, out oldSubscribedTopics))
            {
                if (!_consumerSubScribedTopics.TryUpdate(consumerId, subscribedTopics, oldSubscribedTopics))
                {
                    _logger.Error("Broker consumer status update failed:ConsumerId:{0}", consumerId);
                }
            }
            else
            {
                if (!_consumerSubScribedTopics.TryAdd(consumerId, subscribedTopics))
                {
                    _logger.Error("Broker consumer status add failed:ConsumerId:{0}", consumerId);
                }
            }
            DateTime oldHeartbeatTime;
            if (_consumerHeartbeatTimes.TryGetValue(consumerId, out oldHeartbeatTime))
            {
                if (!_consumerHeartbeatTimes.TryUpdate(consumerId, DateTime.Now, oldHeartbeatTime))
                {
                    _logger.Error("Broker consumer heartbeat time update failed:ConsumerId:{0}", consumerId);
                }
            }
            else
            {
                if (!_consumerHeartbeatTimes.TryAdd(consumerId, DateTime.Now))
                {
                    _logger.Error("Broker consumer heartbeat time add failed:ConsumerId:{0}", consumerId);
                }
            }
        }

        private void CheckHeartbeatTimes()
        {
            if (_consumerHeartbeatTimes.Count == 0) return;
            foreach (var heartbeatTime in _consumerHeartbeatTimes)
            {
                if (heartbeatTime.Value.AddSeconds(Setting.ConsumerActiveTimeout) >= DateTime.Now) continue;
                DateTime lastHeartbeatTime;
                IList<string> subscribedTopics;
                if (!_consumerSubScribedTopics.TryRemove(heartbeatTime.Key, out subscribedTopics))
                {
                    _logger.Error("Consumermanager clear inactive consumer-topics failed: ConsumerId={0}", heartbeatTime.Key);
                }
                if (!_consumerHeartbeatTimes.TryRemove(heartbeatTime.Key, out lastHeartbeatTime))
                {
                    _logger.Error("Consumermanager clear inactive consumer-heartbeatTime failed: ConsumerId={0}", heartbeatTime.Key);
                }
            }
        }
    }
}
