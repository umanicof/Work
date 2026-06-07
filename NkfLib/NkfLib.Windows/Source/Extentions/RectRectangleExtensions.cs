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
using System.Windows.Media;
using Rectangle = System.Drawing.Rectangle;
using DrawingPoint = System.Drawing.Point;
using System.Diagnostics;

namespace NkfLib
{
    // <summary>
    /// Rect, Rectangle 型の拡張メソッドを管理するクラス
    ///
    /// @note: Rectangle(GDI+) <=> Rect(WPF) の変換では、本来はDPIを考慮した座標変換を行わなければならない場合が多々ある。
    ///        GDI+はデバイス座標を使い、WPFはDPIを考慮した座標（DIP or dp）を使用している。
    ///        Windowsにおいては、dp * (デバイスのDPI/96) = デバイスのpixel、となる
    /// </summary>
    public static partial class RectRectangleExtensions
    {
        /// <summary>
        /// 値が全て0（デフォルト値）
        /// ※IsEmptyはおそらく初期化しているかどうかも判定に含む
        /// </summary>
        public static bool IsZero(this Rect self)
        {
            return self.X == 0 && self.Y == 0 && self.Width == 0 && self.Height == 0;
        }

        /// <summary>
        /// 値が全て0（デフォルト値）
        /// ※IsEmptyはおそらく初期化しているかどうかも判定に含む
        /// </summary>
        public static bool IsZero(this Rectangle self)
        {
            return self.X == 0 && self.Y == 0 && self.Width == 0 && self.Height == 0;
        }

        /// <summary>
        /// Rectangle(GDI+) => Rect(WPF) キャスト
        /// </summary>
        public static Rect ToRect(this Rectangle self)
        {
            return new Rect(self.X, self.Y, self.Width, self.Height);
        }

        /// <summary>
        /// Rect(WPF) => Rectangle(GDI+) キャスト
        /// </summary>
        public static Rectangle ToRectangle(this Rect self)
        {
            return new Rectangle((int)self.X, (int)self.Y, (int)self.Width, (int)self.Height);
        }

        /// <summary>
        /// Rectangle(GDI+) => Rect(WPF) 座標変換
        /// ・マルチディスプレイ環境でDPIが異なると上手く動かない可能性が高い
        ///   => 変換したい先のディスプレイに配置されているVisualをセットすれば行ける？
        /// </summary>
        public static Rect ToRectWithTransform(this Rectangle self, Visual visual)
        {
            var lt = new DrawingPoint(self.Left, self.Top).ToPointWithTransform(visual);
            var rb = new DrawingPoint(self.Right, self.Bottom).ToPointWithTransform(visual);
            return new Rect(lt, rb);
        }

        /// <summary>
        /// Rect(WPF) => Rectangle(GDI+) 座標変換
        /// ・マルチディスプレイ環境でDPIが異なると上手く動かない可能性が高い
        ///   => 変換したい先のディスプレイに配置されているVisualをセットすれば行ける？
        /// </summary>
        public static Rectangle ToRectangleWithTransform(this Rect self, Visual visual)
        {
            var lt = new Point(self.Left, self.Top).ToDrawingPointWithTransform(visual);
            var rb = new Point(self.Right, self.Bottom).ToDrawingPointWithTransform(visual);
            return new Rectangle(lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);
        }

        /// <summary>
        /// 加算
        /// </summary>
        /// <returns></returns>
        public static Rect Add(this Rect self, Rect src)
        {
            return new Rect(self.X + src.X, self.Y + src.Y, self.Width + src.Width, self.Height + src.Height);
        }

        /// <summary>
        /// 加算
        /// </summary>
        /// <returns></returns>
        public static Rectangle Add(this Rectangle self, Rectangle src)
        {
            return new Rectangle(self.X + src.X, self.Y + src.Y, self.Width + src.Width, self.Height + src.Height);
        }

        /// <summary>
        /// 位置設定
        /// </summary>
        /// <returns></returns>
        public static Rect SetLocation(this Rect self, double x, double y)
        {
            return new Rect(x, y, self.Width, self.Height);
        }

        /// <summary>
        /// 位置設定
        /// </summary>
        /// <returns></returns>
        public static Rectangle SetLocation(this Rectangle self, int x, int y)
        {
            return new Rectangle(x, y, self.Width, self.Height);
        }

        /// <summary>
        /// オフセット移動
        /// ・Offsetメソッドとは違い、元の矩形は変更せずに新しい矩形を返す
        /// </summary>
        /// <returns></returns>
        public static Rect MoveOffset(this Rect self, double x, double y)
        {
            return new Rect(self.X + x, self.Y + y, self.Width, self.Height);
        }

        /// <summary>
        /// オフセット移動
        /// ・Offsetメソッドとは違い、元の矩形は変更せずに新しい矩形を返す
        /// </summary>
        /// <returns></returns>
        public static Rectangle MoveOffset(this Rectangle self, int x, int y)
        {
            return new Rectangle(self.X + x, self.Y + y, self.Width, self.Height);
        }

        /// <summary>
        /// 外側に指定pixel分広げる
        /// ・指定pixcelがマイナス値であれば内側に縮める
        /// </summary>
        /// <returns></returns>
        public static Rect Stretch(this Rect self, double px)
        {
            return self.Stretch(px, px, px, px);
        }
        public static Rect Stretch(this Rect self, double horizontal, double vertical)
        {
            return self.Stretch(horizontal, vertical, horizontal, vertical);
        }
        public static Rect Stretch(this Rect self, double left, double top, double right, double bottom)
        {
            double newWidth = self.Width + left + right;
            double newHeight = self.Height + top + bottom;
            Debug.Assert(newWidth >= 0 && newHeight >= 0);
            return new Rect(self.X - left, self.Y - top, newWidth, newHeight);
        }

        /// <summary>
        /// 外側に指定pixel分広げる
        /// ・指定pixcelがマイナス値であれば内側に縮める
        /// </summary>
        /// <returns></returns>
        public static Rectangle Stretch(this Rectangle self, int px)
        {
            return self.Stretch(px, px, px, px);
        }
        public static Rectangle Stretch(this Rectangle self, int marginH, int marginV)
        {
            return self.Stretch(marginH, marginV, marginH, marginV);
        }
        public static Rectangle Stretch(this Rectangle self, int left, int top, int right, int bottom)
        {
            int newWidth = self.Width + left + right;
            int newHeight = self.Height + top + bottom;
            Debug.Assert(newWidth >= 0 && newHeight >= 0);
            return new Rectangle(self.X - left, self.Y - top, newWidth, newHeight);
        }


        /// <summary>
        /// 全て結合
        /// </summary>
        /// <param name="self"></param>
        /// <returns>>結合後の矩形（無ければEmpty）</returns>
        public static Rect UnionAll(this IEnumerable<Rect> self)
        {
            Rect rect = Rect.Empty;
            foreach (var r in self)
            {
                rect = rect.IsEmpty ? r : Rect.Union(rect, r);
            }
            return rect;
        }

        /// <summary>
        /// 全て結合
        /// </summary>
        /// <param name="self"></param>
        /// <returns>結合後の矩形（無ければEmpty）</returns>
        public static Rectangle UnionAll(this IEnumerable<Rectangle> self)
        {
            Rectangle rect = Rectangle.Empty;
            foreach (var r in self)
            {
                rect = rect.IsEmpty ? r : Rectangle.Union(rect, r);
            }
            return rect;
        }
    }
}
