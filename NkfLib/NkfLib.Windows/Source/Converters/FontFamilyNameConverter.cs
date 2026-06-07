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
    /// フォントファミリ => フォントファミリ名 コンバータ
    /// ・先頭のフォントファミリ名を返す
    /// ・@note: 結構な回数が呼ばれ、処理負荷を増大させる場合がある。
    ///          キャッシュしてみたが余り効果はない。
    /// </summary>
    public class FontFamilyNameConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new FontFamilyNameConverter();

        //static Dictionary<FontFamily, string> cache_ = new Dictionary<FontFamily, string>();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s = ((FontFamily)value).FamilyNames.FirstOrDefault().Value;
            //DebugLog.WriteLine("FontFamilyNameConverter:" + s);
            return s;
            /*
            FontFamily fontFamily = ((FontFamily)value);
            if (!cache_.ContainsKey(fontFamily)) {
                cache_[fontFamily] = fontFamily.FamilyNames.FirstOrDefault().Value;
            }
            return cache_[fontFamily];
            */
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
