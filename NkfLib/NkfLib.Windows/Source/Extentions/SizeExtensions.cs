using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace NkfLib
{
    // <summary>
    /// Size型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class SizeExtensions
    {
        /// <summary>
        /// アスペクト比（幅/高さ）に変換
        /// </summary>
        public static double ToAspect(this Size self)
        {
            return self.Width / self.Height;
        }
    }
}
