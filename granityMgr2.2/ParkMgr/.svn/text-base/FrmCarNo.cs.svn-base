using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmCarNo : DevExpress.XtraEditors.XtraForm
    {
        private string TagDate = string.Empty;

        public string tagDate
        {
            get { return TagDate; }
            set { TagDate = value; }
        }

        public FrmCarNo()
        {
            InitializeComponent();
        }

        private void FrmCarNo_Load(object sender, EventArgs e)
        {
            this.txtCadNo.Focus();
            this.setContainerTagValue(this, this.TagDate, "pm");
            this.txtCadNo.Text = this.txtReadCarNo.Text.Trim();
        }

        /// <summary>
        /// 确定，开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAffirm_Click(object sender, EventArgs e)
        {
            string cardTypeName = basefun.valtag(tagDate, "{卡类名称}");
            if (this.txtCadNo.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入车牌号码", "系统提示！");
                return;
            }
            if (cardTypeName != "临时卡")
            {
                if (this.txtCadNo.Text.Trim() != this.txtReadCarNo.Text.Trim())
                {
                    XtraMessageBox.Show("输入车牌号码跟发卡时的车牌不一致！", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
            }
            TagDate = basefun.setvaltag(tagDate, "输入进场车牌", this.txtCadNo.Text.Trim());
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        /// <summary>
        /// 取消，不开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        /// <summary>
        /// 更加tag标记值更新控件内控件值,有下拉框则添加对应“名称”值
        /// </summary>
        /// <param name="ct">控件容器</param>
        /// <param name="tag">tag格式数据</param>
        /// <param name="keyName">tag标记映射标记名称</param>
        /// <returns>返回tag格式数据</returns>
        private string setContainerTagValue(Control ct, string tag, string keyName)
        {
            if (null == ct || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(keyName))
                return tag;
            string t = Convert.ToString(ct.Tag);
            string pm = basefun.valtag(t, keyName);
            if (!string.IsNullOrEmpty(pm))
            {
                string val = basefun.valtag(tag, "{" + pm + "}");
                string v = val;
                if (string.IsNullOrEmpty(val))
                    v = basefun.valtag(tag, pm);
                if (!(ct is DevExpress.XtraEditors.LookUpEdit))
                    ct.Text = v;
                else
                {
                    DevExpress.XtraEditors.LookUpEdit cbb = ct as DevExpress.XtraEditors.LookUpEdit;
                    try { cbb.EditValue = int.Parse(val); }
                    catch { }
                    if (string.IsNullOrEmpty(val))
                        tag = basefun.setvaltag(tag, pm + "名称", cbb.Text);
                    else
                        tag = basefun.setvaltag(tag, "{" + pm + "名称" + "}", cbb.Text);
                }
            }
            foreach (Control child in ct.Controls)
                tag = this.setContainerTagValue(child, tag, keyName);

            return tag;
        }
    }
}