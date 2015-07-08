using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.Owin.Cors;

namespace PCITC.MES.MM.Tapper.Engine.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HubConfiguration
            {
                EnableJSONP = true,         //for cross-domain access
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true
            };
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.MapSignalR(config);
        }
    }
}
