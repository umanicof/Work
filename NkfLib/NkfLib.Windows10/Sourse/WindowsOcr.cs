using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Globalization;
using System.Windows;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Windows.Media;

namespace NkfLib
{
    /// <summary>
    /// Windows OCRクラス
    /// </summary>
    public partial class WindowsOcr
    {
        // SoftwareBitmap変換時に使用するフォーマット
        // ・BitmapPixelFormet: Gray8, Gray16, P010, Nv12, Yuy2, Unknown は変換時に例外発生
        //static readonly BitmapPixelFormat kBitmapPixelFormat = BitmapPixelFormat.Rgba16;
        //static readonly BitmapAlphaMode kBitmapAlphaMode = BitmapAlphaMode.Premultiplied;
        static readonly BitmapPixelFormat kBitmapPixelFormat = BitmapPixelFormat.Bgra8;
        static readonly BitmapAlphaMode kBitmapAlphaMode = BitmapAlphaMode.Ignore;

        // 認識言語
        public Language OcrLanguage { get; set; } = new Language("en-US");

        // 結合モード種別
        public enum CombineModeType
        { 
            Auto,     // 単語と文を自動判定
            Text,     // 全てを一つのテキストとして結合
            Word,     // 全て単語として分割
        }
        public CombineModeType CombineMode { get; set; } = CombineModeType.Auto;

        // リサイズ設定
        // ・認識しやすいよう画像をリサイズ（主に拡大）する時の設定
        // ・補完モードが選べるのはBitmap使用時のみ
        public double ResizeScale { get; set; } = 1.0;
        public InterpolationMode ResizeMode { get; set; } = InterpolationMode.HighQualityBicubic;

        // 認識種別
        public enum RecognizeType
        { 
            Sentence,   // 文
            Word,       // 単語
        }

        // 認識結果
        public class RecognitionResult
        {
            public RecognizeType type;  // 認識種別
            public Rect bounds;         // バウンディングボックス
            public string text;         // 文字列
        }

        const float kWrapSpaceMax = 14.0f;              // 自動判定モード時に文の折り返しと判断するバウンディングボックスの距離の最大値（設定値にする）
        const string kWordModeReplaceRegex = "$[.,]";   // 単語モードの文字列置換に使う正規表現（設定値にする）

        // OCRエンジン
        OcrEngine _ocrEngine;
        OcrEngine ocrEngine
        {
            get {
                if (_ocrEngine == null) {
                    DebugAvailableRecognizerLanguages();
                   //_ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                   _ocrEngine = OcrEngine.TryCreateFromLanguage(OcrLanguage);
                }
                Debug.Assert(_ocrEngine != null);
                return _ocrEngine;
            } 
        }

        /// <summary>
        /// 使用可能な言語をデバッグ出力
        /// </summary>
        public static void DebugAvailableRecognizerLanguages()
        {
            DebugLog.WriteLine($"[WindowsOcr] Languages: {string.Join(",", OcrEngine.AvailableRecognizerLanguages.Select(x => x.LanguageTag))}");
        }

        /// <summary>
        /// Microsoft OCRで文字認識させる（Bitmap）
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public async Task<List<RecognitionResult>> RecognizeAsync(Bitmap bitmap, bool dispaseBitmap = false)
        {
            // #負荷メモ#
            // ・補完とSoftwareBitmap変換で300ms程度
            // ・RecognizeAsyncで400ms程度
            // ・デバッグ表示無しのToResultで40～60ms（デバッグ表示有りで250～300ms）
            bool resized = ResizeScale != 1.0f;
            if (resized)
            {
                bitmap = BitmapUtil.Resize(bitmap, ResizeScale, ResizeMode, dispaseBitmap);
            }
            using (var softwareBitmap = await BitmapUtil.ToSoftwareBitmapAsync(bitmap, resized || dispaseBitmap, kBitmapPixelFormat, kBitmapAlphaMode))
            {
                return await RecognizeAsync(softwareBitmap);
            }
        }

        /// <summary>
        /// Microsoft OCRで文字認識させる（BitmapSource）
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public async Task<List<RecognitionResult>> RecognizeAsync(BitmapSource bitmapSource)
        {
            if (ResizeScale != 1.0f)
            { // @note: 補完方法が選べない？
                bitmapSource = new TransformedBitmap(bitmapSource, new ScaleTransform(ResizeScale, ResizeScale));
            }
            using (var softwareBitmap = await BitmapUtil.ToSoftwareBitmapAsync(bitmapSource, kBitmapPixelFormat, kBitmapAlphaMode)) {
                return await RecognizeAsync(softwareBitmap);
            }
        }

        /// <summary>
        /// Microsoft OCRで文字認識させる（SoftwareBitmap）
        /// ・拡縮機能が備わってない（作るのが面倒）ので、privateメソッドにしている
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        async Task<List<RecognitionResult>> RecognizeAsync(SoftwareBitmap softwareBitmap)
        {
            var result = await ocrEngine.RecognizeAsync(softwareBitmap);
            return ToResult(result);
        }

