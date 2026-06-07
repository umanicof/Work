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
    /// 連結 コンバータ
    /// ・複数のコンバータを連結する
    /// </summary>
    public class ComposingConverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            for (int i = 0; i < Count; i++) {
                value = this[i].Convert(value, targetType, (i == 0) ? parameter : null, culture);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            for (int i = this.Count - 1; i >= 0; i--) {
                value = this[i].ConvertBack(value, targetType, (i == this.Count - 1) ? parameter : null, culture);
            }

            return value;
        }
    }
}
