namespace Granity.granityMgr.BaseDicinfo
{
    partial class FrmDorUsers
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
            this.treDoorAll = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treDeptUser = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btClose = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.treDoorAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treDeptUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treDoorAll
            // 
            this.treDoorAll.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Blue;
            this.treDoorAll.Appearance.FocusedCell.Options.UseForeColor = true;
            this.treDoorAll.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn2,
            this.treeListColumn5});
            this.treDoorAll.Dock = System.Windows.Forms.DockStyle.Left;
            this.treDoorAll.Location = new System.Drawing.Point(0, 0);
            this.treDoorAll.Name = "treDoorAll";
            this.treDoorAll.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.treDoorAll.OptionsBehavior.Editable = false;
            this.treDoorAll.OptionsBehavior.KeepSelectedOnClick = false;
            this.treDoorAll.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treDoorAll.OptionsView.ShowCheckBoxes = true;
            this.treDoorAll.OptionsView.ShowHorzLines = false;
            this.treDoorAll.OptionsView.ShowVertLines = false;
            this.treDoorAll.Size = new System.Drawing.Size(205, 426);
            this.treDoorAll.TabIndex = 7;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "所有门信息";
            this.treeListColumn2.FieldName = "部门人员的门权限信息";
            this.treeListColumn2.MinWidth = 35;
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 0;
            this.treeListColumn2.Width = 106;
            // 
            // treeListColumn5
            // 
            this.treeListColumn5.Caption = "id";
            this.treeListColumn5.FieldName = "id";
            this.treeListColumn5.Name = "treeListColumn5";
            // 
            // treDeptUser
            // 
            this.treDeptUser.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Blue;
            this.treDeptUser.Appearance.FocusedCell.Options.UseForeColor = true;
            this.treDeptUser.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn3,
            this.treeListColumn4});
            this.treDeptUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treDeptUser.Location = new System.Drawing.Point(0, 0);
            this.treDeptUser.Name = "treDeptUser";
            this.treDeptUser.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.treDeptUser.OptionsBehavior.Editable = false;
            this.treDeptUser.OptionsBehavior.KeepSelectedOnClick = false;
            this.treDeptUser.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treDeptUser.OptionsView.ShowCheckBoxes = true;
            this.treDeptUser.OptionsView.ShowHorzLines = false;
            this.treDeptUser.OptionsView.ShowVertLines = false;
            this.treDeptUser.Size = new System.Drawing.Size(226, 371);
            this.treDeptUser.TabIndex = 58;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.Caption = "所有部门信息";
            this.treeListColumn3.FieldName = "名称";
            this.treeListColumn3.MinWidth = 35;
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.OptionsColumn.AllowEdit = false;
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 0;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.Caption = "id";
            this.treeListColumn4.FieldName = "id";
            this.treeListColumn4.Name = "treeListColumn4";
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(126, 10);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(88, 33);
            this.btClose.TabIndex = 1;
            this.btClose.Text = "退出";
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.btClose);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(205, 371);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(226, 55);
            this.panelControl1.TabIndex = 59;
            // 
            // panelControl2
            // 
            this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl2.Controls.Add(this.treDeptUser);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(205, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(226, 371);
            this.panelControl2.TabIndex = 60;
            // 
            // FrmDorUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 426);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.treDoorAll);
            this.Name = "FrmDorUsers";
            this.Text = "查看门对应用户";
            this.Load += new System.EventHandler(this.FrmDorUsers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.treDoorAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treDeptUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList treDoorAll;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;
        private DevExpress.XtraTreeList.TreeList treDeptUser;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraEditors.SimpleButton btClose;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
    }
}