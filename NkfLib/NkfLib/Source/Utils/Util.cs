using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Diagnostics;
using System.ComponentModel;

namespace NkfLib
{
    /// <summary>
    /// ユーティリティ
    /// </summary>
    public static partial class Util
    {
        /// <summary>
        /// デザインモードかどうかの判定
        /// </summary>
        /// <returns></returns>
        public static bool InDesignMode()
        {
            return  LicenseManager.UsageMode != LicenseUsageMode.Runtime;
        }

        /// <summary>
        /// デバッグログの外部ファイルへの出力開始
        /// 出典：https://dobon.net/vb/dotnet/programing/tracelisteners.html
        /// </summary>
        static bool _StartedOutputDebugLog;
        public static void StartOutputDebugLog()
        {
            if (_StartedOutputDebugLog)
                return;
            _StartedOutputDebugLog = true;
#if false
            var dtl = (DefaultTraceListener)Trace.Listeners["Default"];
            dtl.LogFileName = Setting.DebugLogName;
#else
            var tl = Trace.Listeners["LogFile"];
            //出力ファイルを指定して、StreamWriterオブジェクトを作成
            StreamWriter sw = new StreamWriter(Setting.DebugLogPath);
            //自動的にフラッシュされるようにする
            sw.AutoFlush = true;
            //スレッドセーフラッパを作成
            TextWriter tw = TextWriter.Synchronized(sw);
            //名前を LogFile としてTextWriterTraceListenerオブジェクトを作成
            TextWriterTraceListener twtl = new TextWriterTraceListener(tw, "LogFile");
            //リスナコレクションに追加する
            Trace.Listeners.Add(twtl);
#endif
        }
        
        /// <summary>
        /// 入力値を最小値、最大値で丸める
        /// </summary>
        public static T Clamp<T>(this T val, T min, T max)
            where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) 
                return min;
            else if (val.CompareTo(max) > 0) 
                return max;
            else
                return val;
        }

        /// <summary>
        /// 入力値を0～maxの間でループするように丸める（maxは含まない）
        /// </summary>
        public static double Loop(double x, double max)
        {
            while (x < max) {
                x += max;
            }
            return x % max;
        }
        public static int Loop(int x, int max)
        {
            while (x < max) {
                x += max;
            }
            return x % max;
        }

        /// <summary>
        /// 角度を正の値に丸める
        /// </summary>
        public static double RoundAngle(double angle)
        {
            return Loop(angle, 360);
        }

        /// <summary>
        /// スワップ
        /// </summary>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// スワップ（コンテナ用）
        /// ・今のところコンテナごとに定義するしかないか
        /// </summary>
        public static void SwapIndexer<T>(T[] container, int lhs, int rhs)
        {
            var temp = container[lhs];
            container[lhs] = container[rhs];
            container[rhs] = temp;
        }
        public static void SwapIndexer<T>(List<T> container, int lhs, int rhs)
        {
            var temp = container[lhs];
            container[lhs] = container[rhs];
            container[rhs] = temp;
        }
        public static void SwapIndexer<T>(ObservableCollection<T> container, int lhs, int rhs)
        {
            var temp = container[lhs];
            container[lhs] = container[rhs];
            container[rhs] = temp;
        }

        /// <summary>
        /// GUIDの生成
        /// </summary>
        /// <returns></returns>
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
        
        /// <summary>
        /// ランダムな英数字の文字列を生成
        /// </summary>
        const string randomChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string GenerateRandomAlphanumeric(int length)
        {
            // GeneratePasswordは記号が入ってしまう
            //return System.Web.Security.Membership.GeneratePassword(length, 0);

            StringBuilder sb = new StringBuilder(length);
            Random r = new Random();

            for (int i = 0; i < length; i++) {
                int pos = r.Next(randomChars.Length);
                sb.Append(randomChars[pos]);
            }
            var ret = sb.ToString();
            //DebugLog.WriteLine("GenerateRandomAlphanumeric:" + ret);
            return ret;
        }

        /// <summary>
        /// XMLの整形
        /// ・内部的にパースすることで整形している
        /// </summary>
        public static string FormatXml(string xml)
        {
            try {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception) {
                return xml;
            }
        }
    }
}
