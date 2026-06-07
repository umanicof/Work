using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_WEBGL
using UniRx;
#else
using Reactive.Bindings;
#endif

namespace NkfLib
{
    /// <summary>
    /// 言語辞書クラス
    /// ・KeyとValueはそれぞれ別々に言語・国コードが設定される想定
    /// ・Jsonファイルへのロード・セーブ機能を持つ
    /// ・起動時や言語・国コードが設定された時に自動的にロードされる
    /// </summary>
    public abstract class LanguageDictionary<TKey, TValue> : IReady
    {
        static readonly string kNullLanguageCode = "auto";

        /// <summary>
        /// 名前
        /// </summary>
        public virtual string Name { get; set; } = "dictionary";

        /// <summary>
        /// セーブ有効
        /// </summary>
        public virtual bool SaveEnabled { get; set; } = true;

        // デフォルトの相対ファイルパス
        // ・ファイル名は言語・国コードによって変わる
        // ・言語・国コードは基本的に国コード無し（"en", "jp"など）を使用する。ただし方言などで国コードが必要な場合は付ける。
        // ・ファイルは実行ファイルパスの下に配置されている想定
        // ・{0}は名前、{1}はKeyで使用する言語・国コード、{2}はValueで使用する言語・国コード
        public virtual string BaseFilePath { get; set; } = @"Data\{0}_{1}_{2}.json";

        /// <summary>
        /// 言語・国コード
        /// </summary>
        public ReactiveProperty<string> KeyLanguageCode { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ValueLanguageCode { get; private set; } = new ReactiveProperty<string>();

        /// <summary>
        /// 内容（辞書）
        /// </summary>
        public ReactiveProperty<Dictionary<TKey, TValue>> Contents { get; private set; } = new ReactiveProperty<Dictionary<TKey, TValue>>(new Dictionary<TKey, TValue>());

        /// <summary>
        /// 準備完了
        /// ・初期化時の非同期処理が完了した
        /// </summary>        
        public virtual bool IsReady => _isReady1 && _isReady2;
        bool _isReady1;
        bool _isReady2;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LanguageDictionary(string name = null, bool saveEnabled = true, string keyLangageCode = null, string valueLangageCode = null)
        {
            Debug.Assert(Contents.Value != null);

            if (!name.IsNullOrWhiteSpace())
            {
                Name = name;
            }
            SaveEnabled = saveEnabled;
            if (!keyLangageCode.IsNullOrWhiteSpace())
            {
                KeyLanguageCode.Value = keyLangageCode;
            }
            if (!valueLangageCode.IsNullOrWhiteSpace())
            {
                ValueLanguageCode.Value = valueLangageCode;
            }

            KeyLanguageCode.Subscribe(async value => {
                Contents.Value.Clear();
                await LoadAsync();
                _isReady1 = true;
            });
            ValueLanguageCode.Subscribe(async value => {
                Contents.Value.Clear();
                await LoadAsync();
                _isReady2 = true;
            });
        }

        /// <summary>
        /// ロード
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public virtual async Task<bool> LoadAsync()
        {
            var result = await JsonUtil.LoadAsync<Dictionary<TKey, TValue>>(GetFilePath());
            if (!result.success)
                return false;

            Debug.Assert(result.content != null);
            Contents.Value = result.content;
            return true;
        }

        /// <summary>
        /// セーブ
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public virtual async Task<bool> SaveAsync()
        {
            if (!SaveEnabled)
                return true; // セーブ無効は成功で返す

            return await JsonUtil.SaveAsync(GetFilePath(), Contents.Value);
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public virtual bool Delete()
        {
            if (!SaveEnabled)
                return true; // セーブ無効は成功で返す

            Contents.Value.Clear();
            return JsonUtil.Delete(GetFilePath());
        }

        /// <summary>
        /// ファイルパス取得
        /// </summary>
        /// <param name="keyLanguageCode">Keyの言語・国コード："en","ja-JP"など</param>
        /// <param name="valueLanguageCode">Valueの言語・国コード："en","ja-JP"など</param>
        /// <returns></returns>
        public virtual string GetFilePath()
        {
            return GetFilePath(KeyLanguageCode.Value, ValueLanguageCode.Value);
        }        
        protected virtual string GetFilePath(string keyLanguageCode, string valueLanguageCode)
        {
            keyLanguageCode = keyLanguageCode ?? kNullLanguageCode;
            valueLanguageCode = valueLanguageCode ?? kNullLanguageCode;

            var path = ApplicationInfo.ExeDirPath + @"\" + string.Format(BaseFilePath, Name, keyLanguageCode.ToLower(), valueLanguageCode.ToLower());
            DebugLog.WriteLine($"[StringCache:{Name}] GetLanguageFilePath: {path}");
            return path;
        }
    }
}
