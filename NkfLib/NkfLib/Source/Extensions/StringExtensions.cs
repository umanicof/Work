using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Globalization;
using System.Security.Cryptography;
using System.Data;

namespace NkfLib
{
    // <summary>
    /// string型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 空チェック
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// 空チェック（空白含む）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// string => byte 変換
        /// </summary>
        public static byte ToByte(this string self)
        {
            return byte.Parse(self);
        }
        public static byte ToByteOrDefault(this string self, byte defaultValue = default)
        {
            if (byte.TryParse(self, out byte value))
                return value;

            return defaultValue;
        }

        /// <summary>
        /// string(16進表記) => byte 変換
        /// </summary>
        public static byte HexToByte(this string self)
        {
            return byte.Parse(self, NumberStyles.HexNumber);
        }
        public static int HexToByteOrDefault(this string self, int defaultValue = default)
        {
            if (byte.TryParse(self, NumberStyles.HexNumber, null, out byte value)) 
                return value;

            return defaultValue; 
        }
        
        /// <summary>
        /// string => int 変換
        /// </summary>
        public static int ToInt(this string self)
        {
            return int.Parse(self);
        }
        public static int ToIntOrDefault(this string self, int defaultValue = default)
        {
            if (int.TryParse(self, out int value)) 
                return value;

            return defaultValue; 
        }

        /// <summary>
        /// string(16進表記) => int 変換
        /// </summary>
        public static int HexToInt(this string self)
        {
            return int.Parse(self, NumberStyles.HexNumber);
        }
        public static int HexToIntOrDefault(this string self, int defaultValue = default)
        {
            if (int.TryParse(self, NumberStyles.HexNumber, null, out int value)) 
                return value;

            return defaultValue; 
        }

        /// <summary>
        /// string => float 変換
        /// </summary>
        public static float ToFloat(this string self)
        {
            return float.Parse(self);
        }
        public static float ToFloatOrDefault(this string self, float defaultValue = default)
        {
            if (float.TryParse(self, out float value)) 
                return value;

            return defaultValue; 
        }

        /// <summary>
        /// string => double 変換
        /// </summary>
        public static double ToDouble(this string self)
        {
            return double.Parse(self);
        }
        public static double ToDoubleOrDefault(this string self, double defaultValue = default)
        {
            if (double.TryParse(self, out double value)) 
                return value;

            return defaultValue; 
        }

        /// <summary>
        /// eval風 文字列式の計算
        /// ・出典    ：https://qiita.com/mizu-kazu/items/e75e5cd8c91dbf34d44c
        /// ・別の方法：https://dobon.net/vb/dotnet/programing/eval.html
        /// 
        /// ※やや重いので問題になるようなら別の方法を検討する
        /// ※小数以下の計算ができていない（整数までしか計算できていない）可能性あり
        /// </summary>
        public static double Compute(this string self)
        {
            using (DataTable dt = new DataTable())
            {
                return Convert.ToDouble(dt.Compute(self, "").ToString());
            }
        }

