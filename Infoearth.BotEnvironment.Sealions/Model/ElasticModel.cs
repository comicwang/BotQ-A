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
    public class ElasticModel<T> where T: BaseHighlight
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
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 0;
    }

    public class BaseHighlight
    {
        [Column(Ignore = true)]
        public string highlight { get; set; }
        [Column(Ignore = true)]
        public string _id { get; set; }
        [Column(Ignore = true)]
        public double _score { get; set; }
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
