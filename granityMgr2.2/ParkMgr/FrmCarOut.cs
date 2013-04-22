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
    public partial class FrmCarOut : DevExpress.XtraEditors.XtraForm
    {
        private string TagDate = string.Empty;

        public string tagDate
        {
            get { return TagDate; }
            set { TagDate = value; }
        }
        public FrmCarOut()
        {
            InitializeComponent();
        }

        private void FrmCarOut_Load(object sender, EventArgs e)
        {
            this.txtCarOut.Focus();
            this.setContainerTagValue(this, this.TagDate, "pm");
            this.txtCarOut.Text = this.txtCarInNo.Text.Trim();
        }

        private void btAffirm_Click(object sender, EventArgs e)
        {
            string cardTypeName = basefun.valtag(tagDate, "{��������}");
            if (this.txtCarOut.Text == string.Empty)
            {
                XtraMessageBox.Show("�������������", "ϵͳ��ʾ��");
                return;
            }
            if (cardTypeName != "��ʱ��")
            {
                if (this.txtCarInNo.Text != this.txtCarOut.Text)
                {
                    XtraMessageBox.Show("����ĳ��Ƹ��������Ʋ�һ��", "ϵͳ��ʾ��");
                    return;
                }
            }
            TagDate = basefun.setvaltag(tagDate, " �����������", this.txtCarOut.Text.Trim());
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        /// <summary>
        /// ����tag���ֵ���¿ؼ��ڿؼ�ֵ,������������Ӷ�Ӧ�����ơ�ֵ
        /// </summary>
        /// <param name="ct">�ؼ�����</param>
        /// <param name="tag">tag��ʽ����</param>
        /// <param name="keyName">tag���ӳ��������</param>
        /// <returns>����tag��ʽ����</returns>
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
                        tag = basefun.setvaltag(tag, pm + "����", cbb.Text);
                    else
                        tag = basefun.setvaltag(tag, "{" + pm + "����" + "}", cbb.Text);
                }
            }
            foreach (Control child in ct.Controls)
                tag = this.setContainerTagValue(child, tag, keyName);

            return tag;
        }
    }
}