using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions.QA_Bot
{
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

        private ElasticSearchContext _esEngine = ElasticSearchContext.Intance;
        /// <summary>
        /// 执行器引擎
        /// </summary>
        public ElasticSearchContext ESEngine { get => _esEngine; }


        public QAContextBase()
        {
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
                var indexAttr = propertys[j].GetCustomAttributes<IndexAttribute>();
                if (indexAttr.Any())
                {
                    string indexName = indexAttr.FirstOrDefault().Name;
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        Index = indexName;
                    }
                    Index = propertys[j].Name;
                }
                var indexTypeAttr = propertys[j].GetCustomAttributes<IndexTypeAttribute>();
                if (indexTypeAttr.Any())
                {
                    string indexTypeName = indexTypeAttr.FirstOrDefault().Name;
                    if (!string.IsNullOrEmpty(indexTypeName))
                    {
                        IndexType = indexTypeName;
                    }
                    IndexType = propertys[j].Name;
                }
            }
            if (string.IsNullOrEmpty(_key))
                throw new InvalidOperationException("所创建的继承类未指定QA的标题列");
            if (string.IsNullOrEmpty(_answer))
                throw new InvalidOperationException("所创建的继承类未指定QA的答案列");
            if (string.IsNullOrEmpty(_index))
                throw new InvalidOperationException("所创建的继承类未指定ES的索引名称");
            if (string.IsNullOrEmpty(_indexType))
                throw new InvalidOperationException("所创建的继承类未指定ES的索引类型名称");

            _esEngine.SetESParam(_index, _indexType);
        }

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
    }
}
