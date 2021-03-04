using Infoearth.BotEnvironment.Sealions;
using Infoearth.BotEnvironment.Sealions.QA_Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTest.Controllers
{
    public class HomeController : Controller
    {
        IQA_Bot elasticSearchContext = new QA_Bot_Html<MonitorQA>();

        public HomeController()
        {
            elasticSearchContext.SetRobotName(System.Configuration.ConfigurationManager.AppSettings["BotName"]);
            elasticSearchContext.SetNoAnswerWord(System.Configuration.ConfigurationManager.AppSettings["SystemContact"]);
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string GetFirstword()
        {
           return elasticSearchContext.GetFirstWord();
        }

        public string Answer(string key)
        {
            return elasticSearchContext.AnswerQuestion(key);
        }

        public string GetDetail(string keyword)
        {
            return elasticSearchContext.GetDetail(keyword);
        }
    }
}