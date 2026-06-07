using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    // <summary>
    /// IList型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class IListExtensions
    {
        /// <summary>
        /// nullの場合は空のListを返却
        /// </summary>
        public static IList AsSafe(this IList self)
        {
            return self ?? new object[0]; // この定義がベストかは不明
        }

        /// <summary>
        /// swap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public static IList<T> Swap<T>(this IList<T> list, int index1, int index2)
        {
            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
            return list;
        }
    }
}
