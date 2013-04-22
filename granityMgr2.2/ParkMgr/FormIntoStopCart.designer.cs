namespace 停车场管理系统
{
    partial class FormIntoStopCart
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button5 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCarID = new System.Windows.Forms.ComboBox();
            this.cmbCardClass = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCardID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtStarDate = new System.Windows.Forms.DateTimePicker();
            this.dtStarTime = new System.Windows.Forms.DateTimePicker();
            this.tbnOut = new System.Windows.Forms.Button();
            this.tbnSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(365, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 167);
            this.panel1.TabIndex = 54;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 191);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(670, 357);
            this.dataGridView1.TabIndex = 53;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(172, 116);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(43, 23);
            this.button5.TabIndex = 52;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 122);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 51;
            this.label11.Text = "车牌号码:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 50;
            this.label6.Text = "卡类:";
            // 
            // cmbCarID
            // 
            this.cmbCarID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCarID.FormattingEnabled = true;
            this.cmbCarID.Location = new System.Drawing.Point(75, 118);
            this.cmbCarID.Name = "cmbCarID";
            this.cmbCarID.Size = new System.Drawing.Size(91, 20);
            this.cmbCarID.TabIndex = 49;
            // 
            // cmbCardClass
            // 
            this.cmbCardClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCardClass.FormattingEnabled = true;
            this.cmbCardClass.Location = new System.Drawing.Point(75, 84);
            this.cmbCardClass.Name = "cmbCardClass";
            this.cmbCardClass.Size = new System.Drawing.Size(91, 20);
            this.cmbCardClass.TabIndex = 48;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(171, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 47;
            this.label5.Text = "卡号:";
            // 
            // txtCardID
            // 
            this.txtCardID.Location = new System.Drawing.Point(242, 89);
            this.txtCardID.Name = "txtCardID";
            this.txtCardID.Size = new System.Drawing.Size(87, 21);
            this.txtCardID.TabIndex = 46;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(171, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 45;
            this.label4.Text = "开始时间：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 44;
            this.label3.Text = "结束时间：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 43;
            this.label2.Text = "结束日期：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 42;
            this.label1.Text = "开始日期：";
            // 
            // dtEndTime
            // 
            this.dtEndTime.CustomFormat = "00:00:00";
            this.dtEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtEndTime.Location = new System.Drawing.Point(242, 52);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.ShowUpDown = true;
            this.dtEndTime.Size = new System.Drawing.Size(87, 21);
            this.dtEndTime.TabIndex = 41;
            this.dtEndTime.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // dtEndDate
            // 
            this.dtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtEndDate.Location = new System.Drawing.Point(75, 51);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.ShowUpDown = true;
            this.dtEndDate.Size = new System.Drawing.Size(91, 21);
            this.dtEndDate.TabIndex = 40;
            this.dtEndDate.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // dtStarDate
            // 
            this.dtStarDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtStarDate.Location = new System.Drawing.Point(75, 18);
            this.dtStarDate.Name = "dtStarDate";
            this.dtStarDate.ShowUpDown = true;
            this.dtStarDate.Size = new System.Drawing.Size(91, 21);
            this.dtStarDate.TabIndex = 39;
            this.dtStarDate.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // dtStarTime
            // 
            this.dtStarTime.CustomFormat = "00:00:00";
            this.dtStarTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtStarTime.Location = new System.Drawing.Point(242, 19);
            this.dtStarTime.Name = "dtStarTime";
            this.dtStarTime.ShowUpDown = true;
            this.dtStarTime.Size = new System.Drawing.Size(87, 21);
            this.dtStarTime.TabIndex = 38;
            this.dtStarTime.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // tbnOut
            // 
            this.tbnOut.Location = new System.Drawing.Point(592, 81);
            this.tbnOut.Name = "tbnOut";
            this.tbnOut.Size = new System.Drawing.Size(75, 23);
            this.tbnOut.TabIndex = 37;
            this.tbnOut.Text = "退出";
            this.tbnOut.UseVisualStyleBackColor = true;
            // 
            // tbnSearch
            // 
            this.tbnSearch.Location = new System.Drawing.Point(592, 41);
            this.tbnSearch.Name = "tbnSearch";
            this.tbnSearch.Size = new System.Drawing.Size(75, 23);
            this.tbnSearch.TabIndex = 36;
            this.tbnSearch.Text = "查询";
            this.tbnSearch.UseVisualStyleBackColor = true;
            // 
            // FormIntoStopCart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 552);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbCarID);
            this.Controls.Add(this.cmbCardClass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCardID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtEndTime);
            this.Controls.Add(this.dtEndDate);
            this.Controls.Add(this.dtStarDate);
            this.Controls.Add(this.dtStarTime);
            this.Controls.Add(this.tbnOut);
            this.Controls.Add(this.tbnSearch);
            this.Name = "FormIntoStopCart";
            this.Text = "场内停车";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbCarID;
        private System.Windows.Forms.ComboBox cmbCardClass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCardID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.DateTimePicker dtEndTime;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.DateTimePicker dtStarDate;
        internal System.Windows.Forms.DateTimePicker dtStarTime;
        private System.Windows.Forms.Button tbnOut;
        private System.Windows.Forms.Button tbnSearch;
    }
}