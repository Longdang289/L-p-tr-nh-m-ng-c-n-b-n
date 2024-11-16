using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnLon
{
    public class Drawing
    {
        public List<Point> Points { get; set; } // Danh sách các điểm vẽ trong nét
        public int PenColor { get; set; } // Mã màu ARGB của bút
        public float PenWidth { get; set; } // Độ dày của bút

        public Drawing()
        {
            Points = new List<Point>();
        }
    }
}
