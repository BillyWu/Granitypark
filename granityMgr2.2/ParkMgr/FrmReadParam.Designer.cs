namespace Granity.granityMgr.ParkMgr
{
    partial class FrmReadParam
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnReadTime = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDevId = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.txtTx = new System.Windows.Forms.TextBox();
            this.txtXsp = new System.Windows.Forms.TextBox();
            this.chkTx = new System.Windows.Forms.CheckBox();
            this.chkXsp = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(327, 319);
            this.panel2.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(327, 319);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnReadTime);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cboDevId);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(319, 294);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "读取时间";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(28, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "lblMessage";
            // 
            // btnReadTime
            // 
            this.btnReadTime.Location = new System.Drawing.Point(231, 20);
            this.btnReadTime.Name = "btnReadTime";
            this.btnReadTime.Size = new System.Drawing.Size(65, 23);
            this.btnReadTime.TabIndex = 8;
            this.btnReadTime.Text = "读取";
            this.btnReadTime.UseVisualStyleBackColor = true;
            this.btnReadTime.Click += new System.EventHandler(this.btnReadTime_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "设备地址：";
            // 
            // cboDevId
            // 
            this.cboDevId.FormattingEnabled = true;
            this.cboDevId.Location = new System.Drawing.Point(86, 22);
            this.cboDevId.Name = "cboDevId";
            this.cboDevId.Size = new System.Drawing.Size(139, 20);
            this.cboDevId.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnClose);
            this.tabPage2.Controls.Add(this.btnDown);
            this.tabPage2.Controls.Add(this.txtTx);
            this.tabPage2.Controls.Add(this.txtXsp);
            this.tabPage2.Controls.Add(this.chkTx);
            this.tabPage2.Controls.Add(this.chkXsp);
            this.tabPage2.Controls.Add(this.comboBox1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(319, 294);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "屏显信息";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(254, 152);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(57, 26);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "退出";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(254, 108);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 26);
            this.btnDown.TabIndex = 15;
            this.btnDown.Text = "下载";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // txtTx
            // 
            this.txtTx.Location = new System.Drawing.Point(25, 184);
            this.txtTx.Name = "txtTx";
            this.txtTx.Size = new System.Drawing.Size(205, 21);
            this.txtTx.TabIndex = 14;
            // 
            // txtXsp
            // 
            this.txtXsp.Location = new System.Drawing.Point(26, 117);
            this.txtXsp.Name = "txtXsp";
            this.txtXsp.Size = new System.Drawing.Size(205, 21);
            this.txtXsp.TabIndex = 13;
            // 
            // chkTx
            // 
            this.chkTx.AutoSize = true;
            this.chkTx.Location = new System.Drawing.Point(10, 152);
            this.chkTx.Name = "chkTx";
            this.chkTx.Size = new System.Drawing.Size(96, 16);
            this.chkTx.TabIndex = 12;
            this.chkTx.Text = "条形打印信息";
            this.chkTx.UseVisualStyleBackColor = true;
            // 
            // chkXsp
            // 
            this.chkXsp.AutoSize = true;
            this.chkXsp.Location = new System.Drawing.Point(10, 87);
            this.chkXsp.Name = "chkXsp";
            this.chkXsp.Size = new System.Drawing.Size(132, 16);
            this.chkXsp.TabIndex = 11;
            this.chkXsp.Text = "主控制机显示屏信息";
            this.chkXsp.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(79, 46);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(152, 20);
            this.comboBox1.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "设备地址：";
            // 
            // FrmReadParam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 319);
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmReadParam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "参数读取";
            this.Load += new System.EventHandler(this.FrmReadParam_Load);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnReadTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDevId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.TextBox txtTx;
        private System.Windows.Forms.TextBox txtXsp;
        private System.Windows.Forms.CheckBox chkTx;
        private System.Windows.Forms.CheckBox chkXsp;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
    }
}