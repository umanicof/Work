using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class AudioSourceExtensions
    {
        /// <summary>
        /// 自身のクリップを使用したワンショット再生
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static void PlayOneShot(this AudioSource self)
        {
            self.PlayOneShot(self.clip);
        }
    }
}