using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BotApp
{
    [LuisModel("", "")]
    [Serializable]

    public class WeatherDialog : LuisDialog<object>
    {
        public const string Entity_location = "Location";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"您好，我还年轻，目前只能提供中国地区天气查询功能";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }




        [LuisIntent("天气查询")]
        public async Task QueryWeather(IDialogContext context, LuisResult result)
        {
            string location = string.Empty;
            string replyString = "";

            if (TryToFindLocation(result, out location))
            {
                replyString = GetWeather(location);

                JObject WeatherResult = (JObject)JsonConvert.DeserializeObject(replyString);
                var weatherinfo = new
                {
                    城市 = WeatherResult["weatherinfo"]["city"].ToString(),
                    温度 = WeatherResult["weatherinfo"]["temp"].ToString(),
                    湿度 = WeatherResult["weatherinfo"]["SD"].ToString(),
                    风向 = WeatherResult["weatherinfo"]["WD"].ToString(),
                    风力 = WeatherResult["weatherinfo"]["WS"].ToString()
                };


                await context.PostAsync(weatherinfo.城市 + "的天气情况: 温度" + weatherinfo.温度 + "度;湿度" + weatherinfo.湿度 + ";风力" + weatherinfo.风力 + ";风向" + weatherinfo.风向);
            }
            else
            {

                await context.PostAsync("亲你要查询哪个地方的天气信息呢，快把城市的名字发给我吧");
            }
            context.Wait(MessageReceived);

        }
        //[LuisIntent("时间查询")]
        //public async Task QueryTime(IDialogContext context, LuisResult result)
        //{

        //    await context.PostAsync("现在北京时间："+DateTime.Now.ToUniversalTime());
           
        //    context.Wait(MessageReceived);
        //} 
         
       

        private string GetWeather(string location)
        {
            string weathercode = "";
            XmlDocument citycode = new XmlDocument();
            citycode.Load("https://wqbot.blob.core.windows.net/botdemo/CityCode.xml");
            XmlNodeList xnList = citycode.SelectNodes("//province//city//county");
            foreach (XmlElement xnl in xnList)
            {
                if (xnl.GetAttribute("name").ToString() == location)
                    weathercode = xnl.GetAttribute("weatherCode").ToString();
            }
            HttpClient client = new HttpClient();
            string result = client.GetStringAsync("http://www.weather.com.cn/data/sk/" + weathercode + ".html").Result;
            return result;
        }
        private bool TryToFindLocation(LuisResult result, out String location)
        {
            location = "";
            EntityRecommendation title;
            if (result.TryFindEntity("地点", out title))
            {
                location = title.Entity;
            }
            else
            {
                location = "";
            }
            return !location.Equals("");
        }
    }


}
