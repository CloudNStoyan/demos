using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitableNotTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start the HandleFile method.
            Task<int> task = HandleFileAsync();

            // Control returns here before HandleFileAsync returns.
            // ... Prompt the user.
            Console.WriteLine("Please wait patiently " +
                              "while I do something important.");

            // Do something at the same time as the file is being read.
            string line = Console.ReadLine();
            Console.WriteLine("You entered (asynchronous logic): " + line);

            // Wait for the HandleFile task to complete.
            // ... Display its results.
            task.Wait();
            var x = task.Result;
            Console.WriteLine("Count: " + x);

            Console.WriteLine("[DONE]");
            Console.ReadLine();
        }

        static async MyClass HandleFileAsync()
        {
            string file = @"C:\Users\Stoyan\Desktop\e.txt";
            Console.WriteLine("HandleFile enter");
            int count = 0;

            using (var reader = new StreamReader(file))
            {
                string v = await reader.ReadToEndAsync();

                // ... Process the file data somehow.
                count += v.Length;

                // ... A slow-running computation.
                //     Dummy code.
                for (int i = 0; i < 10000000; i++)
                {
                    string a = DateTime.Now.ToString();
                }
            }
            Console.WriteLine("HandleFile exit");
            return count;
        }
    }


    class MyClass
    {
        public static Awaiter Yield() { return new Awaiter(); }

        public struct Awaiter : System.Runtime.CompilerServices.INotifyCompletion
        {
            public Awaiter GetAwaiter() { return this; }

            public bool IsCompleted { get { return false; } }

            public void OnCompleted(Action continuation)
            {
                ThreadPool.QueueUserWorkItem((state) => ((Action)state)(), continuation);
            }

            public void GetResult() { }
        }
    }


}
