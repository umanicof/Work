using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DrawingPoint = System.Drawing.Point;

namespace NkfLib
{
    // <summary>
    /// Point 型の拡張メソッドを管理するクラス
    /// 
    /// @note: Rectangle(GDI+) <=> Rect(WPF) の変換では、本来はDPIを考慮した座標変換を行わなければならない場合が多々ある。
    ///        GDI+はデバイス座標を使い、WPFはDPIを考慮した座標（DIP or dp）を使用している。
    ///        Windowsにおいては、dp * (デバイスのDPI/96) = デバイスのpixel、となる
    /// </summary>
    public static partial class PointExtensions
    {

        /// <summary>
        /// Point(GDI+) => Point(WPF) キャスト
        /// </summary>
        public static Point ToPoint(this DrawingPoint self)
        {
            return new Point(self.X, self.Y);
        }

        /// <summary>
        /// Point(WPF) => Point(GDI+) キャスト
        /// </summary>
        public static DrawingPoint ToDrawingPoint(this Point self)
        {
            return new DrawingPoint((int)self.X, (int)self.Y);
        }

        /// <summary>
        /// Point(GDI+) => Point(WPF) 座標変換
        /// ・マルチディスプレイ環境でDPIが異なると上手く動かない可能性が高い
        ///   => 変換したい先のディスプレイに配置されているVisualをセットすれば行ける？
        /// </summary>
        public static Point ToPointWithTransform(this DrawingPoint self, Visual visual)
        {
            return visual.PointFromScreen(self.ToPoint());
        }

        /// <summary>
        /// Point(WPF) => Point(GDI+) 座標変換
        /// ・マルチディスプレイ環境でDPIが異なると上手く動かない可能性が高い
        ///   => 変換したい先のディスプレイに配置されているVisualをセットすれば行ける？
        /// </summary>
        public static DrawingPoint ToDrawingPointWithTransform(this Point self, Visual visual)
        {
            return visual.PointToScreen(self).ToDrawingPoint();
        }
    }
}
