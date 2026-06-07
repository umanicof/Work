using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class CollideTest : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            Debug.Log(gameObject.name + " collide " + collision.gameObject.name);
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log(gameObject.name + " trigger " + other.gameObject.name);
        }
    }
}