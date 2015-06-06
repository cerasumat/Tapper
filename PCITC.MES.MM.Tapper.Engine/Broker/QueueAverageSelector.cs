using System.Collections.Generic;
using System.Threading;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class QueueAverageSelector : IQueueSelector
    {
        private long _index;
        private long _topicIndex;
        private long _queueIndex;

        public int SelectQueueId(IList<int> availableQueueIds, TaskEntity task, string routingKey)
        {
            if (availableQueueIds.Count == 0)
            {
                return -1;
            }
            return availableQueueIds[(int)(Interlocked.Increment(ref _index) % availableQueueIds.Count)];
        }

        public string SelectTopic(IList<string> topics)
        {
            if (topics.Count == 0)
                return null;
            return topics[(int) (Interlocked.Increment(ref _topicIndex)%topics.Count)];
        }

        public int SelectQueueIdInTopic(IList<int> availableQueueIds)
        {
            if (availableQueueIds.Count == 0)
                return -1;
            return availableQueueIds[(int)(Interlocked.Increment(ref _queueIndex) % availableQueueIds.Count)];
        }
    }
}
