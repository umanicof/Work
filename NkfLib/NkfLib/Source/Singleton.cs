using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NkfLib
{
    /// <summary>
    /// シングルトンクラス
    /// ・思ったより使い勝手が悪い。他クラスを継承することができない。
    ///   
    /// 使用例:
    ///   〇 public class Foo : Singleton<Foo> {}
    ///   × public class Foo : Singleton<Bar> {}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        static T _instance;
        public static T Instance 
        {
            get {
                if (_instance == null) {
                    _instance = new Singleton<T>() as T;
                    Debug.Assert(_instance != null);
                }

                return _instance;
            }
        }

        /// <summary>
        /// Singlton<T> => T へのキャスト
        /// </summary>
        /// <param name="inheritance"></param>
        public static implicit operator T(Singleton<T> inheritance)
        {
            return inheritance as T;
        }

        /// <summary>
        /// 破棄
        /// ・再度Instanceを参照すると生成される
        /// </summary>
        public virtual void Dispose()
        {
            _instance = null;
        }
    }
}
