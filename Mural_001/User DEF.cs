using System.IO;
using System.Net.Sockets;

namespace Mural_001;


public class User// Đại diện cho một người dùng, có thể gửi và nhận dữ liệu qua TCP
{
    
    public TcpClient Client { get; set; }// Client đại diện cho kết nối giữa server và client

    public string Username { get; set; }// Username để lưu tên người dùng
           // (ban đầu là chuỗi rỗng, sau đó sẽ được gán khi client gửi thông tin đăng nhập)

    public StreamReader Reader { get; set; }//Reader sử dụng StreamReader để đọc dữ liệu từ kết nối (luồng vào)

    public StreamWriter Writer { get; set; }//Writer sử dụng StreamWriter để gửi dữ liệu qua kết nối (luồng ra)

    
    public User(TcpClient client)// Constructor tham số là TcpClient và khởi tạo các thành phần
    {
        // Gán kết nối TCP vào thuộc tính Client của đối tượng User
        this.Client = client;

        // Khởi tạo tên người dùng ban đầu là chuỗi rỗng
        this.Username = string.Empty;

        // Lấy NetworkStream từ TcpClient (NetworkStream là luồng dữ liệu của kết nối TCP)
        NetworkStream stream = Client.GetStream();

        // Khởi tạo StreamReader để đọc dữ liệu từ luồng của TcpClient
        this.Reader = new StreamReader(stream);

        // Khởi tạo StreamWriter để ghi dữ liệu ra luồng của TcpClient
        this.Writer = new StreamWriter(stream);
    }
}
