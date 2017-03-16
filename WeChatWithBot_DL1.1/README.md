## Demo for using bot direct line V1.1 rest APIs to connect bot with WeChat offical account
In bot frameowrk there is channel - direcline that can help to connect the bot to any canvas like native app, Wechat offical account, Wechat enterprise account,IoT device etc. This sample is based on WeiXinSDK.net C# open source code for Wechat offical account, we used bot DL to connect a WeChat offical account backend to a bot. The message received from WeChat can be transfer to bot. 

Bot DL provids restful APIs, so it can be implemented by any language. The demo was implemeted by C#, if you were other dev langauge like Java, PHP, JS, etc, you could refer to the flow, and build with restful APIs by different languages.

For WeChat offical account, the projet with C# can be used as WeChat offical account backend directly. Hopefully, everyone could build your own bot in WeChat. :-)  

### How doest it work ? 

In Wechat offical account backend, it will get a message from WeChat service
WeixinMessageType.Text will handle text message from the user, where we can use PostMessage function in MSBot to forward this message to bot, and get reply from bot, finally reply to the user in WeChat. Please kindly find code snapshot

        switch (message.Type)
        {
            case WeixinMessageType.Text://文字消息
            {
                var msgId = message.Body.MsgId.Value.ToString();
                string userMessage = message.Body.Content.Value;
                string BotMessage = await MSBot.PostMessage(userMessage);
                result = ReplayPassiveMessageAPI.RepayText(openId, myUserName, BotMessage);
            }
            break;

Now, in MSBot Class we need to implment PostMessage

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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "aDyJxnUSx30.cwA.WOg.4DzXtwItzBC6jyUCxHXG8fLKcgdx2zZYf2BkkfW5Lpc");
            response = await client.GetAsync("/api/tokens/");
            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                response = await client.PostAsJsonAsync("/api/conversations/", conversation);
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

