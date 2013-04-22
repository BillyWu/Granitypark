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

namespace Granity.parkStation
{
    public partial class FrmRemotGate : Form
    {
        string unitName = "";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmRemotGate()
        {
            InitializeComponent();
        }

        private void FrmRemotGate_Load(object sender, EventArgs e)
        {
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);

            unitName = pstrans["name"].ToString();//单元
            this.Text = RemotGroup.Text = unitName;
            //获取当前单元名称
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("远程控制记录", "@db=remote_control");

            dict.Add("入、出场记录历史查询", "@db=park_stall");
            //dict.Add("非法开闸记录", "@db=InvalidOpenGate");
            //数据源
            string datasource = dict[this.unitName];
            this.dbGrid.Tag = datasource;

            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            ds = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, ds);
            //特殊业务处理
            int count = this.dbGrid.Rows.Count - 1;
            if (count < 0)
            {
                count = 0;
            }
            //this.lblCount.Text = "总计 " + Convert.ToString(count) + " 条";
        }
    }
}