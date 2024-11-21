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
        private string serverIP= "13.76.28.212";
        private int controlStartCode=0;
        private string clientIP = "127.0.0.1";
        private string roomID;
        private List<Image> imageList = new List<Image>();
        private Image current_image;
        private List<sticky_note> stickyNotes = new List<sticky_note>();
        private sticky_note current_note;
        private bool isHost;
        private bool isForce = false;
        private bool beForced = false;
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
            tbNoteOne.Text = "150";
            tbNoteTwo.Text = "150";
            isHost = false;
            // Ưu tiên lấy IP từ Wi-Fi
            clientIP = GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            // Nếu không có, lấy từ Ethernet
            if (clientIP == "127.0.0.1")
            {
                clientIP = GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }

            // Kết quả cuối cùng trong biến IP sẽ là địa chỉ LAN hoặc localhost
            // Hiển thị thông tin để kiểm tra
            MessageBox.Show($"Name: {clientName}, Server IP: {serverIP}, Control Code: {controlStartCode}, Room ID: {roomID}" );
            // Thử kết nối TCP đến server khi form được khởi tạo
            ConnectToServer();
            #region Load Vẽ
            pen = new Pen(currentColor, penSize);
            EnableDoubleBuffered(PanelDraw);
            //Tọa độ mới vào
            PanelDraw.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            
            btn_eraser.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Pen.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            
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
            Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
            Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
            int visibleX = formOriginInScreen.X - panelOriginInForm.X;
            int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
            // Đặt văn bản cho label với tọa độ của form
            lbPosition.Text = $"{visibleX}, {visibleY}";
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
                        case 3:
                            receive_image(response);
                            break;
                        case 4:
                            update_image(response);
                            break;
                        case 5:
                            receive_currentNote(response);
                            break;
                        case 6:
                            update_note(response);
                            break;
                        case 7:
                            join_get_image(response);
                            break;
                        case 8:
                            join_get_notes(response);
                            break;
                        case 9:
                            be_in_Forced(response);
                            break;
                        case 10:
                            stop_be_Forced(response);
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
        
        void receive_currentNote(Packet response)
        {
            Debug.WriteLine("Đã nhận note");
            try
            {
                if (string.IsNullOrEmpty(response.DrawingData))
                {
                    Debug.WriteLine("Gói tin bị thiếu DrawingData.");
                    return;
                }

                sticky_note temp = JsonConvert.DeserializeObject<sticky_note>(response.DrawingData);
                if (temp == null)
                {
                    Debug.WriteLine("Deserialization thất bại: DrawingData không hợp lệ.");
                    return;
                }

                if ((clientName == response.Username) && (clientIP==response.IP))
                {
                    Debug.WriteLine("Đã nhận note của tôi");
                    lock (stickyNotes)
                    {
                        current_note.noteID = temp.noteID;
                        foreach (Control control in PanelDraw.Controls)
                        {
                            if (control is TextBox textBox && textBox.Text == current_note.NoteText)
                            {
                                textBox.Tag = current_note.noteID;
                                break;
                            }
                        }
                        stickyNotes.Add(current_note);
                    }
                }
                else
                {
                    Debug.WriteLine("Đã nhận note của người khác");
                    Debug.WriteLine("Full JSON received: " + response.DrawingData);
                    

                    PanelDraw.Invoke(new Action(() =>
                    {
                        try
                        {
                            

                            // Tạo TextBox
                            TextBox stickynewTextBox = new TextBox
                            {
                                Multiline = true,
                                BorderStyle = BorderStyle.FixedSingle,
                                BackColor = temp.BackgroundColor,
                                Location = temp.LocationPoint,
                                Size = temp.NoteSize,
                                Text = temp.NoteText,
                                TextAlign = HorizontalAlignment.Center,
                                Tag = temp.noteID,
                                Font = new Font("Arial", 13, FontStyle.Regular)
                            };

                            bool isDragging = false;
                            Point initialCursorPosition = Point.Empty; // Tọa độ chuột ban đầu

                            // Gắn sự kiện chuột để kéo thả
                            stickynewTextBox.MouseDown += (s, ev) =>
                            {
                                if (ev.Button == MouseButtons.Left)
                                {
                                    // Bắt đầu kéo thả
                                    isDragging = true;
                                    initialCursorPosition = ev.Location;
                                }
                            };

                            stickynewTextBox.MouseMove += (s, ev) =>
                            {
                                if (isDragging && ev.Button == MouseButtons.Left)
                                {
                                    // Tính toán vị trí mới của TextBox
                                    Point newLocation = new Point(
                                        stickynewTextBox.Left + ev.X - initialCursorPosition.X,
                                        stickynewTextBox.Top + ev.Y - initialCursorPosition.Y
                                    );
                                    stickynewTextBox.Location = newLocation;

                                    // Tìm sticky_note tương ứng và cập nhật thông tin
                                    sticky_note sticky = stickyNotes.Find(note => note.noteID == (int)stickynewTextBox.Tag);
                                    if (sticky != null)
                                    {
                                        sticky.UpdateLocation(newLocation);
                                    }
                                }
                            };

                            stickynewTextBox.MouseUp += (s, ev) =>
                            {
                                if (isDragging && ev.Button == MouseButtons.Left)
                                {
                                    // Kết thúc kéo thả
                                    isDragging = false;

                                    // Tìm sticky_note tương ứng và gửi gói tin sau khi thả chuột
                                    sticky_note sticky = stickyNotes.Find(note => note.noteID == (int)stickynewTextBox.Tag);
                                    if (sticky != null)
                                    {
                                        // Serialize sticky_note thành JSON và gửi gói tin
                                        string jsonnewNote = JsonConvert.SerializeObject(sticky);
                                        SendPacket(6, clientName, clientIP, roomID, jsonnewNote);
                                        Debug.WriteLine("Gửi gói 6, di chuyển hoàn tất");
                                    }
                                }
                            };

                            // Gắn sự kiện để cập nhật nội dung
                            stickynewTextBox.TextChanged += (s, ev) =>
                            {
                                // Nếu đang kéo thả, bỏ qua việc cập nhật nội dung
                                if (isDragging)
                                    return;

                                // Tìm sticky_note tương ứng và cập nhật thông tin
                                sticky_note sticky = stickyNotes.Find(note => note.noteID == (int)stickynewTextBox.Tag);
                                if (sticky != null)
                                {
                                    sticky.UpdateText(stickynewTextBox.Text);

                                    // Serialize sticky_note thành JSON và gửi gói tin
                                    string jsonnewNote = JsonConvert.SerializeObject(sticky);
                                    SendPacket(6, clientName, clientIP, roomID, jsonnewNote);
                                    Debug.WriteLine("Gửi gói 6, text");
                                }
                            };

                            // Thêm TextBox vào PanelDraw
                            PanelDraw.Controls.Add(stickynewTextBox);
                            stickynewTextBox.BringToFront();

                            // Thêm vào danh sách stickyNotes
                            lock (stickyNotes)
                            {
                                stickyNotes.Add(temp);
                            }

                            Debug.WriteLine("TextBox created and events registered successfully.");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Lỗi khi xử lý TextBox: {ex.Message}\n{ex.StackTrace}");
                        }
                    }));

                    Debug.WriteLine("TextBox created successfully.");

                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong receive_currentNote: {ex.Message}\n{ex.StackTrace}");
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
            isHost = true;
            roomID = response.RoomID;
            roomIDwrite(response);
        }
        void join_room_status(Packet response) 
        {
            isHost = false;
            roomID = response.RoomID;
            receive_newDrawing(response);
        }
        void join_get_image(Packet response)
        {
            Debug.WriteLine("Da nhan image join");
            try
            {
                if (string.IsNullOrEmpty(response.DrawingData))
                {
                    Console.WriteLine("Không có dữ liệu image trong gói tin.");
                    return;
                }
                // Giải mã danh sách Image từ JSON trong response.DrawingData
                List<Image> images = JsonConvert.DeserializeObject<List<Image>>(response.DrawingData);

                // Cập nhật tất cả Image vào danh sách imageList
                foreach (var img in images)
                {
                    imageList.Add(img);
                }
                Debug.WriteLine("Da them moi image vao danh sach");

                // Sử dụng Invoke nếu cần thiết để đảm bảo thao tác UI thực hiện trên thread chính
                if (PanelDraw.InvokeRequired)
                {
                    Debug.WriteLine("Sử dụng Invoke để cập nhật giao diện.");
                    PanelDraw.Invoke(new Action(() => AddImagesToUI(images)));
                }
                else
                {
                    Debug.WriteLine("Cập nhật giao diện trực tiếp.");
                    AddImagesToUI(images);
                }
            }
            catch
            {
                Debug.WriteLine($"join_get_image: Lỗi không xác định");
            }
            
        }
        void join_get_notes(Packet response)
        {
            Debug.WriteLine("Đã nhận note join");
            try
            {
                if (string.IsNullOrEmpty(response.DrawingData))
                {
                    Console.WriteLine("Không có dữ liệu note trong gói tin.");
                    return;
                }

                // Deserialize gói tin thành danh sách sticky_note
                List<sticky_note> Notes = JsonConvert.DeserializeObject<List<sticky_note>>(response.DrawingData);
                if (Notes == null || Notes.Count == 0)
                {
                    Debug.WriteLine("Danh sách note rỗng hoặc không hợp lệ.");
                    return;
                }

                // Cập nhật danh sách stickyNotes
                lock (stickyNotes)
                {
                    stickyNotes.AddRange(Notes);
                }
                Debug.WriteLine("Đã thêm mới notes vào danh sách.");

                // Thêm sticky_note lên giao diện
                if (PanelDraw.InvokeRequired)
                {
                    Debug.WriteLine("Sử dụng Invoke để cập nhật giao diện.");
                    PanelDraw.Invoke(new Action(() => AddStickyNotesToUI(Notes)));
                }
                else
                {
                    Debug.WriteLine("Cập nhật giao diện trực tiếp.");
                    AddStickyNotesToUI(Notes);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"join_get_notes: Lỗi không xác định - {ex.Message}\n{ex.StackTrace}");
            }
        }

        // Hàm hỗ trợ để thêm sticky_note vào giao diện
        void AddStickyNotesToUI(List<sticky_note> notes)
        {
            foreach (var note in notes)
            {
                TextBox stickyTextBox = new TextBox
                {
                    Multiline = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = note.BackgroundColor,
                    Location = note.LocationPoint,
                    Size = note.NoteSize,
                    Text = note.NoteText,
                    TextAlign = HorizontalAlignment.Center,
                    Tag = note.noteID,
                    Font = new Font("Arial", 13, FontStyle.Regular) // Thiết lập font chữ lớn
                };

                bool isDragging = false;
                Point initialCursorPosition = Point.Empty; // Tọa độ chuột ban đầu

                // Gắn sự kiện chuột để kéo thả
                stickyTextBox.MouseDown += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                    {
                        // Bắt đầu kéo thả
                        isDragging = true;
                        initialCursorPosition = ev.Location;
                    }
                };

                stickyTextBox.MouseMove += (s, ev) =>
                {
                    if (isDragging && ev.Button == MouseButtons.Left)
                    {
                        // Tính toán vị trí mới của TextBox
                        Point newLocation = new Point(
                            stickyTextBox.Left + ev.X - initialCursorPosition.X,
                            stickyTextBox.Top + ev.Y - initialCursorPosition.Y
                        );
                        stickyTextBox.Location = newLocation;

                        // Tìm sticky_note tương ứng và cập nhật thông tin
                        sticky_note sticky = stickyNotes.Find(nt => nt.noteID == (int)stickyTextBox.Tag);
                        if (sticky != null)
                        {
                            sticky.UpdateLocation(newLocation);
                        }
                    }
                };

                stickyTextBox.MouseUp += (s, ev) =>
                {
                    if (isDragging && ev.Button == MouseButtons.Left)
                    {
                        // Kết thúc kéo thả
                        isDragging = false;

                        // Tìm sticky_note tương ứng và gửi gói tin sau khi thả chuột
                        sticky_note sticky = stickyNotes.Find(nt => nt.noteID == (int)stickyTextBox.Tag);
                        if (sticky != null)
                        {
                            // Serialize sticky_note thành JSON và gửi gói tin
                            string jsonnewNote = JsonConvert.SerializeObject(sticky);
                            SendPacket(6, clientName, clientIP, roomID, jsonnewNote);
                            Debug.WriteLine("Gửi gói 6, di chuyển hoàn tất");
                        }
                    }
                };

                // Gắn sự kiện để cập nhật nội dung
                stickyTextBox.TextChanged += (s, ev) =>
                {
                    // Nếu đang kéo thả, bỏ qua việc cập nhật nội dung
                    if (isDragging)
                        return;

                    // Tìm sticky_note tương ứng và cập nhật thông tin
                    sticky_note sticky = stickyNotes.Find(nt => nt.noteID == (int)stickyTextBox.Tag);
                    if (sticky != null)
                    {
                        sticky.UpdateText(stickyTextBox.Text);

                        // Serialize sticky_note thành JSON và gửi gói tin
                        string jsonnewNote = JsonConvert.SerializeObject(sticky);
                        SendPacket(6, clientName, clientIP, roomID, jsonnewNote);
                        Debug.WriteLine("Gửi gói 6, text");
                    }
                };

                // Thêm TextBox vào PanelDraw
                PanelDraw.Controls.Add(stickyTextBox);
                stickyTextBox.BringToFront();
            }
        }
        // Hàm thực hiện thêm danh sách Image vào imageList và PanelDraw
        private void AddImagesToUI(List<Image> images)
        {
            foreach (var img in images)
            {
                Debug.WriteLine($"Thêm ImageID: {img.ImageID}, Position: {img.Position}, Size: {img.ImageSize}");

                // Gọi hàm AddPictureBoxToPanel để thêm từng PictureBox
                AddPictureBoxToPanel(img);
            }

            // Làm mới PanelDraw
            PanelDraw.Refresh();
            Debug.WriteLine($"Đã thêm {images.Count} Image vào PanelDraw.");
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

                // Khóa bitmap để tránh truy cập đồng thời
                lock (drawingBitmap)
                {
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
                }

                // Yêu cầu vẽ lại Panel để hiển thị các nét vẽ
                if (PanelDraw.InvokeRequired)
                {
                    PanelDraw.Invoke(new Action(() => PanelDraw.Invalidate()));
                }
                else
                {
                    PanelDraw.Invalidate();
                }

                Console.WriteLine("Đã vẽ lại tất cả các nét vẽ từ dữ liệu nhận được.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi nhận nét vẽ: " + ex.Message);
            }
        }
        void receive_image(Packet response)
        {
            Image temp = JsonConvert.DeserializeObject<Image>(response.DrawingData);
            if ((response.Username == clientName)&&(response.IP==clientIP))//Mai mốt xét IP nx
            {
                
                // Cập nhật ID cho `current_image` và thêm vào danh sách
                current_image.ImageID = temp.ImageID;
                imageList.Add(current_image);

                // Tìm PictureBox tương ứng (nếu có)
                PictureBox pictureBox = PanelDraw.Controls
                    .OfType<PictureBox>()
                    .FirstOrDefault(pb => pb.Tag == null || (int)pb.Tag == -1);

                if (pictureBox != null)
                {
                    // Cập nhật Tag của PictureBox thành temp.ImageID
                    pictureBox.Tag = temp.ImageID;

                    Debug.WriteLine($"PictureBox đã được gán Tag: {temp.ImageID}");
                }
                else
                {
                    Debug.WriteLine("Không tìm thấy PictureBox trống để gán Tag.");
                }
            }
            else
            {
                Debug.WriteLine("Nhan image cua client khac");

                // Sử dụng Invoke để thêm PictureBox vào PanelDraw
                if (PanelDraw.InvokeRequired)
                {
                    PanelDraw.Invoke(new Action(() =>
                    {
                        AddPictureBoxToPanel(temp);
                    }));
                }
                else
                {
                    AddPictureBoxToPanel(temp);
                }

                // Thêm vào danh sách ảnh
                imageList.Add(temp);
            }
        }
        void update_image(Packet response)
        {
            // Giải mã JSON từ response.DrawingData thành đối tượng Image
            Image temp = JsonConvert.DeserializeObject<Image>(response.DrawingData);

            // Kiểm tra xem cần gọi Invoke hay không
            if (PanelDraw.InvokeRequired)
            {
                // Sử dụng Invoke để chạy hàm trên thread chính
                PanelDraw.Invoke(new Action(() => UpdateUI(temp)));
            }
            else
            {
                // Nếu đang ở thread chính, cập nhật trực tiếp
                UpdateUI(temp);
            }
        }
        void update_note(Packet response)
        {
            // Giải mã JSON từ response.DrawingData thành đối tượng note
            sticky_note temp = JsonConvert.DeserializeObject<sticky_note>(response.DrawingData);

            // Kiểm tra xem cần gọi Invoke hay không
            if (PanelDraw.InvokeRequired)
            {
                // Sử dụng Invoke để chạy hàm trên thread chính
                PanelDraw.Invoke(new Action(() => UpdateNoteUI(temp)));
            }
            else
            {
                // Nếu đang ở thread chính, cập nhật trực tiếp
                UpdateNoteUI(temp);
            }
        }

        // Hàm xử lý logic cập nhật UI
        private void UpdateUI(Image temp)
        {
            // Tìm PictureBox trên giao diện bằng cách sử dụng Tag
            PictureBox pictureBox = PanelDraw.Controls
                .OfType<PictureBox>()
                .FirstOrDefault(pb => pb.Tag != null && (int)pb.Tag == temp.ImageID);

            if (pictureBox != null)
            {
                Debug.WriteLine($"Cập nhật PictureBox có Tag (ImageID): {temp.ImageID}");

                // Cập nhật vị trí và kích thước của PictureBox
                pictureBox.Location = temp.Position;
                pictureBox.Size = temp.ImageSize;

                // Cập nhật thông tin Image trong imageList
                var existingImage = imageList.FirstOrDefault(img => img.ImageID == temp.ImageID);
                if (existingImage != null)
                {
                    existingImage.Position = temp.Position;
                    existingImage.ImageSize = temp.ImageSize;
                }
                else
                {
                    // Nếu Image không tồn tại trong imageList, thêm mới
                    imageList.Add(temp);
                    Debug.WriteLine("Thêm mới Image vào danh sách.");
                }
            }
            else
            {
                Debug.WriteLine($"Không tìm thấy PictureBox tương ứng với ImageID: {temp.ImageID}");
            }
        }
        private void UpdateNoteUI(sticky_note temp)
        {
            // Tìm TextBox trên giao diện bằng cách sử dụng Tag
            TextBox stickyTextBox = PanelDraw.Controls
                .OfType<TextBox>()
                .FirstOrDefault(tb => tb.Tag != null && (int)tb.Tag == temp.noteID);

            if (stickyTextBox != null)
            {
                Debug.WriteLine($"Cập nhật TextBox có Tag (noteID): {temp.noteID}");

                // Cập nhật vị trí, kích thước, nội dung, và màu nền của TextBox
                stickyTextBox.Location = temp.LocationPoint;
                stickyTextBox.Size = temp.NoteSize;
                stickyTextBox.Text = temp.NoteText;
                stickyTextBox.BackColor = temp.BackgroundColor;

                // Cập nhật thông tin sticky_note trong danh sách StickyNotes
                var existingNote = stickyNotes.FirstOrDefault(note => note.noteID == temp.noteID);
                if (existingNote != null)
                {
                    existingNote.LocationPoint = temp.LocationPoint;
                    existingNote.NoteSize = temp.NoteSize;
                    existingNote.NoteText = temp.NoteText;
                    existingNote.BackgroundColor = temp.BackgroundColor;
                }
                else
                {
                    // Nếu sticky_note không tồn tại trong danh sách, thêm mới
                    stickyNotes.Add(temp);
                    Debug.WriteLine("Thêm mới sticky_note vào danh sách.");
                }
            }
            else
            {
                Debug.WriteLine($"Không tìm thấy TextBox tương ứng với noteID: {temp.noteID}");
            }
        }
        private void AddPictureBoxToPanel(Image temp)
        {
            PictureBox pictureBox = new PictureBox
            {
                Image = Image.DecodeBase64ToBitmap(temp.content),
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = temp.ImageSize.Width,
                Height = temp.ImageSize.Height,
                Location = temp.Position,
                Tag = temp.ImageID
            };

            // Thêm PictureBox vào PanelDraw
            PanelDraw.Controls.Add(pictureBox);

            // Đăng ký sự kiện chuột cho PictureBox (nếu cần)
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMovePictureBox;
            pictureBox.MouseUp += PictureBox_MouseUpPictureBox;
            pictureBox.MouseWheel += PictureBox_MouseWheel;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Kiểm tra nếu là host
            if (isHost)
            {
                // Hiển thị hộp thoại xác nhận lưu trước khi thoát
                var result = MessageBox.Show(
                    "Bạn có muốn lưu trước khi thoát?",
                    "Xác nhận thoát",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    // Hủy đóng form
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    // Mở hộp thoại lưu file giống như trong btnSave
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Tệp PNG|*.png|Tệp JPG|*.jpg";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Gọi logic lưu file
                        bool isPng = saveDialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
                        Bitmap bitmap = new Bitmap(PanelDraw.Width, PanelDraw.Height, PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            // Xử lý nền trong suốt hoặc màu nền
                            if (isPng)
                            {
                                g.Clear(Color.Transparent);
                            }
                            else
                            {
                                g.Clear(PanelDraw.BackColor);
                            }

                            // Vẽ các đối tượng trong stickyNotes
                            foreach (var note in stickyNotes)
                            {
                                using (Brush brush = new SolidBrush(note.BackgroundColor))
                                {
                                    g.FillRectangle(brush, new Rectangle(note.LocationPoint, note.NoteSize));
                                }

                                using (Font font = new Font("Arial", 13, FontStyle.Regular))
                                using (Brush textBrush = new SolidBrush(Color.Black))
                                {
                                    StringFormat stringFormat = new StringFormat
                                    {
                                        Alignment = StringAlignment.Center,
                                        LineAlignment = StringAlignment.Center
                                    };
                                    g.DrawString(note.NoteText, font, textBrush,
                                        new RectangleF(note.LocationPoint.X, note.LocationPoint.Y, note.NoteSize.Width, note.NoteSize.Height),
                                        stringFormat);
                                }
                            }

                            // Vẽ các đối tượng trong imageList
                            foreach (var img in imageList)
                            {
                                if (!string.IsNullOrEmpty(img.content))
                                {
                                    var decodedImage = Image.DecodeBase64ToBitmap(img.content);
                                    g.DrawImage(decodedImage, new Rectangle(img.Position, img.ImageSize));
                                }
                            }

                            // Vẽ các đường nét từ drawingBitmap
                            if (drawingBitmap != null)
                            {
                                g.DrawImage(drawingBitmap, 0, 0);
                            }
                        }

                        // Lưu bitmap ra tệp
                        ImageFormat format = isPng ? ImageFormat.Png : ImageFormat.Jpeg;
                        bitmap.Save(saveDialog.FileName, format);
                        bitmap.Dispose();
                    }
                }
            }

            // Xử lý đóng kết nối TCP (áp dụng cho cả host và client)
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
                    // Bắt lỗi nếu có sự cố khi đóng kết nối
                    Console.WriteLine("Error closing connection: " + ex.Message);
                }
            }
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
            
            
            btnSave.BringToFront();
            btn_Pen.BringToFront();
            btn_eraser.BringToFront();
            
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
                // Đảm bảo currentDrawing được khởi tạo
                if (currentDrawing == null)
                {
                    currentDrawing = new Drawing(); // Thay `Drawing` bằng kiểu thực tế của `currentDrawing`
                    currentDrawing.Points = new List<Point>();
                }

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
        private  void PanelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            
            Debug.Write("\n");
            if (isDrawing)
            {
                isDrawing = false;
                if (currentDrawing != null)
                {
                    // Chuyển nét vẽ hiện tại thành JSON và gửi tới server
                    string jsonData = JsonConvert.SerializeObject(currentDrawing);
                    SendPacket(2, clientName, clientIP, roomID, jsonData);

                    currentDrawing = null; // Đặt lại sau khi gửi
                }
                PanelDraw.Invalidate();
                Debug.WriteLine("Thông số nhả chuột:\n" + "isDrawing: " + isDrawing + "\n" + "isInDrawingMode: " + isInDrawingMode + "\n");
                
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
            saveDialog.Filter = "Tệp JPG|*.jpg";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // Tạo một bitmap mới với kích thước và màu nền giống với PanelDraw
                Bitmap bitmap = new Bitmap(PanelDraw.Width, PanelDraw.Height);

                // Vẽ nội dung lên bitmap
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Vẽ màu nền của PanelDraw
                    g.Clear(PanelDraw.BackColor);

                    // Vẽ các đối tượng trong stickyNotes
                    foreach (var note in stickyNotes)
                    {
                        using (Brush brush = new SolidBrush(note.BackgroundColor))
                        {
                            g.FillRectangle(brush, new Rectangle(note.LocationPoint, note.NoteSize));
                        }

                        // Vẽ nội dung ghi chú
                        using (Font font = new Font("Arial", 13, FontStyle.Regular))
                        using (Brush textBrush = new SolidBrush(Color.Black))
                        {
                            StringFormat stringFormat = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            g.DrawString(note.NoteText, font, textBrush,
                                new RectangleF(note.LocationPoint.X, note.LocationPoint.Y, note.NoteSize.Width, note.NoteSize.Height),
                                stringFormat);
                        }
                    }

                    // Vẽ các đối tượng trong imageList
                    foreach (var img in imageList)
                    {
                        if (!string.IsNullOrEmpty(img.content))
                        {
                            var decodedImage = Image.DecodeBase64ToBitmap(img.content);
                            g.DrawImage(decodedImage, new Rectangle(img.Position, img.ImageSize));
                        }
                    }

                    // Vẽ các đường nét từ drawingBitmap
                    if (drawingBitmap != null)
                    {
                        g.DrawImage(drawingBitmap, 0, 0);
                    }
                }

                // Lưu bitmap ra tệp
                ImageFormat format = saveDialog.FileName.EndsWith(".jpg") ? ImageFormat.Jpeg : ImageFormat.Png;
                bitmap.Save(saveDialog.FileName, format);

                // Giải phóng bộ nhớ
                bitmap.Dispose();

                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // Tải ảnh từ đường dẫn
                System.Drawing.Image loadedImage = new Bitmap(openDialog.FileName);

                // Tạo PictureBox với hình ảnh đã tải
                PictureBox pictureBox = new PictureBox
                {
                    Image = loadedImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = loadedImage.Width, // Giới hạn chiều rộng theo panel
                    Height = loadedImage.Height, // Giới hạn chiều cao theo panel
                    Location = new Point(visibleX, visibleY),
                };

                // Đảm bảo PictureBox nằm trong giới hạn của Panel
                if (pictureBox.Right > PanelDraw.Width)
                {
                    pictureBox.Left = PanelDraw.Width - pictureBox.Width;
                }

                if (pictureBox.Bottom > PanelDraw.Height)
                {
                    pictureBox.Top = PanelDraw.Height - pictureBox.Height;
                }

                // Thêm ảnh vào danh sách
                var newImage = new Image(-1, pictureBox.Location, pictureBox.Size, loadedImage);
                current_image = newImage;

                // Chuyển đối tượng Image thành JSON
                string jsonImage = JsonConvert.SerializeObject(newImage);

                // Gửi packet tới server
                SendPacket(3, clientName, clientIP, roomID, jsonImage);

                // Thêm PictureBox vào panel
                PanelDraw.Controls.Add(pictureBox);

                // Đăng ký sự kiện chuột cho PictureBox
                pictureBox.MouseDown += PictureBox_MouseDown;
                pictureBox.MouseMove += PictureBox_MouseMovePictureBox;
                pictureBox.MouseUp += PictureBox_MouseUpPictureBox;
                pictureBox.MouseWheel += PictureBox_MouseWheel;
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

                // Đảm bảo PictureBox nằm trong PanelDraw
                if (newX < 0) newX = 0;
                if (newY < 0) newY = 0;
                if (newX + pictureBox.Width > PanelDraw.Width) newX = PanelDraw.Width - pictureBox.Width;
                if (newY + pictureBox.Height > PanelDraw.Height) newY = PanelDraw.Height - pictureBox.Height;

                // Cập nhật vị trí mới
                pictureBox.Left = newX;
                pictureBox.Top = newY;
            }
        }

        //Nhả thôi không đồng bộ chuột và ảnh nữa
        private void PictureBox_MouseUpPictureBox(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;

            // Sử dụng ImageID từ Tag
            int imageID = (int)pictureBox.Tag;
            var image = imageList.FirstOrDefault(img => img.ImageID == imageID);

            if (image != null)
            {
                // Cập nhật vị trí mới
                image.UpdatePosition(pictureBox.Location);

                // Mã hóa đối tượng thành JSON
                string jsonImage = JsonConvert.SerializeObject(image);

                // Gửi gói tin cập nhật tới server
                SendPacket(4, clientName, clientIP, roomID, jsonImage);
                Debug.WriteLine($"Gói tin đã gửi (di chuyển): {jsonImage}");
            }
            else
            {
                Debug.WriteLine($"Không tìm thấy Image với ImageID {imageID}");
            }
            isResizing = false;
        }
        
        //Lăn chuột thu phóng ảnh
        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;

            int zoomAmount = 30;
            int newWidth = pictureBox.Width + (e.Delta > 0 ? zoomAmount : -zoomAmount);
            int newHeight = pictureBox.Height + (e.Delta > 0 ? zoomAmount : -zoomAmount);

            if (newWidth > PanelDraw.Width) newWidth = PanelDraw.Width;
            if (newHeight > PanelDraw.Height) newHeight = PanelDraw.Height;
            if (newWidth < 10) newWidth = 10;
            if (newHeight < 10) newHeight = 10;

            pictureBox.Size = new Size(newWidth, newHeight);

            int imageID = (int)pictureBox.Tag;
            var image = imageList.FirstOrDefault(img => img.ImageID == imageID);

            if (image != null)
            {
                image.UpdateSize(pictureBox.Size);

                string jsonImage = JsonConvert.SerializeObject(image);
                SendPacket(4, clientName, clientIP, roomID, jsonImage);
                Debug.WriteLine($"Gói tin đã gửi (thu phóng): {jsonImage}");
            }
            else
            {
                Debug.WriteLine($"Không tìm thấy Image với ImageID {imageID}");
            }
        }
        #endregion
        #region Kéo thả Panel
        //------------------------------Kéo thả Panel-------------------------------------


        private void panelMovable_MouseDown(object sender, MouseEventArgs e)
        {
            if (beForced == true) return;
            if (e.Button == MouseButtons.Right)
            {
                isDragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = PanelDraw.Location;
            }
        }

        private void panelMovable_MouseMove(object sender, MouseEventArgs e)
        {
            if (beForced == true) return;
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
            if (beForced == true) return;
            isDragging = false; // Kết thúc kéo
            if (isForce == true)
            {
                // Lấy tọa độ hiện tại của góc trên bên trái của form
                Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
                Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
                int visibleX = formOriginInScreen.X - panelOriginInForm.X;
                int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
                // Đặt văn bản cho label với tọa độ của form
                lbPosition.Text = $"{visibleX}, {visibleY}";
                // Serialize nội dung của lbPosition.Text trực tiếp thành JSON
                string jsonLocation = JsonConvert.SerializeObject(lbPosition.Text);

                // Gửi gói tin với nội dung JSON hóa
                SendPacket(9, clientName, clientIP, roomID, jsonLocation);
            }
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
                    // Xác định tọa độ hiển thị trong panel
                    Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
                    Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
                    int visibleX = formOriginInScreen.X - panelOriginInForm.X;
                    int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
                    
                    int One, Two;
                    // Thử chuyển đổi giá trị từ `tbNoteOne.Text` và `tbNoteTwo.Text` sang số nguyên
                    if (!int.TryParse(tbNoteOne.Text, out One))
                    {
                        One = 150;
                    }

                    if (!int.TryParse(tbNoteTwo.Text, out Two))
                    {
                        Two = 150;
                    }
                    // Lấy kích thước của panel và ghi chú
                    int panelCenterX = PanelDraw.Width / 2; // Tọa độ X trung tâm của panel
                    int panelCenterY = PanelDraw.Height / 2; // Tọa độ Y trung tâm của panel
                    int stickyNoteX = panelCenterX - (One / 2); // Đảm bảo phần tử nằm chính giữa X của panel
                    int stickyNoteY = panelCenterY - (Two / 2); // Đảm bảo phần tử nằm chính giữa Y của panel
                     // Chuyển tọa độ trung tâm sang không gian form
                    int finalX = stickyNoteX + offsetX;
                    int finalY = stickyNoteY + offsetY;
                    // Tạo sticky_note logic
                    sticky_note newStickyNote = new sticky_note
                    {
                        BackgroundColor = colorDialog.Color,
                        LocationPoint = new Point(finalX,finalY),
                        NoteSize = new Size(One, Two),
                        NoteText = "",
                        noteID = -1
                    };

                    // Tạo TextBox để hiển thị trên giao diện
                    TextBox stickyTextBox = new TextBox
                    {
                        Multiline = true,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = newStickyNote.BackgroundColor,
                        Location = newStickyNote.LocationPoint,
                        Size = newStickyNote.NoteSize,
                        Text = newStickyNote.NoteText,
                        TextAlign = HorizontalAlignment.Center,
                        Font = new Font("Arial", 13, FontStyle.Regular),
                        Tag = -1
                    };

                    // Cờ để kiểm soát hành động kéo thả
                    bool isDragging = false;
                    Point initialCursorPosition = Point.Empty; // Tọa độ chuột ban đầu

                    // Gắn sự kiện chuột để kéo thả
                    stickyTextBox.MouseDown += (s, ev) =>
                    {
                        if (ev.Button == MouseButtons.Left)
                        {
                            // Bắt đầu kéo thả
                            isDragging = true;
                            initialCursorPosition = ev.Location;
                        }
                    };

                    stickyTextBox.MouseMove += (s, ev) =>
                    {
                        if (isDragging && ev.Button == MouseButtons.Left)
                        {
                            // Tính toán vị trí mới của TextBox
                            Point newLocation = new Point(
                                stickyTextBox.Left + ev.X - initialCursorPosition.X,
                                stickyTextBox.Top + ev.Y - initialCursorPosition.Y
                            );
                            stickyTextBox.Location = newLocation;

                            // Tìm sticky_note tương ứng và cập nhật thông tin
                            sticky_note sticky = stickyNotes.Find(note => note.noteID == (int)stickyTextBox.Tag);
                            if (sticky != null)
                            {
                                sticky.UpdateLocation(newLocation);
                            }
                        }
                    };

                    stickyTextBox.MouseUp += (s, ev) =>
                    {
                        if (isDragging && ev.Button == MouseButtons.Left)
                        {
                            // Kết thúc kéo thả
                            isDragging = false;

                            // Tìm sticky_note tương ứng và gửi gói tin sau khi thả chuột
                            sticky_note sticky = stickyNotes.Find(note => note.noteID == (int)stickyTextBox.Tag);
                            if (sticky != null)
                            {
                                // Serialize sticky_note thành JSON và gửi gói tin
                                string jsonnewNote = JsonConvert.SerializeObject(sticky);
                                SendPacket(6, clientName, clientIP, roomID, jsonnewNote);
                                Debug.WriteLine("Gửi gói 6, di chuyển hoàn tất");
                            }
                        }
                    };

                    // Gắn sự kiện để cập nhật nội dung
                    stickyTextBox.TextChanged += (s, ev) =>
                    {
                        // Nếu đang kéo thả, bỏ qua việc cập nhật nội dung
                        if (isDragging)
                            return;

                        // Tìm sticky_note tương ứng và cập nhật thông tin
                        sticky_note sticky = stickyNotes.Find(note => note.noteID == (int)stickyTextBox.Tag);
                        if (sticky != null)
                        {
                            sticky.UpdateText(stickyTextBox.Text);

                            // Serialize sticky_note thành JSON và gửi gói tin
                            string jsonnewNote = JsonConvert.SerializeObject(sticky);
                            SendPacket(6, clientName, clientIP, roomID, jsonnewNote);
                            Debug.WriteLine("Gửi gói 6, text");
                        }
                    };


                    // Thêm TextBox vào Panel
                    PanelDraw.Controls.Add(stickyTextBox);
                    stickyTextBox.BringToFront();
                    // Gán current_note
                    current_note = newStickyNote;

                    // Gán Tag cho TextBox bằng với noteID mặc định (-1)
                    stickyTextBox.Tag = current_note.noteID;
                    // Serialize sticky_note thành JSON để gửi đi
                    string jsonNote = JsonConvert.SerializeObject(newStickyNote);
                    SendPacket(5, clientName, clientIP, roomID, jsonNote);
                    Debug.WriteLine($"Đã gửi gói 5: {jsonNote}");
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
                        if (pen != null) // Nếu bút không phải màu nền (tức là đang vẽ)
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
        void stop_be_Forced(Packet response)
        {
            beForced = false;
            btnTpToPosition.Enabled = true;
            // Phân tách chuỗi tọa độ "x,y"
            var coordinates = lbPosition.Text.Split(',');
            if (coordinates.Length == 2 &&
                int.TryParse(coordinates[0].Trim(), out int x) &&
                int.TryParse(coordinates[1].Trim(), out int y))
            {
                // Cập nhật tọa độ của PanelDraw
                PanelDraw.Location = new Point(-x, -y);
                Debug.WriteLine($"Cập nhật PanelDraw đến tọa độ: {x}, {y}");
            }
        }
        void be_in_Forced(Packet response)
        {
            
            try
            {
                if (beForced == false)
                {
                    // Lấy tọa độ hiện tại của góc trên bên trái của form
                    Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
                    Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
                    int visibleX = formOriginInScreen.X - panelOriginInForm.X;
                    int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
                    // Đặt văn bản cho label với tọa độ của form
                    lbPosition.Text = $"{visibleX}, {visibleY}";
                    beForced = true;
                }
                
                btnTpToPosition.Enabled = false;
                // Kiểm tra và xử lý dữ liệu từ response.DrawingData
                if (!string.IsNullOrEmpty(response.DrawingData))
                {
                    // Gọi hàm View_to_view để xử lý chuỗi JSON và thay đổi tọa độ
                    View_to_view(response.DrawingData);
                }
                else
                {
                    Debug.WriteLine("Dữ liệu DrawingData trống hoặc không hợp lệ.");
                }
            }
            catch
            {
                Debug.WriteLine("Lỗi bị ép");
            }
        }
        void View_to_view(string a)
        {
            try
            {
                if (!string.IsNullOrEmpty(a))
                {
                    // Deserialize chuỗi JSON để lấy tọa độ
                    string positionText = JsonConvert.DeserializeObject<string>(a);

                    if (!string.IsNullOrEmpty(positionText))
                    {
                        

                        // Phân tách chuỗi tọa độ "x,y"
                        var coordinates = positionText.Split(',');
                        if (coordinates.Length == 2 &&
                            int.TryParse(coordinates[0].Trim(), out int x) &&
                            int.TryParse(coordinates[1].Trim(), out int y))
                        {
                            // Cập nhật tọa độ của PanelDraw
                            PanelDraw.Location = new Point(-x, -y);
                            Debug.WriteLine($"Cập nhật PanelDraw đến tọa độ: {x}, {y}");
                        }
                        else
                        {
                            Debug.WriteLine("Chuỗi tọa độ không hợp lệ.");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Dữ liệu đầu vào View_to_view trống.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong View_to_view: {ex.Message}");
            }
        }
        
        //Kéo giật tọa độ theo label
        private async void btnTpToPosition_Click(object sender, EventArgs e)
        {
            // Lấy tọa độ hiện tại của góc trên bên trái của form
            Point panelOriginInForm = PanelDraw.PointToScreen(new Point(0, 0));
            Point formOriginInScreen = this.PointToScreen(new Point(0, 0));
            int visibleX = formOriginInScreen.X - panelOriginInForm.X;
            int visibleY = formOriginInScreen.Y - panelOriginInForm.Y;
            // Đặt văn bản cho label với tọa độ của form
            lbPosition.Text = $"{visibleX}, {visibleY}";


            if(isForce==false)
            {
                // Trạng thái ban đầu: "Force View"
                if (!string.IsNullOrEmpty(lbPosition.Text))
                {
                    // Serialize nội dung của lbPosition.Text trực tiếp thành JSON
                    string jsonLocation = JsonConvert.SerializeObject(lbPosition.Text);

                    // Gửi gói tin với nội dung JSON hóa
                    await SendPacketAsync(9, clientName, clientIP, roomID, jsonLocation);

                    Debug.WriteLine($"Gửi gói tin 9: {jsonLocation}");

                    // Đổi trạng thái button và biến cờ
                    btnTpToPosition.Text = "Cancel";
                    isForce = true;
                }
                else
                {
                    MessageBox.Show("Label không có vị trí để teleport.");
                }
            }
            else if(isForce==true)
            {
                // Trạng thái khi "Cancel" được nhấn
                // Gửi gói tin Release
                await SendPacketAsync(10, clientName, clientIP, roomID, "Release");
                Debug.WriteLine("Gửi gói tin 10: Release");

                // Đổi trạng thái button và biến cờ
                btnTpToPosition.Text = "Force View";
                isForce = false;
            }
        }
        #endregion
    }
}
