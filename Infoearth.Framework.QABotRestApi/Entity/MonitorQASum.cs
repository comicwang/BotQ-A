using Infoearth.BotEnvironment.Sealions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infoearth.Framework.QABotRestApi
{
	/// <summary>
	/// QA问题统计
	/// </summary>
	[Index(IndexName = "monitor", TypeName = "QA_Bot_Sum",ParentName = "QA_Bot")]
	public class MonitorQASum : ESBase
    {
		/// <summary>
		/// 肯定的数量
		/// </summary>
		public int sure_count { get; set; }

		/// <summary>
		/// 否定的数量
		/// </summary>
		public int no_count { get; set; }
	}
}