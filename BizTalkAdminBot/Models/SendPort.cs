using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{

    public class Schedule
    {
        public bool ServiceWindowEnabled { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }

    public class PrimaryTransport
    {
        public string Address { get; set; }
        public string TransportType { get; set; }
        public string TransportTypeData { get; set; }
        public string SendHandler { get; set; }
        public int RetryCount { get; set; }
        public int RetryInterval { get; set; }
        public bool OrderedDelivery { get; set; }
        public Schedule Schedule { get; set; }
    }

    public class Schedule2
    {
        public bool ServiceWindowEnabled { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }

    public class SecondaryTransport
    {
        public string Address { get; set; }
        public string TransportType { get; set; }
        public string TransportTypeData { get; set; }
        public string SendHandler { get; set; }
        public int RetryCount { get; set; }
        public int RetryInterval { get; set; }
        public bool OrderedDelivery { get; set; }
        public Schedule2 Schedule { get; set; }
    }

    public class Body
    {
        public bool BeforeSendPipeline { get; set; }
        public bool AfterSendPipeline { get; set; }
        public bool BeforeReceivePipeline { get; set; }
        public bool AfterReceivePipeline { get; set; }
    }

    public class Property
    {
        public bool BeforeSendPipeline { get; set; }
        public bool AfterSendPipeline { get; set; }
        public bool BeforeReceivePipeline { get; set; }
        public bool AfterReceivePipeline { get; set; }
    }

    //private class Tracking
    //{
    //    public Body Body { get; set; }
    //    public Property Property { get; set; }
    //}

    public class EncryptionCert
    {
        public string CommonName { get; set; }
        public string Thumbprint { get; set; }
    }

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
        //public Tracking Tracking { get; set; }
        public EncryptionCert EncryptionCert { get; set; }
        public string Filter { get; set; }
        public int Priority { get; set; }
        public bool RouteFailedMessage { get; set; }
        public bool OrderedDelivery { get; set; }
        public bool StopSendingOnFailure { get; set; }
    }
}
