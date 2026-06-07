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
    /// Thickness 計算コンバータ
    /// ・parameterに"@VALUE+3"などと指定して計算する
    /// </summary>
    public class ThicknessComputeConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new ThicknessComputeConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formula = parameter as string;
            if (formula.IsNullOrEmpty())
                return value;
            if (!(value is Thickness from))
                throw new ArgumentException();

            return new Thickness(formula.Replace("@VALUE", from.Left.ToString()).Compute(),
                                 formula.Replace("@VALUE", from.Top.ToString()).Compute(),
                                 formula.Replace("@VALUE", from.Right.ToString()).Compute(),
                                 formula.Replace("@VALUE", from.Bottom.ToString()).Compute());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
