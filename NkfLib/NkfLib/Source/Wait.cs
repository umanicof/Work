using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace NkfLib
{
    /// <summary>
    /// 待ちクラス
    /// ・Lockによって呼び出し元のスレッドは待ち状態に入り、Unlockによって解除される。
    /// ・複数スレッドからLockされた場合、Unlockで全て解除される。
    /// ・一旦解除されたLockを再ロックするか、Lockしないかは、引数で指定する。
    /// </summary>
    public class Wait
    {
        Task _task = new Task(() => { });

        /// <summary>
        /// ロック中かどうか
        /// </summary>
        public bool IsLocked { get; private set; }

        /// <summary>
        /// ロック（非同期待ち）
        /// </summary>
        /// <returns></returns>
        public async Task LockAsync(bool reset = false)
        {
            if (_task.IsCompleted) {
                if (!reset)
                    return; // リセットしないなら抜ける
                _task = new Task(() => { });　// 待ちタスクの再生成
            }
            IsLocked = true;
            await _task;
        }

        /// <summary>
        /// ロック解除
        /// </summary>
        /// <returns></returns>
        public void Unlock()
        {
            IsLocked = false;
            if (_task.IsCompleted)
                return; // 完了済みならスタートできないので
            _task.Start();
            while (!_task.IsCompleted) { } // 完了しないまま再入される場合があるので、ここで完了するまで待つ
        }
    }
}
