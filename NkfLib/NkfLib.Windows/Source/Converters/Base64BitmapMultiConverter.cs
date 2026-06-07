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
    /// 種別, Base64 <=> BitmapSource コンバータ
    /// </summary>
    public class Base64BitmapMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IMultiValueConverter Current = new Base64BitmapMultiConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // 種別は使用していない
            return BitmapUtil.Base64ToBitmapSource((string)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            object[] values = new object[2];
            values[0] = FormatType.Png; // PNG固定
            values[1] = BitmapUtil.BitmapSourceToBase64((BitmapSource)value);
            return values;
        }
    }
}
