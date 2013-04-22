using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Granity.granityMgr.util;

namespace Granity.granityMgr
{
    public partial class FrmDBDatabase : DevExpress.XtraEditors.XtraForm
    {
        FormFactory dbhelp = new FormFactory();
        private bool success = false;

        public bool Success
        {
            get { return success; }
        }
        public FrmDBDatabase()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            FormFactory dbhelp = new FormFactory();

            bool fag = dbhelp.WriteConfig(txtserver.Text, txtdatebase.Text, txtuserid.Text, txtpassword.Text, "Granity.granityMgr", this.txtXT.Text, this.txtServerIP.Text);
            if (!fag)
            {
                MessageBox.Show("输入连接字符串错误，请重新输入！", "系统提示！");
                return;
            }
            if (!dbhelp.ConnectionResult("value", "Granity.granityMgr", "select * from 用户信息表"))
            {
                MessageBox.Show("输入连接字符串错误，请重新输入！", "系统提示！");
                this.txtserver.Text = "";
                this.txtdatebase.Text = "";
                this.txtuserid.Text = "";
                this.txtpassword.Text = "";
                return;
            }
            success = true;
            this.Close();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}