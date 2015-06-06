using System.Collections.Generic;

namespace PCITC.MES.MM.Tapper.Engine.Consumer
{
    public interface ITaskHandler
    {
        string ServiceUrl { get; set; }
        string ServiceBinding { get; set; }
        dynamic TaskExcute(string topic, string methodName, Dictionary<string, string> methodParams);
    }
}
