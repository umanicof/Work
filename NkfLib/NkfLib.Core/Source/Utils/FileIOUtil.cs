using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// ファイルI/Oユーティリティ
    /// </summary>
    public static partial class FileIOUtil
    {       
        /// <summary>
        /// ディレクトリをコピーする
        /// </summary>
        /// <param name="targetPath">コピー先のディレクトリ</param>
        /// <param name="sourcePath">コピー元のディレクトリ</param>
        public static void CopyDirectory(string targetPath, string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !Directory.Exists(sourcePath)) return;
            if (string.IsNullOrEmpty(targetPath) || !Directory.Exists(targetPath)) return;
            //MessageBox.Show("CopyDirectory:" + sourcePath + " to " + targetPath);

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(
                sourcePath,
                targetPath,
                Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
                Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing
            );
        }
    }
}
