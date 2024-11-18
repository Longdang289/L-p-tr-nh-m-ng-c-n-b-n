using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mural_001
{
    public class sticky_note : UserControl
    {
        private bool isDragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public int noteID { get; set; }
        public string NoteText { get; set; }
        public Point LocationPoint { get; set; }
        public Size NoteSize { get; set; }
        public Color BackgroundColor { get; set; }

        public sticky_note()
        {
            noteID = -1;
            NoteText = string.Empty;
            LocationPoint = new Point(0, 0);
            NoteSize = new Size(150, 150);
            BackgroundColor = Color.LightYellow;

            this.Width = NoteSize.Width;
            this.Height = NoteSize.Height;
            this.BackColor = BackgroundColor;
            this.Text = NoteText;
        }

        public void StartDragging(Point cursorPosition)
        {
            isDragging = true;
            dragCursorPoint = cursorPosition;
            dragFormPoint = LocationPoint;
        }

        public void Drag(Point newCursorPosition)
        {
            if (isDragging)
            {
                int deltaX = newCursorPosition.X - dragCursorPoint.X;
                int deltaY = newCursorPosition.Y - dragCursorPoint.Y;

                LocationPoint = new Point(dragFormPoint.X + deltaX, dragFormPoint.Y + deltaY);
            }
        }

        public void StopDragging()
        {
            isDragging = false;
        }

        public void HandleTextChanged()
        {
            // Implement logic for text overflow or other behaviors
        }
    }

}
