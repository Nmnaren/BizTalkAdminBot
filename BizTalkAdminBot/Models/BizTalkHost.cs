using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Models
{
    public class BizTalkHost
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public string NTGroupName { get; set; }
        public string Type { get; set; }
        public bool IsTrusted { get; set; }
    }
}
