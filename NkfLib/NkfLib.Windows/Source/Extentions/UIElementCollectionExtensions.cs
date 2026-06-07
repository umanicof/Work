using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace NkfLib
{
    // <summary>
    /// UIElementCollection型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class UIElementCollectionExtensions
    {
        /// <summary>
        /// 先頭要素を取得
        /// </summary>
        public static UIElement FirstOrDefault(this UIElementCollection self)
        {
            if (self.Count <= 0) return null;

            return self[0];
        }

        /// <summary>
        /// 終端要素を取得
        /// </summary>
        public static UIElement LastOrDefault(this UIElementCollection self)
        {
            if (self.Count <= 0) return null;

            return self[self.Count - 1];
        }

        /// <summary>
        /// 指定の型の要素を全て削除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        public static void RemoveAll<T>(this UIElementCollection self)
             where T : UIElement
        {
            var elements = self.OfType<T>().ToList();
            foreach (var e in elements)
            {
                self.Remove(e);
            }
        }
    }
}
