using DirectLineSDK;
using DirectLineSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Web;

namespace Deepleo.Web
{
    public class MSBot
    {

        static string replyMessages = null;
        static BotClient botClient = null;

        public async static Task<string> PostMessage(string message)
        {
            if(botClient == null)
            {
                botClient = new BotClient();
            }

            //这里测试的是贩卖机的Bot
            string botFrom = "changeTobotHandler";
            string botConnectorKey = "Put Bot Direct key here";

            ConversationResult result = await botClient.Conversation(message, botConnectorKey);

            if (result.botActivities != null)
            {
                foreach (Activities activities in result.botActivities.activities.Where(item => item.from.id == botFrom))
                {
                    replyMessages = activities.text;
                }
            }
            return replyMessages;
        }


    }


}