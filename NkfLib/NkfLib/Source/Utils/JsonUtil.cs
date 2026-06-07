using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    /// <summary>
    /// Jsonユーティリティ
    /// </summary>
    public static partial class JsonUtil
    {
        static readonly Encoding kDefaultEncoding = new UTF8Encoding(false);

        /// <summary>
        /// ロード
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public static async Task<(bool success, T content)> LoadAsync<T>(string filePath)
            where T : class           
        {
            try
            {
                if (!File.Exists(filePath))
                    return (false, null); // ファイル無しは失敗

                var json = await File.ReadAllTextAsync(filePath, kDefaultEncoding);
                var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                return (true, jobj);
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine($"[JsonUtil] LoadAsync error:{ex.Message}");
                return (false, null);
            }
        }
        
        /// <summary>
        /// セーブ
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public static async Task<bool> SaveAsync<T>(string filePath, T context)
        {
            try
            {
                FileIOUtil.CreatePassedDirectory(Path.GetDirectoryName(filePath)); // ディレクトリ作成
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(context, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json, kDefaultEncoding);
                return true;

            }
            catch (Exception ex)
            {
                DebugLog.WriteLine($"[JsonUtil] SaveAsync error:{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        public static bool Delete(string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;

            }
            catch (Exception ex)
            {
                DebugLog.WriteLine($"[JsonUtil] Delete error:{ex.Message}");
                return false;
            }
        }
    }
}
