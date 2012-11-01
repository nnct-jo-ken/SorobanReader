using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using OpenCvSharp;
using SD = System.Drawing;
using SDI = System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SorobanCaptureLib
{
    /// <summary>
    /// CaptureCameraクラスでcaptureした画像を扱うクラス
    /// .NET(Bitmap), OpenCV(IplImage), XNA(Texture2D)への変換に対応している
    /// </summary>
    public class CaptureImage
    {
        SD.Bitmap data;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">Bitmap形式の撮影画像</param>
        public CaptureImage(SD.Bitmap source)
        {
            data = source;
        }
        
        /// <summary>
        /// Bitmap形式に変換
        /// </summary>
        /// <returns>Bitmap形式の画像</returns>
        public SD.Bitmap ToBitmap()
        {
            return data;
        }

        /// <summary>
        /// OpenCVのIplImageに変換
        /// </summary>
        /// <returns>IplImage形式の画像</returns>
        public IplImage ToIplImage()
        {
            // Bitmapから大きさの情報をとる
            IplImage img = new IplImage(Cv.Size(data.Width, data.Height), BitDepth.U8, 4);
            int[] tmp = new int[data.Width * data.Height];

            unsafe
            {
                SDI.BitmapData bd = data.LockBits(
                  new SD.Rectangle(0, 0, data.Width, data.Height),
                  SDI.ImageLockMode.ReadOnly,
                  SDI.PixelFormat.Format32bppArgb
                  );

                uint* byteData = (uint*)bd.Scan0;

                // Switch bgra -> rgba
                for (int i = 0; i < data.Width * data.Height; i++)
                    byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);

                // copy data
                Marshal.Copy(bd.Scan0, tmp, 0, tmp.Length);
                Marshal.Copy(tmp, 0, img.ImageData, tmp.Length);

                // unlock bitmap
                data.UnlockBits(bd);
            }
            IplImage cvt = img.Clone();
            img.Flip(cvt, FlipMode.XY);

            return cvt;
        }

        /// <summary>
        /// XNAのTexture形式に変換
        /// </summary>
        /// <param name="GraphicsDevice">XNAのGraphicsDevice</param>
        /// <returns>Texture2D形式の画像</returns>
        public Texture2D ToTexture(GraphicsDevice GraphicsDevice)
        {
            return GetTexture(GraphicsDevice, data);
        }

        /// <summary>
        /// Bitmap → Texture形式に変換する
        /// </summary>
        /// <param name="dev">XNAのGraphicsDevice</param>
        /// <param name="bmp">Bitmap形式の元画像</param>
        /// <returns>Texture2D形式の画像</returns>
        private static Texture2D GetTexture(GraphicsDevice dev, SD.Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(dev, bmp.Width, bmp.Height);

            unsafe
            {
                // lock bitmap
                SDI.BitmapData origdata =
                    bmp.LockBits(new SD.Rectangle(0, 0, bmp.Width, bmp.Height), SDI.ImageLockMode.ReadOnly, bmp.PixelFormat);

                uint* byteData = (uint*)origdata.Scan0;

                // copy data
                Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                byteData = null;

                // unlock bitmap
                bmp.UnlockBits(origdata);
            }

            texture.SetData(imgData);

            return texture;
        }
    }
}
