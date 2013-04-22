namespace Granity.granityMgr.Eatery
{
    partial class FrmCashRegisterII
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
            this.BtCancel = new DevExpress.XtraEditors.SimpleButton();
            this.BtSave = new DevExpress.XtraEditors.SimpleButton();
            this.BtDel = new DevExpress.XtraEditors.SimpleButton();
            this.BtAdd = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.grdCashList = new DevExpress.XtraGrid.GridControl();
            this.grdviewCashList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCashList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdviewCashList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.VistaTimeProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.BtCancel);
            this.panelControl1.Controls.Add(this.BtSave);
            this.panelControl1.Controls.Add(this.BtDel);
            this.panelControl1.Controls.Add(this.BtAdd);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(747, 47);
            this.panelControl1.TabIndex = 2;
            // 
            // BtCancel
            // 
            this.BtCancel.Location = new System.Drawing.Point(354, 9);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(110, 31);
            this.BtCancel.TabIndex = 0;
            this.BtCancel.Text = "退出";
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // BtSave
            // 
            this.BtSave.Location = new System.Drawing.Point(238, 9);
            this.BtSave.Name = "BtSave";
            this.BtSave.Size = new System.Drawing.Size(110, 31);
            this.BtSave.TabIndex = 0;
            this.BtSave.Text = "保存";
            this.BtSave.Click += new System.EventHandler(this.BtSave_Click);
            // 
            // BtDel
            // 
            this.BtDel.Location = new System.Drawing.Point(122, 9);
            this.BtDel.Name = "BtDel";
            this.BtDel.Size = new System.Drawing.Size(110, 31);
            this.BtDel.TabIndex = 0;
            this.BtDel.Text = "删除";
            this.BtDel.Click += new System.EventHandler(this.BtDel_Click);
            // 
            // BtAdd
            // 
            this.BtAdd.Location = new System.Drawing.Point(5, 9);
            this.BtAdd.Name = "BtAdd";
            this.BtAdd.Size = new System.Drawing.Size(110, 31);
            this.BtAdd.TabIndex = 0;
            this.BtAdd.Text = "添加";
            this.BtAdd.Click += new System.EventHandler(this.BtAdd_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.grdCashList);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 47);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(747, 343);
            this.groupControl1.TabIndex = 3;
            this.groupControl1.Text = "现金明细";
            // 
            // grdCashList
            // 
            this.grdCashList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdCashList.Location = new System.Drawing.Point(2, 21);
            this.grdCashList.MainView = this.grdviewCashList;
            this.grdCashList.Name = "grdCashList";
            this.grdCashList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemDateEdit1});
            this.grdCashList.Size = new System.Drawing.Size(743, 320);
            this.grdCashList.TabIndex = 1;
            this.grdCashList.Tag = "@db=餐厅现金登记";
            this.grdCashList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdviewCashList});
            // 
            // grdviewCashList
            // 
            this.grdviewCashList.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.grdviewCashList.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdviewCashList.Appearance.Row.Options.UseTextOptions = true;
            this.grdviewCashList.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdviewCashList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.grdviewCashList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn2});
            this.grdviewCashList.GridControl = this.grdCashList;
            this.grdviewCashList.Name = "grdviewCashList";
            this.grdviewCashList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.grdviewCashList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.grdviewCashList.OptionsBehavior.KeepFocusedRowOnUpdate = false;
            this.grdviewCashList.OptionsView.ColumnAutoWidth = false;
            this.grdviewCashList.OptionsView.ShowGroupPanel = false;
            this.grdviewCashList.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grdviewCashList_InitNewRow);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "id";
            this.gridColumn1.FieldName = "ID";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Equals;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "餐厅名称";
            this.gridColumn3.FieldName = "餐厅ID";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 96;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "日期";
            this.gridColumn4.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn4.FieldName = "日期";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            this.gridColumn4.Width = 141;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "操作员";
            this.gridColumn5.FieldName = "操作员";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            this.gridColumn5.Width = 77;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "备注";
            this.gridColumn6.FieldName = "备注";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            this.gridColumn6.Width = 212;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "金额";
            this.gridColumn2.DisplayFormat.FormatString = "c1";
            this.gridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn2.FieldName = "金额";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 64;
            // 
            // repositoryItemDateEdit1
            // 
            this.repositoryItemDateEdit1.AutoHeight = false;
            this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
            this.repositoryItemDateEdit1.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // FrmCashRegisterII
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 390);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.panelControl1);
            this.Name = "FrmCashRegisterII";
            this.Text = "现金收入登记";
            this.Load += new System.EventHandler(this.FrmCashRegisterII_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdCashList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdviewCashList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton BtSave;
        private DevExpress.XtraEditors.SimpleButton BtDel;
        private DevExpress.XtraEditors.SimpleButton BtAdd;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl grdCashList;
        private DevExpress.XtraGrid.Views.Grid.GridView grdviewCashList;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.SimpleButton BtCancel;
    }
}