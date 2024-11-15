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

        public string IP { get; set; }//IP client gửi

        public string RoomID { get; set; }// ID của phòng mà người dùng đang tham gia
        public string DrawingData { get; set; } // Dữ liệu JSON của các nét vẽ
        // Chuỗi biểu diễn của bitmap,cái này chuyển ma trận pixel ra string qua bên kia chuyển lại ra thành ảnh
        public string BitmapString { get; set; }
        // Constructor Packet
        public Packet(int code, string username, string ip, string roomID, string drawingData=null)
        {
            Code = code;
            Username = username;
            IP = ip;
            RoomID = roomID;
            DrawingData = drawingData;
        }

        public Packet() { }
    }

}
