using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Globalization;
using System.Security.Cryptography;
using System.Data;

namespace NkfLib
{
    // <summary>
    /// string型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 数値判定（全て）
        /// </summary>
        public static bool IsNumeric(this string self)
        {
            return Microsoft.VisualBasic.Information.IsNumeric(self);
        }
    }
}
