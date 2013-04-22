using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.UserRight;
using Granity.winTools;

namespace Granity.granityMgr.UserManager
{
    public partial class FrmUpdatePass : DevExpress.XtraEditors.XtraForm
    {
        public FrmUpdatePass()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            User u = BindManager.getUser();
            if (this.txtOldPass.Text == "")
            {
                XtraMessageBox.Show("请输入当前密码！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (this.txtNewPass.Text == "")
            {
                XtraMessageBox.Show("请输入新密码！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (this.txtRePass.Text == "")
            {
                XtraMessageBox.Show("请输入确认密码！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (this.txtNewPass.Text == this.txtRePass.Text)
            {
                if (!u.ModifyPassword(this.txtOldPass.Text, this.txtNewPass.Text))
                {
                    this.txtOldPass.Text = "";
                    this.txtNewPass.Text = "";
                    this.txtRePass.Text = "";
                    XtraMessageBox.Show("原密码不正确,请重新输入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                XtraMessageBox.Show("成功修改密码！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                XtraMessageBox.Show("密码输入不一致,请重新输入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txtNewPass.Text = "";
                this.txtRePass.Text = "";
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
    }
}