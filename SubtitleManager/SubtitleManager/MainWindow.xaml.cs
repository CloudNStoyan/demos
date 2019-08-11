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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FileData { get; set; }
        private SubRip[] SubRipSubs { get; set; }
        private Aegisub[] Aegisubs { get; set; }
        private int CurrentSub { get; set; }
        private string CurrentSubPath { get; set; }
        private bool SubsAreLoaded { get; set; }
        private SubtitleType LoadedSubtitleType { get; set; }

        public MainWindow() 
        {
            this.InitializeComponent();
            this.LoadLast();
        }

        private void LoadLast()
        {
            if (File.Exists(CustomPaths.Temp))
            {
                this.FileData = FileManager.ReadFileText(CustomPaths.Temp);
                this.CurrentSubPath = this.FileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
                this.SubRipSubs = this.ParseSrt(this.FileData);
                AlertService.Alert(CustomMessages.LoadedFromLastUse, AlertType.Info);
            }
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
                this.CurrentSubPath = dialog.FileName;
                this.FileData = FileManager.ReadFileText(this.CurrentSubPath);

                switch (Path.GetExtension(this.CurrentSubPath))
                {
                    case CustomExtension.Srt:
                        this.SubRipSubs = this.ParseSrt(this.FileData);
                        this.SubCount.Text = CustomMessages.Subcount + this.SubRipSubs.Length;
                        this.LoadedSubtitleType = SubtitleType.SubRip;
                        break;
                    case CustomExtension.Ass:
                        this.Aegisubs = this.ParseAss(this.FileData.Split(new[] { CustomString.NewLine }, StringSplitOptions.None));
                        this.SubCount.Text = this.Aegisubs.Length.ToString();
                        this.LoadedSubtitleType = SubtitleType.Aegisub;
                        break;
                }

                this.FillSub();
                this.SubsAreLoaded = true;
                this.CurrentSub = 0;
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
                Properties.Settings.Default["LastSubtitlePath"] = this.CurrentSubPath;
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
                FileManager.DeleteFile(CustomPaths.Temp);
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

        private Aegisub[] ParseAss(string[] assTextLines)
        {
            return new Aegisub[1];
        }

        private void DeleteTemp(object sender, RoutedEventArgs e)
        {
            FileManager.DeleteFile(CustomPaths.Temp);
            AlertService.Alert(CustomMessages.TempDeleted, AlertType.Info);
        }
    }
}
