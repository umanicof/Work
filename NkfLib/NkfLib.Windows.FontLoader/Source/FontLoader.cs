using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using WaterTrans.TypeLoader;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// グリフ型情報
    /// ・他に欲しいメトリック情報（メモ）
    ///   Ruby：ルビ、ital：筆記体
    /// </summary>
    public class GlyphTypeInfo
    {
        public Uri Uri { get; set; }
        public FontFamily FontFamily { get; set; }
        public GlyphTypeface Gtf { get; set; }
        public SingleGlyphConverter VConv { get; set; }
        public SingleAdjustmentMetrics PaltH { get; set; } // 横書き用 Proportional Alternate Widths。Glyphの全角幅の代わりのメトリクスとして適用する。
        public SingleAdjustmentMetrics HaltH { get; set; } // 横書き用 Alternate Half Widths。カッコや句読点や記号などの半角幅のGlyphのメトリクスを全角幅にするために適用する
        public SingleAdjustmentMetrics PaltV { get; set; } // 縦書き用
        public SingleAdjustmentMetrics HaltV { get; set; } // 縦書き用
    }

    /// <summary>
    /// フォントローダークラス
    /// ・GlyphTypeInfoをフォントファミリ名ごとに管理する
    /// 　@note: 本当は同一のフォントファミリ名において、さらにボールドや斜体などのバリエーションを管理しないといけないはずだが、やっていない
    /// </summary>
    public class FontLoader
    {
        /// <summary>
        /// ロード済みのグリフ型情報
        /// ・キーはフォントファミリ名
        /// </summary>
        public static Dictionary<string, GlyphTypeInfo> LoadedGlyphTypeInfo { get; private set; } = new Dictionary<string, GlyphTypeInfo>();

        /// <summary>
        /// 明示的な初期化
        /// ・処理は無いが明示的に静的コンストラクタを起動するためにある
        /// ・少々妙な仕組みだが、これを呼ばなくても動く、と考えたら悪くない仕組みかも。
        /// </summary>
        public static void Init() { }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static FontLoader()
        {
            // 【デザインモード】
            if (Util.InDesignMode())
                return;

            LoadEmbedded(); // 埋め込みフォントをロード
        }

        /// <summary>
        /// グリフ型情報生成
        /// </summary>
        /// <returns></returns>
        static void AddGlyphTypeInfo(string familyName, Uri uri)
        {
            if (LoadedGlyphTypeInfo.ContainsKey(familyName)) return; // 登録済み

            var gti = new GlyphTypeInfo();
            gti.Uri = uri;
            gti.FontFamily = new FontFamily(uri, "./#" + familyName);

            //gti.Gtf = new GlyphTypeface(uri, StyleSimulations);
            gti.Gtf = new GlyphTypeface(uri);

            // .ttcのURIではFragmentに数値が入っている。ただし、埋め込みフォントにもFragmentが使われている
            //  @note: 埋め込みフォントかつ.ttcの場合は不具合になるかも
            int num = 0;
            int.TryParse(uri.Fragment.Replace("#", ""), out num);
            TypefaceInfo ti = new TypefaceInfo(gti.Gtf.GetFontStream(), num);

            gti.VConv = ti.GetVerticalGlyphConverter();
            //vconv = ti.GetAdvancedVerticalGlyphConverter(); // 別タイプ（英数字が90度回転）

            gti.PaltH = ti.GetProportionalAdjustmentMetrics(false);
            gti.HaltH = ti.GetHalfAdjustmentMetrics(false);
            gti.PaltV = ti.GetProportionalAdjustmentMetrics(true);
            gti.HaltV = ti.GetHalfAdjustmentMetrics(true);

            LoadedGlyphTypeInfo[familyName] = gti;
        }

        /// <summary>
        /// インストール済みのフォントをロード
        /// ・インストールフォントの上書きが多い環境だと、例外エラー発生でデバッグ表示が増えて重くなる
        /// ・インストールフォントが多い環境だとメモリを大量に消費する
        ///   => 基本的にこのメソッドは使わない方が良い
        /// </summary>
        /// <returns></returns>
        static bool _isLoadedInstalled;
        public static void LoadInstalled()
        {
            if (_isLoadedInstalled) return;
            _isLoadedInstalled = true;

            // フォントURIのリストの作成
            var uris = new List<Uri>();
            string fontDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            foreach (string filePath in System.IO.Directory.GetFiles(fontDir, "*.ttf")) { // TrueType
                uris.Add(new Uri(filePath));
            }

            foreach (string filePath in System.IO.Directory.GetFiles(fontDir, "*.otf")) { // OpenType
                uris.Add(new Uri(filePath));
            }

            foreach (string filePath in System.IO.Directory.GetFiles(fontDir, "*.ttc")) { // TrueTypeCollection
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    for (int i = 0; i < TypefaceInfo.GetCollectionCount(fs); i++) {
                        uris.Add(new UriBuilder("file", "", -1, filePath, "#" + i.ToString()).Uri);
                    }
                }
            }

            // フォントURIのリストからファミリ名を取得し、ファミリ名とURIの辞書を作成
            foreach (Uri uri in uris) {
                try {
                    GlyphTypeface gtf = new GlyphTypeface(uri); // 例外が発生するファイルが存在する
                    foreach (string familyName in gtf.FamilyNames.Values) {
                        AddGlyphTypeInfo(familyName, uri);
                    }
                }
                catch (NullReferenceException) { }
            }
        }

        /// <summary>
        /// 埋め込みフォントのURI対応辞書をロード
        /// ・@note: DLL化するならAssemblyの参照の仕方を考える必要がある
        /// </summary>
        /// <returns></returns>
        static bool _isLoadedEmbedded;
        public static void LoadEmbedded()
        {
            if (_isLoadedEmbedded)
                return;
            _isLoadedEmbedded = true;

            // リソースファイルの読み込み
            Assembly asm = Assembly.GetEntryAssembly();
            string resName = asm.GetName().Name + ".g.resources";
            using (var stream = asm.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream)) {
                foreach (var name in reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key)) {
                    string ext = System.IO.Path.GetExtension(name);
                    if (ext != ".otf" && ext != ".ttf" && ext != ".ttc") continue;
                    // 埋め込みフォントのURI対応辞書を作成
                    try {
                        Uri uri = new Uri("pack://application:,,,/" + ApplicationInfo.AssemblyName + ";component/" + name);
                        GlyphTypeface gtf = new GlyphTypeface(uri); // 例外が発生するファイルが存在する
                        foreach (string familyName in gtf.FamilyNames.Values) {
                            Debug.WriteLine("embedded fontFamily:" + familyName + " URI:" + uri);
                            AddGlyphTypeInfo(familyName, uri);
                        }
                    }
                    catch (Exception e) {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
