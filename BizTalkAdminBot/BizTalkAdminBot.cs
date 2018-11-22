﻿#region  References
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using BizTalkAdminBot.Helpers;
using BizTalkAdminBot.Models;
#endregion

#region  namespace
namespace BizTalkAdminBot
{
    #region Class
    public class BizTalkAdminBot:IBot
    {
        private readonly BizTalkAdminBotAccessors _accessors;

        private readonly DialogSet _dialogs;

        //Constructor Through which the accessors get injected at the StartUp
        public BizTalkAdminBot(BizTalkAdminBotAccessors accessors)
        {
            //Connection Name is required to enable the Bot to connect to service providers through OAuth
            if(string.IsNullOrWhiteSpace(Constants.OAuthConnectionName))
            {
                throw new ArgumentNullException("Connection name needs to be set in the Constants class");
            }

                _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

                _dialogs = new DialogSet(_accessors.ConversationDialogState);
                _dialogs.Add(DialogHelpers.OAuthPrompt(Constants.OAuthConnectionName));
                _dialogs.Add(new WaterfallDialog(Constants.RootDialogName, new WaterfallStep[]{PromptStepAsync, ProcessStepAsync}));
            
        }

        #region  Methods
        /// <summary>
        /// Main method which is invoked on each conversation turn.
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken= default(CancellationToken))
        {
            DialogContext dc = null;
            
            //based upon the activity received, the decision needs to be made if a new dialog is created or an existing one is used.

            switch(turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await ProcessCommandsAsync(turnContext, cancellationToken);
                    break;
                
                case ActivityTypes.Event:
                    dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dc.ContinueDialogAsync(cancellationToken);
                    if(!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync(Constants.RootDialogName, cancellationToken);

                    }
                    break;
                
                case ActivityTypes.ConversationUpdate:
                    foreach(var member in turnContext.Activity.MembersAdded)
                    {
                        if(member.Id != turnContext.Activity.Recipient.Id)
                        {
                            string adaptiveCardPath = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.WelcomeMessage.ToString());
                            
                            var reply = turnContext.Activity.CreateReply();
                            string welcomeCardJson = GenericHelpers.ReadTextFromFile(@".\wwwroot\Resources\AdaptiveCards\WelcomeMessage.json");
                            reply.Attachments = new List<Attachment>()
                            {
                                DialogHelpers.CreateAdaptiveCardAttachment(welcomeCardJson)
                            };
                            await turnContext.SendActivityAsync(DialogHelpers.CreateReply(turnContext, adaptiveCardPath, true), cancellationToken);
                            
                        }
                    }
                    break;

            }
            
        }

        /// <summary>
        /// This method makes descision based upon the actual command received from the user.
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogContext> ProcessCommandsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            //Create the dialog context for the current turn
            var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            string adaptiveCardData;

            string command = turnContext.Activity.Text;
            Activity reply = null;

            // Based upon the parse command different logic will be called to construct the reply activities
            switch(command)
            {
                case "logoff":
                case "signoff":
                case "signout":
                case "logout":
                    var botAdapter = turnContext.Adapter as BotFrameworkAdapter;
                    
                    //Signout and cancel the token saved in the Azure Bot Service
                    await botAdapter.SignOutUserAsync(turnContext, Constants.OAuthConnectionName, cancellationToken:cancellationToken);

                    //Tell the user that they are signed out
                    adaptiveCardData = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.SignOutMessage.ToString());
                    await turnContext.SendActivityAsync(DialogHelpers.CreateReply(turnContext, adaptiveCardData, true), cancellationToken);
                
                    //end the dialog as the user is signed out. A new login will begin the new dialog.
                    await dc.EndDialogAsync(Constants.RootDialogName , cancellationToken);
                    await _accessors.ApplicationState.DeleteAsync(turnContext, cancellationToken: cancellationToken);
                    await _accessors.UserState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
                    break;

                case "help": 
                    
                    adaptiveCardData = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.HelpMessage.ToString());
                    await turnContext.SendActivityAsync(DialogHelpers.CreateReply(turnContext, adaptiveCardData, true), cancellationToken);
                    break;

                default:
                    await dc.ContinueDialogAsync(cancellationToken);
                    if(!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync(Constants.RootDialogName, cancellationToken: cancellationToken);
                    }
                    break;
            }
            return dc;

        }

        /// <summary>
        /// Method to check if the Login in needs to be performed.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            //Verify if the message in the current turn is not a magic code.
            if(stepContext.Context.Activity.Type == ActivityTypes.Message &&
                !Regex.IsMatch(stepContext.Context.Activity.Text, @"(\d{6})"))
                {
                    await _accessors.CommandState.SetAsync(stepContext.Context, stepContext.Context.Activity.Text, cancellationToken: cancellationToken);
                    await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
                }
                return await stepContext.BeginDialogAsync(Constants.LoginPromtName, cancellationToken:cancellationToken);
                
        }


        /// <summary>
        /// Method to process the text commands 
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //This works with only the ImBack type events sent by the emulator
            if(stepContext.Result != null)
            {
                var tokenResponse = stepContext.Result as TokenResponse;

                if(tokenResponse?.Token != null)
                {
                    var parts = _accessors.CommandState.GetAsync(stepContext.Context, () => string.Empty, cancellationToken: cancellationToken).Result.Split(' ');
                    string command = parts[0].ToLowerInvariant();

                    string adaptiveCardData;

                    switch(command)
                    {
                        case "getallapplications":
                            //BizTalkOperationApiHelper apiHelper = new BizTalkOperationApiHelper("getallapplications");

                            //List<Application> applications = await apiHelper.GetApplicationsAsync();
                            string sampleAppListJson = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetApplications.json");
                            List<Application> bizTalkApplications= JsonConvert.DeserializeObject<List<Application>>(sampleAppListJson);
                            
                            //Save the list of application object in the memory so as to see if the result was queried during the same session.
                            //This saves the communication with the Logic App thus saving the number of round trips
                            await _accessors.ApplicationState.SetAsync(stepContext.Context, bizTalkApplications, cancellationToken: cancellationToken);
                            await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);

                            adaptiveCardData = AdaptiveCardsHelper.CreateGetApplicationsAdaptiveCard(bizTalkApplications);

                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken: cancellationToken);
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, string.Format(Constants.AdaptiveCardPath, 
                            Constants.AdaptiveCards.OperationsMessage.ToString()) ,true), cancellationToken);
                            break;

                        case "getorchbyapp":

                            //Check the accessors to check if the ApplicationState contains a list of the application, if yes select it else, query the details from
                            // On premises BizTalk System. This is done to avoid fetching the applications multiple times.

                            //Uncomment This
                            //BizTalkOperationApiHelper apiHelper = new BizTalkOperationApiHelper("getallapplications");
                            //List<Application> applications = await apiHelper.GetApplicationsAsync();
                            var apps = await _accessors.ApplicationState.GetAsync(stepContext.Context, () => new List<Application>(), cancellationToken: cancellationToken);

                            //Uncomment This Later on 
                            //if(apps.Count == 0 || apps == null)
                            // {
                            //     //BizTalkOperationApiHelper apiHelper = new BizTalkOperationApiHelper("getallapplications");
                            //     //List<Application> applications = await apiHelper.GetApplicationsAsync();
                            // }

                            string appListJson = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetApplications.json");
                            
                            List<Application> bizTalkApps = JsonConvert.DeserializeObject<List<Application>>(appListJson);
                            
                            adaptiveCardData = AdaptiveCardsHelper.CreateSelectApplicationListAdaptiveCard(bizTalkApps);

                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken: cancellationToken);
                            break;

                        case "gethosts":
                            string hostList = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetHosts.json");
                            List<Host> hosts = JsonConvert.DeserializeObject<List<Host>>(hostList);
                            adaptiveCardData = AdaptiveCardsHelper.CreateGetHostsAdaptiveCard(hosts);

                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken);
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, string.Format(Constants.AdaptiveCardPath, 
                            Constants.AdaptiveCards.OperationsMessage.ToString()) ,true), cancellationToken);
                            break;

                        case "feedback":
                            adaptiveCardData = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.FeedBackCard.ToString());
                            
                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, true), cancellationToken: cancellationToken);
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, string.Format(Constants.AdaptiveCardPath, 
                            Constants.AdaptiveCards.OperationsMessage.ToString()) ,true), cancellationToken);
                            break;
                        
                        default:
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, string.Format(Constants.AdaptiveCardPath, 
                            Constants.AdaptiveCards.OperationsMessage.ToString()) ,true), cancellationToken);
                            break;
                    }
                }
            }
            else
            {
                var token = JToken.Parse(stepContext.Context.Activity.ChannelData.ToString());
                if(System.Convert.ToBoolean(token["postback"].Value<string>()))
                {
                    JToken commandToken = JToken.Parse(stepContext.Context.Activity.Value.ToString());
                    string command = GenericHelpers.ParseCommand(commandToken);
                    string adaptiveCardData;

                    switch(command)
                    {
                        case "getorchbyapp":
                            
                            string sampleOrchList = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetOrchestrations.json");
                            List<Orchestration> orchestrations= JsonConvert.DeserializeObject<List<Orchestration>>(sampleOrchList);

                            string getOrchJson = AdaptiveCardsHelper.CreateGetOrchestrationsAdaptiveCard(orchestrations, "ConfigureHTTPReceiveUsingBTDF");
                            adaptiveCardData = AdaptiveCardsHelper.CreateGetOrchestrationsAdaptiveCard(orchestrations, "ConfigureHTTPReceiveUsingBTDF");
                            
                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken);
                            break;

                        case "feedback":
                            await stepContext.Context.SendActivityAsync("Thank You for the feedback.");
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, string.Format(Constants.AdaptiveCardPath, 
                            Constants.AdaptiveCards.OperationsMessage.ToString()) ,true), cancellationToken);
                            break;

                        default:
                            await stepContext.Context.SendActivityAsync("Unable To Perform the task");
                            break;
                            
                    }
                    await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, string.Format(Constants.AdaptiveCardPath, 
                            Constants.AdaptiveCards.OperationsMessage.ToString()) ,true), cancellationToken);
                  

                }
                else
                {
                    await stepContext.Context.SendActivityAsync("We couldn't Sign you in. Please try again later");

                } 

            }
            await _accessors.CommandState.DeleteAsync(stepContext.Context, cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        #endregion    
    }
    #endregion
}
#endregion
