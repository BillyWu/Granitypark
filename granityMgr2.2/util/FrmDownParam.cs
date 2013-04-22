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
    /// �豸��������
    /// </summary>
    public partial class FrmDownParam : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "�Ž�����";
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
            //��ʼ�������͵�Ԫ
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.Text = this.unitName = Convert.ToString(this.paramwin["���ܵ�Ԫ"]);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //������
            string tagps = "@�Ž�����=�Ž�,@���ѻ�����=����,@��������=ͣ����";
            NameObjectList ps = ParamManager.createParam(tagps);
            this.paramwin["ͨѶЭ��"] = ps[unitName];
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
        /// �Ƿ�ɹ�ִ���豸ͬ��
        /// </summary>
        private bool isSuccessSynDevice = true;
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        private bool isDownLoaded = true;
        /// <summary>
        /// ͬ���豸���ز���
        /// </summary>
        /// <param name="devid">�豸ID</param>
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
        /// ͬ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataTable tabdown = this.ds.Tables["������Э��ָ��"];
            if (null == tabdown || tabdown.Rows.Count < 1)
                return;
            DataRow dr = this.dbDevList.GetDataRow(this.dbDevList.FocusedRowHandle);
            if (null == dr || DBNull.Value == dr["ID"])
                return;
            string devid = Convert.ToString(dr["ID"].ToString());
            string tag = basefun.setvaltag("", "�豸ID", devid);
            string cmds = "";
            int len = tabdown.Rows.Count;
            for (int i = 0; i < len; i++)
            {
                dr = tabdown.Rows[i];
                if (!true.Equals(dr["����"]))
                    continue;
                cmds += Convert.ToString(dr["����"]) + ",";
            }
            cmds = cmds.Replace(",", "|");
            if (string.IsNullOrEmpty(cmds))
                return;
            tag = basefun.setvaltag(tag, "ָ��", cmds);
            ThreadManager.QueueUserWorkItem(delegate(object obj) { this.commiDevice(obj); }, tag);
        }

        bool isRunning = false;
        /// <summary>
        /// ˢ�½�����
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

                    XtraMessageBox.Show("�豸����ͬ���ɹ�", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    XtraMessageBox.Show("�豸����ͬ��ʧ�ܣ������豸ͨѶ������ͨѶ", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// ��ǰ�иı�ʱˢ����ϸ
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
            ps["�豸ID"] = curDevID;
            this.ds.Tables["������Э��ָ��"].Clear();
            this.Query.FillDataSet("������Э��ָ��", ps, this.ds);
        }
        /// <summary>
        /// �˳�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnclose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("ȷ���رմ���!", "ϵͳ��ʾ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
    }
}