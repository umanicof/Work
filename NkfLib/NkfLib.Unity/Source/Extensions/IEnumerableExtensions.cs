using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NkfLib.Unity
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// ForEach拡張
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (T element in self)
            {
                action(element);
            }
        }
    }
}