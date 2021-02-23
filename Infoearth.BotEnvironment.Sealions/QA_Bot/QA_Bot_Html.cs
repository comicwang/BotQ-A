using Infoearth.BotEnvironment.Sealions.QA_Bot;
using System;
using System.Collections.Generic;
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
        private string botName = "小象";
        private string system = "小红（QQ123456）";
        private string _key = string.Empty;
        private string _answer = string.Empty;
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
            var result = elasticSearchHelper.Search<T>("test", "monitor", key, _key);
            bool suceess = DealQuestion(result,out answerWord);
            if (suceess == false)
            {
                //2.对标题进行单词匹配
                result = elasticSearchHelper.Search<T>("test", "monitor", key, _key, PlainElastic.Net.Operator.OR);
                suceess = DealQuestion(result,out answerWord);
                if (suceess == false)
                {
                    //3.对所有属性进行全词匹配
                    result = elasticSearchHelper.Search<T>("test", "monitor", key);
                    suceess = DealQuestion(result, out answerWord);
                    if (suceess == false)
                    {
                        //4.对所有属性进行单词匹配
                        result = elasticSearchHelper.Search<T>("test", "monitor", key, null, PlainElastic.Net.Operator.OR);
                        suceess = DealQuestion(result,out answerWord);
                        if (suceess == false)
                        {
                            answerWord = $"抱歉，您的问题{botName}处理不了，请尝试更换查询问题或者联系管理员【{system}】";
                        }
                    }
                }
            }
            TargetResults = result.list;
            return answerWord;
        }

        private bool DealQuestion(ElasticModel<T> result,out string answerWord)
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
                var key = propKey.GetValue(result.list[0]);
                var answer = propAnswer.GetValue(result.list[0]);
                answerWord = $"{key}<br/>";
                answerWord += answer;
            }
            else
            {
                answerWord= $"{botName}为您找到多个问题，请问你想问的是哪个问题？<br/>请回复1,2,3..,【没有找到你要问的问题回复0，重新提问】<br/>";
                int index = 1;
                foreach (var item in result.list)
                {
                    answerWord += $"{index}:{propKey.GetValue(item)}<br/>";                   
                    index++;
                }               
            }
            return true;
        }

        public string GetFirstWord()
        {
            string result = $"您好，我是您的智能助手{botName}，请问有什么能够帮助你？<br/>";

            result += "您可以尝试提问:<br/>";

            result += "怎么新增监测点信息，怎么挂载设备到监测点<br/>";

            return result;
        }
    }
}
