using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;

namespace PCITC.MES.MM.Tapper.Engine.SignalR
{
    [HubName("notifyHub")]
    public class NotificationHub : Hub, INotify
    {
        public NotificationHub()
        {
        }
        public override Task OnConnected()
        {
            Clients.Client(Context.ConnectionId).receiveMsg("Connect successful");
            return base.OnConnected();
        }

        private IDisposable _listener;
        private string Url { get; set; }

        public void Start()
        {
            try
            {
                var ip = string.Empty;
                foreach (var interfaces in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var address in interfaces.GetIPProperties().UnicastAddresses)
                    {
                        if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var add = address.Address.ToString();
                            if (add.StartsWith("127.") || add.StartsWith("169.")) continue;
                            ip = address.Address.ToString();
                            break;
                        }
                    }
                }
                Url = string.Format("http://{0}:{1}", ip, "7878");
                _listener = WebApp.Start<Startup>(Url);
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public void Stop()
        {
            if (_listener != null) _listener.Dispose();
        }

        public void Notify(string msg)
        {
            try
            {
                Clients.All.receiveMsg(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUrl()
        {
            return string.IsNullOrEmpty(Url) ? "Notify service not start yet..." : Url;
        }
    }
}
