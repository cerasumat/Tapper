using System;
using System.Linq;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Engine.SignalR;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class Broker
    {
        private static Broker _instance;
        private readonly ILogger _logger;
        private readonly ITopicService _topicService;
        private readonly ITaskQueueService _queueService;
        private readonly IConsumerManager _consumerManager;
        private readonly INotifyService _notifyService;
        private readonly ITaskService _taskService;
        private readonly INotify _notify;

        public BrokerSettings Setting { get; private set; }
        public static Broker Instance
        {
            get { return _instance; }
        }

        public string NotifyUrl
        {
            get { return _notify.GetUrl(); }
        }

        private Broker(BrokerSettings setting)
        {
            Setting = setting ?? new BrokerSettings();
            _topicService = ObjectContainer.Resolve<ITopicService>();
            _queueService = ObjectContainer.Resolve<ITaskQueueService>();
            _taskService = ObjectContainer.Resolve<ITaskService>();
            _consumerManager = ObjectContainer.Resolve<IConsumerManager>();
            _notifyService = ObjectContainer.Resolve<INotifyService>();
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
            _notify = ObjectContainer.Resolve<INotify>();
        }

        public static Broker Create(BrokerSettings setting = null)
        {
            if (_instance != null)
            {
                throw new NotSupportedException("Broker已创建.");
            }
            _instance = new Broker(setting);
            return _instance;
        }

        public Broker Start()
        {
            _topicService.Start();
            _queueService.Start();
            _consumerManager.Start();
            _notifyService.Start();
            //_notify.Start();
            _logger.Info("Broker started, producer=[{0}], consumer=[{1}], admin=[{2}]", Setting.ProducerIPEndPoint, Setting.ConsumerIPEndPoint, Setting.AdminIPEndPoint);
            _notifyService.AddDebugNotify("任务调度器启动", null, null);
            return this;
        }

        public Broker Shutdown()
        {
            _queueService.Shutdown();
            _logger.Info("Broker shutdown, producer=[{0}], consumer=[{1}], admin=[{2}]", Setting.ProducerIPEndPoint, Setting.ConsumerIPEndPoint, Setting.AdminIPEndPoint);
            _notifyService.AddDebugNotify("任务调度器关闭", null, null);
            return this;
        }

        public BrokerStatisticInfo GetBrokerStatisticInfo()
        {
            var statisticInfo = new BrokerStatisticInfo();
            statisticInfo.TopicCount = _queueService.GetAllTopics().Count();
            statisticInfo.QueueCount = _queueService.QueueCount;
            statisticInfo.InMemoryTaskCount = _queueService.TaskCount;
            statisticInfo.ActiveTaskCount = _queueService.ActiveTaskCount;
            statisticInfo.ConsumerCount = _consumerManager.GetActiveConsumersCount();   
            return statisticInfo;
        }
    }
}
