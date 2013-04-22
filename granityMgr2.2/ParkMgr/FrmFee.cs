using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Estar.Common.Tools;
using Granity.winTools;
using System.Drawing.Printing;
using Estar.Business.DataManager;
using System.Collections;
using DevExpress.XtraEditors;
namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmFee : DevExpress.XtraEditors.XtraForm
    {

        private string tag;
        /// <summary>
        /// ��ȡ�������շ�����
        /// </summary>
        public string DataTag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }
        private QueryDataRes query = null;
        /// <summary>
        /// ��ѯʵ��
        /// </summary>
        public QueryDataRes Query
        {
            get { return query; }
            set { query = value; }
        }

        Regex regUperCase = new Regex("[A-Z]", RegexOptions.Compiled);

        public FrmFee()
        {
            InitializeComponent();
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

        /// <summary>
        /// ���س�ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmFee_Load(object sender, EventArgs e)
        {

            getCbbProvince();
            string code = this.query.ExecuteScalar("��ӡ������к�", new NameObjectList()).ToString();
            this.tag = basefun.setvaltag(this.tag, "{��ӡ���}", code);
            this.setContainerTagValue(this, this.tag, "pm");
            string realCar = basefun.valtag(tag, "{���ƺ���}");
            string avtCar = basefun.valtag(tag, "{ʶ���ƺ���}");

        }

        /// <summary>
        /// �õ�������������
        /// </summary>
        private void getCbbProvince()
        {
            DataTable tab = new DataTable();
            tab.Columns.Add("����");
            string[] aa = new string[] { "J��","J��","Y��","Yԥ","L��","H��","H��","X��","W��","L³","X��","S��","Y��","J��","M��",
                                         "G��","E��","C��","Y��","Qǭ","Y��","S��","G��","Q��","Z��","G��","N��","Z��"};
            foreach (string s in aa)
            {
                DataRow dr = tab.NewRow();
                dr["����"] = s;
                tab.Rows.Add(dr);
            }
        }
        /// <summary>
        /// ȷ���շѿ�բ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCharge_Click(object sender, EventArgs e)
        {

            string cardType = basefun.valtag(tag, "{����}");
            if (cardType == "9" && Convert.ToDecimal(this.txtLeaveMoney.Text) < Convert.ToDecimal(this.txtConsumeMoney.Text))
            {
                XtraMessageBox.Show("��ֵ�����㣬�����ѣ�", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.tag = basefun.setvaltag(this.tag, "{�շ�}", this.txtConsumeMoney.Text.Trim());
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btFree_Click(object sender, EventArgs e)
        {
            this.tag = basefun.setvaltag(this.tag, "{�շѽ��}", "0");
            this.tag = basefun.setvaltag(tag, "{�շ�}", "0");
            this.Close();
        }

        /// <summary>
        /// �ر�ʱ���³��ƺ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmFee_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ParkMgr.Report.PrintInfo print = new Granity.granityMgr.ParkMgr.Report.PrintInfo(this.tag);
            //print.ShowPreview();
            //print.Print();
        }

        /// <summary>
        /// ��Ǯ�䴦�����
        /// </summary>
        private void OpenCashbox()
        {
            //����һ��ProcessStartInfoʵ��
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            //�����������̵ĳ�ʼĿ¼
            info.WorkingDirectory = Application.StartupPath;
            //�����������̵�Ӧ�ó�����ĵ���
            info.FileName = @"open.exe";
            //�����������̵Ĳ���
            info.Arguments = "";
            //�����ɰ�������������Ϣ�Ľ�����Դ
            try
            {
                System.Diagnostics.Process.Start(info);
            }
            catch (System.ComponentModel.Win32Exception we)
            {
                XtraMessageBox.Show(this, we.Message);
            }
        }

        private void BtClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}