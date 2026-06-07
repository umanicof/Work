using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// デバッグログクラス
    /// ・出力有無を制御したいログ出力に使用
    /// </summary>
    public class DebugLog
    {
        #region writeline
        [Conditional("ENABLED_DEBUGLOG")]
        public static void WriteLine(object value)
        {
            Debug.WriteLine(value);
        }

        [Conditional("ENABLED_DEBUGLOG")]
        public static void WriteLine(object value, string category)
        {
            Debug.WriteLine(value, category);
        }

        [Conditional("ENABLED_DEBUGLOG")]
        public static void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        [Conditional("ENABLED_DEBUGLOG")]
        public static void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        [Conditional("ENABLED_DEBUGLOG")]
        public static void WriteLine(string message, string category)
        {
            Debug.WriteLine(message, category);
        }
        #endregion
    }
}
