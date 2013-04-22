using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Common.Tools;
using Granity.winTools;
using Estar.Business.UserRight;
using Estar.Business.DataManager;
using System.Net;
using System.Runtime.InteropServices;
using Granity.granityMgr.util;
namespace Granity.granityMgr
{
    /// <summary>
    /// ϵͳ��¼
    /// </summary>
    public partial class FrmLogin : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("Kernel32.dll", EntryPoint = "SetLocalTime")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);
        string unitName = "�û���¼";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        /// <summary>
        /// ִ�����ݴ����Query,�ɵ�Ԫ��ʼ��
        /// </summary>
        QueryDataRes Query = null;
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

            FormFactory dbhelp = new FormFactory();

            if (!dbhelp.ConnectionResult("value", "Granity.granityMgr", "select * from �û���Ϣ��"))
            {


                FrmDBDatabase db = new FrmDBDatabase();
                db.ShowDialog();
                if (!db.Success)
                    return;
            }
            try
            {

                //��ȡҵ��Ԫ�ʹ��ݲ���
                this.paramwin = BindManager.getSystemParam();
                NameObjectList pstrans = BindManager.getTransParam();
                ParamManager.MergeParam(this.paramwin, pstrans);
                unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
                //������
                BindManager bg = new BindManager(this);
                this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
                this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
                bg.BindFld(this, this.dsUnit);
                setSystemTime();
            }
            catch
            {
                this.Close();

            }
        }

        /// <summary>
        /// ͬ��������ʱ��
        /// </summary>
        private void setSystemTime()
        {
            //DataTable tab = this.dsUnit.Tables["������ʱ��"];
            //DateTime systemTime = Convert.ToDateTime(tab.Rows[0]["����"]);
            //SystemTime sysTime = new SystemTime();
            //sysTime.wYear = Convert.ToUInt16(systemTime.Year);
            //sysTime.wMonth = Convert.ToUInt16(systemTime.Month);
            //sysTime.wDay = Convert.ToUInt16(systemTime.Day);
            //sysTime.wHour = Convert.ToUInt16(systemTime.Hour);
            //sysTime.wMinute = Convert.ToUInt16(systemTime.Minute);
            //sysTime.wSecond = Convert.ToUInt16(systemTime.Second);
            //sysTime.wMilliseconds = Convert.ToUInt16(systemTime.Millisecond);
            //bool fag = SetLocalTime(ref sysTime);
        }

        /// <summary>
        /// ��ѯ�Ƿ��ǲ���Ա������ԱҪ����
        /// </summary>
        /// <returns></returns>
        public DataTable User()
        {
            //�õ�����Դ
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.dsUnit.Tables["����Ա"].Clear();
            //ִ�в�ѯ����
            NameObjectList ps = new NameObjectList();
            ps["�ʺ�"] = cbbAccount.Text;
            query.FillDataSet("����Ա", ps, this.dsUnit);
            return dsUnit.Tables["����Ա"];
        }

        public DataTable getUser()
        {

            //�õ�����Դ
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.dsUnit.Tables["������Ա"].Clear();
            //ִ�в�ѯ����
            NameObjectList ps = new NameObjectList();
            ps["ֵ����"] = cbbAccount.Text;
            query.FillDataSet("������Ա", ps, this.dsUnit);
            return dsUnit.Tables["������Ա"];
        }

        /// <summary>
        /// �ж��Ƿ񽻰�
        /// </summary>
        /// <returns></returns>
        public bool IHandOver()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();

            bool isOver = true;
            DataTable tabStatus = this.dsUnit.Tables["����״̬"];
            if (tabStatus.Rows.Count < 0)
            {
                if (tabStatus.Rows[0]["ֵ����"].ToString() != cbbAccount.Text)
                {
                    return false;
                }
            }
            else
            {
                DataTable dt = getUser();
                if (dt.Rows.Count < 1) return true;
                if (dt.Rows[0]["����IP"].ToString() != myip)
                    return false;
            }
            //DataTable tab = User();
            //if (tab == null || tab.Rows.Count < 1) return false;
            //if (Convert.ToString(tab.Rows[0]["��ɫ"]) == "����Ա" || Convert.ToString(tab.Rows[0]["��ɫ"]) == "ϵͳ����Ա")
            //{
            //    IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            //    string myip = IpEntry.AddressList[0].ToString();
            //    string filter = "����IP='{0}'";
            //    filter = string.Format(filter, myip);
            //    //�õ�����Դ
            //    QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            //    this.dsUnit.Tables["���ƹ���"].Clear();
            //    //ִ�в�ѯ����
            //    query.FillDataSet("���ƹ���", this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
            //    DataRow drduty = null;
            //    if (null != dsUnit.Tables["���ƹ���"] && dsUnit.Tables["���ƹ���"].Rows.Count > 0)
            //        drduty = dsUnit.Tables["���ƹ���"].Rows[0];

            //    if (drduty != null && myip.Equals(drduty["����IP"]) && this.cbbAccount.Text != Convert.ToString(drduty["ֵ����"]))
            //    {
            //        return false;
            //    }
            //}
            return isOver;
        }
        /// <summary>
        /// �û���¼,�жϽ��Ӱ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, EventArgs e)
        {
            //������ڽ�ֹʹ��
           
            if (string.IsNullOrEmpty(this.cbbAccount.Text) || string.IsNullOrEmpty(this.tbPassword.Text))
            {
                MessageBox.Show("�û��������벻��Ϊ�գ�", "��¼��ʾ");
                return;
            }
            //�ж���ͣ�����񽻰�
            if (IHandOver() == false)
            {
                MessageBox.Show("�õ���δ���࣬�����Ƚ��Ӱ�!", "��ʾ", MessageBoxButtons.OK);
                return;
            }
            else
            {
                PopupBaseEdit pe;
                bool isNotExist = false;
                User user = new User(this.cbbAccount.Text, ref isNotExist);
                if (!isNotExist)
                    isNotExist = !user.login(tbPassword.Text);
                if (isNotExist)
                {
                    MessageBox.Show("�û��������벻��ȷ��", "��¼��ʾ");
                    return;
                }
                //���õ�ǰϵͳ�û�����������
                BindManager.setUser(user.UserAccounts);
                DataTable tab = this.dsUnit.Tables["����״̬"];
                if (tab != null && tab.Select("ֵ����='" + this.cbbAccount.Text.Trim() + "'").Length == 0)
                {
                    string operName = this.cbbAccount.Text.Trim();
                    string workStartTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string ip = this.paramwin.AllValues.GetValue(2).ToString();
                    string tag = string.Empty;
                    tag = basefun.setvaltag(tag, "ֵ����", operName);
                    tag = basefun.setvaltag(tag, "�Ӱ�ʱ��", workStartTime);
                    tag = basefun.setvaltag(tag, "����IP", ip);
                    NameObjectList ps = ParamManager.createParam(tag);
                    ParamManager.MergeParam(ps, this.paramwin, false);
                    this.Query.ExecuteNonQuery("����״̬", ps, ps, ps);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }
}