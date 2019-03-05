using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Open file(Path): ");
            string path = Console.ReadLine();

            var result = ReadFile(path);
            Console.WriteLine("Say something to see that the thread is not blocked: ");
            string a = Console.ReadLine();
            Console.WriteLine("You said: " + a);

            result.Wait();
            Console.WriteLine(result.Result);
        }

        static async Task<string> ReadFile(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var file = await reader.ReadToEndAsync();


                for (int i = 0; i < 1000000; i++)
                {
                    var a = DateTime.Now.ToString();
                }

                Console.WriteLine("I finished!");

                return file;
            }
        }
    }
}
