using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{
 
    public class InboundPort
    {
        public string Name { get; set; }
        public string Binding { get; set; }
        public string ReceivePort { get; set; }
        public string PortType { get; set; }
    }

    public class OrchestrationTracking
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

    public class Orchestration
    {
        public string FullName { get; set; }
        public string AssemblyName { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Host { get; set; }
        public List<InboundPort> InboundPorts { get; set; }
        public List<object> OutboundPorts { get; set; }
        public List<object> UsedRoles { get; set; }
        public List<object> ImplementedRoles { get; set; }
        public List<object> InvokedOrchestrations { get; set; }
        public OrchestrationTracking Tracking { get; set; }
    }
}
