namespace WindowsFormsApp1
{
    partial class Home
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbServerIP = new System.Windows.Forms.Label();
            this.picMain = new System.Windows.Forms.PictureBox();
            this.tbServerIP = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnOffline = new System.Windows.Forms.Button();
            this.lbName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbRoomID = new System.Windows.Forms.TextBox();
            this.lbRoomID = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            this.SuspendLayout();
            // 
            // lbServerIP
            // 
            this.lbServerIP.AutoSize = true;
            this.lbServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbServerIP.Location = new System.Drawing.Point(32, 99);
            this.lbServerIP.Name = "lbServerIP";
            this.lbServerIP.Size = new System.Drawing.Size(62, 16);
            this.lbServerIP.TabIndex = 0;
            this.lbServerIP.Text = "Server IP";
            // 
            // picMain
            // 
            this.picMain.Location = new System.Drawing.Point(431, 0);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(372, 450);
            this.picMain.TabIndex = 1;
            this.picMain.TabStop = false;
            // 
            // tbServerIP
            // 
            this.tbServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbServerIP.Location = new System.Drawing.Point(136, 96);
            this.tbServerIP.Name = "tbServerIP";
            this.tbServerIP.Size = new System.Drawing.Size(152, 22);
            this.tbServerIP.TabIndex = 3;
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.LimeGreen;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.Location = new System.Drawing.Point(136, 239);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(83, 120);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "Create a room";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.Lime;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnConnect.Location = new System.Drawing.Point(136, 365);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(172, 58);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect ";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnJoin
            // 
            this.btnJoin.BackColor = System.Drawing.Color.LimeGreen;
            this.btnJoin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnJoin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJoin.Location = new System.Drawing.Point(225, 239);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(83, 120);
            this.btnJoin.TabIndex = 6;
            this.btnJoin.Text = "Join a room";
            this.btnJoin.UseVisualStyleBackColor = false;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // btnOffline
            // 
            this.btnOffline.BackColor = System.Drawing.Color.LimeGreen;
            this.btnOffline.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOffline.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOffline.Location = new System.Drawing.Point(32, 239);
            this.btnOffline.Name = "btnOffline";
            this.btnOffline.Size = new System.Drawing.Size(98, 184);
            this.btnOffline.TabIndex = 7;
            this.btnOffline.Text = "Start offline";
            this.btnOffline.UseVisualStyleBackColor = false;
            this.btnOffline.Click += new System.EventHandler(this.btnOffline_Click);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(32, 146);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(44, 16);
            this.lbName.TabIndex = 8;
            this.lbName.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbName.Location = new System.Drawing.Point(136, 143);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(152, 22);
            this.tbName.TabIndex = 9;
            // 
            // tbRoomID
            // 
            this.tbRoomID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRoomID.Location = new System.Drawing.Point(136, 194);
            this.tbRoomID.Name = "tbRoomID";
            this.tbRoomID.Size = new System.Drawing.Size(152, 22);
            this.tbRoomID.TabIndex = 11;
            // 
            // lbRoomID
            // 
            this.lbRoomID.AutoSize = true;
            this.lbRoomID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRoomID.Location = new System.Drawing.Point(32, 197);
            this.lbRoomID.Name = "lbRoomID";
            this.lbRoomID.Size = new System.Drawing.Size(60, 16);
            this.lbRoomID.TabIndex = 10;
            this.lbRoomID.Text = "Room ID";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.picMain;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbRoomID);
            this.Controls.Add(this.lbRoomID);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.btnOffline);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.tbServerIP);
            this.Controls.Add(this.picMain);
            this.Controls.Add(this.lbServerIP);
            this.Name = "Home";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbServerIP;
        private System.Windows.Forms.PictureBox picMain;
        private System.Windows.Forms.TextBox tbServerIP;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Button btnOffline;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbRoomID;
        private System.Windows.Forms.Label lbRoomID;
    }
}

