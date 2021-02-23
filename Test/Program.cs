using Infoearth.BotEnvironment.Sealions;
using System;

namespace Test
{
    class Program
    {

        static string botName = "小象";
        static string prefix = "我的问题>";
        static string system = "小红（QQ123456）";

        static void Main(string[] args)
        {
            ElasticSearchContext elasticSearchHelper = ElasticSearchContext.Intance;
            /***
             * 智能助手开场白
             ***/
            Console.WriteLine($"您好，我是您的智能助手{botName}，请问有什么能够帮助你？");
            Console.WriteLine("您可以尝试提问：");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("怎么新增监测点信息，怎么挂载设备到监测点");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prefix);
            while (true)
            {
                string key = Console.ReadLine();
                if (key == "E")
                {
                    Console.WriteLine("Bye!");
                    break;
                }
                //1.对标题进行全词匹配
                var result = elasticSearchHelper.Search<monitor>("test", "monitor", key,"keyword");
                bool suceess = DealQuestion(result);
                if(suceess==false)
                {
                    //2.对标题进行单词匹配
                    result = elasticSearchHelper.Search<monitor>("test", "monitor", key, "keyword",PlainElastic.Net.Operator.OR);
                    suceess = DealQuestion(result);
                    if (suceess == false)
                    {
                        //3.对所有属性进行全词匹配
                        result = elasticSearchHelper.Search<monitor>("test", "monitor", key);
                        suceess = DealQuestion(result);
                        if (suceess == false)
                        {
                            //4.对所有属性进行单词匹配
                            result = elasticSearchHelper.Search<monitor>("test", "monitor", key,null, PlainElastic.Net.Operator.OR);
                            suceess = DealQuestion(result);
                            if (suceess == false)
                            {
                                Console.WriteLine($"抱歉，您的问题{botName}处理不了，请尝试更换查询问题或者联系管理员【{system}】");
                            }
                        }
                    }
                }
                Console.WriteLine("请问还有什么能够帮到您的【退出请输入E】?");
                Console.Write(prefix);
            }
        }

        static bool DealQuestion(ElasticModel<monitor> result)
        {
            if (result.list.Count == 0)
            {
                //放松匹配属性
                return false;
            }
            else if (result.list.Count == 1)
            {
                Console.WriteLine($"{botName}为您找到问题>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine(result.list[0].keyword);
                Console.WriteLine("问题答案如下>>>>>>>>>>>>>>>>>>>>>>>");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.list[0].answer);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine($"{botName}为您找到多个问题，请问你想问的是哪个问题？请回答1,2,3..,【没有回复0，重新提问】");
                int index = 1;
                foreach (var item in result.list)
                {
                    Console.WriteLine($"{index}:{item.keyword}");
                    index++;
                }
                bool success = true;
                int que = -1;
                do
                {
                    Console.Write(prefix);
                    string question = Console.ReadLine();
                    success = int.TryParse(question, out que);
                    if (success == false || que > result.list.Count)
                        Console.WriteLine($"{botName}不明白您的意思，请重新输入");
                } while ((success == false || que > result.list.Count) && que != 0);
                if (que != 0)
                {
                    Console.WriteLine($"{botName}为您找到问题>>>>>>>>>>>>>>>>>>>>>>");
                    Console.WriteLine(result.list[que - 1].keyword);
                    Console.WriteLine("问题答案如下>>>>>>>>>>>>>>>>>>>>>>>");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(result.list[que - 1].answer);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            return true;
        }
    }
}
