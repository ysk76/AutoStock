using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing.Imaging;

namespace ADBKnowns
{
    internal class Preprocessing
    {
        public Bitmap RectangleBlack(string device)
        {
            using (Bitmap bitmap = new Bitmap($@"C:\VisualStudio\ADBKnowns\Emulator\{device}\allImage.bmp"))
            {
                // 幅と高さを取得
                int height = Convert.ToInt32(bitmap.Height * 0.14);
                //int height = Convert.ToInt32(bitmap.Height * 0.025);
                int height2 = Convert.ToInt32(bitmap.Height * 0.88);
                //int height2 = Convert.ToInt32(bitmap.Height * 0.93);

                // 画像を切り抜く範囲を指定
                Rectangle rect = new Rectangle(0, height, bitmap.Width - 25, height2 - height);
                Bitmap destImage = bitmap.Clone(rect, bitmap.PixelFormat);

                destImage.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{device}\preprocessingBitmap.bmp", ImageFormat.Bmp);

                return destImage;
            }
            
        }
    }
}
