using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    /// <summary>
    /// Base64ユーティリティ
    /// </summary>
    public static partial class Base64Util
    {
        /// <summary>
        /// File(binary) => Base64 読み込み
        /// </summary>
        public static string FileToBase64(string fileName)
        {
            byte[] bs = File.ReadAllBytes(fileName);
            return System.Convert.ToBase64String(bs);
        }

        /// <summary>
        /// Base64 => File(binaly) 読み込み
        /// </summary>
        public static void Base64ToFile(string fileName, string base64String)
        {
            byte[] bs = System.Convert.FromBase64String(base64String);
            File.WriteAllBytes(fileName, bs);
        }

        /// <summary>
        /// MemoryStream => Base64 変換
        /// </summary>
        public static string MemoryToBase64(MemoryStream ms)
        {
            return System.Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Base64 => MemoryStream 変換
        /// </summary>
        public static MemoryStream Base64ToMemory(string base64String)
        {
            return new MemoryStream(System.Convert.FromBase64String(base64String));
        }


    }
}
