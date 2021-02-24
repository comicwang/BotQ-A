using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions.QA_Bot
{
    /// <summary>
    /// 智能QA机器人接口
    /// </summary>
    public interface IQA_Bot
    {
        /// <summary>
        /// 获取开场白
        /// </summary>
        /// <returns></returns>
        string GetFirstWord();
        /// <summary>
        /// 回答问题信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string AnswerQuestion(string key);

        string GetDetail(string key);
    }
}
