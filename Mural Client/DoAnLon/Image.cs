using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
//using System.Reflection.Metadata;
using System.IO;
namespace DoAnLon
{
    public class Image
    {
        public int ImageID { get; set; } // Mã duy nhất của hình ảnh
        public Point Position { get; set; } // Vị trí của ảnh trên panel
        public Size ImageSize { get; set; } // Kích thước ảnh

        public string content { get; set; }


        // Phương thức khởi tạo
        public Image(int imageID, Point position, Size imageSize, System.Drawing.Image bitmap = null)
        {
            ImageID = imageID;
            Position = position;
            ImageSize = imageSize;

            // Mã hóa bitmap thành chuỗi Base64 nếu có
            if (bitmap != null)
            {
                content = EncodeBitmapToBase64(bitmap);
            }
        }
        // Mã hóa bitmap thành Base64
        public static string EncodeBitmapToBase64(System.Drawing.Image bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Lưu dưới dạng PNG
                byte[] bitmapBytes = ms.ToArray();
                return Convert.ToBase64String(bitmapBytes);
            }
        }
        // Giải mã Base64 thành bitmap
        public static System.Drawing.Image DecodeBase64ToBitmap(string base64)
        {
            byte[] bitmapBytes = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(bitmapBytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }
        // Hàm cập nhật vị trí của ảnh
        public void UpdatePosition(Point newPosition)
        {
            Position = newPosition;
        }

        // Hàm cập nhật kích thước của ảnh
        public void UpdateSize(Size newSize)
        {
            ImageSize = newSize;
        }


    }

}
