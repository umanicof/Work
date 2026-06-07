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
    /// 数式比較 => Visibility コンバータ
    /// ・MathCompareConverter と BoolVisibilityConverter を連結
    /// </summary>
    public class MathVisibilityConverter : ComposingConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current 
            = new MathVisibilityConverter { MathCompareConverter.Current, BoolVisibilityConverter.Current };
    }
}
