using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace NkfLib
{
    /// <summary>
    /// Base64ユーティリティ
    /// </summary>
    public static partial class Base64Util
    {
        /// <summary>
        /// Base64 => System.Drawing.Image 変換
        /// </summary>
        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        /// <summary>
        /// System.Drawing.Image => Base64 変換
        /// </summary>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream()) {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to base 64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}
