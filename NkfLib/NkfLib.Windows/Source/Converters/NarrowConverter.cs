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

using Microsoft.VisualBasic;

namespace NkfLib
{
    /// <summary>
    /// 半角 コンバータ
    /// ・半角を全角にはしない
    /// </summary>
    public class NarrowConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new NarrowConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Strings.StrConv((string)value, VbStrConv.Narrow, 0x411);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Strings.StrConv((string)value, VbStrConv.Narrow, 0x411);
        }
    }
}
