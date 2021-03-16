using Infoearth.BotEnvironment.Sealions;
using Infoearth.BotEnvironment.Sealions.QA_Bot;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Infoearth.Framework.QABotRestApi.Controllers
{
    /// <summary>
    /// QA问题回答的Api
    /// </summary>
    public class QAManagerController : ApiControllerBase
    {
        QA_Reply_Html<MonitorQA> _qaBot = new QA_Reply_Html<MonitorQA>();
        QA_Reply_Html<MonitorQASum> _qaSum = new QA_Reply_Html<MonitorQASum>();


        /// <summary>
        /// 分页获取问题库内容
        /// </summary>
        /// <param name="pageData"></param>
        /// <param name="keyword"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PageResult<MonitorQA> PageGetQuestions([FromUri] PageData pageData, string keyword, string model = null)
        {
            var result = _qaBot.AnswerQuestion(pageData, keyword, model);
            List<MonitorQA> temp = _qaBot.TargetResults;
            ////foreach (var item in temp)
            ////{
            ////    if (item.answer.Length > 100)
            ////        item.answer = item.answer.Substring(0, 100);
            ////}
            //删除图片内容           
            return new PageResult<MonitorQA>()
            {
                pageModel = pageData,
                data = temp
            };
        }

        /// <summary>
        /// 保存问题库
        /// </summary>
        /// <param name="monitorQA"></param>
        /// <returns></returns>
        [HttpPost]
        public WebApiResult SaveQuestion(MonitorQA monitorQA)
        {
            if (string.IsNullOrEmpty(monitorQA._id) == false)
                monitorQA.modifyTime = DateTime.Now;
            IndexResult indexResult = _qaBot.Insert(monitorQA);
            
            return Success("保存成功");
        }

        /// <summary>
        /// 删除问题库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public WebApiResult Delete(string id)
        {
           DeleteResult deleteResult=  _qaBot.Delete(id);
            return Success("删除成功");
        }

        /// <summary>
        /// 根据ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public MonitorQA GetById(string id)
        {
           return  _qaBot.SearchById(id);
        }

        /// <summary>
        /// 保存问题推荐
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bo"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public WebApiResult SaveQAReply(string id, bool bo)
        {
            MonitorQASum monitorQASum = _qaSum.SearchById(id);
            if (monitorQASum == null)
            {
                monitorQASum = new MonitorQASum();
                monitorQASum.no_count = 0;
                monitorQASum.sure_count = 0;
                monitorQASum._id = id;
            }
            if (bo)
            {
                monitorQASum.sure_count += 1;
            }
            else
            {
                monitorQASum.no_count += 1;
            }
            IndexResult indexResult = _qaSum.Insert(monitorQASum);
            return Success("保存成功");
        }
    }
}