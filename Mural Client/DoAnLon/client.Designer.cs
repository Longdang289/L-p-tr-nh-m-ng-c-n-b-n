using System;
using System.Drawing;
using System.Windows.Forms;
namespace DoAnLon
{
    public partial class client : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(client));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.PanelDraw = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btn_Pen = new System.Windows.Forms.Button();
            this.btn_eraser = new System.Windows.Forms.Button();
            this.btnStickyNote = new System.Windows.Forms.Button();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.btnUpdataSize = new System.Windows.Forms.Button();
            this.btnTpToPosition = new System.Windows.Forms.Button();
            this.lbPosition = new System.Windows.Forms.Label();
            this.lbRoomID = new System.Windows.Forms.Label();
            this.tbRoomID = new System.Windows.Forms.TextBox();
            this.lbName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbNoteOne = new System.Windows.Forms.TextBox();
            this.tbNoteTwo = new System.Windows.Forms.TextBox();
            this.lbNoteSize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PanelDraw
            // 
            this.PanelDraw.AutoScroll = true;
            this.PanelDraw.AutoSize = true;
            this.PanelDraw.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.PanelDraw.Cursor = System.Windows.Forms.Cursors.Default;
            this.PanelDraw.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PanelDraw.Location = new System.Drawing.Point(8, 121);
            this.PanelDraw.Margin = new System.Windows.Forms.Padding(2);
            this.PanelDraw.Name = "PanelDraw";
            this.PanelDraw.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PanelDraw.Size = new System.Drawing.Size(4000, 3900);
            this.PanelDraw.TabIndex = 12;
            this.PanelDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelDraw_Paint);
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.Location = new System.Drawing.Point(66, 19);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(54, 50);
            this.btnLoad.TabIndex = 8;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(8, 17);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(54, 52);
            this.btnSave.TabIndex = 7;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btn_Pen
            // 
            this.btn_Pen.Image = ((System.Drawing.Image)(resources.GetObject("btn_Pen.Image")));
            this.btn_Pen.Location = new System.Drawing.Point(289, 14);
            this.btn_Pen.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Pen.Name = "btn_Pen";
            this.btn_Pen.Size = new System.Drawing.Size(49, 52);
            this.btn_Pen.TabIndex = 3;
            this.btn_Pen.UseVisualStyleBackColor = true;
            this.btn_Pen.Click += new System.EventHandler(this.btn_Pen_Click);
            // 
            // btn_eraser
            // 
            this.btn_eraser.Image = ((System.Drawing.Image)(resources.GetObject("btn_eraser.Image")));
            this.btn_eraser.Location = new System.Drawing.Point(354, 14);
            this.btn_eraser.Margin = new System.Windows.Forms.Padding(2);
            this.btn_eraser.Name = "btn_eraser";
            this.btn_eraser.Size = new System.Drawing.Size(53, 52);
            this.btn_eraser.TabIndex = 2;
            this.btn_eraser.UseVisualStyleBackColor = true;
            this.btn_eraser.Click += new System.EventHandler(this.btn_eraser_Click);
            // 
            // btnStickyNote
            // 
            this.btnStickyNote.Image = ((System.Drawing.Image)(resources.GetObject("btnStickyNote.Image")));
            this.btnStickyNote.Location = new System.Drawing.Point(124, 19);
            this.btnStickyNote.Margin = new System.Windows.Forms.Padding(2);
            this.btnStickyNote.Name = "btnStickyNote";
            this.btnStickyNote.Size = new System.Drawing.Size(53, 51);
            this.btnStickyNote.TabIndex = 15;
            this.btnStickyNote.UseVisualStyleBackColor = true;
            this.btnStickyNote.Click += new System.EventHandler(this.btnStickyNote_Click);
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(418, 16);
            this.tbSize.Margin = new System.Windows.Forms.Padding(2);
            this.tbSize.Multiline = true;
            this.tbSize.Name = "tbSize";
            this.tbSize.Size = new System.Drawing.Size(37, 21);
            this.tbSize.TabIndex = 16;
            // 
            // btnUpdataSize
            // 
            this.btnUpdataSize.Location = new System.Drawing.Point(457, 15);
            this.btnUpdataSize.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpdataSize.Name = "btnUpdataSize";
            this.btnUpdataSize.Size = new System.Drawing.Size(54, 20);
            this.btnUpdataSize.TabIndex = 17;
            this.btnUpdataSize.Text = "Update";
            this.btnUpdataSize.UseVisualStyleBackColor = true;
            this.btnUpdataSize.Click += new System.EventHandler(this.btnUpdataSize_Click);
            // 
            // btnTpToPosition
            // 
            this.btnTpToPosition.Location = new System.Drawing.Point(535, 15);
            this.btnTpToPosition.Margin = new System.Windows.Forms.Padding(2);
            this.btnTpToPosition.Name = "btnTpToPosition";
            this.btnTpToPosition.Size = new System.Drawing.Size(61, 52);
            this.btnTpToPosition.TabIndex = 19;
            this.btnTpToPosition.Text = "Force View";
            this.btnTpToPosition.UseVisualStyleBackColor = true;
            this.btnTpToPosition.Click += new System.EventHandler(this.btnTpToPosition_Click);
            // 
            // lbPosition
            // 
            this.lbPosition.AutoSize = true;
            this.lbPosition.Location = new System.Drawing.Point(856, 31);
            this.lbPosition.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(25, 13);
            this.lbPosition.TabIndex = 21;
            this.lbPosition.Text = "0, 0";
            // 
            // lbRoomID
            // 
            this.lbRoomID.AutoSize = true;
            this.lbRoomID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRoomID.Location = new System.Drawing.Point(415, 86);
            this.lbRoomID.Name = "lbRoomID";
            this.lbRoomID.Size = new System.Drawing.Size(56, 15);
            this.lbRoomID.TabIndex = 22;
            this.lbRoomID.Text = "Room ID";
            // 
            // tbRoomID
            // 
            this.tbRoomID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRoomID.Location = new System.Drawing.Point(477, 83);
            this.tbRoomID.Name = "tbRoomID";
            this.tbRoomID.Size = new System.Drawing.Size(57, 21);
            this.tbRoomID.TabIndex = 23;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(284, 86);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(41, 15);
            this.lbName.TabIndex = 24;
            this.lbName.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbName.Location = new System.Drawing.Point(331, 83);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(78, 21);
            this.tbName.TabIndex = 25;
            // 
            // tbNoteOne
            // 
            this.tbNoteOne.Location = new System.Drawing.Point(182, 50);
            this.tbNoteOne.Name = "tbNoteOne";
            this.tbNoteOne.Size = new System.Drawing.Size(42, 20);
            this.tbNoteOne.TabIndex = 26;
            // 
            // tbNoteTwo
            // 
            this.tbNoteTwo.Location = new System.Drawing.Point(230, 50);
            this.tbNoteTwo.Name = "tbNoteTwo";
            this.tbNoteTwo.Size = new System.Drawing.Size(42, 20);
            this.tbNoteTwo.TabIndex = 27;
            // 
            // lbNoteSize
            // 
            this.lbNoteSize.AutoSize = true;
            this.lbNoteSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNoteSize.Location = new System.Drawing.Point(195, 21);
            this.lbNoteSize.Name = "lbNoteSize";
            this.lbNoteSize.Size = new System.Drawing.Size(65, 16);
            this.lbNoteSize.TabIndex = 28;
            this.lbNoteSize.Text = "Note Size";
            // 
            // client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 477);
            this.Controls.Add(this.lbNoteSize);
            this.Controls.Add(this.tbNoteTwo);
            this.Controls.Add(this.tbNoteOne);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.tbRoomID);
            this.Controls.Add(this.lbRoomID);
            this.Controls.Add(this.lbPosition);
            this.Controls.Add(this.btnTpToPosition);
            this.Controls.Add(this.btnUpdataSize);
            this.Controls.Add(this.tbSize);
            this.Controls.Add(this.btnStickyNote);
            this.Controls.Add(this.PanelDraw);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btn_Pen);
            this.Controls.Add(this.btn_eraser);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "client";
            this.Text = "Mural";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button btn_eraser;
        private Button btn_Pen;
        private ColorDialog colorDialog1;
        private Button btnSave;
        private Button btnLoad;
        private Panel PanelDraw;
        private Button btnStickyNote;
        private TextBox tbSize;
        private Button btnUpdataSize;
        private Button btnTpToPosition;
        private Label lbPosition;
        private Label lbRoomID;
        private TextBox tbRoomID;
        private Label lbName;
        private TextBox tbName;
        private TextBox tbNoteOne;
        private TextBox tbNoteTwo;
        private Label lbNoteSize;
    }
}

