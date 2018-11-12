namespace BizTalkAdminBot.Helpers
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    #endregion

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
    }
}
