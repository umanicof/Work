using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// グループ化辞書クラス
    /// ・辞書にグループ管理の機能をつけたようなもの。
    /// ・@note: TGroupIdも作ってみたがLinqで問題が出たので却下。型を固定しないと比較などでも困る。
    /// </summary>
    public class GroupingData<TValue>
    {
        public string GroupId { get; set; } // グループID
        public TValue Value { get; set; }   // 管理対象オブジェクト
    }

    public class GroupingDictionary<TKey, TValue> : Dictionary<TKey, GroupingData<TValue>>
    {
        /// <summary>
        /// オブジェクトの追加
        /// </summary>
        public void Add(TKey key, TValue value, string groupId = null)
        {
            this[key] = new GroupingData<TValue> { Value = value, GroupId = groupId };
        }

        /// <summary>
        /// オブジェクトの削除（グループID指定）
        /// </summary>
        public void RemoveByGroupId(string groupId = null)
        {
            var keys = this.Where(x => x.Value.GroupId == groupId).Select(x => x.Key).ToList();
            foreach (var key in keys) {
                Remove(key);
            }
        }

        /// <summary>
        /// オブジェクトの取得
        /// ・なければデフォルト値が返る
        /// </summary>
        public TValue GetValue(TKey key)
        {
            return ContainsKey(key) ? this[key].Value : default(TValue);
        }

        /// <summary>
        /// オブジェクトのリストの取得（グループID指定）
        /// </summary>
        public IEnumerable<TKey> GetKeysByGroupId(string groupId)
        {
            var keys = this.Where(x => x.Value.GroupId == groupId).Select(x => x.Key);
            foreach (var key in keys) {
                yield return key;
            }
        }
        
        /// <summary>
        /// オブジェクトのリストの取得（グループID指定）
        /// </summary>
        public IEnumerable<TValue> GetValuesByGroupId(string groupId)
        {
            var keys = this.Where(x => x.Value.GroupId == groupId).Select(x => x.Key);
            foreach (var key in keys) {
                yield return GetValue(key);
            }
        }

        /// <summary>
        /// キーとオブジェクトのリストの取得（グループID指定）
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> GetPairsByGroupId(string groupId)
        {
            var keys = this.Where(x => x.Value.GroupId == groupId).Select(x => x.Key);
            foreach (var key in keys) {
                yield return new KeyValuePair<TKey, TValue>(key, GetValue(key));
            }
        }
        
        /// <summary>
        /// グループIDの取得
        /// ・なければデフォルト値が返る
        /// </summary>
        public object GetGroupId(TKey key)
        {
            return ContainsKey(key) ? this[key].GroupId : null;

        }

        /// <summary>
        /// グループIDの設定（上書き）
        /// </summary>
        public void SetGroupId(TKey key, string groupId)
        {
            if (ContainsKey(key)) {
                this[key].GroupId = groupId;
            }
        }

        /// <summary>
        /// デバッグ表示
        /// </summary>
        public void DebugWriteLine()
        {
#if DEBUG
            Debug.WriteLine("GroupindDictionary Count:" + Count);
            foreach (var pair in this) {
                Debug.WriteLine(string.Format("+ GroupId:{0}, Key:{1}, Value:{2}", pair.Value.GroupId, pair.Key, pair.Value.Value));
            }
#endif
        }
    }
}
