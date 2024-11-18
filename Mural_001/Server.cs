
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
            tbRoom.Enabled = false;
            tbUser.Enabled = false;
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

                    // Ghi log khi có client kết nối thành công
                    Manager.WriteToLog("Co client moi tham gia");

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

                    // Đọc dữ liệu từ client (bất đồng bộ).
                    requestInJson = await user.Reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(requestInJson)) break; // Thoát vòng lặp nếu không có dữ liệu.

                    // Chuyển đổi dữ liệu JSON mới nhận thành đối tượng Packet
                    //Chính là việc dịch string JSON lại thành thứ nó đã từng(Packet)
                    Packet request = JsonConvert.DeserializeObject<Packet>(requestInJson);
                    Manager.WriteToLog($"Received Code: {request.Code}, RoomID: {request.RoomID}, String: {request.DrawingData}");
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
                        case 3: // Mã 3: Nhận ảnh mới.
                            await receive_new_image(user, request);
                            break;
                        case 4: // Mã 4: Nhận thay đổi size, tọa độ ảnh.
                            await update_image(user, request);
                            break;
                        case 5: //Mã 5: Nhận stickynote
                            await receive_note(user, request);
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
            //newRoom.currentbitmap = request.BitmapString;
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
            Console.WriteLine("Sent packet to client: " + JsonConvert.SerializeObject(message));
            await sendSpecificAsync(user, message); // Gửi gói tin đến người dùng đã tạo phòng.
        }

        // Phương thức xử lý yêu cầu tham gia phòng từ client (bất đồng bộ).
        private async Task join_room_handler_async(User user, Packet request)
        {
            bool roomExist = false; // Biến kiểm tra phòng có tồn tại hay không.
            if (!int.TryParse(request.RoomID, out int id))
            {
                Manager.WriteToLog("Invalid RoomID format");
                return;
            }
            
            Room requestingRoom=null; // Biến sẽ chọn phòng nào phù hợp để gán vào và đưa người join vô

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
                request.Code = 404; // Gửi thông báo lỗi.
                await sendSpecificAsync(user, request); // Gửi thông báo lỗi đến client.
                return;
            }

            user.Username = request.Username; // Cập nhật tên người dùng.
            requestingRoom.userList.Add(user); // Thêm người dùng vào danh sách của phòng.
            
            request.Username = requestingRoom.GetUsernameListInString(); // Lấy danh sách tên người dùng trong phòng.
            // Gửi thông báo cho tất cả người dùng trong phòng về người mới.
            //request.BitmapString = requestingRoom.currentbitmap;//Đồng bộ cho người mới
            //await sendSpecificAsync(user, request);//Gửi đồng bộ
            Manager.WriteToLog("Room " + request.RoomID + ": " + user.Username + " joined"); // Ghi log về việc tham gia phòng.
            Manager.UpdateUserCount(userList.Count); // Cập nhật số lượng người dùng.
            request.DrawingData = requestingRoom.GetDrawingsAsJson();
            request.Code = 1;
            await sendSpecificAsync(user, request);
            Manager.WriteToLog("Đã gửi nét: code: " + request.Code);
            request.DrawingData = requestingRoom.GetImagesAsJson();
            request.Code = 7;
            await sendSpecificAsync(user, request);
            Manager.WriteToLog("Đã gửi ảnh: code: " + request.Code);
            return;
        }
        // Phương thức xử lý yêu cầu đồng bộ bitmap từ client (bất đồng bộ).
        private async Task sync_bitmap_handler_async(User user, Packet request)
        {
            // Lấy phòng từ ID yêu cầu.
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID));
            if (targetRoom != null)
            {
                if (request.DrawingData != null)
                {
                    // Thêm nét vẽ mới vào danh sách Drawing của phòng
                    targetRoom.AddNewDrawingByJSON(request.DrawingData);
                    Manager.WriteToLog("Đã gửi gói cập nhật đồng bộ nét vẽ");

                    if (targetRoom.userList.Count > 1)
                    {
                        foreach (User _user in targetRoom.userList)
                        {
                            if (_user != user) // Gửi dữ liệu đồng bộ cho tất cả người dùng khác trong phòng.
                            {
                                await sendSpecificAsync(_user, request);
                            }
                            else continue;
                        }
                    }
                    else return;
                }
            }
        }

        // Phương thức 
        private async Task receive_new_image(User user, Packet request)
        {
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID)); // Lấy phòng theo ID.
            if (targetRoom != null)
            {
                Image newImage = JsonConvert.DeserializeObject<Image>(request.DrawingData);
                targetRoom.AddImage(newImage);
                newImage.ImageID = targetRoom.imageCount;
                request.DrawingData= JsonConvert.SerializeObject(newImage);
                foreach (User _user in targetRoom.userList)
                {
                    await sendSpecificAsync(_user, request);
                }
            }
        }

        // Phương thức 
        private async Task update_image(User user, Packet request)
        {
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID)); // Lấy phòng theo ID.
            if (targetRoom != null)
            {
                targetRoom.ImageUpdate(request.DrawingData);
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

        private async Task receive_note(User user, Packet request)
        {
            Room targetRoom = roomList.Find(r => r.roomID == int.Parse(request.RoomID)); // Lấy phòng theo ID.
            if (targetRoom != null)
            {
                sticky_note note = JsonConvert.DeserializeObject<sticky_note>(request.DrawingData);
                targetRoom.AddNote(note);
                note.noteID = targetRoom.noteCount;
                request.DrawingData = JsonConvert.SerializeObject(note);
                request.Code = 5;
                foreach (User _user in targetRoom.userList)
                {
                    await sendSpecificAsync(_user, request);
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
