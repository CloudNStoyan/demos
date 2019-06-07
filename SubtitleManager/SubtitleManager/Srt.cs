namespace SubtitleManager
{
    class SubRip
    {
        public int Order { get; set; }
        public string Timeline { get; set; }
        public string Text { get; set; }

        public static SubRip Parse(string[] srtTemplate)
        {
            return new SubRip
            {
                Order = int.Parse(srtTemplate[0]),
                Timeline = srtTemplate[1],
                Text = srtTemplate[2]
            };
        }
    }
}
