using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    /// <summary>
    /// プロジェクト内で一つしか生成されないオブジェクトを保持
    /// ・主にジェネリッククラスにおいて異なるテンプレート引数間でstaticなオブジェクトを保持したい場合に使用する
    /// ・保持するオブジェクトは本クラスで生成する（使用元で生成すると複数回生成される恐れがあるため）
    /// 
    /// 使用例：
    ///   StaticHolder<List<Foo>>.Value.Add(new Foo());
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StaticHolder<T>
    where T : class, new()
    {
        public static T Value { get; } = new T();

        /// <summary>
        /// キャスト
        /// </summary>
        /// <param name="holder"></param>
        public static implicit operator T(StaticHolder<T> holder)
        {
            return Value;
        }
        //public static implicit operator StaticHolder<T>(T target)
        //{
        //    Value = target;
        //    return new StaticHolder<T>();
        //}
    }
}