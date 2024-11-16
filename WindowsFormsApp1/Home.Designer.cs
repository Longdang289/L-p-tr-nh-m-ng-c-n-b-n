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
            this.tbServerIP = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnOffline = new System.Windows.Forms.Button();
            this.lbName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbRoomID = new System.Windows.Forms.TextBox();
            this.lbRoomID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbServerIP
            // 
            this.lbServerIP.AutoSize = true;
            this.lbServerIP.BackColor = System.Drawing.Color.Transparent;
            this.lbServerIP.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbServerIP.ForeColor = System.Drawing.Color.Transparent;
            this.lbServerIP.Location = new System.Drawing.Point(513, 113);
            this.lbServerIP.Name = "lbServerIP";
            this.lbServerIP.Size = new System.Drawing.Size(87, 18);
            this.lbServerIP.TabIndex = 0;
            this.lbServerIP.Text = "Server IP";
            // 
            // tbServerIP
            // 
            this.tbServerIP.BackColor = System.Drawing.SystemColors.Window;
            this.tbServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbServerIP.Location = new System.Drawing.Point(617, 110);
            this.tbServerIP.Name = "tbServerIP";
            this.tbServerIP.Size = new System.Drawing.Size(152, 22);
            this.tbServerIP.TabIndex = 3;
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.Salmon;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.Location = new System.Drawing.Point(617, 253);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(83, 120);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "Create a room";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.Coral;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnConnect.Location = new System.Drawing.Point(617, 379);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(172, 58);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect ";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnJoin
            // 
            this.btnJoin.BackColor = System.Drawing.Color.Salmon;
            this.btnJoin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnJoin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJoin.Location = new System.Drawing.Point(706, 253);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(83, 120);
            this.btnJoin.TabIndex = 6;
            this.btnJoin.Text = "Join a room";
            this.btnJoin.UseVisualStyleBackColor = false;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // btnOffline
            // 
            this.btnOffline.BackColor = System.Drawing.Color.Salmon;
            this.btnOffline.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOffline.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOffline.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOffline.Location = new System.Drawing.Point(513, 253);
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
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.ForeColor = System.Drawing.Color.Transparent;
            this.lbName.Location = new System.Drawing.Point(513, 160);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(56, 18);
            this.lbName.TabIndex = 8;
            this.lbName.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbName.Location = new System.Drawing.Point(617, 157);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(152, 22);
            this.tbName.TabIndex = 9;
            // 
            // tbRoomID
            // 
            this.tbRoomID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRoomID.Location = new System.Drawing.Point(617, 208);
            this.tbRoomID.Name = "tbRoomID";
            this.tbRoomID.Size = new System.Drawing.Size(152, 22);
            this.tbRoomID.TabIndex = 11;
            // 
            // lbRoomID
            // 
            this.lbRoomID.AutoSize = true;
            this.lbRoomID.BackColor = System.Drawing.Color.Transparent;
            this.lbRoomID.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRoomID.ForeColor = System.Drawing.Color.Transparent;
            this.lbRoomID.Location = new System.Drawing.Point(513, 211);
            this.lbRoomID.Name = "lbRoomID";
            this.lbRoomID.Size = new System.Drawing.Size(81, 18);
            this.lbRoomID.TabIndex = 10;
            this.lbRoomID.Text = "Room ID";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.sunlight_colorful_digital_art_sky_blue_background_hexagon_105873_wallhere_com;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(844, 476);
            this.Controls.Add(this.tbRoomID);
            this.Controls.Add(this.lbRoomID);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.btnOffline);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.tbServerIP);
            this.Controls.Add(this.lbServerIP);
            this.Name = "Home";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbServerIP;
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

