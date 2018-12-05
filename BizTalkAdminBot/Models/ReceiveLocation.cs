using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{
    public class ReceiveLocation
    {
        public string Name { get; set; }
        public object Description { get; set; }
        public string ReceivePortName { get; set; }
        public bool Enable { get; set; }
        public bool IsPrimary { get; set; }
        public string Address { get; set; }
        public string PublicAddress { get; set; }
        public string TransportType { get; set; }
        public string TransportTypeData { get; set; }
        public string ReceiveHandler { get; set; }
        public object CustomData { get; set; }
        public string ReceivePipeline { get; set; }
        public object ReceivePipelineData { get; set; }
        public string SendPipeline { get; set; }
        public object SendPipelineData { get; set; }
        public Schedule Schedule { get; set; }
        public object EncryptionCert { get; set; }
        public string FragmentMessages { get; set; }
    }
}
