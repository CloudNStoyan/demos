using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public MainWindow()
        {
            this.InitializeComponent();
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
                this.FileData = File.ReadAllText(filePath);
            }

	        this.Subs = this.ParseSrt(this.FileData.Split('\n'));
        }

        private void NextSub(object s, EventArgs e)
        {
            this.SubtitleArea.Text = this.Subs[this.CurrentSub].Text;
            this.CurrentSub++;
        }

        private void PreviousSub(object s, EventArgs e)
        {
            this.SubtitleArea.Text = this.Subs[this.CurrentSub].Text;
            this.CurrentSub--;
        }

        private void EditSub(object s, EventArgs e)
        {
           // this.Subs[this.CurrentSub] = this.SubtitleArea.Text;
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
                srtList.Add(new Srt()
                {
                    Order = int.Parse(srtTemplate[0]),
                    Timeline = srtTemplate[1],
                    Text = srtTemplate[2]
                });
            }

            return srtList.ToArray();
        }
    }
}
