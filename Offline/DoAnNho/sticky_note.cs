using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnLon
{
    internal class sticky_note : TextBox
    {
        private bool isDragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public sticky_note()
        {
            Multiline = true;
            BorderStyle = BorderStyle.None;
              this.BackColor = Color.LightYellow;
            this.BorderStyle = BorderStyle.FixedSingle;
            // Căn giữa theo chiều ngang
            this.TextAlign = HorizontalAlignment.Center;
            this.Width = 150; // Kích thước mặc định
            this.Height = 150;
            this.MouseDown += StickyNote_MouseDown;
            this.MouseMove += StickyNote_MouseMove;
            this.MouseUp += StickyNote_MouseUp;
            this.TextChanged += StickyNote_TextChanged;
        }
        private void StickyNote_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra nếu văn bản vượt quá kích thước của TextBox
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString(Text, Font, Width);
                if (textSize.Height > Height || textSize.Width > Width)
                {
                    // Xóa ký tự cuối cùng nếu vượt quá kích thước
                    Text = Text.Substring(0, Text.Length - 1);
                    SelectionStart = Text.Length;
                }
            }
        }
                private void StickyNote_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        private void StickyNote_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newCursorPoint = Cursor.Position;
                int deltaX = newCursorPoint.X - dragCursorPoint.X;
                int deltaY = newCursorPoint.Y - dragCursorPoint.Y;

                this.Location = new Point(dragFormPoint.X + deltaX, dragFormPoint.Y + deltaY);
            }
        }

        private void StickyNote_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        public string NoteText
        {
            get { return this.Text; }
            set { this.Text = value; }
        }
    }
}

