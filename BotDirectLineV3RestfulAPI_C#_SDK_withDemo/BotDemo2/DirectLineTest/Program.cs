using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectLineSDK;
using DirectLineSDK.Models;
using Newtonsoft.Json;

namespace DirectLineTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("DirectLineSDK Test");
            Console.WriteLine("开始测试");

            BotClient botClient = new BotClient();

            //这里测试的是贩卖机的Bot
            string botFrom = "vendingMachineBotDemo";
            string botConnectorKey = "aDyJxnUSx30.cwA.WOg.4DzXtwItzBC6jyUCxHXG8fLKcgdx2zZYf2BkkfW5Lpc";

            Start(botClient, "您好", botFrom, botConnectorKey);

            bool isContinue = true;
            while (isContinue)
            {
                string inputText = Console.ReadLine();
                if (string.IsNullOrEmpty(inputText))
                {
                    isContinue = false;
                }
                Start(botClient, inputText, botFrom, botConnectorKey);
            }
        }


        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="inputText"></param>
        /// <param name="botFrom"></param>
        /// <param name="botConnectorKey"></param>
        public static async void Start(BotClient botClient, string inputText, string botFrom, string botConnectorKey)
        {
            Console.WriteLine("正在通信，请稍等...");

            ConversationResult result = await botClient.Conversation(inputText, botConnectorKey);

            if (result.resultMessage != null)
            {
                Console.WriteLine(result.resultMessage);
            }

            if (result.botActivities != null)
            {
                foreach (Activities activities in result.botActivities.activities.Where(item => item.from.id == botFrom))
                {
                    Console.WriteLine();
                    Console.WriteLine("贩卖机Bot：" + activities.text);
                }

                Console.WriteLine();
                Console.Write("请输入：");
            }
        }
    }
}
