using System;

namespace BizTalkAdminBot.Models
{
    public class Schedule
    {
        public DateTime StartDate { get; set; }
        public bool StartDateEnabled { get; set; }
        public DateTime EndDate { get; set; }
        public bool EndDateEnabled { get; set; }
        public bool ServiceWindowEnabled { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}