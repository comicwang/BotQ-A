using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Infoearth.Framework.QABotRestApi
{
    /// <summary>
    /// 接口Token验证属性
    /// </summary>
    public class RequestAuthorizeAttribute : AuthorizeAttribute
    {
        private bool IsResultWrap = false;
        private string auth = System.Configuration.ConfigurationManager.AppSettings["tokenAuth"];

        /// <summary>
        /// 开始验证
        /// </summary>
        /// <param name="actionContext"></param>
        //重写基类的验证方式，加入我们自定义的Ticket验证//验证WebApi的
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket

            var attributesA = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
            var attributesC = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
            bool isAnonymousA = attributesA.Any(a => a is AllowAnonymousAttribute);
            bool isAnonymousC = attributesC.Any(a => a is AllowAnonymousAttribute);
            if (isAnonymousA || isAnonymousC || (!string.IsNullOrEmpty(auth) && auth.ToLower() == "false"))
            {
                base.IsAuthorized(actionContext);
            }
            else
            {

                var authorization = actionContext.Request.Headers.Authorization;
                //如果取不到身份验证信息，并且不允许匿名访问，则返回未验证401
                if (authorization == null)
                {
                    var headers = actionContext.Request.Headers.ToList();
                    var auth = headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault();
                    if (auth != null && auth.Count() > 0)
                    {
                        if (ValidateTicket(auth.ToList()[0]))
                        {
                            base.IsAuthorized(actionContext);
                        }
                        else
                        {
                            HandleUnauthorizedRequest(actionContext);
                        }
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);

                    }

                }
                else
                {
                    //解密用户ticket,并校验用户名密码是否匹配
                    string encryptTicket = "";
                    if (authorization != null)
                    {
                        encryptTicket = authorization.ToString();
                    }
                    if (authorization.Parameter != null)
                    {
                        encryptTicket = authorization.Parameter;
                    }
                    if (ValidateTicket(encryptTicket))
                    {
                        base.IsAuthorized(actionContext);
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
            }
        }

        /// <summary>
        /// 处理未验证通过
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            //var response = actionContext.Response;
            //response.StatusCode = HttpStatusCode.Forbidden;
            //AjaxResponse errorJson = new AjaxResponse(new ErrorInfo() { Code = (int)HttpStatusCode.Forbidden, Message = "服务器拒绝访问, token值已过期, 请重新登陆获取token值" });
            //response.Content = new ObjectContent<AjaxResponse>(errorJson, GlobalConfiguration.Configuration.Formatters.JsonFormatter);
        }

        /// <summary>
        /// 校验ticket是否有效
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private bool ValidateTicket(string ticket)
        {
            //ticket = HttpUtility.UrlDecode(ticket).Split(',')[0];
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            HttpClient httpClient = new HttpClient(handler);
            string url = System.Configuration.ConfigurationManager.AppSettings["SSOUrl"];
            httpClient.BaseAddress = new Uri(url);
            //await异步等待回应
            var response = httpClient.GetAsync("api/SSOAuth/CheckTicket/" + ticket).Result;
            //确保HTTP成功状态值
            response.EnsureSuccessStatusCode();
            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
            var result = response.Content.ReadAsStringAsync().Result;
            if (!IsResultWrap)
            {
                return bool.Parse(result);
            }
            var json = JsonConvert.DeserializeObject<dynamic>(result);
            bool success = json.Success;
            if (success)
            {
                return (bool)json.Result;
            }
            else
            {
                return false;
            }
        }
    }
}