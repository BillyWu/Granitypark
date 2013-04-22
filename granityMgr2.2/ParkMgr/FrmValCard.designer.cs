namespace Granity.parkStation
{
    partial class FrmValCard
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
            this.BtnDownAll = new System.Windows.Forms.Button();
            this.BtnDownOne = new System.Windows.Forms.Button();
            this.BtnReadOne = new System.Windows.Forms.Button();
            this.BtnDelAll = new System.Windows.Forms.Button();
            this.BtnDelOne = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.OpeTypeGroup = new System.Windows.Forms.GroupBox();
            this.lbInfo = new System.Windows.Forms.Label();
            this.rdDowned = new System.Windows.Forms.RadioButton();
            this.rdUnupdate = new System.Windows.Forms.RadioButton();
            this.rdUndel = new System.Windows.Forms.RadioButton();
            this.rbUndown = new System.Windows.Forms.RadioButton();
            this.LbDevInfo = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.WhiteListGroup = new System.Windows.Forms.GroupBox();
            this.gdValList = new System.Windows.Forms.DataGridView();
            this.panel2.SuspendLayout();
            this.OpeTypeGroup.SuspendLayout();
            this.panel3.SuspendLayout();
            this.WhiteListGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gdValList)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnDownAll
            // 
            this.BtnDownAll.Location = new System.Drawing.Point(12, 491);
            this.BtnDownAll.Name = "BtnDownAll";
            this.BtnDownAll.Size = new System.Drawing.Size(75, 23);
            this.BtnDownAll.TabIndex = 2;
            this.BtnDownAll.Text = "下载全部";
            this.BtnDownAll.UseVisualStyleBackColor = true;
            this.BtnDownAll.Click += new System.EventHandler(this.BtnDownAll_Click);
            // 
            // BtnDownOne
            // 
            this.BtnDownOne.Location = new System.Drawing.Point(93, 491);
            this.BtnDownOne.Name = "BtnDownOne";
            this.BtnDownOne.Size = new System.Drawing.Size(75, 23);
            this.BtnDownOne.TabIndex = 3;
            this.BtnDownOne.Text = "下载单条";
            this.BtnDownOne.UseVisualStyleBackColor = true;
            this.BtnDownOne.Click += new System.EventHandler(this.BtnDownOne_Click);
            // 
            // BtnReadOne
            // 
            this.BtnReadOne.Location = new System.Drawing.Point(174, 491);
            this.BtnReadOne.Name = "BtnReadOne";
            this.BtnReadOne.Size = new System.Drawing.Size(75, 23);
            this.BtnReadOne.TabIndex = 4;
            this.BtnReadOne.Text = "读取单条";
            this.BtnReadOne.UseVisualStyleBackColor = true;
            this.BtnReadOne.Click += new System.EventHandler(this.BtnReadOne_Click);
            // 
            // BtnDelAll
            // 
            this.BtnDelAll.Location = new System.Drawing.Point(255, 491);
            this.BtnDelAll.Name = "BtnDelAll";
            this.BtnDelAll.Size = new System.Drawing.Size(75, 23);
            this.BtnDelAll.TabIndex = 5;
            this.BtnDelAll.Text = "删除全部";
            this.BtnDelAll.UseVisualStyleBackColor = true;
            this.BtnDelAll.Click += new System.EventHandler(this.BtnDelAll_Click);
            // 
            // BtnDelOne
            // 
            this.BtnDelOne.Location = new System.Drawing.Point(336, 491);
            this.BtnDelOne.Name = "BtnDelOne";
            this.BtnDelOne.Size = new System.Drawing.Size(75, 23);
            this.BtnDelOne.TabIndex = 6;
            this.BtnDelOne.Text = "删除单条";
            this.BtnDelOne.UseVisualStyleBackColor = true;
            this.BtnDelOne.Click += new System.EventHandler(this.BtnDelOne_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(426, 491);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 7;
            this.BtnClose.Text = "关闭";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(779, 16);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.OpeTypeGroup);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 16);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(779, 91);
            this.panel2.TabIndex = 9;
            // 
            // OpeTypeGroup
            // 
            this.OpeTypeGroup.Controls.Add(this.lbInfo);
            this.OpeTypeGroup.Controls.Add(this.rdDowned);
            this.OpeTypeGroup.Controls.Add(this.rdUnupdate);
            this.OpeTypeGroup.Controls.Add(this.rdUndel);
            this.OpeTypeGroup.Controls.Add(this.rbUndown);
            this.OpeTypeGroup.Controls.Add(this.LbDevInfo);
            this.OpeTypeGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpeTypeGroup.Location = new System.Drawing.Point(0, 0);
            this.OpeTypeGroup.Name = "OpeTypeGroup";
            this.OpeTypeGroup.Size = new System.Drawing.Size(779, 91);
            this.OpeTypeGroup.TabIndex = 2;
            this.OpeTypeGroup.TabStop = false;
            this.OpeTypeGroup.Text = "操作方式";
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(82, 31);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(53, 12);
            this.lbInfo.TabIndex = 5;
            this.lbInfo.Text = "设备信息";
            // 
            // rdDowned
            // 
            this.rdDowned.AutoSize = true;
            this.rdDowned.Location = new System.Drawing.Point(678, 30);
            this.rdDowned.Name = "rdDowned";
            this.rdDowned.Size = new System.Drawing.Size(59, 16);
            this.rdDowned.TabIndex = 4;
            this.rdDowned.TabStop = true;
            this.rdDowned.Text = "已下载";
            this.rdDowned.UseVisualStyleBackColor = true;
            this.rdDowned.CheckedChanged += new System.EventHandler(this.rdDowned_CheckedChanged);
            // 
            // rdUnupdate
            // 
            this.rdUnupdate.AutoSize = true;
            this.rdUnupdate.Location = new System.Drawing.Point(583, 30);
            this.rdUnupdate.Name = "rdUnupdate";
            this.rdUnupdate.Size = new System.Drawing.Size(59, 16);
            this.rdUnupdate.TabIndex = 3;
            this.rdUnupdate.TabStop = true;
            this.rdUnupdate.Text = "未更新";
            this.rdUnupdate.UseVisualStyleBackColor = true;
            this.rdUnupdate.CheckedChanged += new System.EventHandler(this.rdUnupdate_CheckedChanged);
            // 
            // rdUndel
            // 
            this.rdUndel.AutoSize = true;
            this.rdUndel.Location = new System.Drawing.Point(486, 30);
            this.rdUndel.Name = "rdUndel";
            this.rdUndel.Size = new System.Drawing.Size(59, 16);
            this.rdUndel.TabIndex = 2;
            this.rdUndel.TabStop = true;
            this.rdUndel.Text = "未删除";
            this.rdUndel.UseVisualStyleBackColor = true;
            this.rdUndel.CheckedChanged += new System.EventHandler(this.rdUndel_CheckedChanged);
            // 
            // rbUndown
            // 
            this.rbUndown.AutoSize = true;
            this.rbUndown.Checked = true;
            this.rbUndown.Location = new System.Drawing.Point(400, 30);
            this.rbUndown.Name = "rbUndown";
            this.rbUndown.Size = new System.Drawing.Size(59, 16);
            this.rbUndown.TabIndex = 1;
            this.rbUndown.TabStop = true;
            this.rbUndown.Text = "未下载";
            this.rbUndown.UseVisualStyleBackColor = true;
            this.rbUndown.CheckedChanged += new System.EventHandler(this.rbUndown_CheckedChanged);
            // 
            // LbDevInfo
            // 
            this.LbDevInfo.AutoSize = true;
            this.LbDevInfo.Location = new System.Drawing.Point(16, 32);
            this.LbDevInfo.Name = "LbDevInfo";
            this.LbDevInfo.Size = new System.Drawing.Size(59, 12);
            this.LbDevInfo.TabIndex = 0;
            this.LbDevInfo.Text = "当前设备:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.WhiteListGroup);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 107);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(779, 378);
            this.panel3.TabIndex = 10;
            // 
            // WhiteListGroup
            // 
            this.WhiteListGroup.Controls.Add(this.gdValList);
            this.WhiteListGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WhiteListGroup.Location = new System.Drawing.Point(0, 0);
            this.WhiteListGroup.Name = "WhiteListGroup";
            this.WhiteListGroup.Size = new System.Drawing.Size(779, 378);
            this.WhiteListGroup.TabIndex = 2;
            this.WhiteListGroup.TabStop = false;
            this.WhiteListGroup.Text = "白名单";
            // 
            // gdValList
            // 
            this.gdValList.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gdValList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gdValList.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gdValList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdValList.Dock = System.Windows.Forms.DockStyle.Top;
            this.gdValList.Location = new System.Drawing.Point(3, 17);
            this.gdValList.Name = "gdValList";
            this.gdValList.RowTemplate.Height = 23;
            this.gdValList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdValList.Size = new System.Drawing.Size(773, 346);
            this.gdValList.TabIndex = 0;
            this.gdValList.Tag = "@db=有效卡";
            // 
            // FrmValCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 526);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnDelOne);
            this.Controls.Add(this.BtnDelAll);
            this.Controls.Add(this.BtnReadOne);
            this.Controls.Add(this.BtnDownOne);
            this.Controls.Add(this.BtnDownAll);
            this.Name = "FrmValCard";
            this.Text = "有效卡";
            this.Load += new System.EventHandler(this.FrmValCard_Load);
            this.panel2.ResumeLayout(false);
            this.OpeTypeGroup.ResumeLayout(false);
            this.OpeTypeGroup.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.WhiteListGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gdValList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnDownAll;
        private System.Windows.Forms.Button BtnDownOne;
        private System.Windows.Forms.Button BtnReadOne;
        private System.Windows.Forms.Button BtnDelAll;
        private System.Windows.Forms.Button BtnDelOne;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox OpeTypeGroup;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.RadioButton rdDowned;
        private System.Windows.Forms.RadioButton rdUnupdate;
        private System.Windows.Forms.RadioButton rdUndel;
        private System.Windows.Forms.RadioButton rbUndown;
        private System.Windows.Forms.Label LbDevInfo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox WhiteListGroup;
        private System.Windows.Forms.DataGridView gdValList;
    }
}