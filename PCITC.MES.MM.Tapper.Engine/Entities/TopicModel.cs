namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    public class TopicModel
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public string ServiceUrl { get; set; }
        public string ServiceBinding { get; set; }

        public override string ToString()
        {
            return string.Format("[TopicName={0}]", TopicName);
        }
    }
}
