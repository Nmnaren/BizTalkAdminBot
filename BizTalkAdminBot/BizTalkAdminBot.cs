#region  References
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
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
using Microsoft.Extensions.Configuration;
#endregion

#region  namespace
namespace BizTalkAdminBot
{
    #region Class
    public class BizTalkAdminBot:IBot
    {
        private readonly BizTalkAdminBotAccessors _accessors;

        private readonly IConfiguration _configuration;

        private readonly DialogSet _dialogs;

        private readonly string _storageAccountKey;
        

        //Constructor Through which the accessors get injected at the StartUp
        public BizTalkAdminBot(BizTalkAdminBotAccessors accessors, IConfiguration configuration)
        {
            //Connection Name is required to enable the Bot to connect to service providers through OAuth
            if(string.IsNullOrWhiteSpace(Constants.OAuthConnectionName))
            {
                throw new ArgumentNullException("Connection name needs to be set in the Constants class");
            }

                _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

                _configuration = configuration ?? throw new ArgumentException(nameof(configuration));

                _storageAccountKey = _configuration["storageAccount"].ToString();

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
            string adaptiveCardPath = string.Empty;
            
            //based upon the activity received, the decision needs to be made if a new dialog is created or an existing one is used.

            switch(turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await ProcessCommandsAsync(turnContext, cancellationToken);
                    break;
                
                case ActivityTypes.Event:

                    if(turnContext.Activity.Name == "setUserIdEvent")
                    {
                        adaptiveCardPath = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.WelcomeMessage.ToString());
                        await turnContext.SendActivityAsync(DialogHelpers.CreateReply(turnContext, adaptiveCardPath, true), cancellationToken);
                    }
                    else
                    {
                        dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                        await dc.ContinueDialogAsync(cancellationToken);
                        if(!turnContext.Responded)
                        {
                            await dc.BeginDialogAsync(Constants.RootDialogName, cancellationToken);

                        }

                    }
                    
                    break;
                
                case ActivityTypes.ContactRelationUpdate:
                    adaptiveCardPath = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.WelcomeMessage.ToString());
                    await turnContext.SendActivityAsync(DialogHelpers.CreateReply(turnContext, adaptiveCardPath, true), cancellationToken);
                    break;

                case ActivityTypes.ConversationUpdate:
                    foreach(var member in turnContext.Activity.MembersAdded)
                    {
                        if(member.Id != turnContext.Activity.Recipient.Id)
                        {
                            adaptiveCardPath = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.WelcomeMessage.ToString());
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
                    await _accessors.HostState.DeleteAsync(turnContext, cancellationToken);
                    await _accessors.OrchestrationState.DeleteAsync(turnContext, cancellationToken);
                    await _accessors.SendPortState.DeleteAsync(turnContext, cancellationToken);
                    await _accessors.FeedbackState.DeleteAsync(turnContext, cancellationToken);
                    

                    List<string> reports = await _accessors.Reports.GetAsync(turnContext, () => new List<string>(), cancellationToken);
                    
                    await _accessors.Reports.DeleteAsync(turnContext, cancellationToken: cancellationToken);
                    await _accessors.UserState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);

                    //If Get Suspended/Tracked Instances operation was run, delete the blobs in the storage account
                    if(reports.Count() >0)
                    {
                        BlobHelper blobHelper = new BlobHelper(_configuration);
                        
                        foreach(string report in reports)
                        {
                            await blobHelper.DeleteReportBlobAsync(report);
                        }

                    }
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
            //Check if the User has already provided the feedback, if yes, then do not show the FeedBack card again.
                var isFeedbackProvided = await _accessors.FeedbackState.GetAsync(stepContext.Context, ()=> false, cancellationToken);
            BizTalkOperationApiHelper apiHelper;
            List<Application> applications = new List<Application>();
            List<string> reports = new List<string>();
            BlobHelper blobHelper;

            //This works with only the ImBack type events sent by the emulator
            if (stepContext.Result != null)
            {
                var tokenResponse = stepContext.Result as TokenResponse;

                if(tokenResponse?.Token != null)
                {
                    var parts = _accessors.CommandState.GetAsync(stepContext.Context, () => string.Empty, cancellationToken: cancellationToken).Result.Split(' ');
                    string command = parts[0].ToLowerInvariant();

                    string adaptiveCardData;
                    string operationCard = string.Empty;

                    switch(command)
                    {
                        case "getallapplications":
                            apiHelper = new BizTalkOperationApiHelper("getallapplications");

                            applications = await apiHelper.GetApplicationsAsync();
                            //string sampleAppListJson = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetApplications.json");
                            //List<Application> bizTalkApplications= JsonConvert.DeserializeObject<List<Application>>(sampleAppListJson);
                            
                            //Save the list of application object in the memory so as to see if the result was queried during the same session.
                            //This saves the communication with the Logic App thus saving the number of round trips
                            await _accessors.ApplicationState.SetAsync(stepContext.Context, applications, cancellationToken: cancellationToken);
                            await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);

                            adaptiveCardData = AdaptiveCardsHelper.CreateGetApplicationsAdaptiveCard(applications);

                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken: cancellationToken);
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, 
                            string.Format(Constants.AdaptiveCardPath, (isFeedbackProvided ? Constants.AdaptiveCards.OperationMessageNoFB.ToString() : Constants.AdaptiveCards.OperationsMessage.ToString())) 
                             ,true), cancellationToken);
                            break;

