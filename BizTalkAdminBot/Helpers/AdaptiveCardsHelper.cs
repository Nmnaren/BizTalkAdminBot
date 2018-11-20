#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using BizTalkAdminBot.Models;
#endregion
namespace BizTalkAdminBot.Helpers
{

    /// <summary>
    ///This class is used to store metods which create the adaptive cards programatically
    /// </summary>
    public class AdaptiveCardsHelper
    {
        public static string CreateGetApplicationsAdaptiveCard(List<Application> applicationList)
        {
            #region TopLevelColumn
            AdaptiveColumnSet topLevelColumnSet = CreateTopLevelColumnSet();
            #endregion

            #region ApplicationLevelColumnSet
            AdaptiveColumnSet applicationColumnSet = new AdaptiveColumnSet()
            {
                Id = "applicationColumnSet",
                Separator = true,
                Spacing = AdaptiveSpacing.Default,

            };

            List<AdaptiveElement> applicationTextBlocks = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Id = "applicationNameStatic",
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator= true,
                    Size = AdaptiveTextSize.Default,
                    Text = "Application Name",
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Bolder,
                }
            };

            List<AdaptiveElement> statusTextBlocks = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Id = "statusStatic",
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = "Status",
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Bolder
                }
            };

            foreach (Application app in applicationList)
            {
                string name = app.Name;
                string status = app.Status;

                //We add the application name as a new text bock in the application textblock list and status
                // in the status text block list
                AdaptiveTextBlock applicationTextBlock = new AdaptiveTextBlock()
                {
                    Id = name,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = name,
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Default

                };

                AdaptiveTextBlock statusTextBlock = new AdaptiveTextBlock()
                {
                    Id = string.Format("{0}_{1}", name, status),
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = status,
                    Color = status == Constants.ApplicationStatus.Started.ToString() ? AdaptiveTextColor.Good : AdaptiveTextColor.Attention,
                    Weight = AdaptiveTextWeight.Default,

                };

                applicationTextBlocks.Add(applicationTextBlock);
                statusTextBlocks.Add(statusTextBlock);


            }

            AdaptiveColumn appColumn = new AdaptiveColumn()
            {
                Id = "applicationListColumn",
                Items = applicationTextBlocks,
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Width = AdaptiveColumnWidth.Stretch

            };

            AdaptiveColumn statusColumn = new AdaptiveColumn()
            {
                Id = "statusListColumn",
                Items = statusTextBlocks,
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Width = AdaptiveColumnWidth.Auto
            };

            applicationColumnSet.Columns = new List<AdaptiveColumn>()
            {
                appColumn,
                statusColumn
            };
            #endregion

            AdaptiveCard adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(topLevelColumnSet);
            adaptiveCard.Body.Add(applicationColumnSet);
            string adaptiveCardJson = adaptiveCard.ToJson();
            return adaptiveCardJson;
        }

        public static string CreateGetOrchestrationsAdaptiveCard(List<Orchestration> orchestrations)
        {
            #region TopLevelColumn
            AdaptiveColumnSet topLevelColumnSet = CreateTopLevelColumnSet();
            #endregion

            #region ApplicationLevelColumnSet

            AdaptiveColumnSet orchestrationColumnSet = new AdaptiveColumnSet()
            {
                Id = "orchestrationColumnSet",
                Separator = true,
                Spacing = AdaptiveSpacing.Default,

            };

            List<AdaptiveElement> orchestrationTextBlocks = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Id = "orchestrationNameStatic",
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator= true,
                    Size = AdaptiveTextSize.Default,
                    Text = "Orchestration Name",
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Bolder,
                }
            };

            List<AdaptiveElement> statusTextBlocks = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Id = "statusStatic",
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = "Status",
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Bolder
                }
            };

            List<AdaptiveElement> applicationNameTextBlocks = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Id = "applicationNameStatic",
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator= true,
                    Size = AdaptiveTextSize.Default,
                    Text = "Application",
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Bolder,
                }
            };

            List<AdaptiveElement> hostNameTextBlocks = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Id = "hostNameStatic",
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator= true,
                    Size = AdaptiveTextSize.Default,
                    Text = "Host",
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Bolder,
                }
            };

            foreach(Orchestration orchestration in orchestrations)
            {
                string name = orchestration.FullName;
                string status = orchestration.Status;
                string appName = orchestration.ApplicationName;
                string host = orchestration.Host;

                AdaptiveTextBlock orchestrationTextBlock = new AdaptiveTextBlock()
                {
                    Id = name,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = name,
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Default

                };

                AdaptiveTextBlock applicationNameTextBlock = new AdaptiveTextBlock()
                {
                    //GUID is used to make the ID unique
                    Id = string.Format("{0}_{1}", appName, System.Guid.NewGuid().ToString()),
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = appName,
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Default

                };

                AdaptiveTextBlock hostNameTextBlock = new AdaptiveTextBlock()
                {
                    //GUID is used to make the ID unique
                    Id = string.Format("{0}_{1}", host, System.Guid.NewGuid().ToString()),
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = host,
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Default

                };

                AdaptiveTextBlock statusTextBlock = new AdaptiveTextBlock()
                {
                    //GUID is used to make the ID unique
                    Id = string.Format("{0}_{1}", status, System.Guid.NewGuid().ToString()),
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                    Separator = true,
                    Size = AdaptiveTextSize.Default,
                    Text = host,
                    Color = AdaptiveTextColor.Default,
                    Weight = AdaptiveTextWeight.Default

                };

                orchestrationTextBlocks.Add(orchestrationTextBlock);
                applicationNameTextBlocks.Add(applicationNameTextBlock);
                hostNameTextBlocks.Add(hostNameTextBlock);
                statusTextBlocks.Add(statusTextBlock);
            }

            AdaptiveColumn orchestrationCoulmn = new AdaptiveColumn()
            {
                Id = "orchestrationListColumn",
                Items = orchestrationTextBlocks,
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Width = AdaptiveColumnWidth.Stretch

            };
            AdaptiveColumn applicationCoulmn = new AdaptiveColumn()
            {
                Id = "applicationListColumn",
                Items = applicationNameTextBlocks,
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Width = AdaptiveColumnWidth.Stretch

            };

            AdaptiveColumn hostCoulmn = new AdaptiveColumn()
            {
                Id = "hostListColumn",
                Items = hostNameTextBlocks,
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Width = AdaptiveColumnWidth.Stretch

            };

            AdaptiveColumn statusCoulmn = new AdaptiveColumn()
            {
                Id = "statusListColumn",
                Items = statusTextBlocks,
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Width = AdaptiveColumnWidth.Stretch

            };

            orchestrationColumnSet.Columns = new List<AdaptiveColumn>()
            {
                orchestrationCoulmn,
                applicationCoulmn,
                hostCoulmn,
                statusCoulmn
            };
            #endregion

            AdaptiveCard adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(topLevelColumnSet);
            adaptiveCard.Body.Add(orchestrationColumnSet);
            string adaptiveCardJson = adaptiveCard.ToJson();
            return adaptiveCardJson;

        }

        /// <summary>
        /// Create the Top Level Column Set in All Adaptive Cards
        /// </summary>
        /// <returns>Adaptive Column Set</returns>
        private static AdaptiveColumnSet CreateTopLevelColumnSet()
        {
            AdaptiveColumnSet topLevelColumnSet = new AdaptiveColumnSet()
            {
                Id = "topColumnSet",
                Separator = true,
                Spacing = AdaptiveSpacing.Default,
                Columns = new List<AdaptiveColumn>()
                {
                    new AdaptiveColumn()
                    {
                        Id = "bizManImageColumn",
                        Spacing = AdaptiveSpacing.Default,
                        Width = AdaptiveColumnWidth.Auto,
                        Items = new List<AdaptiveElement>()
                        {
                            new AdaptiveImage()
                            {
                                Id = "bizManImage",
                                Spacing = AdaptiveSpacing.Default,
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                                Size = AdaptiveImageSize.Small,
                                Style = AdaptiveImageStyle.Default,

                                //Replace the uri with the base64string later on
                                Url = new Uri("http://localhost/{0}"),
                            }
                        }

                    },
                    new AdaptiveColumn()
                    {
                        Id = "bizTalkGroupInformationColumn",
                        Spacing = AdaptiveSpacing.Default,
                        Style = AdaptiveContainerStyle.Emphasis,
                        Items = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock()
                            {
                                Id = "bizTalkGroupStatic",
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                                Color = AdaptiveTextColor.Default,
                                Text = "BizTalk Group",
                                Size = AdaptiveTextSize.Default,
                                Weight = AdaptiveTextWeight.Lighter,
                                Wrap = false
                            },
                            new AdaptiveTextBlock()
                            {
                                Id = "bizTalkGroupDynamic",
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                                Color = AdaptiveTextColor.Accent,
                                Text = "Production",
                                Size = AdaptiveTextSize.Default,
                                Weight = AdaptiveTextWeight.Bolder,
                                Wrap = false,

                            }
                        }
                    }
                },

            };
            return topLevelColumnSet;
        }

        /// <summary>
        /// Create an AdaptiveCard object from static json file
        /// </summary>
        /// <param name="resourcePath">Path where the json file is stored</param>
        /// <returns>Adaptive Card Object</returns>
        public static AdaptiveCard ParseAdaptiveCard(string resourcePath)
        {
            string adaptiveCardJson = GenericHelpers.ReadTextFromFile(resourcePath);

            var adaptiveCardResult = AdaptiveCard.FromJson(adaptiveCardJson);

            return adaptiveCardResult.Card;

        }
    }
}
