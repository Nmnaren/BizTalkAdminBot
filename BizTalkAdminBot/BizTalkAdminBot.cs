// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BizTalkAdminBot.Helpers;
using BizTalkAdminBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace BizTalkAdminBot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class BizTalkAdminBot : IBot
    {
        private readonly BizTalkAdminBotAccessors _accessors;
        private readonly ILogger _logger;

        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BizTalkAdminBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public BizTalkAdminBot(BizTalkAdminBotAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            if(Constants.OAuthConnectionName == null)
            {
                throw new System.ArgumentNullException("Connection Name");
            }

            _logger = loggerFactory.CreateLogger<BizTalkAdminBot>();
            _logger.LogTrace("BizTalkAdminBot Conctructor Invoked");
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            _dialogs = new DialogSet(_accessors.ConversationDialogState);
            _dialogs.Add(DialogHelpers.OAuthPrompt(Constants.OAuthConnectionName));
            _dialogs.Add(new WaterfallDialog(Constants.RootDialogName, new WaterfallStep[] {PromptStepAsync, ProcessStepAsync}));
            
        }

        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dialogContext = null;

            switch(turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await ProcessInputCommandsAsync(turnContext, cancellationToken);
                    break;
                
                case ActivityTypes.Event:
                case ActivityTypes.Invoke:

                    // Check if the Invoke activity type is emitted by any channel other than msteam
                    if (turnContext.Activity.Type == ActivityTypes.Invoke && turnContext.Activity.ChannelId != "msteams")
                    {
                        throw new System.InvalidOperationException("The Invoke type is only valid onthe MSTeams channel.");
                    }
                    dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dialogContext.ContinueDialogAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dialogContext.BeginDialogAsync(Constants.RootDialogName, cancellationToken: cancellationToken);
                    }
                    break;

                case ActivityTypes.ConversationUpdate:
                    foreach(var member in turnContext.Activity.MembersAdded)
                    {
                        if(member.Id != turnContext.Activity.Recipient.Id)
                        {
                            var reply = turnContext.Activity.CreateReply();
                            string welcomeAdaptiveCardJson = GenericHelpers.ReadTextFromFile(@"./wwwroot/Resources/AdaptiveCards/WelcomeMessage.json");
                            reply.Attachments = new List<Attachment>()
                            {
                                DialogHelpers.CreateAdaptiveCardAttachment(welcomeAdaptiveCardJson)

                            };
                            await turnContext.SendActivityAsync(reply, cancellationToken);
                        }
                    }
                    break;  
            }
        }

        #region DialogTasks

        public async Task<DialogContext> ProcessInputCommandsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            DialogContext dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            
            var activity = dc.Context.Activity;

            string command = await DialogHelpers.ParseCommand(activity);

            switch(command)
            {
                case "signout":
                case "signoff":
                case "logoff":
                case "logout":

                    var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                    await botAdapter.SignOutUserAsync(turnContext, connectionName:Constants.OAuthConnectionName, cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync("You are now signed out. \n Please close the chat window or type in anything to begin again", cancellationToken: cancellationToken);
                    await dc.EndDialogAsync(Constants.RootDialogName, cancellationToken);
                    break;
                
                case "help":

                    var reply = turnContext.Activity.CreateReply();
                    var helpCardJson = GenericHelpers.ReadTextFromFile(@".\wwwroot\Resources\AdaptiveCards\HelpMessage.json");
                    reply.Attachments = new List<Attachment>()
                    {
                        DialogHelpers.CreateAdaptiveCardAttachment(helpCardJson)
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    break;

                default:
                    await dc.ContinueDialogAsync(cancellationToken);
                    if(!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync(Constants.RootDialogName, cancellationToken);
                    }
                    break;
                
            }
            return dc;
        }

        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity;


            //If the activity is the message type and not a magic code then we set the command name and save the user state
            //Post that we begin the Login Dialog.
            if(activity.Type == ActivityTypes.Message && !Regex.IsMatch(activity.Text, @"(\d{6})"))
            {
                await _accessors.CommandState.SetAsync(stepContext.Context, activity.Text, cancellationToken);
                await _accessors.UserState.SaveChangesAsync(stepContext.Context, cancellationToken: cancellationToken);
            }

            return await stepContext.BeginDialogAsync(Constants.LoginPromtName, cancellationToken: cancellationToken);
        }
        

        private async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Check if the dialog called in the previous step was executed successfully
            if(stepContext.Result != null)
            {
                //Check if the the Token exist with the Azure Bot Service
                var tokenResponse = stepContext.Result as TokenResponse;

                if(tokenResponse?.Token != null)
                {
                    var parts = _accessors.CommandState.GetAsync(stepContext.Context, () => string.Empty, cancellationToken: cancellationToken).Result.Split(' ');
                    string command = parts[0].ToLowerInvariant();

                    if(command == "getallapplications")
                    {
                        string sampleJsonMessage = GenericHelpers.ReadTextFromFile(@".\SampleMessages\GetApplicationsResponse.json");
                        List<BizTalkApplication> applications = JsonConvert.DeserializeObject<List<BizTalkApplication>>(sampleJsonMessage);

                        string welcomeAdaptiveCardJson = AdaptiveCardsHelper.CreateGetApplicationsAdaptiveCard(applications);

                        welcomeAdaptiveCardJson = welcomeAdaptiveCardJson.Replace("http://localhost/{0}", string.Format(Constants.CardImageUrl, Constants.BizManImage));

                        var reply = stepContext.Context.Activity.CreateReply();
                        reply.Attachments = new List<Attachment>()
                        {
                            DialogHelpers.CreateAdaptiveCardAttachment(welcomeAdaptiveCardJson)
                        };

                        await stepContext.Context.SendActivityAsync(reply);


                    }
                    else
                    {
                        var reply = stepContext.Context.Activity.CreateReply();
                        string operationsJson = GenericHelpers.ReadTextFromFile(@".\wwwroot\Resources\AdaptiveCards\Operations.json");
                        reply.Attachments = new List<Attachment>()
                        {
                            DialogHelpers.CreateAdaptiveCardAttachment(operationsJson)
                        };
                        await stepContext.Context.SendActivityAsync(reply);
                    }


                }

            }
            else
            {
                await stepContext.Context.SendActivityAsync("We couldn't log you in. Please try again later.", cancellationToken: cancellationToken);
                
            
            }
            await _accessors.CommandState.DeleteAsync(stepContext.Context, cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    
        #endregion
    }
}
