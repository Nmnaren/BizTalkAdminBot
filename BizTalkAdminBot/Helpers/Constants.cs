namespace BizTalkAdminBot.Helpers
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// Class which contains all the comman constants that are used across code
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Connection Name of the Oauth Setting used from Bot Channels Registration
        public const string OAuthConnectionName = "AADV2";


        /// <summary>
        /// Login Prompt Name which will be used to send out a sign in card
        /// </summary>
        public const string LoginPromtName = "loginPrompt";


        /// <summary>
        /// Name of the root dialog
        /// </summary>
        public const string RootDialogName = "rootDialog";

        /// <summary>
        /// Constant used to make the images renderable by the bot as the attachments.
        /// </summary>
        public const string CardImageUrl = "data:image/png;base64,{0}";

        /// <summary>
        /// Content Type used while creating an attachment for adaptive card.
        /// </summary>
        public const string AdaptiveCardContentType = "application/vnd.microsoft.card.adaptive";

        /// <summary>
        ///Version of the Adaptive Schema JSON Format 
        /// </summary>
        public const string AdaptiveCardsSchemaVersion = "1.0";

        /// <summary>
        /// Enumeration Used to Determine the Color of status displayed in the adaptive
        /// </summary>
        public enum ApplicationStatus
        {
            Started,
            PartiallyStarted,
            Stopped
        }

        public enum InstanceStatus
        {
            Suspended
            
        }
        
        /// <summary>
        /// Enumeration Holding the names of the Static adaptive cards
        /// </summary>
        public enum AdaptiveCards
        {
            ErrorMessage,
            FeedBackCard,
            OperationMessageNoFB,
            HelpMessage,
            OperationsMessage,
            SignOutMessage,
            WelcomeMessage
        }


        /// <summary>
        /// Request Used to Post Data to Logic App
        /// </summary>
        public const string PostRequest = "\"operation\": \"{0}\"";

        /// <summary>
        /// Url of the Logic APP which connects the Bot to the On Premises BizTalk Environment
        ///  </summary>
        public const string LogicAppUri = @"https://prod-20.westindia.logic.azure.com:443/workflows/953a15dda4c64207b4ab72931b2c75d7/triggers/request/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Frequest%2Frun&sv=1.0&sig=5R5_66N6Wf9V-aM7UycTENh7FGUNLM7mYNgT567zcto";

        /// <summary>
        /// Dummy Url for the BizMan Header Image
        /// </summary>
        public const string BizManDummyUrl = "http://localhost/{0}";

        /// <summary>
        /// Path to the BizMan Image
        /// </summary>
        public const string BizManImagePath = @".\wwwroot\Resources\Images\BizMan.png";

        public const string AdaptiveCardPath = @"wwwroot\Resources\AdaptiveCards\{0}.json";

        public const string NotFoundMessage = "Sorry, the Search for {} did not yeild any result.";

        public const string BlobConnectionString = @"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net";

        public const string BlobContainerName = "biztalkadminbotcommon";

        public const string ReportBaseUrl = "https://{0}.blob.core.windows.net/biztalkadminbotcommon/{1}";

    }
}
