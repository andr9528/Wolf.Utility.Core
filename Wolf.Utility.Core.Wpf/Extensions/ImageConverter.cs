using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace Wolf.Utility.Core.Wpf.Extensions
{
    public class ImageConverter
    {
        // Source: https://www.codeproject.com/Questions/465372/Csharp-WPF-XAML-Convert-ImageSource-from-to-byte-a
        public static ImageSource ByteToImageSource(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }
}
