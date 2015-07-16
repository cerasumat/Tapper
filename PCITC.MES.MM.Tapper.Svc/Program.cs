//#define LOCAL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
using PCITC.MES.MM.Tapper.Framework.Dapper;
using PCITC.MES.MM.Tapper.Framework.WcfParser;
using PCITC.MES.MM.Tapper.Framework.Configurations;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Serializing;

namespace PCITC.MES.MM.Tapper.Svc
{
    class Program
    {
        private static bool _started = false;
        private static Broker _broker;
        private static Producer _producer;
        private static List<Consumer> _consumers;
        private static IEnumerable<TopicModel> _topics;
        private static ApiSetting Setting;
        static void Main(string[] args)
        {
#if LOCAL
            Setting = new ApiSetting();
            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                connection.Open();
                _topics = connection.QueryList<TopicModel>(null, Setting.TopicModelTable, "*");
                connection.Close();
            }
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
            Uri urlBase = new UriBuilder("HTTP", ip, 7777).Uri;
            var config = new HttpSelfHostConfiguration(urlBase);

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Web api online...");
                Console.ReadLine();
            }
#endif

#if !LOCAL
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TapperSrv()
            };
            ServiceBase.Run(ServicesToRun);
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
