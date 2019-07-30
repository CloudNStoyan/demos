﻿namespace SubtitleManager
{
    public static class CustomPaths
    {
        public const string Temp = "./temp.txt";
    }

    public static class CustomMessages
    {
        public const string NoSubsLoaded = "No subs loaded!";
        public const string LoadedFromLastUse = "Subs loaded from last use!";
        public const string SubsAreSavedTo = " are saved to:";
        public const string NoMoreSubs = "No more subs";
        public const string TempDeleted = "Last temp file deleted.";
        public const string Timestamp = "Timestamp is: ";
        public const string Order = "Order is: ";
        public const string Subcount = "Sub count is: ";
    }

    public static class CustomFilter
    {
        public const string Subs = "SubRip Files (*.srt)|*.srt|Sub Files (*.sub)|*.sub|Aegisub Files (*.ass)|*.ass";
        public const string Extensions = ".srt|.sub";
    }

    public static class CustomExtension
    {
        public const string Srt = ".srt";
        public const string Ass = ".ass";
        public const string Sub = ".sub";
    }
}
