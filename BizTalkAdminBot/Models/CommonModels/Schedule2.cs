using System;

namespace BizTalkAdminBot.Models
{
    public class Schedule2
    {
        public bool ServiceWindowEnabled { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}