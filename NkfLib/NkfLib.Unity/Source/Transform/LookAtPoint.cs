using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    [ExecuteInEditMode] // エディタで実行
    public class LookAtPoint : MonoBehaviour
    {
        [SerializeField] Vector3 point = Vector3.zero;

        void Update()
        {
            transform.LookAt(point, Vector3.forward);
        }
    }
}