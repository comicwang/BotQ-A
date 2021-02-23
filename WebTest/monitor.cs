using Infoearth.BotEnvironment.Sealions;
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
    public class monitor: BaseHighlight
	{
		[Column(Key =true)]
		public string keyword { get; set; }

		public string model { get; set; }
		[Column(Answer =true)]
		public string answer { get; set; }
    }
}
