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

using System.Globalization;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// 先頭大文字変換
    /// ・マルチバインディング。一つ目の入力は変換するかどうか(bool)、２つ目の入力は変換する文字列(string)。
    /// ・入力文字の先頭を大文字にする
    /// </summary>
    public class ToSentenceCaseConverter : IMultiValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IMultiValueConverter Current = new ToSentenceCaseConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[1] == DependencyProperty.UnsetValue || values[1] == null) return ""; // ２番目の入力（Context.Data or Context.Ruby）が存在していないタイミングがある
            return ((bool)values[0]) ? ((string)values[1]).ToSentenceCase() : (string)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
