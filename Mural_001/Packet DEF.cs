using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mural_001
{
    // Đại diện cho một gói dữ liệu
    public class Packet
    {
        public int Code { get; set; }// Mã xác định loại hành động và thông điệp

        public string Username { get; set; }// Tên người dùng gửi gói tin

        public string IP {  get; set; }//IP client gửi

        public string RoomID { get; set; }// ID của phòng mà người dùng đang tham gia
        
        // Chuỗi biểu diễn của bitmap,cái này chuyển ma trận pixel ra string qua bên kia chuyển lại ra thành ảnh
        public string BitmapString { get; set; }
        /*
        public string PenColor { get; set; }// Màu bút được người dùng chọn (trong trường hợp vẽ)

        public float PenWidth { get; set; }// Độ rộng của bút vẽ (trong trường hợp vẽ)

        public int ShapeTag { get; set; }// Mã định danh cho hình dạng vẽ (ví dụ: hình tròn, hình vuông)

        public List<Point> Points_1 { get; set; }// Danh sách các điểm điều khiển cho hình dạng đầu tiên (Points_1) trong bản vẽ
        public List<Point> Points_2 { get; set; }// Danh sách các điểm điều khiển cho hình dạng thứ hai (Points_2) trong bản vẽ
        
        // Vị trí của đối tượng, được lưu dưới dạng mảng tọa độ (x, y, z...)
        public float[] Position { get; set; }
        */
        // Constructor Packet
        public Packet(int code, string username, string ip, string roomID)
        {
            Code = code;
            Username = username;
            IP = ip;
            RoomID = roomID;
        }
        public Packet() { }
    }

}
