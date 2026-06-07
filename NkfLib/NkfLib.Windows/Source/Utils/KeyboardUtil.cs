using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace NkfLib.Utils
{
    public static partial class KeyboardUtil
    {
       /// <summary>
        /// タブレットのソフトウェアキーボード切り替え
        /// </summary>
        public static void ToggleKeyboard()
        {
            try {
                var uiHostNoLaunch = new UIHostNoLaunch();
                var tipInvocation = (ITipInvocation)uiHostNoLaunch;
                tipInvocation.Toggle(GetDesktopWindow());
                Marshal.ReleaseComObject(uiHostNoLaunch);
            }
            catch {
            }
        }

        [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
        class UIHostNoLaunch
        {
        }

        [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ITipInvocation
        {
            void Toggle(IntPtr hwnd);
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 入力キーの文字変換
        /// ・仮想キー（VirtualKey）とはSystem.Windows.Forms.Keysのこと
        /// </summary>
        /// <param name="uCode"></param>
        /// <param name="uMapType"></param>
        /// <returns></returns>
        static byte[] GetCharsFromKey_KeyboardState = new byte[256];
        static StringBuilder GetCharsFromKey_Buf = new StringBuilder(256);
        public static string GetCharsFromKey(Key key, bool shift)
        {
            var virtualKey = KeyInterop.VirtualKeyFromKey(key);

            GetCharsFromKey_KeyboardState[(int)System.Windows.Forms.Keys.ShiftKey] = (byte)(shift ? 0xff : 0x00);

            ToUnicode((uint)virtualKey, 0, GetCharsFromKey_KeyboardState, GetCharsFromKey_Buf, 256, 0);
            return GetCharsFromKey_Buf.ToString();
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

    }
}
