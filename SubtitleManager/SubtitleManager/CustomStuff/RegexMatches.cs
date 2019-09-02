namespace SubtitleManager.CustomStuff
{
    public class RegexMatches
    {
        public const string SubRipMatchRegex = @"(?<Order>\d+)\r\n(?<StartTime>(\d\d:){2}\d\d,\d{3}) --> (?<EndTime>(\d\d:){2}\d\d,\d{3})\r\n(?<Sub>.+)(?=\r\n\r\n\d+|$)";

        public const string SubViewerMatchRegex = @"[0-9]+:[0-9]+:[0-9]+([,\\.][0-9]+)?";
    }   
}
