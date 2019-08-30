using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;
using SubtitleManager.CustomStuff;
using SubtitleManager.Services;
using SubtitleManager.SubtitleTypes;

namespace SubtitleManager
{
    public partial class MainWindow : Window
    {
        private string FileData { get; set; }
        private SubRip[] SubRipSubs { get; set; }
        private Aegisub[] Aegisubs { get; set; }
        private SubViewer[] SubViewerSubs { get; set; }
        private int CurrentSub { get; set; }
        private string CurrentSubPath { get; set; }
        private bool SubsAreLoaded { get; set; }
        private SubtitleType LoadedSubtitleType { get; set; }
        private ConfigurationService ConfigurationService { get; set; }

        public MainWindow() 
        {
            this.InitializeComponent();
            
            this.ConfigurationService = new ConfigurationService();

            this.LoadLast();
        }

        private void LoadLast()
        {
            if (File.Exists(CustomPaths.Temp))
            {
                this.LoadSubs(this.FileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First());

                AlertService.Alert(CustomMessages.LoadedFromLastUse, AlertType.Info);
            }
        }

        private void LoadSubs(string fileName)
        {
            this.CurrentSubPath = fileName;
            this.FileData = FileManager.ReadFileText(this.CurrentSubPath);
            this.ConfigurationService.LastOpenedPath = this.CurrentSubPath;
            switch (Path.GetExtension(this.CurrentSubPath))
            {
                case CustomExtension.Srt:
                    this.SubRipSubs = this.ParseSrt(this.FileData);
                    this.SubCount.Text = CustomMessages.Subcount + this.SubRipSubs.Length;
                    this.LoadedSubtitleType = SubtitleType.SubRip;
                    break;
                case CustomExtension.Ass:
                    this.Aegisubs = this.ParseAss(this.FileData);
                    this.SubCount.Text = CustomMessages.Subcount + this.Aegisubs.Length;
                    this.LoadedSubtitleType = SubtitleType.Aegisub;
                    break;
                case CustomExtension.Sub:
                    this.SubViewerSubs = this.ParseSub(this.FileData);
                    this.SubCount.Text = CustomMessages.Subcount + this.SubViewerSubs.Length;
                    this.LoadedSubtitleType = SubtitleType.SubViewer;
                    break;
            }

            this.FillSub();
            this.SubsAreLoaded = true;
            this.CurrentSub = 0;
        }

        private void BrowseSubs(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = CustomFilter.Extensions,
                Filter = CustomFilter.Subs
            };

            if (dialog.ShowDialog() == true)
            {
                this.LoadSubs(dialog.FileName);
            }
            else
            {
                AlertService.Alert(CustomMessages.NoSubsLoaded, AlertType.Warning);
            }
        }

        private void NextSub(object s, EventArgs e)
        {
            if (this.ValidateSubMoving(SubtitleAction.Increment))
            {
                this.CurrentSub++;
                this.FillSub();
            }
        }

        private void PreviousSub(object s, EventArgs e)
        {
            if (this.ValidateSubMoving(SubtitleAction.Decrement))
            {
                this.CurrentSub--;
                this.FillSub();
            }
        }

        private bool ValidateSubMoving(SubtitleAction actionType)
        {
            if (!this.SubsAreLoaded)
            {
                AlertService.Alert(CustomMessages.NoSubsLoaded, AlertType.Alert);
                return false;
            }

            if ((actionType == SubtitleAction.Increment && this.CurrentSub + 1 <= this.SubRipSubs.Length - 1) ||
                (actionType == SubtitleAction.Decrement && this.CurrentSub - 1 >= 0))
            {
                return true;
            }

            AlertService.Alert(CustomMessages.NoMoreSubs, AlertType.Info);
            return false;
        }

        private void FillSub()
        {
            if (this.LoadedSubtitleType == SubtitleType.SubRip)
            {
                this.SubtitleArea.Text = this.SubRipSubs[this.CurrentSub].Text;
                this.StartTime.Text = CustomMessages.StartTime + this.SubRipSubs[this.CurrentSub].StartTime;
                this.EndTime.Text = CustomMessages.EndTime + this.SubRipSubs[this.CurrentSub].EndTime;
                this.Order.Text = CustomMessages.Order + this.SubRipSubs[this.CurrentSub].Order;
            }
        }

