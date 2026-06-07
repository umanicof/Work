using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    // <summary>
    /// WeakReference型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class WeakReferenceExtensions
    {
        /// <summary>
        /// 参照先の取得
        /// </summary>
        public static T GetTarget<T>(this WeakReference<T> self) where T : class
        {
            T value;
            return self.TryGetTarget(out value) ? value : null;
        }
    }
}
