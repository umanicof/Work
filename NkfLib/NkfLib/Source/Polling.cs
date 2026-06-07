using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    /// <summary>
    /// ポーリング待ちクラス
    /// ・Semaphore や mutex を使うまでもない非同期なポーリング待ちを実現するためのクラス
    /// ・状態はONとOFFの２つのみ
    /// 
    /// ■初期化時に行いたい非同期処理がある場合の対応方法について
    /// 　C#はコンストラクタで非同期待ちをすることができず、その他の例えば初期化、ロードイベントなどで非同期待ちを行ったとしても、
    ///   呼び出し元では待ちを行っていないので期待通りに動かすのは難しい。
    ///   結局、初回のUI操作などで初期化を行うか、このクラスのような仕組みを使って初期化完了のフラグを待つかの２通りとなりそう。  
    ///   良さそうな実装は
    ///   　１．コンストラクタ（もしくは初期化・ロードイベント）実行時に非同期の初期化処理を走らせ、終わったら初期化フラグを立てるようにする
    ///   　２．１のクラスを使用するクラスでも初期化フラグを持ち、１のクラスの初期化フラグを合成する。
    ///   　    ※可能なら初期化フラグを持つクラスでは、自身の初期化フラグがONの時はAPIコール時にAssertを入れた方が良いが、無くても何とかなる
    ///   　３．最終的には画面のクラスやスケジューラに行きつくはずである。それらのクラスでも合成した初期化フラグを持つようにし、
    ///   　    初期化フラグがONの場合は画面の操作やスケジュール処理を無効にする。
    ///   　=> 結局このクラスは必要なさそう
    ///   　=> シングルトンクラスやインスタンスID（or GUID）を使って初期化状態を管理できないかと思ったが、クラスがいつ生成されるか分からない
    ///   　　 のでいつ完全に初期化が終わったのか分からない。
    /// 
    /// ■使用方法
    ///    Polling _polling = new();
    ///    
    ///    async void Foo()
    ///    {
    ///         // ON状態
    ///         _polling.IsOn = true;
    /// 
    ///         // OFF状態
    ///         _polling.IsOff = false;
    ///
    ///         // ON/OFF状態待ち
    ///         await _polling.WaitAsync();
    ///         
    ///         // ロック
    ///         if (!_polling.Lock())
    ///             return;
    ///         
    ///         // ロック（OFF待ち含む）
    ///         await _polling.LockAsync();
    /// 
    ///         // アンロック
    ///         _polling.Unlock();
    ///         
    ///         // ロックオブジェクト取得（using中のみON）
    ///         using (var locking = await _polling.GetLockAsync())
    ///         {
    ///             await Bar();
    ///         }
    ///    }
    /// </summary>
    public class Polling
    {
        /// <summary>
        /// ON状態
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>
        /// OFF状態
        /// </summary>
        public bool IsOff
        {
            get { return !IsOn; }
            set { IsOn = !value;  } 
        }

        /// <summary>
        /// ロック中クラス
        /// </summary>
        public class PollingLocking : DisposePattern
        {
            Polling _polling;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public PollingLocking(Polling busyCondition)
                : base(false) // デバッグログ無し
            {
                _polling = busyCondition;
                _polling.IsOn = true;

                // 破棄
                disposeCalled += () =>
                {
                    _polling.IsOn = false;
                };
            }
        }

        /// <summary>
        /// ON/OFF待ち
        /// </summary>
        /// <returns></returns>
        public async Task WaitAsync(bool waitOn = false)
        {
            if (waitOn ? IsOn : IsOff)
                return;

            await TaskEx.WaitUntil(() => waitOn ? IsOn : IsOff);
        }

        /// <summary>
        /// ロック（OFF待ち含む）
        /// </summary>
        /// <returns></returns>
        public async Task LockAsync()
        {
            await TaskEx.WaitUntil(() => IsOff);
            IsOn = true;
            return;
        }

        /// <summary>
        /// ロック
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public bool Lock()
        {
            if (IsOn)
                return false;

            IsOn = true;
            return true;
        }

        /// <summary>
        /// アンロック
        /// </summary>
        /// <returns></returns>
        public void Unlock()
        {
            IsOn = false;
            return;
        }

        /// <summary>
        /// ロックオブジェクト取得
        /// </summary>
        /// <returns></returns>
        public async Task<PollingLocking> GetLockAsync()
        {
            await WaitAsync();
            return new PollingLocking(this);
        }
#if false
        public PollingLocking Lock()
        {
            if (IsOn)
                throw new InvalidOperationException();

            return new PollingLocking(this);
        }
#endif
    }
}
