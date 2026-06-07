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
    /// 文字列がparameter指定の文字列に含まれていればtrueを返す コンバータ
    /// </summary>
    public class StringContainsConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new StringContainsConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s1 = value as string;
            var s2 = parameter as string;
            if (s1.IsNullOrEmpty() || s2.IsNullOrEmpty()) return false;
            return s2.Contains(s1);
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
