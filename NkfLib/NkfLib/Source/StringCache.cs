using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// 文字列キャッシュクラスの要素
    /// </summary>
    public class StringCacheValue
    {
        public string text;
        public bool hold;
        public DateTime date;
    }

    /// <summary>
    /// 文字列キャッシュクラス
    /// </summary>
    public class StringCache : LanguageDictionary<string, StringCacheValue>
    {         
        /// <summary>
        /// 名前
        /// </summary>
        public override string Name { get; set; } = "cache";

        // デフォルトの相対ファイルパス
        public override string BaseFilePath { get; set; } = @"Caches\{0}_{1}_{2}.json";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StringCache(string name = null, bool saveEnabled = true, string keyLangageCode = null, string valueLangageCode = null)
            : base(name, saveEnabled, keyLangageCode, valueLangageCode)
        {
        }
    }
}
