using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AddCourseUdemyDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                var udemyClient = new UdemyClient();
                var login = udemyClient.Login(args[0], args[1]);
                if (login)
                {
                    if (File.Exists(args[2]))
                    {
                        var readAllLines = File.ReadAllLines(args[2]);
                        foreach (var line in readAllLines)
                        {
                            var addLink = udemyClient.AddLink(line);
                            ShowConsole(addLink);
                        }
                    }
                    else
                    {
                        ShowConsole(udemyClient.AddLink(args[2]));
                    }
                }
            }
        }

        public static void ShowConsole(int isSucess)
        {
            if (isSucess == 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Already!");
            }
            else if (isSucess == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error!");
            }
            Console.ResetColor();
        }
    }
}
