using System;

namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    [Serializable]
    public class BrokerStatisticInfo
    {
        /// <summary>Topic个数
        /// </summary>
        public int TopicCount { get; set; }
        /// <summary>任务队列个数
        /// </summary>
        public int QueueCount { get; set; }
        /// <summary>内存中的总任务数
        /// </summary>
        public long InMemoryTaskCount { get; set; }
        /// <summary>活动的总任务数
        /// </summary>
        public long ActiveTaskCount { get; set; }
        /// <summary>Consumer个数
        /// </summary>
        public int ConsumerCount { get; set; }
    }
}
