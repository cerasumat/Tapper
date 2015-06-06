namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class TaskQueueSettings
    {
        public int TaskMaxRetryTimes { get; set; }
        public int CheckFailedTaskInterval { get; set; }
        public int RemoveTaskFromQueueInterval { get; set; }

        public TaskQueueSettings()
        {
            TaskMaxRetryTimes = 3;
            CheckFailedTaskInterval = 1000 * 15;
            RemoveTaskFromQueueInterval = 1000 * 10;
        }
    }
}
