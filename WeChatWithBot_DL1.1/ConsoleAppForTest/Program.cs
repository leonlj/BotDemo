using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Bot.Connector.DirectLine.Models;


namespace ConsoleAppForTest
{
    class Program
    {
        static void Main()
        {

            HomeController hc = new HomeController();
            hc.TalkToTheBot("你好");
            //return;
            //MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        //private async static void PostMessage(string message)
        //{
        //    HttpClient client;
        //    HttpResponseMessage response;

        //    bool IsReplyReceived = false;

        //    client = new HttpClient();
        //    client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "uc0nyMvM0NI.cwA.wYs.B8F1M7cEBm9StsTDG8pmuOjhnxeCJd2LdvNlKVfBgro");
        //    response = await client.GetAsync("/api/tokens/");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var conversation = new Conversation();
        //        //response = await client.PostAsJsonAsync("/api/conversations/", conversation);
        //        response = await client.PostAsync("/api/conversations/", null);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
        //            string conversationUrl = ConversationInfo.conversationId + "/messages/";
        //            Message msg = new Message() { text = message };
        //            response = await client.PostAsJsonAsync(conversationUrl, msg);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                response = await client.GetAsync(conversationUrl);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
        //                    //string Messages = BotMessage;
        //                    IsReplyReceived = true;
        //                }
        //            }
        //        }

        //    }
        //    //return IsReplyReceived;
        //}

        //static async void MakeRequest()
        //{
        //    var client = new HttpClient();
        //    var queryString = HttpUtility.ParseQueryString("我要一张上海的机票");

        //    // Request headers
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "f338d46de48742f994cda51278a8d02e");

        //    var uri = "https://api.projectoxford.ai/luis/v1/application?id=37af8540-1208-4ff8-8fc6-bc8057fda50d&q=" + "我要一张去广州的机票";

        //    var response = await client.GetAsync(uri);

        //    string JSON = await response.Content.ReadAsStringAsync();

        //    Lusi ro = JsonHelper.Deserialize<Lusi>(JSON);

        //    return;
        //}
    }


    #region public class Chat
    public class Chat
    {
        public string ChatMessage { get; set; }
        public string ChatResponse { get; set; }
        public string watermark { get; set; }
    }
    #endregion

    public class HomeController
    {

        private static string DiretlineUrl
            = @"https://directline.botframework.com";
        private static string directLineSecret =
            "iytv0uIOQNA.cwA.V2I.qGTE72pDm3w_BS4X1KX04gESdEAsJ_zsQ1UCCljDMlk";
        private static string botId =
            "3785f860-5705-40b9-8698-ca1ad4e5633b";

        public async Task<Chat> TalkToTheBot(string paramMessage)
        {
            // Connect to the DirectLine service
            DirectLineClient client = new DirectLineClient(directLineSecret);

            // Try to get the existing Conversation
            Conversation conversation = null;
               // System.Web.HttpContext.Current.Session["conversation"] as Conversation;
            // Try to get an existing watermark 
            // the watermark marks the last message we received

            if (conversation == null)
            {
                // There is no existing conversation
                // start a new one
                conversation = client.Conversations.NewConversation();
            }

            // Use the text passed to the method (by the user)
            // to create a new message
            Message message = new Message
            {
                FromProperty = "Leon",
                Text = paramMessage
            };

            // Post the message to the Bot
            await client.Conversations.PostMessageAsync(conversation.ConversationId, message);

   


            // Return the response as a Chat object
            return null;
        }

        private async Task<Chat> ReadBotMessagesAsync(
            DirectLineClient client, string conversationId, string watermark)
        {
            // Create an Instance of the Chat object
            Chat objChat = new Chat();

            // We want to keep waiting until a message is received
            bool messageReceived = false;
            while (!messageReceived)
            {
                // Get any messages related to the conversation since the last watermark 
                var messages =
                    await client.Conversations.GetMessagesAsync(conversationId, watermark);

                // Set the watermark to the message received
                watermark = messages?.Watermark;

                // Get all the messages 
                var messagesFromBotText = from message in messages.Messages
                                          where message.FromProperty == botId
                                          select message;

                // Loop through each message
                foreach (Message message in messagesFromBotText)
                {
                    // We have Text
                    if (message.Text != null)
                    {
                        // Set the text response
                        // to the message text
                        objChat.ChatResponse
                            += " "
                            + message.Text.Replace("\n\n", "<br />");
                    }

                    // We have an Image

                }

                // Mark messageReceived so we can break 
                // out of the loop
                messageReceived = true;
            }

            // Set watermark on te Chat object that will be 
            // returned
            objChat.watermark = watermark;

            // Return a response as a Chat object
            return objChat;
        }
    }

}