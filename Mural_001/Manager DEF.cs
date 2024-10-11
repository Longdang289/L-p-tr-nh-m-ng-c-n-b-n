using System; 
using System.Net.NetworkInformation; 
using System.Net.Sockets;
using System.Windows.Forms;

namespace Mural_001;

internal class Manager // Quản lý log server, thông tin phòng và người dùng.
{
    ListView Log; //Biến bí danh của cái listview thật (lsvOne)
    TextBox RoomCnt; // Biến bí danh của cái textbox tbRoom
    TextBox UserCnt; //Biến bí danh của cái textbox tbUser

    // Prototype để sau này truyền 3 cái kia vào
    public Manager(ListView log, TextBox room_count, TextBox user_count)
    {
        //Gán tham số vào biến thành viên nội bộ
        this.Log = log; 
        this.RoomCnt = room_count; 
        this.UserCnt = user_count;
    }
    /*
     * Mấy cái hàm( phương thức hành vi) dưới sẽ có Invoke, nghĩa là khi ma UI của thread chính thay đổi
     * (chỉ thread chính mới đổi dc UI thôi) nên nó phải Invoke nghĩa là nó biến cái nội dung hàm về 
     * thành một action trên thread chính ( InvokeRequired=true).
     * Nếu Invoke=false thì nó sẽ chạy lệnh bên dưới, ko cần gọi action lên thread chính
     * Thực ra dù có Invoke hay ko thì cái action nó y chang nhau, viết cho hợp lệ đỡ lỗi chồng chất thread
     */
    // Hàm ghi log thông tin vào ListView
    public void WriteToLog(string line)
    {
        // Kiểm tra nếu cần gọi Invoke (nếu UI được cập nhật từ thread khác).
        if (Log.InvokeRequired)
        {
            // Nếu cần Invoke, thực hiện cập nhật ListView từ thread chính.
            //Viết tham số hàm cho ngầu thôi chứ bình thường vẫn được, tức là cái new Action là cái lệnh kia
            Log.Invoke(new Action(() =>
            {
                // Thêm dòng log vào ListView với định dạng giờ phút và nội dung dòng.
                Log.Items.Add(string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm"), line));
            }));
        }
        else
        {
            // Nếu không cần Invoke, thêm trực tiếp vào ListView.
            Log.Items.Add(string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm"), line));
        }
    }

    // Hàm cập nhật số lượng phòng hiện tại vào TextBox RoomCnt
    public void UpdateRoomCount(int num)
    {
        // Kiểm tra nếu cần gọi Invoke (nếu UI được cập nhật từ thread khác).
        if (RoomCnt.InvokeRequired)
        {
            // Nếu cần Invoke, thực hiện cập nhật RoomCnt từ thread chính.
            RoomCnt.Invoke(new Action(() =>
            {
                // Cập nhật số lượng phòng vào TextBox RoomCnt dưới dạng chuỗi.
                RoomCnt.Text = num.ToString();
            }));
        }
        else
        {
            // Nếu không cần Invoke, cập nhật trực tiếp vào RoomCnt.
            RoomCnt.Text = num.ToString();
        }
    }

    // Hàm cập nhật số lượng người dùng hiện tại vào TextBox UserCnt
    public void UpdateUserCount(int num)
    {
        // Kiểm tra nếu cần gọi Invoke (nếu UI được cập nhật từ thread khác).
        if (UserCnt.InvokeRequired)
        {
            // Nếu cần Invoke, thực hiện cập nhật UserCnt từ thread chính.
            UserCnt.Invoke(new Action(() =>
            {
                // Cập nhật số lượng người dùng vào TextBox UserCnt dưới dạng chuỗi.
                UserCnt.Text = num.ToString();
            }));
        }
        else
        {
            // Nếu không cần Invoke, cập nhật trực tiếp vào UserCnt.
            UserCnt.Text = num.ToString();
        }
    }

    // Hàm hiển thị thông báo lỗi dưới dạng hộp thoại
    public void ShowError(string message)
    {
        // Hiển thị hộp thoại báo lỗi với nội dung message, tiêu đề "Error", nút OK, và biểu tượng lỗi.
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Hàm lấy địa chỉ IPv4 cục bộ dựa trên loại giao diện mạng
    public string GetLocalIPv4(NetworkInterfaceType type)
    {
        string localIPv4 = string.Empty; // Biến để lưu địa chỉ IPv4
        // Lặp qua tất cả các giao diện mạng (network interfaces) trên máy
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Kiểm tra nếu giao diện mạng phù hợp với loại giao diện được chỉ định và đang hoạt động (Up)
            if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
            {
                // Lặp qua tất cả các địa chỉ IP unicast của giao diện mạng này
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    // Nếu địa chỉ IP thuộc họ AddressFamily.InterNetwork (IPv4)
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Lưu địa chỉ IPv4
                        localIPv4 = ip.Address.ToString();
                    }
                }
            }
        }
        return localIPv4; // Trả về địa chỉ IPv4 cục bộ
    }
}
