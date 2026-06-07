using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Rect = System.Windows.Rect;

namespace NkfLib
{
    /// <summary>
    /// スクリーンキャプチャ
    /// ・出典
    ///   Standard: https://www.fenet.jp/dotnet/column/language/4633/
    ///   Windows API: https://dobon.net/vb/dotnet/graphics/screencapture.html
    ///   WPF: https://sourcechord.hatenablog.com/entry/20131013/1381691785
    /// ・プライマリスクリーンのキャプチャしかできないかも？
    /// ・Bitmapを使いまわすことも考えておくべきかも。
    ///   => 使いまわしても特に処理負荷など変わらない
    /// </summary>
    public class ScreenCapture
    {
        // 生成するBitmapのフォーマット
        // ・Undefined, DontCare, Format16bppArgb1555, Format16bppGrayScale は Graphics.FromImage()時に例外発生
        // ・WindowsOcrの形式に合うようなフォーマットにしている
        //static readonly PixelFormat kPixelFormat = PixelFormat.Format32bppArgb;
        public static readonly PixelFormat kPixelFormat = PixelFormat.Format24bppRgb;

        public static int ScreenCount => Screen.AllScreens.Length;

        /// <summary>
        /// 指定矩形のキャプチャ
        /// ・画面全体で30ms～60ms
        /// </summary>
        /// <returns></returns>
        public static Bitmap Capture(Rectangle rect)
        {            
            Bitmap bitmap = new Bitmap(rect.Width, rect.Height, kPixelFormat);
            using (Graphics graphics = Graphics.FromImage(bitmap)) {
                graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, bitmap.Size);
                return bitmap;
            }
        }
        public static Bitmap Capture(Rect rect)
        {
            return Capture(rect.ToRectangle());
        }
        
        /// <summary>
        /// スクリーン全体のキャプチャ
        /// </summary>
        /// <returns></returns>
        public static Bitmap CaptureScreen(int no = 0)
        {
            Debug.Assert(no < ScreenCount);
            // Screen の代わりに SystemParameters.PrimaryScreenWidth も使える
            Rectangle rect = Screen.AllScreens[no].Bounds;
            return Capture(rect);
        }

        /// <summary>
        /// アクティブウィンドウのキャプチャ
        /// </summary>
        /// <returns></returns>
        public static Bitmap CaptureActiveWindow()
        {
            // アクティブウィンドウを取得
            IntPtr activeWindow = GetForegroundWindow();
            GetWindowRect(activeWindow, out RECT r);
            Rectangle rect = new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
            return Capture(rect);
         }

        #region Windows API --------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
 
        [DllImport("user32.Dll")]
        static extern int GetWindowRect(IntPtr hWnd, out RECT rect);
  
        [DllImport("user32.dll")]
        extern static IntPtr GetForegroundWindow();
        #endregion Windows API --------------------------------------------------------

    }
}
