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
    /// 文字列 => Visibility コンバータ
    /// ・文字列の有無に応じたVisibilityを返す
    /// ・パラメータは'|'の文字で区切った文字列で、初めの指定は文字列が無い場合のVisibility、次の指定が文字列が有る場合のVisibilityを表す
    ///   デフォルトは文字列が無ければ"Collapsed"、有れば"Visible"
    /// </summary>
    public class StringVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new StringVisibilityConverter();

        Visibility ToVisibility(string value)
        {
            value = value.Trim();
            return value == "Collapsed" ? Visibility.Collapsed :
                   value == "Hidden"    ? Visibility.Hidden    :
                                          Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility param1 = Visibility.Collapsed;
            Visibility param2 = Visibility.Visible;
            if (!string.IsNullOrEmpty((string)parameter)) {
                string[] parameters = ((string)parameter).Split('|');
                if (parameters.Count() >= 1) {
                    param1 = ToVisibility(parameters[0]);
                }
                if (parameters.Count() >= 2) {
                    param2 = ToVisibility(parameters[1]);
                }
            }

            // 初めのパラメータの処理
            if (string.IsNullOrEmpty((string)value)) {
                return param1;
            }

            // 次のパラメータの処理
            return param2;

            /*
            Visibility visibility = string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
            //DebugLog.WriteLine("StringVisibilityConverter " + visibility);
            return visibility;
            */
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
