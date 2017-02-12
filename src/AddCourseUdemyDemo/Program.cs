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
                        var readAllLines = File.ReadAllLines(args[3]);
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

        public static void ShowConsole(bool isSucess)
        {
            if (isSucess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Error!");
            }
            Console.ResetColor();
        }
    }
}
