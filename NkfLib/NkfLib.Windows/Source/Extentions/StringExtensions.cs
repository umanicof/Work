using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NkfLib
{
    // <summary>
    /// string型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 全角変換
        /// </summary>
        public static string ToWide(this string self)
        {
            return Microsoft.VisualBasic.Strings.StrConv(self, Microsoft.VisualBasic.VbStrConv.Wide, 0x411);
        }

        /// <summary>
        /// 半角に変換
        /// </summary>
        public static string ToNarrow(this string self)
        {
            return Microsoft.VisualBasic.Strings.StrConv(self, Microsoft.VisualBasic.VbStrConv.Narrow, 0x411);
        }

        /// <summary>
        /// カタカナに変換
        /// </summary>
        public static string ToKatakana(this string self)
        {
            return Microsoft.VisualBasic.Strings.StrConv(self, Microsoft.VisualBasic.VbStrConv.Katakana, 0x411);
        }

        /// <summary>
        /// ひらがなに変換
        /// </summary>
        public static string ToHiragana(this string self)
        {
            return Microsoft.VisualBasic.Strings.StrConv(self, Microsoft.VisualBasic.VbStrConv.Hiragana, 0x411);
        }
        
        /// <summary>
        /// FontStyle変換
        /// </summary>
        public static FontStyle ToFontStyle(this string self, bool failureIsDefault = true)
        {
            switch (self) {
            case "Italic":
                return FontStyles.Italic;
            case "Normal":
                return FontStyles.Normal;
            case "Oblique":
                return FontStyles.Oblique;
            default:
                if (failureIsDefault) return FontStyles.Normal;
                throw new ArgumentException(); // 例外発生
            }
        }

        /// <summary>
        /// FontWeight変換
        /// </summary>
        public static FontWeight ToFontWeight(this string self, bool failureIsDefault = true)
        {
            switch (self) {
            case "Black":
                return FontWeights.Black;
            case "Bold":
                return FontWeights.Bold;
            case "DemiBold":
                return FontWeights.DemiBold;
            case "ExtraBlack":
                return FontWeights.ExtraBlack;
            case "ExtraBold":
                return FontWeights.ExtraBold;
            case "ExtraLight":
                return FontWeights.ExtraLight;
            case "Heavy":
                return FontWeights.Heavy;
            case "Light":
                return FontWeights.Light;
            case "Medium":
                return FontWeights.Medium;
            case "Normal":
                return FontWeights.Normal;
            case "Regular":
                return FontWeights.Regular;
            case "SemiBold":
                return FontWeights.SemiBold;
            case "Thin":
                return FontWeights.Thin;
            case "UltraBlack":
                return FontWeights.UltraBlack;
            case "UltraBold":
                return FontWeights.UltraBold;
            case "UltraLight":
                return FontWeights.UltraLight;
            default:
                if (failureIsDefault) return FontWeights.Normal;
                throw new ArgumentException(); // 例外発生
            }
        }
    }
}
