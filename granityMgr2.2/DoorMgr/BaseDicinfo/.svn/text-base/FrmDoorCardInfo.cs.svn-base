using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
namespace Granity.granityMgr.BaseDicinfo
{
    /// <summary>
    /// 门禁卡信息显示
    /// </summary>
    public partial class FrmDoorCardInfo : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门禁分组管理";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        public FrmDoorCardInfo()
        {
            InitializeComponent();
        }

        private void Frm_DoorCardInfo_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据0
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
        }

        private void BtAdd_Click(object sender, EventArgs e)
        {
            this.gridViewTimeList.AddNewRow();
            this.gridViewTimeList.RefreshData();
        }

        private void BtDel_Click(object sender, EventArgs e)
        {
            this.gridViewTimeList.DeleteRow(this.gridViewTimeList.FocusedRowHandle);
            this.gridViewTimeList.RefreshData();
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
            {
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                this.ds.Tables["门禁卡信息"].AcceptChanges();
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtPrint_Click(object sender, EventArgs e)
        {
            this.gridTimeList.ShowPrintPreview();
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}