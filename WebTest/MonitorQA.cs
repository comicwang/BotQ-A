﻿using Infoearth.BotEnvironment.Sealions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTest
{
	/*===================================================
	 * 类名称: monitor
	 * 类描述:
	 * 创建人: wangchong
	 * 创建时间: 2021/2/20 16:56:41
	 * 修改人:
	 * 修改时间:
	 * 版本： @version 1.0
	 =====================================================*/
	[Index(IndexName = "monitor", TypeName = "QA_Bot")]
	public class MonitorQA : ESBase
	{
		[QA(IsKey = true)]
		public string keyword { get; set; }

		public string model { get; set; }
		[QA(IsAnswer = true)]
		public string answer { get; set; }
	}
}
