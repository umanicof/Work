using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class Tag : MonoBehaviour
    {
        // タグ名
        [SerializeField] string _TagName = default;
        int _tagHash;

        // タグリスト
        static Dictionary<int, List<Tag>> Collections { get; } = new Dictionary<int, List<Tag>>();

        void Awake()
        {
            _tagHash = _TagName.Trim().GetHashCode();

            Collections.TryGetValue(_tagHash, out List<Tag> list);
            if (list == null)
            {
                list = new List<Tag>();
                Collections[_tagHash] = list;
            }
            list.Add(this);

        }

        // 破棄
        void OnDestroy()
        {
            Collections[_tagHash].Remove(this);
        }

        /// <summary>
        /// リスト取得
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static List<Tag> GetCollection(string tagName)
        {
            var hash = tagName.Trim().GetHashCode();
            Collections.TryGetValue(hash, out List<Tag> list);
            return list;
        }
    }
}