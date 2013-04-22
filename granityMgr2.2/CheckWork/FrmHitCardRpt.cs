using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using DevExpress.XtraEditors.Repository;

namespace Granity.granityMgr.CheckWork
{
    /// <summary>
    /// 员工打卡记录查询
    /// </summary>
    public partial class FrmHitCardRpt : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "查询管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmHitCardRpt()
        {
            InitializeComponent();
        }

        private void FrmHitCardRpt_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
        }

        /// <summary>
        /// 时间段查，姓名，用户编号，卡号 检索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbHitCard.DataSource as DataTable;
            if (null == tab) return;

            string filter = " a.时间>=(SELECT convert(datetime,'{0}',121)) and a.时间<=(SELECT convert(datetime,'{1}',121)) and(a.卡号 like'%{2}%' or a.用户编号 like '%{2}%'or b.NAME like'%{2}%' or c.名称 like '%{2}%' )";
            filter = string.Format(filter, this.cboStart.Text.Replace("'", "''"), this.cboEnd.Text.Replace("'", "''"), this.txtCheck.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOut_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRpt_Click(object sender, EventArgs e)
        {
            this.dbHitCard.ShowPrintPreview();
        }
    }
}