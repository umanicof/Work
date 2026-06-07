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
    /// ローカライズクラス
    /// </summary>
    public class Localizer : LanguageDictionary<string, string>
    {
        const string kValueLanguageCodeDefault = "en";
        static readonly string kKeyLanguageCode = "en";
        static readonly string kName = "language";

        // デフォルトの相対ファイルパス
        public override string BaseFilePath { get; set; } = @"Settings\{0}_{2}.json";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="valueLangageCode"></param>
        public Localizer(string valueLangageCode = kValueLanguageCodeDefault) : 
            base(kName, true, kKeyLanguageCode, valueLangageCode)
        { 
        }

        /// <summary>
        /// ローカライズテキスト（添え字指定）
        /// ・指定の値が無ければ添え字を返却
        ///   => 定義がなければkeyをそのまま使用する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        { 
            get
            {
                if (Contents.Value.ContainsKey(key))
                {
                    //Debug.WriteLine($"[Localizer] Contains this[{key}]");
                    return Contents.Value[key];
                }
                return key;
            }
        }

        /// <summary>
        /// 翻訳セーブ
        /// ・指定言語に翻訳したファイルを新たにセーブする
        /// ・開発時のローカライズファイル作成用
        /// </summary>
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public async Task<bool> TransferSaveAsync(string valuelanguageCode, ITranslator translator)
        {
            //if (!SaveEnabled)
            //    return true; // セーブ無効は成功で返す

            var newContents = new Dictionary<string, string>();
            foreach (var pair in Contents.Value)
            {
                var result = await translator.TranslateAsync(pair.Value, KeyLanguageCode.Value, valuelanguageCode);
                newContents[pair.Key] = result.text;
            }

            var path = GetFilePath(KeyLanguageCode.Value, valuelanguageCode);
            return await JsonUtil.SaveAsync(path, newContents);
        }
    }
}
