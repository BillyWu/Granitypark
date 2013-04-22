namespace Granity.granityMgr.CheckWork
{
    partial class FrmDeptClass
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
            this.gridControlClass = new DevExpress.XtraGrid.GridControl();
            this.gridViewClass = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewClass)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.BtCancel);
            this.panelControl1.Controls.Add(this.BtSave);
            this.panelControl1.Controls.Add(this.BtDel);
            this.panelControl1.Controls.Add(this.BtAdd);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(819, 44);
            this.panelControl1.TabIndex = 0;
            // 
            // BtCancel
            // 
            this.BtCancel.Location = new System.Drawing.Point(387, 8);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(110, 31);
            this.BtCancel.TabIndex = 3;
            this.BtCancel.Text = "退出";
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // BtSave
            // 
            this.BtSave.Location = new System.Drawing.Point(261, 8);
            this.BtSave.Name = "BtSave";
            this.BtSave.Size = new System.Drawing.Size(110, 31);
            this.BtSave.TabIndex = 3;
            this.BtSave.Text = "保存";
            this.BtSave.Click += new System.EventHandler(this.BtSave_Click);
            // 
            // BtDel
            // 
            this.BtDel.Location = new System.Drawing.Point(135, 8);
            this.BtDel.Name = "BtDel";
            this.BtDel.Size = new System.Drawing.Size(110, 31);
            this.BtDel.TabIndex = 2;
            this.BtDel.Text = "删除";
            this.BtDel.Click += new System.EventHandler(this.BtDel_Click);
            // 
            // BtAdd
            // 
            this.BtAdd.Location = new System.Drawing.Point(8, 8);
            this.BtAdd.Name = "BtAdd";
            this.BtAdd.Size = new System.Drawing.Size(110, 31);
            this.BtAdd.TabIndex = 1;
            this.BtAdd.Text = "添加";
            this.BtAdd.Click += new System.EventHandler(this.BtAdd_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.gridControlClass);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 44);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(819, 352);
            this.groupControl1.TabIndex = 1;
            this.groupControl1.Text = "部门班制";
            // 
            // gridControlClass
            // 
            this.gridControlClass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlClass.Location = new System.Drawing.Point(2, 21);
            this.gridControlClass.MainView = this.gridViewClass;
            this.gridControlClass.Name = "gridControlClass";
            this.gridControlClass.Size = new System.Drawing.Size(815, 329);
            this.gridControlClass.TabIndex = 1;
            this.gridControlClass.Tag = "@db=部门班制";
            this.gridControlClass.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewClass});
            // 
            // gridViewClass
            // 
            this.gridViewClass.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewClass.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewClass.Appearance.Row.Options.UseTextOptions = true;
            this.gridViewClass.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewClass.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridViewClass.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn18,
            this.gridColumn17,
            this.gridColumn19,
            this.gridColumn20,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gridViewClass.GridControl = this.gridControlClass;
            this.gridViewClass.Name = "gridViewClass";
            this.gridViewClass.OptionsBehavior.KeepFocusedRowOnUpdate = false;
            this.gridViewClass.OptionsView.ColumnAutoWidth = false;
            this.gridViewClass.OptionsView.ShowGroupPanel = false;
            this.gridViewClass.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewClass_CellValueChanged);
            // 
            // gridColumn18
            // 
            this.gridColumn18.Caption = "班制编号";
            this.gridColumn18.FieldName = "班制ID";
            this.gridColumn18.Name = "gridColumn18";
            this.gridColumn18.Visible = true;
            this.gridColumn18.VisibleIndex = 1;
            this.gridColumn18.Width = 79;
            // 
            // gridColumn17
            // 
            this.gridColumn17.Caption = "部门";
            this.gridColumn17.FieldName = "部门";
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.Visible = true;
            this.gridColumn17.VisibleIndex = 0;
            this.gridColumn17.Width = 89;
            // 
            // gridColumn19
            // 
            this.gridColumn19.Caption = "启动日期";
            this.gridColumn19.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.gridColumn19.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn19.FieldName = "启动日期";
            this.gridColumn19.Name = "gridColumn19";
            this.gridColumn19.Visible = true;
            this.gridColumn19.VisibleIndex = 2;
            this.gridColumn19.Width = 117;
            // 
            // gridColumn20
            // 
            this.gridColumn20.Caption = "ID";
            this.gridColumn20.FieldName = "ID";
            this.gridColumn20.Name = "gridColumn20";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "结束日期";
            this.gridColumn1.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn1.FieldName = "结束日期";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 3;
            this.gridColumn1.Width = 111;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "换班周期";
            this.gridColumn2.FieldName = "换班周期";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 4;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "周期单位";
            this.gridColumn3.FieldName = "周期单位";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Width = 71;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "加班单";
            this.gridColumn4.FieldName = "加班单";
            this.gridColumn4.Name = "gridColumn4";
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "节假日";
            this.gridColumn5.FieldName = "节假日";
            this.gridColumn5.Name = "gridColumn5";
            // 
            // FrmDeptClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 396);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.panelControl1);
            this.Name = "FrmDeptClass";
            this.Text = "部门班制";
            this.Load += new System.EventHandler(this.FrmDeptClass_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewClass)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gridControlClass;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewClass;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn19;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn20;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.SimpleButton BtSave;
        private DevExpress.XtraEditors.SimpleButton BtDel;
        private DevExpress.XtraEditors.SimpleButton BtAdd;
        private DevExpress.XtraEditors.SimpleButton BtCancel;

    }
}