using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class ListExtensions
    {
        /// <summary>
        /// リサイズ
        /// ・デフォルトコンストラクタが有効な場合のみ
        /// 出典：https://stackoverflow.com/questions/12231569/is-there-in-c-sharp-a-method-for-listt-like-resize-in-c-for-vectort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sz"></param>
        /// <param name="c"></param>
        public static void Resize<T>(this List<T> list, int size, T c = default(T))
        {
            int cur = list.Count;
            if (size < cur)
            {
                list.RemoveRange(size, cur - size);
            }
            else if (size > cur)
            {
                list.AddRange(Enumerable.Repeat(c, size - cur));
            }
        }
    }
}