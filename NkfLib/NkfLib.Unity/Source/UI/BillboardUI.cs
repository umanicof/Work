using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class BillboardUI : MonoBehaviour
    {
        void LateUpdate()
        {
            //　カメラと同じ向きに設定
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}