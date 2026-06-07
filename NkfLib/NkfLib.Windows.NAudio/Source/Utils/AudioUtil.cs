using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

using NAudio.Wave;
using NAudio.MediaFoundation;

namespace NkfLib
{
    /// <summary>
    /// Audioユーティリティ
    /// </summary>
    public static partial class AudioUtil
    {
        /// <summary>
        /// サポートするMediaTypeのリストの取得
        /// </summary>
        /// <param name="formatType"></param>
        /// <param name="sampleRate"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        private static List<MediaType> GetSupportMediaTypes(Guid formatType, int sampleRate = 0, int channels = 0)
        {
            List<MediaType> mediaTypes = MediaFoundationEncoder.GetOutputMediaTypes(formatType).ToList();
            if (sampleRate != 0) {
                mediaTypes = mediaTypes.Where(m => m.SampleRate == sampleRate).ToList();
            }
            if (channels != 0) {
                mediaTypes = mediaTypes.Where(m => m.ChannelCount == channels).ToList();
            }
            return mediaTypes;
        }

        /// <summary>
        /// オーディオコンバート（WAV, MP3, AAC, M4A  => AAC）
        /// </summary>
        public static string ConvertAAC(string base64)
        {
            string tmpPath = System.IO.Path.GetTempFileName();
            string srcPath = tmpPath.Replace(".tmp", ""); // 使用可能な拡張子にする（指定しなければ自動判別っぽい）
            File.Move(tmpPath, srcPath); // リネーム

            tmpPath = System.IO.Path.GetTempFileName();
            string dstPath = tmpPath.Replace(".tmp", ".mp4"); // 使用可能な拡張子にする（指定しないとエラーが発生）
            File.Move(tmpPath, dstPath); // リネーム

            Base64Util.Base64ToFile(srcPath, base64);

            try {
                using (MediaFoundationReader reader = new MediaFoundationReader(srcPath)) {
                    //MediaFoundationEncoder.EncodeToAac(reader, dstPath, 44100);
                    var mediaType = MediaFoundationEncoder.SelectMediaType(
                                        AudioSubtypes.MFAudioFormat_AAC,
                                        new WaveFormat(44100, 1), // @note 何故かステレオで出力すると元データの聞こえ方と変わってしまう
                                        0);
                    //var mediaType = GetSupportMediaTypes(AudioSubtypes.MFAudioFormat_AAC, reader.WaveFormat.SampleRate, reader.WaveFormat.Channels)[0];
                    //log.WriteLine("SampleRate:" + mediaType.SampleRate + " Channels:" + mediaType.ChannelCount + " AverageBytesPerSecond:" + mediaType.AverageBytesPerSecond);
                    using (MediaFoundationEncoder encoder = new MediaFoundationEncoder(mediaType)) {
                        encoder.Encode(dstPath, reader);
                    }
                }

                base64 = Base64Util.FileToBase64(dstPath);
            }
            catch {
                base64 = "";
            }

            var t = FileIOUtil.DeleteFileSafeAsync(srcPath); // 待機しない
            t = FileIOUtil.DeleteFileSafeAsync(dstPath); // 待機しない

            return base64;
        }
    }
}
