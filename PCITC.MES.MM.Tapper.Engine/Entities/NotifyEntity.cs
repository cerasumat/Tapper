using System;
using CeraSumat.Utilities.Validation;

namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    public class NotifyEntity
    {
        //消息GUID
        public string NotifyCode { get; set; }
        public string NotifyLevel { get; set; }
        [Required]
        public string Topic { get; set; }
        [Required]
        public int TaskId { get; set; }
        public string TaskCode { get; set; }
        public DateTime NotifyTime { get; set; }
        public string NotifyContent { get; set; }
        //消息通知对象
        [Required]
        public string NotifyTarget { get; set; }
        //调试堆栈信息
        public string StackInfo { get; set; }
    }
}
