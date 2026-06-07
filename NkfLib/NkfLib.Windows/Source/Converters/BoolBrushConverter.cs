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
    /// bool? => Brush コンバータ
    /// ・パラメータは'|'の文字で区切った文字列で、初めの指定はFalseの場合のBrush、次の指定がTrueの場合のBrush、最後の指定がnullの場合の次の指定がTrueの場合のBrushを表す
    ///   デフォルトはBlack
    /// ・@note: 生成したブラシはキャッシュするようにした方が良いのかもしれない
    /// </summary>
    public class BoolBrushConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new BoolBrushConverter();

        SolidColorBrush ToBrush(string value)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(value);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush param1 = Brushes.Black;
            SolidColorBrush param2 = Brushes.Black;
            SolidColorBrush param3 = Brushes.Black;
            if (!string.IsNullOrEmpty((string)parameter)) {
                string[] parameters = ((string)parameter).Split('|');
                if (parameters.Count() >= 1) {
                    param1 = ToBrush(parameters[0]);
                }
                if (parameters.Count() >= 2) {
                    param2 = ToBrush(parameters[1]);
                }
                if (parameters.Count() >= 3) {
                    param3 = ToBrush(parameters[2]);
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
