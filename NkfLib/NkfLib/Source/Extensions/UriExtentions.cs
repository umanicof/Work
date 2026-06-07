using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    public static partial class UriExtentions
    {
        /// <summary>
        /// ファイルのURI => ファイルパス 変換
        /// </summary>
        public static string ToFilePath(this Uri self)
        {
            return self.LocalPath + Uri.UnescapeDataString(self.Fragment);
        }

        /// <summary>
        /// URI情報のデバッグ表示
        /// 出典：https://dobon.net/vb/dotnet/internet/analyzeurl.html
        /// </summary>
        /// <param name="uri"></param>
        public static void DebugDetail(this Uri self)
        {
            //絶対パス 
            Debug.WriteLine("AbsolutePath:" + self.AbsolutePath);
            //結果: /vb/bbs.cgi

            //絶対URI
            Debug.WriteLine("AbsoluteUri:" + self.AbsoluteUri);
            //結果: http://user:pass@www.dobon.net/vb/bbs.cgi?id=a%20b&n=1#top

            //サーバーのDNS(Domain Name System)ホスト名またはIPアドレスと、ポート番号
            Debug.WriteLine("Authority:" + self.Authority);
            //結果: www.dobon.net

            //DNSの解決に安全に使用できるエスケープ解除されたホスト名
            Debug.WriteLine("DnsSafeHost:" + self.DnsSafeHost);
            //結果: www.dobon.net

            //エスケープフラグメント
            Debug.WriteLine("Fragment:" + self.Fragment);
            //結果: #top

            //サーバーのDNSホスト名またはIPアドレス
            Debug.WriteLine("Host:" + self.Host);
            //結果: www.dobon.net

            //ホスト名の型
            switch (self.HostNameType) {
            case UriHostNameType.Basic:
                Debug.WriteLine(
                    "ホストは設定されましたが、型を決定できません。");
                break;
            case UriHostNameType.Dns:
                Debug.WriteLine(
                    "ホスト名は、ドメイン名システム形式のホスト名です。");
                break;
            case UriHostNameType.IPv4:
                Debug.WriteLine(
                    "ホスト名は、IP Version 4 形式のホストアドレスです。");
                break;
            case UriHostNameType.IPv6:
                Debug.WriteLine(
                    "ホスト名は、IP Version 6 形式のホスト アドレスです。");
                break;
            case UriHostNameType.Unknown:
                Debug.WriteLine("ホスト名の型が指定されていません。");
                break;
            }
            //結果: ホスト名は、ドメイン名システム形式のホスト名です。

            //絶対インスタンスであるかどうか
            Debug.WriteLine("IsAbsoluteUri:" + self.IsAbsoluteUri);
            //結果: True

            //ポート値がこのスキームの既定のポート値かどうか
            Debug.WriteLine("IsDefaultPort:" + self.IsDefaultPort);
            //結果: True

            //ファイルURIかどうか
            Debug.WriteLine("IsFile:" + self.IsFile);
            //結果: False

            //ローカルホストを参照するかどうか
            Debug.WriteLine("IsLoopback:" + self.IsLoopback);
            //結果: False

            //UNCパスかどうか
            Debug.WriteLine("IsUnc:" + self.IsUnc);
            //結果: False

            //Uriコンストラクタに渡された元のURI文字列
            Debug.WriteLine("OriginalString:" + self.OriginalString);
            //結果: http://user:pass@www.dobon.net:80/vb/bbs.cgi?id=a%20b&n=1#top

            //ローカルオペレーティングシステムでのファイル名表現
            Debug.WriteLine("LocalPath:" + self.LocalPath);
            //結果: /vb/bbs.cgi

            //AbsolutePathプロパティとQueryプロパティを疑問符(?)で区切った形式
            Debug.WriteLine("PathAndQuery:" + self.PathAndQuery);
            //結果: /vb/bbs.cgi?id=a%20b&n=1

            //ポート番号
            Debug.WriteLine("Port:" + self.Port);
            //結果: 80

            //クエリ情報
            Debug.WriteLine("Query:" + self.Query);
            //結果: ?id=a%20b&n=1

            //スキーム名
            Debug.WriteLine("Scheme:" + self.Scheme);
            //結果: http

            //セグメント
            foreach (string s in self.Segments)
                Debug.WriteLine("Segment:" + "\t" + s);
            //結果: 
            //    /
            //    vb/
            //    bbs.cgi

            //Uriインスタンスの作成前に、URI文字列がエスケープされているか
            Debug.WriteLine("UserEscaped:" + self.UserEscaped);
            //結果: False

            //ユーザー名、パスワードなどのユーザー固有の情報
            Debug.WriteLine("UserInfo:" + self.UserInfo);
            //結果: user:pass

            //左端からスキームまで
            Debug.WriteLine("Scheme:" + self.GetLeftPart(UriPartial.Scheme));
            //結果: http://

            //左端から権限まで
            Debug.WriteLine("Authority:" + self.GetLeftPart(UriPartial.Authority));
            //結果: http://user:pass@www.dobon.net

            //左端からパスまで
            Debug.WriteLine("Path:" + self.GetLeftPart(UriPartial.Path));
            //結果: http://user:pass@www.dobon.net/vb/bbs.cgi

            //左端からクエリまで
            Debug.WriteLine("Query:" + self.GetLeftPart(UriPartial.Query));
            //結果: http://user:pass@www.dobon.net/vb/bbs.cgi?id=a%20b&n=1

            //エスケープ解除された正規形式のURI 
            Debug.WriteLine("ToString:" + self.ToString());
            //結果: http://user:pass@www.dobon.net/vb/bbs.cgi?id=a b&n=1#top
        }
    }
}
