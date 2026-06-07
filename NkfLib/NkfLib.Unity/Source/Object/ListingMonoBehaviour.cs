using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NkfLib.Unity
{
    /// <summary>
    /// リスト管理される MonoBehavior
    /// ・思ったより使い勝手が悪い。他クラスを継承することができない。
    ///   例１のように使うとリスト管理されるのはFoo（シングルトンタイプ）
    ///   例２のように使うとリスト管理されるのはBar、FooがBarを生成・保持している形（ホルダータイプ）
    ///     => 例２のように使う場面は無いと思う
    ///     
    /// 使用例:
    ///   例１ public class Foo : ListingMonoBehaviour<Foo> {}
    ///   例２ public class Foo : ListingMonoBehaviour<Bar> {}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ListingMonoBehaviour<T> : MonoBehaviour
    where T : class, new()
    {
        T _instance;

        /// <summary>
        /// リスト
        /// </summary>
        public static List<T> List { get; } = new List<T>();

        void Awake()
        {
            _instance = this as T;
            if (_instance == null)
            {
                _instance = new T();
            }
            List.Add(_instance);
        }

        void OnDestroy()
        {
            List.Remove(_instance);
        }
    }
}