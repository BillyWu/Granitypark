using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Estar.Business.DataManager;
using Granity.winTools;
using Granity.communications;
using Estar.Common.Tools;
using System.Globalization;
using Granity.CardOneCommi;


namespace Granity.parkStation.cardManager
{
    public partial class FrmReadRecord : Form
    {
        UnitItem UnitItem;
        QueryDataRes Query;
        NameObjectList paramSystem;

        private CommiTarget target;
        /// <summary>
        /// ��ȡ������ͨѶĿ��,Ϊnull��ͨѶ
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }
        }

        private string deviceID;
        /// <summary>
        /// ��ȡ�������豸ID
        /// </summary>
        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }
        private string deviceName;
        /// <summary>
        /// ��ȡ�������豸����
        /// </summary>
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }

        public FrmReadRecord()
        {
            InitializeComponent();
        }
        private void FrmReadRecord_Load(object sender, EventArgs e)
        {
            this.UnitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "�շ�վ");
            //��ʼ���������ֵ�
            this.Query = new QueryDataRes(this.UnitItem.DataSrcFile);
            this.paramSystem = BindManager.getSystemParam();
            DataSet ds = new DataSet("��Ƭ����");
            this.Query.FillDataSet("��Ƭ����", this.paramSystem, ds);
            this.Query.FillDataSet("��Ƭ����", this.paramSystem, ds);
            DataRow dr = ds.Tables["��Ƭ����"].NewRow();
            ds.Tables["��Ƭ����"].Rows.InsertAt(dr, 0);
            dr = ds.Tables["��Ƭ����"].NewRow();
            ds.Tables["��Ƭ����"].Rows.InsertAt(dr, 0);
            DataGridViewComboBoxColumn col = this.gdRecord.Columns["����"] as DataGridViewComboBoxColumn;
            col.ValueType = typeof(int);
            col.DataSource = ds.Tables["��Ƭ����"];
            col.DisplayMember = "����";
            col.ValueMember = "���";
            col = this.gdRecord.Columns["����"] as DataGridViewComboBoxColumn;
            col.ValueType = typeof(int);
            col.DataSource = ds.Tables["����"];
            col.DisplayMember = "����";
            col.ValueMember = "���";
            lbDevInfo.Text = this.DeviceName + "(" + this.DeviceID + ")";
        }
        /// <summary>
        /// �ɼ���¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPick_Click(object sender, EventArgs e)
        {
            if (null == this.target || string.IsNullOrEmpty(this.deviceID) || string.IsNullOrEmpty(this.deviceName))
                return;
            string tpl = "ͣ����", cmd = "�ռ���һ����¼";
            if (this.rdbRetry.Checked)
                cmd = "�ռ���ǰ��¼";

            CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
            string tagdata = "@�豸��ַ=" + this.deviceID;
            cmdP.setCommand(tpl, cmd, tagdata);
            cmdP.TimeLimit = new TimeSpan(0, 10, 0);
            cmdP.TimeFailLimit = new TimeSpan(0, 10, 0);
            cmdP.TimeSendInv = new TimeSpan(0, 10, 0);
            target.setProtocol(Protocol.PTLPark);

            this.gdRecord.Rows.Clear();
            DataGridViewColumnCollection cols = this.gdRecord.Columns;
            string msg = "";
            while (string.IsNullOrEmpty(msg))
            {
                cmdP.ResetState();
                CommiManager.GlobalManager.SendCommand(this.target, cmdP);
                if (!cmdP.EventWh.WaitOne(2000, false))
                    msg = "ͨѶʧ��,�����豸���ӻ�ͨѶ���������ԣ�";
                string tag = cmdP.ResponseFormat;
                if (string.IsNullOrEmpty(msg) && string.IsNullOrEmpty(tag))
                    msg = "�ɼ���ɣ�";
                if (!string.IsNullOrEmpty(msg))
                    continue;

                //�ɼ���������
                object[] data = new object[cols.Count];
                for (int i = 0; i < cols.Count; i++)
                {
                    if ("���ƺ���" == cols[i].Name)
                        continue;
                    string val = basefun.valtag(tag, "{" + cols[i].Name + "}");
                    if (string.IsNullOrEmpty(val))
                        val = basefun.valtag(tag, cols[i].Name);
                    if ("����" == cols[i].Name || "����" == cols[i].Name)
                        data[i] = Convert.ToInt32(val);
                    else
                        data[i] = val;
                }
                this.gdRecord.Rows.Add(data);
            }
            CommiManager.GlobalManager.RemoveCommand(this.target, cmdP);
            //���ɲ����б�
            NameObjectList[] psList = new NameObjectList[this.gdRecord.Rows.Count];
            for (int i = 0; i < psList.Length; i++)
            {
                NameObjectList p = new NameObjectList();
                ParamManager.MergeParam(p, this.paramSystem, false);
                DataGridViewRow row = this.gdRecord.Rows[i];
                foreach (DataGridViewColumn c in cols)
                    p[c.Name] = row.Cells[c.Name].Value;
                psList[i] = p;
            }
            int devid = Convert.ToInt32(this.deviceID);
            if (devid < 129)
                this.Query.ExecuteNonQuery("�볡�Ǽ�", psList, psList, psList);
            else
                this.Query.ExecuteNonQuery("�����Ǽ�", psList, psList, psList);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// ���ñ���б��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            using (SolidBrush b = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(CultureInfo.CurrentCulture),
                        grid.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }
        /// <summary>
        /// ����ֵ��л��ʽ����ʾʱ�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (0 != Convert.ToInt16(e.Context & (DataGridViewDataErrorContexts.Display | DataGridViewDataErrorContexts.Formatting)))
                e.Cancel = true;
        }
    }
}