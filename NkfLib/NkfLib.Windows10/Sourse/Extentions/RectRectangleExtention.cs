using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UwpRect = Windows.Foundation.Rect;

namespace NkfLib
{
    // <summary>
    /// Rect, Rectangle 型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class RectRectangleExtensions
    {
        /// <summary>
        /// Rectangle => Rect(UWP)
        /// </summary>
        public static UwpRect ToUwpRect(this Rectangle self)
        {
            return new UwpRect(self.X, self.Y, self.Width, self.Height);
        }

        /// <summary>
        /// Rect(WPF) => Rect(UWP)
        /// </summary>
        public static UwpRect ToUwpRect(this Rect self)
        {
            return new UwpRect(self.X, self.Y, self.Width, self.Height);
        }

        /// <summary>
        /// Rect(UWP) => Rectangle
        /// </summary>
        public static Rectangle ToRectangle (this UwpRect self)
        {
            return new Rectangle((int)self.X, (int)self.Y, (int)self.Width, (int)self.Height);
        }

        /// <summary>
        /// Rect(UWP) => Rect(WPF)
        /// </summary>
        public static Rect ToRect (this UwpRect self)
        {
            return new Rect(self.X, self.Y, self.Width, self.Height);
        }
    }
}
