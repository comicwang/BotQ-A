using Infoearth.BotEnvironment.Sealions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infoearth.Framework.QABotRestApi
{
    /// <summary>
    /// 分页信息实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T> where T : class
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageData pageModel { get; set; }

        /// <summary>
        /// 分页数据信息
        /// </summary>
        public List<T> data { get; set; }
    }
}