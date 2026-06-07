using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace NkfLib
{
    // <summary>
    /// ObservableCollection型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class ObservableCollectionExtensions
    {
        /// <summary>
        /// nullの場合は空のObservableCollectionを返却
        /// </summary>
        public static ObservableCollection<T> AsSafe<T>(this ObservableCollection<T> self)
        {
            return self ?? new ObservableCollection<T>();
        }
                
        /// <summary>
        /// RemoveAll
        /// </summary>
        public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            for (int i = collection.Count - 1; i >= 0; i--) {
                if (condition(collection[i])) {
                    collection.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// FindIndex
        /// </summary>
        public static int FindIndex<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            for (int i = 0; i < collection.Count; i++) {
                if (condition(collection[i])) return i;
            }
            return -1;
        }
    }
}
