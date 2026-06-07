using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Toolkit;
using UnityEngine;
using UnityEngine.UI;

namespace NkfLib.Unity
{
    // 継承しなくても使える感じにラップ
    public class ObjectPoolWrap<T> : ObjectPool<T> where T : Component
    {
        Func<T> _onCreateInstance;

        protected override T CreateInstance()
        {
            Debug.Assert(_onCreateInstance != null);
            return _onCreateInstance.Invoke();
        }

        public void SetOnCreateInstance(Func<T> func)
        {
            _onCreateInstance = func;
        }
    }
}
