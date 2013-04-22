using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;

namespace Granity.granityMgr
{
    public partial class FrmSysUsers : Form
    {
        string unitName = "用户管理集团";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        /// <summary>
        /// 验证提示
        /// </summary>
        private ToolTip tip;

        public FrmSysUsers()
        {
            InitializeComponent();
        }

        private void FrmSysUsers_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);

            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            DataTable tab = dsUnit.Tables["新单位树全集"];
            XunFan(tab);
        }

        void XunFan(DataTable dt)
        {

            foreach (DataRow row in dt.Select("PID is null"))
            {
                TreeNode tn = new TreeNode();
                tn.Text = row["部门名称"].ToString();
                XunFanRow(dt, row["ID"].ToString(), tn);
                //foreach (DataRow dr in XunFanRow(dt,row["ID"].ToString()))
                //{
                //    TreeNode tnn=new TreeNode();
                //    tnn.Text = dr["部门名称"].ToString();
                //    tn.Nodes.Add(tnn);
                //}
                trvDept.Nodes.Add(tn);
            }
        }


        void XunFanRow(DataTable dt, string pid, TreeNode TN)
        {
            //foreach (DataRow dr in dt.Select())
            //{
            //    TreeNode tnn = new TreeNode();
            //    tnn.Text = dr["部门名称"].ToString();
            //    tn.Nodes.Add(tnn);
            //}
            //return dt.Select(string.Format("PID='{0}'", pid));

            foreach (DataRow dr in dt.Select(string.Format("PID='{0}'", pid)))
            {
                TreeNode tnn = new TreeNode();
                tnn.Text = dr["部门名称"].ToString();
                XunFanRow(dt, dr["ID"].ToString(), tnn);
                TN.Nodes.Add(tnn);
            }
        }

    }


}