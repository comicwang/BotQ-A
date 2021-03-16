using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions
{
    /*===================================================
	 * 类名称: Model
	 * 类描述:
	 * 创建人: wangchong
	 * 创建时间: 2021/2/20 15:58:29
	 * 修改人:
	 * 修改时间:
	 * 版本： @version 1.0
	 =====================================================*/
    /// <summary>
    /// ik分词结果对象
    /// </summary>
    public class ik
    {
        public List<tokens> tokens { get; set; }
    }
    public class tokens
    {
        public string token { get; set; }
        public int start_offset { get; set; }
        public int end_offset { get; set; }
        public string type { get; set; }
        public int position { get; set; }
    }

    /// <summary>
    /// ES返回对象集合
    /// </summary>
    public class ElasticModel<T> where T: ESBase
    {
        public ElasticModel()
        {
            this.list = new List<T>();
        }
        public int hits { get; set; }
        public int took { get; set; }
        public List<T> list { get; set; }
    }

    public class PageData
    {
        /// <summary>
        /// 页数
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
    }

    public class ESBase
    {
        [Ignore]
        public string highlight { get; set; }
        [Ignore]
        public string _id { get; set; }
        [Ignore]
        public double _score { get; set; }
        [Ignore]
        public string _pid { get; set; }
    }

    public class QueryModel
    {
        public string key { get; set; }

        public string field { get; set; }

        public QueryRelation queryRelation { get; set; }
    }

    public class QueryContext
    {
        public QueryModel queryModel { get; set; }

        public QueryRelation queryRelation { get; set; }
    }

    public enum QueryRelation
    {
        And,
        Or
    }

    public class Model
    {
        public Model() { }

        public string id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public bool sex { get; set; }
        public DateTime birthday { get; set; }
        public string intro { get; set; }
    }
}
