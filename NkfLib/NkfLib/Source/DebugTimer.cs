using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// 時間計測用デバッグタイマ
    /// ・id毎に管理している
    /// ■使用例１
    ///     using (var timer = new DebugTimer())
    ///     {
    ///         ...
    ///     }
    /// 
    /// ■使用例２
    ///     var timer = new DebugTimer();
    ///     ...
    ///     timer.StampPassedMs();
    /// 
    /// ■使用例３
    ///     DebugTimer.Start("Debug1");
    ///     ...
    ///     DebugTimer.StampPassedMs("Debug1");
    /// </summary>
    public class DebugTimer : IDisposable
    {
#if DEBUG
        #region static method ------------------------------------------------------------------------------------
        static Dictionary<string, DebugTimer> _timers = new Dictionary<string, DebugTimer>();

        /// <summary>
        /// FindTimer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static DebugTimer FindTimer(string id = null)
        { 
            return _timers.FirstOrDefault(x => x.Key == id).Value;
        }

        /// <summary>
        /// AddTimer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static DebugTimer AddTimer(string id = null)
        { 
            var t = new DebugTimer(id);
            _timers[id] = t;
            return t;
        }

        /// <summary>
        /// タイマスタート（リスタート）
        /// </summary>
        public static void Start(string id = null)
        {
            var t = FindTimer(id);
            if (t == null) {
                t = AddTimer(id);
            }
            t.Start();
        }

        /// <summary>
        /// 経過時間取得
        /// </summary>
        public static double GetPassedMs(string id = null, bool reset = true)
        {
            var t = FindTimer(id);
            if (t == null)
                return 0;

            return t.GetPassedMs(reset);
        }

        /// <summary>
        /// 経過時間出力
        /// </summary>
        /// <param name="id"></param>
        public static double StampPassedMs(string id = null, bool reset = true, string header = null, string footer = null)
        {
            var t = FindTimer(id);
            if (t == null)
                return 0;

            return t.StampPassedMs(reset, header, footer);
        }
        #endregion static method ------------------------------------------------------------------------------------

        #region instance method ------------------------------------------------------------------------------------
        public string Name { get; private set; }
        public Stopwatch stopwatch { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public DebugTimer(string name = null)
        {
            Name = name;
            stopwatch = new Stopwatch();
            stopwatch.Restart();
        }

        /// <summary>
        /// タイマスタート（リスタート）
        /// </summary>
        public void Start()
        {
            stopwatch.Restart();
        }

        /// <summary>
        /// 経過時間取得
        /// </summary>
        public int GetPassedMs(bool reset = true)
        {
            var ms =  stopwatch.ElapsedMilliseconds;
            if (reset)
            {
                Start();
            }
            return (int)ms;
        }

        /// <summary>
        /// 経過時間出力
        /// </summary>
        public int StampPassedMs(bool reset = true, string header = null, string footer = null)
        {
            var ms = GetPassedMs(reset);
            var title = Name.IsNullOrEmpty() ? (header.IsNullOrEmpty() ? "StampPassedMs:" : $"{header}:")
                                              : (header.IsNullOrEmpty() ? $"#{Name}#" : $"#{Name}# {header}:");
            Debug.WriteLine($"{title} {ms:d}ms {footer ?? string.Empty}");
            return ms;
        }
        
        /// <summary>
        /// 破棄
        /// ・リソースを破棄したいのではなく、usingブロックを抜ける際に経過時間出力したいだけ。
        /// </summary>
        public void Dispose()
        {
            this.StampPassedMs();
        }
        #endregion instance method ------------------------------------------------------------------------------------
#else
        public static void Start(string id = null) {}
        public static double GetPassedMs(string id = null, bool reset = true) { return 0; }
        public static double StampPassedMs(string id = null, bool reset = true,  string header = null, string footer = null) { return 0; }

        public DebugTimer(string name = null) {}
        public void Start() {}
        public int GetPassedMs(bool reset = true) { return 0;}
        public int StampPassedMs(bool reset = true, string header = null, string footer = null) { return 0; }
        public void Dispose() {}
#endif
    }
}
