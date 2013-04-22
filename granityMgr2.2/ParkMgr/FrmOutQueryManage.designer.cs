namespace Granity.parkStation
{
    partial class FrmOutQueryManage
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
            this.MyPrintDocument = new System.Drawing.Printing.PrintDocument();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.dtDev = new DevExpress.XtraEditors.LookUpEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtCarNo = new DevExpress.XtraEditors.TextEdit();
            this.CbCardtype = new DevExpress.XtraEditors.LookUpEdit();
            this.TxtCardNo = new DevExpress.XtraEditors.TextEdit();
            this.txtuserName = new DevExpress.XtraEditors.TextEdit();
            this.txtuserID = new DevExpress.XtraEditors.TextEdit();
            this.dateEnd = new DevExpress.XtraEditors.DateEdit();
            this.dateStart = new DevExpress.XtraEditors.DateEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.lbluid = new System.Windows.Forms.Label();
            this.LbCarNo = new System.Windows.Forms.Label();
            this.LbCardTp = new System.Windows.Forms.Label();
            this.LbCardno = new System.Windows.Forms.Label();
            this.EndDtLab = new System.Windows.Forms.Label();
            this.StartDtLab = new System.Windows.Forms.Label();
            this.BtPrint = new DevExpress.XtraEditors.SimpleButton();
            this.BtClose = new DevExpress.XtraEditors.SimpleButton();
            this.BtQuery = new DevExpress.XtraEditors.SimpleButton();
            this.picpark = new System.Windows.Forms.PictureBox();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.gridIn = new DevExpress.XtraGrid.GridControl();
            this.dbGrid = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtDev.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtCarNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CbCardtype.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtCardNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtuserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtuserID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picpark)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.dtDev);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Controls.Add(this.TxtCarNo);
            this.groupControl1.Controls.Add(this.CbCardtype);
            this.groupControl1.Controls.Add(this.TxtCardNo);
            this.groupControl1.Controls.Add(this.txtuserName);
            this.groupControl1.Controls.Add(this.txtuserID);
            this.groupControl1.Controls.Add(this.dateEnd);
            this.groupControl1.Controls.Add(this.dateStart);
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.lbluid);
            this.groupControl1.Controls.Add(this.LbCarNo);
            this.groupControl1.Controls.Add(this.LbCardTp);
            this.groupControl1.Controls.Add(this.LbCardno);
            this.groupControl1.Controls.Add(this.EndDtLab);
            this.groupControl1.Controls.Add(this.StartDtLab);
            this.groupControl1.Controls.Add(this.BtPrint);
            this.groupControl1.Controls.Add(this.BtClose);
            this.groupControl1.Controls.Add(this.BtQuery);
            this.groupControl1.Controls.Add(this.picpark);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(806, 274);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Tag = "@db=卡片类型";
            this.groupControl1.Text = "出场记录和图像";
            // 
            // dtDev
            // 
            this.dtDev.Location = new System.Drawing.Point(311, 176);
            this.dtDev.Name = "dtDev";
            this.dtDev.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDev.Properties.NullText = "";
            this.dtDev.Size = new System.Drawing.Size(137, 21);
            this.dtDev.TabIndex = 96;
            this.dtDev.Tag = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(258, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 14);
            this.label1.TabIndex = 95;
            this.label1.Text = "所属场:";
            // 
            // TxtCarNo
            // 
            this.TxtCarNo.Location = new System.Drawing.Point(87, 176);
            this.TxtCarNo.Name = "TxtCarNo";
            this.TxtCarNo.Size = new System.Drawing.Size(135, 21);
            this.TxtCarNo.TabIndex = 9;
            // 
            // CbCardtype
            // 
            this.CbCardtype.Location = new System.Drawing.Point(309, 131);
            this.CbCardtype.Name = "CbCardtype";
            this.CbCardtype.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.CbCardtype.Properties.NullText = "";
            this.CbCardtype.Size = new System.Drawing.Size(137, 21);
            this.CbCardtype.TabIndex = 8;
            this.CbCardtype.Tag = "@fld=卡型";
            // 
            // TxtCardNo
            // 
            this.TxtCardNo.Location = new System.Drawing.Point(87, 131);
            this.TxtCardNo.Name = "TxtCardNo";
            this.TxtCardNo.Size = new System.Drawing.Size(135, 21);
            this.TxtCardNo.TabIndex = 7;
            // 
            // txtuserName
            // 
            this.txtuserName.Location = new System.Drawing.Point(309, 87);
            this.txtuserName.Name = "txtuserName";
            this.txtuserName.Size = new System.Drawing.Size(137, 21);
            this.txtuserName.TabIndex = 6;
            // 
            // txtuserID
            // 
            this.txtuserID.Location = new System.Drawing.Point(87, 87);
            this.txtuserID.Name = "txtuserID";
            this.txtuserID.Size = new System.Drawing.Size(135, 21);
            this.txtuserID.TabIndex = 5;
            // 
            // dateEnd
            // 
            this.dateEnd.EditValue = null;
            this.dateEnd.Location = new System.Drawing.Point(309, 43);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEnd.Properties.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            this.dateEnd.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateEnd.Properties.EditFormat.FormatString = "yyyy-MM-dd HH:mm";
            this.dateEnd.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateEnd.Properties.Mask.EditMask = "yyyy-MM-dd HH:mm";
            this.dateEnd.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dateEnd.Size = new System.Drawing.Size(137, 21);
            this.dateEnd.TabIndex = 3;
            this.dateEnd.Tag = "";
            // 
            // dateStart
            // 
            this.dateStart.EditValue = null;
            this.dateStart.Location = new System.Drawing.Point(87, 43);
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
            this.dateStart.Size = new System.Drawing.Size(135, 21);
            this.dateStart.TabIndex = 1;
            this.dateStart.Tag = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(258, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 93;
            this.label2.Text = "姓名:";
            // 
            // lbluid
            // 
            this.lbluid.AutoSize = true;
            this.lbluid.Location = new System.Drawing.Point(44, 90);
            this.lbluid.Name = "lbluid";
            this.lbluid.Size = new System.Drawing.Size(35, 14);
            this.lbluid.TabIndex = 92;
            this.lbluid.Text = "编号:";
            // 
            // LbCarNo
            // 
            this.LbCarNo.AutoSize = true;
            this.LbCarNo.Location = new System.Drawing.Point(44, 179);
            this.LbCarNo.Name = "LbCarNo";
            this.LbCarNo.Size = new System.Drawing.Size(35, 14);
            this.LbCarNo.TabIndex = 91;
            this.LbCarNo.Text = "车牌:";
            // 
            // LbCardTp
            // 
            this.LbCardTp.AutoSize = true;
            this.LbCardTp.Location = new System.Drawing.Point(258, 138);
            this.LbCardTp.Name = "LbCardTp";
            this.LbCardTp.Size = new System.Drawing.Size(35, 14);
            this.LbCardTp.TabIndex = 90;
            this.LbCardTp.Text = "卡类:";
            // 
            // LbCardno
            // 
            this.LbCardno.AutoSize = true;
            this.LbCardno.Location = new System.Drawing.Point(44, 134);
            this.LbCardno.Name = "LbCardno";
            this.LbCardno.Size = new System.Drawing.Size(35, 14);
            this.LbCardno.TabIndex = 89;
            this.LbCardno.Text = "卡号:";
            // 
            // EndDtLab
            // 
            this.EndDtLab.AutoSize = true;
            this.EndDtLab.Location = new System.Drawing.Point(234, 46);
            this.EndDtLab.Name = "EndDtLab";
            this.EndDtLab.Size = new System.Drawing.Size(59, 14);
            this.EndDtLab.TabIndex = 88;
            this.EndDtLab.Text = "结束日期:";
            // 
            // StartDtLab
            // 
            this.StartDtLab.AutoSize = true;
            this.StartDtLab.Location = new System.Drawing.Point(22, 46);
            this.StartDtLab.Name = "StartDtLab";
            this.StartDtLab.Size = new System.Drawing.Size(59, 14);
            this.StartDtLab.TabIndex = 87;
            this.StartDtLab.Text = "开始日期:";
            // 
            // BtPrint
            // 
            this.BtPrint.Location = new System.Drawing.Point(144, 222);
            this.BtPrint.Name = "BtPrint";
            this.BtPrint.Size = new System.Drawing.Size(91, 32);
            this.BtPrint.TabIndex = 82;
            this.BtPrint.Text = "导出";
            this.BtPrint.Click += new System.EventHandler(this.BtPrint_Click);
            // 
            // BtClose
            // 
            this.BtClose.Location = new System.Drawing.Point(241, 222);
            this.BtClose.Name = "BtClose";
            this.BtClose.Size = new System.Drawing.Size(91, 32);
            this.BtClose.TabIndex = 81;
            this.BtClose.Text = "退出";
            this.BtClose.Click += new System.EventHandler(this.BtClose_Click);
            // 
            // BtQuery
            // 
            this.BtQuery.Location = new System.Drawing.Point(47, 222);
            this.BtQuery.Name = "BtQuery";
            this.BtQuery.Size = new System.Drawing.Size(91, 32);
            this.BtQuery.TabIndex = 80;
            this.BtQuery.Text = "查询";
            this.BtQuery.Click += new System.EventHandler(this.BtQuery_Click);
            // 
            // picpark
            // 
            this.picpark.BackgroundImage = global::Granity.granityMgr.Properties.Resources.bk;
            this.picpark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picpark.Location = new System.Drawing.Point(452, 24);
            this.picpark.Name = "picpark";
            this.picpark.Size = new System.Drawing.Size(352, 230);
            this.picpark.TabIndex = 70;
            this.picpark.TabStop = false;
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.gridIn);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(0, 274);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(806, 472);
            this.groupControl2.TabIndex = 1;
            this.groupControl2.Text = "清单";
            // 
            // gridIn
            // 
            this.gridIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridIn.Location = new System.Drawing.Point(2, 21);
            this.gridIn.MainView = this.dbGrid;
            this.gridIn.Name = "gridIn";
            this.gridIn.Size = new System.Drawing.Size(802, 449);
            this.gridIn.TabIndex = 0;
            this.gridIn.Tag = "@db=出场记录和图像";
            this.gridIn.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.dbGrid});
            // 
            // dbGrid
            // 
            this.dbGrid.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.dbGrid.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn11,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn7});
            this.dbGrid.GridControl = this.gridIn;
            this.dbGrid.IndicatorWidth = 30;
            this.dbGrid.Name = "dbGrid";
            this.dbGrid.OptionsBehavior.Editable = false;
            this.dbGrid.OptionsView.ColumnAutoWidth = false;
            this.dbGrid.OptionsView.ShowGroupPanel = false;
            this.dbGrid.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.dbGrid_CustomDrawRowIndicator);
            this.dbGrid.DoubleClick += new System.EventHandler(this.dbGrid_DoubleClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "姓名";
            this.gridColumn1.FieldName = "姓名";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "用户编号";
            this.gridColumn2.FieldName = "用户编号";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "卡号";
            this.gridColumn3.FieldName = "卡号";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "收费金额";
            this.gridColumn11.FieldName = "收费金额";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 3;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "卡类";
            this.gridColumn4.FieldName = "卡类";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "车牌";
            this.gridColumn5.FieldName = "车牌";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "车类";
            this.gridColumn6.FieldName = "车类";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "入场图片";
            this.gridColumn8.FieldName = "入场图片";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 10;
            this.gridColumn8.Width = 100;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "门岗";
            this.gridColumn9.FieldName = "门岗";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 12;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "操作员";
            this.gridColumn10.FieldName = "操作员";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 13;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "出场日期";
            this.gridColumn12.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn12.FieldName = "出场日期";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 9;
            this.gridColumn12.Width = 100;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "停车时长";
            this.gridColumn13.FieldName = "停车时长";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 7;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "出场图片";
            this.gridColumn14.FieldName = "出场图片";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 11;
            this.gridColumn14.Width = 100;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "入场日期";
            this.gridColumn7.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn7.FieldName = "入场日期";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 8;
            this.gridColumn7.Width = 100;
            // 
            // FrmOutQueryManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 746);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Name = "FrmOutQueryManage";
            this.Text = "出场记录和图像";
            this.Load += new System.EventHandler(this.FrmQueryManage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtDev.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtCarNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CbCardtype.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtCardNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtuserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtuserID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picpark)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dbGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Drawing.Printing.PrintDocument MyPrintDocument;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.SimpleButton BtPrint;
        private DevExpress.XtraEditors.SimpleButton BtClose;
        private DevExpress.XtraEditors.SimpleButton BtQuery;
        private System.Windows.Forms.PictureBox picpark;
        private DevExpress.XtraGrid.GridControl gridIn;
        private DevExpress.XtraGrid.Views.Grid.GridView dbGrid;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraEditors.TextEdit TxtCarNo;
        private DevExpress.XtraEditors.LookUpEdit CbCardtype;
        private DevExpress.XtraEditors.TextEdit TxtCardNo;
        private DevExpress.XtraEditors.TextEdit txtuserName;
        private DevExpress.XtraEditors.TextEdit txtuserID;
        private DevExpress.XtraEditors.DateEdit dateEnd;
        private DevExpress.XtraEditors.DateEdit dateStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbluid;
        private System.Windows.Forms.Label LbCarNo;
        private System.Windows.Forms.Label LbCardTp;
        private System.Windows.Forms.Label LbCardno;
        private System.Windows.Forms.Label EndDtLab;
        private System.Windows.Forms.Label StartDtLab;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.LookUpEdit dtDev;
    }
}