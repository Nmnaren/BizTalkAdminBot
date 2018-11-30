using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{
    public class Host
    {
        /// <summary>
        /// Name of the Host
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// If the Host is the Default host 
        /// </summary>
        /// <value></value>
        public bool IsDefault { get; set; }
        
        /// <summary>
        /// BizTalk group name
        /// </summary>
        /// <value></value>
        public string NTGroupName { get; set; }
        
        /// <summary>
        /// Type of the host (InProcess or Isolated)
        /// </summary>
        /// <value></value>
        public string Type { get; set; }
        
        /// <summary>
        /// Is the Host a trusted host
        /// </summary>
        /// <value></value>
        public bool IsTrusted { get; set; }
    }
}
