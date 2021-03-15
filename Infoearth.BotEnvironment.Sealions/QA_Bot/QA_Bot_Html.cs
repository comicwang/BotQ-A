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
    public class QA_Bot_Html<T> : QAContextBase<T>,IQA_Bot where T:ESBase
    {
        private string _botName = DefaultSetter.RobotName;
        private string _contact = DefaultSetter.ContactName;

        public List<T> TargetResults
        {
            get;
            private set;
        }

        public string AnswerQuestion(string key)
        {
            string answerWord = string.Empty;
            //1.对标题进行全词匹配
            var result = this.Search(key, Key);
            bool suceess = DealQuestion(result,out answerWord);
            if (suceess == false)
            {
                //2.对标题进行单词匹配
                result = this.Search(key, Key, PlainElastic.Net.Operator.OR);
                suceess = DealQuestion(result,out answerWord);
                if (suceess == false)
                {
                    //3.对所有属性进行全词匹配
                    result = this.Search(key);
                    suceess = DealQuestion(result, out answerWord);
                    if (suceess == false)
                    {
                        //4.对所有属性进行单词匹配
                        //result = elasticSearchHelper.Search<T>(_index, _indexType, key, null, PlainElastic.Net.Operator.OR);
                        //suceess = DealQuestion(result,out answerWord);
                        //if (suceess == false)
                        //{
                        //    answerWord = $"抱歉，您的问题{_botName}处理不了，请尝试更换查询问题或者联系管理员【{_contact}】";
                        //}
                        answerWord = $"抱歉，您的问题{_botName}处理不了，请尝试更换查询问题或者联系管理员【{_contact}】";
                    }
                }
            }
            TargetResults = result.list;
            return answerWord;
        }

        private bool DealQuestion(ElasticModel<T> result, out string answerWord)
        {
            if (result.list.Count == 0)
            {
                answerWord = string.Empty;
                return false;
            }
            else if (result.list.Count == 1)
            {
                var key = result.list[0].highlight;//propKey.GetValue(result.list[0]);
                var answer = GetAnswerWord(result.list[0]);
                answerWord = $"{key}<br/>";
                answerWord += answer;
            }
            else
            {
                answerWord = $"{_botName}为您找到多个问题，请问您想问的是哪个问题？<br/>";
                foreach (var item in result.list)
                {
                    answerWord += $"<a onclick='GetDetail(this)'>{item.highlight ?? GetKeyWord(item)}</a><br/>";
                }
            }
            return true;
        }

        public string GetFirstWord()
        {
            string result = $"您好，我是您的智能助手{_botName}，请问有什么能够帮助你？<br/>";

            result += "您可以尝试提问:<br/>";

            var suggestQuestion = this.Search(string.Empty, Key, PlainElastic.Net.Operator.OR, new PageData() { PageSize = 3 });         
            foreach (var item in suggestQuestion.list)
            {
                result += $"<a onclick='GetDetail(this)'>{GetKeyWord(item)}</a><br/>";
            }

            return result;
        }

        public string GetDetail(string key)
        {
            string answerWord = string.Empty;
            var result = this.Search(key, Key);
            DealQuestion(result, out answerWord);
            return answerWord;
        }

        public void SetRobotName(string name)
        {
            _botName = name ?? DefaultSetter.RobotName;
        }

        public void SetNoAnswerWord(string word)
        {
            _contact = word ?? DefaultSetter.ContactName;
        }
    }
}
