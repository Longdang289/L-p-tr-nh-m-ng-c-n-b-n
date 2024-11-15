using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mural_001;
public class Room // Đại diện cho một phòng, chứa nhiều người dùng
{
    public int roomID; // ID mỗi phòng
    public List<User> userList = new List<User>(); // Danh sách các người dùng hiện tại trong phòng
    public List<Drawing> Drawings { get; private set; } // Danh sách các nét vẽ
    public string jsondrawlist; // Chuỗi JSON lưu trữ toàn bộ danh sách Drawing

    public Room()
    {
        Drawings = new List<Drawing>();
        jsondrawlist = null; // Ban đầu, jsondrawlist được đặt thành null nếu danh sách Drawing trống
    }

    // Thêm một nét vẽ vào danh sách và cập nhật jsondrawlist
    public void AddDrawing(Drawing drawing)
    {
        Drawings.Add(drawing);
        UpdateJsonDrawList(); // Cập nhật jsondrawlist mỗi khi thêm Drawing mới
    }
    public void AddNewDrawingByJSON(string a)
    {
        Drawing newDrawing = JsonConvert.DeserializeObject < Drawing > (a);
        Drawings.Add(newDrawing);
        UpdateJsonDrawList(); // Cập nhật jsondrawlist mỗi khi thêm Drawing mới
    }
    // Cập nhật chuỗi JSON cho toàn bộ danh sách Drawings
    private void UpdateJsonDrawList()
    {
        jsondrawlist = Drawings.Count > 0 ? JsonConvert.SerializeObject(Drawings) : null;
    }

    // Phương thức để lấy chuỗi JSON hiện tại của danh sách các nét vẽ
    public string GetDrawingsAsJson()
    {
        return jsondrawlist;
    }

    public string GetUsernameListInString() // Trả về list tên người dùng trong phòng, ngăn cách bởi dấu phẩy
    {
        List<string> usernames = new List<string>();
        foreach (User user in userList)
        {
            usernames.Add(user.Username);
        }
        return string.Join(",", usernames.ToArray());
    }
}