                        case "getorchbyapp":
                        case "getsendportsbyapp":

                            //Check the accessors to check if the ApplicationState contains a list of the application, if yes select it else, query the details from
                            // On premises BizTalk System. This is done to avoid fetching the applications multiple times.

                            applications = await _accessors.ApplicationState.GetAsync(stepContext.Context, () => new List<Application>(), cancellationToken: cancellationToken);
                            
                            if(applications.Count == 0 || applications == null)
                            {
                                 apiHelper = new BizTalkOperationApiHelper("getallapplications");
                                 applications = await apiHelper.GetApplicationsAsync();
                                 //string appListJson = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetApplications.json");
                                 //apps = JsonConvert.DeserializeObject<List<Application>>(appListJson);

                             }
                            
                            adaptiveCardData = AdaptiveCardsHelper.CreateSelectApplicationListAdaptiveCard(applications);

                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken: cancellationToken);
                            break;

                        case "gethosts":
                            //Check if the Hosts were queried first. If the accessor contains the list then we do not call Logic App
                            //And we save the state. Else we get the host form the State.
                            
                            List<Host> hosts = await _accessors.HostState.GetAsync(stepContext.Context, ()=> new List<Host>(), cancellationToken);

                            if(hosts.Count ==0 || hosts == null)
                            {
                                string hostList = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetHosts.json");
                                hosts = JsonConvert.DeserializeObject<List<Host>>(hostList);
                                
                            }

                            if(hosts.Count == 0)
                            {
                                await stepContext.Context.SendActivityAsync(string.Format(Constants.NotFoundMessage, "gethosts"), cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await _accessors.HostState.SetAsync(stepContext.Context, hosts, cancellationToken);
                                await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
                                adaptiveCardData = AdaptiveCardsHelper.CreateGetHostsAdaptiveCard(hosts);

                                await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken);
                            
                            }
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, 
                            string.Format(Constants.AdaptiveCardPath, (isFeedbackProvided ? Constants.AdaptiveCards.OperationMessageNoFB.ToString() : Constants.AdaptiveCards.OperationsMessage.ToString())) 
                             ,true), cancellationToken);
                            break;

                        case "getsuspendedinstances":

                            apiHelper = new BizTalkOperationApiHelper("getsuspendedinstances");
                            List<Instance> instances = await apiHelper.GetInstancesAsync();

                            if(instances.Count > 0)
                            {
                                List<Instance> suspendedInstances = instances.Where(x => x.InstanceStatus ==Constants.InstanceStatus.Suspended.ToString()).ToList();

                                if(suspendedInstances.Count() > 0)
                                {
                                    string report = GenericHelpers.GetSuspendedInstancesReport(suspendedInstances);

                                    blobHelper = new BlobHelper(_configuration);
                                    string blobName = await blobHelper.UploadReportToBlobAsync(report, "SuspendedInstances");
                                    adaptiveCardData = AdaptiveCardsHelper.CreateGetSuspendedInstancesAdaptiveCard(suspendedInstances);
                                    adaptiveCardData = adaptiveCardData.Replace(Constants.BizManDummyUrl, string.Format(Constants.ReportBaseUrl, _storageAccountKey, blobName));
                                    await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken);

                                    reports = await _accessors.Reports.GetAsync(stepContext.Context, () => new List<string>(), cancellationToken);
                                    
                                    //If the reports does not contain the generated blob name, then add and save the list to the accessors.
                                    if(!reports.Contains(blobName))
                                    {
                                        reports.Add(blobName);
                                        await _accessors.Reports.SetAsync(stepContext.Context, reports, cancellationToken);
                                        await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
                                    }
                                    
                                }
                                else
                                {
                                    await stepContext.Context.SendActivityAsync("Sorry No Suspended Instances were found on the environment", cancellationToken : cancellationToken);
                                }
                                
                            }
                            else
                            {
                                await stepContext.Context.SendActivityAsync("Sorry No Instances were found on the environment", cancellationToken : cancellationToken);
                            }

                            break;


                        

                        case "feedback":
                            
                            adaptiveCardData = string.Format(Constants.AdaptiveCardPath, Constants.AdaptiveCards.FeedBackCard.ToString());

                            await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, true), cancellationToken: cancellationToken);
                            //await stepContext.Context.SendActivityAsync
                            // (DialogHelpers.CreateReply(stepContext.Context, 
                            // string.Format(Constants.AdaptiveCardPath, (isFeedbackProvided ? Constants.AdaptiveCards.OperationMessageNoFB.ToString() : Constants.AdaptiveCards.OperationsMessage.ToString())) 
                            //  ,true), cancellationToken);
                            break;
                        
                        default:
                            await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, 
                            string.Format(Constants.AdaptiveCardPath, (isFeedbackProvided ? Constants.AdaptiveCards.OperationMessageNoFB.ToString() : Constants.AdaptiveCards.OperationsMessage.ToString())) 
                             ,true), cancellationToken);
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
                    string command = GenericHelpers.ParseToken(commandToken, "command");
                    string adaptiveCardData = string.Empty;
                    string applicationName = string.Empty;


                    switch(command)
                    {
                        case "getorchbyapp":
                            // Check if the Orchestration list is available in the Irchestration State in Accessors.
                            //If the list is there, we will not call the Logic APp else we call the Logic APp and save the 
                            //Orchestration List in the Orchestration State.

                            List<Orchestration> orchestrations = await _accessors.OrchestrationState.GetAsync(stepContext.Context, () => new List<Orchestration>(), cancellationToken );

                            if(orchestrations.Count == 0 || orchestrations == null)
                            {
                                string sampleOrchList = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetOrchestrations.json");
                                orchestrations= JsonConvert.DeserializeObject<List<Orchestration>>(sampleOrchList);
                                

                            }

                            if(orchestrations.Count == 0)
                            {
                                await stepContext.Context.SendActivityAsync(string.Format(Constants.NotFoundMessage, "getorchbyapp"), cancellationToken: cancellationToken);

                            }
                            else
                            {
                                await _accessors.OrchestrationState.SetAsync(stepContext.Context, orchestrations, cancellationToken);
                                await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
                                applicationName = commandToken["applicationChoiceSet"].Value<string>();
                                adaptiveCardData = AdaptiveCardsHelper.CreateGetOrchestrationsAdaptiveCard(orchestrations.Where(x => x.ApplicationName == applicationName).ToList(), applicationName);
                            
                                await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken);

                            }

                            
                            break;

                        case "getsendportsbyapp":
                            //Check if the Send Ports are avaiable in the SendPOrtS STate using the accessors, if they are not, call the Logic APp to
                            //Query the result
                            List<SendPort> sendPorts = await _accessors.SendPortState.GetAsync(stepContext.Context, () => new List<SendPort>(), cancellationToken);

                            if(sendPorts.Count == 0 || sendPorts == null)
                            {
                                apiHelper = new BizTalkOperationApiHelper("getsendportsbyapp");
                                sendPorts = await apiHelper.GetSendPortsAsync();

                                //save the list into SendPort State using Accessors
                                
                            }

                            if(sendPorts.Count == 0)
                            {
                                await stepContext.Context.SendActivityAsync(string.Format(Constants.NotFoundMessage, "getsendportsbyapp"), cancellationToken: cancellationToken);

                            }
                            else
                            {
                                await _accessors.SendPortState.SetAsync(stepContext.Context, sendPorts, cancellationToken);
                                await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
                                applicationName = commandToken["applicationChoiceSet"].Value<string>();
                                adaptiveCardData = AdaptiveCardsHelper.CreateGetSendPortsByAppAdaptiveCard(sendPorts, applicationName);
                                await stepContext.Context.SendActivityAsync(DialogHelpers.CreateReply(stepContext.Context, adaptiveCardData, false), cancellationToken);

                            }
                            break;

                        case "feedback":
                            //Save the state that the user has provided the feedback. If Saved, this case will not executed again in the same session.
                            if(!isFeedbackProvided)
                            {
                                await _accessors.FeedbackState.SetAsync(stepContext.Context, true, cancellationToken);
                                await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
                                isFeedbackProvided = true;


                            }
                            await stepContext.Context.SendActivityAsync("Thank You for the feedback.");
                            break;

                        default:
                            await stepContext.Context.SendActivityAsync("Unable To Perform the task");
                            break;
                            
                    }
                   await stepContext.Context.SendActivityAsync
                            (DialogHelpers.CreateReply(stepContext.Context, 
                            string.Format(Constants.AdaptiveCardPath, (isFeedbackProvided ? Constants.AdaptiveCards.OperationMessageNoFB.ToString() : Constants.AdaptiveCards.OperationsMessage.ToString())) 
                             ,true), cancellationToken);
                  

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
