using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Deepleo.Web
{
    //http://stackoverflow.com/questions/37518770/microsoft-bot-with-direct-line-rest-api-for-newbie-of-bot
    public class MSBot
    {
        public async static Task<string> PostMessage(string message)
        {
            HttpClient client;
            HttpResponseMessage response;

            bool IsReplyReceived = false;

            string ReceivedString = null;

            client = new HttpClient();
            client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "myI48W5qkkk.cwA.t8E.jseT_7Y9odqoQHOvehPnW-voEgR1NC7eW99NfaonN_Q");
            }
            catch (Exception e) {
                return "验证环节失败";

            }

            response = await client.GetAsync("/api/tokens/");
            
            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                try { response = await client.PostAsJsonAsync("/api/conversations/", conversation); }
                catch (Exception e)
                {
                    return "取id环节失败";

                }

                //response = await client.PostAsync("/api/conversations/", null);
                if (response.IsSuccessStatusCode)
                {
                    Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                    string conversationUrl = ConversationInfo.conversationId + "/messages/";
                    Message msg = new Message() { text = message };
                    response = await client.PostAsJsonAsync(conversationUrl, msg);
                    if (response.IsSuccessStatusCode)
                    {
                        response = await client.GetAsync(conversationUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
                            ReceivedString = BotMessage.messages[1].text;
                            IsReplyReceived = true;
                        }
                    }
                }

            }
            return ReceivedString;
        }
    }


    public class Conversation
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public string eTag { get; set; }
    }

    public class MessageSet
    {
        public Message[] messages { get; set; }
        public string watermark { get; set; }
        public string eTag { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string conversationId { get; set; }
        public DateTime created { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string channelData { get; set; }
        public string[] images { get; set; }
        public Attachment[] attachments { get; set; }
        public string eTag { get; set; }
    }

    public class Attachment
    {
        public string url { get; set; }
        public string contentType { get; set; }
    }

}