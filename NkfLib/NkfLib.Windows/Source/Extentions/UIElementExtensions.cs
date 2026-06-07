using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;

using System.Windows.Threading;
using System.Windows;

namespace NkfLib
{
    // <summary>
    /// UIElement 型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class UIElementExtensions
    {
        /// <summary>
        /// 表示領域の取得
        /// </summary>
        public static Rect ToScreenRect(this UIElement self)
        {
            Point targetLeftTop = self.PointToScreen(new Point(0.0, 0.0));
            return new Rect(targetLeftTop.X, targetLeftTop.Y, self.RenderSize.Width, self.RenderSize.Height);
        }
        public static System.Drawing.Rectangle ToScreenRectangle(this UIElement self)
        {
            Point targetLeftTop = self.PointToScreen(new Point(0.0, 0.0));
            return new System.Drawing.Rectangle((int)targetLeftTop.X, (int)targetLeftTop.Y, (int)self.RenderSize.Width, (int)self.RenderSize.Height);
        }

        // サイズは合っているが座標が0,0になっている
        //public static Rect ToRect(this UIElement self)
        //{  
        //    return self.RenderTransform.TransformBounds(new Rect(self.RenderSize));
        //}
    }
}
