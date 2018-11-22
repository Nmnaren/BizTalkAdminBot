using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{

    public class SendPort
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ApplicationName { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsTwoWay { get; set; }
        public string Status { get; set; }
        public string CustomData { get; set; }
        public PrimaryTransport PrimaryTransport { get; set; }
        public SecondaryTransport SecondaryTransport { get; set; }
        public string SendPipeline { get; set; }
        public string SendPipelineData { get; set; }
        public string ReceivePipeline { get; set; }
        public string ReceivePipelineData { get; set; }
        public List<string> InboundTransforms { get; set; }
        public List<string> OutboundTransforms { get; set; }
        public Tracking Tracking { get; set; }
        public EncryptionCert EncryptionCert { get; set; }
        public string Filter { get; set; }
        public int Priority { get; set; }
        public bool RouteFailedMessage { get; set; }
        public bool OrderedDelivery { get; set; }
        public bool StopSendingOnFailure { get; set; }
    }
}
