namespace 停车场管理系统
{
    partial class FormManeySearch
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
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.dtStarTime = new System.Windows.Forms.DateTimePicker();
            this.dtStarDate = new System.Windows.Forms.DateTimePicker();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtEndTime = new System.Windows.Forms.DateTimePicker();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtCardID = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.cmbCardClass = new System.Windows.Forms.ComboBox();
            this.cmbAdmin = new System.Windows.Forms.ComboBox();
            this.cmbCar = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtJLCount = new System.Windows.Forms.TextBox();
            this.txtManeyAll = new System.Windows.Forms.TextBox();
            this.txtcar_tid = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(680, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "查询";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(680, 62);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(71, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "退出";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // dtStarTime
            // 
            this.dtStarTime.CustomFormat = "00:00:00";
            this.dtStarTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtStarTime.Location = new System.Drawing.Point(250, 6);
            this.dtStarTime.Name = "dtStarTime";
            this.dtStarTime.ShowUpDown = true;
            this.dtStarTime.Size = new System.Drawing.Size(74, 21);
            this.dtStarTime.TabIndex = 4;
            this.dtStarTime.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // dtStarDate
            // 
            this.dtStarDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtStarDate.Location = new System.Drawing.Point(83, 5);
            this.dtStarDate.Name = "dtStarDate";
            this.dtStarDate.ShowUpDown = true;
            this.dtStarDate.Size = new System.Drawing.Size(91, 21);
            this.dtStarDate.TabIndex = 5;
            this.dtStarDate.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // dtEndDate
            // 
            this.dtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtEndDate.Location = new System.Drawing.Point(83, 38);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.ShowUpDown = true;
            this.dtEndDate.Size = new System.Drawing.Size(91, 21);
            this.dtEndDate.TabIndex = 6;
            this.dtEndDate.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // dtEndTime
            // 
            this.dtEndTime.CustomFormat = "00:00:00";
            this.dtEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtEndTime.Location = new System.Drawing.Point(250, 39);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.ShowUpDown = true;
            this.dtEndTime.Size = new System.Drawing.Size(74, 21);
            this.dtEndTime.TabIndex = 7;
            this.dtEndTime.Value = new System.DateTime(2009, 8, 18, 0, 0, 0, 0);
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(558, 39);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(83, 21);
            this.txtUserName.TabIndex = 8;
            // 
            // txtCardID
            // 
            this.txtCardID.Location = new System.Drawing.Point(400, 5);
            this.txtCardID.Name = "txtCardID";
            this.txtCardID.Size = new System.Drawing.Size(87, 21);
            this.txtCardID.TabIndex = 9;
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(558, 6);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(83, 21);
            this.txtUserID.TabIndex = 10;
            // 
            // cmbCardClass
            // 
            this.cmbCardClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCardClass.FormattingEnabled = true;
            this.cmbCardClass.Location = new System.Drawing.Point(400, 40);
            this.cmbCardClass.Name = "cmbCardClass";
            this.cmbCardClass.Size = new System.Drawing.Size(87, 20);
            this.cmbCardClass.TabIndex = 11;
            // 
            // cmbAdmin
            // 
            this.cmbAdmin.FormattingEnabled = true;
            this.cmbAdmin.Location = new System.Drawing.Point(83, 70);
            this.cmbAdmin.Name = "cmbAdmin";
            this.cmbAdmin.Size = new System.Drawing.Size(90, 20);
            this.cmbAdmin.TabIndex = 12;
            // 
            // cmbCar
            // 
            this.cmbCar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCar.FormattingEnabled = true;
            this.cmbCar.Location = new System.Drawing.Point(249, 72);
            this.cmbCar.Name = "cmbCar";
            this.cmbCar.Size = new System.Drawing.Size(74, 20);
            this.cmbCar.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "开始日期：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "结束日期：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(179, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "结束时间：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(179, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "开始时间：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(342, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "卡号:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(342, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "卡类:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(493, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 21;
            this.label7.Text = "用户编号:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(493, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 22;
            this.label8.Text = "用户姓名:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "操作员:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(181, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "车型:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(330, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 25;
            this.label11.Text = "车牌号码:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(4, 101);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(789, 405);
            this.dataGridView1.TabIndex = 27;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(152, 520);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 12);
            this.label12.TabIndex = 28;
            this.label12.Text = "记录数:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(418, 521);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 29;
            this.label13.Text = "收费总额:";
            // 
            // txtJLCount
            // 
            this.txtJLCount.Location = new System.Drawing.Point(222, 514);
            this.txtJLCount.Name = "txtJLCount";
            this.txtJLCount.Size = new System.Drawing.Size(100, 21);
            this.txtJLCount.TabIndex = 30;
            // 
            // txtManeyAll
            // 
            this.txtManeyAll.Location = new System.Drawing.Point(495, 514);
            this.txtManeyAll.Name = "txtManeyAll";
            this.txtManeyAll.Size = new System.Drawing.Size(100, 21);
            this.txtManeyAll.TabIndex = 31;
            // 
            // txtcar_tid
            // 
            this.txtcar_tid.Location = new System.Drawing.Point(400, 73);
            this.txtcar_tid.Name = "txtcar_tid";
            this.txtcar_tid.Size = new System.Drawing.Size(87, 21);
            this.txtcar_tid.TabIndex = 32;
            // 
            // FormManeySearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 541);
            this.Controls.Add(this.txtcar_tid);
            this.Controls.Add(this.txtManeyAll);
            this.Controls.Add(this.txtJLCount);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbCar);
            this.Controls.Add(this.cmbAdmin);
            this.Controls.Add(this.cmbCardClass);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.txtCardID);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.dtEndTime);
            this.Controls.Add(this.dtEndDate);
            this.Controls.Add(this.dtStarDate);
            this.Controls.Add(this.dtStarTime);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormManeySearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "收费记录查询";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DateTimePicker dtStarDate;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtCardID;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.ComboBox cmbCardClass;
        private System.Windows.Forms.ComboBox cmbAdmin;
        private System.Windows.Forms.ComboBox cmbCar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.DateTimePicker dtStarTime;
        internal System.Windows.Forms.DateTimePicker dtEndTime;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtJLCount;
        private System.Windows.Forms.TextBox txtManeyAll;
        private System.Windows.Forms.TextBox txtcar_tid;
    }
}