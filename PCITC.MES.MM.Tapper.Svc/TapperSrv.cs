using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace PCITC.MES.MM.Tapper.Svc
{
    partial class TapperSrv : ServiceBase
    {
        private HttpSelfHostServer _server;
        public TapperSrv()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
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
            var urlBase = new UriBuilder("HTTP", ip, 7777).Uri;
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
            if (_server != null)
            {
                _server.CloseAsync().Wait();
                _server.Dispose();
            }

            //_server?.CloseAsync().Wait();
            //_server?.Dispose();
        }
    }
}
