
#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AdaptiveCards;
using BizTalkAdminBot.Models;
#endregion

namespace BizTalkAdminBot.Helpers
{
    
    /// <summary>
    /// Class that contains the helper methods used in constructing replies in a dialog
    /// </summary>
    public class DialogHelpers
    {
        /// <summary>
        /// Create the Adapative Card attachment for adaptive cards.
        /// </summary>
        /// <param name="adaptiveCardString">Json String defining the adaptive card</param>
        /// <returns>Attachment conating the adaptive card</returns>
        public static Attachment CreateAdaptiveCardAttachment(string adaptiveCardString)
        {
            Attachment adaptiveCard = new Attachment()
            {
                ContentType = Constants.AdaptiveCardContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCardString)
            };

            return adaptiveCard;
        }


        #region AdaptiveCards



        /// <summary>
        /// Create the Adaptive Card for the List of the Applications
        /// </summary>
        /// <param name="applicationList">list of the BizTalk Applications</param>
        /// <returns>Adaptive Card json string</returns>
        
        #endregion

    }
}
