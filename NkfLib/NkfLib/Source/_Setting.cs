using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NkfLib
{
    public partial class Setting
    {
        public static readonly string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss"; // DateTimeの基本的なフォーマット
        public static readonly string Developer = "";

        /// <summary>
        /// ファイル名
        /// </summary>
        public static readonly string DebugLogName  = "debug.log";

        /// <summary>
        /// ファイルパス
        /// </summary>
        public static readonly string DebugLogPath  = Path.GetFullPath(DebugLogName);

        /// <summary>
        /// フォルダパス
        /// ・ローカルファイルのフォルダパスをデータフォルダパスとする
        /// </summary>
        public static readonly string DataFolderPath = Path.GetDirectoryName(DebugLogPath);
    }
}
