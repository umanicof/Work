using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;
using System.Linq;

namespace NkfLib.Unity
{
    /// <summary>
    /// 拡張オブジェクトプールの対象オブジェクトのI/F
    /// </summary>
    public interface IPooledObject<T>
        where T : Component
    {
        public ObjectPoolEx<T> PoolRef { get; set; }

        public void OnRent();
        public void OnReturn();
    }
}