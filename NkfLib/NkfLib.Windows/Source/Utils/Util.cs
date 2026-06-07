using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// ユーティリティ
    /// </summary>
    public static partial class Util
    {
        /// <summary>
        /// サウンド再生
        /// </summary>
        public static void PlaySound(Stream strm)
        {
            new SoundPlayer(strm).Play();
        }

        /// <summary>
        /// ウィンドウのアイコン非表示
        /// ・overrideしたOnSourceInitializedメソッドから呼び出す
        /// </summary>
        /// <param name="window"></param>
        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SendMessage(hwnd, WM_SETICON, new IntPtr(0), IntPtr.Zero);
            SendMessage(hwnd, WM_SETICON, new IntPtr(1), IntPtr.Zero);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |  SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;
    }
}
