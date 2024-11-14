using System.Collections.Generic;

namespace Mural_001;
public class Room// Đại diện cho một phòng, chứa nhiều người dùng
{
    public int roomID;// ID mỗi phòng

    public List<User> userList = new List<User>();// Danh sách các người dùng (User) hiện tại trong phòng

    public string currentbitmap;
    public string GetUsernameListInString()// Trả về list tên người dùng trong phòng, ngăn cách bởi dấu phẩy
    {
        // Danh sách tạm thời để lưu tên người dùng (User.Username) bên class User
        List<string> usernames = new List<string>();
        foreach (User user in userList)
        {
            usernames.Add(user.Username);
        }

        // Chuyển đổi danh sách usernames thành mảng chuỗi
        string[] s = usernames.ToArray();

        // Ghép nối các tên người dùng thành một chuỗi duy nhất, ngăn cách bởi dấu phẩy
        string res = string.Join(",", s);

        // Trả về chuỗi chứa tên của tất cả người dùng trong phòng
        return res;
    }
}
