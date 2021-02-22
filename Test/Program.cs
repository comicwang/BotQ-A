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
            string botName = "小象";
            string prefix = "我的问题>";

            ElasticSearchHelper elasticSearchHelper = ElasticSearchHelper.Intance;
            /***
             * 智能助手开场白
             ***/
            Console.WriteLine($"您好，我是您的智能助手{botName}，请问有什么能够帮助你？");
            Console.WriteLine("您可以尝试提问：");
            Console.WriteLine("怎么新增监测点信息，怎么挂载设备到监测点等问题");
            Console.Write(prefix);
            while (true)
            {
                string key = Console.ReadLine();
                if (key == "E")
                {
                    Console.WriteLine("Bye!");

                }
                var result = elasticSearchHelper.Search<monitor>("test", string.Empty, key, "keyword", 0, 20);
                if (result.list.Count == 0)
                {
                    Console.WriteLine("您提的这个问题我暂时不能处理，请联系*********");
                }
                else if (result.list.Count == 1)
                {
                    Console.WriteLine($"{botName}为您找到问题>>>>>>>>>>>>>>>>>>>>>>");
                    Console.WriteLine(result.list[0].keyword);
                    Console.WriteLine("问题答案如下>>>>>>>>>>>>>>>>>>>>>>>");
                    Console.WriteLine(result.list[0].answer);
                }
                else
                {
                    Console.WriteLine($"{botName}为您找到多个问题，请问你想问的是哪个问题？请回答1,2,3..");
                    int index = 1;
                    foreach (var item in result.list)
                    {
                        Console.WriteLine($"{index}:{item.keyword}");
                        index++;
                    }
                    bool success = true;
                    int que = 0;
                    do
                    {
                        Console.Write(prefix);
                        string question = Console.ReadLine();
                        success = int.TryParse(question, out que);
                        if (success == false || que > result.list.Count)
                            Console.WriteLine($"{botName}不明白您的意思，请重新输入");
                    } while (success == false || que > result.list.Count);
                    Console.WriteLine($"{botName}为您找到问题>>>>>>>>>>>>>>>>>>>>>>");
                    Console.WriteLine(result.list[que - 1].keyword);
                    Console.WriteLine("问题答案如下>>>>>>>>>>>>>>>>>>>>>>>");
                    Console.WriteLine(result.list[que - 1].answer);
                }
                Console.WriteLine("请问还有什么能够帮到您的(退出请输入E)?");
                Console.Write(prefix);
            }
        }
    }
}
