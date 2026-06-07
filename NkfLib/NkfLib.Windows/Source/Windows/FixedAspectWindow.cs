using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// 固定アスペクト比ウィンドウ
    /// ・WindowのSizeChangedイベントでも同じような実装はできそう。やってみたが結局同じような問題が発生する。
    /// ・@note 現状サイズを大きくしすぎると効かなくなる。
    ///         WPFのウィンドウの最大サイズはディスプレイのサイズなどによって制限されているように思う。
    /// </summary>
    public class FixedAspectWindow : Window
    {
        /// <summary>
        /// アスペクト比
        /// </summary>
        public double Aspect { get; set; } = Setting.ScreenLayerWidth / Setting.ScreenLayerHeight;

        /// <summary>
        /// 固定アスペクト比有効
        /// </summary>
        public bool IsEnableFixedAspect { get; set; } = true;

        internal enum WM
        {
            WINDOWPOSCHANGING = 0x0046,
        }

        public static class SWP
        {
            public static readonly int
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        public FixedAspectWindow()
        {
            SourceInitialized += OnSourceInitialized;
        }

        void OnSourceInitialized(object sender, EventArgs ea)
        {
            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual((Window)sender);
            hwndSource.AddHook(DragHook);

            // @note 初期値を設定しないと起動時の動作が上手くいかない
            cx_ = (int)Width;
            cy_ = (int)Height;
        }

        int cx_; // 前回cx
        int cy_; // 前回cy
        int state_ = 0; // 0:リサイズ無し、1:高さ変更中、2:幅変更中
        IntPtr DragHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handeled)
        {
            if (!IsEnableFixedAspect) {
                state_ = 0;
                return IntPtr.Zero;
            }

            switch ((WM)msg) {
            case WM.WINDOWPOSCHANGING: {
                    WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    /*
                    // デバッグ表示
                    Debug.WriteLine(pos.flags);
                    if ((pos.flags & (int)SWP.NOMOVE) != 0) {
                        DebugLog.WriteLine("NOMOVE");
                    }
                    if ((pos.flags & (int)SWP.NOSIZE) != 0) {
                        DebugLog.WriteLine("NOSIZE");
                    }
                    if ((pos.flags & (int)SWP.NOZORDER) != 0) {
                        DebugLog.WriteLine("NOZORDER");
                    }
                    if ((pos.flags & (int)SWP.NOREDRAW) != 0) {
                        DebugLog.WriteLine("NOREDRAW");
                    }
                    if ((pos.flags & (int)SWP.NOACTIVATE) != 0) {
                        DebugLog.WriteLine("NOACTIVATE");
                    }
                    if ((pos.flags & (int)SWP.SHOWWINDOW) != 0) {
                        DebugLog.WriteLine("SHOWWINDOW");
                    }
                    if ((pos.flags & (int)SWP.HIDEWINDOW) != 0) {
                        DebugLog.WriteLine("HIDEWINDOW");
                    }
                    if ((pos.flags & (int)SWP.NOCOPYBITS) != 0) {
                        DebugLog.WriteLine("NOCOPYBITS");
                    }
                    if ((pos.flags & (int)SWP.NOOWNERZORDER) != 0) {
                        DebugLog.WriteLine("NOOWNERZORDER");
                    }
                    if ((pos.flags & (int)SWP.NOSENDCHANGING) != 0) {
                        DebugLog.WriteLine("NOSENDCHANGING");
                    }
                    if ((pos.flags & (int)SWP.DEFERERASE) != 0) {
                        DebugLog.WriteLine("DEFERERASE");
                    }
                    if ((pos.flags & (int)SWP.ASYNCWINDOWPOS) != 0) {
                        DebugLog.WriteLine("ASYNCWINDOWPOS");
                    }
                    */

                    if ((pos.flags & (int)SWP.NOMOVE) != 0) {
                        state_ = 0;
                        return IntPtr.Zero;
                    }

                    Window wnd = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                    if (wnd == null) return IntPtr.Zero;

                    // 幅か高さのどちらか一方を基準にしないと対応できなかった
                    // また、DPIの違いによりウィンドウのサイズと比較すると問題があったので修正
                    /*
                    if (state_ == 0) {
                        state_ = (MainWindow.Current.Height != pos.cy) ? 1
                               : (MainWindow.Current.Width  != pos.cx) ? 2
                                                                       : 0;
                    }
                    */
                    if (state_ == 0) {
                        if (pos.cy != cy_) { // 高さ変更中
                            state_ = 1;
                        }
                        else if (pos.cx != cx_) { // 幅変更中
                            state_ = 2;
                        }
                    }

                    if (state_ == 1) {
                        //pos.cx = (int)(pos.cy * (Common.ApplicationWidth / Common.ApplicationHeight));
                        pos.cx = (int)(pos.cy * Aspect);
                        //pos.cy = (int)(pos.cx * (Common.ApplicationHeight / Common.ApplicationWidth));
                    }
                    else if (state_ == 2) {
                        //pos.cy = (int)(pos.cx * (Common.ApplicationHeight / Common.ApplicationWidth));
                        pos.cy = (int)(pos.cx / Aspect);
                        //pos.cx = (int)(pos.cy * (Common.ApplicationWidth / Common.ApplicationHeight));
                    }
                    cx_ = pos.cx;
                    cy_ = pos.cy;

                    // ドラッグ終了
                    if ((pos.flags & (int)SWP.NOACTIVATE) == 0) {
                        state_ = 0;
                    }

                    Marshal.StructureToPtr(pos, lParam, true);
                    handeled = true;

                    //DebugTimer.StampPassedMs(string.Format("FixedAspectWindowDraged state={0} cx={1} cy={2} cx_={3} cy_={4}", state_, pos.cx, pos.cy, cx_, cy_));
                }
                break;
            }

            return IntPtr.Zero;
        }
    }
}
