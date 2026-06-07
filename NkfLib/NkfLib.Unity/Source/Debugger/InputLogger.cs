using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NkfLib.Unity
{
    public class InputLogger : MonoBehaviour
    {
        float _horizontal;
        float _vertical;

        void Update()
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        Debug.Log(code);
                        break;
                    }
                }
            }
            if (!Mathf.Approximately(Input.GetAxis("Horizontal"), _horizontal))
            {
                Debug.Log("Horizontal:" + Input.GetAxis("Horizontal"));
                _horizontal = Input.GetAxis("Horizontal");
            }
            if (!Mathf.Approximately(Input.GetAxis("Vertical"), _vertical))
            {
                Debug.Log("Vertical:" + Input.GetAxis("Vertical"));
                _vertical = Input.GetAxis("Vertical");
            }
        }
    }
}