        /// <summary>
        /// OcrResult => Result
        /// </summary>
        /// <param name="ocrResult"></param>
        /// <returns></returns>
        List<RecognitionResult> ToResult(OcrResult ocrResult)
        {
            DebugLog.WriteLine($"[WindowsOcr] OcrResult Line={ocrResult.Lines.Count}, Word={ocrResult.Lines.Select(x => x.Words.Count).Sum()}, Text={ocrResult.Text}");

            // 単語の結果リスト
            // ・nullにはならない
            // ・ピリオド、カンマも含まれて１単語とされる（取り除く）
            if (CombineMode == CombineModeType.Word) {
                return ocrResult.Lines
                            .SelectMany(x => x.Words)
                            .Select(x => CreateResult(x)).ToList();
            }
            // テキストの結果リスト（要素は一つのみ）
            else if (CombineMode == CombineModeType.Text) {
                if (ocrResult.Lines.Count == 0)
                    return new List<RecognitionResult>(); // 空リスト

                return new List<RecognitionResult>() { CreateResult(ocrResult) };
            }

            // 自動の結果リスト
            // ・Linesでは折り返しの文は複数の文と判断される。そのまま結合すると隙間が無くなるためおかしくなる。
            //   "-"も考慮が必要だろう。だが"-"は単語の途中と複合語の２通りの使い方があるので、自動的な判別は難しいかも。
            // ・Linesは思ったような並びにならない（左上→左下→右上→右下の順に並ぶ）。同じY位置でも間が空いていれば複数の文と捉えられる
            //   ただし縦方向に検索されるため、折り返し文の順番などは合っている。Textで取得した時に良い結果となる。
            else {
                if (ocrResult.Lines.Count == 0)
                    return new List<RecognitionResult>(); // 空リスト

                List<RecognitionResult> results = new();
                RecognitionResult before = null;
                foreach (var line in ocrResult.Lines) {
                    var current = CreateResult(line);

                    if (before != null) {
                        // 前行の結果と比較して、条件が合えばマージする
                        if (//current.type != RecognizeType.Word && before.type != RecognizeType.Word && // いずれも単語でない => 折り返して単語で終る場合がまずい
                            before.type != RecognizeType.Word && // 前行が単語でない
                            current.bounds.Left <= before.bounds.Right && current.bounds.Right >= before.bounds.Left && // 横方向が重なっている
                            (current.bounds.Top - before.bounds.Bottom < kWrapSpaceMax)) { // 縦方向の距離が折り返しと判断される距離より短い
                            // マージ
                            before = new RecognitionResult()
                            {
                                type = RecognizeType.Sentence,
                                bounds = Rect.Union(before.bounds, current.bounds),
                                text = before.text + " " + current.text
                            };
                        }
                        else { // マージしない
                            results.Add(before);
                            before = current;
                        }
                    }
                    else {
                        before = current;
                    }
                }
                if (before != null) {
                    results.Add(before);
                }

#if false
                foreach (var result in results) { 
                    DebugLog.WriteLine($"[WindowsOcr] Result type:{result.type}, bounds:({result.bounds}) {result.text}");
                }
#endif
                return results;
            }

            throw new InvalidEnumArgumentException();
        }

        /// <summary>
        /// 結果の生成
        /// ・引数の範囲を対象にして、結果を生成する
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        RecognitionResult CreateResult(OcrWord word)
        { 
            return CreateResultSub2(true, word.BoundingRect.ToRect(), word.Text);
        }

        RecognitionResult CreateResult(OcrLine line)
        { 
            var rects = line.Words.Select(x => x.BoundingRect.ToRect()).ToList();
            return CreateResultSub1(rects, line.Text);
        }

        RecognitionResult CreateResult(OcrResult ocrResult)
        { 
            var rects = ocrResult.Lines.SelectMany(x => x.Words).Select(x => x.BoundingRect.ToRect()).ToList();
            return CreateResultSub1(rects, ocrResult.Text);
        }

        RecognitionResult CreateResultSub1(List<Rect> rects, string text)
        { 
            Rect rect = rects[0];
            for (int i = 1; i < rects.Count; ++i) {
                rect.Union(rects[i]);
            }

            return CreateResultSub2(rects.Count <= 1, rect, text);
        }

        RecognitionResult CreateResultSub2(bool isWord, Rect rect, string text)
        {
            // 認識時に拡縮しているので値を戻す
            rect = new Rect(rect.X / ResizeScale, rect.Y / ResizeScale, rect.Width / ResizeScale, rect.Height / ResizeScale);
            return new RecognitionResult()
            {
                type = isWord ? RecognizeType.Word : RecognizeType.Sentence,
                bounds = rect,
                text = CombineMode == CombineModeType.Word ? Regex.Replace(text, kWordModeReplaceRegex, "") : text
            };        
        }
    }
}
