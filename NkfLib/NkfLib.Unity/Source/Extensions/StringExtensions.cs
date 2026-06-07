using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;

namespace NkfLib.Unity
{
    // <summary>
    /// string型の拡張メソッドを管理するクラス
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 空チェック
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string self)
        {
            return String.IsNullOrEmpty(self);
        }
    }
}
