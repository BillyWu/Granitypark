namespace Granity.granityMgr.ParkMgr.Report
{
    partial class FrmCarOutTotal
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
            this.lookDate = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.groReport = new DevExpress.XtraEditors.GroupControl();
            this.BtQuery = new DevExpress.XtraEditors.SimpleButton();
            this.dateStart = new DevExpress.XtraEditors.DateEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnclose = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.lookDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lookDate
            // 
            this.lookDate.Location = new System.Drawing.Point(255, 16);
            this.lookDate.Name = "lookDate";
            this.lookDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lookDate.Properties.NullText = "";
            this.lookDate.Size = new System.Drawing.Size(127, 21);
            this.lookDate.TabIndex = 5;
            this.lookDate.Tag = "@db=�볡������ͳ��";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(213, 19);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(36, 14);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "����";
            // 
            // groReport
            // 
            this.groReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groReport.Location = new System.Drawing.Point(0, 53);
            this.groReport.Name = "groReport";
            this.groReport.Size = new System.Drawing.Size(827, 456);
            this.groReport.TabIndex = 9;
            this.groReport.Text = "��ѯ�ֽ��������";
            // 
            // BtQuery
            // 
            this.BtQuery.Location = new System.Drawing.Point(410, 14);
            this.BtQuery.Name = "BtQuery";
            this.BtQuery.Size = new System.Drawing.Size(75, 23);
            this.BtQuery.TabIndex = 4;
            this.BtQuery.Text = "��ѯ";
            this.BtQuery.Click += new System.EventHandler(this.BtQuery_Click_1);
            // 
            // dateStart
            // 
            this.dateStart.EditValue = null;
            this.dateStart.Location = new System.Drawing.Point(72, 16);
            this.dateStart.Name = "dateStart";
            this.dateStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateStart.Properties.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            this.dateStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateStart.Properties.EditFormat.FormatString = "yyyy-MM-dd HH:mm";
            this.dateStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateStart.Properties.Mask.EditMask = "yyyy-MM-dd HH:mm";
            this.dateStart.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dateStart.Size = new System.Drawing.Size(127, 21);
            this.dateStart.TabIndex = 1;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnclose);
            this.panelControl1.Controls.Add(this.lookDate);
            this.panelControl1.Controls.Add(this.BtQuery);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.dateStart);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(827, 53);
            this.panelControl1.TabIndex = 8;
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(513, 14);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(75, 23);
            this.btnclose.TabIndex = 6;
            this.btnclose.Text = "�˳�";
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(6, 19);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "��ʼʱ�䣺";
            // 
            // FrmCarOutTotal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 509);
            this.Controls.Add(this.groReport);
            this.Controls.Add(this.panelControl1);
            this.Name = "FrmCarOutTotal";
            this.Text = "����������ͳ��";
            this.Load += new System.EventHandler(this.FrmCarOutTotal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lookDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LookUpEdit lookDate;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.GroupControl groReport;
        private DevExpress.XtraEditors.SimpleButton BtQuery;
        private DevExpress.XtraEditors.DateEdit dateStart;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnclose;

    }
}