﻿using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Consumer;
using PCITC.MES.MM.Tapper.Engine.Producer;
using PCITC.MES.MM.Tapper.Framework.Configurations;

namespace PCITC.MES.MM.Tapper.Engine.Configurations
{
    public static class ConfigurationExtensions
    {
        public static Configuration RegisterTapperComponents(this Configuration configuration)
        {
            configuration.SetDefault<ITopicService, TopicService>();
            configuration.SetDefault<ITaskQueueService, TaskQueueService>();
            configuration.SetDefault<IQueueSelector, QueueAverageSelector>();
            configuration.SetDefault<ITaskService, TaskService>();
            // -- 2015-6-21 Edit By JiaK--------------------
            // cancel injecting the TaskHandler object case the horinizing concurrent bug;
            //configuration.SetDefault<ITaskHandler, MvTaskHandler>();
            configuration.SetDefault<IModelService, ModelService>();
            configuration.SetDefault<IConsumerManager, ConsumerManager>();
            configuration.SetDefault<INotifyService, NotifyInOracleService>();
            return configuration;
        }
    }
}
