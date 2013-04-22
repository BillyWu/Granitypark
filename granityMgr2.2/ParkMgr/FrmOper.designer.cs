namespace Granity.parkStation
{
    partial class FrmOper
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
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.dbGrid = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cboStartDate = new DevExpress.XtraEditors.DateEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.cboEndTime = new DevExpress.XtraEditors.TimeEdit();
            this.cboStartTime = new DevExpress.XtraEditors.TimeEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.cboEndEmp = new DevExpress.XtraEditors.LookUpEdit();
            this.cboStartEmp = new DevExpress.XtraEditors.LookUpEdit();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnCheck = new DevExpress.XtraEditors.SimpleButton();
            this.cboEndDate = new DevExpress.XtraEditors.DateEdit();
            this.LbStartDt = new System.Windows.Forms.Label();
            this.LbEndDate = new System.Windows.Forms.Label();
            this.LbHander = new System.Windows.Forms.Label();
            this.LbTaker = new System.Windows.Forms.Label();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartDate.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndEmp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartEmp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDate.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDate.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.dbGrid);
            this.groupControl1.Controls.Add(this.panelControl1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(812, 651);
            this.groupControl1.TabIndex = 2;
            this.groupControl1.Text = "交班记录";
            // 
            // dbGrid
            // 
            this.dbGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbGrid.Location = new System.Drawing.Point(2, 109);
            this.dbGrid.MainView = this.gridView1;
            this.dbGrid.Name = "dbGrid";
            this.dbGrid.Size = new System.Drawing.Size(808, 540);
            this.dbGrid.TabIndex = 1;
            this.dbGrid.Tag = "@db=操作员交接班记录";
            this.dbGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1,
            this.gridView2});
            // 
            // gridView1
            // 
            this.gridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn15});
            this.gridView1.GridControl = this.dbGrid;
            this.gridView1.GroupPanelText = "交班记录";
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "门岗";
            this.gridColumn1.FieldName = "门岗";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "交班人";
            this.gridColumn2.FieldName = "交班人";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "接班人";
            this.gridColumn3.FieldName = "接班人";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "交班时间";
            this.gridColumn4.FieldName = "交班时间";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "接班时间";
            this.gridColumn5.FieldName = "接班时间";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "实收金额";
            this.gridColumn6.FieldName = "实收金额";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "应收金额";
            this.gridColumn7.FieldName = "应收金额";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "交班时场内停车";
            this.gridColumn8.FieldName = "交班时场内停车";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "接班时场内停车";
            this.gridColumn9.FieldName = "接班时场内停车";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "交班时临时车";
            this.gridColumn10.FieldName = "交班时临时车";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 9;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "接班时临时车";
            this.gridColumn11.FieldName = "接班时临时车";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 10;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "交班期卡车";
            this.gridColumn12.FieldName = "交班期卡车";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 11;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "接班期卡车";
            this.gridColumn13.FieldName = "接班期卡车";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 12;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "接班储值卡车";
            this.gridColumn14.FieldName = "接班储值卡车";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 13;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "交班储值卡车";
            this.gridColumn15.FieldName = "交班储值卡车";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 14;
            // 
            // gridView2
            // 
            this.gridView2.GridControl = this.dbGrid;
            this.gridView2.Name = "gridView2";
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.cboStartDate);
            this.panelControl1.Controls.Add(this.label2);
            this.panelControl1.Controls.Add(this.cboEndTime);
            this.panelControl1.Controls.Add(this.cboStartTime);
            this.panelControl1.Controls.Add(this.label1);
            this.panelControl1.Controls.Add(this.cboEndEmp);
            this.panelControl1.Controls.Add(this.cboStartEmp);
            this.panelControl1.Controls.Add(this.btnPrint);
            this.panelControl1.Controls.Add(this.btnClose);
            this.panelControl1.Controls.Add(this.btnCheck);
            this.panelControl1.Controls.Add(this.cboEndDate);
            this.panelControl1.Controls.Add(this.LbStartDt);
            this.panelControl1.Controls.Add(this.LbEndDate);
            this.panelControl1.Controls.Add(this.LbHander);
            this.panelControl1.Controls.Add(this.LbTaker);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(2, 21);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(808, 88);
            this.panelControl1.TabIndex = 0;
            // 
            // cboStartDate
            // 
            this.cboStartDate.EditValue = null;
            this.cboStartDate.Location = new System.Drawing.Point(67, 15);
            this.cboStartDate.Name = "cboStartDate";
            this.cboStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStartDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cboStartDate.Size = new System.Drawing.Size(101, 21);
            this.cboStartDate.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(475, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 14);
            this.label2.TabIndex = 31;
            this.label2.Text = "时间";
            // 
            // cboEndTime
            // 
            this.cboEndTime.EditValue = new System.DateTime(2011, 3, 1, 0, 0, 0, 0);
            this.cboEndTime.Location = new System.Drawing.Point(512, 15);
            this.cboEndTime.Name = "cboEndTime";
            this.cboEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cboEndTime.Size = new System.Drawing.Size(75, 21);
            this.cboEndTime.TabIndex = 4;
            // 
            // cboStartTime
            // 
            this.cboStartTime.EditValue = new System.DateTime(2011, 3, 1, 0, 0, 0, 0);
            this.cboStartTime.Location = new System.Drawing.Point(228, 15);
            this.cboStartTime.Name = "cboStartTime";
            this.cboStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cboStartTime.Size = new System.Drawing.Size(75, 21);
            this.cboStartTime.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(191, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 14);
            this.label1.TabIndex = 28;
            this.label1.Text = "时间";
            // 
            // cboEndEmp
            // 
            this.cboEndEmp.Location = new System.Drawing.Point(228, 54);
            this.cboEndEmp.Name = "cboEndEmp";
            this.cboEndEmp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboEndEmp.Properties.NullText = "";
            this.cboEndEmp.Size = new System.Drawing.Size(75, 21);
            this.cboEndEmp.TabIndex = 6;
            // 
            // cboStartEmp
            // 
            this.cboStartEmp.Location = new System.Drawing.Point(67, 54);
            this.cboStartEmp.Name = "cboStartEmp";
            this.cboStartEmp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStartEmp.Properties.NullText = "";
            this.cboStartEmp.Size = new System.Drawing.Size(101, 21);
            this.cboStartEmp.TabIndex = 5;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(454, 49);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(64, 26);
            this.btnPrint.TabIndex = 25;
            this.btnPrint.Text = "导出";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(524, 48);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 27);
            this.btnClose.TabIndex = 24;
            this.btnClose.Text = "退出";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(383, 48);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(63, 27);
            this.btnCheck.TabIndex = 23;
            this.btnCheck.Text = "查询";
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // cboEndDate
            // 
            this.cboEndDate.EditValue = null;
            this.cboEndDate.Location = new System.Drawing.Point(383, 15);
            this.cboEndDate.Name = "cboEndDate";
            this.cboEndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboEndDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cboEndDate.Size = new System.Drawing.Size(82, 21);
            this.cboEndDate.TabIndex = 3;
            // 
            // LbStartDt
            // 
            this.LbStartDt.AutoSize = true;
            this.LbStartDt.Location = new System.Drawing.Point(4, 18);
            this.LbStartDt.Name = "LbStartDt";
            this.LbStartDt.Size = new System.Drawing.Size(55, 14);
            this.LbStartDt.TabIndex = 15;
            this.LbStartDt.Text = "开始日期";
            // 
            // LbEndDate
            // 
            this.LbEndDate.AutoSize = true;
            this.LbEndDate.Location = new System.Drawing.Point(322, 18);
            this.LbEndDate.Name = "LbEndDate";
            this.LbEndDate.Size = new System.Drawing.Size(55, 14);
            this.LbEndDate.TabIndex = 16;
            this.LbEndDate.Text = "结束日期";
            // 
            // LbHander
            // 
            this.LbHander.AutoSize = true;
            this.LbHander.Location = new System.Drawing.Point(14, 57);
            this.LbHander.Name = "LbHander";
            this.LbHander.Size = new System.Drawing.Size(43, 14);
            this.LbHander.TabIndex = 17;
            this.LbHander.Text = "交班人";
            // 
            // LbTaker
            // 
            this.LbTaker.AutoSize = true;
            this.LbTaker.Location = new System.Drawing.Point(179, 57);
            this.LbTaker.Name = "LbTaker";
            this.LbTaker.Size = new System.Drawing.Size(43, 14);
            this.LbTaker.TabIndex = 18;
            this.LbTaker.Text = "接班人";
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "交班期卡车";
            this.gridColumn12.FieldName = "交班期卡车";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 11;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "接班期卡车";
            this.gridColumn13.FieldName = "接班期卡车";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 12;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "接班储值卡车";
            this.gridColumn14.FieldName = "接班储值卡车";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 13;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "交班储值卡车";
            this.gridColumn15.FieldName = "交班储值卡车";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 14;
            // 
            // FrmOper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 651);
            this.Controls.Add(this.groupControl1);
            this.Name = "FrmOper";
            this.Text = "操作员交接班";
            this.Load += new System.EventHandler(this.FrmOper_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartDate.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndEmp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStartEmp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDate.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDate.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.DateEdit cboEndDate;
        private System.Windows.Forms.Label LbStartDt;
        private System.Windows.Forms.Label LbEndDate;
        private System.Windows.Forms.Label LbHander;
        private System.Windows.Forms.Label LbTaker;
        private DevExpress.XtraGrid.GridControl dbGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnCheck;
        private DevExpress.XtraEditors.LookUpEdit cboEndEmp;
        private DevExpress.XtraEditors.LookUpEdit cboStartEmp;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TimeEdit cboEndTime;
        private DevExpress.XtraEditors.TimeEdit cboStartTime;
        private DevExpress.XtraEditors.DateEdit cboStartDate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
    }
}