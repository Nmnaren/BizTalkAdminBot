#region References
using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using BizTalkAdminBot.Models;

#endregion

namespace BizTalkAdminBot
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class BizTalkAdminBotAccessors
    {
        public BizTalkAdminBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(ConversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(UserState));
        }
        public static readonly string DialogStateName = $"{nameof(BizTalkAdminBotAccessors)}.DialogState";

        public static readonly string CommandStateName = $"{nameof(BizTalkAdminBotAccessors)}.CommandState";

        public static readonly string ApplicationStateName = $"{nameof(BizTalkAdminBotAccessors)}.ApplicationState";

        public static readonly string OrchestrationStateName = $"{nameof(BizTalkAdminBotAccessors)}.OrchestrationState";
        public static readonly string HostStateName = $"{nameof(BizTalkAdminBotAccessors)}.HostState";

        public static readonly string SendPortStateName = $"{nameof(BizTalkAdminBotAccessors)}.SendPortState";

        public static readonly string FeedbackStateName = "${nameof(BizTalkAdminBotAccessors)}.FeedbackState";

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        public IStatePropertyAccessor<string> CommandState { get; set; }

        public IStatePropertyAccessor<List<Application>> ApplicationState {get; set; }

        public IStatePropertyAccessor<List<Orchestration>> OrchestrationState {get; set; }
        
        public IStatePropertyAccessor<List<Host>> HostState {get; set; }

        public IStatePropertyAccessor<List<SendPort>> SendPortState {get; set; } 

        public IStatePropertyAccessor<bool> FeedbackState {get; set;}

        public ConversationState ConversationState { get; }

        public UserState UserState { get; }

    }
}
