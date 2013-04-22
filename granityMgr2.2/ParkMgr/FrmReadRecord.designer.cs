namespace Granity.parkStation.cardManager
{
    partial class FrmReadRecord
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbDevInfo = new System.Windows.Forms.Label();
            this.lbldevname = new System.Windows.Forms.Label();
            this.btPick = new System.Windows.Forms.Button();
            this.rdbRetry = new System.Windows.Forms.RadioButton();
            this.rdbNormal = new System.Windows.Forms.RadioButton();
            this.gdRecord = new System.Windows.Forms.DataGridView();
            this.卡号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.卡类 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.车牌号码 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.车型 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.入场时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.出入场时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.收费金额 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.开闸方式 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.设备地址 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gdRecord)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDevInfo);
            this.groupBox1.Controls.Add(this.lbldevname);
            this.groupBox1.Controls.Add(this.btPick);
            this.groupBox1.Controls.Add(this.rdbRetry);
            this.groupBox1.Controls.Add(this.rdbNormal);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(654, 58);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "采集记录";
            // 
            // lbDevInfo
            // 
            this.lbDevInfo.AutoSize = true;
            this.lbDevInfo.Location = new System.Drawing.Point(469, 31);
            this.lbDevInfo.Name = "lbDevInfo";
            this.lbDevInfo.Size = new System.Drawing.Size(41, 12);
            this.lbDevInfo.TabIndex = 6;
            this.lbDevInfo.Text = "label4";
            // 
            // lbldevname
            // 
            this.lbldevname.AutoSize = true;
            this.lbldevname.Location = new System.Drawing.Point(374, 31);
            this.lbldevname.Name = "lbldevname";
            this.lbldevname.Size = new System.Drawing.Size(89, 12);
            this.lbldevname.TabIndex = 3;
            this.lbldevname.Text = "当前设备名称：";
            // 
            // btPick
            // 
            this.btPick.Location = new System.Drawing.Point(134, 14);
            this.btPick.Name = "btPick";
            this.btPick.Size = new System.Drawing.Size(75, 23);
            this.btPick.TabIndex = 2;
            this.btPick.Text = "采集";
            this.btPick.UseVisualStyleBackColor = true;
            this.btPick.Click += new System.EventHandler(this.btPick_Click);
            // 
            // rdbRetry
            // 
            this.rdbRetry.AutoSize = true;
            this.rdbRetry.Location = new System.Drawing.Point(70, 21);
            this.rdbRetry.Name = "rdbRetry";
            this.rdbRetry.Size = new System.Drawing.Size(47, 16);
            this.rdbRetry.TabIndex = 1;
            this.rdbRetry.Text = "补采";
            this.rdbRetry.UseVisualStyleBackColor = true;
            // 
            // rdbNormal
            // 
            this.rdbNormal.AutoSize = true;
            this.rdbNormal.Checked = true;
            this.rdbNormal.Location = new System.Drawing.Point(16, 21);
            this.rdbNormal.Name = "rdbNormal";
            this.rdbNormal.Size = new System.Drawing.Size(47, 16);
            this.rdbNormal.TabIndex = 0;
            this.rdbNormal.TabStop = true;
            this.rdbNormal.Text = "采集";
            this.rdbNormal.UseVisualStyleBackColor = true;
            // 
            // gdRecord
            // 
            this.gdRecord.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gdRecord.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gdRecord.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gdRecord.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gdRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdRecord.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.卡号,
            this.卡类,
            this.车牌号码,
            this.车型,
            this.入场时间,
            this.出入场时间,
            this.收费金额,
            this.开闸方式,
            this.设备地址});
            this.gdRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gdRecord.Location = new System.Drawing.Point(0, 58);
            this.gdRecord.Name = "gdRecord";
            this.gdRecord.ReadOnly = true;
            this.gdRecord.RowTemplate.Height = 23;
            this.gdRecord.Size = new System.Drawing.Size(654, 423);
            this.gdRecord.TabIndex = 1;
            this.gdRecord.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dbgrid_RowPostPaint);
            this.gdRecord.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dbgrid_DataError);
            // 
            // 卡号
            // 
            this.卡号.HeaderText = "卡号";
            this.卡号.Name = "卡号";
            this.卡号.ReadOnly = true;
            // 
            // 卡类
            // 
            this.卡类.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.卡类.HeaderText = "卡类";
            this.卡类.Name = "卡类";
            this.卡类.ReadOnly = true;
            this.卡类.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.卡类.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // 车牌号码
            // 
            this.车牌号码.HeaderText = "车牌号码";
            this.车牌号码.Name = "车牌号码";
            this.车牌号码.ReadOnly = true;
            // 
            // 车型
            // 
            this.车型.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.车型.HeaderText = "车型";
            this.车型.Name = "车型";
            this.车型.ReadOnly = true;
            this.车型.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.车型.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // 入场时间
            // 
            this.入场时间.HeaderText = "入场时间";
            this.入场时间.Name = "入场时间";
            this.入场时间.ReadOnly = true;
            // 
            // 出入场时间
            // 
            this.出入场时间.HeaderText = "出场时间";
            this.出入场时间.Name = "出入场时间";
            this.出入场时间.ReadOnly = true;
            // 
            // 收费金额
            // 
            this.收费金额.HeaderText = "收费金额";
            this.收费金额.Name = "收费金额";
            this.收费金额.ReadOnly = true;
            // 
            // 开闸方式
            // 
            this.开闸方式.HeaderText = "开闸方式";
            this.开闸方式.Name = "开闸方式";
            this.开闸方式.ReadOnly = true;
            this.开闸方式.Visible = false;
            // 
            // 设备地址
            // 
            this.设备地址.HeaderText = "设备地址";
            this.设备地址.Name = "设备地址";
            this.设备地址.ReadOnly = true;
            this.设备地址.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "卡号";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 87;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "车牌号码";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 87;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "入场时间";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 87;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "出场时间";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 88;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "收费金额";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 87;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "开闸方式";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Visible = false;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "设备地址";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Visible = false;
            // 
            // FrmReadRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 481);
            this.Controls.Add(this.gdRecord);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FrmReadRecord";
            this.Text = "采集记录";
            this.Load += new System.EventHandler(this.FrmReadRecord_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gdRecord)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView gdRecord;
        private System.Windows.Forms.RadioButton rdbRetry;
        private System.Windows.Forms.RadioButton rdbNormal;
        private System.Windows.Forms.Button btPick;
        private System.Windows.Forms.Label lbDevInfo;
        private System.Windows.Forms.Label lbldevname;
        private System.Windows.Forms.DataGridViewTextBoxColumn 卡号;
        private System.Windows.Forms.DataGridViewComboBoxColumn 卡类;
        private System.Windows.Forms.DataGridViewTextBoxColumn 车牌号码;
        private System.Windows.Forms.DataGridViewComboBoxColumn 车型;
        private System.Windows.Forms.DataGridViewTextBoxColumn 入场时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn 出入场时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn 收费金额;
        private System.Windows.Forms.DataGridViewTextBoxColumn 开闸方式;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备地址;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    }
}