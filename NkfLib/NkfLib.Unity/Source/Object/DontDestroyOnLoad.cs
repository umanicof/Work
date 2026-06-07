using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField] bool _dontDestroyOnLoad = true;

        void Awake()
        {
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}