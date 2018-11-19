
#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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

    

        public static OAuthPrompt OAuthPrompt(string connectionName)
        {
            var oauthPrompt = new OAuthPrompt(
                Constants.LoginPromtName, new OAuthPromptSettings
                {
                    ConnectionName = connectionName ?? throw new ArgumentNullException("Connection Name cannot be blank."),
                    Text = "Please Sign In",
                    Timeout = 300000,
                    Title = "SignIn"    
                }

            );

            return oauthPrompt;
            
        }

        public static async Task<string> ParseCommand(Activity activity)
        {
            string command = string.Empty;

            if(activity.Text !=null)
            {
                command = activity.Text.ToLowerInvariant();
            }
            else
            {
                string compositeCommand = activity.Value.ToString();
                JToken token = JToken.Parse(compositeCommand);
                command = token["operationInputChoice"].Value<string>();
            }
            return command;
        }


    }
}
