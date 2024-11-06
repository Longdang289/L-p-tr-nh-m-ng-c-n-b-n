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
            this.btnClear = new System.Windows.Forms.Button();
            this.btnReundo = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btn_Pen = new System.Windows.Forms.Button();
            this.btn_eraser = new System.Windows.Forms.Button();
            this.btnStickyNote = new System.Windows.Forms.Button();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.btnUpdataSize = new System.Windows.Forms.Button();
            this.btnTpToPosition = new System.Windows.Forms.Button();
            this.btnGetPosition = new System.Windows.Forms.Button();
            this.lbPosition = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PanelDraw
            // 
            this.PanelDraw.AutoScroll = true;
            this.PanelDraw.AutoSize = true;
            this.PanelDraw.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.PanelDraw.Cursor = System.Windows.Forms.Cursors.Default;
            this.PanelDraw.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PanelDraw.Location = new System.Drawing.Point(12, 186);
            this.PanelDraw.Name = "PanelDraw";
            this.PanelDraw.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PanelDraw.Size = new System.Drawing.Size(6000, 6000);
            this.PanelDraw.TabIndex = 12;
            this.PanelDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelDraw_Paint);
            // 
            // btnClear
            // 
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.Location = new System.Drawing.Point(336, 24);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 78);
            this.btnClear.TabIndex = 14;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnReundo
            // 
            this.btnReundo.Image = ((System.Drawing.Image)(resources.GetObject("btnReundo.Image")));
            this.btnReundo.Location = new System.Drawing.Point(2442, 12);
            this.btnReundo.Name = "btnReundo";
            this.btnReundo.Size = new System.Drawing.Size(73, 70);
            this.btnReundo.TabIndex = 11;
            this.btnReundo.UseVisualStyleBackColor = true;
            this.btnReundo.Click += new System.EventHandler(this.btnReundo_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.Location = new System.Drawing.Point(2337, 12);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(77, 70);
            this.btnUndo.TabIndex = 10;
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.Location = new System.Drawing.Point(12, 103);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(81, 77);
            this.btnLoad.TabIndex = 8;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(12, 17);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(81, 80);
            this.btnSave.TabIndex = 7;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btn_Pen
            // 
            this.btn_Pen.Image = ((System.Drawing.Image)(resources.GetObject("btn_Pen.Image")));
            this.btn_Pen.Location = new System.Drawing.Point(434, 22);
            this.btn_Pen.Name = "btn_Pen";
            this.btn_Pen.Size = new System.Drawing.Size(74, 80);
            this.btn_Pen.TabIndex = 3;
            this.btn_Pen.UseVisualStyleBackColor = true;
            this.btn_Pen.Click += new System.EventHandler(this.btn_Pen_Click);
            // 
            // btn_eraser
            // 
            this.btn_eraser.Image = ((System.Drawing.Image)(resources.GetObject("btn_eraser.Image")));
            this.btn_eraser.Location = new System.Drawing.Point(531, 22);
            this.btn_eraser.Name = "btn_eraser";
            this.btn_eraser.Size = new System.Drawing.Size(79, 80);
            this.btn_eraser.TabIndex = 2;
            this.btn_eraser.UseVisualStyleBackColor = true;
            this.btn_eraser.Click += new System.EventHandler(this.btn_eraser_Click);
            // 
            // btnStickyNote
            // 
            this.btnStickyNote.Image = ((System.Drawing.Image)(resources.GetObject("btnStickyNote.Image")));
            this.btnStickyNote.Location = new System.Drawing.Point(250, 24);
            this.btnStickyNote.Name = "btnStickyNote";
            this.btnStickyNote.Size = new System.Drawing.Size(80, 78);
            this.btnStickyNote.TabIndex = 15;
            this.btnStickyNote.UseVisualStyleBackColor = true;
            this.btnStickyNote.Click += new System.EventHandler(this.btnStickyNote_Click);
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(627, 24);
            this.tbSize.Multiline = true;
            this.tbSize.Name = "tbSize";
            this.tbSize.Size = new System.Drawing.Size(53, 30);
            this.tbSize.TabIndex = 16;
            // 
            // btnUpdataSize
            // 
            this.btnUpdataSize.Location = new System.Drawing.Point(686, 23);
            this.btnUpdataSize.Name = "btnUpdataSize";
            this.btnUpdataSize.Size = new System.Drawing.Size(81, 31);
            this.btnUpdataSize.TabIndex = 17;
            this.btnUpdataSize.Text = "Update";
            this.btnUpdataSize.UseVisualStyleBackColor = true;
            this.btnUpdataSize.Click += new System.EventHandler(this.btnUpdataSize_Click);
            // 
            // btnTpToPosition
            // 
            this.btnTpToPosition.Location = new System.Drawing.Point(868, 17);
            this.btnTpToPosition.Name = "btnTpToPosition";
            this.btnTpToPosition.Size = new System.Drawing.Size(79, 80);
            this.btnTpToPosition.TabIndex = 19;
            this.btnTpToPosition.Text = "Teleport To Position";
            this.btnTpToPosition.UseVisualStyleBackColor = true;
            this.btnTpToPosition.Click += new System.EventHandler(this.btnTpToPosition_Click);
            // 
            // btnGetPosition
            // 
            this.btnGetPosition.Location = new System.Drawing.Point(783, 17);
            this.btnGetPosition.Name = "btnGetPosition";
            this.btnGetPosition.Size = new System.Drawing.Size(79, 80);
            this.btnGetPosition.TabIndex = 20;
            this.btnGetPosition.Text = "Get Position";
            this.btnGetPosition.UseVisualStyleBackColor = true;
            this.btnGetPosition.Click += new System.EventHandler(this.btnGetPosition_Click);
            // 
            // lbPosition
            // 
            this.lbPosition.AutoSize = true;
            this.lbPosition.Location = new System.Drawing.Point(1284, 48);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(35, 20);
            this.lbPosition.TabIndex = 21;
            this.lbPosition.Text = "0, 0";
            // 
            // client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2546, 734);
            this.Controls.Add(this.lbPosition);
            this.Controls.Add(this.btnGetPosition);
            this.Controls.Add(this.btnTpToPosition);
            this.Controls.Add(this.btnUpdataSize);
            this.Controls.Add(this.tbSize);
            this.Controls.Add(this.btnStickyNote);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.PanelDraw);
            this.Controls.Add(this.btnReundo);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btn_Pen);
            this.Controls.Add(this.btn_eraser);
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
        private Button btnUndo;
        private Button btnReundo;
        private Panel PanelDraw;
        private Button btnClear;
        private Button btnStickyNote;
        private TextBox tbSize;
        private Button btnUpdataSize;
        private Button btnTpToPosition;
        private Button btnGetPosition;
        private Label lbPosition;
    }
}

