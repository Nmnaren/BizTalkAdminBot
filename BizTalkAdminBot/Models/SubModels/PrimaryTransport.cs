using System;

namespace BizTalkAdminBot.Models
{
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
}