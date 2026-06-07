using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// 円弧
    /// ・@note 反時計回りは実装が不完全
    /// </summary>
    public class Arc : Shape
    {
        // 境界線の色は Stroke プロパティ
        // 同、幅は StrokeThickness プロパティで設定する

        /// <summary>
        /// 中心点
        /// </summary>
        public Point Center
        {
            get { return (Point)base.GetValue(CenterProperty); }
            set { base.SetValue(CenterProperty, value); }
        }
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(Arc),
                new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get { return (double)base.GetValue(RadiusProperty); }
            set { base.SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(
                nameof(Radius), // プロパティ名
                typeof(double), // プロパティの型
                typeof(Arc), // プロパティを所有する型
                new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 開始角度
        /// ・正の値に丸めている
        /// </summary>
        public double StartAngle
        {
            get { return Util.RoundAngle((double)base.GetValue(StartAngleProperty)); }
            set { base.SetValue(StartAngleProperty, value); }
        }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(
                nameof(StartAngle), // プロパティ名
                typeof(double), // プロパティの型
                typeof(Arc), // プロパティを所有する型
                new FrameworkPropertyMetadata(140.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 終了角度
        /// ・正の値に丸めている
        /// </summary>
        public double EndAngle
        {
            get { return Util.RoundAngle((double)base.GetValue(EndAngleProperty)); }
            set { base.SetValue(EndAngleProperty, value); }
        }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register(
                nameof(EndAngle), // プロパティ名
                typeof(double), // プロパティの型
                typeof(Arc), // プロパティを所有する型
                new FrameworkPropertyMetadata(40.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 表示割合
        /// </summary>
        public double Percent
        {
            get { return (double)base.GetValue(PercentProperty); }
            set { base.SetValue(PercentProperty, value); }
        }
        public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register(
                nameof(Percent), // プロパティ名
                typeof(double), // プロパティの型
                typeof(Arc), // プロパティを所有する型
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 表示割合の角度（読み取り専用）
        /// ・正の値に丸めている
        /// </summary>
        public double PercentAngle
        {
            get {
                double angle;

                if (EndAngle < StartAngle) {
                    angle = (EndAngle + 360 - StartAngle) * Percent / 100 + StartAngle;
                }
                else {
                    angle = (EndAngle - StartAngle) * Percent / 100 + StartAngle;
                }
                return Util.RoundAngle(angle);
            }
        }

        /// <summary>
        /// 始点（読み取り専用）
        /// </summary>
        public Point StartPoint
        {
            get { return ToPoint(StartAngle); }
        }

        /// <summary>
        /// 終点（読み取り専用）
        /// </summary>
        public Point EndPoint
        {
            get { return ToPoint(EndAngle); }
        }

        /// <summary>
        /// 表示割合の点（読み取り専用）
        /// </summary>
        public Point PercentPoint
        {
            get { return ToPoint(PercentAngle); }
        }

        /// <summary>
        /// 円弧が 180 度を超えるとき true（読み取り専用）
        /// </summary>
        public bool IsLargeArc
        {
            get {
                bool ret;
                if (PercentAngle < StartAngle) {
                    ret = (PercentAngle + 360 - StartAngle) > 180;
                }
                else {
                    ret = (PercentAngle - StartAngle) > 180;
                }
                return ret;
            }
        }

        /// <summary>
        /// 円弧を描画する方向
        /// </summary>
        public SweepDirection SweepDirection
        {
            get { return (SweepDirection)base.GetValue(SweepDirectionProperty); }
            set { base.SetValue(SweepDirectionProperty, value); }
        }
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register(
                nameof(SweepDirection),
                typeof(SweepDirection),
                typeof(Arc),
                new FrameworkPropertyMetadata(SweepDirection.Clockwise, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Arc()
        {
            StrokeEndLineCap   = PenLineCap.Round;
            StrokeStartLineCap = PenLineCap.Round;
        }

        /// <summary>
        /// 角度から位置を算出
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        Point ToPoint(double angle)
        {
            return new Point(
                Math.Cos(angle * (Math.PI / 180)) * Radius + Center.X, 
                Math.Sin(angle * (Math.PI / 180)) * Radius + Center.Y
            );
        }

        /// <summary>
        /// DefiningGeometryプロパティ
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (Math.Abs(StartAngle - PercentAngle) < Double.Epsilon) return new GeometryGroup(); // 角度が等しい

                // 円弧
                PathFigure figure = new PathFigure();
                figure.StartPoint = StartPoint;

                ArcSegment segment = new ArcSegment();
                segment.Point = PercentPoint;
                segment.Size = new Size(Radius, Radius);
                segment.RotationAngle = 0.0;
                segment.IsLargeArc = IsLargeArc;
                segment.SweepDirection = SweepDirection;
                segment.IsStroked = true;

                figure.Segments.Add(segment);

                PathGeometry arc = new PathGeometry();
                arc.Figures.Add(figure);

                GeometryGroup group = new GeometryGroup();
                group.Children.Add(arc);
                
                return group;
            }
        }

    }
}
