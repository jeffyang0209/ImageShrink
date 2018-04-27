using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ConsoleApp5
{
    public class ImageShrink:IDisposable
    {
        /// <summary>
        /// 原始圖檔
        /// </summary>
        internal Image _Image { get; set; }

        /// <summary>
        /// 將傳入的byte壓縮
        /// </summary>
        /// <param name="fileContents"></param>
        public ImageShrink(byte[] fileContents)
        {
            _Image = ByteArrayToImage(fileContents);
        }

        /// 將 Image 轉換為 Byte 陣列。
        /// </summary>
        /// <param name="imageFormat">指定影像格式。</param>     
        /// <param name="multiple">縮圖倍數</param>     
        public byte[] ImageToBuffer(ImageFormat imageFormat, int multiple = 0)
        {
            ImgShrink(multiple);
            if (_Image == null) { return null; }
            byte[] data = null;
            using (MemoryStream oMemoryStream = new MemoryStream())
            {
                //建立副本
                using (Bitmap oBitmap = new Bitmap(_Image))
                {
                    //儲存圖片到 MemoryStream 物件，並且指定儲存影像之格式
                    oBitmap.Save(oMemoryStream, imageFormat);
                    //設定資料流位置
                    oMemoryStream.Position = 0;
                    //設定 buffer 長度
                    data = new byte[oMemoryStream.Length];
                    //將資料寫入 buffer
                    oMemoryStream.Read(data, 0, Convert.ToInt32(oMemoryStream.Length));
                    //將所有緩衝區的資料寫入資料流
                    oMemoryStream.Flush();
                }
            }
            return data;
        }

        /// <summary>
        /// 圖片壓縮，並回傳原尺寸及等比例縮圖
        /// </summary>
        /// <param name="multiple">縮圖倍數</param>
        private void ImgShrink(int multiple = 0)
        {
            Image.GetThumbnailImageAbort callBack = new Image.GetThumbnailImageAbort(() => false);
            Bitmap bmpImage = new Bitmap(_Image);

            // 清空圖片 header meta 資料以減少檔案大小 (目前 item.RemovePropertyItem 經測試無法使用，網路上有人發表問微軟也是沒下文，因此只能先用清空的方式刪除)
            foreach (PropertyItem item in bmpImage.PropertyItems)
            {
                PropertyItem newItem = bmpImage.GetPropertyItem(item.Id);
                newItem.Len = 0;
                newItem.Value = null;
                bmpImage.SetPropertyItem(newItem);
            }

            // 設定圖片品質
            ImageCodecInfo codecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);

            // 是否縮圖
            _Image = multiple == 0 ?
                bmpImage : bmpImage.GetThumbnailImage(bmpImage.Width / multiple, bmpImage.Height / multiple, callBack, IntPtr.Zero);
        }


        /// <summary>
        /// 取得圖片品質Code
        /// </summary>
        /// <param name="format">圖片格式</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID.Equals(format.Guid))
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// byte轉成img
        /// </summary>
        /// <param name="byteArrayIn">bytes</param>
        /// <returns>Image</returns>
        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public void Dispose()
        {
            this._Image.Dispose();
            this._Image = null;
        }
    }
}
