using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace MCROrganizer.Core.Utils
{
    public class OpenFileDialogUtils
    {
        public static Boolean TryGetImage(out BitmapImage image)
        {
            image = null;
            OpenFileDialog openFileDialog = new()
            {
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            try
            {
                image = new BitmapImage(new(openFileDialog.FileName));
            }
            catch
            {
                MessageBox.Show("Could not load the image file. Please try again with a different image.", PathUtils.Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
