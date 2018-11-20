using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{
    public class Instance
    {
        public string Id { get; set; }
        public string Class { get; set; }
        public DateTime CreationTime { get; set; }
        public string ErrorDescription { get; set; }
        public string HostName { get; set; }
        public string InstanceStatus { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTypeID { get; set; }
        public string Application { get; set; }
        public string ErrorCode { get; set; }
        public string PendingOperation { get; set; }
        public string MessageBoxServer { get; set; }
        public string MessageBoxDb { get; set; }
        public string ProcessingServer { get; set; }
        public DateTime SuspendTime { get; set; }
        public string Adapter { get; set; }
        public string Uri { get; set; }
        public DateTime PendingJobSubmitTime { get; set; }
    }
}
