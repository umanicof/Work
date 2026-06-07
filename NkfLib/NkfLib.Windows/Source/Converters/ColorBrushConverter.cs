using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// Color（#RRGGBB, #AARRGGBB表記）<=> Brush コンバータ
    /// ・実際にはコンバーターを通す必要なし？
    /// </summary>
    public class ColorBrushConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new ColorBrushConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s = value as string;
            if (s.IsNullOrEmpty())
                throw new ArgumentException();

            return new BrushConverter().ConvertFromString(s); // "Red"などの指定もブラシにできる
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            if (brush == null)
                throw new ArgumentException();

            return brush.Color.ToARGB();
        }
    }
}
