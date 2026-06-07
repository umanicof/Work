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
    /// 数式比較コンバータ
    /// ・MathConverterの仕組みを利用して数式同士の比較を行う。
    /// ・演算子は一つのみ対応。括弧も非対応。
    /// ・縦棒（'|'）区切りで複数引数入力。
    /// 　第一引数：入力文字列
    /// 　第二引数：結果がtrueの場合の戻り値
    /// 　第三引数：結果がfalseの場合の戻り値
    /// </summary>
    public class MathCompareConverter : IValueConverter
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static IValueConverter Current = new MathCompareConverter();

        //static readonly string[] compares_ = new[] { "==", "!=", "<=", ">=", "<", ">", "!"};
        static readonly string[] compares_ = new[] { "eq", "ne", "le", "ge", "lt", "gt", "not"}; // XAMLで使えない記号があるので文字表現
        //static readonly string[] logics_   = new[] { "&&", "||", "!" };
        static readonly string[] operators_ = compares_;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameters = ((string)parameter).Split('|').ToList();
            //var s = parameters[0].Replace("@VALUE", value.ToString());
            var s = parameters[0].Replace("@VALUE", value.ToString().Replace("-", Calculator.SignMinus)); // マイナス符号を置き換える

            // トークンを取得して数値と演算子に切り分け
            var numbers = new List<double>();
            var operaters = new List<string>();
            while (true) {
                var token = GetNextToken(s);
                if (token == string.Empty) break;
                s = s.Remove(0, token.Length);
                if (operators_.Contains(token)) {
                    operaters.Add(token);
                }
                else {
                    numbers.Add(token.Compute()); // 文字列の計算
                }
            }

            // 演算子が無ければデフォルト値を追加
            if (operaters.Count <= 0) {
                operaters.Add("");
            }

            // 演算子１つ限定で比較
            bool result;
            switch (operaters[0]) {
            case "eq":
                result = numbers[0] == numbers[1];
                break;
            case "ne":
                result = numbers[0] != numbers[1];
                break;
            case "le":
                result = numbers[0] <= numbers[1];
                break;
            case "ge":
                result = numbers[0] >= numbers[1];
                break;
            case "lt":
                result = numbers[0] < numbers[1];
                break;
            case "gt":
                result = numbers[0] > numbers[1];
                break;
            case "not":
                result = !(numbers[0] != 0);
                break;
            default:
                result = numbers[0] != 0;
                break;
            }

            if (parameters.Count <= 1) return result;
            if (result) return parameters[1];
            if (parameters.Count <= 2) return result;
            return parameters[2];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 次のトークンの取得
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static string GetNextToken(string s)
        {
            if (s == string.Empty) return string.Empty;
            var tokens = s.Split(operators_, StringSplitOptions.None);
            if (tokens[0] != string.Empty) return tokens[0];

            string token = "";
            foreach (char c in s) {
                token += c;
                if (operators_.Contains(token)) return token;
            }

            return string.Empty;
        }
    }
}
