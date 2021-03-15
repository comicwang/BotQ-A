using PlainElastic.Net;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions.QA_Bot
{
    /// <summary>
    /// QA上下文
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QAContextBase<T> where T : ESBase
    {
        private string _key;
        /// <summary>
        /// 获取QA标题列名称
        /// </summary>
        public string Key { get => _key; private set => _key = value; }

        private string _answer;
        /// <summary>
        /// 获取QA答案列名称
        /// </summary>
        public string Answer { get => _answer; private set => _answer = value; }

        private string _model;
        /// <summary>
        /// 答案所属模块
        /// </summary>
        public string AnswerModel { get => _model; private set => _model = value; }

        private string _index;
        /// <summary>
        /// 获取ES索引名称
        /// </summary>

        public string Index { get => _index; private set => _index = value; }

        private string _indexType;
        /// <summary>
        /// 获取ES索引类型名称
        /// </summary>
        public string IndexType { get => _indexType; private set => _indexType = value; }

        private string _parentType;
        /// <summary>
        /// 获取ES索引类型名称
        /// </summary>
        public string ParentType { get => _parentType; private set => _parentType = value; }

        private ElasticSearchContext _esEngine = ElasticSearchContext.Intance;
        /// <summary>
        /// 执行器引擎
        /// </summary>
        public ElasticSearchContext ESEngine { get => _esEngine; }


        public QAContextBase()
        {
            var index= typeof(T).GetCustomAttributes<IndexAttribute>();
            if(index==null||index.Count()==0)
                throw new InvalidOperationException("所创建的继承类未指定ES索引特性");
            IndexAttribute indexAttribute = index.FirstOrDefault();
            _index = indexAttribute.IndexName;
            _indexType = indexAttribute.TypeName;
            _parentType = indexAttribute.ParentName;

            PropertyInfo[] propertys = typeof(T).GetProperties();
            for (int j = 0; j < propertys.Length; j++)
            {
                var exportAttr = propertys[j].GetCustomAttributes<QAAttribute>();
                if (exportAttr.Any(t => t.IsKey))
                {
                    Key = propertys[j].Name;
                }
                if (exportAttr.Any(t => t.IsAnswer))
                {
                    Answer = propertys[j].Name;
                }
                if (exportAttr.Any(t => t.IsModel))
                {
                    AnswerModel = propertys[j].Name;
                }              
            }
            //if (string.IsNullOrEmpty(_key))
            //    throw new InvalidOperationException("所创建的继承类未指定QA的标题列");
            //if (string.IsNullOrEmpty(_answer))
            //    throw new InvalidOperationException("所创建的继承类未指定QA的答案列");
            if (string.IsNullOrEmpty(_index))
                throw new InvalidOperationException("所创建的继承类未指定ES的索引名称");
            if (string.IsNullOrEmpty(_indexType))
                throw new InvalidOperationException("所创建的继承类未指定ES的索引类型名称");
        }

        #region 上下文方法封装
        /// <summary>
        /// 插入数据文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jsonDocument"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IndexResult Insert(T jsonDocument, string parent = null)
        {
            return _esEngine.Index<T>(_index, _indexType,jsonDocument, parent);
        }


        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T SearchById(string id)
        {
            return _esEngine.SearchById<T>(_index, _indexType, id);
        }

        /// <summary>
        /// 根据主键集合查询
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public T[] SearchByIds(List<string> ids)
        {
            return _esEngine.SearchByIds<T>(_index, _indexType, ids);
        }

        /// <summary>
        /// 分页排序查询
        /// </summary>
        /// <param name="pageData"></param>
        /// <param name="orderField"></param>
        /// <returns></returns>
        public ElasticModel<T> SearchOrder(PageData pageData, string orderField)
        {
            return _esEngine.SearchOrder<T>(_index, _indexType, pageData, orderField);
        }

        public ElasticModel<T> SearchIk(PageData pageData, params QueryContext[] queryContexts)
        {
            return _esEngine.SearchIk<T>(_index, _indexType, pageData, queryContexts);
        }

        //全文检索，单个字段或者多字段 或关系
        //字段intro 包含词组key中的任意一个单词
        /// <summary>
        /// 搜索索引信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="intro">搜索属性名称（默认不填为所有）</param>
        /// <param name="opera">分词匹配模式（默认不填为所有词匹配）</param>
        /// <returns></returns>
        public ElasticModel<T> Search(string key, string intro = null, Operator opera = Operator.AND, PageData pageData = null, string model = null, string modelintro = null)
        {
            return _esEngine.Search<T>(_index, _indexType, key, intro, opera, pageData, model, modelintro);
        }

        /// <summary>
        /// 删除某条数据
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeleteResult Delete(string id)
        {
            return _esEngine.Delete(_index, _indexType, id);
        }


        #endregion


        /// <summary>
        /// 获取标题的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetKeyWord(T obj)
        {
            PropertyInfo propKey = typeof(T).GetProperties().FirstOrDefault(s => s.Name == _key);
            return propKey.GetValue(obj)?.ToString();
        }

        /// <summary>
        /// 获取答案值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetAnswerWord(T obj)
        {
            PropertyInfo propKey = typeof(T).GetProperties().FirstOrDefault(s => s.Name == _answer);
            return propKey.GetValue(obj)?.ToString();
        }

        /// <summary>
        /// 获取答案模块值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetModelWord(T obj)
        {
            PropertyInfo propKey = typeof(T).GetProperties().FirstOrDefault(s => s.Name == _model);
            return propKey.GetValue(obj)?.ToString();
        }
    }
}
