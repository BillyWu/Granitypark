using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net;
using System.IO.Ports;
using System.Windows.Forms;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using Granity.CardOneCommi;
using Granity.communications;
using DevExpress.XtraEditors;
using Estar.Business.UserRight;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FormUpDownWork : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "交接班管理";


        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;

        public FormUpDownWork()
        {
            InitializeComponent();
        }

        private void FormUpDownWork_Load(object sender, EventArgs e)
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
            DataTable tab = this.dsUnit.Tables["接班人员"];
            if (null == tab || tab.Rows.Count < 1)
                return;
            DataTable tabUser = this.dsUnit.Tables["用户信息"];
            foreach (DataRow dr in tabUser.Rows)
                this.ccbNextUser.Items.Add(Convert.ToString(dr["帐号"]));
            tbUserName.Text = BindManager.getUser().UserAccounts;
        }
        /// <summary>
        /// 确定交班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbOK_Click(object sender, EventArgs e)
        {
            bool isNotExist = false;
            string user = Convert.ToString(this.ccbNextUser.Text);
            string pws = this.tbPassword.Text;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pws))
            {
                MessageBox.Show("请选择接班人和输入接班人密码！", "接班提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.paramwin["交班人"] = BindManager.getUser().UserAccounts;
            this.paramwin["密码"] = pws;

            User user1 = new User(user, ref isNotExist);
            if (!isNotExist)
                isNotExist = !user1.login(tbUserName.Text, pws);
            if (isNotExist)
            {
                MessageBox.Show("密码不正确！", "系统提示");
                return;
            }
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.paramwin["操作员"] = BindManager.getUser().UserName;
            this.paramwin["值班人"] = ccbNextUser.Text;
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            this.paramwin["电脑IP"] = myip;
            bool isSuccess = query.ExecuteNonQuery("交接班", this.paramwin, this.paramwin, this.paramwin);
            if (!isSuccess)
            {
                MessageBox.Show("执行交接班时失败请与管理员联系！", "接班提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            BindManager.setUser(user);
            string operName = this.ccbNextUser.Text.Trim();
            string workStartTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ip = this.paramwin.AllValues.GetValue(2).ToString();
            string tag = string.Empty;
            tag = basefun.setvaltag(tag, "值班人", operName);
            tag = basefun.setvaltag(tag, "接班时间", workStartTime);
            tag = basefun.setvaltag(tag, "电脑IP", ip);
            NameObjectList ps = ParamManager.createParam(tag);
            ParamManager.MergeParam(ps, this.paramwin, false);
            query.ExecuteNonQuery("当班状态", ps, ps, ps);
            MessageBox.Show("已经成功执行交接班！\r\n" + this.ccbNextUser.Text + " 开始工作！", "接班提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}