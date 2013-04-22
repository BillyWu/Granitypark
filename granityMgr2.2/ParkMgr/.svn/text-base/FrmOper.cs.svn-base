using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using System.Drawing.Printing;
using Granity.winTools;
using DevExpress.XtraEditors;

namespace Granity.parkStation
{
    public partial class FrmOper : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "操作员交接班记录";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmOper()
        {
            InitializeComponent();
        }

        private void FrmOper_Load(object sender, EventArgs e)
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
            LoadOper(cboEndEmp);
            LoadOper(cboStartEmp);
        }
        /// <summary>
        /// 加载操作员
        /// </summary>s
        public void LoadOper(DevExpress.XtraEditors.LookUpEdit cbo)
        {
            DataRow drCardType = this.dsUnit.Tables["操作员"].NewRow();
            drCardType["帐号"] = "帐号";
            drCardType["编号"] = 1;
            this.dsUnit.Tables["操作员"].Rows.Add(drCardType);
            cbo.Properties.DataSource = this.dsUnit.Tables["操作员"];
            cbo.Properties.DisplayMember = "帐号";
            cbo.Properties.ValueMember = "编号";
            cbo.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("帐号", "帐号", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
        }
        private void BtClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string StartDtTime = this.cboStartDate.Text + " " + this.cboStartTime.Text;
            string EndDtTime = this.cboEndDate.Text + " " + this.cboEndTime.Text;
            //获取当前单元名称
            DataTable tab = this.dbGrid.DataSource as DataTable;
            if (null == tab) return;
            string filter = "";
            filter = "a.接班时间 between '{0}' and '{1}'and a.接班人 like '%{2}%'and a.交班人 like '%{3}%'";
            filter = string.Format(filter, StartDtTime.Replace("'", "''"), EndDtTime.Replace("'", "''"), this.cboStartEmp.Text.Replace("'", "''"), this.cboEndEmp.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            this.dbGrid.ShowPrintPreview();
        }
    }
}