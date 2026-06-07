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
    /// パラメータ比較 コンバータ
    /// ・valueとparameterを文字列で比較して、同じであればtrueを返す
    /// </summary>
    public class IsEqualParameterConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new IsEqualParameterConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //DebugLog.WriteLine("IsEqualConverter value:" + value + " parameter:" + parameter + " result:" + (value.ToString() == parameter.ToString()));
            return (value.ToString() == parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? parameter : null;
            //throw new NotImplementedException();
        }
    }
}
