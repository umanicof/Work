using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    /// <summary>
    /// 翻訳I/F
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// 翻訳
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<(bool success, string text)> TranslateAsync(string text);
        public Task<(bool success, string text)> TranslateAsync(string text, string sourceLanguageCode, string targetLanguageCode);
    }
}
