using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Text;
using Bot_Application2.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace VendingLuisDlg2
{


    [LuisModel("da9e23fd-b4b5-496a-88b3-43484caef4f4", "e81aa411c89f4da88c7bda4022a846fc")]



    [Serializable]
    public class SimpleLUISDialog : LuisDialog<object>
    {

        public const string HELLO_ANSWERE = "您好，我是贩卖机管理小助手，请您先登录系统，输入验证码：";
        public const string A_VERIFY = "验证成功，请问有什么可以帮您 ？ ";
        public const string A_MACHINE_PRODUCT_STATUS = "号贩卖机";
        public const string A_MACHINE_PRODUCT_STATUS1 = "瓶可乐，";
        public const string A_MACHINE_PRODUCT_STATUS2 = "瓶雪碧。";
        public const string A_SUPPLY_PRODUCT = "已经安排了。请问还有什么可以帮您 ？";
        public const string MACHINE_STATUS = "号贩卖机目前工作正常，但是，根据我们的预测，周六需要补充";
        public const string MACHINE_STATUS2 = "瓶可乐。";
        public const string A_MACHINE_LOCATION = "号贩卖机的具体位置请参考地图链接 http://j.map.baidu.com/TDG0F";
        public const string A_MACHINE_POWERPI = "贩卖机整体状态PowerBi链接请参考 https://msit.powerbi.com/view?r=eyJrIjoiYWU2OTlkOGItYmNlMS00NmQ4LWJkMTUtMGE3YzRiOTU0MzM1IiwidCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsImMiOjV9 ";
        public const string ENTITY_MACHINENO = "";
        public const string RE_CONFIRM = "不客气，请问还有什么可以帮您?";
        public const string STATUS_QUERY_URL = "http://smartapi.chinacloudsites.cn/api/v1/querygoodsstatus?deviceid=98011609002D";
        public const string TEMPER_QUERY_URL = "http://smartapi.chinacloudsites.cn/api/v1/querymachinestatus?deviceid=98011609002D";
        private string machineNo = "1234";

        //Hint: You can use google map link for showing the location info.
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"对不起，目前我不支持这项服务，可以询问我：" + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("验证码")]
        public async Task WeatherQuery(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(A_VERIFY);
            context.Wait(MessageReceived);

        }

        [LuisIntent("打招呼")]
        public async Task SetTemp(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(HELLO_ANSWERE);
            context.Wait(MessageReceived);

        }


        [LuisIntent("贩卖机状态")]
        public async Task DecreaseTemp(IDialogContext context, LuisResult result)
        {
            Random rd = new Random();
            int number1 = rd.Next(1,50);
            int number2 = rd.Next(1,15);
            int numMachine = rd.Next(1000, 2000);
            string num1 = Convert.ToString(number1);
            string num2 = Convert.ToString(number2);
            string machineNo = Convert.ToString(numMachine);
            string machineStatus = HttpGet(STATUS_QUERY_URL,"");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if(true == goodsData.success)
            {
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                for (int i = 0; i< goodsData.goodsdata.Length; i++)
                {
                    if(goodsData.goodsdata[i].goodsstatus == 1)
                    {
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "货道" + "故障" +
                            "商品名称："+ goodsData.goodsdata[i].goodsname;
                        MyStringBuilder.Append(tempResult);

                        MyStringBuilder.AppendLine("");
                    }
                }
                string finalStatusResult = MyStringBuilder.ToString();
                if (finalStatusResult.Length > 1)
                {
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + "工作正常");

            }
            else
            {
                await context.PostAsync("无法查询到贩卖机状态，请稍后重试");
            }
            //await context.PostAsync(machineNo + A_MACHINE_PRODUCT_STATUS + num1 + A_MACHINE_PRODUCT_STATUS1 + num2 + A_MACHINE_PRODUCT_STATUS2);
            //await context.PostAsync(machineStatus);
            context.Wait(MessageReceived);
        }
        [LuisIntent("有无货物")]
        public async Task goodsStatus(IDialogContext context, LuisResult result)
        {
            string machineStatus = HttpGet(STATUS_QUERY_URL, "");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if (true == goodsData.success)
            {
                List<Goodsdata> shortageGoods = new List<Goodsdata>();
                List<string> resultGoods = new List<string>();
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                int goodsStockNo;
                for (int i = 0; i < goodsData.goodsdata.Length; i++)
                {
                    if (goodsData.goodsdata[i].goodsstock == 1)
                    {
                        shortageGoods.Add(goodsData.goodsdata[i]);
                        goodsStockNo = goodsData.goodsdata[i].goodsno;
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "货道" +
                            goodsData.goodsdata[i].goodsname + "缺货";
                        MyStringBuilder.Append(tempResult);
                        MyStringBuilder.AppendLine("");
                    }
                }
                string finalStatusResult = MyStringBuilder.ToString();
                if(finalStatusResult.Length > 1)
                {
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                else
                {
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + "货物充足");
                }

            }
            else
            {
                await context.PostAsync("无法查询到贩卖机状态，请稍后重试");
            }
            //await context.PostAsync(machineNo + A_MACHINE_PRODUCT_STATUS + num1 + A_MACHINE_PRODUCT_STATUS1 + num2 + A_MACHINE_PRODUCT_STATUS2);
            //await context.PostAsync(machineStatus);
            context.Wait(MessageReceived);
        }
        [LuisIntent("货道状态")]
        public async Task goodsLineStatus(IDialogContext context, LuisResult result)
        {
            string machineLineNo = result.Entities[0].Entity;
            string machineStatus = HttpGet(STATUS_QUERY_URL, "");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if (true == goodsData.success)
            {
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string goodsStockStatus ="";
                string goodsStatus;
                //int goodsStockNo;
                for (int i = 0; i < goodsData.goodsdata.Length; i++)
                {
                    if (goodsData.goodsdata[i].goodsno.ToString() == machineLineNo)
                    {
                        //shortageGoods.Add(goodsData.goodsdata[i]);
                        //goodsStockNo = goodsData.goodsdata[i].goodsno;
                        if(0 == goodsData.goodsdata[i].goodsstock)
                        {
                            goodsStockStatus = "有货";
                        }
                        else
                        {
                            goodsStockStatus = "缺货";
                        }
                        if(0 == goodsData.goodsdata[i].goodsstatus)
                        {
                            goodsStatus = "正常,";
                        }
                        else
                        {
                            goodsStatus = "故障,";
                        }
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "号货道" + goodsStatus +
                            "商品名称："+ goodsData.goodsdata[i].goodsname + goodsStockStatus;
                        MyStringBuilder.Append(tempResult);
                        MyStringBuilder.AppendLine("");
                        break;
                    }
                }
                if(goodsStockStatus.Length > 1)
                {
                    string finalStatusResult = MyStringBuilder.ToString();
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                else
                {
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + "货道"+ machineLineNo
                        + "不存在");
                }
            }
            else
            {
                await context.PostAsync("无法查询到贩卖机状态，请稍后重试");
            }
            //await context.PostAsync(machineNo + A_MACHINE_PRODUCT_STATUS + num1 + A_MACHINE_PRODUCT_STATUS1 + num2 + A_MACHINE_PRODUCT_STATUS2);
            //await context.PostAsync(machineStatus);
            context.Wait(MessageReceived);
        }
        [LuisIntent("货道商品的名称")]
        public async Task goodsLineName(IDialogContext context, LuisResult result)
        {
            string machineLineNo = result.Entities[0].Entity;
            string machineStatus = HttpGet(STATUS_QUERY_URL, "");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if (true == goodsData.success)
            {
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string goodsStockStatus ="";
                //int goodsStockNo;
                for (int i = 0; i < goodsData.goodsdata.Length; i++)
                {
                    if (goodsData.goodsdata[i].goodsno.ToString() == machineLineNo)
                    {
                        //shortageGoods.Add(goodsData.goodsdata[i]);
                        //goodsStockNo = goodsData.goodsdata[i].goodsno;
                        if (0 == goodsData.goodsdata[i].goodsstock)
                        {
                            goodsStockStatus = "有货";
                        }
                        else
                        {
                            goodsStockStatus = "缺货";
                        }
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "货道" +
                            "商品名称：" + goodsData.goodsdata[i].goodsname + goodsStockStatus;
                        MyStringBuilder.Append(tempResult);
                        MyStringBuilder.AppendLine("");
                        break;
                    }
                }
                if (goodsStockStatus.Length > 1)
                {
                    string finalStatusResult = MyStringBuilder.ToString();
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                else
                {
                    await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + "货道" + machineLineNo
                        + "不存在");
                }
            }
            else
            {
                await context.PostAsync("无法查询到贩卖机状态，请稍后重试");
            }
            //await context.PostAsync(machineNo + A_MACHINE_PRODUCT_STATUS + num1 + A_MACHINE_PRODUCT_STATUS1 + num2 + A_MACHINE_PRODUCT_STATUS2);
            //await context.PostAsync(machineStatus);
            context.Wait(MessageReceived);
        }
        [LuisIntent("补货请求")]
        public async Task Reload_return(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(A_SUPPLY_PRODUCT);
            context.Wait(MessageReceived);

        }

        [LuisIntent("特定贩售机状态")]
        public async Task IncreaseTemp(IDialogContext context, LuisResult result)
        {
            Random rd2 = new Random();
            int number3 = rd2.Next(1,100);
            string num3 = Convert.ToString(number3);            
            //string strRet = result.Entities[0].Entity;
            machineNo = result.Entities[0].Entity;
            await context.PostAsync(machineNo + MACHINE_STATUS + num3 + MACHINE_STATUS2);
            context.Wait(MessageReceived);
            machineNo = "1234";
        }

        [LuisIntent("贩售机位置")]
        public async Task vendingMachineLoation(IDialogContext context, LuisResult result)
        {
            //machineNo = result.Entities[0].Entity;
            await context.PostAsync("0127号" + A_MACHINE_LOCATION);
            context.Wait(MessageReceived);
            //machineNo = "1234";
        }
        [LuisIntent("整体状态")]
        public async Task wholeStatusQuery(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("0127号" + A_MACHINE_POWERPI);
            context.Wait(MessageReceived);
        }
        [LuisIntent("感谢")]
        public async Task Thanks_return(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(RE_CONFIRM);
            context.Wait(MessageReceived);
        }
        [LuisIntent("贩卖机的温度")]
        public async Task tempuratureQuery(IDialogContext context, LuisResult result)
        {
            string machineTemperStatus = HttpGet(TEMPER_QUERY_URL, "");
            MachineTemper machineTemper = (MachineTemper)JsonConvert.DeserializeObject(machineTemperStatus, typeof(MachineTemper));
            if (true == machineTemper.success)
            {
                //List<Goodsdata> shortageGoods = new List<Goodsdata>();
                List<string> resultMachineTmp = new List<string>();
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string temperMode = "关闭";
                for (int i = 0; i < machineTemper.areadatas.Length; i++)
                {
                    switch (machineTemper.areadatas[i].temperaturemode)
                    {
                        case 0:
                            temperMode = "关闭";
                        break;
                        case 1:
                            temperMode = "加热";
                        break;
                        case 2:
                            temperMode = "制冷";
                        break;
                        default:
                        break;
                    }
                    tempResult = machineTemper.areadatas[i].warmareaname + "温度:" + machineTemper.areadatas[i].temperature.ToString() + 
                        " 温度模式:" + temperMode;
                    MyStringBuilder.Append(tempResult);
                    MyStringBuilder.AppendLine("");
                }
                string finalStatusResult = MyStringBuilder.ToString();
                await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + "温度状态如下: \r\n"+ finalStatusResult);
            }
            else
            {
                await context.PostAsync("无法查询到贩卖机状态，请稍后重试");
            }
            //await context.PostAsync("贩卖机的温度状态如下：");
            context.Wait(MessageReceived);
        }

        [LuisIntent("目前温度模式")]
        public async Task tempuratureModeQuery(IDialogContext context, LuisResult result)
        {
            string machineTemperStatus = HttpGet(TEMPER_QUERY_URL, "");
            MachineTemper machineTemper = (MachineTemper)JsonConvert.DeserializeObject(machineTemperStatus, typeof(MachineTemper));
            if (true == machineTemper.success)
            {
                //List<Goodsdata> shortageGoods = new List<Goodsdata>();
                List<string> resultMachineTmp = new List<string>();
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string temperMode = "关闭";
                for (int i = 0; i < machineTemper.areadatas.Length; i++)
                {
                    switch (machineTemper.areadatas[i].temperaturemode)
                    {
                        case 0:
                            temperMode = "关闭";
                            break;
                        case 1:
                            temperMode = "加热";
                            break;
                        case 2:
                            temperMode = "制冷";
                            break;
                        default:
                            break;
                    }
                    tempResult = machineTemper.areadatas[i].warmareaname + "模式:" + temperMode;
                    MyStringBuilder.Append(tempResult);
                    MyStringBuilder.AppendLine("");
                }
                string finalStatusResult = MyStringBuilder.ToString();
                await context.PostAsync("0127" + A_MACHINE_PRODUCT_STATUS + "温度模式如下: \r\n" + finalStatusResult);
            }
            else
            {
                await context.PostAsync("无法查询到贩卖机状态，请稍后重试");
            }
            //await context.PostAsync("贩卖机的温度状态如下：");
            context.Wait(MessageReceived);
        }
    }
}