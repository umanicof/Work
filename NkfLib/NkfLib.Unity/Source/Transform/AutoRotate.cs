using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField]
        Vector3 _RotSpeed;

        void Update()
        {
            transform.Rotate(_RotSpeed * Time.deltaTime);
        }
    }
}