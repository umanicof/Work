using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows.Media;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// MediaPlayerラッパー
    /// ・MediaPlayerはストリーム/バイト列を再生できず、一時ファイルに変換してから再生する必要がある。
    ///   一時ファイルのアクセスの競合や一時ファイルの削除漏れを回避するため、ラッパーを作成した。
    /// </summary>
    public class MediaPlayerWrapper : IDisposable
    {
        string sourcePath_;
        MediaPlayer Player { get; set; } = new MediaPlayer();

        /// <summary>
        /// プレイヤー状態
        /// </summary>
        public enum StatusType
        {
            Close,
            Open,
            Play,
            Pause,
        }
        public StatusType Status { get; private set; } = StatusType.Close;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MediaPlayerWrapper()
        {
            Player.MediaEnded += Player_MediaEnded; // 再生終了イベント

            string tmpPath = System.IO.Path.GetTempFileName();
            sourcePath_ = tmpPath.Replace(".tmp", ".wav"); // MediaPlayerで使用可能な拡張子にする
            File.Move(tmpPath, sourcePath_); // リネーム

            DebugLog.WriteLine("SoundFile:" + sourcePath_);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                Player.Close();
                Status = StatusType.Close;
            }

            if (sourcePath_ == null)
                return;
            // ごくわずかだが例外が発生する場合があったので非同期に
            var t = FileIOUtil.DeleteFileSafeAsync(sourcePath_); // 待機しない
            sourcePath_ = null;
        }

        ~MediaPlayerWrapper()
        {
            Dispose(false);
        }

        /// <summary>
        /// ファイルオープン
        /// </summary>
        public void Open(string base64)
        {
            Base64Util.Base64ToFile(sourcePath_, base64);
            Player.Open(new Uri(sourcePath_));
            Status = StatusType.Open;
        }
        
        /// <summary>
        /// 再生
        /// </summary>
        public void Play()
        {
            Debug.Assert(Status != StatusType.Close);
            Player.Play();
            Status = StatusType.Play;
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        public void Pause()
        {
            Debug.Assert(Status != StatusType.Close);
            Player.Pause();
            Status = StatusType.Pause;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Player.Stop();
            Dispose();
        }

        /// <summary>
        /// 再生終了イベント
        /// ・内部エラーが発生し、このイベントが発生しない場合がある
        /// </summary>
        void Player_MediaEnded(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
