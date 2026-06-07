//#define ON_UNLOADED_DISPOSE  // アンロード時のプロセス終了の仕組み
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_WEBGL
using UniRx;
#else
using System.Reactive.Disposables;
#endif
namespace NkfLib
{
    /// <summary>
    /// Disposeパターンの実装クラス
    /// ・Disposeパターン：https://docs.microsoft.com/ja-jp/dotnet/standard/garbage-collection/implementing-dispose
    ///                    https://stackoverflow.com/questions/898828/use-of-finalize-dispose-method-in-c-sharp
    ///                    Effective C#4.0 項目17 Disposeパターンの標準的な実装
    ///
    /// ・Disposeパターンについて                   
    ///   IDisposable が実装されており、Dispose呼び出しまたはファイナライザ実行によってリソースが破棄される。
    ///   Dispose呼び出しが行われれば managed objects と unmanaged objects の両方が破棄され、ファイナライザ実行では umnagaged objects のみが破棄される。
    ///   （Disposeが呼び出されなくても、managed objects はファイナライザ実行時点で自動的に破棄されている。ファイナライザ呼び出しタイミングはGC次第）
    /// ・本クラスの実装について
    /// 　managedDisposed, ManagedDisposer はDispose呼び出し時にのみ処理され、disposed, Disposer はDispose呼び出しまたはファイナライザ実行の時点で
    /// 　処理される。
    /// 　厳密には managedDisposed, ManagedDisposer は managed objects用、disposed, Disposer は unmanaged objects用ということになるが、
    ///   その辺り意識せずに後者だけ使用するようにしても問題ないかも知れない。
    /// 
    /// 【重要】.NET Core や .NET 5 ではプロセス終了時に未実行のファイナライザが呼ばれないとのこと。（.NET Frameworkでは呼ばれる）
    ///         すなわち、手動でDisposeを呼ばないと解放されない。
    ///         出典：https://dotnetcsharptips.com/net5_finalizer/
    ///         本クラスにアセンブリのアンロード時に解放される仕組みを作成（ON_UNLOADED_DISPOSE）したが、近年のマルチプロセスOSではメモリ空間が
    ///         プロセス（プログラム）ごとに独立に確保されているので、プロセス終了とともに解放されるので問題は起こりにくい（Wiki調べ）
    ///         ということで無効化している。
    /// </summary>
    public class DisposePattern : IDisposable
    {
        public bool EnabledDebugLog { get; set; }

        public bool IsDisposed { get; private set; }

        public event Action disposed;       // unmanaged objects
        public event Action disposeCalled;  // managed objects（Disposeコール時）

        /// <summary>
        /// 破棄実行クラス（unmanaged objects用）
        /// ・IDisposableなクラスを登録することでDispose時に一緒に破棄してくれる。ReactivePropertyのAddTo先などに使用。
        /// </summary>
        public CompositeDisposable Disposer { get; private set; } = new CompositeDisposable();

        /// <summary>
        /// 明示的な破棄実行クラス（managed objects用）
        /// ・IDisposableなクラスを登録することでDispose時に一緒に破棄してくれる。ReactivePropertyのAddTo先などに使用。
        /// ・明示的なDispose呼び出し時のみ処理される
        /// </summary>
        public CompositeDisposable ManagedDisposer { get; private set; } = new CompositeDisposable();


#if !NETFRAMEWORK && ON_UNLOADED_DISPOSE
        /// <summary>
        // アセンブリのアンロード時の破棄実行クラス
        /// </summary>
        static CompositeDisposable UnloadDisposer { get; set; } = new();

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DisposePattern()
        {
            ApplicationInfo.Unloading += () => UnloadDisposer.Dispose();
        }
#endif
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DisposePattern(bool enabledDebugLog = true)
        {
            EnabledDebugLog = enabledDebugLog;
#if !NETFRAMEWORK && ON_UNLOADED_DISPOSE
            UnloadDisposer.Add(this);
#endif
        }

        /// <summary>
        /// 破棄（外部呼出し用）
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // ファイナライザを呼ばない
        }

        /// <summary>
        /// 破棄
        /// </summary>
        /// <param name="disposing">ファイナライザから呼ばれた場合はfalse</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (EnabledDebugLog)
                DebugLog.WriteLine($"[{GetType().Name}(DisposePattern)] Dispose({disposing})");

            if (disposing)
            {
                // dispose managed state (managed objects).
                ManagedDisposer.Dispose();
                disposeCalled?.Invoke();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.
            Disposer.Dispose();
            disposed?.Invoke();


#if !NETFRAMEWORK && ON_UNLOADED_DISPOSE
            UnloadDisposer.Remove(this);
#endif
            IsDisposed = true;
        }

        /// <summary>
        /// ファイナライザ
        /// 【重要】.NET 5ではプロセス終了時に自動的に呼ばれない
        /// </summary>
        ~DisposePattern()
        {
            if (EnabledDebugLog)
                DebugLog.WriteLine($"[{GetType().Name}(DisposePattern)] ~DisposePattern()");

            Dispose(false);
        }
    }
}