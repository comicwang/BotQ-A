using Infoearth.BotEnvironment.Sealions;
using Infoearth.BotEnvironment.Sealions.QA_Bot;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Infoearth.Framework.QABotRestApi.Controllers
{
    /// <summary>
    /// QA问题回答的Api
    /// </summary>
    public class QAReplyController : ApiControllerBase
    {
        QA_Reply_Html<MonitorQA> _qaBot = new QA_Reply_Html<MonitorQA>();

        /// <summary>
        /// 分页获取问题库内容
        /// </summary>
        /// <param name="pageData"></param>
        /// <param name="keyword"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult<MonitorQA> PageQAReply([FromUri] PageData pageData, string keyword, string model = null)
        {
            var result = _qaBot.AnswerQuestion(pageData, keyword, model);
            List<MonitorQA> temp = _qaBot.TargetResults;
            foreach (var item in temp)
            {
                if (item.answer.Length > 100)
                    item.answer = item.answer.Substring(0, 100);
            }
            //删除图片内容           
            return new PageResult<MonitorQA>()
            {
                pageModel = pageData,
                data = temp
            };
        }

        /// <summary>
        /// 获取问题答案,一个或者多个
        /// </summary>
        /// <param name="keyword">问题关键字</param>
        /// <param name="model">问题模块</param>
        /// <returns></returns>
        [HttpGet]
        public List<MonitorQA> QAReply(string keyword, string model = null)
        {
            string result = _qaBot.AnswerQuestion(null, keyword, model);
            List<MonitorQA> temp = _qaBot.TargetResults;
            foreach (var item in temp)
            {
                if (item.answer.Length > 100)
                    item.answer = item.answer.Substring(0, 100);
            }
            return temp;
        }

        /// <summary>
        /// 获取查询字符串的分词内容
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [HttpGet]
        public List<string> SplitWord(string word)
        {
            return _qaBot.ESEngine.GetSplitWord(word);
        }

        /// <summary>
        /// 获取问题的详细信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public MonitorQA GetDetail(string key)
        {
            string result = _qaBot.GetDetail(key);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MonitorQA>(result);
        }
    }
}