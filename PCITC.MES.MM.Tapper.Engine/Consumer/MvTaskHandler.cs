using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            switch (topic.ToUpper())
            {
                case "UR":
                {
                    var mi = typeof (ITapperSrv).GetMethod(methodName);
                    var paramArray = GetParametersArray(mi, methodParams);
#if DEBUG
                    try
                    {
#endif
                        //Trace.WriteLine(string.Format("Task invoke:{0}",methodName));
                        dynamic result = WcfChannelFactory.ExecuteMetod<ITapperSrv>("ur", ServiceUrl, ServiceBinding,
                            methodName, paramArray);
                        //Trace.WriteLine(result.ToString());
                        return result;
#if DEBUG
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
#endif
                    }
                    break;
                case "CU":
                {
                    var mi = typeof (ICuOperateSrv).GetMethod(methodName);
                    var paramArray = GetParametersArray(mi, methodParams);
                    try
                    {
                        dynamic result = WcfChannelFactory.ExecuteMetod<ICuOperateSrv>("cu", ServiceUrl, ServiceBinding,
                            methodName, paramArray);
                        return result;
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
                    break;
                case "TM":
                {
                    var mi = typeof (ITankAutoSvc).GetMethod(methodName);
                    var paramArray = GetParametersArray(mi, methodParams);
                    dynamic result = WcfChannelFactory.ExecuteMetod<ITankAutoSvc>("tm", ServiceUrl, ServiceBinding,
                        methodName, paramArray);
                    return result;
                }
                    break;
                case "PB":
                {
                    var mi = typeof (IParsService).GetMethod(methodName);
                    var paramArray = GetParametersArray(mi, methodParams);
                    dynamic result = WcfChannelFactory.ExecuteMetod<IParsService>("pb", ServiceUrl,
                        ServiceBinding,
                        methodName, paramArray);
                    return result;
                }
                    break;
                default:
                    return false;
            }
        }

        private static object[] GetParametersArray(MethodInfo mi,Dictionary<string, string> methodParams)
        {
            var paramInfos = mi.GetParameters();
            return paramInfos.Select(p => p.ParameterType.CreateInstance(p.Name, methodParams)).ToArray();
        }
    }
}
