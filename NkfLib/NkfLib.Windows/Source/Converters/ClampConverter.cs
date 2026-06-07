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
    /// 丸め コンバータ
    /// ・パラメータは'|'の文字で区切った文字列で、初めの指定は最小値、次の指定が最大値
    ///   デフォルトは最小値は0、最大値は1.0
    /// </summary>
    public class ClampConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new ClampConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double param1 = 0;
            double param2 = 1.0;
            if (!string.IsNullOrEmpty((string)parameter)) {
                string[] parameters = ((string)parameter).Split('|');
                if (parameters.Count() >= 1) {
                    param1 = Double.Parse(parameters[0].Trim());
                }
                if (parameters.Count() >= 2) {
                    param2 = Double.Parse(parameters[1].Trim());
                }
            }

            double val = System.Convert.ToDouble(value);
            return Util.Clamp(val, param1, param2);
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
