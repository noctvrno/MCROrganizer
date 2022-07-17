using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MCROrganizer.Core.Utils
{
    public static class MessageBoxUtils
    {
        public static void ShowError(String message)
        {
            MessageBox.Show(message, PathUtils.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(String message)
        {
            MessageBox.Show(message, PathUtils.Caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
