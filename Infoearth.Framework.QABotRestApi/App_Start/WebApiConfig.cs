using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Infoearth.Framework.QABotRestApi
{
    /// <summary>
    /// weiapi注册
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 开始注册
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            // Web API configuration and services
            config.Filters.Add(new ExceptionHandlingAttribute());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
