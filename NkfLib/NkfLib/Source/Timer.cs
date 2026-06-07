using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace NkfLib
{
    /// <summary>
    /// 定期的に処理を実行するタイマークラス
    /// ・PCLにはdispatcherTimerが存在しないので、別途作成
    /// </summary>
    public class Timer : CancellationTokenSource, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="action">実行するアクション</param>
        /// <param name="state">継続アクションによって使用されるデータを表すオブジェクト</param>
        /// <param name="dueTime">最初の実行までに遅延する時間（ミリ秒）</param>
        /// <param name="period">アクションを実行する間隔（ミリ秒）</param>
        /// <param name="cts">CancellationTokenSourceオブジェクト</param>
        public Timer(Action<Object> action, object state, int dueTime, int period, CancellationTokenSource cts)
        {
            Task.Delay(dueTime, cts.Token).ContinueWith(
            async (t, s) =>
            {
                var tuple = (Tuple<Action<Object>, object>)s;

                while (!cts.IsCancellationRequested) {
                    await Task.Run(() => tuple.Item1(tuple.Item2), cts.Token);
                    await Task.Delay(period);
                }
            },
            Tuple.Create(action, state),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
        }

        public new void Dispose() 
        {
            base.Cancel(); 
        }
    }
}
