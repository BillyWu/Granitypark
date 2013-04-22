using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Estar.Business.UserRight;
using Granity.winTools;

namespace Granity.granityMgr
{
    public partial class FrmUpdatePassword : Form
    {
        public FrmUpdatePassword()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            User u = BindManager.getUser();
            if (txtOrderPassword.Text == "")
            {
                MessageBox.Show("请输入当前密码！");
            }
            else if (txtNewPassword.Text == "")
            {
                MessageBox.Show("请输入新密码！");
            }
            else if (txtTruePassword.Text == "")
            {
                MessageBox.Show("请输入确认密码！");
            }
            else if (txtNewPassword.Text == txtTruePassword.Text)
            {
                if (!u.ModifyPassword(txtOrderPassword.Text, txtNewPassword.Text))
                {
                    this.txtOrderPassword.Text = "";
                    txtNewPassword.Text = "";
                    txtTruePassword.Text = "";
                    MessageBox.Show("原密码不正确,请重新输入！", "修改密码", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("成功修改密码！", "修改密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("密码输入不一致,请重新输入！");
                txtNewPassword.Text = "";
                txtTruePassword.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}