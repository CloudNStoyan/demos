using System;
using System.IO;
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
	    string[] subs = this.FileData.split('\n').Select(x => x).Where(x  => !string.isNullOrWhitespace(x));
        }

    }
}
