using CeraSumat.Utilities.Validation;

namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    public class TopicModel
    {
        [Required]
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        [Required]
        public string ServiceUrl { get; set; }
        [Required]
        public string ServiceBinding { get; set; }

        public override string ToString()
        {
            return string.Format("[TopicName={0}]", TopicName);
        }
    }
}
