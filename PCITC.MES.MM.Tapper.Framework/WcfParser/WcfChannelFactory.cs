using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PCITC.MES.MM.Tapper.Framework.WcfParser
{
    public class WcfChannelFactory
    {
        private static ConcurrentDictionary<string, ChannelFactory> _channelFactories=new ConcurrentDictionary<string, ChannelFactory>();

        private static ChannelFactory<T> GetFactory<T>(string factoryName, string uri, string bindingName)
        {
            lock (_channelFactories)
            {
                ChannelFactory factory;
                _channelFactories.TryGetValue(factoryName, out factory);
                if (factory == null)
                {
                    var binding = CreateBinding(GetBindingType(bindingName));
                    var endpoint = new EndpointAddress(uri);
                    var newFactory = new ChannelFactory<T>(binding, endpoint);
                    if (_channelFactories.TryAdd(factoryName, newFactory))
                        return newFactory;
                }
                else if (factory.State == CommunicationState.Closed || factory.State == CommunicationState.Faulted)
                {
                    ChannelFactory removeFacotry;
                    _channelFactories.TryRemove(factoryName, out removeFacotry);
                    var binding = CreateBinding(GetBindingType(bindingName));
                    var endpoint = new EndpointAddress(uri);
                    var newFactory = new ChannelFactory<T>(binding, endpoint);
                    if (_channelFactories.TryAdd(factoryName, newFactory))
                        return newFactory;
                }
                return factory as ChannelFactory<T>;
            }
        }

        public static dynamic ExecuteMetod<T>(string factoryName,string uri, string bindingName, string methodName, params object[] args)
        {
            var channelFactory = GetFactory<T>(factoryName, uri, bindingName);
            var proxy = (channelFactory.CreateChannel() as ICommunicationObject);
            //-----------------------------------------------------------------------
            //-- Open the proxy explict make sure the paralle execution on the service side
            if (proxy == null) return false;
            proxy.Open();
            //-----------------------------------------------------------------------
            dynamic result;
            using (proxy as IDisposable)
            {
                if (proxy == null) return false;
                try
                {
                    var type = typeof(T);
                    var mi = type.GetMethod(methodName);
                    result = mi.Invoke(proxy, args);
                }
                catch (TimeoutException)
                {
                    (proxy as ICommunicationObject)?.Abort();
                    throw;
                }
                catch (CommunicationException)
                {
                    (proxy as ICommunicationObject)?.Abort();
                    throw;
                }
                catch (Exception)
                {
                    (proxy as ICommunicationObject)?.Abort();
                    throw;
                }
                proxy.Close();
            }
            return result;
        }

        //public static object ExecuteMetod<T>(string uri,string bindingName, string methodName, params object[] args)
        //{
        //    var binding = CreateBinding(GetBindingType(bindingName));
        //    var endpoint = new EndpointAddress(uri);
        //    using (var channelFactory = new ChannelFactory<T>(binding, endpoint))
        //    {
        //        var instance = channelFactory.CreateChannel();
        //        using (instance as IDisposable)
        //        {
        //            try
        //            {
        //                var type = typeof (T);
        //                var mi = type.GetMethod(methodName);
        //                return mi.Invoke(instance, args);
        //            }
        //            catch (TimeoutException)
        //            {
        //                (instance as ICommunicationObject)?.Abort();
        //                throw;
        //            }
        //            catch (CommunicationException)
        //            {
        //                (instance as ICommunicationObject)?.Abort();
        //                throw;
        //            }
        //            catch (Exception)
        //            {
        //                (instance as ICommunicationObject)?.Abort();
        //                throw;
        //            }
        //        }
        //    }
        //}

        public static T CreateWcfServiceByUrl<T>(string url)
        {
            return CreateWcfServiceByUrl<T>(url, "wsHttpBinding");
        }

        public static T CreateWcfServiceByUrl<T>(string url, string bing)
        {
            if (string.IsNullOrEmpty(url)) throw new NotSupportedException("this url isn`t Null or Empty!");
            EndpointAddress address = new EndpointAddress(url);
            Binding binding = CreateBinding(GetBindingType(bing));
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            return factory.CreateChannel();
        }

        private static Binding CreateBinding(BindingType binding)
        {
            Binding bindinginstance = null;
            switch (binding)
            {
                case BindingType.BasicHttpBinding:
                {
                    var ws = new BasicHttpBinding
                    {
                        MaxBufferSize = 2147483647,
                        MaxBufferPoolSize = 2147483647,
                        MaxReceivedMessageSize = 2147483647,
                        ReaderQuotas = {MaxStringContentLength = 2147483647},
                        CloseTimeout = new TimeSpan(0, 10, 0),
                        OpenTimeout = new TimeSpan(0, 10, 0),
                        ReceiveTimeout = new TimeSpan(0, 10, 0),
                        SendTimeout = new TimeSpan(0, 10, 0)
                    };
                    bindinginstance = ws;
                }
                    break;
                case BindingType.NetNamedPipeBinding:
                {
                    var ws = new NetNamedPipeBinding
                    {
                        MaxReceivedMessageSize = 65535000
                    };
                    bindinginstance = ws;
                }
                    break;
                case BindingType.NetPeerTcpBinding:
                {
                    var ws = new NetPeerTcpBinding
                    {
                        MaxReceivedMessageSize = 65535000
                    };
                    bindinginstance = ws;
                }
                    break;
                case BindingType.NetTcpBinding:
                {
                    var ws = new NetTcpBinding
                    {
                        MaxReceivedMessageSize = 65535000,
                        Security = {Mode = SecurityMode.None}
                    };
                    bindinginstance = ws;
                }
                    break;
                case BindingType.WsDualHttpBinding:
                {
                    var ws = new WSDualHttpBinding {MaxReceivedMessageSize = 65535000};
                    bindinginstance = ws;
                }
                    break;
                case BindingType.WebHttpBinding:
                    //var ws = new WebHttpBinding();
                    //ws.MaxReceivedMessageSize = 65535000;
                    //bindinginstance = ws;
                    break;
                case BindingType.WsFederationHttpBinding:
                {
                    var ws = new WSFederationHttpBinding {MaxReceivedMessageSize = 65535000};
                    bindinginstance = ws;
                }
                    break;
                case BindingType.WsHttpBinding:
                {
                    var ws = new WSHttpBinding(SecurityMode.None)
                    {
                        MaxBufferPoolSize = 2147483647,
                        MaxReceivedMessageSize = 2147483647
                    };
                    ws.Security.Message.ClientCredentialType = MessageCredentialType.None;
                    ws.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                    ws.ReaderQuotas.MaxDepth = 2147483647;
                    ws.ReaderQuotas.MaxStringContentLength = 2147483647;
                    ws.ReaderQuotas.MaxArrayLength = 2147483647;
                    ws.ReaderQuotas.MaxBytesPerRead = 2147483647;
                    ws.ReaderQuotas.MaxNameTableCharCount = 2147483647;
                    bindinginstance = ws;
                }
                    break;
            }
            return bindinginstance;
        }

        private static BindingType GetBindingType(string bindingStr)
        {
            BindingType type;
            switch (bindingStr.ToLower())
            {   
                case "basichttpbinding":
                  type=BindingType.BasicHttpBinding;
                    break;
                case "netnamedpipebinding":
                    type=BindingType.NetNamedPipeBinding;
                    break;
                case "netpeertcpbinding":
                    type=BindingType.NetPeerTcpBinding;
                    break;
                case "nettcpbinding":
                    type = BindingType.NetTcpBinding;
                    break;
                case "wsdualhttpbinding":
                    type=BindingType.WsDualHttpBinding;
                    break;
                case "wsfederationhttpbinding":
                    type=BindingType.WsFederationHttpBinding;
                    break;
                case "wshttpbinding":
                    type=BindingType.WsHttpBinding;
                    break;
                case "webhttpbinding":
                    type = BindingType.WebHttpBinding;
                    break;
                default:
                    type=BindingType.BasicHttpBinding;
                    break;
            }
            return type;
        }
    }
}
