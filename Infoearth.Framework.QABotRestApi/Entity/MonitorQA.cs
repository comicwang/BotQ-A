using Infoearth.BotEnvironment.Sealions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Framework.QABotRestApi
{
	/*===================================================
	 * 类名称: MonitorQA
	 * 类描述:
	 * 创建人: wangchong
	 * 创建时间: 2021/2/20 16:56:41
	 * 修改人:
	 * 修改时间:
	 * 版本： @version 1.0
	 =====================================================*/

	/// <summary>
	/// 监测预警的QA模板
	/// </summary>
	[Index(IndexName = "monitor",TypeName = "QA_Bot")]
	public class MonitorQA : ESBase
	{
		/// <summary>
		/// 标题
		/// </summary>
		[QA(IsKey = true)]
		public string keyword { get; set; }
		/// <summary>
		/// 模块
		/// </summary>
		[QA(IsModel =true)]
		public string model { get; set; }
		/// <summary>
		/// 问题答案
		/// </summary>
		[QA(IsAnswer = true)]
		public string answer { get; set; }

		/// <summary>
		/// 内推问题
		/// </summary>
		public bool innerSuggestion { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime createTime { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		public DateTime modifyTime { get; set; }
	}
}
