using Infoearth.BotEnvironment.Sealions.QA_Bot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions
{
    /// <summary>
    /// 问答智能机器人（web版）
    /// </summary>
    public class QA_Bot_Html<T> : IQA_Bot where T:BaseHighlight
    {
        private string _botName = "小象";
        private string _contact = "小红（QQ123456）";
        private string _key = string.Empty;
        private string _answer = string.Empty;
        private string _hightlight = string.Empty;
        private string _index = string.Empty;
        private string _indexType = string.Empty;
        ElasticSearchContext elasticSearchHelper = ElasticSearchContext.Intance;

        public QA_Bot_Html()
        {
            PropertyInfo[] propertys = typeof(T).GetProperties();
            for (int j = 0; j < propertys.Length; j++)
            {
                var exportAttr = propertys[j].GetCustomAttributes<ColumnAttribute>();
                if (exportAttr.Any(t => t.Key))
                {
                    _key = propertys[j].Name;
                }
                if (exportAttr.Any(t => t.Answer))
                {
                    _answer = propertys[j].Name;
                }
            }
            //获取Index和IndexType
            string[] strs = ConfigurationManager.AppSettings["ElasticSearchConnection"].Split(' ');
            if (strs.Length < 4)
                throw new InvalidOperationException("ES配置错误");
            _index = strs[2];
            _indexType = strs[3];
            string name = ConfigurationManager.AppSettings["BotName"];
            if (!string.IsNullOrEmpty(name))
                _botName = name;
            string contact = ConfigurationManager.AppSettings["SystemContact"];
            if (!string.IsNullOrEmpty(contact))
                _contact = contact;
        }

        public List<T> TargetResults
        {
            get;
            private set;
        }

        public string AnswerQuestion(string key)
        {
            string answerWord = string.Empty;
            //1.对标题进行全词匹配
            var result = elasticSearchHelper.Search<T>(_index, _indexType, key, _key);
            bool suceess = DealQuestion(result,out answerWord);
            if (suceess == false)
            {
                //2.对标题进行单词匹配
                result = elasticSearchHelper.Search<T>(_index, _indexType, key, _key, PlainElastic.Net.Operator.OR);
                suceess = DealQuestion(result,out answerWord);
                if (suceess == false)
                {
                    //3.对所有属性进行全词匹配
                    result = elasticSearchHelper.Search<T>(_index, _indexType, key);
                    suceess = DealQuestion(result, out answerWord);
                    if (suceess == false)
                    {
                        //4.对所有属性进行单词匹配
                        result = elasticSearchHelper.Search<T>(_index, _indexType, key, null, PlainElastic.Net.Operator.OR);
                        suceess = DealQuestion(result,out answerWord);
                        if (suceess == false)
                        {
                            answerWord = $"抱歉，您的问题{_botName}处理不了，请尝试更换查询问题或者联系管理员【{_contact}】";
                        }
                    }
                }
            }
            TargetResults = result.list;
            return answerWord;
        }

        private bool DealQuestion(ElasticModel<T> result, out string answerWord)
        {
            PropertyInfo propKey = typeof(T).GetProperties().FirstOrDefault(s => s.Name == _key);
            PropertyInfo propAnswer = typeof(T).GetProperties().FirstOrDefault(s => s.Name == _answer);
            if (result.list.Count == 0)
            {
                answerWord = string.Empty;
                return false;
            }
            else if (result.list.Count == 1)
            {
                var key = result.list[0].highlight;//propKey.GetValue(result.list[0]);
                var answer = propAnswer.GetValue(result.list[0]);
                answerWord = $"{key}<br/>";
                answerWord += answer;
            }
            else
            {
                answerWord = $"{_botName}为您找到多个问题，请问您想问的是哪个问题？<br/>";
                foreach (var item in result.list)
                {
                    answerWord += $"<a onclick='GetDetail(this)'>{item.highlight}</a><br/>";
                }
            }
            return true;
        }

        public string GetFirstWord()
        {
            string result = $"您好，我是您的智能助手{_botName}，请问有什么能够帮助你？<br/>";

            result += "您可以尝试提问:<br/>";

            var suggestQuestion = elasticSearchHelper.Search<T>(_index, _indexType, string.Empty, _key, PlainElastic.Net.Operator.OR, new PageData() { PageSize = 3 });
            PropertyInfo propKey = typeof(T).GetProperties().FirstOrDefault(s => s.Name == _key);
            foreach (var item in suggestQuestion.list)
            {
                result += $"<a onclick='GetDetail(this)'>{propKey.GetValue(item)}</a><br/>";
            }

            return result;
        }

        public string GetDetail(string key)
        {
            string answerWord = string.Empty;
            var result = elasticSearchHelper.Search<T>(_index, _indexType, key, _key);
            DealQuestion(result, out answerWord);
            return answerWord;
        }
    }
}