        /// <summary>
        /// Enum変換
        /// ・対象がEnumの名前であれば、Enumに変換する
        /// </summary>
        /// <typeparam name="T">Enumの型</typeparam>
        /// <param name="self"></param>
        /// <param name="failureIsDefault">変換失敗時にデフォルト値を返すかどうか</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string self, bool failureIsDefault = true)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            if (!self.IsNullOrEmpty()) {
                // Enum.TryParseは使い勝手が悪い模様
                if (Enum.IsDefined(typeof(T), self)) {
                    return (T)Enum.Parse(typeof(T), self);
                }
            }

            if (failureIsDefault) 
                return default(T);

            throw new ArgumentException(); // 例外発生
        }

        /// <summary>
        /// Description属性からのEnum変換
        /// ・ToEnumに混ぜてしまおうとも思ったが、とりあえず分けている
        /// </summary>
        /// <typeparam name="T">Enumの型</typeparam>
        /// <param name="self"></param>
        /// <param name="failureIsDefault">変換失敗時にデフォルト値を返すかどうか</param>
        /// <returns></returns>
        public static T ToEnumFromDescription<T>(this string self, bool failureIsDefault = true) where T : struct, IComparable, IConvertible, IFormattable
        {
            foreach (var e in typeof(T).GetFields()) {
                var attr = Attribute.GetCustomAttribute(e, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null && attr.Description == self) {
                    return (T)e.GetValue(null);
                }
            }

            if (failureIsDefault)
                return default(T);

            throw new ArgumentException(); // 例外発生
        }

        /* いまいちなので廃止
        /// <summary>
        /// フォーマット
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Formats(this string self, params object[] values)
        {
            return String.Format(self, values);
        }
        */

        /// <summary>
        /// 指定文字でのサプライ
        /// </summary>
        public static string Supply(this string self, char c, int length)
        {
            return (self + new string(c, length)).Substring(0, length);
        }

        /// <summary>
        /// ひらがな判定（全て）
        /// </summary>
        public static bool IsHiragana(this string self)
        {
            return (Regex.IsMatch(self, @"^\p{IsHiragana}*$"));
        }

        /// <summary>
        /// カタカナ判定（全て）
        /// </summary>
        public static bool IsKatakana(this string self)
        {
            return (Regex.IsMatch(self, @"^\p{IsKatakana}*$"));
        }

        /// <summary>
        /// 表示文字の先頭を大文字にする
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string self)
        {
            if (self.IsNullOrEmpty())
                return self;

            return self.Substring(0, 1).ToUpper() + self.Substring(1);
        }

        /// <summary>
        /// URLデコードする
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToUrlDecode(this string self)
        {
            if (self.IsNullOrEmpty())
                return self;

            return System.Web.HttpUtility.UrlDecode(self);
        }

        /// <summary>
        /// HTMLデコードする
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToHtmlDecode(this string self)
        {
            if (self.IsNullOrEmpty())
                return self;

            return System.Web.HttpUtility.HtmlDecode(self);
        }
        
        /// <summary>
        /// 日付に変換
        /// </summary>
        public static DateTime ToDateTime(this string self, string format = null, DateTime? defaultValue = null)
        {
            format = format ?? Setting.DefaultDateTimeFormat;
            DateTime ret;
            if (DateTime.TryParseExact(self, format, null, DateTimeStyles.None, out ret)) {
                return ret;
            }
            return defaultValue ?? DateTime.MinValue;
        }

        /// <summary>
        /// 日付（null許容型）に変換
        /// </summary>
        public static DateTime? ToDateTimeNullable(this string self, string format = null, DateTime? defaultValue = null)
        {
            format = format ?? Setting.DefaultDateTimeFormat;
            DateTime ret;
            if (DateTime.TryParseExact(self, format, null, DateTimeStyles.None, out ret)) {
                return ret;
            }
            return defaultValue;
        }

        /// <summary>
        /// バージョン比較
        /// ・バージョンを表す文字列を比較
        /// </summary>
        /// <param name="self"></param>
        /// <param name="version"></param>
        /// <returns>
        /// 自身が小さい: 0より小さい値
        /// 自身と等しい: 0
        /// 自身が大きい: 0より大きい値
        /// </returns>
        public static int CompareToVersion(this string self, string version)
        {
            return new Version(self??"0.0.0").CompareTo(new Version(version));
        }
        
        /// <summary>
        /// 文字列のハッシュ値（SHA256）を生成
        /// </summary>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public static string GenerateSha256(this string self)
        {
            // 文字列をUTF-8エンコードでバイト配列として取り出す
            byte[] byteValues = Encoding.UTF8.GetBytes(self);

            // SHA256のハッシュ値を計算する
            SHA256 crypto256 = SHA256.Create();
            byte[] hash256Value = crypto256.ComputeHash(byteValues);

            // SHA256の計算結果をUTF8で文字列として取り出す
            StringBuilder hashedText = new StringBuilder();
            for (int i = 0; i < hash256Value.Length; i++) {
                // 16進の数値を文字列として取り出す
                hashedText.AppendFormat("{0:x2}", hash256Value[i]);
            }
            return hashedText.ToString();
        }

        /// <summary>
        /// トークン分割
        /// ・Splitの戻り値に合わせて配列で返している
        /// </summary>
        public static string[] SplitToken(this string self, IEnumerable<string> keywords)
        {
            var tokens = new List<string>();
            var datas = self.Split(keywords.ToArray(), StringSplitOptions.RemoveEmptyEntries); // 指定のキーワードを使って分割して取得された文字列群

            string target = self;
            foreach (var data in datas) {
                var index = target.IndexOf(data);
                Debug.Assert(index >= 0);

                // キーの前の文字列の取得
                if (index > 0) {
                    tokens.Add(target.Substring(0, index));
                }

                // キーの文字列の取得
                tokens.Add(target.Substring(index, data.Length));

                // 取得済みの文字列を削除
                target = target.Substring(index + data.Length);
            }

            // 残った文字列を追加
            if (!target.IsNullOrEmpty()) {
                tokens.Add(target);
            }
            return tokens.ToArray();
        }
    }
}
