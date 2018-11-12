namespace BizTalkAdminBot
{

    #region References
    using System;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    #endregion


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
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        public IStatePropertyAccessor<string> CommandState { get; set; }

        public ConversationState ConversationState { get; }

        public UserState UserState { get; }

    }
}
