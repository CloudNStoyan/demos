namespace SubtitleManager.SubtitleTypes
{
    class SubRip
    {
        public int Order { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Text { get; set; }

        public static SubRip Parse(int order, string startTime, string endTime, string text)
        {
            return new SubRip
            {
                Order = order,
                StartTime = startTime,
                EndTime = endTime,
                Text = text
            };
        }
    }
}
