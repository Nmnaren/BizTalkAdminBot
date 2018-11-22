using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{
 
    
    

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
        public Tracking Tracking { get; set; }
    }
}
