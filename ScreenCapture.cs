using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;

public static class ScreenCapture
{
    public static Bitmap Capture(string device, string directryName)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = @"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            Arguments = $"-s {device} exec-out screencap -p",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        using Process p = Process.Start(psi);

        using MemoryStream ms = new MemoryStream();

        try
        {
            p.StandardOutput.BaseStream.CopyTo(ms);

            p.WaitForExit();

            ms.Position = 0;

            var bitmap = new Bitmap(ms);

            bitmap.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\allImage.bmp", ImageFormat.Bmp);

            return new Bitmap(ms);
        }
        catch
        {
            return new Bitmap(1, 1);
        }
        
       
    }

    public static Bitmap HomeImage(string directryName)
    {
        Bitmap image = new Bitmap($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\allImage.bmp");
        Mat mat = BitmapConverter.ToMat(image);

        int sx = Convert.ToInt32(image.Width * 0.73);
        int sy = Convert.ToInt32(image.Height * 0.19);
        int ex = Convert.ToInt32(image.Width * 0.2);
        int ey = Convert.ToInt32(image.Height * 0.08);


        // 切り出し矩形
        Rect roi = new Rect(sx, sy, ex, ey);

        // Matで切り出し
        Mat croppedMat = new Mat(mat, roi).Clone();

        Bitmap bitmap = BitmapConverter.ToBitmap(croppedMat);
        //----------確認用
        bitmap.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\homeImage.bmp", ImageFormat.Bmp);
        //----------
        bitmap.Dispose();
        image.Dispose();

        return bitmap;
    }

    public static Bitmap CloseMark(string directryName)
    {
        Bitmap image = new Bitmap($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\allImage.bmp");
        Mat mat = BitmapConverter.ToMat(image);

        int sx = Convert.ToInt32(image.Width * 0.4);
        int sy = Convert.ToInt32(image.Height * 0.88);
        int ex = Convert.ToInt32(image.Width * 0.15);
        int ey = Convert.ToInt32(image.Height * 0.1);


        // 切り出し矩形
        Rect roi = new Rect(sx, sy, ex, ey);

        // Matで切り出し
        Mat croppedMat = new Mat(mat, roi).Clone();

        //----------確認用
        Bitmap bitmap = BitmapConverter.ToBitmap(croppedMat);
        // 保存
        bitmap.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\Close.bmp", ImageFormat.Bmp);
        //----------
        bitmap.Dispose();
        image.Dispose();

        return bitmap;
    }

    public static Bitmap GetMark(string directryName)
    {
        Bitmap image = new Bitmap($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\allImage.bmp");
        Mat mat = BitmapConverter.ToMat(image);

        int sx = Convert.ToInt32(image.Width * 0.28);
        int sy = Convert.ToInt32(image.Height * 0.25);
        int ex = Convert.ToInt32(image.Width * 0.48);
        int ey = Convert.ToInt32(image.Height * 0.15);


        // 切り出し矩形
        Rect roi = new Rect(sx, sy, ex, ey);

        // Matで切り出し
        Mat croppedMat = new Mat(mat, roi).Clone();

        //----------確認用
        Bitmap bitmap = BitmapConverter.ToBitmap(croppedMat);
        // 保存
        bitmap.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\Get.bmp", ImageFormat.Bmp);
        //----------
        bitmap.Dispose();
        image.Dispose();

        return bitmap;
    }

    public static Bitmap advertisement(string directryName)
    {
        Bitmap image = new Bitmap($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\allImage.bmp");
        Mat mat = BitmapConverter.ToMat(image);

        int sx = Convert.ToInt32(image.Width * 0.1);
        int sy = Convert.ToInt32(image.Height * 0.9);
        int ex = Convert.ToInt32(image.Width * 0.3);
        int ey = Convert.ToInt32(image.Height * 0.1);


        // 切り出し矩形
        Rect roi = new Rect(sx, sy, ex, ey);

        // Matで切り出し
        Mat croppedMat = new Mat(mat, roi).Clone();

        //----------確認用
        Bitmap bitmap = BitmapConverter.ToBitmap(croppedMat);
        // 保存
        bitmap.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\advertisement.bmp", ImageFormat.Bmp);
        //----------
        bitmap.Dispose();
        image.Dispose();

        return bitmap;
    }


    public static Bitmap GetPoint(string directryName)
    {
        Bitmap image = new Bitmap($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\allImage.bmp");
        Mat mat = BitmapConverter.ToMat(image);

        int sx = Convert.ToInt32(image.Width * 0.05);
        int sy = Convert.ToInt32(image.Height * 0.03);
        int ex = Convert.ToInt32(image.Width * 0.15);
        int ey = Convert.ToInt32(image.Height * 0.1);


        // 切り出し矩形
        Rect roi = new Rect(sx, sy, ex, ey);

        // Matで切り出し
        Mat croppedMat = new Mat(mat, roi).Clone();

        // 4倍に拡大（超重要）
        Mat resized = new Mat();
        Cv2.Resize(croppedMat, resized, new OpenCvSharp.Size(), 4, 4, InterpolationFlags.Cubic);

        //----------確認用
        Bitmap bitmap = BitmapConverter.ToBitmap(resized);
        // 保存
        bitmap.Save($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\GetPoint.bmp", ImageFormat.Bmp);
        //----------
        bitmap.Dispose();
        image.Dispose();

        return bitmap;
    }

}