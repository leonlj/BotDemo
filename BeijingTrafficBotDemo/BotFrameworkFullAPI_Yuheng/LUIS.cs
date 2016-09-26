using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace BotFrameworkFullAPI_Yuheng
{


   [LuisModel("4471c27f-01ac-4f92-955b-b792ec531ad3", "781479ab342a42f88ca645b30d22b411")]



    [Serializable]
    public class SimpleLUISDialog : LuisDialog<object>
    {


        public const string ENTITY_PLATE= "车牌号";
        private string plate = "京A12345";

        public const string HELLO_ANSWERE = "您好，我是交管局智能服务机器人小豆,请问有什么可以帮您 ?";
        public const string ASK_FOR_PlATE = "好的，请您提供车牌号码";
        public const string RESULT_RETURN1 = "恭喜您，您的车辆";
        public const string RESULT_RETURN2 = "没有任何违章记录";
        public const string RE_CONFIRM = "不客气，请问还有什么可以帮您?";


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"嘛玩意，小豆并没懂啊 说人话 比如： " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("违章查询")]
        public async Task WeatherQuery(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(ASK_FOR_PlATE);
            context.Wait(MessageReceived);

        }

        [LuisIntent("打招呼")]
        public async Task SetTemp(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(HELLO_ANSWERE);
            context.Wait(MessageReceived);

        }



        [LuisIntent("车牌号获取")]
        public async Task DecreaseTemp(IDialogContext context, LuisResult result)
        {
            EntityRecommendation platenum;
            if (!result.TryFindEntity(ENTITY_PLATE, out platenum))
            {
            platenum = new EntityRecommendation(type: ENTITY_PLATE) { Entity = string.Empty };
            }


            if (platenum != null)
            {
                plate = $" 京{platenum.Entity} ";
                await context.PostAsync(RESULT_RETURN1+plate+RESULT_RETURN2);
            }
            else
            {
                await context.PostAsync("不能找到您的车辆信息");
            }

            context.Wait(MessageReceived);
        }
        [LuisIntent("谢谢")]
        public async Task Thanks_return(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(RE_CONFIRM);
            context.Wait(MessageReceived);

        }

        [LuisIntent("没有确认")]
        public async Task IncreaseTemp(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("感谢您的查询，祝您生活愉快，再见！");
            context.Wait(MessageReceived);

        }

        [LuisIntent("验车查询")]
        public async Task verifyVehicle(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("中关村附近的验车地址已经帮您找到了，请打开链接查看  http://j.map.baidu.com/nEDvk ");
            context.Wait(MessageReceived);

        }

        public SimpleLUISDialog(ILuisService service = null)
            : base(service)
        {
        }

    }
}