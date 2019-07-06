using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

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
            if (File.Exists(CustomPaths.Temp))
            {
                this.FileData = this.ReadFileText(CustomPaths.Temp);
                this.CurrentSubPath = this.FileData.Split('\n')[0];
                this.SubRipSubs = this.ParseSrt(this.FileData.Split('\n').Skip(1).ToArray());
                AlertService.Alert(CustomMessages.LoadedFromLastUse, AlertType.Info);
            }
        }

        private void BrowseSubs(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".srt|.sub",
                Filter = "SubRip Files (*.srt)|*.srt|Sub Files (*.sub)|*.sub|Aegisub Files (*.ass)|*.ass"
            };

            if (dialog.ShowDialog() == true)
            {
                this.CurrentSubPath = dialog.FileName;
                this.FileData = this.ReadFileText(this.CurrentSubPath);

                if (Path.GetExtension(this.CurrentSubPath) == ".srt")
                {
                    this.SubRipSubs = this.ParseSrt(this.FileData.Split('\n'));
                    this.SubCount.Text = this.SubRipSubs.Length.ToString();
                    this.LoadedSubtitleType = SubtitleType.SubRip;
                }
                else if (Path.GetExtension(this.CurrentSubPath) == ".ass")
                {
                    this.Aegisubs = this.ParseAss(this.FileData.Split('\n'));
                    this.SubCount.Text = this.Aegisubs.Length.ToString();
                    this.LoadedSubtitleType = SubtitleType.Aegisub;
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

            AlertService.Alert("No more subs", AlertType.Info);
            return false;
        }

        private void FillSub()
        {
            switch (this.LoadedSubtitleType)
            {
                case SubtitleType.SubRip:
                    this.SubtitleArea.Text = this.SubRipSubs[this.CurrentSub].Text;
                    this.Timestamp.Text = this.SubRipSubs[this.CurrentSub].Timeline;
                    this.Order.Text = this.SubRipSubs[this.CurrentSub].Order.ToString();
                    break;
                default:
                    break;
            }
        }

        private void EditSub(object s, EventArgs e)
        {
            if (this.SubRipSubs != null && this.SubRipSubs.Length > 0)
            {
                this.SubRipSubs[this.CurrentSub].Text = this.SubtitleArea.Text;
                var list = new List<string> {"#" + this.CurrentSubPath + "\r\n"};
                list.AddRange(this.SubRipSubs.Select(x => x.Order + "\r\n" + x.Text + "\r\n" + x.Timeline));
                this.WriteFileText(CustomPaths.Temp, list.ToArray());
            }
        }

        private void SaveSubs(object s, EventArgs e)
        {
            if (this.SubsAreLoaded)
            {
                string subs = string.Join("\r\n",
                    this.SubRipSubs.Select(x => x.Order + "\r\n" + x.Text + "\r\n" + x.Timeline + "\r\n").ToArray());
                this.WriteFileText(this.CurrentSubPath, subs);
                this.DeleteFile(CustomPaths.Temp);
                AlertService.Alert("SubRipSubs are saved to:" + this.CurrentSubPath, AlertType.Info);
            }
            else
            {
                AlertService.Alert(CustomMessages.NoSubsLoaded, AlertType.Alert);
            }
        }

        private SubRip[] ParseSrt(string[] srtTextLines)
        {
            var list = new List<string[]>();

            var tempList = new List<string>();

            foreach (var line in srtTextLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    tempList.Add(line);
                }
                else
                {
                    list.Add(tempList.ToArray());
                    tempList = new List<string>();
                }
            }

            var srtList = new List<SubRip>();

            foreach (string[] srtTemplate in list)
            {
                if (srtTemplate.Length > 2)
                {
                    srtList.Add(SubRip.Parse(srtTemplate));
                }
            }

            return srtList.ToArray();
        }

        private Aegisub[] ParseAss(string[] assTextLines)
        {
            return new Aegisub[1];
        }

        private string ReadFileText(string path)
        {
            try
            {
                return File.ReadAllText(path, Encoding.Default);
            }
            catch (Exception e)
            {
                AlertService.Alert(e.Message, AlertType.Warning);
            }

            return "";
        }

        private void WriteFileText(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content, Encoding.Default);
            }
            catch (Exception e)
            {
                AlertService.Alert(e.Message, AlertType.Warning);
            }
        }

        private void WriteFileText(string path, string[] content)
        {
            try
            {
                File.WriteAllLines(path, content, Encoding.Default);
            }
            catch (Exception e)
            {
                AlertService.Alert(e.Message, AlertType.Warning);
            }
        }

        private void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                AlertService.Alert(e.Message, AlertType.Warning);
            }
        }
    }
}
