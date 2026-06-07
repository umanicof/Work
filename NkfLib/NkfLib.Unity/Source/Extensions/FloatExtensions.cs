using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class FloatExtensions
    {
        // .NET Framework側の話だが、float.Epsilonは計算機イプシロンではない。
        // http://nakamura001.hatenablog.com/entry/20150117/1421501942
        // https://blog.masuqat.net/2014/04/13/machine-epsilon-with-csharp/

        //const float kComputerEpsilon = 1.192093E-07f;
        const float kComputerEpsilon = Vector2.kEpsilon; // 余裕を持たせて

        /// <summary>
        /// 誤差を考慮した比較
        /// </summary>
        /// <param name="self"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static bool EqualsByEpsilon(this float self, float dst)
        {
            return Mathf.Abs(self - dst) < kComputerEpsilon;
        }
    }
}