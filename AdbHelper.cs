using System.Collections.Generic;
using System.Diagnostics;
using OpenCvSharp;

//ADB操作専用ユーティリティ
public static class AdbHelper
{
    public static void StartServer()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            //接続端末一覧表示
            FileName = @"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            Arguments = "start-server",
            RedirectStandardOutput = true,//コマンド結果を取得
            UseShellExecute = false,//出力リダイレクト可能
            CreateNoWindow = true,//黒画面を出さない
            Verb = "runas"//管理者権限
        };

        using Process p = Process.Start(psi);

        p.WaitForExit();
    }

    //エミュレーター接続
    public static bool GetDevices(string device)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            //接続端末一覧表示
            FileName = @"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            Arguments = "connect " + device,
            RedirectStandardOutput = true,//コマンド結果を取得
            UseShellExecute = false,//出力リダイレクト可能
            CreateNoWindow = true,//黒画面を出さない
            Verb = "runas"//管理者権限
        };

        using Process p = Process.Start(psi);

        string result = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        bool connected = false;
        if (result.Contains("connected ") && result.Contains(device))
        {
            connected = true;
        }

        return connected;
    }

    public static string GetXandY(string device)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            //接続端末一覧表示
            FileName = @"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            Arguments = "-s " + device + " shell wm size",
            RedirectStandardOutput = true,//コマンド結果を取得
            UseShellExecute = false,//出力リダイレクト可能
            CreateNoWindow = true,//黒画面を出さない
            Verb = "runas"//管理者権限
        };

        using Process p = Process.Start(psi);

        string result = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        return result;
    }



    /// <summary>
    /// 画面クリック
    /// </summary>
    /// <param name="device">ADBデバイスID</param>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    public static void Tap(string device, int x, int y)
    {
        Process.Start(@"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            $"-s {device} shell input tap {x} {y}");
    }

    public static void Swipe(string device, int x1, int y1, int x2, int y2, int duration = 300)
    {
        Process.Start(@"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            $"-s {device} shell input swipe {x1} {y1} {x2} {y2} {duration}");
    }

    public static void ScrollDown(string device)
    {
        Swipe(device, 500, 1500, 500, 500);
    }

    public static void Home(string device)
    {
        Process.Start(@"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            $"-s {device} shell input keyevent 3");
    }

    public static void Back(string device)
    {
        Process.Start(@"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            $"-s {device} shell input keyevent 4");
    }

    public static void TaskKill(string device)
    {
        Process.Start(@"C:\VisualStudio\ADBKnowns\platform-tools\adb.exe",
            $"-s {device} shell am force-stop com.knowns.customer");
    }
}