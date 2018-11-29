using System;
namespace BizTalkAdminBot.Models
{
    public class Reports
    {
        private string _report;
        private ReportExtensions _extension;

        public ReportExtensions Extension
        {
            get
            {
                return _extension;
            }

            set
            {
                _extension = value;
            }
        }

        public Reports(string report, ReportExtensions extension)
        {
            _report = report;
            _extension = extension;
        }

        public enum ReportExtensions
        {
            htm,
            jpeg,
            png
        }



    }
}