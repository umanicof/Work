using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// BitamapImageキャッシュクラス
    /// ・this[]またはReferenceメソッドがコールされるタイミングで、指定の相対パスからBitmapImageを生成する。
    /// 　生成したBitmapImageクラスは相対パスをキーとした辞書にキャッシュされる。
    /// 　元々キャッシュがあればBitmapImageクラスは生成せずにキャッシュを返す。
    /// ・GroupIdによる一括のキャッシュクリアが可能。GroupIdはthis[]またはReferenceメソッドの引数となっている。
    /// ・他クラスによるBitmapImageの参照が無くなっても、BitmapImageは破棄されない。BitmapImageの破棄は明示的に行う必要がある。
    /// </summary>
    public class BitmapImageCache : GroupingDictionary<string, BitmapImage>
    {
#if false // @note ReplaceImageCacheと競合するので無効に
        /// <summary>
        /// 自身のインスタンス
        /// </summary>
        public static BitmapImageCache Current { get; } = new BitmapImageCache();
#endif

        /// <summary>
        /// インデクサ
        /// ・インデックスは文字列で 「キー|グループID」で指定する。グループIDは省略可能
        /// ・取得時にキーが存在しない場合、キーをリソースの相対パスと見なし、BitmapImageを自動生成する
        /// </summary>
        public new BitmapImage this[string key_groupId]
        {
            get
            {
                var strings = key_groupId.Split('|');
                var key = strings[0];
                var groupId = strings.Count() <= 1 ? null : strings[1];

                return GetBitmapImageOrCreate(key, groupId);
            }

            set 
            {
                var strings = key_groupId.Split('|');
                var key = strings[0];
                var groupId = strings.Count() <= 1 ? null : strings[1];

                Add(key, value, groupId);
            }
        }

        /// <summary>
        /// BitmapImageの取得または生成
        /// ・なければ指定のパスから生成および追加
        /// </summary>
        public BitmapImage GetBitmapImageOrCreate(string path, string groupId = null)
        {
            var bi = GetBitmapImage(path);
            if (bi != null) return bi;

            // BitmapImageの生成と登録
            bi = BitmapUtil.CreateBitmapImage(new Uri(ApplicationInfo.ResourcesDirectory + path, UriKind.Absolute)); // 相対パスを絶対パスに変換して使う
            if (bi != null) {
                Add(path, bi, groupId);
            }
            return bi;
        }

        /// <summary>
        /// BitmapImageの取得
        /// </summary>
        public BitmapImage GetBitmapImage(string key)
        {
            //Debug.Assert(!key.IsNullOrEmpty()); // Content.Dataにkeyがセット済みの前提でアクセスする箇所がある

            if (ContainsKey(key)) return base[key].Value; // キャッシュ有り

            return null;
        }

#if false
        /// <summary>
        /// BitmapImageの削除（グループID指定）
        /// ・デバッグ時のログ表示のために上書きしているだけ
        /// </summary>
        public new void RemoveByGroupId(string groupId = null)
        {
            foreach (var key in GetKeysByGroupId(groupId)) {
                DebugLog.WriteLine(string.Format("BitmapImageCache.Remove({0}, {1})", groupId, key));
            }

            base.RemoveByGroupId(groupId);
        }
        
        /// <summary>
        /// BitmapImageの追加
        /// ・デバッグ時のログ表示のために上書きしているだけ
        /// </summary>
        public new void Add(string key, BitmapImage bi, string groupId = null)
        {
            DebugLog.WriteLine(string.Format("BitmapImageCache.Add({0}, {1})", groupId, key));
            base.Add(key, bi, groupId);
        }
#endif
    }
}
