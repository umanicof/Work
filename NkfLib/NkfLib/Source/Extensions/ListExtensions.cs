using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    // <summary>
    /// List型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class ListExtensions
    {
        /// <summary>
        /// シャッフル ※破壊的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> self)
        {
            Random r = new Random();
            for (int i = 0; i < self.Count; i++) {
                T temp = self[i];
                int randomIndex = r.Next(self.Count());
                self[i] = self[randomIndex];
                self[randomIndex] = temp;
            }

            return self;
        }


        /// <summary>
        /// 回転（↑方向） ※破壊的
        /// </summary>
        public static List<T> RotateT<T>(this List<T> self)
        {
            if (self.Count <= 0)
            {
                return self;
            }

            T temp = self[0];
            self.RemoveAt(0);
            self.Add(temp);
            return self;
        }

        /// <summary>
        /// ToSortedSet
        /// </summary>
        public static SortedSet<T> ToSortedSet<T>(this List<T> self)
        {
            SortedSet<T> set = new SortedSet<T>();
            if (self.Count <= 0)
            {
                return set;
            }

            foreach (var item in self)
            {
                set.Add(item);
            }
            return set;
        }
    }
}
