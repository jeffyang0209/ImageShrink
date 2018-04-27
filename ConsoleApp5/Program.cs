using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            GO(@"C:\Users\Jeff\Desktop\Games");
        }

        public static void GO(string path)
        {
            var files = new DirectoryInfo(path).GetFiles();
            // 處理所有檔案
            foreach (var file in files)
            {
                if (file.Extension == ".jpg" && file.FullName.IndexOf("_xs") == -1)
                {
                    if (file.Name[0] != '.')
                    {
                        var res = ToBinaryByFileStream(file.FullName, 10);

                        File.WriteAllBytes(file.FullName.Insert(file.FullName.Length - 4, "_xs"), res);

                        //res = ToBinaryByFileStream(file.FullName, 1);

                        //File.WriteAllBytes(file.FullName, res);
                    }
                }
                // File.Delete(file.FullName);
            }

            var folder = new DirectoryInfo(path).GetDirectories();

            foreach (var f in folder)
            {
                GO(f.FullName);
            }
        }

        private static byte[] ToBinaryByFileStream(string imageFile, int size)
        {
            using (FileStream fs = File.OpenRead(imageFile))
            {
                Bitmap bmp1 = (Bitmap)Image.FromStream(fs);
                byte[] result;

                var b = ImageToBuffer(bmp1, ImageFormat.Jpeg);
                using (var imgShrink = new ImageShrink(b))
                {
                    result = imgShrink.ImageToBuffer(ImageFormat.Jpeg, size);
                }
                return result;
            }
        }

        public static byte[] ImageToBuffer(Image _Image, ImageFormat imageFormat, int multiple = 0)
        {
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
    }
}
