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
    /// QA模板回答
    /// </summary>
    public class QA_Reply_Html<T> : QAContextBase<T> where T : ESBase
    {
        private string _botName = DefaultSetter.RobotName;
        private string _contact = DefaultSetter.ContactName;

        public List<T> TargetResults
        {
            get;
            private set;
        }

        public string AnsweQuestionByIk(PageData pageData, string key, string model)
        {
            string answerWord = string.Empty;
            var result = this.SearchIk(pageData,
                string.IsNullOrEmpty(key) ? null :
                new QueryContext()
                {
                    queryModel = new QueryModel()
                    {
                        key = key,
                        field = Key,
                        queryRelation = QueryRelation.And
                    },
                    queryRelation = QueryRelation.And
                }, string.IsNullOrEmpty(model) ? null : new QueryContext()
                {
                    queryModel = new QueryModel()
                    {
                        key = model,
                        field = AnswerModel,
                        queryRelation = QueryRelation.And
                    },
                    queryRelation = QueryRelation.And
                }
                );
            bool suceess = DealQuestion(result, out answerWord);
            if (suceess == false)
            {
                result = this.SearchIk(pageData,
                string.IsNullOrEmpty(key) ? null :
                new QueryContext()
                {
                    queryModel = new QueryModel()
                    {
                        key = key,
                        field = Key,
                        queryRelation = QueryRelation.Or
                    },
                    queryRelation = QueryRelation.And
                }, string.IsNullOrEmpty(model) ? null : new QueryContext()
                {
                    queryModel = new QueryModel()
                    {
                        key = model,
                        field = AnswerModel,
                        queryRelation = QueryRelation.And
                    },
                    queryRelation = QueryRelation.And
                }
                );
                suceess = DealQuestion(result, out answerWord);
                if (suceess == false)
                {
                    result = this.SearchIk(pageData,
                  string.IsNullOrEmpty(key) ? null :
                  new QueryContext()
                  {
                      queryModel = new QueryModel()
                      {
                          key = key,
                          queryRelation = QueryRelation.And
                      },
                      queryRelation = QueryRelation.And
                  }, string.IsNullOrEmpty(model) ? null : new QueryContext()
                  {
                      queryModel = new QueryModel()
                      {
                          key = model,
                          field = AnswerModel,
                          queryRelation = QueryRelation.And
                      },
                      queryRelation = QueryRelation.And
                  }
                  );
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

        public string AnswerQuestion(PageData pageData,string key,string model)
        {
            string answerWord = string.Empty;
            //1.对标题进行全词匹配
            var result = this.Search(key, Key, PlainElastic.Net.Operator.AND, pageData, model, AnswerModel);
            bool suceess = DealQuestion(result, out answerWord);
            if (suceess == false)
            {
                //2.对标题进行单词匹配
                result = this.Search(key, Key, PlainElastic.Net.Operator.OR, pageData, model, AnswerModel);
                suceess = DealQuestion(result, out answerWord);
                if (suceess == false)
                {
                    //3.对所有属性进行全词匹配
                    result = this.Search(key,null, PlainElastic.Net.Operator.AND, pageData, model, AnswerModel);
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
            else if (result.list.Count == 0)
                answerWord = Newtonsoft.Json.JsonConvert.SerializeObject(result.list.FirstOrDefault());
            else
                answerWord = Newtonsoft.Json.JsonConvert.SerializeObject(result.list);
            return true;
        }

        public string GetFirstWord()
        {
            var suggestQuestion = this.Search(string.Empty, Key, PlainElastic.Net.Operator.OR, new PageData() { PageSize = 3 });

            return Newtonsoft.Json.JsonConvert.SerializeObject(suggestQuestion.list);
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
