using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace TryingLogging
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Logger = logger;

            Log.Information("Added user.");

            Console.WriteLine("This shows up later in the code so the real log is in the time being created.");
        }
    }
}
