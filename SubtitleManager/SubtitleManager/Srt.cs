using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleManager
{
    class Srt
    {
        public int Order { get; set; }
        public string Timeline { get; set; }
        public string Text { get; set; }

        public static Srt Parse(string[] srtTemplate)
        {
            return new Srt()
            {
                Order = int.Parse(srtTemplate[0]),
                Timeline = srtTemplate[1],
                Text = srtTemplate[2]
            };
        }
    }
}
