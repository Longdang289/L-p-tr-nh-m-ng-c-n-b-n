using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Mural_001;
using System.Net.NetworkInformation;
using System.Diagnostics;//Debug
using System.Linq;

namespace DoAnLon
{
    public partial class client : Form
    {
        //
        private readonly Action reopenHomeForm;//Cái này để dùng delegate mở lại home
        private string clientName="";
        private string serverIP="";
        private int controlStartCode=0;
        private string clientIP = "127.0.0.1";
        private string roomID;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private StreamReader reader;
        private StreamWriter writer;
        private const int port = 9999;
        //
        #region Khai báo vẽ
        private Drawing currentDrawing; // Lưu nét vẽ hiện tại
        private bool isDrawing = false;
        private Point lastPoint;
        private Pen pen;
        private Color currentColor = Color.Black;
        private int penSize = 2;
        private bool isInDrawingMode = false;
        private Bitmap drawingBitmap; // Bitmap để vẽ lên
        private const int DISTANCE_THRESHOLD = 3;  // Ngưỡng khoảng cách cố định để tăng cường nội suy
        private const float INTERPOLATION_MULTIPLIER = 1.5f;  // Tăng cường hệ số nhân nội suy khi khoảng cách lớn
        private Graphics drawingGraphics; // Graphics liên kết với Bitmap
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        private Stack<Bitmap> redoStack = new Stack<Bitmap>(); // Stack để lưu các trạng thái Redo

        private List<PictureBox> pictureBoxes = new List<PictureBox>(); // Danh sách chứa các PictureBox
        private bool isResizing = false; // Kiểm tra có đang thay đổi kích thước hay không
        //private Point initialMousePos; // Vị trí chuột ban đầu
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
        //private DrawShape currentShape = DrawShape.Line;
        #endregion
        #region Rác
        /*public class DoubleBufferedPanel : Panel
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
        */
        /*private void InitializeOriginalSizes()
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
        }*/
        #endregion
        // Constructor mặc định (không có tham số)
        public client()
        {
            InitializeComponent();
            lbRoomID.Visible = false;
            tbRoomID.Visible = false;
            tbRoomID.Enabled = false;
            lbName.Visible = false;
            tbName.Visible = false;
            tbName.Enabled = false;
        }

