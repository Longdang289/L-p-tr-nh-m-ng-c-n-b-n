using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoAnLon
{
    public class sticky_note
    {
        public int noteID { get; set; } // ID duy nhất cho mỗi ghi chú
        public string NoteText { get; set; } // Nội dung ghi chú
        public Point LocationPoint { get; set; } // Tọa độ của ghi chú
        public Size NoteSize { get; set; } // Kích thước ghi chú
        public Color BackgroundColor { get; set; } // Màu nền ghi chú

        // Constructor mặc định
        public sticky_note()
        {
            noteID = -1;
            NoteText = string.Empty;
            LocationPoint = new Point(0, 0);
            NoteSize = new Size(150, 150);
            BackgroundColor = Color.LightYellow;
        }

        // Hàm cập nhật vị trí
        public void UpdateLocation(Point newLocation)
        {
            LocationPoint = newLocation;
        }

        // Hàm cập nhật kích thước
        public void UpdateSize(Size newSize)
        {
            NoteSize = newSize;
        }

        // Hàm cập nhật nội dung text
        public void UpdateText(string newText)
        {
            NoteText = newText;
        }

        // Hàm cập nhật màu nền
        public void UpdateBackgroundColor(Color newColor)
        {
            BackgroundColor = newColor;
        }
    }


}
