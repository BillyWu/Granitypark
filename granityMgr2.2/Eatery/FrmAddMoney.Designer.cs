namespace Granity.granityMgr.Eatery
{
    partial class FrmAddMoney
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
            this.moneyList = new System.Windows.Forms.ListBox();
            this.panelControl6 = new DevExpress.XtraEditors.PanelControl();
            this.btnLeft = new DevExpress.XtraEditors.SimpleButton();
            this.btnLeftAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnRight = new DevExpress.XtraEditors.SimpleButton();
            this.btnRigthAll = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            this.detpList = new System.Windows.Forms.ListBox();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnclose = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.cboDept = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtMoney = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
            this.panelControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
            this.panelControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDept.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMoney.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.panelControl3);
            this.panelControl1.Controls.Add(this.panelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(806, 653);
            this.panelControl1.TabIndex = 0;
            // 
            // panelControl3
            // 
            this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl3.Controls.Add(this.panelControl5);
            this.panelControl3.Controls.Add(this.panelControl4);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(2, 53);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(802, 598);
            this.panelControl3.TabIndex = 1;
            // 
            // panelControl5
            // 
            this.panelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl5.Controls.Add(this.moneyList);
            this.panelControl5.Controls.Add(this.panelControl6);
            this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl5.Location = new System.Drawing.Point(321, 0);
            this.panelControl5.Name = "panelControl5";
            this.panelControl5.Size = new System.Drawing.Size(481, 598);
            this.panelControl5.TabIndex = 3;
            // 
            // moneyList
            // 
            this.moneyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moneyList.FormattingEnabled = true;
            this.moneyList.ItemHeight = 14;
            this.moneyList.Location = new System.Drawing.Point(71, 0);
            this.moneyList.Name = "moneyList";
            this.moneyList.Size = new System.Drawing.Size(410, 592);
            this.moneyList.TabIndex = 5;
            // 
            // panelControl6
            // 
            this.panelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl6.Controls.Add(this.btnLeft);
            this.panelControl6.Controls.Add(this.btnLeftAll);
            this.panelControl6.Controls.Add(this.btnRight);
            this.panelControl6.Controls.Add(this.btnRigthAll);
            this.panelControl6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl6.Location = new System.Drawing.Point(0, 0);
            this.panelControl6.Name = "panelControl6";
            this.panelControl6.Size = new System.Drawing.Size(71, 598);
            this.panelControl6.TabIndex = 4;
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(11, 402);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(45, 26);
            this.btnLeft.TabIndex = 32;
            this.btnLeft.Text = "<";
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnLeftAll
            // 
            this.btnLeftAll.Location = new System.Drawing.Point(8, 270);
            this.btnLeftAll.Name = "btnLeftAll";
            this.btnLeftAll.Size = new System.Drawing.Size(45, 26);
            this.btnLeftAll.TabIndex = 31;
            this.btnLeftAll.Text = "<<";
            this.btnLeftAll.Click += new System.EventHandler(this.btnLeftAll_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(8, 153);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(45, 26);
            this.btnRight.TabIndex = 30;
            this.btnRight.Text = ">";
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnRigthAll
            // 
            this.btnRigthAll.Location = new System.Drawing.Point(8, 51);
            this.btnRigthAll.Name = "btnRigthAll";
            this.btnRigthAll.Size = new System.Drawing.Size(48, 26);
            this.btnRigthAll.TabIndex = 29;
            this.btnRigthAll.Text = ">>";
            this.btnRigthAll.Click += new System.EventHandler(this.btnRigthAll_Click);
            // 
            // panelControl4
            // 
            this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl4.Controls.Add(this.detpList);
            this.panelControl4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl4.Location = new System.Drawing.Point(0, 0);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Size = new System.Drawing.Size(321, 598);
            this.panelControl4.TabIndex = 2;
            // 
            // detpList
            // 
            this.detpList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detpList.FormattingEnabled = true;
            this.detpList.ItemHeight = 14;
            this.detpList.Location = new System.Drawing.Point(0, 0);
            this.detpList.Name = "detpList";
            this.detpList.Size = new System.Drawing.Size(321, 592);
            this.detpList.TabIndex = 1;
            this.detpList.Tag = "@db=补助管理";
            // 
            // panelControl2
            // 
            this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl2.Controls.Add(this.btnclose);
            this.panelControl2.Controls.Add(this.btnSave);
            this.panelControl2.Controls.Add(this.cboDept);
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Controls.Add(this.txtMoney);
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(2, 2);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(802, 51);
            this.panelControl2.TabIndex = 0;
            this.panelControl2.Tag = "db=批量补助";
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(455, 10);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(70, 26);
            this.btnclose.TabIndex = 31;
            this.btnclose.Text = "退出";
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(370, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(70, 26);
            this.btnSave.TabIndex = 30;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // cboDept
            // 
            this.cboDept.Location = new System.Drawing.Point(70, 14);
            this.cboDept.Name = "cboDept";
            this.cboDept.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDept.Properties.NullText = "";
            this.cboDept.Size = new System.Drawing.Size(108, 21);
            this.cboDept.TabIndex = 1;
            this.cboDept.Tag = "@fld=部门";
            this.cboDept.TextChanged += new System.EventHandler(this.cboDept_TextChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(201, 17);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(28, 14);
            this.labelControl1.TabIndex = 11;
            this.labelControl1.Text = "金额:";
            // 
            // txtMoney
            // 
            this.txtMoney.Location = new System.Drawing.Point(244, 14);
            this.txtMoney.Name = "txtMoney";
            this.txtMoney.Size = new System.Drawing.Size(101, 21);
            this.txtMoney.TabIndex = 2;
            this.txtMoney.TextChanged += new System.EventHandler(this.txtMoney_TextChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(38, 17);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(28, 14);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "部门:";
            // 
            // FrmAddMoney
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 653);
            this.Controls.Add(this.panelControl1);
            this.Name = "FrmAddMoney";
            this.Text = "补助管理";
            this.Load += new System.EventHandler(this.FrmAddMoney_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
            this.panelControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
            this.panelControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDept.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMoney.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LookUpEdit cboDept;
        private DevExpress.XtraEditors.TextEdit txtMoney;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private System.Windows.Forms.ListBox detpList;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.PanelControl panelControl5;
        private System.Windows.Forms.ListBox moneyList;
        private DevExpress.XtraEditors.PanelControl panelControl6;
        private DevExpress.XtraEditors.SimpleButton btnLeft;
        private DevExpress.XtraEditors.SimpleButton btnLeftAll;
        private DevExpress.XtraEditors.SimpleButton btnRight;
        private DevExpress.XtraEditors.SimpleButton btnRigthAll;
        private DevExpress.XtraEditors.SimpleButton btnclose;
    }
}