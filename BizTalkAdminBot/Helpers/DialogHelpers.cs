
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

        /// <summary>
        /// Method to Create the Sign In Card Which will be rendered to user if they are not signed in
        /// </summary>
        /// <param name="connectionName">Connection Name used in BOt Channels Registration</param>
        /// <returns>OAuthPrompt object which is rendered as a Sign In Card on Channel</returns>
        public static OAuthPrompt OAuthPrompt(string connectionName)
        {
            var oauthPrompt = new OAuthPrompt(
                Constants.LoginPromtName, new OAuthPromptSettings
                {

                    ConnectionName = connectionName ?? throw new ArgumentNullException("Connection Name cannot be blank."),
                    Text = "Please Sign In",
                    Timeout = 300000,
                    Title = "Sign In"   

                }

            );

            return oauthPrompt;
            
        }

      
        /// <summary>
        /// Parses the Value Token from Activty to decide upon the command.
        /// </summary>
        /// <param name="jToken">Value Token in the Activty</param>
        /// <returns>parse Command as string</returns>
        public static string ParseCommand(JToken jToken)
        {
            string command = string.Empty;

            if(jToken != null)
            {
                command = jToken["command"].Value<string>();

            }

            return command;
        }

        /// <summary>
        /// Method To Create Reply which sends out Operations
        /// </summary>
        /// <param name="stepContext">Waterfall step context</param>
        /// <returns>Actvity Object which contains operations adaptive card</returns>
        public static Activity CreateOperationsReply(WaterfallStepContext stepContext)
        {
            var reply = stepContext.Context.Activity.CreateReply();
            string operationMessageJson = GenericHelpers.ReadTextFromFile(@"wwwroot\Resources\AdaptiveCards\OperationsMessage.json");
            reply.Attachments = new List<Attachment>()
                {
                    //DialogHelpers.CreateBotFunctionsHeroCard("Your Options").ToAttachment()
                    DialogHelpers.CreateAdaptiveCardAttachment(operationMessageJson)
                };
            return reply;
        }


    }
}
