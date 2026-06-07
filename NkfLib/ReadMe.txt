■NkfLibについて
　各ソリューションで使用する共有ライブラリをまとめたもの。
　
■構成
・共有プロジェクトはアンダーバー無し（NkfLibなど）。クラスライブラリはアンダーバー有り（_NkfLib）。
・クラスライブラリは主にビルドテスト用
・共有プロジェクト
　namespaceは NkfLib に統一。
　各共有プロジェクトで同名のpartialなクラスが存在する。
　NkfLib ...................... 共通ライブラリ。全てのライブラリはこれが必要。（.NET Standard2.1）
　NkfLib.DeepL ................ DeepL用ライブあり（.NET Standard2.1）
　NkfLib.Unity ................ Unity用ライブラリ（.NET Standard2.1）
　NkfLib.Core ................. .NET(旧.NET Core)用ライブラリ
　NkfLib.Windows .............. Windows（7以降）用（主にUI、WPF絡み）のライブラリ。
　　　　　　　　　　　　　　　　プロジェクト設定：UseWindowsForms, UseWPF
　NkfLib.Windows10 ............ Windows10用のライブラリ。Windows10以降の機能が必要なもののみ。
　　　　　　　　　　　　　　　　プロジェクト設定：UseWindowsForms, UseWPF
　NkfLib.Windows.FontLoader ... Windows用ライブラリのグリフ情報読み込み。縦書き対応などに使用
　　　　　　　　　　　　　　　　パッケージ：WaterTrans.TypeLoader
　NkfLib.Windows.NAudio ....... NAudio関連のライブラリ。
　　　　　　　　　　　　　　　　パッケージ：NAudio
・Unityで使用するために.NET Standard2.1を最下層のライブラリ（NkfLib）とする。手動でC#9.0を有効にしている
　=> 最新ではC#10.0まで行ける？
・Newtonsoft.Jsonを使用するライブラリは分けた方が良さげ
　.NET Standard 2.1だとMicrosoft.CSharpとSystem.Dynamic.Runtimeの読み込みが必須になってしまう
　=> と思ったがNkfLib内に使用するクラスがあり分離が面倒

■使用方法
共有プロジェクトを各ソリューションに読み込み、参照する。
・Windowsソリューション
　必要な共有プロジェクトを参照する（はず）
・Unityプロジェクト
　(1) UniTask、UniRxを追加
　　　=> Git URLから追加しようとしたが、manifestファイルが見つからないというエラーが出て取得できなかった（UniTaskにはあるがエラー）
　　　   仕方ないのでReleaseビルドのunitypackageを使用
　(2) Unityの環境設定でNugetの取得URLなど設定する（Json.NETをNuGetから取得している影響）
　　　参考URL：https://qiita.com/akiojin/items/ac05392d97abb8797dcd
　　　　　　   https://www.nowsprinting.com/entry/2023/12/21/024620
　　　直近の取得URL：
　　　　URL  : https://package.openupm.com
　　　　Scope: org.nuget
　　　　※サイトが閉鎖されるなどで変わってしまう
      ⇒ Json.NETはUnityのPackageManagerで「Install package by name..」から「com.unity.nuget.newtonsoft-json」
      　 を指定してインストールするのが良い。最新のJson.NETはUnityに合わないのでUnity用のものが用意されている模様。
　(3) Json.NETをNugetからインポート
　(4) 「パッケージマネージャー」タブの左上の「＋」から「ディスクからパッケージを加える」を選択
　    NkfLib、NkfLib.Unity 共有プロジェクトを追加する
　参考：https://neue.cc/2024/01/15_shareprojectinunity.html
　・参考URLではクラスライブラリを参照する話になっているが、NkfLibは共有プロジェクトを参照する
　　・参考URLのやり方ではクラスライブラリフォルダにソースを入れる必要がありそうだったため。構造の都合。
　・参考URLを参考にプロジェクトに設定をいくつかしている。
　　・package.json、.asmdefファイルを追加
　　・packageが他のpackageを使用する場合には依存関係を記述する必要がある。
　　　packageが存在しないAssets下のクラスは基本的に読み取れない模様（いくつか回避方法はあるようだが）
　　・Directory.Build.propsファイルは追加していない（bin、objは作られないので問題ない）
　　・Unity用のクラスライブラリ（_NkfLib.Unity）を用意しているがあくまで参照用。ビルドはできない。

■.NET についてメモ
　.NET Standard : 対応範囲が一番広い。Unityなどはこれ。ランタイムではなくAPIの共通仕様。.NET, .NET Core、.NET FrameWork がその実装
　　　　　　　　　※Unityは基本的に .NET Standard であり起源はMono。.NET Coreではない（互換性がない）
　.NET Framework: .NETの古い実装。歴史的には.NET Standardより古く、これから.NET Standardの仕様がまとめられたという経緯。Windows専用。
　　　　　　　　　主にWindows環境など対応デバイスが制限される。
　.NET Core     : .NET Frameworkの後継、,NET Core 3 で終了。
　.NET          : .NET Coreの進化版。,NET Core と .NET Framework を統合。

