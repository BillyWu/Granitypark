namespace Granity.parkStation
{
    partial class FrmCarStallSet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCarStallSet));
            this.MyPrintDocument = new System.Drawing.Printing.PrintDocument();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dbGrid = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtName1 = new System.Windows.Forms.TextBox();
            this.chkno = new System.Windows.Forms.CheckBox();
            this.chkYes = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cboEnd = new System.Windows.Forms.DateTimePicker();
            this.cbostart = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.dtptime2 = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpstop2 = new System.Windows.Forms.DateTimePicker();
            this.Dpttop1 = new System.Windows.Forms.DateTimePicker();
            this.DtpTime1 = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.txtCard = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtNum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tsbAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbDelel = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).BeginInit();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // MyPrintDocument
            // 
            this.MyPrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.MyPrintDocument_PrintPage);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(822, 701);
            this.panel2.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.dbGrid);
            this.panel5.Location = new System.Drawing.Point(7, 196);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(791, 502);
            this.panel5.TabIndex = 9;
            // 
            // dbGrid
            // 
            this.dbGrid.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dbGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dbGrid.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dbGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dbGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbGrid.Location = new System.Drawing.Point(0, 0);
            this.dbGrid.Name = "dbGrid";
            this.dbGrid.ReadOnly = true;
            this.dbGrid.RowTemplate.Height = 23;
            this.dbGrid.Size = new System.Drawing.Size(791, 502);
            this.dbGrid.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Location = new System.Drawing.Point(3, 93);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(804, 97);
            this.panel3.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtName1);
            this.groupBox2.Controls.Add(this.chkno);
            this.groupBox2.Controls.Add(this.chkYes);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.cboEnd);
            this.groupBox2.Controls.Add(this.cbostart);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.dtptime2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.dtpstop2);
            this.groupBox2.Controls.Add(this.Dpttop1);
            this.groupBox2.Controls.Add(this.DtpTime1);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtNumber);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(804, 97);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Tag = "@db=时段设置";
            this.groupBox2.Text = "时段设置";
            // 
            // txtName1
            // 
            this.txtName1.Location = new System.Drawing.Point(242, 17);
            this.txtName1.Name = "txtName1";
            this.txtName1.Size = new System.Drawing.Size(89, 21);
            this.txtName1.TabIndex = 49;
            this.txtName1.Tag = "@fld=时段名称";
            // 
            // chkno
            // 
            this.chkno.AutoSize = true;
            this.chkno.Location = new System.Drawing.Point(735, 51);
            this.chkno.Name = "chkno";
            this.chkno.Size = new System.Drawing.Size(60, 16);
            this.chkno.TabIndex = 48;
            this.chkno.Tag = "@fld=星期日";
            this.chkno.Text = "星期日";
            this.chkno.UseVisualStyleBackColor = true;
            // 
            // chkYes
            // 
            this.chkYes.AutoSize = true;
            this.chkYes.Location = new System.Drawing.Point(672, 51);
            this.chkYes.Name = "chkYes";
            this.chkYes.Size = new System.Drawing.Size(60, 16);
            this.chkYes.TabIndex = 47;
            this.chkYes.Tag = "@fld=星期六";
            this.chkYes.Text = "星期六";
            this.chkYes.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(670, 27);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 46;
            this.label14.Text = "休息日:";
            // 
            // cboEnd
            // 
            this.cboEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.cboEnd.Location = new System.Drawing.Point(562, 55);
            this.cboEnd.Name = "cboEnd";
            this.cboEnd.Size = new System.Drawing.Size(89, 21);
            this.cboEnd.TabIndex = 45;
            this.cboEnd.Tag = "@fld=截止日期";
            // 
            // cbostart
            // 
            this.cbostart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.cbostart.Location = new System.Drawing.Point(407, 56);
            this.cbostart.Name = "cbostart";
            this.cbostart.Size = new System.Drawing.Size(89, 21);
            this.cbostart.TabIndex = 44;
            this.cbostart.Tag = "@fld=开始日期";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(502, 58);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 28;
            this.label13.Text = "截止日期:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(345, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 12);
            this.label12.TabIndex = 27;
            this.label12.Text = "起始日期:";
            // 
            // dtptime2
            // 
            this.dtptime2.Checked = false;
            this.dtptime2.CustomFormat = "HH:mm";
            this.dtptime2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtptime2.Location = new System.Drawing.Point(77, 52);
            this.dtptime2.Name = "dtptime2";
            this.dtptime2.ShowUpDown = true;
            this.dtptime2.Size = new System.Drawing.Size(89, 21);
            this.dtptime2.TabIndex = 26;
            this.dtptime2.Tag = "@fld=开始时间2";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 25;
            this.label11.Text = "起始时间2:";
            // 
            // dtpstop2
            // 
            this.dtpstop2.Checked = false;
            this.dtpstop2.CustomFormat = "HH:mm";
            this.dtpstop2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpstop2.Location = new System.Drawing.Point(242, 51);
            this.dtpstop2.Name = "dtpstop2";
            this.dtpstop2.ShowUpDown = true;
            this.dtpstop2.Size = new System.Drawing.Size(89, 21);
            this.dtpstop2.TabIndex = 23;
            this.dtpstop2.Tag = "@fld=截止时间2";
            // 
            // Dpttop1
            // 
            this.Dpttop1.Checked = false;
            this.Dpttop1.CustomFormat = "HH:mm";
            this.Dpttop1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Dpttop1.Location = new System.Drawing.Point(562, 18);
            this.Dpttop1.Name = "Dpttop1";
            this.Dpttop1.ShowUpDown = true;
            this.Dpttop1.Size = new System.Drawing.Size(89, 21);
            this.Dpttop1.TabIndex = 22;
            this.Dpttop1.Tag = "@fld=截止时间1";
            // 
            // DtpTime1
            // 
            this.DtpTime1.Checked = false;
            this.DtpTime1.CustomFormat = "HH:mm";
            this.DtpTime1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DtpTime1.Location = new System.Drawing.Point(407, 16);
            this.DtpTime1.Name = "DtpTime1";
            this.DtpTime1.ShowUpDown = true;
            this.DtpTime1.Size = new System.Drawing.Size(89, 21);
            this.DtpTime1.TabIndex = 21;
            this.DtpTime1.Tag = "@fld=开始时间1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(177, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 16;
            this.label10.Text = "截止时间2:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(502, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "截止时间1:";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(76, 16);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(89, 21);
            this.txtNumber.TabIndex = 8;
            this.txtNumber.Tag = "@fld=时段编号";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(345, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "起始时间1:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(177, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "名     称:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "编     号:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox1);
            this.panel4.Location = new System.Drawing.Point(3, 18);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(804, 64);
            this.panel4.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtRemark);
            this.groupBox1.Controls.Add(this.txtCard);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.txtNum);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(804, 64);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "@db=车位组设置";
            this.groupBox1.Text = "车位组设置";
            // 
            // txtRemark
            // 
            this.txtRemark.Location = new System.Drawing.Point(656, 18);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(139, 21);
            this.txtRemark.TabIndex = 11;
            this.txtRemark.Tag = "@fld=备注";
            // 
            // txtCard
            // 
            this.txtCard.Location = new System.Drawing.Point(458, 18);
            this.txtCard.Name = "txtCard";
            this.txtCard.Size = new System.Drawing.Size(139, 21);
            this.txtCard.TabIndex = 10;
            this.txtCard.Tag = "@fld=数量";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(248, 18);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(139, 21);
            this.txtName.TabIndex = 9;
            this.txtName.Tag = "@fld=名称";
            // 
            // txtNum
            // 
            this.txtNum.Location = new System.Drawing.Point(61, 18);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(139, 21);
            this.txtNum.TabIndex = 8;
            this.txtNum.Tag = "@fld=车组编号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(615, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "备注:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(405, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "车位数:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "名称:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "编号:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(822, 40);
            this.panel1.TabIndex = 6;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAdd,
            this.tsbDelel,
            this.tsbSave,
            this.toolStripButton1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(822, 25);
            this.toolStrip2.TabIndex = 5;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tsbAdd
            // 
            this.tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbAdd.Image")));
            this.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAdd.Name = "tsbAdd";
            this.tsbAdd.Size = new System.Drawing.Size(33, 22);
            this.tsbAdd.Text = "添加";
            this.tsbAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbDelel
            // 
            this.tsbDelel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDelel.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelel.Image")));
            this.tsbDelel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelel.Name = "tsbDelel";
            this.tsbDelel.Size = new System.Drawing.Size(33, 22);
            this.tsbDelel.Text = "删除";
            this.tsbDelel.Click += new System.EventHandler(this.tsbDelel_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(33, 22);
            this.tsbSave.Text = "保存";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(33, 22);
            this.toolStripButton1.Text = "打印";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // FrmCarStallSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 753);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FrmCarStallSet";
            this.Text = "车位设置";
            this.Load += new System.EventHandler(this.FrmCarStallSet_Load);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).EndInit();
            this.panel3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Drawing.Printing.PrintDocument MyPrintDocument;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView dbGrid;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.TextBox txtCard;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkno;
        private System.Windows.Forms.CheckBox chkYes;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DateTimePicker cboEnd;
        private System.Windows.Forms.DateTimePicker cbostart;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.DateTimePicker dtptime2;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.DateTimePicker dtpstop2;
        public System.Windows.Forms.DateTimePicker Dpttop1;
        public System.Windows.Forms.DateTimePicker DtpTime1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton tsbAdd;
        private System.Windows.Forms.ToolStripButton tsbDelel;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.TextBox txtName1;
    }
}