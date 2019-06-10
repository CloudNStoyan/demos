using System.Windows;

namespace SubtitleManager
{
    public static class AlertService
    {
        public static void Alert(string message)
        {
            MessageBox.Show(message, "Alert");
        }
    }
}
