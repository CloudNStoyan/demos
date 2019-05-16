using System;
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
        private string[] Subs { get; set; }
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

	        this.Subs = this.FileData.Split('\n').Where(x  => !string.IsNullOrWhiteSpace(x)).Select(x => x).ToArray();
        }

        private void NextSub(object s, EventArgs e)
        {
            this.SubtitleArea.Text = this.Subs[this.CurrentSub];
            this.CurrentSub++;
        }
    }
}
