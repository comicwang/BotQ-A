using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions
{
    /// <summary>
    /// 指定实体的ES属性,这里是约定
    /// 其中Name表示ES的索引名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// ES的索引列标识
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// ES的索引类型列标识
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 表示ES的父节点名称
        /// </summary>
        public string ParentName { get; set; }
    }

    /// <summary>
    /// 指定实体的ES属性不加入ES映射,这里是约定
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
    }
}
