using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace NkfLib.Unity
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// プレハブのインスタンスかどうかを判定
        /// </remarks>
        public static bool IsPrefabInstance(this GameObject self)
        {
#if false // WebGLのビルドでエラーが出たので無効に
            return PrefabUtility.GetCorrespondingObjectFromSource(self) == null;
#else
            Debug.Assert(false);
            return false;
#endif
        }

        /// <summary>
        /// すべての子孫オブジェクトを返します
        /// </summary>
        /// <param name="self">GameObject 型のインスタンス</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
        /// <returns>子孫オブジェクトの配列</returns>
        public static GameObject[] GetDescendants(
        this GameObject self,
        bool includeInactive = false)
        {
            return self.GetComponentsInChildren<Transform>(includeInactive)
                .Where(c => c != self.transform)
                .Select(c => c.gameObject)
                .ToArray();
        }

        /// <summary>
        /// すべての子オブジェクトを返します
        /// </summary>
        /// <param name="self">GameObject 型のインスタンス</param>
        /// <returns>子オブジェクトの配列</returns>
        public static IEnumerable<GameObject> GetChildren(this GameObject self)
        {
            //return ((IEnumerable<Transform>)self.transform).Select(x => x.gameObject);

            var list = new List<GameObject>();
            foreach (Transform childTransform in self.transform)
            {
                list.Add(childTransform.gameObject);
            }
            return list;
        }

        /// <summary>
        /// コンポーネントを削除します
        /// </summary>
        public static void RemoveComponent<T>(this GameObject self) where T : Component
        {
            var target = self.GetComponent<T>();
            if (target)
            {
                GameObject.Destroy(target);
            }
        }
    }
}