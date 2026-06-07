using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    // <summary>
    /// IEnumerable型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class IEnumerableExtensions
    {
        /// <summary>
        /// nullの場合は空のEnumerableを返却
        /// </summary>
        public static IEnumerable<T> AsSafe<T>(this IEnumerable<T> self)
        {
            return self ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// シーケンスを指定されたサイズのチャンクに分割
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> self, int chunkSize)
        {
            if (chunkSize <= 0) {
                throw new ArgumentException("Chunk size must be greater than 0.", "chunkSize");
            }

            var result = new List<T>(chunkSize);
            foreach (var item in self) {
                result.Add(item);
                if (result.Count == chunkSize) {
                    yield return result;
                    result = new List<T>(chunkSize);
                }
            }
            if (result.Count != 0) {
                yield return result.ToArray();
            }

            /* こちらは何度も回って効率が悪いらしい
            while (self.Any()) {
                yield return self.Take(chunkSize);
                self = self.Skip(chunkSize);
            }
            */
        }

        /// <summary>
        /// 要素をランダムに取得
        /// </summary>
        public static T ElementAtRandom<T>(this IEnumerable<T> self)
        {
            if (self.Count() <= 0) return default(T);

            Random r = new Random();
            return self.ElementAt(r.Next(self.Count()));
        }

        /// <summary>
        /// ForEach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self) {
                action(item);
            }
        }

#if false // LINQのメソッドと競合する場合がある
        /// <summary>
        /// ToHashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
        {
            return new HashSet<T>(self);
        }
#endif

        /// <summary>
        /// Null除外
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> self)
            where T : class
        {
            //if (self == null)
            //    return Enumerable.Empty<T>();

            return self.Where(x => x != null)!;
        }
    }
}
