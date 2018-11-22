using System;

namespace BizTalkAdminBot.Models
{
    public class Property
    {
        public bool BeforeSendPipeline { get; set; }
        public bool AfterSendPipeline { get; set; }
        public bool BeforeReceivePipeline { get; set; }
        public bool AfterReceivePipeline { get; set; }
    }
}