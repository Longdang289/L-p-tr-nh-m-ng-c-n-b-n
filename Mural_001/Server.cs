
using System;
using System.Collections.Generic;
using System.Net.Sockets; 
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms; 
using Newtonsoft.Json; 
using System.Net.NetworkInformation; 
using Mural_001; 

namespace Mural_001
{
    
    public partial class Server : Form
    {
        private List<Room> roomList = new List<Room>();//Quản lý các phòng dc mở ra
        
        private List<User> userList = new List<User>();//Quản lý các user dc Accept
        //TcpListener để lắng nghe các kết nối TCP từ client.
        private TcpListener listener;
        // Đối tượng Manager để quản lý ghi log, cập nhật thông tin phòng và người dùng.
        private Manager Manager;

        public Server()
        {
            InitializeComponent();
            //Gọi constructor Manager truyền listview và 2 textbox vô
            Manager = new Manager(lsvOne, tbRoom, tbUser);
        }

        // Sự kiện khi nút "Start" được nhấn.
        private async void btnStart_Click(object sender, EventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, 9999); // Khởi tạo TcpListener để lắng nghe trên mọi IP và port 9999.
            listener.Start(); // Bắt đầu lắng nghe kết nối từ client.

            Manager.WriteToLog("Start listening for incoming requests...");
            // Ghi log thông báo bắt đầu lắng nghe vào lsvOne kèm ngày giờ (xem file Manager DEF)
            this.btnStart.Enabled = false; // Vô hiệu hóa nút "Start" để khỏi spam luồng
            this.btnStop.Enabled = true; // Kích hoạt nút "Stop" để người dùng có thể dừng server.

            await ListenAsync(); // Bắt đầu phương thức bất đồng bộ lắng nghe kết nối.
        }

        // Sự kiện khi nút "Stop" được nhấn.
        private void btnStop_Click(object sender, EventArgs e)
        {
            Manager.WriteToLog("Stop listening for incoming requests");
            // Ghi log thông báo dừng lắng nghe lên lsvOne với ngày giờ
            foreach (User user in userList) // Lặp qua danh sách người dùng hiện có.
            {
                user.Client.Close(); // Đóng kết nối cho mỗi người dùng.
            }
            listener.Stop(); // Dừng TcpListener, không lắng nghe kết nối mới nữa.

            this.btnStop.Enabled = false; // Vô hiệu hóa nút "Stop".
            this.btnStart.Enabled = true; // Kích hoạt lại nút "Start" để có thể khởi động lại server.
        }

        // Sự kiện khi nút "Server IP" được nhấn.
        private void btnServerIP_Click(object sender, EventArgs e)
        {
            //Gọi hàm của class Manager để lấy được IPv4 ghi lên textbox
            tbServerIP.Text = Manager.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
        }

        // Phương thức lắng nghe kết nối từ client (bất đồng bộ)( xài khi ấn Start)
        private async Task ListenAsync()
        {
            try
            {
                while (true)
                {
                    // Chấp nhận kết nối từ client (bất đồng bộ) và trả về đối tượng TcpClient.
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    // Bắt đầu một tác vụ mới để xử lý từng client.
                    _ = Task.Run(() => ReceiveAsync(client));
                }
            }
            catch
            {
                // Trong trường hợp gặp lỗi, khởi động lại listener.

                Manager.WriteToLog("Try to hear again");
                listener = new TcpListener(IPAddress.Any, 9999);
                listener.Start();
            }
        }