        // Constructor với tham số
        public client(string name, string serverIP, int controlStartCode, string roomID, Action ReopenHomeForm)
        {
            InitializeComponent();
            tbRoomID.Enabled = false;
            tbRoomID.Text = roomID;
            tbName.Enabled = false;
            tbName.Text=name;
            this.reopenHomeForm = ReopenHomeForm;
            this.clientName = name;
            this.serverIP = serverIP;
            this.controlStartCode = controlStartCode;
            this.roomID= roomID;
            // Ưu tiên lấy IP từ Wi-Fi
            clientIP = GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            // Nếu không có, lấy từ Ethernet
            if (clientIP == "127.0.0.1")
            {
                clientIP = GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }

            // Kết quả cuối cùng trong biến IP sẽ là địa chỉ LAN hoặc localhost
            // Hiển thị thông tin để kiểm tra
            MessageBox.Show($"Client Name: {clientName}, Server IP: {serverIP}, Control Code: {controlStartCode}, Room ID: {roomID}" );
            // Thử kết nối TCP đến server khi form được khởi tạo
            ConnectToServer();
            #region Load Vẽ
            pen = new Pen(currentColor, penSize);
            EnableDoubleBuffered(PanelDraw);
            //Tọa độ mới vào
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
            this.roomID = roomID;
            #endregion
        }
        public string GetLocalIPv4(NetworkInterfaceType type)
        {
            // Lặp qua tất cả các giao diện mạng trên máy
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Kiểm tra giao diện mạng theo loại giao diện và trạng thái hoạt động
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    // Lặp qua tất cả các địa chỉ IP unicast của giao diện mạng này
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        // Kiểm tra xem địa chỉ IP có phải IPv4 không
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // Trả về địa chỉ IPv4 khi tìm thấy
                            return ip.Address.ToString();
                        }
                    }
                }
            }

            // Nếu không tìm thấy địa chỉ LAN, trả về địa chỉ localhost
            return "127.0.0.1";
        }
        private void ConnectToServer()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverIP, port);
                networkStream = tcpClient.GetStream();
                reader = new StreamReader(networkStream, Encoding.UTF8);
                writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };
                MessageBox.Show($"Đã kết nối tới server vẽ tại {serverIP}:{port}", "Kết nối thành công");
                if (controlStartCode == 0) SendPacket(controlStartCode, clientName, clientIP, roomID, null);
                if (controlStartCode == 1) SendPacket(controlStartCode, clientName, clientIP, roomID);
                // Bắt đầu nhận tin nhắn không đồng bộ
                //Đa luồng, tạo luồng mới chỉ để receive, luồng chính chạy cái khác
                Task.Run(() => ReceiveAsyn());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kết nối tới server vẽ: {ex.Message}", "Kết nối thất bại");
            }
        }
        // Hàm gửi Packet đã JSON hóa tới server
        public void SendPacket(int code, string name, string ip, string roomid, string drawingData = null)
        {
            try
            {
                Packet packet = new Packet
                {
                    Code = code,
                    Username = name,
                    IP = ip,
                    RoomID = roomid,
                    DrawingData = drawingData // Thêm BitmapString vào Packet nếu có
                };

                // Chuyển đổi Packet thành JSON và gửi cùng dòng kết thúc
                string json = JsonConvert.SerializeObject(packet);
                writer.WriteLine(json);  // Gửi cùng dòng kết thúc
                writer.Flush();  // Đảm bảo gửi dữ liệu ngay lập tức
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending packet: " + ex.Message);
            }
        }
        private async Task SendPacketAsync(int code, string name, string ip, string roomid, string drawingData = null)
        {
            try
            {
                Packet packet = new Packet
                {
                    Code = code,
                    Username = name,
                    IP = ip,
                    RoomID = roomid,
                    //BitmapString = bitmapString // Thêm BitmapString vào Packet nếu có
                    DrawingData = drawingData
                };

                // Chuyển đổi Packet thành JSON và gửi cùng dòng kết thúc
                string json = JsonConvert.SerializeObject(packet);
                await writer.WriteLineAsync(json);  // Sử dụng WriteLineAsync cho bất đồng bộ
                await writer.FlushAsync();  // Đảm bảo gửi dữ liệu ngay lập tức
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending packet: " + ex.Message);
            }
        }
        

        

        //Luồng nhận Receive
        private async Task ReceiveAsyn()
        {
            try
            {
                //Vì dùng await ReadLineAsync nên nó có thể nhận receive liên tục
                //Khi nào ko nhận mà rảnh thì mới đọc và xử lý switch case
                while (true)
                {
                    // Đọc dòng dữ liệu JSON từ server
                    //Dùng ReadLineAsync nên nó đưa việc đọc dữ liệu nhập
                    //vào CallBack Queue để luồng hiện tại là luồng receive có thể
                    //làm việc khác trong khi người dùng nhập input để đọc
                    string responseInJson = await reader.ReadLineAsync();

                    // Kiểm tra xem phản hồi có rỗng hay không
                    if (string.IsNullOrEmpty(responseInJson))
                    {
                        Console.WriteLine("Received empty response from server.");
                        break;
                    }

                    //Console.WriteLine("Received JSON from server: " + responseInJson); // Log JSON nhận được

                    // Chuyển đổi dữ liệu JSON thành đối tượng Packet
                    Packet response = JsonConvert.DeserializeObject<Packet>(responseInJson);
                    Debug.WriteLine("Nhận được gói có mã là "+response.Code+"\n");
                    // Xử lý phản hồi dựa trên mã Code
                    switch (response.Code)
                    {
                        case 0:
                            generate_room_status(response);
                            break;
                        case 1:
                            join_room_status(response);
                            break;
                        case 2:
                            receive_currentDrawing(response);
                            break;
                        default:
                            packetHandleElse(response);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ReceiveAsync: " + ex.Message); // Log lỗi khi nhận dữ liệu
                tcpClient.Close();
            }
        }
        void packetHandleElse(Packet response)
        {
            if (response.Code == 404)
            {
                join_wrong_room();
            }
        }
        
        void join_wrong_room()
        {
            MessageBox.Show("Phòng không tồn tại");
            // Gọi phương thức delegate để mở lại Home form
            reopenHomeForm?.Invoke();

            // Đóng form Client
            this.Close();
        }
        void roomIDwrite(Packet response)
        {
            roomID = response.RoomID;

            if (tbRoomID.InvokeRequired)
            {
                // Sử dụng trực tiếp Invoke để cập nhật UI ngay lập tức
                tbRoomID.Invoke(new Action(() => tbRoomID.Text = response.RoomID.ToString()));
            }
            else
            {
                tbRoomID.Text = response.RoomID.ToString();
            }
        }
        void generate_room_status(Packet response)
        {
            roomID = response.RoomID;
            roomIDwrite(response);
        }
        void join_room_status(Packet response) 
        {
            //MessageBox.Show("Đã tham gia phòng " + response.RoomID);
            roomID = response.RoomID;
            receive_newDrawing(response);
        }
        void receive_currentDrawing(Packet response) 
        {
            try
            {
                if (string.IsNullOrEmpty(response.DrawingData))
                {
                    Console.WriteLine("Không có dữ liệu nét vẽ trong gói tin.");
                    return;
                }

                // Giải mã JSON thành đối tượng Drawing
                var drawing = JsonConvert.DeserializeObject<Drawing>(response.DrawingData);

                if (drawing == null)
                {
                    Console.WriteLine("Lỗi khi chuyển đổi dữ liệu JSON thành Drawing.");
                    return;
                }

                Console.WriteLine("Received PenColor: " + drawing.PenColor);  // Kiểm tra giá trị màu

                // Nếu PenColor không đúng, đặt lại thành màu đen
                if (drawing.PenColor == 0)
                {
                    drawing.PenColor = Color.Black.ToArgb();
                }

                // Vẽ nét lên `drawingBitmap` thay vì trực tiếp lên Panel
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    using (Pen pen = new Pen(Color.FromArgb(drawing.PenColor), drawing.PenWidth))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        for (int i = 1; i < drawing.Points.Count; i++)
                        {
                            g.DrawLine(pen, drawing.Points[i - 1], drawing.Points[i]);
                        }
                    }
                }

                // Yêu cầu vẽ lại Panel để hiển thị các nét vẽ
                PanelDraw.Invalidate();

                Console.WriteLine("Đã vẽ nét vẽ thành công từ dữ liệu nhận được.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi nhận nét vẽ: " + ex.Message);
            }
        }
        void receive_newDrawing(Packet response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.DrawingData))
                {
                    Console.WriteLine("Không có dữ liệu nét vẽ trong gói tin.");
                    return;
                }

                // Giải mã JSON thành danh sách các đối tượng Drawing
                var drawingList = JsonConvert.DeserializeObject<List<Drawing>>(response.DrawingData);

                if (drawingList == null || drawingList.Count == 0)
                {
                    Console.WriteLine("Lỗi khi chuyển đổi dữ liệu JSON thành danh sách Drawing.");
                    return;
                }
                // Nếu PenColor không đúng, đặt lại thành màu đen
                
                // Vẽ từng nét vẽ trong danh sách lên `drawingBitmap`
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    foreach (var drawing in drawingList)
                    {
                        // Nếu PenColor không đúng, đặt lại thành màu đen
                        if (drawing.PenColor == 0)
                        {
                            drawing.PenColor = Color.Black.ToArgb();
                        }
                        using (Pen pen = new Pen(Color.FromArgb(drawing.PenColor), drawing.PenWidth))
                        {
                            for (int i = 1; i < drawing.Points.Count; i++)
                            {
                                g.DrawLine(pen, drawing.Points[i - 1], drawing.Points[i]);
                            }
                        }
                    }
                }

                // Yêu cầu vẽ lại Panel để hiển thị các nét vẽ
                PanelDraw.Invalidate();

                Console.WriteLine("Đã vẽ lại tất cả các nét vẽ từ dữ liệu nhận được.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi nhận nét vẽ: " + ex.Message);
            }
        }
        /*void receive_newbitmap(Packet response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.BitmapString))
                {
                    Console.WriteLine("Không có bitmap trong gói tin.");
                    return;
                }

                byte[] bitmapBytes = Convert.FromBase64String(response.BitmapString);
                var imageConverter = new ImageConverter();
                Bitmap receivedBitmap = (Bitmap)imageConverter.ConvertFrom(bitmapBytes);

                if (receivedBitmap == null)
                {
                    Console.WriteLine("Lỗi khi chuyển đổi dữ liệu byte thành bitmap.");
                    return;
                }

                // Vẽ transparent bitmap nhận được lên drawingBitmap
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    g.DrawImage(receivedBitmap, Point.Empty);
                }

                PanelDraw.Invalidate();
                Console.WriteLine("Cập nhật bitmap thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi nhận bitmap: " + ex.Message);
            }
        }
        string convert_bitmap_to_server(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                Console.WriteLine("Bitmap is null, cannot convert to server format.");
                return null;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] bitmapBytes = ms.ToArray();
                    string bitmapString = Convert.ToBase64String(bitmapBytes);
                    return bitmapString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting bitmap to server format: " + ex.Message);
                return null;
            }
        }
        */

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Nếu có kết nối TCP (tcpClient không null và đang kết nối), thì đóng kết nối.
            if (tcpClient != null && tcpClient.Connected)
            {
                try
                {
                    writer?.Close();
                    reader?.Close();
                    networkStream?.Close();
                    tcpClient?.Close();
                }
                catch (Exception ex)
                {
                    // Bắt lỗi nếu có sự cố khi đóng kết nối, tránh gây ra lỗi khi đóng form.
                    Console.WriteLine("Error closing connection: " + ex.Message);
                }
            }

            // Giải phóng tài nguyên khác nếu cần, để đảm bảo form luôn đóng được.
        }
        #region ESC
        //---------------------------------ESC để thoát---------------------------------
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) // Kiểm tra xem phím nhấn có phải là Esc không
            {
                Application.Exit(); // Thoát chương trình
            }
        }
        #endregion
        #region Form Load
        //----------------------------------Form Load-----------------------------------
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
        #endregion
        #region Pen Click
        //---------------------------------Nhấn nút Pen----------------------------------
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
                    //PanelDraw.Invalidate(); // Yêu cầu vẽ lại panelDraw
                }
            }
        }
        #endregion
        #region Erase Click
        //------------------------------Nhấn nút erase-----------------------------------
        private void btn_eraser_Click(object sender, EventArgs e)
        {
            isInDrawingMode = true;
            pen.Color = PanelDraw.BackColor;
            pen.Width = penSize;
            redoStack.Clear(); // Xóa redoStack khi người dùng vẽ mới
        }
        #endregion
        #region Sự kiện Vẽ
        //-----------------------------------Sự kiện vẽ-----------------------------------
        // Danh sách các điểm vẽ (với các điểm nội suy)
        //List<Point> drawingPoints = new List<Point>();
        // Hàm tính khoảng cách giữa hai điểm
        private float GetDistance(Point p1, Point p2)
        {
            return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        //Tính số nội suy tỉ lệ nghịch độ dày
        private int CalculateInterpolationSteps(float penWidth, float distance)
        {
            // Số bước cơ bản: càng nhỏ khi bút càng dày, càng nhiều điểm nội suy khi bút mỏng
            int baseSteps = (int)(15 - (penWidth / 72) * 12)+5;

            // Nếu vượt ngưỡng, tăng thêm số bước tỉ lệ thuận với khoảng cách và hệ số INTERPOLATION_MULTIPLIER
            int additionalSteps = distance > DISTANCE_THRESHOLD
                ? (int)((distance / DISTANCE_THRESHOLD) * baseSteps * INTERPOLATION_MULTIPLIER)
                : baseSteps;

            return Math.Max(additionalSteps, 2);  // Đảm bảo luôn có ít nhất 2 bước
        }

        // Hàm lấy các điểm nội suy giữa hai điểm start và end
        private List<Point> GetInterpolatedPoints(Point start, Point end, int steps)
        {
            List<Point> points = new List<Point>();
            float dx = (end.X - start.X) / (float)steps;
            float dy = (end.Y - start.Y) / (float)steps;

            for (int i = 1; i <= steps; i++)
            {
                int interpolatedX = (int)(start.X + i * dx);
                int interpolatedY = (int)(start.Y + i * dy);
                points.Add(new Point(interpolatedX, interpolatedY));
            }

            return points;
        }
        // Hàm tính Bezier curve giữa 3 điểm (start, control, end)
        private List<Point> GetBezierPoints(Point start, Point control, Point end, int steps)
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                float x = (1 - t) * (1 - t) * start.X + 2 * (1 - t) * t * control.X + t * t * end.X;
                float y = (1 - t) * (1 - t) * start.Y + 2 * (1 - t) * t * control.Y + t * t * end.Y;
                points.Add(new Point((int)x, (int)y));
            }

            return points;
        }
        //Ấn chuột trái lên Panel
        private void PanelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Khởi tạo drawingBitmap nếu null
                if (drawingBitmap == null)
                {
                    drawingBitmap = new Bitmap(PanelDraw.Width, PanelDraw.Height);
                    using (Graphics g = Graphics.FromImage(drawingBitmap))
                    {
                        g.Clear(Color.White);  // Đặt nền trắng cho bitmap
                    }
                }

                isDrawing = true;
                lastPoint = e.Location;  // Ghi nhận vị trí bắt đầu vẽ
                currentDrawing = new Drawing
                {
                    PenColor = pen.Color.ToArgb(),
                    PenWidth = pen.Width
                };
                currentDrawing.Points.Add(lastPoint);
            }
        }



        //Kéo thả chuột tạo ra nét vẽ theo
        private void PanelDraw_MouseMove(object sender, MouseEventArgs e)
        {

            if (isDrawing)
            {
                Point currentPoint = e.Location;

                // Tính toán khoảng cách giữa điểm hiện tại và điểm cuối
                float distance = GetDistance(lastPoint, currentPoint);

                // Tăng cường điểm nội suy nếu khoảng cách vượt ngưỡng
                int interpolationSteps = CalculateInterpolationSteps(pen.Width, distance);

                // Lấy các điểm nội suy giữa lastPoint và currentPoint
                List<Point> interpolatedPoints = GetInterpolatedPoints(lastPoint, currentPoint, interpolationSteps);

                // Lưu các điểm nội suy vào danh sách
                currentDrawing.Points.AddRange(interpolatedPoints);

                // Vẽ các đoạn ngắn giữa các điểm nội suy trên `drawingBitmap`
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    foreach (var point in interpolatedPoints)
                    {
                        g.DrawLine(pen, lastPoint, point);
                        lastPoint = point;
                    }
                }

                // Yêu cầu `PanelDraw` vẽ lại bằng cách hiển thị `drawingBitmap`
                PanelDraw.Invalidate();
            }
        }

        //Nhả chuột thì không vẽ nữa
        private async void PanelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            
            Debug.Write("\n");
            if (isDrawing)
            {
                isDrawing = false;
                if (currentDrawing != null)
                {
                    // Chuyển nét vẽ hiện tại thành JSON và gửi tới server
                    string jsonData = JsonConvert.SerializeObject(currentDrawing);
                    await SendPacketAsync(2, clientName, clientIP, roomID, jsonData);

                    currentDrawing = null; // Đặt lại sau khi gửi
                }
                PanelDraw.Invalidate();
                Debug.WriteLine("Thông số nhả chuột:\n" + "isDrawing: " + isDrawing + "\n" + "isInDrawingMode: " + isInDrawingMode + "\n");
                //await SendPacketAsync(2, clientName, clientIP, roomID, convert_bitmap_to_server(transparentBitmap));
            }
        }

        #endregion
        #region Thu phóng Panel
        //--------------------Thu phóng Panel không mất nội dung-------------------------
        private void PanelDraw_Resize(object sender, EventArgs e)
        {
            if (drawingBitmap != null) // Kiểm tra nếu Bitmap hiện tại không rỗng
            {
                Bitmap oldBitmap = drawingBitmap; // Lưu lại Bitmap cũ
                // Khởi tạo Bitmap mới với kích thước mới của Panel
                drawingBitmap = new Bitmap(PanelDraw.Width, PanelDraw.Height);
                // Giải phóng Graphics cũ, tạo Graphics mới cho Bitmap mới
                drawingGraphics?.Dispose();
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                drawingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // Vẽ lại nội dung của Bitmap cũ lên Bitmap mới
                drawingGraphics.DrawImage(oldBitmap, Point.Empty);
                // Giải phóng tài nguyên của Bitmap cũ
                oldBitmap.Dispose();
            }
            // Yêu cầu vẽ lại panel để hiển thị đúng nội dung
            PanelDraw.Invalidate();
        }
        #endregion
        #region Lưu PNG
        //-------------------------------Lưu lại thành PNG----------------------------
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Tệp PNG|*.png";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                drawingBitmap.Save(saveDialog.FileName);
            }
        }
        #endregion
        #region Insert ảnh
        //---------------------------------Insert Ảnh----------------------------------
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
                    Width = loadedImage.Width, // Giới hạn chiều rộng theo chiều rộng của panel
                    Height = loadedImage.Height, // Giới hạn chiều cao theo chiều cao của panel
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
        #endregion
        #region Lưu state panel hiện tại
        //--------------------------Lưu lại trạng thái panel hiện tại-------------------
        private void SaveState()
        {
            undoStack.Push((Bitmap)drawingBitmap.Clone()); // Lưu trạng thái hiện tại
        }
        #endregion
        #region Undo
        //----------------------------------Nút UNDO----------------------------------
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
        #endregion
        #region Redo
        //-----------------------------------Nút Redo---------------------------------
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
    #endregion
    #region Vẽ bitmap lên panel
    //----------------------------Hàm vẽ cái bitmap lên cái Panel-----------------
    private void PanelDraw_Paint(object sender, PaintEventArgs e)
    {
            if (drawingBitmap != null)
            {
                e.Graphics.DrawImage(drawingBitmap, 0, 0); // Vẽ toàn bộ `drawingBitmap` lên `PanelDraw`
            }
        }

        #endregion
        #region Thu phóng, di chuyển ảnh
        //----------------------------Kéo, thu phóng ảnh dc insert---------------------------


        //Ấn ảnh nào, kéo ảnh đó
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offsetX = e.X; // Lưu vị trí chuột khi bắt đầu di chuyển
                offsetY = e.Y;
            }
        }

        //Đổi tọa độ ảnh theo tọa độ chuột
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

        //Nhả thôi không đồng bộ chuột và ảnh nữa
        private void PictureBox_MouseUpPictureBox(object sender, MouseEventArgs e)
        {
            // Reset lại biến sau khi thả chuột
            isResizing = false;
        }

        //Lăn chuột thu phóng ảnh
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
        #endregion
        #region Kéo thả Panel
        //------------------------------Kéo thả Panel-------------------------------------


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
        #endregion
        #region Double Buffer

        //------------------Enable Buffer đôi cho Panel vẽ khỏi lag-------------------------
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
        #endregion
        #region Nút Clear
        //-----------------------Nút Clear hết panel trừ stickynote-------------------------
        private void btnClear_Click(object sender, EventArgs e)
        {
            drawingGraphics.Clear(PanelDraw.BackColor); // Xóa toàn bộ Bitmap
            PanelDraw.Invalidate(); // Vẽ lại panel
            redoStack.Clear(); // Xóa redoStack khi người dùng vẽ mới
        }
        #endregion
        #region Load panel căn giữa
        //-------------------Load PanelDraw căn giữa ko cần form load-----------------------
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
        #endregion
        #region Sticky Note
        //---------------------------------Ấn gọi stickynote---------------------------------
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
        #endregion
        #region Độ to nét vẽ
        //-------------------------------Ấn Update to nhỏ nét vẽ--------------------------------
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
        #endregion
        #region Giật tọa độ
        //----------------------------------Giật tọa độ--------------------------------------

        //Lấy tọa độ gán vô label 
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
        //Kéo giật tọa độ theo label
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
        #endregion
    }
}
