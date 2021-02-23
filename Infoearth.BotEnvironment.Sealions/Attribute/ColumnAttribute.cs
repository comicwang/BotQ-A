using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 忽略映射
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public bool Key { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        public bool Answer { get; set; }
    }
}
