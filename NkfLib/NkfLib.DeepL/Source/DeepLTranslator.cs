using DeepL;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace NkfLib
{
    /// <summary>
    /// DeepL翻訳クラス
    /// ・DeepL.NETのドキュメントなど：https://github.com/lecode-official/deepl-dotnet/tree/master/documentation
    /// ・HttpClientの取り扱いの問題：https://qiita.com/nskhara/items/b7c31d60531ffbe29537
    /// 　=> DeepL.NET内部でHttpClientを使っているので考慮が必要と考えている
    /// ・国・言語コード：https://so-zou.jp/web-app/tech/data/code/language.htm
    ///   => RFC4646。大文字小文字は区別されないとのこと。
    /// ・本クラスではDisposeパターンを実装しているので、Disposeを明示的に呼び出さなくても解放される。
    /// ・DeepL.NETにはHttpClientの破棄のためにDisposeパターンが実装されているようなので、
    ///   本クラスに実装する必要は無かったかもしれない。
    /// 
    /// 使用例：
    /// 　DeepLTranslator _deepl = new DeepLTranslator();
    /// 　
    ///   async Task init()
    ///   {
    ///       _deepl.AuthenticationKey = "<authentication-key>"
    ///       _deepl.TargetLanguageCode.Value = "ja";
    ///       _ = _deepl.UpdateApiInfoAsync();
    ///       await _deepl.TranslateAsync("hello world!");
    ///   }
    /// </summary>
    public class DeepLTranslator : DisposePattern, ITranslator
    {
        // 設定値
        public ReactivePropertySlim<bool> UseFreeApi { get; set; } = new(true);
        public ReactivePropertySlim<string> AuthenticationKey { get; set; } = new();

        // 翻訳元言語（nullの場合は自動判定）
        public ReactivePropertySlim<string> SourceLanguageCode { get; set; } = new();
        SupportedLanguage _sourceLanguage;

        // 翻訳先言語
        public ReactivePropertySlim<string> TargetLanguageCode { get; set; } = new("ja");
        SupportedLanguage _targetLanguage;

        // 設定値
        public ReactivePropertySlim<UsageStatistics> Usage { get; private set; } = new();
        public ReactivePropertySlim<List<SupportedLanguage>> SupportedSourceLanguages { get; private set; } = new();
        public ReactivePropertySlim<List<SupportedLanguage>> SupportedTargetLanguages { get; private set; } = new();

        DeepLClient _client;
        DeepLClient Client
        {
            get {
                Debug.Assert(!AuthenticationKey.Value.IsNullOrEmpty());

                if (_client == null)
                {
                    _client = new DeepLClient(AuthenticationKey.Value, useFreeApi: UseFreeApi.Value);
                }
                return _client;
            }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DeepLTranslator()
        { 
            // HttpClientがDNSの変更を検知しない問題への対処
            var sp = ServicePointManager.FindServicePoint(new Uri(DeepLClient.freeApiBaseUrl));
            sp.ConnectionLeaseTimeout = 60*1000; // 1 minute
            sp = ServicePointManager.FindServicePoint(new Uri(DeepLClient.proApiBaseUrl));
            sp.ConnectionLeaseTimeout = 60*1000; // 1 minute 
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DeepLTranslator(string authenticationKey = null, bool useFreeApi = true)
        {
            AuthenticationKey.Value = authenticationKey;
            UseFreeApi.Value = useFreeApi;

            // 設定パラメータ変更時のDeepLClient再生成
            UseFreeApi.Subscribe(_ => {
                DisposeClient(); // 破棄（次回通信時に生成）
            }).AddTo(ManagedDisposer);
            AuthenticationKey.Subscribe(_ => {
                DisposeClient(); // 破棄（次回通信時に生成）
            }).AddTo(ManagedDisposer);

            // 翻訳元、翻訳先言語指定
            SourceLanguageCode.Subscribe(value => {
                _sourceLanguage = value.IsNullOrWhiteSpace() ? null : DeepLClient.CodeToSupportedLanguage(value, true);
            }).AddTo(ManagedDisposer);
            TargetLanguageCode.Subscribe(value => {
                Debug.Assert(!value.IsNullOrWhiteSpace());
                _targetLanguage = DeepLClient.CodeToSupportedLanguage(value, false);
            }).AddTo(ManagedDisposer);

            // 破棄
            // ・HttpClientはunmanaged objectsを使っている可能性が高い
            //   ただ、Disposeパターンを実装しているようなので、放っておいても良さそう
            disposed += () =>
            {
                DisposeClient();
            };
        }

        /// <summary>
        /// DeepLClientの破棄
        /// </summary>
        void DisposeClient()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
            _client = null;
        }

        /// <summary>
        /// API情報を更新
        /// ・Usage, SupportedLaunguageが更新される
        /// ・本クラスの生成時にコールすることを推奨
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public async Task<bool> UpdateApiInfoAsync()
        {
            try
            {
                // 使用法取得
                var task1 = Task.Run(() =>
                {
                    Usage.Value = Client.GetUsageStatisticsAsync().Result;
                    if (Usage.Value == null)
                        return;
                    DebugLog.WriteLine($"[DeepLTranslator] CharacterCount:{Usage.Value.CharacterCount}");
                    DebugLog.WriteLine($"[DeepLTranslator] CharacterLimit:{Usage.Value.CharacterLimit}");
                });

                // サポート言語取得
                var task2 = Task.Run(() =>
                {
                    SupportedSourceLanguages.Value = Client.GetSupportedLanguagesAsync(true).Result.ToList();
                    if (SupportedSourceLanguages.Value == null)
                        return;
                    var value = string.Join(", ", SupportedSourceLanguages.Value.Select(x => x.LanguageCode));
                    DebugLog.WriteLine($"[DeepLTranslator] SupportedSourceLanguages:{value}");
                });
                var task3 = Task.Run(() =>
                {
                    SupportedTargetLanguages.Value = Client.GetSupportedLanguagesAsync(false).Result.ToList();
                    if (SupportedTargetLanguages.Value == null)
                        return;
                    var value = string.Join(", ", SupportedTargetLanguages.Value.Select(x => x.LanguageCode));
                    DebugLog.WriteLine($"[DeepLTranslator] SupportedTargetLanguages:{value}");
                });

                await Task.WhenAll(task1, task2, task3);
                return true;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine($"[DeepLTranslator] UpdateApiInfoAsync error:  {ex.InnerException.Message}");
                return false;
            }
        }

        /// <summary>
        /// 翻訳
        /// </summary>
        /// <param name="text"></param>
        public async Task<(bool success, string text)> TranslateAsync(string text)
        {
            return await TranslateAsync(text, _sourceLanguage, _targetLanguage);
        }
        public async Task<(bool success, string text)> TranslateAsync(string text, string sourceLanguageCode, string targetLanguageCode)
        {
            return await TranslateAsync(text,
                                        sourceLanguageCode.IsNullOrWhiteSpace() ? null : DeepLClient.CodeToSupportedLanguage(sourceLanguageCode, true),
                                        targetLanguageCode.IsNullOrWhiteSpace() ? null : DeepLClient.CodeToSupportedLanguage(targetLanguageCode, false));
        }
        async Task<(bool success, string text)> TranslateAsync(string text, SupportedLanguage sourceLanguage, SupportedLanguage targetLanguage)
        {
            Debug.Assert(targetLanguage != null);

            try {
                //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                Translation translation = await Client.TranslateAsync(
                    text,
                    sourceLanguage, // null可
                    targetLanguage
                //cancellationToken: cancellationTokenSource.Token
                );
                //Console.WriteLine(translation.DetectedSourceLanguage);
                DebugLog.WriteLine($"[DeepLTranslator] TranslateAsync text={translation.Text}");
                return (true, translation.Text);
            }
            catch (Exception ex) {
                DebugLog.WriteLine($"[DeepLTranslator] TranslateAsync error: {ex.Message}");
                return (false, null);
            }
        }
    }
}
