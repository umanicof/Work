using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NkfLib
{
    /// <summary>
    /// セマフォ拡張クラス
    /// ・内部的には SemaphoreSlim を使用して、非同期な排他制御を実装しやすくしたクラス
    /// ・Lockメソッドを用意して、usingを抜けた時にセマフォが解放される仕組みを追加
    /// ・SemaphoreSlim の WaitAsync() は SemaphoreEx の AcquireAsync() に名称変更
    /// 
    /// ■使用方法
    ///    SemaphoreEx _semaphore = new(1);
    ///    
    ///    async void Foo()
    ///    {
    ///         // 使用可能なセマフォの発生待ち
    ///         await _semaphore.WaitRemainsAsync();
    ///
    ///         // セマフォのロック（using中のみ獲得）
    ///         // ※ロックできなければ例外発生
    ///         using (var locking = _semaphore.GetLock())
    ///         {
    ///             await Bar();
    ///         }
    ///         
    ///         // セマフォのロック（using中のみ獲得）
    ///         // ※ロックできるまで非同期待ち
    ///         using (var locking = await _semaphore.GetLockAsync())
    ///         {
    ///             await Bar();
    ///         }
    ///
    ///         // セマフォの獲得
    ///         await _semaphore.AcquireAsync();
    ///         
    ///         await Bar();
    ///         
    ///         // セマフォの返却
    ///         _semaphore.Release();
    ///    }
    /// </summary>
    public class SemaphoreEx
    {
        // セマフォの最大カウント
        public int MaxCount { get; private set; }

        // セマフォの残りカウント（獲得可能カウント）
        public int RemainCount => _semaphoreSlim.CurrentCount;

        SemaphoreSlim _semaphoreSlim;

        /// <summary>
        /// ロック中クラス
        /// </summary>
        public class SemaphoreLocking : DisposePattern
        {
            SemaphoreSlim _semaphoreSlim;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public SemaphoreLocking(SemaphoreSlim semaphoreSlim)
            {
                _semaphoreSlim = semaphoreSlim;

                // 破棄
                disposeCalled += () =>
                {
                    _semaphoreSlim.Release();
                };
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="maxCount"></param>
        public SemaphoreEx(int maxCount = 1)
        {
            MaxCount = maxCount;

            _semaphoreSlim = new SemaphoreSlim(maxCount, maxCount);
        }

        /// <summary>
        /// セマフォ獲得可能
        /// </summary>
        /// <returns></returns>
        public bool CanAcquire()
        {
            return RemainCount > 0;
        }

        /// <summary>
        /// セマフォ獲得可能待ち
        /// @note: SemaphoreSlimとは機能が異なるので注意
        /// </summary>
        /// <returns></returns>
        public async Task WaitCanAcquireAsync()
        {
            if (CanAcquire())
                return;

            await TaskEx.WaitUntil(() => CanAcquire());
        }

        /// <summary>
        /// セマフォ獲得
        /// @note: SemaphoreSlim における WaitAsync() と同義
        /// </summary>
        /// <returns></returns>
        public async Task AcquireAsync()
        {
            await _semaphoreSlim.WaitAsync();
        }

        /// <summary>
        /// セマフォ返却
        /// @note: SemaphoreSlim における WaitAsync() と同義
        /// </summary>
        /// <returns></returns>
        public void Release(int releaseCount = 1)
        {
            _semaphoreSlim.Release(releaseCount);
        }

        /// <summary>
        /// ロックオブジェクト取得
        /// </summary>
        /// <returns></returns>
        public async Task<SemaphoreLocking> GetLockAsync()
        {
            await _semaphoreSlim.WaitAsync();
            return new SemaphoreLocking(_semaphoreSlim);
        }
        public SemaphoreLocking GetLock()
        {
            if (!CanAcquire())
                throw new InvalidOperationException();

            _semaphoreSlim.Wait();
            return new SemaphoreLocking(_semaphoreSlim);
        }

    }
}
