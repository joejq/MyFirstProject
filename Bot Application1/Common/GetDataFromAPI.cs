using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CalligenceBot.Common
{
    public static class GetDataFromAPI
    {
        public static HttpResponseMessage response;

        public static HttpClient client;

        /// <summary>
        /// 请求获取的数据，并转为确定对象
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="parameter">请求参数</param>
        /// <returns></returns>
        public static async Task<JObject> DataToObject(string url, string parameter)
        {
            JObject jObject = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    url +=  (string.IsNullOrEmpty(parameter)) ? "" : "?" + parameter;
                    response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //Console.WriteLine("请求成功");
                        var content = await response.Content.ReadAsStringAsync();
                        try
                        {
                            jObject = JObject.Parse(content);
                        } catch(Exception ex) { } //return null
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return jObject;
        }

        /// <summary>
        /// 请求获取数据，并转化为确定对象集合
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="parameter">请求参数</param>
        /// <returns></returns>
        public static async Task<JArray> DataToJArray(string url, string parameter)
        {
            JArray jArray = null;
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    url += (string.IsNullOrEmpty(parameter)) ? "" : "?" + parameter;
                    response = client.GetAsync(url).Result;  // Blocking call（阻塞调用）! 
                    if (response.IsSuccessStatusCode)
                    {
                        //Console.WriteLine("请求成功");
                        var content = await response.Content.ReadAsStringAsync();
                        try
                        {
                            jArray = JArray.Parse(content);
                        } catch (Exception ex) { } //return null
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return jArray;
        }

        /// <summary>
        /// 请求获取字符串数据
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="parameter">请求参数</param>
        /// <returns></returns>
        public static async Task<string> DataToString(string url, string parameter)
        {
            string str = string.Empty;
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    url += (string.IsNullOrEmpty(parameter)) ? "" : "?" + parameter;
                    response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        str = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return str;
        }

        public static async Task<string> PostJson(string url, string parameter, string content)
        {
            string str = string.Empty;
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    LearnMore learnMore = new LearnMore()
                    {
                        Lcontent = content
                    };
                    var jObect = JsonConvert.SerializeObject(learnMore, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    url += (string.IsNullOrEmpty(parameter)) ? "" : "?" + parameter;
                    response = client.PostAsJsonAsync(url, jObect).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        str = await response.Content.ReadAsStringAsync();
                        //jObject = JsonConvert.DeserializeObject(content) as JObject;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return str;
        }
        
    }
    public class LearnMore
    {
        public LearnMore()
        {
            Subtime = DateTime.Now;
            Lok = 0;
            //Delflag = 0;
        }
        public int ID { get; set; }
        public string Lcontent { get; set; }
        public Nullable<short> Lok { get; set; }
        public Nullable<System.DateTime> Subtime { get; set; }
    }
}