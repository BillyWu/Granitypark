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
        /// 读取或设置通讯目标,为null不通讯
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }
        }

        private string deviceID;
        /// <summary>
        /// 读取或设置设备ID
        /// </summary>
        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }
        private string deviceName;
        /// <summary>
        /// 读取或设置设备名称
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
            this.UnitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "收费站");
            //初始化下拉框字典
            this.Query = new QueryDataRes(this.UnitItem.DataSrcFile);
            this.paramSystem = BindManager.getSystemParam();
            DataSet ds = new DataSet("卡片类型");
            this.Query.FillDataSet("卡片类型", this.paramSystem, ds);
            this.Query.FillDataSet("卡片类型", this.paramSystem, ds);
            DataRow dr = ds.Tables["卡片类型"].NewRow();
            ds.Tables["卡片类型"].Rows.InsertAt(dr, 0);
            dr = ds.Tables["卡片类型"].NewRow();
            ds.Tables["卡片类型"].Rows.InsertAt(dr, 0);
            DataGridViewComboBoxColumn col = this.gdRecord.Columns["卡类"] as DataGridViewComboBoxColumn;
            col.ValueType = typeof(int);
            col.DataSource = ds.Tables["卡片类型"];
            col.DisplayMember = "卡类";
            col.ValueMember = "编号";
            col = this.gdRecord.Columns["车型"] as DataGridViewComboBoxColumn;
            col.ValueType = typeof(int);
            col.DataSource = ds.Tables["车型"];
            col.DisplayMember = "车类";
            col.ValueMember = "编号";
            lbDevInfo.Text = this.DeviceName + "(" + this.DeviceID + ")";
        }
        /// <summary>
        /// 采集记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPick_Click(object sender, EventArgs e)
        {
            if (null == this.target || string.IsNullOrEmpty(this.deviceID) || string.IsNullOrEmpty(this.deviceName))
                return;
            string tpl = "停车场", cmd = "收集下一条记录";
            if (this.rdbRetry.Checked)
                cmd = "收集当前记录";

            CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
            string tagdata = "@设备地址=" + this.deviceID;
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
                    msg = "通讯失败,请检查设备连接或通讯参数后重试！";
                string tag = cmdP.ResponseFormat;
                if (string.IsNullOrEmpty(msg) && string.IsNullOrEmpty(tag))
                    msg = "采集完成！";
                if (!string.IsNullOrEmpty(msg))
                    continue;

                //采集加入数据
                object[] data = new object[cols.Count];
                for (int i = 0; i < cols.Count; i++)
                {
                    if ("车牌号码" == cols[i].Name)
                        continue;
                    string val = basefun.valtag(tag, "{" + cols[i].Name + "}");
                    if (string.IsNullOrEmpty(val))
                        val = basefun.valtag(tag, cols[i].Name);
                    if ("卡类" == cols[i].Name || "车型" == cols[i].Name)
                        data[i] = Convert.ToInt32(val);
                    else
                        data[i] = val;
                }
                this.gdRecord.Rows.Add(data);
            }
            CommiManager.GlobalManager.RemoveCommand(this.target, cmdP);
            //生成参数列表
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
                this.Query.ExecuteNonQuery("入场登记", psList, psList, psList);
            else
                this.Query.ExecuteNonQuery("出场登记", psList, psList, psList);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 设置表格行标号
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
        /// 表格字典列或格式列显示时出错忽略
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