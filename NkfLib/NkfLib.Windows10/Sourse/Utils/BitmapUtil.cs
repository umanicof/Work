using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using GraphicsBitmapDecoder = Windows.Graphics.Imaging.BitmapDecoder;
using MediaBitmapFrame = System.Windows.Media.Imaging.BitmapFrame;

namespace NkfLib
{
    /// <summary>
    /// ビットマップ関連ユーティリティ
    /// ・種類
    ///   SoftwareBitmap ... UWPのBitmap。Dispose不要？
    /// </summary>
    public static partial class BitmapUtil
    {
        /// <summary>
        /// byte型の配列 => SoftwareBitmap
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static async Task<SoftwareBitmap> ToSoftwareBitmapAsync(byte[] byteArray)
        {
            // @note: 洗練されてなさそう
            using (var randomAccessStream = new InMemoryRandomAccessStream())
            using (var outputStream = randomAccessStream.GetOutputStreamAt(0))
            using (var dataWriter = new DataWriter(outputStream))
            {
                dataWriter.WriteBytes(byteArray);
                await dataWriter.StoreAsync();
                if (!await outputStream.FlushAsync()) 
                    throw new InvalidOperationException();

                var decoder = await GraphicsBitmapDecoder.CreateAsync(randomAccessStream);

                return await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }
        }
        
        /// <summary>
        /// Bitmap => SoftwareBitmap
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static async Task<SoftwareBitmap> ToSoftwareBitmapAsync(Bitmap bitmap,
                                                                      bool disposeBitmap = false, 
                                                                      BitmapPixelFormat bitmapPixelFormat = BitmapPixelFormat.Rgba16,
                                                                      BitmapAlphaMode bitmapAlphaMode = BitmapAlphaMode.Premultiplied)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                // Pngはやや重い。Bmp, Jpegは同じくらい。
                bitmap.Save(stream.AsStream(), ImageFormat.Bmp);
                GraphicsBitmapDecoder decoder = await GraphicsBitmapDecoder.CreateAsync(stream);
                if (disposeBitmap) {
                    bitmap.Dispose();
                }
                return await decoder.GetSoftwareBitmapAsync(bitmapPixelFormat, bitmapAlphaMode);
            }
        }

        /// <summary>
        /// BitmapSource => SoftwareBitmap
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static async Task<SoftwareBitmap> ToSoftwareBitmapAsync(BitmapSource bitmapSource,
                                                                      BitmapPixelFormat bitmapPixelFormat = BitmapPixelFormat.Rgba16,
                                                                      BitmapAlphaMode bitmapAlphaMode = BitmapAlphaMode.Premultiplied)
        {
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(MediaBitmapFrame.Create(bitmapSource));
            using (var stream = new InMemoryRandomAccessStream())
            {
                encoder.Save(stream.AsStream());
                GraphicsBitmapDecoder decoder = await GraphicsBitmapDecoder.CreateAsync(stream);
                return await decoder.GetSoftwareBitmapAsync(bitmapPixelFormat, bitmapAlphaMode);
            }
        }
    }
}
