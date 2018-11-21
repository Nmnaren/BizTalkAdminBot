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

        public static string CreateGetOrchestrationsAdaptiveCard(List<Orchestration> orchestrations, string appName)
        {
            #region TopLevelColumn
            AdaptiveColumnSet topLevelColumnSet = CreateTopLevelColumnSet();
            #endregion

            #region Container

            AdaptiveContainer container = new AdaptiveContainer()
            {
                Id = "container",
                Spacing = AdaptiveSpacing.Default,
                Separator = true,
                Items = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock()
                    {
                        Id = "OrchestrationByAppName",
                        Color = AdaptiveTextColor.Default,
                        IsSubtle = true,
                        Separator = true,
                        Spacing = AdaptiveSpacing.Default,
                        Text = string.Format("Orchestrations in {0}", appName)

                    }
                }

            };
            
            #endregion

            #region FactSet

            
            foreach(Orchestration orchestration in orchestrations)
            {
                string name = orchestration.FullName;
                name = name.Substring(orchestration.AssemblyName.Length + 1);

                AdaptiveFactSet orchFactSet = new AdaptiveFactSet()
                {
                    Id = name,
                    Separator = true,
                    Facts = new List<AdaptiveFact>()
                    {
                        new AdaptiveFact()
                        {
                            Title = "Name",
                            Value = name,
                        },
                        new AdaptiveFact()
                        {
                            Title = "Host",
                            Value = orchestration.Host
                        },

                        new AdaptiveFact()
                        {
                            Title = "Status",
                            Value = orchestration.Status
                        }
                    }
                };
                container.Items.Add(orchFactSet);
                
            }
            #endregion


            AdaptiveCard adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(topLevelColumnSet);
            adaptiveCard.Body.Add(container);
            string adaptiveCardJson = adaptiveCard.ToJson();
            return adaptiveCardJson;

        }

        /// <summary>
        /// Create the application list drop down
        /// </summary>
        /// <param name="applications"></param>
        /// <returns></returns>
        public static string CreateSelectApplicationListAdaptiveCard(List<Application> applications)
        {
            #region TopLevelColumn
            AdaptiveColumnSet topLevelColumnSet = CreateTopLevelColumnSet();
            #endregion


            #region ChoiceList
            
            AdaptiveChoiceSetInput choiceSetInput = new AdaptiveChoiceSetInput()
            {
                Id = "applicationChoiceSet",
                Separator = true,
                Style = AdaptiveChoiceInputStyle.Compact,
                                
            };

            foreach(Application app in  applications)
            {
                string name = app.Name;

                AdaptiveChoice choice = new AdaptiveChoice()
                {
                    Title = name,
                    Value = name
                };

                choiceSetInput.Choices.Add(choice);
                
            }
            AdaptiveCard adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(topLevelColumnSet);
            adaptiveCard.Body.Add(new AdaptiveTextBlock(){Id = "header", Text = "Please Select an Application", Wrap = true, Color = AdaptiveTextColor.Accent, IsSubtle = true});
            adaptiveCard.Body.Add(choiceSetInput);
            adaptiveCard.Actions = new List<AdaptiveAction>()
            {
                new AdaptiveSubmitAction()
                {
                    Id = "submit",
                    Title = "Submit",
                    DataJson = "{\"command\": \"getorchbyapp\"}"
                }
            };
            
            #endregion

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
