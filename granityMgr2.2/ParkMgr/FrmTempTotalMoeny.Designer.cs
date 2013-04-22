namespace Granity.granityMgr.ParkMgr
{
    partial class FrmTempTotalMoeny
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
            this.BtClose = new DevExpress.XtraEditors.SimpleButton();
            this.txtConsumeMoney = new DevExpress.XtraEditors.TextEdit();
            this.txtLeaveMoney = new DevExpress.XtraEditors.TextEdit();
            this.textBox4 = new DevExpress.XtraEditors.TextEdit();
            this.txtRealCar = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.btCharge = new DevExpress.XtraEditors.SimpleButton();
            this.printFee = new System.Drawing.Printing.PrintDocument();
            this.textBox2 = new DevExpress.XtraEditors.TextEdit();
            this.txtCarType = new DevExpress.XtraEditors.TextEdit();
            this.txtCardNo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtCardType = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtAvtCar = new DevExpress.XtraEditors.TextEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtConsumeMoney.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeaveMoney.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRealCar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCarType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAvtCar.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // BtClose
            // 
            this.BtClose.Location = new System.Drawing.Point(220, 245);
            this.BtClose.Name = "BtClose";
            this.BtClose.Size = new System.Drawing.Size(96, 37);
            this.BtClose.TabIndex = 3;
            this.BtClose.Text = "退   出";
            this.BtClose.Click += new System.EventHandler(this.BtClose_Click);
            // 
            // txtConsumeMoney
            // 
            this.txtConsumeMoney.Location = new System.Drawing.Point(72, 201);
            this.txtConsumeMoney.Name = "txtConsumeMoney";
            this.txtConsumeMoney.Properties.AutoHeight = false;
            this.txtConsumeMoney.Properties.ReadOnly = true;
            this.txtConsumeMoney.Size = new System.Drawing.Size(334, 31);
            this.txtConsumeMoney.TabIndex = 45;
            this.txtConsumeMoney.Tag = "@pm=收费金额";
            // 
            // txtLeaveMoney
            // 
            this.txtLeaveMoney.Location = new System.Drawing.Point(72, 158);
            this.txtLeaveMoney.Name = "txtLeaveMoney";
            this.txtLeaveMoney.Properties.AutoHeight = false;
            this.txtLeaveMoney.Properties.ReadOnly = true;
            this.txtLeaveMoney.Size = new System.Drawing.Size(120, 31);
            this.txtLeaveMoney.TabIndex = 40;
            this.txtLeaveMoney.Tag = "@pm=卡片余额";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(286, 158);
            this.textBox4.Name = "textBox4";
            this.textBox4.Properties.AutoHeight = false;
            this.textBox4.Properties.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(120, 31);
            this.textBox4.TabIndex = 35;
            this.textBox4.Tag = "@pm=停车时长";
            // 
            // txtRealCar
            // 
            this.txtRealCar.Location = new System.Drawing.Point(72, 55);
            this.txtRealCar.Name = "txtRealCar";
            this.txtRealCar.Properties.AutoHeight = false;
            this.txtRealCar.Properties.ReadOnly = true;
            this.txtRealCar.Size = new System.Drawing.Size(120, 31);
            this.txtRealCar.TabIndex = 15;
            this.txtRealCar.Tag = "@pm=车牌号码";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(6, 209);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(60, 14);
            this.labelControl6.TabIndex = 48;
            this.labelControl6.Text = "停车费用：";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(6, 166);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(60, 14);
            this.labelControl5.TabIndex = 47;
            this.labelControl5.Text = "卡片余额：";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(12, 63);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(48, 14);
            this.labelControl8.TabIndex = 42;
            this.labelControl8.Text = "车   牌：";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(220, 166);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(60, 14);
            this.labelControl4.TabIndex = 43;
            this.labelControl4.Text = "停车时长：";
            // 
            // btCharge
            // 
            this.btCharge.Location = new System.Drawing.Point(118, 245);
            this.btCharge.Name = "btCharge";
            this.btCharge.Size = new System.Drawing.Size(96, 37);
            this.btCharge.TabIndex = 0;
            this.btCharge.Text = "收    费";
            this.btCharge.Click += new System.EventHandler(this.btCharge_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(286, 108);
            this.textBox2.Name = "textBox2";
            this.textBox2.Properties.AutoHeight = false;
            this.textBox2.Properties.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(120, 31);
            this.textBox2.TabIndex = 30;
            this.textBox2.Tag = "@pm=出场时间";
            // 
            // txtCarType
            // 
            this.txtCarType.Location = new System.Drawing.Point(286, 55);
            this.txtCarType.Name = "txtCarType";
            this.txtCarType.Properties.AutoHeight = false;
            this.txtCarType.Properties.ReadOnly = true;
            this.txtCarType.Size = new System.Drawing.Size(120, 31);
            this.txtCarType.TabIndex = 20;
            this.txtCarType.Tag = "@pm=车型名称";
            // 
            // txtCardNo
            // 
            this.txtCardNo.Location = new System.Drawing.Point(72, 9);
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Properties.AutoHeight = false;
            this.txtCardNo.Properties.ReadOnly = true;
            this.txtCardNo.Size = new System.Drawing.Size(120, 31);
            this.txtCardNo.TabIndex = 5;
            this.txtCardNo.TabStop = false;
            this.txtCardNo.Tag = "@pm=卡号";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(220, 116);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(60, 14);
            this.labelControl3.TabIndex = 46;
            this.labelControl3.Text = "出场时间：";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(228, 63);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(52, 14);
            this.labelControl2.TabIndex = 45;
            this.labelControl2.Text = "车    型：";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(14, 17);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(52, 14);
            this.labelControl1.TabIndex = 44;
            this.labelControl1.Text = "卡    号：";
            // 
            // txtCardType
            // 
            this.txtCardType.Location = new System.Drawing.Point(286, 9);
            this.txtCardType.Name = "txtCardType";
            this.txtCardType.Properties.AutoHeight = false;
            this.txtCardType.Properties.ReadOnly = true;
            this.txtCardType.Size = new System.Drawing.Size(120, 31);
            this.txtCardType.TabIndex = 10;
            this.txtCardType.Tag = "@pm=卡类名称";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(228, 17);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(52, 14);
            this.labelControl7.TabIndex = 58;
            this.labelControl7.Text = "卡    型：";
            // 
            // txtAvtCar
            // 
            this.txtAvtCar.Location = new System.Drawing.Point(72, 108);
            this.txtAvtCar.Name = "txtAvtCar";
            this.txtAvtCar.Properties.AutoHeight = false;
            this.txtAvtCar.Properties.ReadOnly = true;
            this.txtAvtCar.Size = new System.Drawing.Size(120, 31);
            this.txtAvtCar.TabIndex = 25;
            this.txtAvtCar.Tag = "@pm=入场时间";
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(6, 116);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(60, 14);
            this.labelControl9.TabIndex = 60;
            this.labelControl9.Text = "入场时间：";
            // 
            // FrmTempTotalMoeny
            // 
            this.AcceptButton = this.btCharge;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 285);
            this.Controls.Add(this.txtAvtCar);
            this.Controls.Add(this.labelControl9);
            this.Controls.Add(this.txtCardType);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.BtClose);
            this.Controls.Add(this.txtConsumeMoney);
            this.Controls.Add(this.txtLeaveMoney);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.txtRealCar);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.btCharge);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.txtCarType);
            this.Controls.Add(this.txtCardNo);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.MaximizeBox = false;
            this.Name = "FrmTempTotalMoeny";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "临时卡记费";
            this.Load += new System.EventHandler(this.FrmTempTotalMoeny_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtConsumeMoney.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeaveMoney.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRealCar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCarType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAvtCar.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton BtClose;
        private DevExpress.XtraEditors.TextEdit txtConsumeMoney;
        private DevExpress.XtraEditors.TextEdit txtLeaveMoney;
        private DevExpress.XtraEditors.TextEdit textBox4;
        private DevExpress.XtraEditors.TextEdit txtRealCar;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btCharge;
        private System.Drawing.Printing.PrintDocument printFee;
        private DevExpress.XtraEditors.TextEdit textBox2;
        private DevExpress.XtraEditors.TextEdit txtCarType;
        private DevExpress.XtraEditors.TextEdit txtCardNo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtCardType;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit txtAvtCar;
        private DevExpress.XtraEditors.LabelControl labelControl9;
    }
}