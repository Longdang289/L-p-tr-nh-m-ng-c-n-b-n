using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace DoAnLon
{
    public partial class client : Form
    {
        private bool isDrawing = false;
        private Point lastPoint;
        private Pen pen;
        private Color currentColor = Color.Black;
        private int penSize = 2;
        private bool isInDrawingMode = false;
        private Bitmap drawingBitmap; // Bitmap để vẽ lên
        private Graphics drawingGraphics; // Graphics liên kết với Bitmap
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        private Stack<Bitmap> redoStack = new Stack<Bitmap>(); // Stack để lưu các trạng thái Redo

        private List<PictureBox> pictureBoxes = new List<PictureBox>(); // Danh sách chứa các PictureBox
        private bool isResizing = false; // Kiểm tra có đang thay đổi kích thước hay không
        private Point initialMousePos; // Vị trí chuột ban đầu
        private Size initialSize; // Kích thước ban đầu của ảnh
        private Point initialPicturePos; // Vị trí ban đầu của ảnh
        private bool isDragging = false;
        private Point dragCursorPoint; // Điểm chuột khi bắt đầu kéo
        private Point dragFormPoint; // Điểm form khi bắt đầu kéo

        private Dictionary<PictureBox, Size> originalSizes = new Dictionary<PictureBox, Size>();
        private float zoomFactor = 1.0f; // Tỷ lệ phóng to/thu nhỏ hiện tại
        private const float zoomIncrement = 0.1f; // Số lượng phóng to/thu nhỏ mỗi lần cuộn chuột
        private int offsetX, offsetY;
        private enum DrawShape { Line, Rectangle, Circle }
        private DrawShape currentShape = DrawShape.Line;

        public client()
        {
            InitializeComponent();
            pen = new Pen(currentColor, penSize);
            EnableDoubleBuffered(PanelDraw);

            PanelDraw.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_eraser.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Pen.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnReundo.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.MinimumSize = new Size(300, 300);



            // Đăng ký sự kiện chuột cho panelDraw
            PanelDraw.MouseDown += PanelDraw_MouseDown;
            PanelDraw.MouseMove += PanelDraw_MouseMove;
            PanelDraw.MouseUp += PanelDraw_MouseUp;
            PanelDraw.Paint += PanelDraw_Paint;
            PanelDraw.Resize += PanelDraw_Resize;
     
            // Đăng ký sự kiện cho panel bạn muốn di chuyển
            PanelDraw.MouseDown += new MouseEventHandler(panelMovable_MouseDown);
            PanelDraw.MouseMove += new MouseEventHandler(panelMovable_MouseMove);
            PanelDraw.MouseUp += new MouseEventHandler(panelMovable_MouseUp);

            this.KeyPreview = true; // Bật sự kiện KeyDown cho form
            this.KeyDown += new KeyEventHandler(Form_KeyDown); // Đăng ký sự kiện KeyDown

            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnLoad.BringToFront();
            btnUndo.BringToFront();
            btnReundo.BringToFront();
            btnSave.BringToFront();
            btn_Pen.BringToFront();
            btn_eraser.BringToFront();
            btnClear.BringToFront();
            this.WindowState = FormWindowState.Maximized;
            if (PanelDraw.Width > 0 && PanelDraw.Height > 0)
            {
                drawingBitmap = new Bitmap(PanelDraw.Width, PanelDraw.Height);
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                drawingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // Điều chỉnh tỷ lệ phóng to/thu nhỏ
                PanelDraw.Scale(new SizeF(zoomFactor, zoomFactor));
            }

        }

        private void btn_Pen_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentColor = colorDialog.Color; // Lưu màu đã chọn
                    pen.Color = currentColor; // Thiết lập bút vẽ với màu đã chọn
                    pen.Width = penSize;
                    tbSize.Text = penSize.ToString();
                    // Chuyển sang chế độ vẽ
                    isInDrawingMode = true;
                    PanelDraw.Cursor = Cursors.Cross; // Thay đổi con trỏ chuột để nhận biết chế độ vẽ

                    PanelDraw.Invalidate(); // Yêu cầu vẽ lại panelDraw
                }
            }
        }

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            pen.Color = PanelDraw.BackColor;
            pen.Width = penSize;
            redoStack.Clear(); // Xóa redoStack khi người dùng vẽ mới
        }

        private void PanelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (isInDrawingMode && e.Button == MouseButtons.Left) // Kiểm tra chế độ vẽ
            {
                SaveState();
                redoStack.Clear();
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private void PanelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point currentPoint = e.Location;
                drawingGraphics.DrawLine(pen, lastPoint, currentPoint); // Vẽ lên Bitmap của panelDraw
                lastPoint = currentPoint;

                PanelDraw.Invalidate(); // Yêu cầu vẽ lại panelDraw
            }
        }

        private void PanelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (isInDrawingMode) // Chỉ tắt chế độ vẽ khi không vẽ
            {
                isDrawing = false;
            }
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

        private void PanelDraw_Resize(object sender, EventArgs e)
        {
            if (drawingBitmap != null)
            {
                // Tạo Bitmap mới với kích thước mới của Panel
                Bitmap oldBitmap = drawingBitmap;

                // Chỉ tạo mới Bitmap nếu kích thước khác
                drawingBitmap = new Bitmap(PanelDraw.Width, PanelDraw.Height);

                // Giải phóng đối tượng Graphics cũ nếu tồn tại
                drawingGraphics?.Dispose();

                drawingGraphics = Graphics.FromImage(drawingBitmap);
                drawingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Vẽ lại nội dung của Bitmap cũ vào Bitmap mới
                drawingGraphics.DrawImage(oldBitmap, Point.Empty);

                // Giải phóng Bitmap cũ
                oldBitmap.Dispose();
            }

            // Yêu cầu vẽ lại panel
            PanelDraw.Invalidate();
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
            Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
            Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
            int visibleX = formOriginInScreen.X - panelOriginInForm.X;
            int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Tệp Hình Ảnh|*.png;*.jpg;*.bmp";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                Image loadedImage = new Bitmap(openDialog.FileName);

                // Tạo PictureBox với hình ảnh đã tải
                PictureBox pictureBox = new PictureBox
                {
                    Image = loadedImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = Math.Min(200, PanelDraw.Width), // Giới hạn chiều rộng theo chiều rộng của panel
                    Height = Math.Min(200, PanelDraw.Height), // Giới hạn chiều cao theo chiều cao của panel
                  Location = new Point(visibleX, visibleY),
                };

                // Đảm bảo PictureBox nằm trong giới hạn của Panel
                if (pictureBox.Right > PanelDraw.Width)
                {
                    pictureBox.Left = PanelDraw.Width - pictureBox.Width; // Căn phải
                }

                if (pictureBox.Bottom > PanelDraw.Height)
                {
                    pictureBox.Top = PanelDraw.Height - pictureBox.Height; // Căn dưới
                }

                // Đăng ký sự kiện chuột cho PictureBox
                pictureBox.MouseDown += PictureBox_MouseDown;
                pictureBox.MouseMove += PictureBox_MouseMovePictureBox;
                pictureBox.MouseUp += PictureBox_MouseUpPictureBox;
                pictureBox.MouseWheel += PictureBox_MouseWheel;

                // Thêm PictureBox vào panel, không phải vào form
                PanelDraw.Controls.Add(pictureBox);
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
                redoStack.Push((Bitmap)drawingBitmap.Clone()); // Lưu trạng thái hiện tại vào redoStack trước khi Undo
                drawingBitmap = undoStack.Pop();
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                PanelDraw.Invalidate(); // Vẽ lại panel
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

        private void btnReundo_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push((Bitmap)drawingBitmap.Clone()); // Lưu trạng thái hiện tại vào undoStack trước khi Redo
                drawingBitmap = redoStack.Pop(); // Khôi phục trạng thái từ redoStack
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                PanelDraw.Invalidate(); // Vẽ lại panel
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offsetX = e.X; // Lưu vị trí chuột khi bắt đầu di chuyển
                offsetY = e.Y;
            }
        }

        private void PictureBox_MouseMovePictureBox(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox pictureBox = (PictureBox)sender;

                // Tính toán vị trí mới của PictureBox
                int newX = pictureBox.Left + (e.X - offsetX);
                int newY = pictureBox.Top + (e.Y - offsetY);

                // Đảm bảo PictureBox nằm trong panelDraw
                if (newX < 0) newX = 0;
                if (newY < 0) newY = 0;
                if (newX + pictureBox.Width > PanelDraw.Width) newX = PanelDraw.Width - pictureBox.Width;
                if (newY + pictureBox.Height > PanelDraw.Height) newY = PanelDraw.Height - pictureBox.Height;

                pictureBox.Left = newX;
                pictureBox.Top = newY;
            }
        }

        private void PictureBox_MouseUpPictureBox(object sender, MouseEventArgs e)
        {
            // Reset lại biến sau khi thả chuột
            isResizing = false;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;

            // Phóng to hoặc thu nhỏ PictureBox khi cuộn chuột
            int zoomAmount = 30; // Tùy chỉnh mức phóng to/thu nhỏ
            int newWidth = pictureBox.Width + (e.Delta > 0 ? zoomAmount : -zoomAmount);
            int newHeight = pictureBox.Height + (e.Delta > 0 ? zoomAmount : -zoomAmount);

            // Đảm bảo kích thước PictureBox không vượt quá giới hạn của PanelDraw
            if (newWidth > PanelDraw.Width) newWidth = PanelDraw.Width;
            if (newHeight > PanelDraw.Height) newHeight = PanelDraw.Height;

            // Cập nhật kích thước PictureBox
            pictureBox.Size = new Size(newWidth, newHeight);
        }
        private void panelMovable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isDragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = PanelDraw.Location;
            }
        }

        private void panelMovable_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Tính toán vị trí mới của panel
                Point newCursorPoint = Cursor.Position;
                int dx = newCursorPoint.X - dragCursorPoint.X; // Biến đổi vị trí chuột
                int dy = newCursorPoint.Y - dragCursorPoint.Y;

                PanelDraw.Location = new Point(dragFormPoint.X + dx, dragFormPoint.Y + dy);

            }
        }

        private void panelMovable_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false; // Kết thúc kéo
        }

 

        private void EnableDoubleBuffered(Panel panel)
        {
            // Lấy đối tượng Type của Panel
            Type panelType = panel.GetType();

            // Lấy thuộc tính "DoubleBuffered" qua phản chiếu
            PropertyInfo doubleBufferedProperty = panelType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

            // Thiết lập giá trị thành true
            if (doubleBufferedProperty != null)
            {
                doubleBufferedProperty.SetValue(panel, true, null);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            drawingGraphics.Clear(PanelDraw.BackColor); // Xóa toàn bộ Bitmap
            PanelDraw.Invalidate(); // Vẽ lại panel
            redoStack.Clear(); // Xóa redoStack khi người dùng vẽ mới
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) // Kiểm tra xem phím nhấn có phải là Esc không
            {
                Application.Exit(); // Thoát chương trình
            }
        }
       
        

       
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Giả sử panelDraw là panel lớn hơn màn hình
            int panelWidth = PanelDraw.Width;
            int panelHeight = PanelDraw.Height;

            // Tính vị trí bắt đầu để phần chính giữa panelDraw nằm ở giữa màn hình
            int centerX = (panelWidth - this.ClientSize.Width) / 2;
            int centerY = (panelHeight - this.ClientSize.Height) / 2;

            // Di chuyển panelDraw để phần visible nằm chính giữa
            PanelDraw.Location = new Point(-centerX, -centerY);
        }

        private void btnStickyNote_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
                    Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
                    int visibleX = formOriginInScreen.X - panelOriginInForm.X;
                    int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;

                    // Tạo sticky note mới và đặt màu nền
                    sticky_note newStickyNote = new sticky_note();
                    newStickyNote.BackColor = colorDialog.Color;
                    newStickyNote.Location = new Point(visibleX, visibleY);
                    this.PanelDraw.Controls.Add(newStickyNote);
                    newStickyNote.BringToFront(); // Đảm bảo nó hiển thị trên các control khác
                }
            }
        }

        private void btnUpdataSize_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(tbSize.Text, out int newSize))
                {
                    if (newSize >= 1 && newSize <= 72)
                    {
                        penSize = newSize;
                        if (pen != null && pen.Color != PanelDraw.BackColor) // Nếu bút không phải màu nền (tức là đang vẽ)
                        {
                            pen.Width = penSize; // Cập nhật kích thước bút ngay lập tức
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng nhập giá trị từ 1 đến 72", "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập một số hợp lệ", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Opps: {ex.Message}", "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetPosition_Click(object sender, EventArgs e)
        {
            // Lấy tọa độ hiện tại của góc trên bên trái của form
            Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
            Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
            int visibleX = formOriginInScreen.X - panelOriginInForm.X;
            int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
            // Đặt văn bản cho label với tọa độ của form
            lbPosition.Text = $"{visibleX}, {visibleY}";
        }

        private void btnTpToPosition_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem lbPosition có chứa dữ liệu hay không
            if (!string.IsNullOrEmpty(lbPosition.Text))
            {
                // Lấy vị trí đã lưu từ lbPosition
                var selectedPosition = lbPosition.Text;

                // Phân tách chuỗi vị trí (giả sử vị trí lưu dạng "X, Y")
                var coordinates = selectedPosition.Split(',');
                if (coordinates.Length == 2 &&
                    int.TryParse(coordinates[0].Trim(), out int x) &&
                    int.TryParse(coordinates[1].Trim(), out int y))
                {
                    PanelDraw.Location = new Point(-x, -y);
                }
                else
                {
                    MessageBox.Show("Vị trí không hợp lệ.");
                }
            }
            else
            {
                MessageBox.Show("Label không có vị trí để teleport.");
            }
        }

        private void InitializeOriginalSizes()
        {
            foreach (Control control in PanelDraw.Controls)
            {
                if (control is PictureBox pictureBox)
                {
                    if (!originalSizes.ContainsKey(pictureBox))
                    {
                        originalSizes[pictureBox] = pictureBox.Size; // Lưu kích thước gốc của PictureBox
                    }
                }
            }
        }
    }
}
