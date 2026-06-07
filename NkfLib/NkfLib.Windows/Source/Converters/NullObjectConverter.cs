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

namespace NkfLib
{
    /// <summary>
    /// null => object コンバータ
    /// ・対象がnullの場合は、parameterを返す。nullで無ければそのまま返す。
    /// 　parameterが"true", "false"の場合はbool、数値の場合はdouble、それ以外はstringで返す。
    /// </summary>
    public class NullObjectConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new NullObjectConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null) return value;
            var s = parameter.ToString();
            if (s.ToLower() == "true") return true;
            if (s.ToLower() == "false") return false;
            if (s.IsNumeric()) return Double.Parse(s);
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
