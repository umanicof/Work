using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// インスタンスが最大一つに制限されているオブジェクト
/// ・シングルトンとは厳密には違う。このクラスではインスタンスを生成しない。
/// ・思ったより使い勝手が悪い。他クラスを継承することができない。
/// ・インスタンスを作り直す場合はDisposeを呼び出すこと
/// 
/// 使用例:
///   〇 public class Foo : SingleObject<Foo> {}
///   × public class Foo : SingleObject<Bar> {}
/// </summary>
/// <typeparam name="T"></typeparam>

namespace NkfLib
{
    public class SingleObject<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// カレント
        /// </summary>
        public static T Current { get; private set; }

        /// <summary>
        /// SingleObject<T> => T へのキャスト
        /// </summary>
        /// <param name="inheritance"></param>
        public static implicit operator T(SingleObject<T> inheritance)
        {
            return inheritance as T;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SingleObject()
        {
            Debug.Assert(Current == null, "Instance already created.");
            Current = this as T;
            Debug.Assert(Current != null, "The template argument is not itself.");
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public virtual void Dispose()
        {
            Current = null;
        }
    }
}