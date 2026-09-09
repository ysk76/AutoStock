using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ADBKnowns;


public class BotWorker
{
    string device;

    int count = 0;
    int end = 0;
    int stack = 0;

    string EqualText = "";
    int EqualNum = 0;

    string getPointText = "";

    string path = "";

    public BotWorker(string device)
    {
        this.device = device;
    }

    public void Run()
    {
        bool flow = true;

        var startTime = DateTime.Now;

        var imageSize = AdbHelper.GetXandY(device);
        var x = "";
        var y = "";

        if (imageSize.Contains(":"))
        {
            var XandY = imageSize.Split(':')[1];
            x = XandY.Split('x')[0].Trim();
            y = XandY.Split('x')[1].Trim();
        }
        else
        {
            flow = false;
        }


        while (flow)
        {
            var random = new Random();
            int aaa = random.Next(2, 5);
            Thread.Sleep(aaa * 50);

            //経過時間
            var endTime = DateTime.Now;
            TimeSpan timeSpan = endTime - startTime;

            //右手操作のX軸
            var rightx = Convert.ToInt32(int.Parse(x) * 0.5);
            var leftx = Convert.ToInt32(int.Parse(x) * 0.8);
            int randomX = random.Next(rightx, leftx);


            var directryName = device.Split(':')[1];
            path = $@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //全画面スクリーンショット
            Bitmap screen = ScreenCapture.Capture(device, directryName);

            if(screen.Height == 1 && screen.Width == 1)
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string filePath = Path.Combine(@"C:\VisualStudio\ADBKnowns\Log", fileName);
                File.AppendAllText(filePath, $"{device}:接続失敗" + Environment.NewLine);
                flow = false;
            }

            //ホーム画面に戻っていないか
            Bitmap homeImage = ScreenCapture.HomeImage(directryName);
            var homeTexts = OCR.ocr($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\homeImage.bmp");

            if (homeTexts.Contains("Knowns"))
            {
                var clickX = Convert.ToInt32(int.Parse(x) * 0.78);
                var clickY = Convert.ToInt32(int.Parse(y) * 0.17);
                Thread.Sleep(3500);
                AdbHelper.Tap(device, clickX, clickY);
                continue;
            }

            //閉じるを探す
            Bitmap closeMark = ScreenCapture.CloseMark(directryName);
            var closeTexts = OCR.ocr($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\Close.bmp");

            if (closeTexts.Contains("閉じる"))
            {
                var clickX = Convert.ToInt32(int.Parse(x) * 0.5);
                var clickY = Convert.ToInt32(int.Parse(y) * 0.92);
                AdbHelper.Tap(device, clickX, clickY + random.Next(-2, 2));
                continue;
            }

            //広告付きGETボタン
            Bitmap getMark = ScreenCapture.GetMark(directryName);
            var getTexts = OCR.ocr($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\Get.bmp");

            if (getTexts.Contains("GET"))
            {
                //var clickX = int.Parse(x) / 5;
                var clickY = Convert.ToInt32(int.Parse(y) * 0.75);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-15, 15));
                Thread.Sleep(50);
                clickY = Convert.ToInt32(int.Parse(y) * 0.7);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-15, 15));
                clickY = Convert.ToInt32(int.Parse(y) * 0.64);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-15, 15));
                Thread.Sleep(50);
                clickY = Convert.ToInt32(int.Parse(y) * 0.58);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-30, 30));
                clickY = Convert.ToInt32(int.Parse(y) * 0.52);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-30, 30));
                continue;
            }



            //キャプチャ画像の前処理
            Preprocessing preprocessing = new Preprocessing();
            preprocessing.RectangleBlack(directryName);
            string text = OCR.ocr($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\preprocessingBitmap.bmp");

            double one = 0.61;
            double two = 0.685;
            double three = 0.76;
            double four = 0.83;

            double start = 0.65;
            double ad2 = 0.64;
            double ad3 = 0.46;
            double ad4 = 0.75;
            double ad5 = 0.7;
            double ad6 = 0.58;

            //閉じるを探す
            Bitmap advertisement = ScreenCapture.advertisement(directryName);
            var advertisementTexts = OCR.ocr($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\advertisement.bmp");

            if (advertisementTexts.Contains("前にもどる"))
            {
                one = 0.68;
                two = 0.755;
                three = 0.83;
                four = 0.9;

                start = 0.72;
                ad2 = 0.71;
                ad3 = 0.53;
                ad4 = 0.82;
                ad5 = 0.77;
                ad6 = 0.65;
            }


            text = text.Replace(" ", "").Replace(":", "").Replace(";", "").Replace("・", "").Replace("、", "").Replace("「", "").Replace("」", "").Replace("]", "");

            if (EqualText == text)
            {
                EqualNum++;
            }
            else
            {
                EqualText = text;
                EqualNum = 0;
            }

            if (EqualNum >= 15)
            {
                AdbHelper.TaskKill(device);
                EqualNum = 0;
            }


            if (text.Contains("選択肢をひとつずつ提示します") ||
                text.Contains("商品を今後購入したいと思いますか") ||
                text.Contains("通じて商品やサービスを知ること") ||
                text.Contains("直接商品やサービスの購入利用") ||
                text.Contains("のイメージにあてはまりますか") ||
                text.Contains("企業イメージにあてはまりますか"))
            {
                //3 or 4
                int randomNumber = random.Next(1, 3); // 1以上5未満のランダムな整数

                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * three);
                if (randomNumber % 2 == 0)
                {
                    clickY = Convert.ToInt32(int.Parse(y) * three);
                }
                else
                {
                    clickY = Convert.ToInt32(int.Parse(y) * four);
                }
                AdbHelper.Tap(device, randomX, clickY + random.Next(-30, 40));
                Thread.Sleep(randomNumber * 50);

                count = 0;
                stack = 0;
            }
            else if (text.Contains("についてお聞きします"))
            {
                //var clickX = int.Parse(x) / 2;
                var clickY = int.Parse(y) / 2;

                AdbHelper.Tap(device, randomX, clickY + random.Next(50, 500));
                count = 0;
                stack = 0;
            }
            else if (text.Contains("を知っています") ||
                        text.Contains("を利用したことがありますか") ||
                        text.Contains("を最後に利用したのはいつ") ||
                        text.Contains("を今後利用したいと思いますか") ||
                        text.Contains("焼き鳥ではないもの") ||
                        text.Contains("掃除をするときに使わないもの") ||
                        text.Contains("へびカメレオン") ||
                        text.Contains("クリスマスにプレゼントを配るのは誰でしょう") ||
                        text.Contains("TDRとはどのテーマパークの事でしょう") ||
                        text.Contains("コーヒーにミルクを入れた飲み物") ||
                        text.Contains("ー般道沿いでお土産物等を販売している無料休憩施設") ||
                        text.Contains("日光の三ザルとは見ざる言わざるともう一つは") ||
                        text.Contains("部屋に入る前にトントントンとドアをたたく事を") ||
                        text.Contains("母の日によく贈られる花を選んでくだ") ||
                        text.Contains("白くないものを選んでください") ||
                        text.Contains("空の移動に利用する乗り物") ||
                        text.Contains("三密ではないもの") ||
                        text.Contains("血液型にないものを") ||
                        text.Contains("こどもの日を選んでください") ||
                        text.Contains("肉食動物を選んでください") ||
                        text.Contains("マリンスポーツではないもの") ||
                        text.Contains("海外旅行をする時に必要なものは") ||
                        text.Contains("アンパンマンのパンの種類") ||
                        text.Contains("結婚式で花嫁さんが投げるもの") ||
                        text.Contains("商品を購入したことがありますか") ||
                        text.Contains("アルコール飲料ではないもの") ||
                        text.Contains("あなたの【性別】を") ||
                        text.Contains("自動車を減速停止させる機能") ||
                        text.Contains("雨の日が多くなる時期") ||
                        text.Contains("リンゴの英訳") ||
                        text.Contains("コンビニエンスストアでないもの") ||
                        text.Contains("空に見えるもの") ||
                        text.Contains("座れないものを") ||
                        text.Contains("トカゲフクロウ") ||
                        text.Contains("運動会の競技にないもの") ||
                        text.Contains("英語のありがとう") ||
                        text.Contains("人差し指中指") ||
                        text.Contains("スーツを着た時に首に絞めるもの") ||
                        text.Contains("キノコではないもの") ||
                        text.Contains("花粉症とは一般的に何の") ||
                        text.Contains("お正月にお参りに行くこと") ||
                        text.Contains("ご結婚はされていますか") ||
                        text.Contains("ピアノの音階ではないもの") ||
                        text.Contains("体温を測る道具") ||
                        text.Contains("バイリンガルとはなんでしょう") ||
                        text.Contains("警察への緊急電話番号") ||
                        text.Contains("クリスマスイブは何月何日") ||
                        text.Contains("鳥の数え方") ||
                        text.Contains("海鮮物ではないもの") ||
                        text.Contains("手足のない") ||
                        text.Contains("白雪姫が食べてしまった") ||
                        text.Contains("母の日によく贈られる花を選んでくだ") ||
                        text.Contains("白くないものを選んでください") ||
                        text.Contains("空の移動に利用する乗り物") ||
                        text.Contains("三密ではないもの") ||
                        text.Contains("血液型にないものを") ||
                        text.Contains("こどもの日を選んでください") ||
                        text.Contains("クリスマスにプレゼントを配る") ||
                        text.Contains("聞き取りを補う医療機器") ||
                        text.Contains("サンマの漢字はどれ") ||
                        text.Contains("次のうち東京にないもの") ||
                        text.Contains("二か国語を使いこなすこと") ||
                        text.Contains("作られていない家") ||
                        text.Contains("涼むためのもの") ||
                        text.Contains("学校行事にないもの") ||
                        text.Contains("ポイ活とは") ||
                        text.Contains("という企業名を") ||
                        text.Contains("という企業の事業内容") ||
                        text.Contains("への信頼度を") ||
                        text.Contains("のサービスを今後") ||
                        text.Contains("掃除をするときに使わないもの") ||
                        text.Contains("グッズを購入したことがありますか") ||
                        text.Contains("グッズを今後購入したいですか") ||
                        text.Contains("コラボ商品やサービスを購入または利用") ||
                        text.Contains("雪姫が食べてしまったもの") ||
                        text.Contains("飛べる虫を選んでください") ||
                        text.Contains("卵料理ではないもの") ||
                        text.Contains("ボールを使わないスポーツ") ||
                        text.Contains("購入したい購入したくない") ||
                        text.Contains("オリンピックのメダルの種類にないもの") ||
                        text.Contains("冷たい食べ物") ||
                        text.Contains("すっぱい食べ物はどれ") ||
                        text.Contains("鳥類ではない動物") ||
                        text.Contains("パンダの漢字はどれ") ||
                        text.Contains("食べられないパン") ||
                        text.Contains("商品を購入したことがありま") ||
                        text.Contains("購入したことがある購入したことはない") ||
                        text.Contains("日本の都市を選んで") ||
                        text.Contains("目当てにその作品を観る読む聞くなどしたことがありますか") ||
                        text.Contains("作品を今後観る読む聞くなどしたいですか") ||
                        text.Contains("起用された商品やサービスを購入または利用したいですか") ||
                        text.Contains("目当てにその活動を観るなどしたことがありますか") ||
                        text.Contains("活動を今後観るなどしたいですか") ||
                        text.Contains("夏の風物詩ではないもの") ||
                        text.Contains("成長するとカエルになるもの") ||
                        text.Contains("観たことがある観たことはない") ||
                        text.Contains("を今後観たいですか") ||
                        text.Contains("コラボしている商品やサービスを購入または利用したいですか") ||
                        text.Contains("起用されたコラボが商品やサービスを購入または利用したいですか") ||
                        text.Contains("をプレイしたことがありますか") ||
                        text.Contains("を今後プレイしたいですか") ||
                        text.Contains("がコラボしている商品やサービス") ||
                        text.Contains("を今後読みたいですか") ||
                        text.Contains("のグッズを購入したことがありますか") ||
                        text.Contains("プレイしたいプレイしたくない") ||
                        text.Contains("を今後観たり読みたいですか")||
                        text.Contains("観たい観たくない") ||
                        text.Contains("購入利用したい購入利用したくない") ||
                        text.Contains("?知っている") ||
                        text.Contains("を読んだことがありますか") ||
                        text.Contains("を最後に読んだのはいつですか") ||
                        text.Contains("を観たことがありますか") ||
                        text.Contains("読んだことがある") ||
                        text.Contains("ペンギンソウトカゲ") ||
                        text.Contains("いる商品やサービスを購入または利用したいですか") ||
                        text.Contains("りますか?購入したことがある") ||
                        text.Contains("ありますか?観たことがある") ||
                        text.Contains("ビスを購入または利用したいですか") ||
                        text.Contains("名前も見た目も知っていた名前だけ知っていた") ||
                        text.Contains("知っている") && text.Length <= 10 ||
                        text.Contains("日本の首都") ||
                        text.Contains("おとぎ話三匹の") ||
                        text.Contains("お菓子作りの職人を"))
            {
                //3
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * three);

                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 30));
                //Thread.Sleep(300);
                Thread.Sleep(50);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("交番にいるのは誰でしょう") ||
                        text.Contains("背面の甲羅を持つ生物") ||
                        text.Contains("生まれた月") ||
                        text.Contains("1月2月3月4月") ||
                        text.Contains("ヒヨコとはなんの子供") ||
                        text.Contains("野菜でないもの") ||
                        text.Contains("ヘアケア製品ではないのもの") ||
                        text.Contains("世界最速の陸上動物") ||
                        text.Contains("携帯電話会社ではないもの") ||
                        text.Contains("もみあげとは何でしょう") ||
                        text.Contains("日本で一番大きい湖はどれでしょう") ||
                        text.Contains("日本昔話桃太郎でお供になっていない動物を") ||
                        text.Contains("節分に撤くものは") ||
                        text.Contains("浦島太郎が助けた動物") ||
                        text.Contains("オーストラリアにいてユーカリを食べる動物") ||
                        text.Contains("毛が生えていない動物はどれ") ||
                        text.Contains("地球の形を選んで") ||
                        text.Contains("カマボコの原材料は") ||
                        text.Contains("夏の期間授業や業務を休みにする期間") ||
                        text.Contains("ヨーグルトは何からできている") ||
                        text.Contains("日本で夏に行われる祖先の霊を祀る行事") ||
                        text.Contains("海に暮らす動物") ||
                        text.Contains("牛乳の原材料はなんでしょう") ||
                        text.Contains("甘いものを選んで") ||
                        text.Contains("乗り物ではないもの") ||
                        text.Contains("じゃんけんの出し方にないもの") ||
                        text.Contains("七夕にかざるもの") ||
                        text.Contains("カットなどを主な仕事とする職業") ||
                        text.Contains("フルマラソンで走る距離") ||
                        text.Contains("シャツやブラウスの前を留めるもの") ||
                        text.Contains("手に塗るものはどれ") ||
                        text.Contains("ビールの原材料") ||
                        text.Contains("黄色い食べ物") ||
                        text.Contains("梅酒は何から作る") ||
                        text.Contains("世界三大珍味") ||
                        text.Contains("貝類ではないもの") ||
                        text.Contains("心臓はカラダのどこ") ||
                        text.Contains("シマウマの肌の色は何色") ||
                        text.Contains("クリスマスに関連しないもの") ||
                        text.Contains("夕に願いを書いた短") ||
                        text.Contains("現在のお住まいは次のうちどれに該当") ||
                        text.Contains("世帯の資産金額について最も近いもの") ||
                        text.Contains("世帯年収について最も近いもの") ||
                        text.Contains("自由に使えるお金の金額") ||
                        text.Contains("貯蓄している金額") ||
                        text.Contains("見つけたら幸せになると言われているもの") ||
                        text.Contains("ことわざで犬も歩けば") ||
                        text.Contains("お腹の袋でこどもを育てる") ||
                        text.Contains("吊り橋効果とはどんなこと") ||
                        text.Contains("聞くための感覚器") ||
                        text.Contains("昆虫を選んでください") ||
                        text.Contains("日本で夏に行われる祖先") ||
                        text.Contains("季節の種類ではない") ||
                        text.Contains("食べものではないもの") ||
                        text.Contains("七五三は") ||
                        text.Contains("洋服をかけるもの") ||
                        text.Contains("フランスの国旗に使われていない色") ||
                        text.Contains("お腹の袋でことどもを育てる") ||
                        text.Contains("主にお米を炊くため") ||
                        text.Contains("乳製品ではないもの") ||
                        text.Contains("赤ずきんに出てくる動物") ||
                        text.Contains("墨を吐く生き物はどれ") ||
                        text.Contains("トランプのマークではないもの") ||
                        text.Contains("夏の遊びを選んで") ||
                        text.Contains("雨の日に使わないもの") ||
                        text.Contains("日本の国歌を選んで") ||
                        text.Contains("果物ではないもの") ||
                        text.Contains("バレンタインデーに贈られるお菓子") ||
                        text.Contains("おみくじの結果でないもの") ||
                        text.Contains("座るためのものではないもの") ||
                        text.Contains("ラケットを使うスポーツ") ||
                        text.Contains("日本で一番高い山") ||
                        text.Contains("正午とは何時のこと") ||
                        text.Contains("吸盤がついた8本の腕") ||
                        text.Contains("花が咲かないもの") ||
                        text.Contains("15時12時") ||
                        text.Contains("中華料理ではないもの") ||
                        text.Contains("バットを使うスポーツ") ||
                        text.Contains("こうえんと読まない漢字") ||
                        text.Contains("マルイモディのお買い物が") ||
                        text.Contains("了解しました") ||
                        text.Contains("ジブリに登場するキャラクターを選んでください") ||
                        text.Contains("とても感じるやや感じる") ||
                        text.Contains("運転する際に免許が必要な乗り物") ||
                        text.Contains("金閣寺がある都道府県") ||
                        text.Contains("肉食肌竜を選んでください"))
            {
                //2
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * two);

                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));//1315
                Thread.Sleep(50);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("具体的な意見が特にない") ||
                        text.Contains("特に記載することがない場合は必ず") ||
                        text.Contains("いただいた回答は大切に活用させていただきます") ||
                        text.Contains("同じ文章の使いまわし他社サイトの") ||
                        text.Contains("洋東子ではないもの") ||
                        text.Contains("水中や水辺に暮らすの生き物を展示した施設") ||
                        text.Contains("乾燥した砂漠でも生育できる多肉植物") ||
                        text.Contains("アニメや映画の吹き替えをする仕事") ||
                        text.Contains("ワインの主な原料となる果物") ||
                        text.Contains("空を飛ぶ動物") ||
                        text.Contains("ヒヨコが成長した動物") ||
                        text.Contains("お米の品種名ではないもの") ||
                        text.Contains("四国にはない県") ||
                        text.Contains("宿泊施設ではないもの") ||
                        text.Contains("何ヶ月か選んでください") ||
                        text.Contains("食事をするときに使わない") ||
                        text.Contains("妊婦のマークを") ||
                        text.Contains("法律上の争訟を審理する仕事") ||
                        text.Contains("ひな祭りに飾っておくものは") ||
                        text.Contains("野菜を売っているお店") ||
                        text.Contains("多肉植物を選んで") ||
                        text.Contains("弦楽器でないもの") ||
                        text.Contains("三角形のものを") ||
                        text.Contains("セーターを編むために") ||
                        text.Contains("干支に存在しない動物") ||
                        text.Contains("牛丼チェーン店ではないものを") ||
                        text.Contains("洋菓子ではないもの") ||
                        text.Contains("トイレでつかうもの") ||
                        text.Contains("信号の色にないもの") ||
                        text.Contains("試験を受けることを何というか") ||
                        text.Contains("家族を表現する名称ではないもの") ||
                        text.Contains("海の中にいるけど魚ではない") ||
                        text.Contains("チェリーとはどの果物") ||
                        text.Contains("正月に届く新年を祝う挨拶状") ||
                        text.Contains("ドレミファ") ||
                        text.Contains("に続く2音はどれ") ||
                        text.Contains("ドラえもんの説明") ||
                        text.Contains("豆腐は何からできている") ||
                        text.Contains("二日前のことを何というか") ||
                        text.Contains("ポケモンのキャラクターを") ||
                        text.Contains("黒くない") ||
                        text.Contains("天気の種類にはない") ||
                        text.Contains("お仏壇に供えるもの") ||
                        text.Contains("長さの単位でないもの") ||
                        text.Contains("雨の日にさすもの") ||
                        text.Contains("中国の有名な観光地を選んで") ||
                        text.Contains("ピーチとはどの果物でしょう") ||
                        text.Contains("世界三大美人として知られる人物") ||
                        text.Contains("支に存在しない動物を選んで") ||
                        text.Contains("日本札の単位にないもの") ||
                        text.Contains("首の長い動物") ||
                        text.Contains("アメリカの通貨単位") ||
                        text.Contains("ワールドの和訳") ||
                        text.Contains("感情を表す漢字ではないもの") ||
                        text.Contains("顔のパーツにないもの") ||
                        text.Contains("磯辺揚げとはなんでしょう") ||
                        text.Contains("赤くないものを") ||
                        text.Contains("お子様はいらっしゃいますか") ||
                        text.Contains("現在ご自身の親と同居") ||
                        text.Contains("現在ご自身または配偶者の方は妊娠") ||
                        text.Contains("魚類を選んで") ||
                        text.Contains("新型コロナウィルスの予防に着ける") ||
                        text.Contains("浴室にはないもの") ||
                        text.Contains("サンタクロースが乗っている") ||
                        text.Contains("タマゴをフライパンに落として焼いたもの") ||
                        text.Contains("レーズンはどれ") ||
                        text.Contains("料理をするときには使わないもの") ||
                        text.Contains("子供の日とは") ||
                        text.Contains("令和の前の元号を") ||
                        text.Contains("USJとは") ||
                        text.Contains("履くためのものではない") ||
                        text.Contains("夏季オリンピックが開催される頻度") ||
                        text.Contains("雪が降るのはどの季節") ||
                        text.Contains("の株を現在持っています") ||
                        text.Contains("猫の手足のぷにぷにした部分") ||
                        text.Contains("音を出す打楽器") ||
                        text.Contains("校行事にないもの") ||
                        text.Contains("1か月が31日まである月を選んで") ||
                        text.Contains("大みそかを選んで") ||
                        text.Contains("何度倒れても起き上がることを意味する四字熟語") ||
                        text.Contains("の株を今後購入したいですか") ||
                        text.Contains("グッズを購入したいですか") ||
                        text.Contains("活動を今後観るなしたいですか") ||
                        text.Contains("今後プレイレしたいですか") ||
                        text.Contains("今後観たりみたいですか") ||
                        text.Contains("読みたい読みたくない") ||
                        text.Contains("今後観るなどたいですか") ||
                        text.Contains("ですか?観たい") ||
                        text.Contains("ますか?購入したことがある") ||
                        text.Contains("を購入または利用したいですか") ||
                        text.Contains("中体な見が特にない場合"))
            {
                //4
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * four);

                AdbHelper.Tap(device, randomX, clickY + random.Next(-5, 10));
                Thread.Sleep(50);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("をどのくらい好きですか") ||
                        text.Contains("を利用して満足しましたか") ||
                        text.Contains("食事をするときに使わないもの") ||
                        text.Contains("耳に着ける飾りをなんと言う") ||
                        text.Contains("お坊さんがいるのはどこでしょう") ||
                        text.Contains("草食動物を選んでください") ||
                        text.Contains("世界で一番高い山はどれでしょう") ||
                        text.Contains("節分に恵方を向いて食べるもの") ||
                        text.Contains("丸いものを選んで") ||
                        text.Contains("麺類ではないもの") ||
                        text.Contains("爪をメイクするもの") ||
                        text.Contains("日本が属する地域を") ||
                        text.Contains("ホワイトチョコレートとは") ||
                        text.Contains("結婚指輪は一般的に何指") ||
                        text.Contains("成虫になってから1週間ほどしか生きられない") ||
                        text.Contains("耳に着ける飾りをなんと") ||
                        text.Contains("サッカーのゴールを守る") ||
                        text.Contains("2月14日は何の日") ||
                        text.Contains("魚の種類ではないもの") ||
                        text.Contains("犬種ではないもの") ||
                        text.Contains("芝生の上で行うスポーツは") ||
                        text.Contains("氷上のスポーツでないものを") ||
                        text.Contains("中国の首都を選んでください") ||
                        text.Contains("桃太郎が行ったのは次のうち") ||
                        text.Contains("絵の具の赤と白を混ぜると何色になる") ||
                        text.Contains("カミナリが鳴って空が光る現象") ||
                        text.Contains("次のうち川を選んでください") ||
                        text.Contains("熊本県のマスコットキャラクター") ||
                        text.Contains("フランスの首都を選んでください") ||
                        text.Contains("ヒゲをそる時に使う物") ||
                        text.Contains("液体ではないもの") ||
                        text.Contains("60歳1歳5歳") ||
                        text.Contains("ミツバチが集めてくるもの") ||
                        text.Contains("夏に現れる夜になるとお尻が光る虫") ||
                        text.Contains("英語でMONDAYは何曜日か") ||
                        text.Contains("イチゴを漢字で書いたら") ||
                        text.Contains("スマイルの意味") ||
                        text.Contains("温まるためのもの") ||
                        text.Contains("オタマジャクシが成長すると何になる") ||
                        text.Contains("ミッキーマウスはどの動物がモチーフ") ||
                        text.Contains("果物を選んでください") ||
                        text.Contains("食事の時に出てくる味噌を使った料理") ||
                        text.Contains("金属ではないもの") ||
                        text.Contains("電車に乗る時に必要なものは") ||
                        text.Contains("こどもの日に飾っておくもの") ||
                        text.Contains("耳があるパンは次のうち") ||
                        text.Contains("夢を食べると言われる動物") ||
                        text.Contains("図書館で借りるもの") ||
                        text.Contains("邦画でないもの") ||
                        text.Contains("清水の舞台から飛び降りる") ||
                        text.Contains("キャットフードを食べる動物") ||
                        text.Contains("初夢に見ると縁起の良い夢") ||
                        text.Contains("日本にある湖") ||
                        text.Contains("水上のスポーツでないもの") ||
                        text.Contains("元旦を選んでください") ||
                        text.Contains("ブラジルのダンスとして有名なもの") ||
                        text.Contains("中国の首都を選んでください") ||
                        text.Contains("桃太郎が行ったのは次のうち") ||
                        text.Contains("絵の具の赤と白を混ぜると何色になる") ||
                        text.Contains("カミナリが鳴って空が光る現象") ||
                        text.Contains("次のうち川を選んでください") ||
                        text.Contains("熊本県のマスコットキャラクター") ||
                        text.Contains("フランスの首都を選んでください") ||
                        text.Contains("ヒゲをそる時に使う物") ||
                        text.Contains("サッカーのゴールを守る") ||
                        text.Contains("2月14日は何の日") ||
                        text.Contains("横に歩く生き物") ||
                        text.Contains("現在世帯にあなたを含めて何名") ||
                        text.Contains("あなたの最終学歴を") ||
                        text.Contains("個人の資産金額について最も近いもの") ||
                        text.Contains("あなた個人の年収について") ||
                        text.Contains("動画を見て再チャレンジ") ||
                        text.Contains("どの程度接していますか") ||
                        text.Contains("ハンバーガーチェーン店ではないもの") ||
                        text.Contains("日本の桜の季節") ||
                        text.Contains("アンパンマンの主要キャラクターではないの") ||
                        text.Contains("首に巻かないもの") ||
                        text.Contains("靴の種類ではないもの") ||
                        text.Contains("ピンクリボンとはなんでしょう") ||
                        text.Contains("次のうちで寒くない場所") ||
                        text.Contains("二輪車を選んでください") ||
                        text.Contains("とても好きやや好き") ||
                        text.Contains("鮭の卵を選んで") ||
                        text.Contains("名前も顔も知っている名前だけ知っている") ||
                        text.Contains("邦画でないもの") ||
                        text.Contains("日本で一番南にある県"))
            {
                //1
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * one);

                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 70));
                Thread.Sleep(50);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("を利用される目的シーンを") ||
                        text.Contains("をどなたと利用する事が多いですか") ||
                        text.Contains("複数選択可") ||
                        text.Contains("数選択可)") ||
                        text.Contains("ついてあなたに当てはまるものを全て選択") ||
                        text.Contains("ファンクラブやメンバーシップ等の有料会員になりたい") ||
                        text.Contains("を観た理由はなんですか") ||
                        text.Contains("ファッション(Tシャツ/パーカー/バッグ等)") ||
                        text.Contains("を観る理由はなんですか") ||
                        text.Contains("キャラクターくじ(コンビニくじ") ||
                        text.Contains("手がけた/歌う楽曲を聴きたい") ||
                        text.Contains("手がけた書籍を読みたい") ||
                        text.Contains("クッション/抱き枕") ||
                        text.Contains("男女混合3人以上") ||
                        text.Contains("ライブ配信や動画を観たい") ||
                        text.Contains("をプレイする理由はなんですか") ||
                        text.Contains("を読む理由はなんですか") ||
                        text.Contains("をどのくらい好きですか") ||
                        text.Contains("を観たり読む理由はなんですか") ||
                        text.Contains("についてあなたに当てはまるものを全て選んでください") ||
                        text.Contains("知識情報を得られる") ||
                        text.Contains("観る時に利用する媒体はどれ") ||
                        text.Contains("話のネタにできる") ||
                        text.Contains("を観た時に利用した媒体はどれ") ||
                        text.Contains("を観たり読む理由はなんですか"))
            {
                //スクロール

                AdbHelper.ScrollDown(device);
                Thread.Sleep(500);
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * three);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-20, 50));
                Thread.Sleep(500);
                //var clickXx = int.Parse(x) / 2;
                var clickYy = Convert.ToInt32(int.Parse(y) * four);
                AdbHelper.Tap(device, randomX, clickYy + random.Next(-10, 65));
                Thread.Sleep(500);

                if(stack >= 15)
                {
                    AdbHelper.TaskKill(device);
                    stack = 0;
                }
                stack++;
                count = 0;
            }
            else if (text.Contains("アンケートを始めましょう") ||
                        text.Contains("今日のアンケート確認完了") ||
                        text.Contains("報酬アップのアンケートを開始") ||
                        text.Contains("もし特にイメージがない場合は必ずスキップしてください"))
            {
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * start);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-30, 30));
                count = 0;
                stack = 0;
            }
            else if (text.Contains("生まれた年"))
            {
                AdbHelper.ScrollDown(device);
                Thread.Sleep(1000);
                AdbHelper.ScrollDown(device);
                Thread.Sleep(1000);
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * three);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-15, 15));
                //var clickXx = int.Parse(x) / 2;
                var clickYy = Convert.ToInt32(int.Parse(y) * four);
                AdbHelper.Tap(device, randomX, clickYy + random.Next(-20, 20));
                count = 0;
                stack = 0;
            }
            else if (text.Contains("に対する満足度を教えてください") ||
                        text.Contains("を家族や友人におすすめしたいと思いますか") ||
                        text.Contains("とても満足やや満足") ||
                        text.Contains("出身地の都道府県") ||
                        text.Contains("商品を最後に購入したのはいつですか") ||
                        text.Contains("の商品をこの1年間でどの程度購入") ||
                        text.Contains("あなたの職業を教えてください") ||
                        text.Contains("あなたの職業を教えて") ||
                        text.Contains("働いている業界を教えて") ||
                        text.Contains("作品を最後に観る読む聞くなどしたのはいつ") ||
                        text.Contains("活動を最後に観るなどしたのはいつですか") ||
                        text.Contains("最後に観たのはいつですか") ||
                        text.Contains("の将来性を感じますか") ||
                        text.Contains("で働きたいですか") ||
                        text.Contains("の活動をテレビや新聞WEB記事などで見ることがありますか") ||
                        text.Contains("の広告をテレビやWEB街中で見ることがありますか") ||
                        text.Contains("を最近半年間でどの程度利用されましたか") ||
                        text.Contains("を最後にプレイしたのはいつですか") ||
                        text.Contains("に満足しましたか") ||
                        text.Contains("を最後に読んだのはいつですか") ||
                        text.Contains("1ヶ月以内3か月以内") ||
                        text.Contains("とても見るたまに見る") ||
                        text.Contains("1万円未満"))
            {
                int randomNumber = random.Next(1, 5); // 1以上5未満のランダムな整数

                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * one);
                if (randomNumber == 1)
                {
                    clickY = Convert.ToInt32(int.Parse(y) * one);
                }
                else if (randomNumber == 2)
                {
                    clickY = Convert.ToInt32(int.Parse(y) * two);
                }
                else if (randomNumber == 3)
                {
                    clickY = Convert.ToInt32(int.Parse(y) * three);
                }

                AdbHelper.Tap(device, randomX, clickY + random.Next(-20, 20));
                Thread.Sleep(randomNumber * 50);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("広告音量に") ||
                        text.Contains("についてお聞きしま") ||
                        text.Contains("広告が流れます") ||
                        text.Contains("についてお聞します") ||
                        text.Contains("最大4コイン") ||
                        text.Contains("最大5コイン") ||
                        text.Contains("最大3コイン") ||
                        text.Contains("コインを受け取る"))
            {
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * three);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                clickY = Convert.ToInt32(int.Parse(y) * ad2);
                AdbHelper.Tap(device, randomX, clickY);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("1年間でいくらまで使いたい") ||
                        text.Contains("に対して1年間でいくら使いたいですか") ||
                        text.Contains("に1年間でいくらまてで使いたいですか") ||
                        text.Contains("の活動に対して1年間でいくら使いたいですか") ||
                        text.Contains("1年間でいくら使いたいですか"))
            {
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * ad3);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-200, 100));
            }
            else if (text.Contains("次のアンケートへ進む") ||
                        text.Contains("抽選でボーナスコインをプレゼント") ||
                        text.Contains("アプリリニューアルに伴うポイント") ||
                        text.Contains("次に開いたときは途中から再開できます") ||
                        text.Contains("デジコ商品券交換に関する不具合について") ||
                        text.Contains("デジコ交換機能の不具合について") ||
                        text.Contains("Amazonギフト交換機能一時停止のお知らせ") ||
                        text.Contains("Amazonギフトカード交換の再開見込みについて") ||
                        text.Contains("Amazonギフトの交換再開と仕様変更のお知らせ") ||
                        text.Contains("コンプリート後の") ||
                        text.Contains("すべて回答後の") ||
                        text.Contains("GET") ||
                        text.Contains("ー部ギフト交換に関する不具合復旧のお知らせ") ||
                        text.Contains("特典あり"))
            {
                //var clickX = int.Parse(x) / 5;
                var clickY = Convert.ToInt32(int.Parse(y) * ad4);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                Thread.Sleep(300);
                clickY = Convert.ToInt32(int.Parse(y) * ad5);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                clickY = Convert.ToInt32(int.Parse(y) * ad2);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                Thread.Sleep(300);
                clickY = Convert.ToInt32(int.Parse(y) * ad6);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                clickY = Convert.ToInt32(int.Parse(y) * 0.8);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                clickY = Convert.ToInt32(int.Parse(y) * 0.85);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                count = 0;
                stack = 0;
            }
            else if (text.Contains("サポートOSバージョン変更予定のお知らせ"))
            {
                //var clickX = int.Parse(x) / 5;
                var clickY = Convert.ToInt32(int.Parse(y) * 0.8);
                AdbHelper.Tap(device, randomX, clickY + random.Next(-10, 10));
                Thread.Sleep(300);
            }
            else if (text.Contains("今日のトリビア"))
            {
                //var clickX = int.Parse(x) / 2;
                var clickY = Convert.ToInt32(int.Parse(y) * 0.92);

                AdbHelper.Tap(device, randomX, clickY + random.Next(-5, 10));
                Thread.Sleep(50);
                count = 0;
                stack = 0;
            }
            else if (text.Contains("また明日お会い") ||
                     text.Contains("すべてのチャレンジCOMPLETE"))
            {
                end++;
                if (end >= 5)
                {
                    //正常終了
                    string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    string filePath = Path.Combine(@"C:\VisualStudio\ADBKnowns\Log", fileName);
                    File.AppendAllText(filePath, $"{device}:正常終了。経過時間:{(int)timeSpan.TotalMinutes}" + Environment.NewLine);

                    Bitmap getPoint = ScreenCapture.GetPoint(directryName);
                    getPointText = OCR.ocr($@"C:\VisualStudio\ADBKnowns\Emulator\{directryName}\GetPoint.bmp");

                    flow = false;
                    end = 0;
                }
            }
            else if ((int)timeSpan.TotalMinutes >= 50)
            {
                //時間切れ
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string filePath = Path.Combine(@"C:\VisualStudio\ADBKnowns\Log", fileName);
                File.AppendAllText(filePath, $"{device}:時間切れ" + text + Environment.NewLine);
                flow = false;

            }
            else if (text.Contains("Chromeを自分用にカスタマイズ") ||
                     text.Contains("FedExはクッキーを使用しています"))
            {
                AdbHelper.Home(device);
            }
            else
            {
                count++;
                if (count >= 7)
                {
                    random = new Random();
                    int randomNumber = random.Next(1, 5); // 1以上5未満のランダムな整数

                    if (randomNumber % 2 == 0)
                    {
                        AdbHelper.Home(device);
                    }
                    else
                    {
                        AdbHelper.TaskKill(device);
                    }

                    count = 0;
                }
            }
        }
    }
}