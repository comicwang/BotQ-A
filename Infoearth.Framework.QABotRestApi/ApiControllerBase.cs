using Infoearth.Util;
using Infoearth.Util.WebControl;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Infoearth.Framework.QABotRestApi
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2013-2016 
    /// 创建人：佘赐雄
    /// 日 期：2015.11.9 10:45
    /// 描 述：基控Api制器
    /// </summary>
    [RequestAuthorize]
    public abstract class ApiControllerBase : ApiController
    {
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public virtual string ToJsonResult(object data)
        {
            return data.ToJson();
        }
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public virtual WebApiResult Success(string message)
        {
            return new WebApiResult { type = ResultType.success, message = message };
        }
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public virtual WebApiResult Success(string message, object data)
        {
            return new WebApiResult { type = ResultType.success, message = message, resultdata = data };
        }
        /// <summary>
        /// 返回失败消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public virtual WebApiResult Error(string message)
        {
            return new WebApiResult { type = ResultType.error, message = message };
        }
    }

    /// <summary>
    /// WebAPi结果
    /// </summary>
    public class WebApiResult
    {
        /// <summary>
        /// 获取操作结果类型
        /// </summary>
        public ResultType type { get; set; }

        /// <summary>
        /// 获取操作结果编码
        /// </summary>
        public int resultcode { get; set; }

        /// <summary>
        /// 获取消息内容
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 获取返回数据
        /// </summary>
        public object resultdata { get; set; }

        /// <summary>
        /// 附属信息
        /// </summary>
        public object otherinfo { get; set; }
    }
}