        private void EditSub(object s, EventArgs e)
        {
            if (this.LoadedSubtitleType == SubtitleType.SubRip && this.SubRipSubs != null && this.SubRipSubs.Length > 0)
            {
                this.SubRipSubs[this.CurrentSub].Text = this.SubtitleArea.Text;
                var list = new List<string>();
                list.AddRange(this.SubRipSubs.Select(x => x.Order + Environment.NewLine + x.Text + Environment.NewLine + x.StartTime + CustomString.SubtitleTimelineSeperator + x.EndTime));
                FileManager.WriteFileText(CustomPaths.Temp, list.ToArray());
            }
        }

        private void SaveSubs(object s, EventArgs e)
        {
            if (this.SubsAreLoaded)
            {
                string subs = string.Join(Environment.NewLine,
                    this.SubRipSubs.Select(x => x.Order + Environment.NewLine + x.Text + Environment.NewLine + x.StartTime + CustomString.SubtitleTimelineSeperator + x.EndTime + Environment.NewLine).ToArray());
                FileManager.WriteFileText(this.CurrentSubPath, subs);
                AlertService.Alert(this.LoadedSubtitleType + CustomMessages.SubsAreSavedTo + this.CurrentSubPath, AlertType.Info);
            }
            else
            {
                AlertService.Alert(CustomMessages.NoSubsLoaded, AlertType.Alert);
            }
        }

        private SubRip[] ParseSrt(string srtRawText)
        {
            var myRegex = new Regex(RegexMatches.SubRipMatchRegex, RegexOptions.Multiline);

            var srtList = new List<SubRip>();

            foreach (Match myMatch in myRegex.Matches(srtRawText))
            {
                if (myMatch.Success)
                {
                    int order = int.Parse(myMatch.Groups[RegexGroups.Order].Value);
                    string startTime = myMatch.Groups[RegexGroups.StartTime].Value;
                    string endTime = myMatch.Groups[RegexGroups.EndTime].Value;
                    string subtitleText = myMatch.Groups[RegexGroups.Sub].Value;

                    srtList.Add(SubRip.Parse(order, startTime, endTime, subtitleText));
                }
            }

            return srtList.ToArray();
        }

        private Aegisub[] ParseAss(string subtitleData)
        {
            string[] subtitleLines = subtitleData.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Where(x => x.StartsWith(CustomString.AegisubDialog)).Select(x => x).ToArray();

            return subtitleLines.Where(subtitleFormat => subtitleFormat.Length == 10)
                .Select(subtitleFormat => subtitleFormat.Split(',')).Select(subtitleFormat => new Aegisub
                {
                    Layer = int.Parse(subtitleFormat[0]),
                    Start = subtitleFormat[1],
                    End = subtitleFormat[2],
                    Style = subtitleFormat[3],
                    Name = subtitleFormat[4],
                    MarginL = subtitleFormat[5],
                    MarginR = subtitleFormat[6],
                    MarginV = subtitleFormat[7],
                    Effect = subtitleFormat[8],
                    Text = subtitleFormat[9]
                }).ToArray();
        }

        private SubViewer[] ParseSub(string subtitleData)
        {
            var regex = new Regex(RegexMatches.SubViewerMatchRegex, RegexOptions.Multiline);

            var subList = new List<SubViewer>();

            foreach (Match match in regex.Matches(subtitleData))
            {
                string startTime = match.Groups[RegexGroups.StartTime].Value;
                string endTime = match.Groups[RegexGroups.EndTime].Value;
                string sub = match.Groups[RegexGroups.Sub].Value;

                subList.Add(new SubViewer
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    Text = sub
                });
            }

            return subList.ToArray();
        }

        private void DeleteTemp(object sender, RoutedEventArgs e)
        {
            FileManager.DeleteFile(CustomPaths.Temp);
            AlertService.Alert(CustomMessages.TempDeleted, AlertType.Info);
        }
    }
}
