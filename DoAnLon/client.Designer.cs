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
            this.btn_eraser = new System.Windows.Forms.Button();
            this.btn_Pen = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnReundo = new System.Windows.Forms.Button();
            this.PanelDraw = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_eraser
            // 
            this.btn_eraser.Location = new System.Drawing.Point(183, 10);
            this.btn_eraser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_eraser.Name = "btn_eraser";
            this.btn_eraser.Size = new System.Drawing.Size(50, 49);
            this.btn_eraser.TabIndex = 2;
            this.btn_eraser.Text = "cục gôm";
            this.btn_eraser.UseVisualStyleBackColor = true;
            this.btn_eraser.Click += new System.EventHandler(this.btn_eraser_Click);
            // 
            // btn_Pen
            // 
            this.btn_Pen.Location = new System.Drawing.Point(129, 10);
            this.btn_Pen.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_Pen.Name = "btn_Pen";
            this.btn_Pen.Size = new System.Drawing.Size(50, 49);
            this.btn_Pen.TabIndex = 3;
            this.btn_Pen.Text = "bút";
            this.btn_Pen.UseVisualStyleBackColor = true;
            this.btn_Pen.Click += new System.EventHandler(this.btn_Pen_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(297, 14);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(69, 45);
            this.trackBar1.TabIndex = 4;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(75, 14);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 45);
            this.button1.TabIndex = 5;
            this.button1.Text = "clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(14, 23);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(50, 45);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(14, 81);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(45, 45);
            this.btnLoad.TabIndex = 8;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(495, 16);
            this.btnUndo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(50, 45);
            this.btnUndo.TabIndex = 10;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnReundo
            // 
            this.btnReundo.Location = new System.Drawing.Point(549, 16);
            this.btnReundo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnReundo.Name = "btnReundo";
            this.btnReundo.Size = new System.Drawing.Size(50, 45);
            this.btnReundo.TabIndex = 11;
            this.btnReundo.Text = "Reundo";
            this.btnReundo.UseVisualStyleBackColor = true;
            // 
            // PanelDraw
            // 
            this.PanelDraw.Location = new System.Drawing.Point(95, 16);
            this.PanelDraw.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PanelDraw.Name = "PanelDraw";
            this.PanelDraw.Size = new System.Drawing.Size(13, 11);
            this.PanelDraw.TabIndex = 12;
            this.PanelDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelDraw_Paint);
            // 
            // client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 420);
            this.Controls.Add(this.PanelDraw);
            this.Controls.Add(this.btnReundo);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btn_Pen);
            this.Controls.Add(this.btn_eraser);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "client";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button btn_eraser;
        private Button btn_Pen;
        private ColorDialog colorDialog1;
        private TrackBar trackBar1;
        private Button button1;
        private Button btnSave;
        private Button btnLoad;
        private Button btnUndo;
        private Button btnReundo;
        private Panel PanelDraw;
    }
}

