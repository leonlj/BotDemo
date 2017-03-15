using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CognitiveServicesAI
{
    //调用方法
    //URL = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=C1vO0m46h0XAE8HBO_v3AYtgCCkBCCpRSfCQjeOpYj9CcVJNHRIn4DeXlSCyXhKz9egEDpcA3PkQWF6MQHgUmPMN_pBk4d8v3JOzKPW5AFRIimr_fM77YV6XhlurQptSDEUcABAOPF&media_id=8eI9y6_iBHbfHwHfQFx1jPhLSRhAUy0NRm_yZEh23cRecmwk5Pyr_-IBBkuv8sYN";
    //string token = new BaiduSpeechRecognition().getStrText("zh", URL, "amr", "8000");

    public class BaiduSpeechRecognition
    {
        public static string API_id = "7328169";
        public static string API_record = null;
        public static string API_record_format = null;
        public static string API_record_HZ = null;
        public static string API_key = "lIHBamxGQ4fKDxRuXvGF6Uas";
        public static string API_secret_key = "4bd695165d883a5127572b8e25c8166b";
        public static string API_language = null;
        public static string API_access_token = null;
        public static string strJSON = "";

        private string getStrAccess()
        {
            //方法参数说明:
            //para_API_key:API_key(你的KEY)
            //para_API_secret_key(你的SECRRET_KEY)

            //方法返回值说明:
            //百度认证口令码,access_token
            string access_html = null;
            string access_token = null;
            string getAccessUrl = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials" +
           "&client_id=" + API_key + "&client_secret=" + API_secret_key;
            try
            {
                HttpWebRequest getAccessRequest = WebRequest.Create(getAccessUrl) as HttpWebRequest;
                //getAccessRequest.Proxy = null;
                getAccessRequest.ContentType = "multipart/form-data";
                getAccessRequest.Accept = "*/*";
                getAccessRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";
                getAccessRequest.Timeout = 30000;//30秒连接不成功就中断 
                getAccessRequest.Method = "post";

                HttpWebResponse response = getAccessRequest.GetResponse() as HttpWebResponse;
                using (StreamReader strHttpComback = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    access_html = strHttpComback.ReadToEnd();
                    string result = access_html.Split(',')[0];
                    result = result.Split(':')[1];
                    access_token = result.Replace("\"", "");
                }
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
            return access_token;
        }

        public string getStrText(string para_API_language, string para_record_URL, string para_format, string para_Hz)
        {
            //该方法返回值:
            //该方法执行正确返回值是语音翻译的文本,错误是错误号,可以去看百度语音文档,查看对应错误
            string strText = null;
            string error = null;

            byte[] voice = Download(para_record_URL);
            string token = getStrAccess();
            string getTextUrl = "http://vop.baidu.com/server_api?lan=" + para_API_language + "&cuid=" + API_id + "&token=" + token;
            HttpWebRequest getTextRequst = WebRequest.Create(getTextUrl) as HttpWebRequest;

            getTextRequst.ContentType = "audio /" + para_format + ";rate=" + para_Hz;
            //getTextRequst.ContentLength = fi.Length;
            getTextRequst.ContentLength = voice.Length;
            getTextRequst.Method = "post";
            getTextRequst.Accept = "*/*";
            getTextRequst.KeepAlive = true;
            getTextRequst.Timeout = 30000;//30秒连接不成功就中断 
            using (Stream writeStream = getTextRequst.GetRequestStream())
            {
                writeStream.Write(voice, 0, voice.Length);
            }

            HttpWebResponse getTextResponse = getTextRequst.GetResponse() as HttpWebResponse;
            using (StreamReader strHttpText = new StreamReader(getTextResponse.GetResponseStream(), Encoding.UTF8))
            {
                strJSON = strHttpText.ReadToEnd();
                string result = strJSON.Split(',').Where(sn => sn.StartsWith("\"result\"")).ToList()[0];
                result = result.Split(':')[1];
                strText = result.Replace("\"", "").Trim('[', ']');
            }
            return strText;
        }

        public static byte[] Download(string uri)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            byte[] bArr = new byte[response.ContentLength];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            //while (size > 0)
            //{
            //    size = responseStream.Read(bArr, 0, (int)bArr.Length);
            //}
            return bArr;
        }
    }
}
