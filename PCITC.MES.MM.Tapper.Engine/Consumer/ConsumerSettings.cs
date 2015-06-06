namespace PCITC.MES.MM.Tapper.Engine.Consumer
{
    public class ConsumerSettings
    {
        public int ConsumeThreadMaxCount { get; set; }
        public int UpdateTopicQueueCountInterval { get; set; }
        public int HeartbeatBrokerInterval { get; set; }
        public int PullThresholdForQueue { get; set; }
        public int PullTaskFromBrokerInterval { get; set; }
        public int PullMessageBatchSize { get; set; }
        public TaskHandleMode TaskHandleMode { get; set; }

        public ConsumerSettings()
        {
            ConsumeThreadMaxCount = 64;
            UpdateTopicQueueCountInterval = 1000 * 60;
            HeartbeatBrokerInterval = 1000 * 5;
            PullThresholdForQueue = 256;
            PullTaskFromBrokerInterval = 1000 * 10;
            PullMessageBatchSize = 16;
            TaskHandleMode = TaskHandleMode.Parallel;
        }
    }
}
