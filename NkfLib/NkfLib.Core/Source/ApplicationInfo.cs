using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Loader;

namespace NkfLib
{
    /// <summary>
    /// アプリケーション情報
    /// ・主にAssemblyInfoから情報を取得
    /// </summary>
    public static partial class ApplicationInfo
    {
        /// <summary>
        /// アンロード時のイベントハンドラ追加
        /// </summary>
        public static void AddUnloadEventHandler(Action<AssemblyLoadContext> handler)
        {
            // アンロード時のイベント実行
            AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Unloading += handler;
        }

        /// <summary>
        /// アンロード時のイベントハンドラ削除
        /// </summary>
        public static void RemoveUnloadEventHandler(Action<AssemblyLoadContext> handler)
        {
            // アンロード時のイベント実行
            AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Unloading -= handler;
        }
    }
}
