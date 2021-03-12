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
            IndexResult indexResult = _qaBot.ESEngine.Index(monitorQA._id, monitorQA);
            
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
           DeleteResult deleteResult=  _qaBot.ESEngine.Delete(id);
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
           return  _qaBot.ESEngine.SearchById<MonitorQA>(id);
        }
    }
}