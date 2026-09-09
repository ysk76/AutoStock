using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    [DllImport("powrprof.dll", SetLastError = true)]
    public static extern bool SetSuspendState(bool hibernate, bool forceCrtical, bool disableWakeEvent);
    
    //並列処理制御(MAX3つ)
    static SemaphoreSlim semaphore = new SemaphoreSlim(2);

    //非同期処理
    static async Task Main()
    {
        string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
        string filePath = Path.Combine(@"C:\VisualStudio\ADBKnowns\Log", fileName);
        File.AppendAllText(filePath, fileName + Environment.NewLine);

        //起動するショートカットリスト
        var shortcuts = new List<(string shortcut, string device)>
        {
            (@"C:\KnownsList\Player1.lnk", "127.0.0.1:5555"),
            (@"C:\KnownsList\Player2.lnk", "127.0.0.1:5585"),
            (@"C:\KnownsList\Player9.lnk", "127.0.0.1:5645"),
            (@"C:\KnownsList\Player17.lnk", "127.0.0.1:5725"),
            (@"C:\KnownsList\Player18.lnk", "127.0.0.1:5735"),
            (@"C:\KnownsList\Player21.lnk", "127.0.0.1:5765"),
            (@"C:\KnownsList\Player23.lnk", "127.0.0.1:5785"),
            (@"C:\KnownsList\Player30.lnk", "127.0.0.1:5855"),
            (@"C:\KnownsList\Player31.lnk", "127.0.0.1:5865"),
            (@"C:\KnownsList\Player32.lnk", "127.0.0.1:5875"),
            (@"C:\KnownsList\Player34.lnk", "127.0.0.1:5895"),
            (@"C:\KnownsList\Player35.lnk", "127.0.0.1:5905"),
            //(@"C:\KnownsList\Player36.lnk", "127.0.0.1:5915"),
            (@"C:\KnownsList\Player37.lnk", "127.0.0.1:5925"),
            (@"C:\KnownsList\Player39.lnk", "127.0.0.1:5945"),
            (@"C:\KnownsList\Player40.lnk", "127.0.0.1:5955"),
            (@"C:\KnownsList\Player43.lnk", "127.0.0.1:5985"),
            //(@"C:\KnownsList\Player46.lnk", "127.0.0.1:6015"),
            //(@"C:\KnownsList\Player50.lnk", "127.0.0.1:6055"),
            (@"C:\KnownsList\Player51.lnk", "127.0.0.1:6065"),
            ////////(@"C:\KnownsList\Player52.lnk", "127.0.0.1:6075"),
            (@"C:\KnownsList\Player56.lnk", "127.0.0.1:6115"),
            //(@"C:\KnownsList\Player57.lnk", "127.0.0.1:6125"),
            (@"C:\KnownsList\Player61.lnk", "127.0.0.1:6165"),
            (@"C:\KnownsList\Player63.lnk", "127.0.0.1:6185"),
            (@"C:\KnownsList\Player67.lnk", "127.0.0.1:6225"),
            //(@"C:\KnownsList\Player68.lnk", "127.0.0.1:6235"),
            (@"C:\KnownsList\Player69.lnk", "127.0.0.1:6245"),
            (@"C:\KnownsList\Player75.lnk", "127.0.0.1:6305"),
            (@"C:\KnownsList\Player76.lnk", "127.0.0.1:6315"),
            //(@"C:\KnownsList\Player77.lnk", "127.0.0.1:6325"),
            //(@"C:\KnownsList\Player79.lnk", "127.0.0.1:6345"),
            (@"C:\KnownsList\Player82.lnk", "127.0.0.1:6375"),
            //(@"C:\KnownsList\Player83.lnk", "127.0.0.1:6385"),
            (@"C:\KnownsList\Player84.lnk", "127.0.0.1:6395"),
            (@"C:\KnownsList\Player85.lnk", "127.0.0.1:6405"),
            //(@"C:\KnownsList\Player86.lnk", "127.0.0.1:6415"),
            //(@"C:\KnownsList\Player87.lnk", "127.0.0.1:6425"),
            (@"C:\KnownsList\Player88.lnk", "127.0.0.1:6435"),
            (@"C:\KnownsList\Player89.lnk", "127.0.0.1:6445"),
            (@"C:\KnownsList\Player90.lnk", "127.0.0.1:6455"),
            (@"C:\KnownsList\Player92.lnk", "127.0.0.1:6475"),
            (@"C:\KnownsList\Player95.lnk", "127.0.0.1:6505"),
            //(@"C:\KnownsList\Player96.lnk", "127.0.0.1:6515"),
            (@"C:\KnownsList\Player97.lnk", "127.0.0.1:6525"),
            (@"C:\KnownsList\Player98.lnk", "127.0.0.1:6535"),
            (@"C:\KnownsList\Player102.lnk", "127.0.0.1:6575"),
            (@"C:\KnownsList\Player103.lnk", "127.0.0.1:6585"),
            (@"C:\KnownsList\Player105.lnk", "127.0.0.1:6605"),
            (@"C:\KnownsList\Player106.lnk", "127.0.0.1:6615"),
            (@"C:\KnownsList\Player107.lnk", "127.0.0.1:6625"),
            (@"C:\KnownsList\Player108.lnk", "127.0.0.1:6635"),
            (@"C:\KnownsList\Player113.lnk", "127.0.0.1:6685"),
            (@"C:\KnownsList\Player114.lnk", "127.0.0.1:6695"),
            (@"C:\KnownsList\Player115.lnk", "127.0.0.1:6705"),
            (@"C:\KnownsList\Player116.lnk", "127.0.0.1:6715"),
            (@"C:\KnownsList\Player121.lnk", "127.0.0.1:6765"),
        };

        List<Task> tasks = new List<Task>();

        Random rand = new Random();      
        AdbHelper.StartServer();

        /*
        // シャッフル
        var shuffled = shortcuts.OrderBy(x => rand.Next()).ToList();
        foreach (var shortcut in shuffled)
        {
            tasks.Add(RunInstance(shortcut.shortcut, shortcut.device));
            Thread.Sleep(20000);
        }

        await Task.WhenAll(tasks);
        */

        List<string> endlist = new List<string>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream) // ファイルの終端まで繰り返し
            {
                string line = reader.ReadLine(); // 1行を読み込む
                if (line.Contains("正常終了"))
                    endlist.Add(line);
            }
        }

        // シャッフル
        var shuffled = shortcuts.OrderBy(x => rand.Next()).ToList();
        foreach (var shortcut in shuffled)
        {
            bool frg = true;

            var shortcutip = shortcut.ToString().Split(',')[1];
            foreach(var end in endlist)
            {
                var ip = end.Split(':')[0] + ":" + end.Split(':')[1];
                if (shortcutip.Contains(ip))
                {
                    frg = false;
                    break;
                }
            }

            if (frg)
            {
                tasks.Add(RunInstance(shortcut.shortcut, shortcut.device));
                Thread.Sleep(20000);
            }
        }

        await Task.WhenAll(tasks);



        List<string> errorlist = new List<string>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream) // ファイルの終端まで繰り返し
            {
                string line = reader.ReadLine(); // 1行を読み込む
                if(line.Contains("時間切れ") || line.Contains("接続失敗"))
                    errorlist.Add(line);
            }
        }

        foreach (var list in errorlist)
        {
            var ip = list.Split(':')[0] + ":" + list.Split(':')[1];
            for(int  i = 0; i < shortcuts.Count; i++)
            {
                if (shortcuts[i].ToString().Contains(ip))
                {
                    tasks.Add(RunInstance(shortcuts[i].shortcut, shortcuts[i].device));
                    Thread.Sleep(20000);
                }
            }
        }

        await Task.WhenAll(tasks);

        //スリープ
        SetSuspendState(false, false, false);
        //プログラム終了
        Environment.Exit(0);
    }

    //1インスタンス担当
    static async Task RunInstance(string shortcut, string device)
    {
        //同時実行枠を確認(3枠)
        await semaphore.WaitAsync();

        Process emulatorProcess = null;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = shortcut,
                Verb = "runas",
                UseShellExecute = true
            };

            //ショートカットを実行
            emulatorProcess = Process.Start(psi);

            //起動待ち。とりあえず20秒
            await Task.Delay(40000);

            //ADB接続可能インスタンスを確認
            var connected = AdbHelper.GetDevices(device);

            //ADB接続確認
            if (!connected)
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string filePath = Path.Combine(@"C:\VisualStudio\ADBKnowns\Log", fileName);
                File.AppendAllText(filePath, $"{device}:ADB接続失敗" + Environment.NewLine);
                return;
            }

            //エミュレーター操作
            var bot = new BotWorker(device);

            //実行
            await Task.Run(() => bot.Run());
        }
        finally
        {
            //エミュレーターを解放
            if (emulatorProcess != null && !emulatorProcess.HasExited)
                emulatorProcess.Kill();

            //処理が終わったら枠を解放
            semaphore.Release();
        }
    }
}