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
namespace Granity.granityMgr.Eatery
{
    /// <summary>
    /// 充值补助退款查询
    /// </summary>
    public partial class FrmAddReduceMoney : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "充值补助退款查询";//项目
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmAddReduceMoney()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 根据条件查询相应的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtQuery_Click(object sender, EventArgs e)
        {
            NameObjectList ps = new NameObjectList();
            ps["StartDate"] = this.dateStart.EditValue;
            ps["EndDate"] = this.dateEnd.EditValue;
            ps["Type"] = this.lookType.EditValue;
            ps["Dept"] = this.lookDept.EditValue;
            ps["EmployNO"] = (object)this.txtEmployNo.Text;
            ParamManager.MergeParam(ps, this.paramwin,false);
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = this.grdConsumeList.DataSource as DataTable;
            tab.Clear();
            query.FillDataSet(tab.TableName, ps, this.ds);
        }

        private void FrmAddReduceMoney_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            this.dateStart.EditValue = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.dateEnd.EditValue = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
            ps["StartDate"] = this.dateStart.EditValue.ToString();
            ps["EndDate"] = this.dateEnd.EditValue.ToString();
            ds = bg.BuildDataset(this.unitItem, ps);
            bg.BindFld(this, ds);
            InitLookUp();
        }

        /// <summary>
        /// 初始化部门，分类下拉框
        /// </summary>
        private void InitLookUp()
        {
            DataRow drDept = this.ds.Tables["部门"].NewRow();
            drDept["名称"] = "全部";
            drDept["代码"] = string.Empty;
            this.ds.Tables["部门"].Rows.Add(drDept);
            this.lookDept.Properties.DataSource = this.ds.Tables["部门"];
            this.lookDept.Properties.DisplayMember = "名称";
            this.lookDept.Properties.ValueMember = "代码";
            this.lookDept.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("代码", "代码", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near),
              new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});

            DataRow drType = this.ds.Tables["类别"].NewRow();
            drType["名称"] = "全部";
            this.ds.Tables["类别"].Rows.Add(drType);
            this.lookType.Properties.DataSource = this.ds.Tables["类别"];
            this.lookType.Properties.DisplayMember = "名称";
            this.lookType.Properties.ValueMember = "名称";
            this.lookType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});

        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}