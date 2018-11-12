namespace BizTalkAdminBot.Models
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    #endregion

    /// <summary>
    /// Model which signifies a BizTalk application.
    /// </summary>
    public class BizTalkApplication
    {
        /// <summary>
        /// Name of the BizTalk application
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of BizTalk application
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is the Application Default BizTalk application
        /// </summary>
        public bool IsDefaultApplication { get; set; }

        /// <summary>
        /// Is the application system application
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Status of the application
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Is the application configured
        /// </summary>
        public bool IsConfigured { get; set; }

        /// <summary>
        /// List of the Application Referenced
        /// </summary>
        public List<object> ApplicationReferences { get; set; }

        /// <summary>
        /// List of the applications that reference this application
        /// </summary>
        public List<string> ApplicationBackReferences { get; set; }
    }

}
