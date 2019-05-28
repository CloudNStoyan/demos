using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace TryingPuppeeteerSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Run();

            Process.Start("dwarf.jpg");
            Console.WriteLine("Dwarf");
        }

        static async Task Run()
        {
            string url = "https://www.transformice.com/";
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false
            });
            var page = await browser.NewPageAsync();

            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1800,
                Height = 1080
            });

            await page.GoToAsync(url);

            await page.ScreenshotAsync("dwarf.jpg");
        }
    }
}
