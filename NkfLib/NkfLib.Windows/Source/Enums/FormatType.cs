using System;
using System.Collections.Generic;
using System.Text;

namespace NkfLib
{
    public enum FormatType
    {
        None,
        Auto, // 自動判別
        Text,
        Jpeg,
        Png,
        Gif,
        Tiff,
        Wav,
        Mp3,
        Aac,
        M4a,
    }

    public static class FormatTypeExtentions
    { 

        /// <summary>
        /// 画像判定
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static bool IsImage(this FormatType self)
        {
            return self == FormatType.Jpeg ||
                   self == FormatType.Png ||
                   self == FormatType.Gif ||
                   self == FormatType.Tiff;
        }

        /// <summary>
        /// 音声判定
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static bool IsAudio(this FormatType self)
        { 
            return self == FormatType.Wav ||
                   self == FormatType.Mp3 ||
                   self == FormatType.Aac ||
                   self == FormatType.M4a;
        }
    }
}
