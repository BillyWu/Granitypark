namespace Granity.parkStation
{
    partial class FrmDownTime
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.MngTimeGrp = new System.Windows.Forms.GroupBox();
            this.lbDeviceInfo = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnDown = new System.Windows.Forms.Button();
            this.BtnInit = new System.Windows.Forms.Button();
            this.BtnRefsh = new System.Windows.Forms.Button();
            this.LbDevInfo = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ValListGrp = new System.Windows.Forms.GroupBox();
            this.gdValList = new System.Windows.Forms.DataGridView();
            this.panel2.SuspendLayout();
            this.MngTimeGrp.SuspendLayout();
            this.panel3.SuspendLayout();
            this.ValListGrp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gdValList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(907, 15);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.MngTimeGrp);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(907, 83);
            this.panel2.TabIndex = 3;
            // 
            // MngTimeGrp
            // 
            this.MngTimeGrp.Controls.Add(this.lbDeviceInfo);
            this.MngTimeGrp.Controls.Add(this.BtnClose);
            this.MngTimeGrp.Controls.Add(this.BtnDown);
            this.MngTimeGrp.Controls.Add(this.BtnInit);
            this.MngTimeGrp.Controls.Add(this.BtnRefsh);
            this.MngTimeGrp.Controls.Add(this.LbDevInfo);
            this.MngTimeGrp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MngTimeGrp.Location = new System.Drawing.Point(0, 0);
            this.MngTimeGrp.Name = "MngTimeGrp";
            this.MngTimeGrp.Size = new System.Drawing.Size(907, 83);
            this.MngTimeGrp.TabIndex = 2;
            this.MngTimeGrp.TabStop = false;
            this.MngTimeGrp.Text = "有效时段管理";
            // 
            // lbDeviceInfo
            // 
            this.lbDeviceInfo.AutoSize = true;
            this.lbDeviceInfo.Location = new System.Drawing.Point(73, 32);
            this.lbDeviceInfo.Name = "lbDeviceInfo";
            this.lbDeviceInfo.Size = new System.Drawing.Size(53, 12);
            this.lbDeviceInfo.TabIndex = 7;
            this.lbDeviceInfo.Text = "设备信息";
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(648, 27);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 6;
            this.BtnClose.Text = "返回";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click_1);
            // 
            // BtnDown
            // 
            this.BtnDown.Location = new System.Drawing.Point(567, 27);
            this.BtnDown.Name = "BtnDown";
            this.BtnDown.Size = new System.Drawing.Size(75, 23);
            this.BtnDown.TabIndex = 3;
            this.BtnDown.Text = "下载";
            this.BtnDown.UseVisualStyleBackColor = true;
            this.BtnDown.Click += new System.EventHandler(this.BtnDown_Click_1);
            // 
            // BtnInit
            // 
            this.BtnInit.Location = new System.Drawing.Point(486, 27);
            this.BtnInit.Name = "BtnInit";
            this.BtnInit.Size = new System.Drawing.Size(75, 23);
            this.BtnInit.TabIndex = 2;
            this.BtnInit.Text = "初始化";
            this.BtnInit.UseVisualStyleBackColor = true;
            this.BtnInit.Click += new System.EventHandler(this.BtnInit_Click_1);
            // 
            // BtnRefsh
            // 
            this.BtnRefsh.Location = new System.Drawing.Point(405, 27);
            this.BtnRefsh.Name = "BtnRefsh";
            this.BtnRefsh.Size = new System.Drawing.Size(75, 23);
            this.BtnRefsh.TabIndex = 1;
            this.BtnRefsh.Text = "刷新";
            this.BtnRefsh.UseVisualStyleBackColor = true;
            this.BtnRefsh.Click += new System.EventHandler(this.BtnRefsh_Click_1);
            // 
            // LbDevInfo
            // 
            this.LbDevInfo.AutoSize = true;
            this.LbDevInfo.Location = new System.Drawing.Point(12, 32);
            this.LbDevInfo.Name = "LbDevInfo";
            this.LbDevInfo.Size = new System.Drawing.Size(65, 12);
            this.LbDevInfo.TabIndex = 0;
            this.LbDevInfo.Text = "当前设备：";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ValListGrp);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 98);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(907, 413);
            this.panel3.TabIndex = 4;
            // 
            // ValListGrp
            // 
            this.ValListGrp.Controls.Add(this.gdValList);
            this.ValListGrp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ValListGrp.Location = new System.Drawing.Point(0, 0);
            this.ValListGrp.Name = "ValListGrp";
            this.ValListGrp.Size = new System.Drawing.Size(907, 413);
            this.ValListGrp.TabIndex = 2;
            this.ValListGrp.TabStop = false;
            this.ValListGrp.Text = "有效时段清单";
            // 
            // gdValList
            // 
            this.gdValList.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gdValList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gdValList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gdValList.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gdValList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdValList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gdValList.Location = new System.Drawing.Point(3, 17);
            this.gdValList.Name = "gdValList";
            this.gdValList.RowTemplate.Height = 23;
            this.gdValList.Size = new System.Drawing.Size(901, 393);
            this.gdValList.TabIndex = 1;
            this.gdValList.Tag = "@db=时段设置";
            // 
            // FrmDownTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 517);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.Name = "FrmDownTime";
            this.Text = "加载有效时段";
            this.Load += new System.EventHandler(this.FrmDownTime_Load);
            this.panel2.ResumeLayout(false);
            this.MngTimeGrp.ResumeLayout(false);
            this.MngTimeGrp.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ValListGrp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gdValList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox MngTimeGrp;
        private System.Windows.Forms.Label lbDeviceInfo;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnDown;
        private System.Windows.Forms.Button BtnInit;
        private System.Windows.Forms.Button BtnRefsh;
        private System.Windows.Forms.Label LbDevInfo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox ValListGrp;
        private System.Windows.Forms.DataGridView gdValList;
    }
}