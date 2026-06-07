using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Diagnostics;

namespace NkfLib
{
    // <summary>
    /// RepeatButton型の拡張メソッドを管理するクラス
    /// ※UIAutomationProvider.dllの参照が必要
    /// </summary>
    public static partial class RepeatButtonExtensions
    {
        /// <summary>
        /// クリックの呼び出し
        /// </summary>
        async public static void PerformClick(this RepeatButton button)
        {
            if (!button.IsEnabled) return;
            (new RepeatButtonAutomationPeer(button) as IInvokeProvider).Invoke();

            /* VisualStateを変えられるが、本来やりたいのはIsPressedの変化なので意味がなかった
            VisualStateManager.GoToState(button, "Pressed", true);
            await Task.Delay(100);
            VisualStateManager.GoToState(button, "Normal", true);
            */

            // 無理やりprivateなIsPressedプロパティを更新している
            typeof(RepeatButton).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button, new object[] { true });
            await Task.Delay(100);
            typeof(RepeatButton).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button, new object[] { false });
        }
    }
}
