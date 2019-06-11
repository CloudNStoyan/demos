﻿using System.Windows;

namespace SubtitleManager
{
    public enum AlertType
    {
        Alert,
        Warning,
        Info
    }

    public static class AlertService
    {
        public static void Alert(string message, AlertType alertType)
        {
            MessageBox.Show(message, alertType.ToString());
        }
    }
}
