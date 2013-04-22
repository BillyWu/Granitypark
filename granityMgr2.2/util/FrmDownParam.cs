using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Granity.communications;
using Estar.Common.Tools;
using Granity.winTools;
using DevExpress.XtraGrid.Views.Grid;
using System.Threading;
using Granity.commiServer;

namespace Granity.granityMgr.util
{
    /// <summary>
    /// 设备更新下载
    /// </summary>
    public partial class FrmDownParam : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门禁管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        public FrmDownParam()
        {
            InitializeComponent();
        }
        private void FrmDoorManager_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.Text = this.unitName = Convert.ToString(this.paramwin["功能单元"]);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            string tagps = "@门禁管理=门禁,@消费机管理=消费,@参数下载=停车场";
            NameObjectList ps = ParamManager.createParam(tagps);
            this.paramwin["通讯协议"] = ps[unitName];
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
            this.frmPrb = new frmProcessBar();
            this.frmPrb.Hide();
        }
        private frmProcessBar frmPrb = null;
        /// <summary>
        /// 是否成功执行设备同步
        /// </summary>
        private bool isSuccessSynDevice = true;
        /// <summary>
        /// 是否下载完毕
        /// </summary>
        private bool isDownLoaded = true;
        /// <summary>
        /// 同步设备下载参数
        /// </summary>
        /// <param name="devid">设备ID</param>
        private void commiDevice(object arg)
        {
            string tag = Convert.ToString(arg);
            if (string.IsNullOrEmpty(tag))
                return;
            isDownLoaded = false;
            SynDeviceParam syn = new SynDeviceParam();
            isSuccessSynDevice = syn.CommiDevice(tag);
            while (!this.frmPrb.Visible)
                Thread.Sleep(100);
            isDownLoaded = true;
        }
        /// <summary>
        /// 同步下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataTable tabdown = this.ds.Tables["可下载协议指令"];
            if (null == tabdown || tabdown.Rows.Count < 1)
                return;
            DataRow dr = this.dbDevList.GetDataRow(this.dbDevList.FocusedRowHandle);
            if (null == dr || DBNull.Value == dr["ID"])
                return;
            string devid = Convert.ToString(dr["ID"].ToString());
            string tag = basefun.setvaltag("", "设备ID", devid);
            string cmds = "";
            int len = tabdown.Rows.Count;
            for (int i = 0; i < len; i++)
            {
                dr = tabdown.Rows[i];
                if (!true.Equals(dr["下载"]))
                    continue;
                cmds += Convert.ToString(dr["名称"]) + ",";
            }
            cmds = cmds.Replace(",", "|");
            if (string.IsNullOrEmpty(cmds))
                return;
            tag = basefun.setvaltag(tag, "指令", cmds);
            ThreadManager.QueueUserWorkItem(delegate(object obj) { this.commiDevice(obj); }, tag);
        }

        bool isRunning = false;
        /// <summary>
        /// 刷新进程条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmPrb_Tick(object sender, EventArgs e)
        {
            if (isDownLoaded && !this.frmPrb.Visible || isRunning)
                return;
            isRunning = true;
            if (isDownLoaded)
            {
                this.frmPrb.Hide();
                this.frmPrb.Position = 0;
                if (isSuccessSynDevice)

                    XtraMessageBox.Show("设备参数同步成功", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    XtraMessageBox.Show("设备参数同步失败，请检查设备通讯及网络通讯", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isRunning = false;
                return;
            }
            else if (!this.frmPrb.Visible)
                this.frmPrb.Show();
            if (this.frmPrb.Position > 99)
                this.frmPrb.Position = 0;
            else if (this.frmPrb.Position < 5)
                this.frmPrb.Position += 2;
            else
                this.frmPrb.Position += 5;
            isRunning = false;
        }
        private string curDevID = "";
        /// <summary>
        /// 当前行改变时刷新明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbDevList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView grid = sender as GridView;
            if (null == grid || e.FocusedRowHandle < 0)
                return;
            DataRow dr = grid.GetDataRow(e.FocusedRowHandle);
            if (null == dr || this.curDevID.Equals(Convert.ToString(dr["ID"])))
                return;
            this.curDevID = Convert.ToString(dr["ID"]);
            NameObjectList ps = new NameObjectList();
            ps["设备ID"] = curDevID;
            this.ds.Tables["可下载协议指令"].Clear();
            this.Query.FillDataSet("可下载协议指令", ps, this.ds);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnclose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
    }
}