using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NkfLib.Unity
{
    /// <summary>
    /// インスタンスが最大一つに制限されている MonoBehavior
    /// ・シングルトンとは厳密には違うが、同じように扱っても良い
    /// ・思ったより使い勝手が悪い。他クラスを継承することができない。
    ///   
    /// 使用例:
    ///   〇 public class Foo : SingleMonoBehaviour<Foo> {}
    ///   × public class Foo : SingleMonoBehaviour<Bar> {}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SingleMonoBehaviour<T> : MonoBehaviour
    where T : class
    {
        /// <summary>
        /// カレント
        /// </summary>
        public static T Current { get; private set; }

        /// <summary>
        /// キャスト
        /// </summary>
        /// <param name="holder"></param>
        public static implicit operator T(SingleMonoBehaviour<T> inheritance)
        {
            return inheritance;
        }

        void Awake()
        {
            Debug.Assert(Current == null, "Instance already created.");
            Current = this as T;
            Debug.Assert(Current != null, "The template argument is not itself.");
        }

        void OnDestroy()
        {
            Current = null;
        }
    }
}