        // Phương thức nhận và xử lý dữ liệu từ client (bất đồng bộ).
        //Tham số truyền vào constructor chính là một kết nối để khai thác luồng ghi đọc
        //Xem thêm ở file User DEF
        private async Task ReceiveAsync(TcpClient client)
        {
            User user = new User(client); // Tạo đối tượng `User` mới từ kết nối của client.
            userList.Add(user); // Thêm người dùng vào danh sách userList.

            try
            {
                string requestInJson = string.Empty; // Chuỗi để lưu yêu cầu dưới dạng JSON.
                while (true)
                {

                    /*
                     *Về JSON, nôm na nó là string, là một đoạn văn bản đặt trong {}
                     *Kiểu string dễ tải đi trong mạng giống như HTTP
                     *Tuy kiểu nhị phân (BinaryFormatter- chuyển mọi thứ về 0,1 thay vì văn bản)
                     *tiết kiệm dung lượng hơn nhưng có ngôn ngữ không hiểu được
                     *Còn string thì chắc chắn là ngôn ngữ nào cũng hiểu
                     *JSON không tự nó chunking được, nó phải gộp hết đủ thì nó mới giải mã được
                     *Về cái này thì nó phế hơn BinaryFormatter
                     *Mình phải tự làm cơ chế chunking nếu muốn gửi dữ liệu lớn
                     *Nếu không thì tự cái dung lượng gói tin là đủ nghẽn rồi, Asyn cx cứu ko nổi
                     *Tạm thời chúng ta sẽ chưa tính đến cái này, có sao xài vậy đi
                     */

                    // Đọc dữ liệu từ client (bất đồng bộ).
                    requestInJson = await user.Reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(requestInJson)) break; // Thoát vòng lặp nếu không có dữ liệu.

                    // Chuyển đổi dữ liệu JSON mới nhận thành đối tượng Packet
                    //Chính là việc dịch string JSON lại thành thứ nó đã từng(Packet)
                    Packet request = JsonConvert.DeserializeObject<Packet>(requestInJson);

                    // Kiểm tra mã yêu cầu và gọi hàm xử lý tương ứng.
                    switch (request.Code)
                    {
                        case 0: // Mã 0: Tạo phòng.
                            await generate_room_handler_async(user, request);
                            break;
                        case 1: // Mã 1: Tham gia phòng.
                            await join_room_handler_async(user, request);
                            break;
                            /*
                             * Về bitmap, nó là ma trận, là tọa độ một đống điểm ảnh pixel 
                             * Nghĩa là dữ liệu đống tọa độ đó để miêu tả một cái hình ảnh
                             */
                        case 2: // Mã 2: Đồng bộ bitmap.
                            await sync_bitmap_handler_async(user, request);
                            break;
                        case 3: // Mã 3: Gửi bitmap.
                            await send_bitmap_handler_async(user, request);
                            break;
                        case 4: // Mã 4: Gửi đồ họa.
                            await send_graphics_handler_async(user, request);
                            break;
                    }
                }
            }
            catch
            {
                Manager.WriteToLog("Error in receiving");
                close_client(user); // Đóng kết nối của client nếu gặp lỗi.
            }
        }

        // Phương thức xử lý yêu cầu tạo phòng từ client (bất đồng bộ). Tức case 0
        private async Task generate_room_handler_async(User user, Packet request)
        {
            user.Username = request.Username; // Cập nhật tên người dùng từ yêu cầu.

            Random r = new Random(); // Tạo đối tượng Random để tạo ID phòng.
            int roomID = r.Next(1000, 9999); // Tạo ID phòng ngẫu nhiên 4 số cho đẹp
            Room newRoom = new Room { roomID = roomID }; // Tạo đối tượng Room mới với ID phòng.

            newRoom.userList.Add(user); // Thêm người đòi tạo vào danh sách người dùng của phòng.
            roomList.Add(newRoom); // Thêm phòng mới vào danh sách phòng.

            Manager.WriteToLog(user.Username + " created new room. Room code: " + newRoom.roomID); // Ghi log về việc tạo phòng.
            Manager.UpdateRoomCount(roomList.Count); // Cập nhật số lượng phòng.
            Manager.UpdateUserCount(userList.Count); // Cập nhật số lượng người dùng.

            // Tạo gói tin thông báo phòng mới cho người dùng.
            Packet message = new Packet
            {
                Code = 0, // Mã 0: Tạo phòng.
                Username = request.Username, // Gửi lại tên người dùng.
                RoomID = roomID.ToString() // Gửi lại ID phòng.
            };

            await sendSpecificAsync(user, message); // Gửi gói tin đến người dùng đã tạo phòng.
        }

        // Phương thức xử lý yêu cầu tham gia phòng từ client (bất đồng bộ).
        private async Task join_room_handler_async(User user, Packet request)
        {
            bool roomExist = false; // Biến kiểm tra phòng có tồn tại hay không.
            int id = int.Parse(request.RoomID.ToString()); // Lấy ID phòng từ yêu cầu.
            Room requestingRoom = null; // Biến sẽ chọn phòng nào phù hợp để gán vào và đưa người join vô

            // Tìm phòng theo ID trong danh sách phòng.
            foreach (Room room in roomList)
            {
                if (room.roomID == id)
                {
                    requestingRoom = room;
                    roomExist = true;
                    break;
                }
            }

            if (!roomExist) // Nếu phòng không tồn tại.
            {
                request.Username = "error :this room does not exist"; // Gửi thông báo lỗi.
                await sendSpecificAsync(user, request); // Gửi thông báo lỗi đến client.
                return;
            }

            user.Username = request.Username; // Cập nhật tên người dùng.
            requestingRoom.userList.Add(user); // Thêm người dùng vào danh sách của phòng.

            request.Username = requestingRoom.GetUsernameListInString(); // Lấy danh sách tên người dùng trong phòng.
            // Gửi thông báo cho tất cả người dùng trong phòng về người mới.
            foreach (User _user in requestingRoom.userList)
            {
                await sendSpecificAsync(_user, request);
            }

            Manager.WriteToLog("Room " + request.RoomID + ": " + user.Username + " joined"); // Ghi log về việc tham gia phòng.
            Manager.UpdateUserCount(userList.Count); // Cập nhật số lượng người dùng.
        }

        // Phương thức xử lý yêu cầu đồng bộ bitmap từ client (bất đồng bộ).
        private async Task sync_bitmap_handler_async(User user, Packet request)
        {
            // Lấy phòng từ ID yêu cầu.
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID));
            if (targetRoom != null)
            {
                foreach (User _user in targetRoom.userList)
                {
                    if (_user != user) // Gửi dữ liệu đồng bộ cho tất cả người dùng khác trong phòng.
                    {
                        await sendSpecificAsync(_user, request);
                    }
                }
            }
        }

        // Phương thức xử lý yêu cầu gửi bitmap từ client (bất đồng bộ).
        private async Task send_bitmap_handler_async(User user, Packet request)
        {
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID)); // Lấy phòng theo ID.
            if (targetRoom != null)
            {
                // Gửi bitmap cho tất cả người dùng trong phòng, trừ người gửi.
                foreach (User _user in targetRoom.userList)
                {
                    if (_user != user)
                    {
                        await sendSpecificAsync(_user, request);
                    }
                }
            }
        }

        // Phương thức xử lý yêu cầu gửi đồ họa từ client (bất đồng bộ).
        private async Task send_graphics_handler_async(User user, Packet request)
        {
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID)); // Lấy phòng theo ID.
            if (targetRoom != null)
            {
                // Gửi dữ liệu đồ họa cho tất cả người dùng trong phòng, trừ người gửi.
                foreach (User _user in targetRoom.userList)
                {
                    if (_user != user)
                    {
                        await sendSpecificAsync(_user, request);
                    }
                }
            }
        }

        // Phương thức đóng kết nối của người dùng.
        private void close_client(User user)
        {
            Manager.WriteToLog("Connection closed: " + user.Username); // Ghi log về việc ngắt kết nối.
            user.Client.Close(); // Đóng kết nối.
            userList.Remove(user); // Xóa người dùng khỏi danh sách.
            Manager.UpdateUserCount(userList.Count); // Cập nhật số lượng người dùng.
        }

        // Phương thức gửi gói tin đến người dùng cụ thể (bất đồng bộ).
        private async Task sendSpecificAsync(User user, Packet message)
        {
            string json = JsonConvert.SerializeObject(message); // Chuyển đổi gói tin thành JSON.
            await user.Writer.WriteLineAsync(json); // Gửi dữ liệu JSON qua mạng.
            await user.Writer.FlushAsync(); // Đảm bảo dữ liệu được gửi ngay lập tức.
        }
    }
}
