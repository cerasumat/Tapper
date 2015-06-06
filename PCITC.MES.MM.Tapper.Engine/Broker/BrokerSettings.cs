using System.Net;
using PCITC.MES.MM.Tapper.Framework.Utilities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class BrokerSettings
    {
        // ReSharper disable once InconsistentNaming
        public IPEndPoint ProducerIPEndPoint { get; set; }
        // ReSharper disable once InconsistentNaming
        public IPEndPoint ConsumerIPEndPoint { get; set; }
        // ReSharper disable once InconsistentNaming
        public IPEndPoint AdminIPEndPoint { get; set; }
        public bool NotifyWhenTaskArrived { get; set; }
        public int NotifyTaskArrivedThreadMaxCount { get; set; }
        public int ScanNotActiveConsumerInterval { get; set; }
        public int ConsumerExpiredTimeout { get; set; }
        public int QueueTaskMaxCachSize { get; set; }
        public int TopicMaxQueueCount { get; set; }

        public BrokerSettings()
        {
            ProducerIPEndPoint = new IPEndPoint(SocketUtils.GetLocalIPV4(), 7778);
            ConsumerIPEndPoint = new IPEndPoint(SocketUtils.GetLocalIPV4(), 7779);
            AdminIPEndPoint = new IPEndPoint(SocketUtils.GetLocalIPV4(), 7777);
            NotifyWhenTaskArrived = true;
            NotifyTaskArrivedThreadMaxCount = 32;
            ScanNotActiveConsumerInterval = 1000 * 10;
            ConsumerExpiredTimeout = 1000 * 60;
            QueueTaskMaxCachSize = 500;
            TopicMaxQueueCount = 64;
        }
    }
}
