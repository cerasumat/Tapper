using System.Collections.Generic;
using System.Linq;
using PCITC.MES.MM.Tapper.Framework.WcfParser;

namespace PCITC.MES.MM.Tapper.Engine.Consumer
{
    public class MvTaskHandler : ITaskHandler
    {
        public string ServiceUrl { get; set; }          //service contains all the mv automation tasks
        public string ServiceBinding { get; set; }

        public MvTaskHandler()
        {
        }
        public MvTaskHandler(string serviceUrl, string serviceBinding)
        {
            ServiceUrl = serviceUrl;
            ServiceBinding = serviceBinding;
        }

        public dynamic TaskExcute(string topic, string methodName, Dictionary<string, string> methodParams)
        {
            var paramArray = GetParametersArray(methodName, methodParams);
            switch (topic)
            {
                case "UR":
                    dynamic result = WcfChannelFactory.ExecuteMetod<IUrService4Tapper>("ur", ServiceUrl, ServiceBinding, methodName, paramArray);
                    return result;
                default:
                    return false;
            }
        }

        private static object[] GetParametersArray(string methodName,Dictionary<string, string> methodParams)
        {
            var method = typeof(IUrService4Tapper).GetMethod(methodName);
            var paramInfos = method.GetParameters();
            return paramInfos.Select(p => p.ParameterType.CreateInstance(p.Name, methodParams)).ToArray();
        }
    }
}
