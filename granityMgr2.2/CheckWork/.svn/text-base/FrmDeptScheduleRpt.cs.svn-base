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
    /// 部门排班查询
    /// </summary>
    public partial class FrmDeptScheduleRpt : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "查询管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmDeptScheduleRpt()
        {
            InitializeComponent();
        }
        private void FrmDeptScheduleRpt_Load(object sender, EventArgs e)
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
            LoadDept();
        }

        /// <summary>
        /// 加载部门
        /// </summary>
        public void LoadDept()
        {
            DataRow drDept = this.dsUnit.Tables["部门"].NewRow();
            drDept["名称"] = "全部";
            drDept["编号"] = string.Empty;
            this.dsUnit.Tables["部门"].Rows.Add(drDept);
            this.cboDept.Properties.DataSource = this.dsUnit.Tables["部门"];
            this.cboDept.Properties.DisplayMember = "名称";
            this.cboDept.Properties.ValueMember = "编号";
            this.cboDept.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
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
        /// 时间段查询,部门，班制 检索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbDeptRpt.DataSource as DataTable;
            if (null == tab) return;
            string filter = "a.日期>=(SELECT convert(datetime,'{0}',121)) and a.日期<=(SELECT convert(datetime, '{1}',121))  and (c.名称 like '%{2}%')";
            filter = string.Format(filter, this.cboStart.Text.Replace("'", "''"), this.cboEnd.Text.Replace("'", "''"), this.cboDept.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRpt_Click(object sender, EventArgs e)
        {
            this.dbDeptRpt.ShowPrintPreview();
        }
    }
}