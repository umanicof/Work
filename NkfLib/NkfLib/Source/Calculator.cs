using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace NkfLib
{
    /// <summary>
    /// 電卓クラス
    /// ・数式を入力として計算結果を返却
    /// ・入手したMathConverter（https://rachel53461.wordpress.com/2011/08/20/the-math-converter/）のソースから作成
    /// ・その他の計算方法
    /// 　https://dobon.net/vb/dotnet/programing/eval.html
    /// 　
    /// ※このクラスはマイナス符号の値が含まれていると上手く動かないし、おそらく四則演算の順番も守られていない。よって使用は推奨されない。
    /// 　マイナス符号の値は入力前にSignMinusの値に置換してやれば動く。
    /// </summary>
    public class Calculator
    {
        // @note: マイナス符号がOperatorとして判断されトークン分割できないので置換する必要がある
        public static readonly string SignMinus = "minus";
        // Does a math equation on the bound value.
        // Use @VALUE in your mathEquation as a substitute for bound value
        // Operator order is parenthesis first, then Left-To-Right (no operator precedence)
        static readonly char[] _allOperators = new[] { '+', '-', '*', '/', '%', '#', '(', ')' }; // @note: '#'は割って商を出す

        static readonly List<string> _grouping = new List<string> { "(", ")" };
        static readonly List<string> _operators = new List<string> { "+", "-", "*", "/", "%", "#" };

        /// <summary>
        /// 計算
        /// </summary>
        /// <param name="mathEquation">数式</param>
        /// <returns></returns>
        public static double Compute(string mathEquation)
        {
            // Parse value into equation and remove spaces
            mathEquation = mathEquation.Replace(" ", "");

            // Validate values and get list of numbers in equation
            var numbers = new List<double>();
            double tmp;

            foreach (string s in mathEquation.Split(_allOperators)) {
                if (s != string.Empty) {
                    if (double.TryParse(s.Replace(SignMinus, "-"), out tmp)) {
                        numbers.Add(tmp);
                    }
                    else {
                        // Handle Error - Some non-numeric, operator, or grouping character found in string
                        throw new InvalidCastException();
                    }
                }
            }

            // Begin parsing method
            EvaluateMathString(ref mathEquation, ref numbers, 0);

            // After parsing the numbers list should only have one value - the total
            return numbers[0];
        }

        // Evaluates a mathematical string and keeps track of the results in a List<double> of numbers
        static void EvaluateMathString(ref string mathEquation, ref List<double> numbers, int index)
        {
            // Loop through each mathemtaical token in the equation
            string token = GetNextToken(mathEquation);

            while (token != string.Empty) {
                // Remove token from mathEquation
                mathEquation = mathEquation.Remove(0, token.Length);

                // If token is a grouping character, it affects program flow
                if (_grouping.Contains(token)) {
                    switch (token) {
                    case "(":
                        EvaluateMathString(ref mathEquation, ref numbers, index);
                        break;

                    case ")":
                        return;
                    }
                }

                // If token is an operator, do requested operation
                if (_operators.Contains(token)) {
                    // If next token after operator is a parenthesis, call method recursively
                    string nextToken = GetNextToken(mathEquation);
                    if (nextToken == "(") {
                        EvaluateMathString(ref mathEquation, ref numbers, index + 1);
                    }

                    // Verify that enough numbers exist in the List<double> to complete the operation
                    // and that the next token is either the number expected, or it was a ( meaning
                    // that this was called recursively and that the number changed
                    if (numbers.Count > (index + 1) &&
                        (double.Parse(nextToken) == numbers[index + 1] || nextToken == "(")) {
                        switch (token) {
                        case "+":
                            numbers[index] = numbers[index] + numbers[index + 1];
                            break;
                        case "-":
                            numbers[index] = numbers[index] - numbers[index + 1];
                            break;
                        case "*":
                            numbers[index] = numbers[index] * numbers[index + 1];
                            break;
                        case "/":
                            numbers[index] = numbers[index] / numbers[index + 1];
                            break;
                        case "%":
                            numbers[index] = numbers[index] % numbers[index + 1];
                            break;
                        case "#":
                            numbers[index] = Math.Floor(numbers[index] / numbers[index + 1]);
                            break;
                        }
                        numbers.RemoveAt(index + 1);
                    }
                    else {
                        // Handle Error - Next token is not the expected number
                        throw new FormatException("Next token is not the expected number");
                    }
                }

                token = GetNextToken(mathEquation);
            }
        }

        // Gets the next mathematical token in the equation
        static string GetNextToken(string mathEquation)
        {
            // If we're at the end of the equation, return string.empty
            if (mathEquation == string.Empty) {
                return string.Empty;
            }

            // Get next operator or numeric value in equation and return it
            string tmp = "";
            foreach (char c in mathEquation) {
                if (_allOperators.Contains(c)) {
                    return (tmp == "" ? c.ToString() : tmp);
                }
                else {
                    tmp += c;
                }
            }

            return tmp;
        }
    }
}
