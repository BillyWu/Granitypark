using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Estar.Common.Tools;
using Granity.winTools;
using Estar.Business.DataManager;

namespace Granity.granityMgr
{
    public partial class FrmDepart : Form
    {
        string unitName = "组织机构管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmDepart()
        {
            InitializeComponent();
        }
        private void FrmDepart_Load(object sender, EventArgs e)
        {
            //读取业务单元和传递参数
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitName = pstrans["name"].ToString();//单元
            //获取当前单元名称
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("组织机构管理", "@db=组织机构");
            //数据源
            string datasource = dict[this.unitName];
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            ps["UnitCode"] = "10";
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            dsUnit = bg.BuildDataset(this.unitItem, ps);
            bg.BindFld(this, dsUnit);
            //对树的操作
            bg.BindTrv(this.trvDept, this.dsUnit.Tables["组织机构"], "名称", "ID", "PID", "@名称={名称},@代码={代码},@序号={序号},@ParentCode={ParentCode},@独立管理={独立管理},@分类={分类},@性质={性质},@部门主管={部门主管},@部门职责={部门职责},@班次代码={班次代码},@班次名称={班次名称},@周一={周},@周二={周二},@周三={周三},@周四={周四},@周五={周五},@周六={周六},@周日={周日},@level={level}");
            this.trvDept.ExpandAll();
        }
        private void trvDept_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (null == e.Node)
                return;
            //查询明细数据
            DataTable tabDetail = this.dsUnit.Tables["组织机构"];
            string tag = Convert.ToString(e.Node.Tag);
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            ParamManager.MergeParam(ps, ParamManager.createParam(tag));
            ps["UnitCode"] = basefun.valtag(e.Node.Tag.ToString(), "代码");
            if (null != tabDetail) tabDetail.Clear();
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            query.FillDataSet("组织机构", ps, this.dsUnit);
            checkBox1.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["独立管理"].ToString());
            checkBox2.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周一"].ToString());
            checkBox3.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周二"].ToString());
            checkBox4.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周三"].ToString());
            checkBox5.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周四"].ToString());
            checkBox6.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周五"].ToString());
            checkBox7.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周六"].ToString());
            checkBox8.Checked = Convert.ToBoolean(this.dsUnit.Tables[0].Rows[0]["周日"].ToString());
            //if (null == tabChanged)
            //    return;

            //tabDetail = this.dsUnit.Tables["组织机构"];

            //TreeNode trnsel, trn;
            //trn = trnsel = e.Node;
            //string strdept = "";
            //while (null != trnsel)
            //{
            //    if (null != trn.FirstNode)
            //    {
            //        trn = trn.FirstNode;
            //        continue;
            //    }
            //    string t = Convert.ToString(trn.Tag);
            //    strdept += " or 部门='" + basefun.valtag(t, "部门") + "'";
            //    if (trnsel == trn)
            //        break;
            //    if (null != trn.NextNode)
            //        trn = trn.NextNode;
            //    else
            //        trn = trn.Parent;
            //    if (trnsel == trn)
            //        break;
            //}
            //if (strdept.Length > 0)
            //    strdept = strdept.Substring(3);
            //BindManager.UpdateTable(tabDetail, tabChanged, strdept);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {

        }

    }
}