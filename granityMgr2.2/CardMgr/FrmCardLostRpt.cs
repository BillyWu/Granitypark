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
using DevExpress.XtraEditors;

namespace Granity.granityMgr.CardMgr
{
    /// <summary>
    /// 卡片挂失解挂记录
    /// </summary>
    public partial class FrmCardLostRpt : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "卡片挂失解挂记录";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmCardLostRpt()
        {
            InitializeComponent();
        }
        private void FrmCardLostRpt_Load(object sender, EventArgs e)
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
            Loadtype();
        }

        public void Loadtype()
        {
            DataTable dtType = new DataTable();
            dtType.Columns.Add("id");
            dtType.Columns.Add("type");
            string[] Type ={ "挂失", "解挂" };
            for (int i = 0; i < Type.Length; i++)
            {
                DataRow dr = dtType.NewRow();
                dr["id"] = i.ToString();
                dr["type"] = Type[i].ToString();
                dtType.Rows.Add(dr);
            }
            this.cboType.Properties.DataSource = dtType;//指定表
            this.cboType.Properties.DisplayMember = "type";//对应windowsr的Text
            this.cboType.Properties.ValueMember = "id";//对应windows的value
            this.cboType.Properties.Columns.AddRange(new
                         DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                             new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", " 类别", 100, 
                             DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
        }
        /// <summary>
        /// 条件查询 用户编号，卡号，姓名，部门检索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            //获取当前单元名称
            DataTable tab = this.dbGrid.DataSource as DataTable;
            if (null == tab) return;
            string filter = "";
            filter = "(a.用户编号 like '%{0}%'or a.卡号 like '%{0}%' or b.name like '%{0}%' or e.名称 like '%{0}%') and a.类别 like '%{1}%'";
            filter = string.Format(filter, this.tbcardno.Text.Replace("'", "''"), this.cboType.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
        }
        /// <summary>
        /// 关闭退出
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
        /// 数据导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcel_Click(object sender, EventArgs e)
        {
            this.dbGrid.ShowPrintPreview();

        }
    }
}