using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;

using System.Windows.Threading;
using System.Windows;

namespace NkfLib
{
    // <summary>
    /// FrameworkElement 型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class FrameworkElementExtensions
    {
        /// <summary>
        /// 開始イベント登録
        /// ・Loadedイベントのタイミングで一回のみ実行される
        /// </summary>
        public static void AddStarted(this FrameworkElement self, Action handler)
        {
            if (handler == null) return;

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (sender, e) =>
            {
                self.Loaded -= loadedHandler;
                handler();
            };
            self.Loaded += loadedHandler;
        }
    }
}
