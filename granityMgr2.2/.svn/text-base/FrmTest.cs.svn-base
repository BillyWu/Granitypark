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
namespace Granity.granityMgr
{
    public partial class FrmTest : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "基本设置";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;

        DataSet ds = null;
        public FrmTest()
        {
            InitializeComponent();
        }

        private void FrmTest_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.ds = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.ds);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < this.gridViewClass.RowCount;i++ )
            //{
            //    DataRow dr = this.gridViewClass.GetDataRow(i);
            //    XtraMessageBox.Show(dr["部门"].ToString());
            //}
        }
    }
}