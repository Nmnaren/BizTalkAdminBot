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

        /// <summary>
        /// Only Available for Send Ports.Do Not Use With Orchestrations
        /// </summary>
        /// <value></value>
        public Body Body { get; set; }
        
        /// <summary>
        /// Only Available for Send Ports.Do Not Use With Orchestrations
        /// </summary>
        /// <value></value>
        public Property Property { get; set; }
    }
}