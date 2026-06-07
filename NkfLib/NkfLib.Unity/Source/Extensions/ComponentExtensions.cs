using System.Linq;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// すべての子孫オブジェクトを返します
        /// </summary>
        /// <param name="self">Component 型のインスタンス</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
        /// <returns>子孫オブジェクトの配列</returns>
        public static GameObject[] GetDescendants(
            this Component self,
            bool includeInactive = false)
        {
            return self.GetComponentsInChildren<Transform>(includeInactive)
                .Where(c => c != self.transform)
                .Select(c => c.gameObject)
                .ToArray();
        }

        /// <summary>
        /// コンポーネントを削除します
        /// </summary>
        public static void RemoveComponent<T>(this Component self) where T : Component
        {
            GameObject.Destroy(self.GetComponent<T>());
        }
    }
}