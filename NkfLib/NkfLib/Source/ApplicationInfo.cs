using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Reflection;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// アプリケーション情報
    /// ・主にAssemblyInfoから情報を取得
    /// </summary>
    public static partial class ApplicationInfo
    {
        /// <summary>
        /// 実行ファイルパス
        /// </summary>
        public static string ExeFilePath { get; } = GetAssemblyLocation();

        /// <summary>
        /// 実行ディレクトリ
        /// </summary>
        public static string ExeDirPath { get; } = System.IO.Path.GetDirectoryName(ExeFilePath);

        /// <summary>
        /// アセンブリ名（実行ファイル名から拡張子を除いたもの）
        /// </summary>
        public static string AssemblyName { get; } = System.IO.Path.GetFileNameWithoutExtension(ExeFilePath);

        /// <summary>
        /// タイトル
        /// ・いわゆるアプリケーション名としてはこちらを使用
        /// </summary>
        public static string Title { get; } = GetAssemblyTitle();

        /// <summary>
        /// 製品名
        /// </summary>
        public static string ProductName { get; } = GetAssemblyProduct();

        /// <summary>
        /// バージョン
        /// </summary>
        public static string Version { get; } = GetAssemblyInformationalVersion();

        /// <summary>
        /// 会社名（発行者）
        /// </summary>
        public static string Company { get; } = GetAssemblyCompany();

        /// <summary>
        /// コピーライト（発行者）
        /// </summary>
        public static string Copyright { get; } = GetAssemblyCopyright();

        /// <summary>
        /// 開発者
        /// </summary>
        public static string Developer { get; } = Setting.Developer;

        /// <summary>
        /// リソース参照のためのディレクトリURI名
        /// </summary>
        public static string ResourcesDirectory { get; } = "pack://application:,,,/" + ApplicationInfo.AssemblyName + ";component/";

        /// <summary>
        /// リソースの相対パス名の一覧
        /// ・小文字で取得される模様
        /// </summary>
        public static HashSet<string> ResourcePaths { get; } = GetResourcePaths();

        /// <summary>
        /// 指定のリソースの相対パスが存在するかどうか
        /// </summary>
        public static bool ExistsResourcePath(string path)
        {
            return ResourcePaths.Contains(path.ToLower()); // 小文字に変換して比較
        }

        /// <summary>
        /// AssemblyLocationの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// GetCustomAttributeの共通メソッド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static T GetCustomAttribute<T>()
            where T : System.Attribute
        {
            return (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        }

        /// <summary>
        /// AssemblyVersionの取得
        /// </summary>
        /// <returns></returns>
        static string AssemblyVersion()
        {
            var attr = GetCustomAttribute<AssemblyVersionAttribute>();
            return attr?.Version;
        }

        /// <summary>
        /// AssemblyInformationalVersionの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyInformationalVersion()
        {
            var attr = GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attr?.InformationalVersion;
        }

        /// <summary>
        /// AssemblyTitleの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyTitle()
        {
            var attr = GetCustomAttribute<AssemblyTitleAttribute>();
            return attr?.Title;
        }

        /// <summary>
        /// AssemblyDescriptionの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyDescription()
        {
            var attr = GetCustomAttribute<AssemblyDescriptionAttribute>();
            return attr?.Description;
        }

        /// <summary>
        /// AssemblyCompanyの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyCompany()
        {
            var attr = GetCustomAttribute<AssemblyCompanyAttribute>();
            return attr?.Company;
        }

        /// <summary>
        /// AssemblyProductの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyProduct()
        {
            var attr = GetCustomAttribute<AssemblyProductAttribute>();
            return attr?.Product;
        }

        /// <summary>
        /// AssemblyCopyrightの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyCopyright()
        {
            var attr = GetCustomAttribute<AssemblyCopyrightAttribute>();
            return attr?.Copyright;
        }

        /// <summary>
        /// AssemblyTrademarkの取得
        /// </summary>
        /// <returns></returns>
        static string GetAssemblyTrademark()
        {
            var attr = GetCustomAttribute<AssemblyTrademarkAttribute>();
            return attr?.Trademark;
        }


        /// <summary>
        /// リソースの相対パス名の一覧の取得
        /// </summary>
        /// <returns></returns>
        static HashSet<string> GetResourcePaths()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName)) {
                using (var reader = new System.Resources.ResourceReader(stream)) {
                    return reader.Cast<DictionaryEntry>().Select(entry => ((string)entry.Key).ToUrlDecode()).ToHashSet();
                }
            }
        }
    }
}
