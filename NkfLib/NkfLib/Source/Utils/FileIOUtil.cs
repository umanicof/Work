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
        /// 安全なファイル削除（非同期）
        /// ・ファイル削除が完全に行われるまで繰り返す。例外エラーも発生させない。
        /// </summary>
        public static async Task DeleteFileSafeAsync(string path)
        {
            bool success = false;
            do {
                try {
                    if (File.Exists(path)) {
                        File.Delete(path);
                    }
                    success = true;
                }
                catch {
                    await Task.Delay(500);
                }
            } while (!success);
        }

        /// <summary>
        /// 指定したディレクトリの完全な削除
        /// </summary>
        /// <param name="path"></param>
        /// <param name="removeSelf">自身を削除するかどうか</param>
        public static void DeleteDirectory(string path, bool removeSelf = true)
        {
            DeleteDirectory(new DirectoryInfo(path), removeSelf);
        }
        private static void DeleteDirectory(DirectoryInfo hDirectoryInfo, bool removeSelf = true)
        {
            // すべてのファイルの読み取り専用属性を解除する
            foreach (FileInfo cFileInfo in hDirectoryInfo.GetFiles()) {
                if ((cFileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    cFileInfo.Attributes = FileAttributes.Normal;
                }
                cFileInfo.Delete();
            }

            // サブディレクトリ内の読み取り専用属性を解除する (再帰)
            foreach (DirectoryInfo hDirInfo in hDirectoryInfo.GetDirectories()) {
                DeleteDirectory(hDirInfo);
            }

            if (removeSelf) {
                // このディレクトリの読み取り専用属性を解除する
                if ((hDirectoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    hDirectoryInfo.Attributes = FileAttributes.Directory;
                }

                // このディレクトリを削除する
                hDirectoryInfo.Delete(true);
            }
        }

        /// <summary>
        /// パスを通るすべてのディレクトリとサブディレクトリを作成
        /// </summary>
        /// <param name="dirPath">ディレクトリのパス（ファイルパスは指定しない）</param>
        /// <returns></returns>
        public static DirectoryInfo CreatePassedDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
                return null;

            return Directory.CreateDirectory(dirPath);
        }
    }
}
