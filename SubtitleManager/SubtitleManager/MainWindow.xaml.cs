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
                this.CurrentSubPath = this.FileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
                this.SubRipSubs = this.ParseSrt(this.FileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToArray());
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
                this.FileData = this.ReadFileText(this.CurrentSubPath);

                switch (Path.GetExtension(this.CurrentSubPath))
                {
                    case CustomExtension.Srt:
                        string[] splitedFileData =
                            this.FileData.Split(new[] { CustomString.NewLine }, StringSplitOptions.None);
                        this.SubRipSubs = this.ParseSrt(splitedFileData[0].StartsWith("#") ? splitedFileData.Skip(1).ToArray() : splitedFileData);
                        this.SubCount.Text = this.SubRipSubs.Length.ToString();
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
            if (this.LoadedSubtitleType == SubtitleType.SubRip && this.SubRipSubs != null && this.SubRipSubs.Length > 0)
            {
                this.SubRipSubs[this.CurrentSub].Text = this.SubtitleArea.Text;
                var list = new List<string> {"#" + this.CurrentSubPath + Environment.NewLine };
                list.AddRange(this.SubRipSubs.Select(x => x.Order + Environment.NewLine + x.Text + Environment.NewLine + x.Timeline));
                this.WriteFileText(CustomPaths.Temp, list.ToArray());
            }
        }

        private void SaveSubs(object s, EventArgs e)
        {
            if (this.SubsAreLoaded)
            {
                string subs = string.Join(Environment.NewLine,
                    this.SubRipSubs.Select(x => x.Order + Environment.NewLine + x.Text + Environment.NewLine + x.Timeline + Environment.NewLine).ToArray());
                this.WriteFileText(this.CurrentSubPath, subs);
                this.DeleteFile(CustomPaths.Temp);
                AlertService.Alert(this.LoadedSubtitleType + CustomMessages.SubsAreSavedTo + this.CurrentSubPath, AlertType.Info);
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

            return string.Empty;
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.DeleteFile("./temp.txt");
        }
    }
}
