using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;

namespace BotApp
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                //if (activity.Text.Contains("Hi") || activity.Text.Contains("您好") || activity.Text.Contains("你好"))
                //{
                //    Activity reply = activity.CreateReply($"您是要看什么？");
                //    await connector.Conversations.ReplyToActivityAsync(reply);
                //}
                //else 
                if (activity.Text == "1")
                {
                    Activity reply = activity.CreateReply();
                    reply.Attachments = new List<Attachment>();
                    reply.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "http://pic6.huitu.com/res/20130116/84481_20130116142820494200_1.jpg",
                        ContentType = "image/jpg",
                        Name = "Bender_Rodriguez.jpg"
                    });

                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else if (activity.Text == "Hi")
                {
                    Activity replyToConversation = activity.CreateReply("我已经准备好了今天的汇报！");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "http://api.page2images.com/ccimages/f0/b6/HOjVZowBJ0TersO7.png"));
                    cardImages.Add(new CardImage(url: "http://api.page2images.com/ccimages/f0/b6/HOjVZowBJ0TersO7.png"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction splButton = new CardAction()
                    {
                        Value = "start",
                        Type = "postBack",
                        Title = "start"

                    };
                    cardButtons.Add(splButton);
                    CardAction AplButton = new CardAction()
                    {
                        Value = "5",
                        Type = "postBack",
                        Title = "All"
                    };
                    cardButtons.Add(AplButton);
                    CardAction eplButton = new CardAction()
                    {
                        Value = "http://botwebchat.chinacloudsites.cn/Bot/Email",
                        Type = "openUrl",
                        Title = "Email Me"
                    };
                    cardButtons.Add(eplButton);
                    HeroCard plCard = new HeroCard()
                    {
                        Title = "",
                        Subtitle = "",
                        Images = cardImages,
                        Buttons = cardButtons

                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                else if (activity.Text == "start")
                {
                    Activity reply = activity.CreateReply($"start");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }

                else if (activity.Text == "3")
                {
                    Activity replyToConversation = activity.CreateReply("全部报表如下");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "http://pic.qiantucdn.com/58pic/17/41/38/88658PICNuP_1024.jpg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "https://www.baidu.com",
                        Type = "openUrl",
                        Title = "WikiPedia Page"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = "I'm a thumbnail card",
                        Subtitle = "Pig Latin Wikipedia Page",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);


                }
                else if (activity.Text == "4")
                {
                    Activity replyToConversation = activity.CreateReply("Receipt card");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "http://pic.qiantucdn.com/58pic/17/41/38/88658PICNuP_1024.jpg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "https://www.baidu.com",
                        Type = "openUrl",
                        Title = "WikiPedia Page"
                    };
                    cardButtons.Add(plButton);
                    ReceiptItem lineItem1 = new ReceiptItem()
                    {
                        Title = "Pork Shoulder",
                        Subtitle = "8 lbs",
                        Text = null,
                        Image = new CardImage(url: "http://pic.qiantucdn.com/58pic/17/41/38/88658PICNuP_1024.jpg"),
                        Price = "16.25",
                        Quantity = "1",
                        Tap = null
                    };
                    ReceiptItem lineItem2 = new ReceiptItem()
                    {
                        Title = "Bacon",
                        Subtitle = "5 lbs",
                        Text = null,
                        Image = new CardImage(url: "http://pic.qiantucdn.com/58pic/17/41/38/88658PICNuP_1024.jpg"),
                        Price = "34.50",
                        Quantity = "2",
                        Tap = null
                    };
                    List<ReceiptItem> receiptList = new List<ReceiptItem>();
                    receiptList.Add(lineItem1);
                    receiptList.Add(lineItem2);
                    ReceiptCard plCard = new ReceiptCard()
                    {
                        Title = "I'm a receipt card, isn't this bacon expensive?",
                        Buttons = cardButtons,
                        Items = receiptList,
                        Total = "275.25",
                        Tax = "27.52"
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                else if (activity.Text == "5")
                {
                    Activity replyToConversation = activity.CreateReply("Should go to conversation, with a carousel");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    replyToConversation.Attachments = new List<Attachment>();
                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    cardContentList.Add("PigLatin", "http://img.taopic.com/uploads/allimg/140324/267861-14032421510254.jpg");


                    foreach (KeyValuePair<string, string> cardContent in cardContentList)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: cardContent.Value));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction uplButton = new CardAction()
                        {
                            Value = "上一页",
                            Type = "postBack",
                            Title = "上一页"
                        };
                        cardButtons.Add(uplButton);
                        CardAction plButton = new CardAction()
                        {
                            Value = "http://pbiembedtestweb.azurewebsites.net/Dashboard/Report?reportId=89957040-2963-4015-adbc-c169bfff0738",
                            Type = "openUrl",
                            Title = "打开报表"
                        };
                        cardButtons.Add(plButton);

                        CardAction dplButton = new CardAction()
                        {
                            Value = "下一页",
                            Type = "postBack",
                            Title = "下一页"
                        };
                        cardButtons.Add(dplButton);
                        HeroCard plCard = new HeroCard()
                        {
                            Title = "",
                            Subtitle = "",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                    }
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                else if (activity.Text == "下一页")
                {
                    Activity replyToConversation = activity.CreateReply("Should go to conversation, with a carousel");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    replyToConversation.Attachments = new List<Attachment>();
                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    cardContentList.Add("Pork Shoulder", "http://pic40.nipic.com/20140415/2531170_191119071000_2.jpg");

                    foreach (KeyValuePair<string, string> cardContent in cardContentList)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: cardContent.Value));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction uplButton = new CardAction()
                        {
                            Value = "上一页",
                            Type = "postBack",
                            Title = "上一页"
                        };
                        cardButtons.Add(uplButton);
                        CardAction plButton = new CardAction()
                        {
                            Value = "http://pbiembedtestweb.azurewebsites.net/Dashboard/Report?reportId=89957040-2963-4015-adbc-c169bfff0738",
                            Type = "openUrl",
                            Title = "打开报表"
                        };
                        cardButtons.Add(plButton);

                        CardAction dplButton = new CardAction()
                        {
                            Value = "下一页",
                            Type = "postBack",
                            Title = "下一页"
                        };
                        cardButtons.Add(dplButton);
                        HeroCard plCard = new HeroCard()
                        {
                            Title = "",
                            Subtitle = "",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                    }
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                else if (activity.Text == "上一页")
                {
                    Activity replyToConversation = activity.CreateReply("Should go to conversation, with a carousel");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    replyToConversation.Attachments = new List<Attachment>();
                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    cardContentList.Add("Bacon", "http://pic40.nipic.com/20140412/2531170_084639470000_2.jpg");

                    foreach (KeyValuePair<string, string> cardContent in cardContentList)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: cardContent.Value));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction uplButton = new CardAction()
                        {
                            Value = "上一页",
                            Type = "postBack",
                            Title = "上一页"
                        };
                        cardButtons.Add(uplButton);
                        CardAction plButton = new CardAction()
                        {
                            Value = "http://pbiembedtestweb.azurewebsites.net/Dashboard/Report?reportId=89957040-2963-4015-adbc-c169bfff0738",
                            Type = "openUrl",
                            Title = "打开报表"
                        };
                        cardButtons.Add(plButton);

                        CardAction dplButton = new CardAction()
                        {
                            Value = "下一页",
                            Type = "postBack",
                            Title = "下一页"
                        };
                        cardButtons.Add(dplButton);
                        HeroCard plCard = new HeroCard()
                        {
                            Title = "",
                            Subtitle = "",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                    }
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                else if (activity.Text == "6")
                {

                }
                else if (activity.Text == "7")
                {


                }
                else if (activity.Text == "8")
                {

                }
                else if (activity.Text == "9")
                {

                }
                else if (activity.Text == "10")
                {

                }
                else
                {
                    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");

                    //await connector.Conversations.ReplyToActivityAsync(reply);
                    await Conversation.SendAsync(activity, () => new WeatherDialog());
                }

               
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}