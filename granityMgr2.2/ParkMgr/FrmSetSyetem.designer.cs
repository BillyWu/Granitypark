namespace Granity.parkStation
{
    partial class FrmSetSyetem
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
            this.SetSysgroup = new System.Windows.Forms.GroupBox();
            this.OnCloseBtn = new System.Windows.Forms.Button();
            this.DownComBtn = new System.Windows.Forms.Button();
            this.CheckBtn = new System.Windows.Forms.Button();
            this.InitBtn = new System.Windows.Forms.Button();
            this.DevListGroup = new System.Windows.Forms.GroupBox();
            this.dbGrid = new System.Windows.Forms.DataGridView();
            this.DevListGrid = new System.Windows.Forms.DataGridView();
            this.SetComGroup = new System.Windows.Forms.GroupBox();
            this.MoniDevBtn = new System.Windows.Forms.Button();
            this.ValidCardBtn = new System.Windows.Forms.Button();
            this.ReadRecBtn = new System.Windows.Forms.Button();
            this.SetFeeBtn = new System.Windows.Forms.Button();
            this.SetValTimeBtn = new System.Windows.Forms.Button();
            this.SetTimeBtn = new System.Windows.Forms.Button();
            this.SetSysgroup.SuspendLayout();
            this.DevListGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DevListGrid)).BeginInit();
            this.SetComGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // SetSysgroup
            // 
            this.SetSysgroup.Controls.Add(this.OnCloseBtn);
            this.SetSysgroup.Controls.Add(this.DownComBtn);
            this.SetSysgroup.Controls.Add(this.CheckBtn);
            this.SetSysgroup.Controls.Add(this.InitBtn);
            this.SetSysgroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.SetSysgroup.Location = new System.Drawing.Point(0, 0);
            this.SetSysgroup.Name = "SetSysgroup";
            this.SetSysgroup.Size = new System.Drawing.Size(723, 60);
            this.SetSysgroup.TabIndex = 0;
            this.SetSysgroup.TabStop = false;
            this.SetSysgroup.Text = "系统设置";
            // 
            // OnCloseBtn
            // 
            this.OnCloseBtn.Location = new System.Drawing.Point(302, 20);
            this.OnCloseBtn.Name = "OnCloseBtn";
            this.OnCloseBtn.Size = new System.Drawing.Size(75, 23);
            this.OnCloseBtn.TabIndex = 6;
            this.OnCloseBtn.Text = "返回";
            this.OnCloseBtn.UseVisualStyleBackColor = true;
            this.OnCloseBtn.Click += new System.EventHandler(this.OnCloseBtn_Click);
            // 
            // DownComBtn
            // 
            this.DownComBtn.Location = new System.Drawing.Point(204, 20);
            this.DownComBtn.Name = "DownComBtn";
            this.DownComBtn.Size = new System.Drawing.Size(92, 23);
            this.DownComBtn.TabIndex = 5;
            this.DownComBtn.Text = "加载控制参数";
            this.DownComBtn.UseVisualStyleBackColor = true;
            this.DownComBtn.Click += new System.EventHandler(this.DownComBtn_Click);
            // 
            // CheckBtn
            // 
            this.CheckBtn.Location = new System.Drawing.Point(123, 20);
            this.CheckBtn.Name = "CheckBtn";
            this.CheckBtn.Size = new System.Drawing.Size(75, 23);
            this.CheckBtn.TabIndex = 3;
            this.CheckBtn.Text = "巡检";
            this.CheckBtn.UseVisualStyleBackColor = true;
            this.CheckBtn.Click += new System.EventHandler(this.CheckBtn_Click);
            // 
            // InitBtn
            // 
            this.InitBtn.Location = new System.Drawing.Point(42, 20);
            this.InitBtn.Name = "InitBtn";
            this.InitBtn.Size = new System.Drawing.Size(75, 23);
            this.InitBtn.TabIndex = 2;
            this.InitBtn.Text = "初始化";
            this.InitBtn.UseVisualStyleBackColor = true;
            this.InitBtn.Click += new System.EventHandler(this.InitBtn_Click);
            // 
            // DevListGroup
            // 
            this.DevListGroup.Controls.Add(this.dbGrid);
            this.DevListGroup.Controls.Add(this.DevListGrid);
            this.DevListGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.DevListGroup.Location = new System.Drawing.Point(0, 60);
            this.DevListGroup.Name = "DevListGroup";
            this.DevListGroup.Size = new System.Drawing.Size(723, 418);
            this.DevListGroup.TabIndex = 1;
            this.DevListGroup.TabStop = false;
            this.DevListGroup.Text = "设备清单";
            // 
            // dbGrid
            // 
            this.dbGrid.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dbGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dbGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dbGrid.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dbGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dbGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbGrid.Location = new System.Drawing.Point(3, 17);
            this.dbGrid.Name = "dbGrid";
            this.dbGrid.RowTemplate.Height = 23;
            this.dbGrid.Size = new System.Drawing.Size(717, 398);
            this.dbGrid.TabIndex = 1;
            this.dbGrid.Tag = "@db=设备列表";
            // 
            // DevListGrid
            // 
            this.DevListGrid.BackgroundColor = System.Drawing.Color.Gray;
            this.DevListGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DevListGrid.GridColor = System.Drawing.Color.Chocolate;
            this.DevListGrid.Location = new System.Drawing.Point(4, 20);
            this.DevListGrid.Name = "DevListGrid";
            this.DevListGrid.RowTemplate.Height = 23;
            this.DevListGrid.Size = new System.Drawing.Size(832, 392);
            this.DevListGrid.TabIndex = 0;
            // 
            // SetComGroup
            // 
            this.SetComGroup.Controls.Add(this.MoniDevBtn);
            this.SetComGroup.Controls.Add(this.ValidCardBtn);
            this.SetComGroup.Controls.Add(this.ReadRecBtn);
            this.SetComGroup.Controls.Add(this.SetFeeBtn);
            this.SetComGroup.Controls.Add(this.SetValTimeBtn);
            this.SetComGroup.Controls.Add(this.SetTimeBtn);
            this.SetComGroup.Location = new System.Drawing.Point(0, 484);
            this.SetComGroup.Name = "SetComGroup";
            this.SetComGroup.Size = new System.Drawing.Size(720, 80);
            this.SetComGroup.TabIndex = 2;
            this.SetComGroup.TabStop = false;
            this.SetComGroup.Text = "参数设置";
            // 
            // MoniDevBtn
            // 
            this.MoniDevBtn.Location = new System.Drawing.Point(516, 31);
            this.MoniDevBtn.Name = "MoniDevBtn";
            this.MoniDevBtn.Size = new System.Drawing.Size(75, 23);
            this.MoniDevBtn.TabIndex = 5;
            this.MoniDevBtn.Text = "设备监控";
            this.MoniDevBtn.UseVisualStyleBackColor = true;
            this.MoniDevBtn.Click += new System.EventHandler(this.MoniDevBtn_Click);
            // 
            // ValidCardBtn
            // 
            this.ValidCardBtn.Location = new System.Drawing.Point(435, 31);
            this.ValidCardBtn.Name = "ValidCardBtn";
            this.ValidCardBtn.Size = new System.Drawing.Size(75, 23);
            this.ValidCardBtn.TabIndex = 4;
            this.ValidCardBtn.Text = "有效卡";
            this.ValidCardBtn.UseVisualStyleBackColor = true;
            this.ValidCardBtn.Click += new System.EventHandler(this.ValidCardBtn_Click);
            // 
            // ReadRecBtn
            // 
            this.ReadRecBtn.Location = new System.Drawing.Point(354, 31);
            this.ReadRecBtn.Name = "ReadRecBtn";
            this.ReadRecBtn.Size = new System.Drawing.Size(75, 23);
            this.ReadRecBtn.TabIndex = 3;
            this.ReadRecBtn.Text = "提取记录";
            this.ReadRecBtn.UseVisualStyleBackColor = true;
            this.ReadRecBtn.Click += new System.EventHandler(this.ReadRecBtn_Click);
            // 
            // SetFeeBtn
            // 
            this.SetFeeBtn.Location = new System.Drawing.Point(238, 31);
            this.SetFeeBtn.Name = "SetFeeBtn";
            this.SetFeeBtn.Size = new System.Drawing.Size(110, 23);
            this.SetFeeBtn.TabIndex = 2;
            this.SetFeeBtn.Text = "加载收费标准";
            this.SetFeeBtn.UseVisualStyleBackColor = true;
            this.SetFeeBtn.Click += new System.EventHandler(this.SetFeeBtn_Click);
            // 
            // SetValTimeBtn
            // 
            this.SetValTimeBtn.Location = new System.Drawing.Point(139, 31);
            this.SetValTimeBtn.Name = "SetValTimeBtn";
            this.SetValTimeBtn.Size = new System.Drawing.Size(93, 23);
            this.SetValTimeBtn.TabIndex = 1;
            this.SetValTimeBtn.Text = "加载有效时段";
            this.SetValTimeBtn.UseVisualStyleBackColor = true;
            this.SetValTimeBtn.Click += new System.EventHandler(this.SetValTimeBtn_Click);
            // 
            // SetTimeBtn
            // 
            this.SetTimeBtn.Location = new System.Drawing.Point(42, 31);
            this.SetTimeBtn.Name = "SetTimeBtn";
            this.SetTimeBtn.Size = new System.Drawing.Size(91, 23);
            this.SetTimeBtn.TabIndex = 0;
            this.SetTimeBtn.Text = "加载系统时间";
            this.SetTimeBtn.UseVisualStyleBackColor = true;
            this.SetTimeBtn.Click += new System.EventHandler(this.SetTimeBtn_Click);
            // 
            // FrmSetSyetem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 568);
            this.Controls.Add(this.SetComGroup);
            this.Controls.Add(this.DevListGroup);
            this.Controls.Add(this.SetSysgroup);
            this.Name = "FrmSetSyetem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.FrmSetSyetem_Load);
            this.SetSysgroup.ResumeLayout(false);
            this.DevListGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DevListGrid)).EndInit();
            this.SetComGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SetSysgroup;
        private System.Windows.Forms.GroupBox DevListGroup;
        private System.Windows.Forms.Button InitBtn;
        private System.Windows.Forms.Button CheckBtn;
        private System.Windows.Forms.Button OnCloseBtn;
        private System.Windows.Forms.Button DownComBtn;
        private System.Windows.Forms.DataGridView DevListGrid;
        private System.Windows.Forms.GroupBox SetComGroup;
        private System.Windows.Forms.Button SetValTimeBtn;
        private System.Windows.Forms.Button SetTimeBtn;
        private System.Windows.Forms.Button MoniDevBtn;
        private System.Windows.Forms.Button ValidCardBtn;
        private System.Windows.Forms.Button ReadRecBtn;
        private System.Windows.Forms.Button SetFeeBtn;
        private System.Windows.Forms.DataGridView dbGrid;
    }
}