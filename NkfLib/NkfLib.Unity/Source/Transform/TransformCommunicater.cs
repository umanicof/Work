using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using OneLine;

namespace NkfLib.Unity
{
    /// <summary>
    /// Transformのパラメータを指定のTargetに伝える
    /// ・親子関係を作りたくないがTransformのパラメータは伝達したい場合などに使用
    /// </summary>
    public class TransformCommunicater : MonoBehaviour
    {
        [System.Serializable]
        public struct Constraints
        {
            public bool position;
            public bool rotation;
            public bool scale;
        }

        [SerializeField] Transform _Target;
        [SerializeField] Constraints _Communication;

        void Update()
        {
            if (_Target == null)
                return;

            if (_Communication.position)
            {
                _Target.position = transform.position;
            }
            if (_Communication.rotation)
            {
                _Target.rotation = transform.rotation;
            }
            if (_Communication.scale)
            {
                _Target.localScale = transform.localScale;
            }
        }
    }
}