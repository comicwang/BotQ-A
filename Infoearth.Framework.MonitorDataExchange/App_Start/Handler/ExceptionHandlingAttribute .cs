using Infoearth.Util;
using Infoearth.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace Infoearth.Framework.QABotRestApi
{
    /// <summary>
    /// 异常日志记录
    /// </summary>
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 进入异常
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            WriteLog(context);
            base.OnException(context);
        }

        /// <summary>
        /// 写入日志（log4net）
        /// </summary>
        /// <param name="context">提供使用</param>
        private void WriteLog(HttpActionExecutedContext context)
        {
            if (context == null)
                return;
            Exception Error = context.Exception;
            LogMessage logMessage = new LogMessage();
            logMessage.OperationTime = DateTime.Now;
            logMessage.Url = HttpContext.Current.Request.RawUrl;
            logMessage.Class = context.ActionContext.ToString();
            logMessage.Ip = Net.Ip;
            logMessage.Host = Net.Host;
            logMessage.Browser = Net.Browser;
            Log.WriteLog(Newtonsoft.Json.JsonConvert.SerializeObject(logMessage), Error);
        }
    }
}