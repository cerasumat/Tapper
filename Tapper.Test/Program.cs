using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security;
using System.ServiceModel;
using System.Linq;
using System.Text.RegularExpressions;
using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Configurations;
using PCITC.MES.MM.Tapper.Engine.Consumer;
using PCITC.MES.MM.Tapper.Engine.Producer;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Configurations;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Serializing;
using PCITC.MES.MM.Tapper.Framework.WcfParser;

namespace Tapper.Test
{
    public class TestClass
    {
        public void SayHello(string hello)
        {
            Console.WriteLine("you said:{0}", hello);
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }

        public int Sub(int x, int y)
        {
            return x - y;
        }

        public int AddInt(int x, int y)
        {
            return x + y;
        }

        public void Test()
        {
            MethodInfo methodInfo = typeof(TestClass).GetMethod("AddInt");
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                paramTypes[i] = ps[i].ParameterType;
            }
            //var del = Delegate.CreateDelegate(typeof(Func<int, int, int>), methodInfo);
            var del=Delegate.CreateDelegate(typeof (Func<TestClass, int, int, int>), methodInfo);
            var result1 = del.DynamicInvoke(this,1, 2);
            //var dele = DelegateFactory.CreateDelegate<Func<MessageUtils,int, int, int>>(methodInfo, paramTypes);
            //var result = dele.DynamicInvoke(new object[] { util, 1, 2});
        }

        public static ulong Hash33(string inStr)
        {
            ulong hash = 0;
            for (var i = 0; i < inStr.Length; i++)
            {
                hash += hash*33 + inStr[i];
            }
            return hash;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var hash = TestClass.Hash33("53aa6f1a8c1549e3b383adc09a89384c");
            hash= TestClass.Hash33("53aa6f1a8c1549e3b383adc09a89384a");
            /*
            //IAuthorizeService dpser = WcfChannelFactory.CreateWcfServiceByUrl<IAuthorizeService>("http://10.238.225.201:8080/AppService/AuthorizeService.svc", "wshttpbinding");
            var method=typeof (IAuthorizeService).GetMethod("ListPropertyValueByOperationCode");
            //var method = srvType.GetMethod("ListPropertyValueByOperationCode");
            var paramInfos = method.GetParameters();

            var paramStr = "{loginName:'jcc',authClassCode:'CIOFP',operationCode:'C0001',propertyCode:''}";
            var serializer=new NewtonsoftJsonSerializer();
            var param=serializer.Deserialize<Dictionary<string, string>>(paramStr);
            var psArray = paramInfos.Select(p => p.ParameterType.CreateInstance(p.Name, param)).ToArray();

            var results=WcfChannelFactory.ExecuteMetod<IAuthorizeService>("http://10.238.225.201:8080/AppService/AuthorizeService.svc", "wshttpbinding",
                "ListPropertyValueByOperationCode", psArray);
                */

            //var result=typeof (IAuthorizeService).InvokeMember("ListPropertyValueByOperationCode",
            //BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, dpser, psArray);
            //var permissions=dpser.ListPropertyValueByOperationCode("jcc", "CIOFP","C0001",null);

            //var loggerFactory = new Log4NetLoggerFactory("Log4Net.config");
            //var pLogger = loggerFactory.Create("ProducerLogger");
            //var cLogger = loggerFactory.Create("ConsumerLogger");
            //var bLogger = loggerFactory.Create("BrokerLogger");
            //pLogger.Debug("Producer events{0}.", "01");
            //cLogger.Info("Consumer events.");
            //bLogger.Error("Broker events.");
            //Console.ReadLine();

            InitializeTapper();
            var broker = Broker.Create().Start();
            var staticInfos = broker.GetBrokerStatisticInfo();
            Console.WriteLine("Topic count: {0}.", staticInfos.TopicCount);
            Console.WriteLine("Task queue count: {0}.", staticInfos.QueueCount);
            Console.WriteLine("All task count: {0}.", staticInfos.InMemoryTaskCount);
            Console.WriteLine("Active task count: {0}.", staticInfos.ActiveTaskCount);
            Console.WriteLine("Active consumer count: {0}.", staticInfos.ConsumerCount);
            var producer = new Producer("P1").Start();
            var consumer1 = new Consumer("C1");
            var consumer2 = new Consumer("C2");
            var consumer3 = new Consumer("C3");
            //consumer.Subscribe("TM").Subscribe("UR").Subscribe("PB").Start();
            consumer1.Subscribe("TM").Start();
            consumer2.Subscribe("UR").Start();
            consumer3.Subscribe("PB").Start();
            while (Console.ReadLine() != "q")
            {
                staticInfos = broker.GetBrokerStatisticInfo();
                Console.WriteLine("Topic count: {0}.", staticInfos.TopicCount);
                Console.WriteLine("Task queue count: {0}.", staticInfos.QueueCount);
                Console.WriteLine("All task count: {0}.", staticInfos.InMemoryTaskCount);
                Console.WriteLine("Active task count: {0}.", staticInfos.ActiveTaskCount);
                Console.WriteLine("Active consumer count: {0}.", staticInfos.ConsumerCount);
            }
        }

        static void InitializeTapper()
        {
            Configuration.Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterTapperComponents();
        }
    }
}
