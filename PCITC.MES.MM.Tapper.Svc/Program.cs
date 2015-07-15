#define LOCAL
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

namespace PCITC.MES.MM.Tapper.Svc
{
    class Program
    {
        static void Main(string[] args)
        {
#if LOCAL
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
    }
}
