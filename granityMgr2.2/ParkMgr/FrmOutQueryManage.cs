using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using System.Drawing.Printing;
using System.Data.SqlClient;
using System.IO;
using DevExpress.XtraEditors;


namespace Granity.parkStation
{
    /// <summary>
    /// 出场纪录和查询
    /// </summary>
    public partial class FrmOutQueryManage : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "出场记录和图像";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmOutQueryManage()
        {
            InitializeComponent();
        }
        private void FrmQueryManage_Load(object sender, EventArgs e)
        {
            //读取业务单元和传递参数
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
            LoadCardTyep();
           
        }
        /// <summary>
        /// 加载卡片类型
        /// </summary>
        public void LoadCardTyep()
        {
            DataRow drCardType = this.dsUnit.Tables["卡片类型"].NewRow();
            drCardType["卡类"] = "卡类";
            drCardType["编号"] = 1;
            this.dsUnit.Tables["卡片类型"].Rows.Add(drCardType);
            this.CbCardtype.Properties.DataSource = this.dsUnit.Tables["卡片类型"];
            this.CbCardtype.Properties.DisplayMember = "卡类";
            this.CbCardtype.Properties.ValueMember = "编号";
            this.CbCardtype.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("卡类", "卡类", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
        }

       

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtQuery_Click(object sender, EventArgs e)
        {
            if (this.dateStart.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入开始时间", "系统提示！");
                return;
            }
            if (this.dateEnd.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入结束时间", "系统提示！");
                return;
            }
            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["StartTime"] = this.dateStart.EditValue.ToString();
            pstrans["EndTime"] = this.dateEnd.EditValue.ToString();
            pstrans["CardType"] = this.CbCardtype.EditValue != null ? this.CbCardtype.EditValue.ToString() : string.Empty;
            pstrans["CarNo"] = this.TxtCarNo.Text.Trim();
            pstrans["UsrNo"] = this.txtuserID.Text.Trim();
            pstrans["Name"] = this.txtuserName.Text.Trim();
            pstrans["CardNo"] = this.TxtCardNo.Text.Trim();
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            BindManager bg = new BindManager(this);
            DataSet ds = bg.BuildDataset(this.unitItem, pstrans);
            bg.BindFld(this, ds);
        }
        /// <summary>
        /// 出入场记录对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbGrid_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow drv = this.dbGrid.GetDataRow(e.RowHandle);
            FrmOut_InParkPic frmOut = new FrmOut_InParkPic();
            string img = this.dbGrid.GetDataRow(e.RowHandle)["入场图片"].ToString();
            BindManager.SetControlValue(drv, frmOut, "fld", null);
            frmOut.ShowDialog();

        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtClose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 打印记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtPrint_Click(object sender, EventArgs e)
        {
            this.gridIn.ShowPrintPreview();
        }

        private void dbGrid_DoubleClick(object sender, EventArgs e)
        {
            if (this.dbGrid.RowCount == 0)
                return;
            DataRow drv = this.dbGrid.GetDataRow(this.dbGrid.FocusedRowHandle);
            FrmOut_InParkPic frmOut = new FrmOut_InParkPic();
            string img = this.dbGrid.GetDataRow(this.dbGrid.FocusedRowHandle)["入场图片"].ToString();
            BindManager.SetControlValue(drv, frmOut, "fld", null);
            frmOut.ShowDialog();
        }

        private void dbGrid_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = e.RowHandle.ToString();
            }
        }
    }
}