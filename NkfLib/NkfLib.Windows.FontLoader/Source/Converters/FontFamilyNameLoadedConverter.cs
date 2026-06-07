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

using System.Drawing;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// フォントファミリ名 => ロード済みフォントファミリ コンバータ
    /// </summary>
    public class FontFamilyNameLoadedConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new FontFamilyNameLoadedConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !FontLoader.LoadedGlyphTypeInfo.ContainsKey((string)value)) {
                return System.Drawing.SystemFonts.DefaultFont; // @note エラーになるのでシステムフォントを返す
                //return FontLoader.LoadedFamilies.FirstOrDefault().Value; 
            }

            return FontLoader.LoadedGlyphTypeInfo[(string)value].FontFamily;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
