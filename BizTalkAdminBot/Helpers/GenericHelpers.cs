    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Text;
    using BizTalkAdminBot.Models;
    #endregion
namespace BizTalkAdminBot.Helpers
{


    /// <summary>
    /// Class used to house the helper methods which can be used across the code.
    /// </summary>
    public class GenericHelpers
    {

        /// <summary>
        /// Converts the Resource at the provided resource path to a base64String
        /// </summary>
        /// <param name="resourcePath">Path where the resource resides</param>
        /// <returns>The base64 encoded string for the resource</returns>
        public static string ConvertResourcesToBase64String(string resourcePath)
        {
            string base64Resource = string.Empty;

            byte[] base64Array = File.ReadAllBytes(resourcePath);

            base64Resource = System.Convert.ToBase64String(base64Array);

            return base64Resource;
        
        }

        /// <summary>
        /// Get the Text Stored in a File.
        /// </summary>
        /// <param name="resourcePath">Path where the File resides</param>
        /// <returns>String containing the text in File</returns>
        public static string ReadTextFromFile(string resourcePath)
        {
            return File.ReadAllText(resourcePath);
        }

        /// <summary>
        /// Parses the Value Token from Activty to get concerned Value.
        /// </summary>
        /// <param name="jToken">Value Token in the Activty</param>
        /// <param name="key">Key whose value is to be parsed</param>
        /// <returns>Parsed Value as string</returns>
        public static string ParseToken(JToken jToken, string key)
        {
            string value = string.Empty;

            if(jToken != null)
            {
                value = jToken[key].Value<string>();

            }

            return value;
        }

        /// <summary>
        /// Generate the Suspeneded Instances report.
        /// </summary>
        /// <param name="instances">List of the Suspended Service instances</param>
        /// <returns>Report HTML in string format</returns>
        public static string GetSuspendedInstancesReport(List<Instance> instances)
        {
            string skeletonReport = ReadTextFromFile(@".\wwwroot\Resources\Reports\SuspendedInstancesReport.htm");

            StringBuilder sb = new StringBuilder();

            foreach(Instance instance in instances)
            {
                sb.Append("<tr>");
                sb.AppendFormat("{0}{1}{2}", "<td style>", instance.ServiceType,"</td>");
                sb.AppendFormat("{0}{1}{2}", "<td style>", instance.Class,"</td>");
                sb.AppendFormat("{0}{1}{2}", "<td style>", instance.CreationTime.ToShortDateString(),"</td>");
                sb.AppendFormat("{0}{1}{2}", "<td width=70%>", instance.ErrorDescription,"</td>");
                sb.Append("</tr>");
            }

            return string.Format(skeletonReport, System.Convert.ToString(sb));
             
        }

        
    }
}
