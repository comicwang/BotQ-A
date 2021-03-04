using Infoearth.BotEnvironment.Sealions;
using System;
using TestAdd;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            QA_Bot_Console<monitorQA> _context = new QA_Bot_Console<monitorQA>();
            _context.SetRobotName("小红");
            _context.SetNoAnswerWord("请联系XXXX");
            string prefix = ">";
            /***
             * 智能助手开场白
             ***/
            Console.WriteLine(_context.GetFirstWord());
            Console.Write(prefix);
            while (true)
            {
                string key = Console.ReadLine();
                if (key == "E")
                {
                    Console.WriteLine("Bye!");
                    break;
                }
                string result = _context.AnswerQuestion(key);
                Console.WriteLine(result);
                //进入选题模式
                if (_context.TargetResults.Count > 0)
                {
                    bool success = true;
                    int que = -1;
                    do
                    {
                        Console.Write(prefix);
                        string question = Console.ReadLine();
                        success = int.TryParse(question, out que);
                        if (success == false || que > _context.TargetResults.Count)
                            Console.WriteLine(result);
                    } while ((success == false || que > _context.TargetResults.Count) && que != 0);
                    if (que != 0)
                    {
                        Console.WriteLine(_context.TargetResults[que].answer);
                    }
                }
                Console.WriteLine("请问还有什么能够帮到您的【退出请输入E】?");
                Console.Write(prefix);
            }
        }
    }
}
