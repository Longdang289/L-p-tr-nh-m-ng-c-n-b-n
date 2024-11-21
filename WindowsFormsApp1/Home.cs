using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoAnLon;
namespace WindowsFormsApp1
{
    public partial class Home : Form
    {
        private bool worked = true;
        private string name; // Tên từ tbName
        private string serverIP = "127.0.0.1"; // Địa chỉ IP từ Server
        private int controlCode = 0; // Mã điều khiển, cố định là 1
        private string roomID = null;//Mã phòng mặc định là NULL
        public Home()
        {
            InitializeComponent();
            /*
            lbServerIP.BackColor = Color.Transparent;
            lbName.BackColor = Color.Transparent;
            lbRoomID.BackColor = Color.Transparent;
            */
            // Khi form load ra, ẩn các label và textbox
            lbName.Visible = false;
            lbRoomID.Visible = false;
            tbName.Visible = false;
            tbRoomID.Visible = false;
            tbServerIP.Text = serverIP;

            //picMain.Image = Properties.Resources.picMain; 
            //picMain.SizeMode = PictureBoxSizeMode.Normal;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Khi nhấn btnCreate, hiển thị lbName và tbName
            lbName.Visible = true;
            tbName.Visible = true;

            // Đảm bảo lbRoomID và tbRoomID vẫn ẩn
            lbRoomID.Visible = false;
            tbRoomID.Visible = false;
            // Lấy thông tin từ các textbox khi nhấn btnCreate

            controlCode = 0;
            //MessageBox.Show("Thông tin đã được thiết lập. Nhấn 'Connect' để kết nối.");
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            // Khi nhấn btnJoin, hiển thị lbName, lbRoomID, tbName, và tbRoomID
            lbName.Visible = true;
            lbRoomID.Visible = true;
            tbName.Visible = true;
            tbRoomID.Visible = true;
            //Lấy thông tin
            // Lấy thông tin RoomID từ người dùng
            controlCode = 1;




            //MessageBox.Show("Thông tin đã được thiết lập. Nhấn 'Connect' để kết nối.");
        }

        private void btnOffline_Click(object sender, EventArgs e)
        {
            // Tạo một instance của form ClientOffline
            clientOffline clientOfflineForm = new clientOffline();

            // Hiển thị form ClientOffline
            this.Hide();
            clientOfflineForm.ShowDialog();

            // Đóng form Home
            this.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            worked = true;
            name = tbName.Text;
            serverIP = tbServerIP.Text;
            if (controlCode == 1 && string.IsNullOrEmpty(tbRoomID.Text))
            {
                MessageBox.Show("Vui lòng nhập Room ID");
                return;
            }
            // Kiểm tra RoomID có phải là số nguyên hay không
            if (controlCode == 1 && !int.TryParse(tbRoomID.Text, out _))
            {
                MessageBox.Show("Room ID phải là một số nguyên.");
                return;
            }
            // Kiểm tra thông tin cần thiết trước khi mở form ClientOffline
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tên");
                return;
            }

            // Nếu RoomID hợp lệ, gán vào biến roomID
            roomID = tbRoomID.Text;
            // Mở form ClientOffline và truyền tham số
            // Tạo đối tượng client và truyền delegate để mở lại form Home khi cần
            var clientForm = new client(name, serverIP, controlCode, roomID, ReopenHomeForm);
            this.Hide();
            clientForm.ShowDialog();
            if (worked == true) this.Close();
            worked = true;
        }
        // Phương thức này sẽ mở lại form Home
        private void ReopenHomeForm()
        {
            worked = false;
            this.Show();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            tbServerIP.Text = "13.76.28.212";

        }
        private void tbName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
