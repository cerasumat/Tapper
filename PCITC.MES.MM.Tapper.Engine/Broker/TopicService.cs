using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Dapper;
using PCITC.MES.MM.Tapper.Framework.Log4Net;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class TopicService : ITopicService
    {
        private readonly ConcurrentDictionary<string, TopicModel> _topicModelDict;
        private readonly ILogger _logger;
        public TopicServiceSettings Setting { get; private set; }

        public IList<string> AllTopics { get { return _topicModelDict.Keys.ToList(); } }

        public TopicService()
        {
            Setting = new TopicServiceSettings();
            _topicModelDict = new ConcurrentDictionary<string, TopicModel>();
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
        }

        public TopicModel GetTopicModelByName(string topicName)
        {
            TopicModel topic;
            _topicModelDict.TryGetValue(topicName, out topic);
            return topic;
        }

        public void Start()
        {
            _topicModelDict.Clear();
            _logger.Info("初始化Topic模型...");
            try
            {
                using (var connection = new OracleConnection(Setting.ConnectionStr))
                {
                    connection.Open();
                    var topics = connection.QueryList<TopicModel>(null, Setting.TopicModelTable);
                    if (topics == null)
                    {
                        _logger.Fatal("没有Topic配置，请检查系统配置.");
                        return;
                    }
                    foreach (var topicModel in topics)
                    {
                        if (!IsServiceUrlValidate(topicModel.ServiceUrl))
                        {
                            _logger.Error("Topic服务路径配置错误.Topic={0}",topicModel.ToString());
                            continue;
                        }
                        if (!_topicModelDict.TryAdd(topicModel.TopicName, topicModel))
                        {
                            _logger.Error("Topic加入列表失败.Topic={0}",topicModel.ToString());
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                _logger.Fatal("加载Topic失败，请检查系统配置.", exp);
            }
        }

        private static bool IsServiceUrlValidate(string serviceUrl)
        {
            var urlRegex = new Regex(@"^[a-zA-z]+://[^\s]*$");
            return urlRegex.IsMatch(serviceUrl) || serviceUrl.Contains("net.tcp://");
        }
    }
}
