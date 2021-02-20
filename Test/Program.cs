using Infoearth.BotEnvironment.Sealions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ElasticSearchHelper elasticSearchHelper = ElasticSearchHelper.Intance;
            while (true)
            {
                Console.WriteLine("请输入查询关键字：");
                string key = Console.ReadLine();
                var result = elasticSearchHelper.Search<monitor>("test", string.Empty, key, "answer", 0, 20);
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                Console.WriteLine(str);
            }       
        }
    }
}
