using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using DrawingColor = System.Drawing.Color;
using DrawingColorTranslator = System.Drawing.ColorTranslator;

namespace NkfLib
{
    // <summary>
    /// Color型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class ColorExtensions
    {
        /// <summary>
        /// Color(WPF) => Color(GDI+) 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DrawingColor ToColor(this Color self)
        { 
            return DrawingColor.FromArgb(self.A, self.R, self.G, self.B);
        }
        
        /// <summary>
        /// Color(GDI+) => Color(WPF) 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Color ToColor(this DrawingColor self)
        {
            return Color.FromArgb(self.A, self.R, self.G, self.B);
        }

        /// <summary>
        /// Color(GDI+) => #RRGGBB 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToRGB(this DrawingColor self)
        {
            return string.Format($"#{self.R:X2}{self.G:X2}{self.B:X2}");
        }

        /// <summary>
        /// Color(WPF) => #RRGGBB 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToRGB(this Color self)
        {
            return string.Format($"#{self.R:X2}{self.G:X2}{self.B:X2}");
        }

        /// <summary>
        /// Color(GDI+) => #AARRGGBB 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToARGB(this DrawingColor self)
        {
            return string.Format($"#{self.A:X2}{self.R:X2}{self.G:X2}{self.B:X2}");
        }

        /// <summary>
        /// Color(WPF) => #AARRGGBB 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToARGB(this Color self)
        {
            return string.Format($"#{self.A:X2}{self.R:X2}{self.G:X2}{self.B:X2}");
        }

        /// <summary>
        /// #RRGGBB, #AARRGGBB => Color(GDI+) 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DrawingColor ToDrawingColor(this string self)
        {
#if true
            return DrawingColorTranslator.FromHtml(self);
#else
            if (self.Length == 7) { 
                return DrawingColor.FromArgb(("ff" + self.Substring(1)).HexToInt());
            }
            else if (self.Length == 9) {
                return DrawingColor.FromArgb(self.Substring(1).HexToInt());
            }

            throw new FormatException();
#endif
        }

        /// <summary>
        /// #RRGGBB, #AARRGGBB => Color(WPF) 変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Color ToColor(this string self)
        {
#if true
            return (Color)ColorConverter.ConvertFromString(self);
#else
            if (self.Length == 7) {
                return Color.FromArgb(0xff, self[1..3].HexToByte(), self[3..5].HexToByte(), self[5..7].HexToByte());
            }
            else if (self.Length == 9) {
                return Color.FromArgb(self[1..3].HexToByte(), self[3..5].HexToByte(), self[5..7].HexToByte(), self[7..9].HexToByte());
            }

            throw new FormatException();
#endif
        }

        /// <summary>
        /// Color(WPF) => Blush 変換
        /// </summary>
        /// <returns></returns>
        public static SolidColorBrush ToBrush(this Color self)
        {
            return new SolidColorBrush(self);
        }
    }
}
