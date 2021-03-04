using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions
{
    /// <summary>
    /// 指定实体的QA属性,这里是约定
    /// 其中Key表示QA的标题，Answer表示QA的答案，是集成ESBase基类必须加的2大类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QAAttribute : Attribute
    {
        /// <summary>
        /// 忽略映射
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public bool IsKey { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        public bool IsAnswer { get; set; }
    }
}
