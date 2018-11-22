using System;

namespace BizTalkAdminBot.Models
{
    public class Tracking
    {
        public bool ServiceStartEnd { get; set; }
        public bool MessageSendReceive { get; set; }
        public bool InboundMessageBody { get; set; }
        public bool OutboundMessageBody { get; set; }
        public bool OrchestartionEvents { get; set; }
        public bool TrackPropertiesForIncomingMessages { get; set; }
        public bool TrackPropertiesForOutgoingMessages { get; set; }
        public Body Body { get; set; }
        public Property Property { get; set; }
    }
}