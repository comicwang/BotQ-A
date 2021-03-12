using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infoearth.Framework.QABotRestApi
{
    /// <summary>
    /// 2018-3-16 by wangchong
    /// 功能： 后台请求Webapi帮助类
    /// </summary>
    public class WebapiRequestHelp
    {
        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T">请求返回类型</typeparam>
        /// <param name="uri">请求地址ip和port</param>
        /// <param name="url">请求地址</param>
        /// <param name="request">请求参数</param>
        /// <param name="token">请求参数</param>
        /// <returns>转换类型后的数据</returns>
        public static T PostApi<T>(string uri, string url, object request, string token = null)
        {
            try
            {
                if (token == null)
                    token = HttpContext.Current.Request.Headers["Authorization"];
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.BaseAddress = new Uri(uri);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                //await异步等待回应
                var response = httpClient.PostAsync(url, contentJson).Result;
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>((response.Content.ReadAsStringAsync().Result));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">请求返回类型</typeparam>
        /// <param name="uri">请求地址ip和port</param>
        /// <param name="url">请求地址</param>
        /// <param name="request">请求参数</param>
        /// <param name="token">请求参数</param>
        /// <returns>Json字符串</returns>
        public static T GetApi<T>(string uri, string url, string request, string token = null)
        {
            try
            {
                if (token == null)
                    token = HttpContext.Current.Request.Headers["Authorization"];
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.BaseAddress = new Uri(uri);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


                //await异步等待回应
                var response = httpClient.GetAsync(url + (string.IsNullOrEmpty(request) ? string.Empty : "/" + request)).Result;
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                string json = response.Content.ReadAsStringAsync().Result;
                if (json == null)
                {
                    return default(T);
                }
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Post请求,返回Json字符串
        /// </summary>
        /// <param name="uri">请求地址ip和port</param>
        /// <param name="url">请求地址</param>
        /// <param name="request">请求参数</param>
        /// <param name="token">请求参数</param>
        /// <returns></returns>
        public static string Post(string uri, string url, object request, string token = null)
        {
            try
            {
                if (token == null)
                    token = HttpContext.Current.Request.Headers["Authorization"];
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.BaseAddress = new Uri(uri);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                //await异步等待回应
                var response = httpClient.PostAsync(url, contentJson).Result;
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get请求,返回Json字符串
        /// </summary>
        /// <param name="uri">请求地址ip和port</param>
        /// <param name="url">请求地址</param>
        /// <param name="request">请求参数</param>
        /// <param name="token">请求参数</param>
        /// <returns>Json字符串</returns>
        public static string GetApi(string uri, string url, string request, string token = null)
        {
            try
            {
                if (token == null)
                    token = HttpContext.Current.Request.Headers["Authorization"];
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.BaseAddress = new Uri(uri);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                //await异步等待回应
                var response = httpClient.GetAsync(url + (string.IsNullOrEmpty(request) ? string.Empty : "/" + request)).Result;
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
