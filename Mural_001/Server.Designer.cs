namespace Mural_001
{
    partial class Server
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnStart = new Button();
            btnStop = new Button();
            btnGet = new Button();
            lsvOne = new ListView();
            tbServerIP = new TextBox();
            tbRoom = new TextBox();
            tbUser = new TextBox();
            lbRoom = new Label();
            lbUser = new Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(41, 12);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(145, 74);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(41, 109);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(145, 74);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnGet
            // 
            btnGet.Location = new Point(41, 215);
            btnGet.Name = "btnGet";
            btnGet.Size = new Size(145, 74);
            btnGet.TabIndex = 2;
            btnGet.Text = "Get IP";
            btnGet.UseVisualStyleBackColor = true;
            btnGet.Click += btnServerIP_Click;
            // 
            // lsvOne
            // 
            lsvOne.Location = new Point(268, 4);
            lsvOne.Name = "lsvOne";
            lsvOne.Size = new Size(520, 342);
            lsvOne.TabIndex = 3;
            lsvOne.UseCompatibleStateImageBehavior = false;
            // 
            // tbServerIP
            // 
            tbServerIP.Location = new Point(61, 323);
            tbServerIP.Name = "tbServerIP";
            tbServerIP.Size = new Size(100, 23);
            tbServerIP.TabIndex = 4;
            
            // 
            // tbRoom
            // 
            tbRoom.Location = new Point(213, 404);
            tbRoom.Name = "tbRoom";
            tbRoom.Size = new Size(100, 23);
            tbRoom.TabIndex = 5;
            // 
            // tbUser
            // 
            tbUser.Location = new Point(526, 404);
            tbUser.Name = "tbUser";
            tbUser.Size = new Size(100, 23);
            tbUser.TabIndex = 6;
            // 
            // lbRoom
            // 
            lbRoom.AutoSize = true;
            lbRoom.Location = new Point(213, 365);
            lbRoom.Name = "lbRoom";
            lbRoom.Size = new Size(75, 15);
            lbRoom.TabIndex = 7;
            lbRoom.Text = "Room Count";
            // 
            // lbUser
            // 
            lbUser.AutoSize = true;
            lbUser.Location = new Point(541, 365);
            lbUser.Name = "lbUser";
            lbUser.Size = new Size(66, 15);
            lbUser.TabIndex = 8;
            lbUser.Text = "User Count";
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lbUser);
            Controls.Add(lbRoom);
            Controls.Add(tbUser);
            Controls.Add(tbRoom);
            Controls.Add(tbServerIP);
            Controls.Add(lsvOne);
            Controls.Add(btnGet);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "Server";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Button btnGet;
        private ListView lsvOne;
        private TextBox tbServerIP;
        private TextBox tbRoom;
        private TextBox tbUser;
        private Label lbRoom;
        private Label lbUser;
    }
}
