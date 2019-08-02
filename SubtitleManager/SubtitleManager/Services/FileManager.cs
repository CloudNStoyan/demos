using System;
using System.IO;
using System.Text;

namespace SubtitleManager.Services
{
    public static class FileManager
    {
        public static string ReadFileText(string path)
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

        public static void WriteFileText(string path, string content)
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

        public static void WriteFileText(string path, string[] content)
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

        public static void DeleteFile(string path)
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
