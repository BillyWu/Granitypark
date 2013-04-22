namespace Granity.CardOneCommi
{
    partial class CardReader
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStop = new System.Windows.Forms.Button();
            this.butReadCardNo = new System.Windows.Forms.Button();
            this.labStatus = new System.Windows.Forms.Label();
            this.txtCardNo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("宋体", 8F);
            this.btnStop.Location = new System.Drawing.Point(135, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(40, 21);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // butReadCardNo
            // 
            this.butReadCardNo.Enabled = false;
            this.butReadCardNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butReadCardNo.Font = new System.Drawing.Font("宋体", 8F);
            this.butReadCardNo.Location = new System.Drawing.Point(94, 3);
            this.butReadCardNo.Name = "butReadCardNo";
            this.butReadCardNo.Size = new System.Drawing.Size(40, 21);
            this.butReadCardNo.TabIndex = 7;
            this.butReadCardNo.Text = "读卡";
            this.butReadCardNo.UseVisualStyleBackColor = true;
            // 
            // labStatus
            // 
            this.labStatus.AutoSize = true;
            this.labStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labStatus.Font = new System.Drawing.Font("宋体", 8F);
            this.labStatus.ForeColor = System.Drawing.Color.DarkRed;
            this.labStatus.Location = new System.Drawing.Point(178, 8);
            this.labStatus.Name = "labStatus";
            this.labStatus.Size = new System.Drawing.Size(67, 11);
            this.labStatus.TabIndex = 6;
            this.labStatus.Text = "准备读卡...";
            // 
            // txtCardNo
            // 
            this.txtCardNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCardNo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCardNo.Location = new System.Drawing.Point(1, 3);
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.ReadOnly = true;
            this.txtCardNo.Size = new System.Drawing.Size(94, 21);
            this.txtCardNo.TabIndex = 5;
            // 
            // CardReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.butReadCardNo);
            this.Controls.Add(this.labStatus);
            this.Controls.Add(this.txtCardNo);
            this.Name = "CardReader";
            this.Size = new System.Drawing.Size(246, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button butReadCardNo;
        private System.Windows.Forms.Label labStatus;
        private System.Windows.Forms.TextBox txtCardNo;
    }
}
