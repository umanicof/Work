using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// 暗号クラス
    /// ・32進数文字列を用いた独自の暗号化を行う
    /// </summary>
    public class Cipher
    {
        static string _convchar = "0123456789ACDEFGHJKLMNPQRTUVWXYZ"; // 32進数の並び。数字と間違えやすい'B','I','O','S'の文字を除いている

        /// <summary>
        /// 指定文字列から32進数の文字列を抽出
        /// ・'-'と小文字は取り除かれる
        /// </summary>
        public static string ExtractStr32(string s)
        {
            string s2 = "";
            for (int i = 0; i < s.Length; ++i) {
                if (_convchar.IndexOf(s[i]) < 0) continue;
                s2 += s[i];
            }
            return s2;
        }
           
        /// <summary>
        /// 32進数の文字列をバイト列に変換
        /// ・Base64のような本格的な仕組みであれば、bitを詰める必要がある
        /// </summary>
        public static byte[] Str32ToBytes(string s)
        {
            // 結果バッファを生成する。
            var buf = new byte[s.Length];

            for (int i = 0; i < s.Length; ++i) { 
                var value = _convchar.IndexOf(s[i]);
                if (value < 0) {
                    throw new System.ArgumentException("変換できない文字コードが含まれています"); // 変換不可
                }
                buf[i] = (byte)value;
            }

            //NkfDebug.WriteLine("Str32ToBytes:" + s + "  to " + BytesToStr32(buf));

            return buf;
        }

        /// <summary>
        /// バイト列を32進数の文字列に変換
        /// ・Base64のような本格的な仕組みであれば、bitを詰める必要がある
        /// </summary>
        public static string BytesToStr32(byte[] buf)
        {
            string s = "";
            for (int i = 0; i < buf.Length; ++i) {
                var index = buf[i];
                if (index > (_convchar.Length - 1)) {
                    throw new System.ArgumentException("変換できないバイナリが含まれています"); // 変換不可
                }
                s += _convchar[index];
            }

            return s;
        }

        /// <summary>
        /// 5bitのデータを右ローテート
        /// </summary>
        public static byte Rotate5bitR(byte data, int num = 1)
        {
            num %= 5;
            while (num-- > 0) {
                data = (byte)(((data >> 1) | (data << 4)) % 32);
            }
            return data;
        }

        /// <summary>
        /// 5bitのデータを左ローテート
        /// </summary>
        public static byte Rotate5bitL(byte data, int num = 1)
        {
            num %= 5;
            while (num-- > 0) {
                data = (byte)(((data << 1) | (data >> 4)) % 32);
            }
            return data;
        }

        /// <summary>
        /// 暗号化
        /// </summary>
        public static string Encrypt(string target, string password)
        {
            if (target.Length != password.Length) {
                throw new System.ArgumentException("平文とパスワードの長さが一致していません"); // 変換不可
            }

            var bytesTarget   = Str32ToBytes(target);
            var bytesPassword = Str32ToBytes(password);

            // パスワードをXOR
            for (int i = 0; i < bytesTarget.Length; ++i) {
                bytesTarget[i] ^= bytesPassword[i];
            }

            // 自身以外のバッファを順番に左ローテートしながらXOR
            for (int i = 0; i < bytesTarget.Length; ++i) {
                for (int j = 1; j < bytesTarget.Length; ++j) {
                    bytesTarget[i] ^= Rotate5bitL(bytesTarget[(i + j) % bytesTarget.Length], j);
                }
            }

            return BytesToStr32(bytesTarget);
        }

        /// <summary>
        /// 復号
        /// </summary>
        public static String Decrypt(string target, string password)
        {
            if (target.Length != password.Length) {
                throw new System.ArgumentException("暗号文とパスワードの長さが一致していません"); // 変換不可
            }

            var bytesTarget = Str32ToBytes(target);
            var bytesPassword = Str32ToBytes(password);

            // 自身以外のバッファを順番に左ローテートしながらXOR
            for (int i = bytesTarget.Length - 1; i >= 0; --i) {
                for (int j = 1; j < bytesTarget.Length; ++j) {
                    bytesTarget[i] ^= Rotate5bitL(bytesTarget[(i + j) % bytesTarget.Length], j);
                }
            }

            for (int i = bytesTarget.Length - 1; i >= 0; --i) {
                // パスワードをXOR
                bytesTarget[i] ^= bytesPassword[i];
            }

            return BytesToStr32(bytesTarget);
        }

#if DEBUG
        /// <summary>
        /// サンプルコード
        /// </summary>
        /// <returns></returns>
        public static bool Sample()
        {
            var password = "AAAAAAAAAAAAAAAAAAAAAAAA";
            var planetext = "171217121712171217128560";
            Debug.WriteLine("平文:" + planetext + " パスワード:" + password);
            var encrypttext = Encrypt(planetext, password);

            Debug.WriteLine("=> 暗号文:" + encrypttext);
            Debug.WriteLine("   認証ID:" + "0" + encrypttext.Substring(0,  4) + "-"
                                               + encrypttext.Substring(4,  5) + "-"
                                               + encrypttext.Substring(9,  5) + "-"
                                               + encrypttext.Substring(14, 5) + "-"
                                               + encrypttext.Substring(19, 5));
            Debug.WriteLine("暗号文:" + encrypttext + " パスワード:" + password);
            string decrypttext = Decrypt(encrypttext, password);
            Debug.WriteLine("=> 復号文:" + decrypttext);

            return false;
        }
#endif
    }
}
