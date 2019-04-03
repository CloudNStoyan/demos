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
        static void Main(string[] args)
        {
            var task = Task.Run(Run);
            task.Wait();
            Process.Start("dwarf.jpg");
            Console.WriteLine("Dwarf");
        }

        static async Task Run()
        {
            string url = "https://www.transformice.com/";
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
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

            await page.ClickAsync("#swf2");
            await page.WaitForTimeoutAsync(20000);
            await page.Keyboard.PressAsync("ArrowLeft", new PressOptions()
            {
                Delay = 2000
            });

            await page.ScreenshotAsync("dwarf.jpg");
        }
    }
}
