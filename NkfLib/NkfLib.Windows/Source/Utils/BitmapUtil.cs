using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MediaBitmapFrame = System.Windows.Media.Imaging.BitmapFrame;

namespace NkfLib
{
    /// <summary>
    /// ビットマップ関連ユーティリティ
    /// ・種類
    ///   Bitmap ... ネイティブリソース。Dispose要。
    ///   BitmapSource ... WPFのBitmap。Dispose不要。
    /// ・処理負荷的にはBitmapを使うのも、BitmapSourceを使うのも殆ど変わらない。
    /// 　ただBitmapSourceに変換が必要がないなどであれば、その分の負荷は減る。
    /// </summary>
    public static partial class BitmapUtil
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        
        /// <summary>
        /// ファイルからBitmapを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Bitmap LoadBitmap(string path)
        {
            return new Bitmap(path);
        }

        /// <summary>
        /// SourceBitmap => byte型の配列
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static byte[] ToArray(BitmapSource bitmapSource)
        {
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(MediaBitmapFrame.Create(bitmapSource));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Bitmap => BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(Bitmap bitmap, bool disposeBitmap = false)
        {
           var hBitmap = bitmap.GetHbitmap(); 

           var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
               hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(hBitmap); // 必要

            if (disposeBitmap) {
                bitmap.Dispose();
            }

            return bitmapSource;
        }

        /// <summary>
        /// ファイルからBitmapSouceを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BitmapSource LoadBitmapSource(string path)
        {
            //return CreateBitmapSource(new Uri(path));
            return new BitmapImage(new Uri(path));
        }

        /// <summary>
        /// BitmapImage生成（メモリリーク対策含む）
        /// ・冗長なのでは @todo:調査要
        /// </summary>
        public static BitmapImage CreateBitmapImage(object source)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad; // メモリリーク対策らしい ※これにより生成時ではなくロード時に負荷がかかるようになる？
            bi.CreateOptions = BitmapCreateOptions.None; // メモリリーク対策らしい
            if (source is MemoryStream) {
                bi.StreamSource = (MemoryStream)source;
            }
            else if (source is Uri) {
                bi.UriSource = (Uri)source;
            }
            else {
                throw new ArgumentException("CreateBitmapImage Unsupport Source.");
            }
            bi.EndInit();
            bi.Freeze(); // メモリリーク対策らしい

            return bi;
        }

        /// <summary>
        /// BitmapSource生成
        /// ・冗長なのでは @todo:調査要
        /// </summary>
        static BitmapSource CreateBitmapSource(object source)
        {
            var bi = CreateBitmapImage(source);

            // dpiを合わせる
            int width = bi.PixelWidth;
            int height = bi.PixelHeight;

            int stride = width * bi.Format.BitsPerPixel;
            byte[] pixelData = new byte[stride * height];
            bi.CopyPixels(pixelData, stride, 0);

            var bs = BitmapSource.Create(width, height, Setting.Dpi, Setting.Dpi, bi.Format, null, pixelData, stride);
            bs.Freeze(); // 負荷軽減を期待
            return bs;
        }

        /// <summary>
        /// Base64 => BitmapSource コンバート
        /// </summary>
        public static BitmapSource Base64ToBitmapSource(string base64String)
        {
            if (base64String.IsNullOrEmpty()) return null;
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
                return CreateBitmapSource(ms);
            }
        }

        /// <summary>
        /// Base64 => BitmapImage コンバート
        /// </summary>
        public static BitmapImage Base64ToBitmapImage(string base64String)
        {
            if (base64String.IsNullOrEmpty()) return null;
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
                return CreateBitmapImage(ms);
            }
        }
        
        /// <summary>
        /// BitmapSource => Base64 コンバート
        /// </summary>
        public static string BitmapSourceToBase64(BitmapSource bs, FormatType type = FormatType.Png, int qualityLevel = 30)
        {
            if (bs == null) return "";

            using (MemoryStream ms = new MemoryStream()) {
                switch (type) {
                case FormatType.Png:
                    PngBitmapEncoder enpng = new PngBitmapEncoder();
                    enpng.Frames.Add(MediaBitmapFrame.Create(bs));
                    enpng.Save(ms);
                    break;
                case FormatType.Jpeg:
                    // @note: 透過情報が黒で出力されてしまう問題がある
                    JpegBitmapEncoder enjpg = new JpegBitmapEncoder();
                    enjpg.QualityLevel = qualityLevel;
                    enjpg.Frames.Add(MediaBitmapFrame.Create(bs));
                    enjpg.Save(ms);
                    break;
                default:
                    return "";
                }
                byte[] bitmapdata = ms.ToArray();

                return Convert.ToBase64String(bitmapdata);
            }
        }


        /// <summary>
        /// Bitmapのリサイズ（補完有り）
        /// </summary>
        /// <returns></returns>
        public static Bitmap Resize(Bitmap bitmap, double scale, InterpolationMode mode, bool disposeBitmap = false)
        {
            Debug.Assert(bitmap != null && mode != InterpolationMode.Invalid && scale > 0);
            int w = (int)(bitmap.Width * scale);
            int h = (int)(bitmap.Height * scale);
            Bitmap dest = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(dest);
            g.InterpolationMode = mode;
            g.DrawImage(bitmap, 0, 0, w, h);

            if (disposeBitmap)
            {
                bitmap.Dispose();
            }
            return dest;
        }

        /// <summary>
        /// Bitmapの一部を切り抜いて上書き
        /// </summary>
        /// <param name="src">切り抜き元のビットマップ</param>
        /// <param name="dst">上書きするビットマップ</param>
        /// <param name="srcRect">切り抜く矩形</param>
        /// <param name="dstRect">上書きする矩形（スケーリング）</param>
        /// <param name="disposeSrcBitmap"></param>
        /// <returns>上書き後のdst</returns>
        public static Bitmap DrawImage(Bitmap src, Bitmap dst, Rectangle srcRect, Rectangle dstRect, bool disposeSrcBitmap = false)
        {
            using (var g = Graphics.FromImage(dst))
            {
                g.DrawImage(src, dstRect, srcRect, GraphicsUnit.Pixel);
            }

            if (disposeSrcBitmap)
            {
                src.Dispose();
            }

            return dst;
        }
    }
}
