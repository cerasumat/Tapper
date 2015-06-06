namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class ConsumerManagerSettings
    {
        public int ConsumerActiveTimeout { get; set; }
        public int ScanConsumerInterval { get; set; }

        public ConsumerManagerSettings()
        {
            ConsumerActiveTimeout = 30;     //单位s
            ScanConsumerInterval = 1000 * 5;      //means that 6 times inactive the consumer
        }
    }
}
