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
    /// bool? => Visibility コンバータ
    /// ・パラメータは'|'の文字で区切った文字列で、初めの指定はFalseの場合のVisibility、次の指定がTrueの場合のVisibility、最後の指定がnullの場合の次の指定がTrueの場合のVisibilityを表す
    ///   デフォルトはFalseは"Hidden"、Trueは"Visible"、nullは"Collapsed"
    /// </summary>
    public class BoolVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new BoolVisibilityConverter();

        Visibility ToVisibility(string value)
        {
            value = value.Trim();
            return value == "Collapsed" ? Visibility.Collapsed :
                   value == "Hidden"    ? Visibility.Hidden :
                                          Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility param1 = Visibility.Hidden;
            Visibility param2 = Visibility.Visible;
            Visibility param3 = Visibility.Collapsed;
            if (!string.IsNullOrEmpty((string)parameter)) {
                string[] parameters = ((string)parameter).Split('|');
                if (parameters.Count() >= 1) {
                    param1 = ToVisibility(parameters[0]);
                }
                if (parameters.Count() >= 2) {
                    param2 = ToVisibility(parameters[1]);
                }
                if (parameters.Count() >= 3) {
                    param3 = ToVisibility(parameters[2]);
                }
            }

            // 初めのパラメータの処理
            var val = value as bool?;
            if (val != null && !(bool)val) {
                return param1;
            }

            // 次のパラメータの処理
            if (val != null && (bool)val) {
                return param2;
            }

            // 最後のパラメータの処理
            return param3;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
