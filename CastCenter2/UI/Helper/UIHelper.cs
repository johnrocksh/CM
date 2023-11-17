namespace CastManager.Helper
{
    using System;
    using System.Collections.Generic;
    using CastManager.Logger;
    using System.Linq;
    using System.Net.Cache;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;

    static public class UIHelper
    {
        /// <summary>
        /// Opens a file open dialog to select a picture
        /// </summary>
        /// <returns> Image path </returns>
        static public string OpenImageFileDialog()
        {
            //Open File Dialog and show it
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
                "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
                "All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            Logger.WriteLine($"{openFileDialog.FileName}");
            return openFileDialog.FileName;
        }
    }
}
