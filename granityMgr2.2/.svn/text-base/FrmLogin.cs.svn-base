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
    /// 系统登录
    /// </summary>
    public partial class FrmLogin : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("Kernel32.dll", EntryPoint = "SetLocalTime")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);
        string unitName = "用户登录";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        /// <summary>
        /// 执行数据处理的Query,由单元初始化
        /// </summary>
        QueryDataRes Query = null;
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

            FormFactory dbhelp = new FormFactory();

            if (!dbhelp.ConnectionResult("value", "Granity.granityMgr", "select * from 用户信息表"))
            {


                FrmDBDatabase db = new FrmDBDatabase();
                db.ShowDialog();
                if (!db.Success)
                    return;
            }
            try
            {

                //读取业务单元和传递参数
                this.paramwin = BindManager.getSystemParam();
                NameObjectList pstrans = BindManager.getTransParam();
                ParamManager.MergeParam(this.paramwin, pstrans);
                unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
                //绑定数据
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
        /// 同步服务器时间
        /// </summary>
        private void setSystemTime()
        {
            //DataTable tab = this.dsUnit.Tables["服务器时间"];
            //DateTime systemTime = Convert.ToDateTime(tab.Rows[0]["日期"]);
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
        /// 查询是否是操作员，操作员要交班
        /// </summary>
        /// <returns></returns>
        public DataTable User()
        {
            //得到数据源
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.dsUnit.Tables["操作员"].Clear();
            //执行查询操作
            NameObjectList ps = new NameObjectList();
            ps["帐号"] = cbbAccount.Text;
            query.FillDataSet("操作员", ps, this.dsUnit);
            return dsUnit.Tables["操作员"];
        }

        public DataTable getUser()
        {

            //得到数据源
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.dsUnit.Tables["当班人员"].Clear();
            //执行查询操作
            NameObjectList ps = new NameObjectList();
            ps["值班人"] = cbbAccount.Text;
            query.FillDataSet("当班人员", ps, this.dsUnit);
            return dsUnit.Tables["当班人员"];
        }

        /// <summary>
        /// 判断是否交班
        /// </summary>
        /// <returns></returns>
        public bool IHandOver()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();

            bool isOver = true;
            DataTable tabStatus = this.dsUnit.Tables["当班状态"];
            if (tabStatus.Rows.Count < 0)
            {
                if (tabStatus.Rows[0]["值班人"].ToString() != cbbAccount.Text)
                {
                    return false;
                }
            }
            else
            {
                DataTable dt = getUser();
                if (dt.Rows.Count < 1) return true;
                if (dt.Rows[0]["电脑IP"].ToString() != myip)
                    return false;
            }
            //DataTable tab = User();
            //if (tab == null || tab.Rows.Count < 1) return false;
            //if (Convert.ToString(tab.Rows[0]["角色"]) == "操作员" || Convert.ToString(tab.Rows[0]["角色"]) == "系统管理员")
            //{
            //    IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            //    string myip = IpEntry.AddressList[0].ToString();
            //    string filter = "电脑IP='{0}'";
            //    filter = string.Format(filter, myip);
            //    //得到数据源
            //    QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            //    this.dsUnit.Tables["班制管理"].Clear();
            //    //执行查询操作
            //    query.FillDataSet("班制管理", this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
            //    DataRow drduty = null;
            //    if (null != dsUnit.Tables["班制管理"] && dsUnit.Tables["班制管理"].Rows.Count > 0)
            //        drduty = dsUnit.Tables["班制管理"].Rows[0];

            //    if (drduty != null && myip.Equals(drduty["电脑IP"]) && this.cbbAccount.Text != Convert.ToString(drduty["值班人"]))
            //    {
            //        return false;
            //    }
            //}
            return isOver;
        }
        /// <summary>
        /// 用户登录,判断交接班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, EventArgs e)
        {
            //软件过期禁止使用
           
            if (string.IsNullOrEmpty(this.cbbAccount.Text) || string.IsNullOrEmpty(this.tbPassword.Text))
            {
                MessageBox.Show("用户名或密码不能为空！", "登录提示");
                return;
            }
            //判断是停车场否交班
            if (IHandOver() == false)
            {
                MessageBox.Show("该电脑未交班，请首先交接班!", "提示", MessageBoxButtons.OK);
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
                    MessageBox.Show("用户名或密码不正确！", "登录提示");
                    return;
                }
                //设置当前系统用户并打开主窗口
                BindManager.setUser(user.UserAccounts);
                DataTable tab = this.dsUnit.Tables["当班状态"];
                if (tab != null && tab.Select("值班人='" + this.cbbAccount.Text.Trim() + "'").Length == 0)
                {
                    string operName = this.cbbAccount.Text.Trim();
                    string workStartTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string ip = this.paramwin.AllValues.GetValue(2).ToString();
                    string tag = string.Empty;
                    tag = basefun.setvaltag(tag, "值班人", operName);
                    tag = basefun.setvaltag(tag, "接班时间", workStartTime);
                    tag = basefun.setvaltag(tag, "电脑IP", ip);
                    NameObjectList ps = ParamManager.createParam(tag);
                    ParamManager.MergeParam(ps, this.paramwin, false);
                    this.Query.ExecuteNonQuery("当班状态", ps, ps, ps);
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