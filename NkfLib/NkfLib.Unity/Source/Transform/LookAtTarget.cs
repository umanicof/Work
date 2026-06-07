//using OneLine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    [ExecuteInEditMode] // エディタで実行
    public class LookAtTarget : MonoBehaviour
    {
        [System.Serializable]
        public struct Constraints
        {
            public bool x;
            public bool y;
            public bool z;
        }
        [field: SerializeField] public Transform Target { get; set; }
        //[SerializeField, OneLine(Header = LineHeader.Short)] Constraints _freezeRotate;
        [SerializeField] Constraints _freezeRotate;

        void Update()
        {
            if (Target == null)
                return;

            var lookAtPosition = Target.position;
            if (_freezeRotate.x)
            {
                lookAtPosition.z = transform.position.z;
            }
            if (_freezeRotate.y)
            {
                lookAtPosition.x = transform.position.x;
            }
            if (_freezeRotate.z)
            {
                lookAtPosition.y = transform.position.y;
            }

            transform.LookAt(lookAtPosition, Vector3.forward);
        }
    }
}