namespace Granity.granityMgr.ParkMgr
{
    partial class FrmTable
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
            this.MyPrintDocument = new System.Drawing.Printing.PrintDocument();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CountRecordGroup = new System.Windows.Forms.GroupBox();
            this.lbcount = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.OnCloseBtn = new System.Windows.Forms.Button();
            this.PrintBtn = new System.Windows.Forms.Button();
            this.QueryBtn = new System.Windows.Forms.Button();
            this.cbRecordTp = new System.Windows.Forms.ComboBox();
            this.TableLab = new System.Windows.Forms.Label();
            this.SelectDayLab = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ListGroup = new System.Windows.Forms.GroupBox();
            this.RecordGrid = new System.Windows.Forms.DataGridView();
            this.panel2.SuspendLayout();
            this.CountRecordGroup.SuspendLayout();
            this.panel3.SuspendLayout();
            this.ListGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RecordGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // MyPrintDocument
            // 
            this.MyPrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.MyPrintDocument_PrintPage);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(801, 19);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CountRecordGroup);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(801, 65);
            this.panel2.TabIndex = 3;
            // 
            // CountRecordGroup
            // 
            this.CountRecordGroup.Controls.Add(this.lbcount);
            this.CountRecordGroup.Controls.Add(this.dtpDate);
            this.CountRecordGroup.Controls.Add(this.OnCloseBtn);
            this.CountRecordGroup.Controls.Add(this.PrintBtn);
            this.CountRecordGroup.Controls.Add(this.QueryBtn);
            this.CountRecordGroup.Controls.Add(this.cbRecordTp);
            this.CountRecordGroup.Controls.Add(this.TableLab);
            this.CountRecordGroup.Controls.Add(this.SelectDayLab);
            this.CountRecordGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CountRecordGroup.Location = new System.Drawing.Point(0, 0);
            this.CountRecordGroup.Name = "CountRecordGroup";
            this.CountRecordGroup.Size = new System.Drawing.Size(801, 65);
            this.CountRecordGroup.TabIndex = 1;
            this.CountRecordGroup.TabStop = false;
            this.CountRecordGroup.Text = "入场车流量统计";
            // 
            // lbcount
            // 
            this.lbcount.AutoSize = true;
            this.lbcount.Location = new System.Drawing.Point(700, 24);
            this.lbcount.Name = "lbcount";
            this.lbcount.Size = new System.Drawing.Size(53, 12);
            this.lbcount.TabIndex = 8;
            this.lbcount.Text = "总记录数";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(76, 19);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(151, 21);
            this.dtpDate.TabIndex = 7;
            // 
            // OnCloseBtn
            // 
            this.OnCloseBtn.Location = new System.Drawing.Point(574, 19);
            this.OnCloseBtn.Name = "OnCloseBtn";
            this.OnCloseBtn.Size = new System.Drawing.Size(43, 24);
            this.OnCloseBtn.TabIndex = 6;
            this.OnCloseBtn.Text = "退出";
            this.OnCloseBtn.UseVisualStyleBackColor = true;
            this.OnCloseBtn.Click += new System.EventHandler(this.OnCloseBtn_Click_1);
            // 
            // PrintBtn
            // 
            this.PrintBtn.Location = new System.Drawing.Point(508, 20);
            this.PrintBtn.Name = "PrintBtn";
            this.PrintBtn.Size = new System.Drawing.Size(49, 23);
            this.PrintBtn.TabIndex = 5;
            this.PrintBtn.Text = "打印";
            this.PrintBtn.UseVisualStyleBackColor = true;
            this.PrintBtn.Click += new System.EventHandler(this.PrintBtn_Click);
            // 
            // QueryBtn
            // 
            this.QueryBtn.Location = new System.Drawing.Point(431, 20);
            this.QueryBtn.Name = "QueryBtn";
            this.QueryBtn.Size = new System.Drawing.Size(54, 23);
            this.QueryBtn.TabIndex = 4;
            this.QueryBtn.Text = "统计";
            this.QueryBtn.UseVisualStyleBackColor = true;
            this.QueryBtn.Click += new System.EventHandler(this.QueryBtn_Click);
            // 
            // cbRecordTp
            // 
            this.cbRecordTp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRecordTp.FormattingEnabled = true;
            this.cbRecordTp.Items.AddRange(new object[] {
            "日报表",
            "月报表",
            "年报表"});
            this.cbRecordTp.Location = new System.Drawing.Point(268, 20);
            this.cbRecordTp.Name = "cbRecordTp";
            this.cbRecordTp.Size = new System.Drawing.Size(144, 20);
            this.cbRecordTp.TabIndex = 3;
            // 
            // TableLab
            // 
            this.TableLab.AutoSize = true;
            this.TableLab.Location = new System.Drawing.Point(233, 25);
            this.TableLab.Name = "TableLab";
            this.TableLab.Size = new System.Drawing.Size(29, 12);
            this.TableLab.TabIndex = 2;
            this.TableLab.Text = "报表";
            // 
            // SelectDayLab
            // 
            this.SelectDayLab.AutoSize = true;
            this.SelectDayLab.Location = new System.Drawing.Point(21, 24);
            this.SelectDayLab.Name = "SelectDayLab";
            this.SelectDayLab.Size = new System.Drawing.Size(59, 12);
            this.SelectDayLab.TabIndex = 0;
            this.SelectDayLab.Text = "选择日期:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ListGroup);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 84);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(801, 602);
            this.panel3.TabIndex = 4;
            // 
            // ListGroup
            // 
            this.ListGroup.Controls.Add(this.RecordGrid);
            this.ListGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListGroup.Location = new System.Drawing.Point(0, 0);
            this.ListGroup.Name = "ListGroup";
            this.ListGroup.Size = new System.Drawing.Size(801, 602);
            this.ListGroup.TabIndex = 2;
            this.ListGroup.TabStop = false;
            this.ListGroup.Text = "清单";
            // 
            // RecordGrid
            // 
            this.RecordGrid.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.RecordGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.RecordGrid.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.RecordGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RecordGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecordGrid.Location = new System.Drawing.Point(3, 17);
            this.RecordGrid.Name = "RecordGrid";
            this.RecordGrid.RowTemplate.Height = 23;
            this.RecordGrid.Size = new System.Drawing.Size(795, 582);
            this.RecordGrid.TabIndex = 0;
            // 
            // FrmTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 698);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FrmTable";
            this.Text = "车流量";
            this.Load += new System.EventHandler(this.FrmTable_Load);
            this.panel2.ResumeLayout(false);
            this.CountRecordGroup.ResumeLayout(false);
            this.CountRecordGroup.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ListGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RecordGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Drawing.Printing.PrintDocument MyPrintDocument;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox CountRecordGroup;
        private System.Windows.Forms.Label lbcount;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button OnCloseBtn;
        private System.Windows.Forms.Button PrintBtn;
        private System.Windows.Forms.Button QueryBtn;
        private System.Windows.Forms.ComboBox cbRecordTp;
        private System.Windows.Forms.Label TableLab;
        private System.Windows.Forms.Label SelectDayLab;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox ListGroup;
        private System.Windows.Forms.DataGridView RecordGrid;
    }
}