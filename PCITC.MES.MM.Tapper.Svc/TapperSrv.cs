using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Configurations;
using PCITC.MES.MM.Tapper.Engine.Consumer;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Engine.Producer;
using PCITC.MES.MM.Tapper.Engine.SignalR;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.WcfParser;
using PCITC.MES.MM.Tapper.Framework.Configurations;
using PCITC.MES.MM.Tapper.Framework.Dapper;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Serializing;

namespace PCITC.MES.MM.Tapper.Svc
{
    partial class TapperSrv : ServiceBase
    {
        private bool _started = false;
        private Broker _broker;
        private Producer _producer;
        private List<Consumer> _consumers;
        private IEnumerable<TopicModel> _topics;
        private ApiSetting Setting { get; }

        private HttpSelfHostServer _server;
        public TapperSrv()
        {
            InitializeComponent();
            Setting = new ApiSetting();
        }

        protected override void OnStart(string[] args)
        {
            GetTopics();
            StartService();
            _started = true;

            var urlBase = new UriBuilder("HTTP", GetLocalIp(), 7777).Uri;
            var config = new HttpSelfHostConfiguration(urlBase);
            config.Routes.MapHttpRoute(
                "TapperApi", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            //---------------------------------------------------------
            //--不能用using,因为这个线程不会阻塞,using将释放_server资源
            //---------------------------------------------------------
            //using (_server = new HttpSelfHostServer(config))
            //{
            //    _server.OpenAsync().Wait();
            //}
            _server = new HttpSelfHostServer(config);
            _server.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
            if (_consumers.Count > 0)
            {
                foreach (var consumer in _consumers)
                {
                    consumer.Shutdown();
                }
            }
            _producer.Shutdown();
            _broker.Shutdown();
            _started = false;

            if (_server != null)
            {
                _server.CloseAsync().Wait();
                _server.Dispose();
            }

            //_server?.CloseAsync().Wait();
            //_server?.Dispose();
        }

        private static string GetLocalIp()
        {
            var ip = string.Empty;
            foreach (var interfaces in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var address in interfaces.GetIPProperties().UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var add = address.Address.ToString();
                        if (add.StartsWith("127") || add.StartsWith("169")) continue;
                        ip = address.Address.ToString();
                        break;
                    }
                }
            }
            return ip;
        }

        private void GetTopics()
        {
            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                connection.Open();
                _topics = connection.QueryList<TopicModel>(null, Setting.TopicModelTable, "*");
                connection.Close();
            }
        }

        private void StartService()
        {
            if (_started) return;
            WcfChannelFactory.CloseChannelFactory();
            if (_broker == null)
            {
                InitializeTapper();
                _broker = Broker.Create().Start();
            }
            else
                _broker.Start();
            if (_producer == null)
                _producer = new Producer("P1").Start();
            else
                _producer.Start();
#if !DEBUG
            if (_consumers != null && _consumers.Count > 0)
            {
                foreach (var consumer in _consumers)
                {
                    consumer.Start();
                }
            }
            else
            {
                _consumers = new List<Consumer>();
                if (_topics.Any())
                {
                    var count = 1;
                    foreach (var topic in _topics)
                    {
                        _consumers.Add(new Consumer("C" + count.ToString("N")).Subscribe(topic.TopicName).Start());
                        count++;
                    }
                }
            }
#endif
        }

        private static void InitializeTapper()
        {
            Configuration.Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterTapperComponents()
                .RegisterNotification();
        }
    }
}
