using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class AutoMove : MonoBehaviour
    {
        [SerializeField] float _MoveSpeed = 3.0f;

        const float kMaxScreenOutX = Common.kMaxScreenX + 20.0f;
        const float kMinScreenOutX = Common.kMinScreenX - 20.0f;

        void Start()
        {
        }

        void Update()
        {
            // ‰æ–ÊŠO•â³
            if (transform.position.x > kMaxScreenOutX)
            {
                transform.position = new Vector3(kMinScreenOutX + 0.1f, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < kMinScreenOutX)
            {
                transform.position = new Vector3(kMaxScreenOutX - 0.1f, transform.position.y, transform.position.z);
            }

            var p = transform.position;
            p.x += Time.deltaTime * _MoveSpeed;
            transform.position = p;
        }
    }
}