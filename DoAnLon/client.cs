using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnLon
{
    public partial class client : Form
    {
        private bool isDrawing = false;
        private Point lastPoint;
        private Pen pen;
        private Color currentColor = Color.Black;
        private int penSize = 2;
        private Bitmap drawingBitmap; // Bitmap để vẽ lên
        private Graphics drawingGraphics; // Graphics liên kết với Bitmap
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        private Panel panelDraw;


        public client()
        {
            InitializeComponent();

            // Khởi tạo Panel để vẽ
            panelDraw = new DoubleBufferedPanel();
            panelDraw.Dock = DockStyle.Fill; // Panel sẽ chiếm toàn bộ Form
            this.Controls.Add(panelDraw); // Thêm panelDraw vào Form

            pen = new Pen(currentColor, penSize);

            // Đăng ký sự kiện chuột cho panelDraw (không phải Form)
            panelDraw.MouseDown += panelDraw_MouseDown;
            panelDraw.MouseMove += panelDraw_MouseMove;
            panelDraw.MouseUp += panelDraw_MouseUp;
            panelDraw.Paint += PanelDraw_Paint;
            panelDraw.Resize += panelDraw_Resize;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (panelDraw.Width > 0 && panelDraw.Height > 0)
            {
                drawingBitmap = new Bitmap(panelDraw.Width, panelDraw.Height);
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                drawingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }
        }

        private void btn_Pen_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentColor = colorDialog.Color; // Lưu màu đã chọn
                }
            }

            // Thiết lập bút vẽ với màu và kích thước đã chọn
            pen.Color = currentColor;
            pen.Width = penSize;

            // Invalidate panel để vẽ lại nếu cần
            panelDraw.Invalidate();

        }

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            pen.Color = panelDraw.BackColor;
            pen.Width = 10; // Tăng kích thước của tẩy
        }
        private void panelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Kiểm tra nút chuột
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private void panelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point currentPoint = e.Location;
                drawingGraphics.DrawLine(pen, lastPoint, currentPoint); // Vẽ lên Bitmap của panelDraw
                lastPoint = currentPoint;

                panelDraw.Invalidate(); // Yêu cầu vẽ lại panelDraw
            }
        }

        private void panelDraw_MouseUp(object sender, MouseEventArgs e)     
        {
            isDrawing = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            penSize = trackBar1.Value;
            pen.Width = penSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drawingGraphics.Clear(panelDraw.BackColor); // Xóa toàn bộ Bitmap
            panelDraw.Invalidate(); // Vẽ lại panel
        }
   

  
        public class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.UpdateStyles();
            }
        }
        private void panelDraw_Resize(object sender, EventArgs e)
        {
            if (panelDraw.Width > 0 && panelDraw.Height > 0)
            {
                Bitmap oldBitmap = drawingBitmap;

                drawingBitmap = new Bitmap(panelDraw.Width, panelDraw.Height);
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                drawingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                if (oldBitmap != null)
                {
                    drawingGraphics.DrawImage(oldBitmap, Point.Empty);
                    oldBitmap.Dispose(); // Giải phóng Bitmap cũ
                }

                panelDraw.Invalidate(); // Vẽ lại panel
            }
        }

   

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Tệp PNG|*.png";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                drawingBitmap.Save(saveDialog.FileName);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Tệp Hình Ảnh|*.png;*.jpg;*.bmp";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                drawingBitmap = new Bitmap(openDialog.FileName);
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                panelDraw.Invalidate(); // Vẽ lại panel
            }
        }
        private void SaveState()
        {
            undoStack.Push((Bitmap)drawingBitmap.Clone()); // Lưu trạng thái hiện tại
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                drawingBitmap = undoStack.Pop();
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                panelDraw.Invalidate(); // Vẽ lại panel
            }
        }

            private void PanelDraw_Paint(object sender, PaintEventArgs e)
            {
                if (drawingBitmap != null)
                {
                    // Vẽ Bitmap lên panelDraw
                    e.Graphics.DrawImage(drawingBitmap, Point.Empty);
                }
            }
    }
}
