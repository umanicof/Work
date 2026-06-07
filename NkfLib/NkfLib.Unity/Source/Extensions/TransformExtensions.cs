using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 子をリストとして取得
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenToList(this Transform self)
        {
            //return  self.Cast<Transform>().ToList(); // 例外あり
            return self.OfType<Transform>().ToList(); // 例外なし（as）
        }
    }
}