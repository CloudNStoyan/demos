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
        private Srt[] Subs { get; set; }
        private int CurrentSub { get; set; }
        private string CurrentSubPath { get; set; }
        private bool SubsAreLoaded { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            if (File.Exists("./temp.txt"))
            {
                this.FileData = this.ReadFileText("./temp.txt");
                this.CurrentSubPath = this.FileData.Split('\n')[0];
                this.Subs = this.ParseSrt(this.FileData.Split('\n').Skip(1).ToArray());
                MessageBox.Show("Loaded subs from last use.");
            }
        }

        private void BrowseSubs(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".srt",
                Filter = "Subtitle Files (*.srt)|*.srt"
            };

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;
                this.FileData = this.ReadFileText(filePath);
                this.CurrentSubPath = filePath;
            }

	        this.Subs = this.ParseSrt(this.FileData.Split('\n'));
            this.FillSub();
            this.SubsAreLoaded = true;
        }

        private void NextSub(object s, EventArgs e)
        {
            if (this.SubsAreLoaded)
            {
                this.CurrentSub++;
                this.FillSub();
            }
            else
            {
                MessageBox.Show("No subs loaded!");
            }
        }

        private void PreviousSub(object s, EventArgs e)
        {
            if (this.SubsAreLoaded)
            {
                this.CurrentSub--;
                this.FillSub();
            }
            else
            {
                MessageBox.Show("No subs loaded!");
            }
        }

        private void FillSub()
        {
            this.SubtitleArea.Text = this.Subs[this.CurrentSub].Text;
            this.Timestamp.Text = this.Subs[this.CurrentSub].Timeline;
            this.Order.Text = this.Subs[this.CurrentSub].Order.ToString();
        }

        private void EditSub(object s, EventArgs e)
        {
            if (this.Subs != null && this.Subs.Length > 0)
            {
                this.Subs[this.CurrentSub].Text = this.SubtitleArea.Text;
                var list = new List<string> {"#" + this.CurrentSubPath + "\r\n"};
                list.AddRange(this.Subs.Select(x => x.Order + "\r\n" + x.Text + "\r\n" + x.Timeline));
                this.WriteFileText("./temp.srt", list.ToArray());
            }
        }

        private void SaveSubs(object s, EventArgs e)
        {
            string subs = string.Join("\r\n",
                this.Subs.Select(x => x.Order + "\r\n" + x.Text + "\r\n" + x.Timeline + "\r\n").ToArray());
            this.WriteFileText(this.CurrentSubPath, subs);
            this.DeleteFile("./temp.txt");
            MessageBox.Show("Subs are saved to:" + this.CurrentSubPath);
        }

        private Srt[] ParseSrt(string[] srtTextLines)
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

            var srtList = new List<Srt>();

            foreach (string[] srtTemplate in list)
            {
                if (srtTemplate.Length > 2)
                {
                    srtList.Add(Srt.Parse(srtTemplate));
                }
            }

            return srtList.ToArray();
        }

        private string ReadFileText(string path)
        {
            try
            {
                return File.ReadAllText(path, Encoding.Default);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
                MessageBox.Show(e.Message);
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
                MessageBox.Show(e.Message);
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
                MessageBox.Show(e.Message);
            }
        }
    }
}
