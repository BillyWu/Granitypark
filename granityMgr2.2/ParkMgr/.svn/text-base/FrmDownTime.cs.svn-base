using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.communications;
using Granity.CardOneCommi;

namespace Granity.parkStation
{
    public partial class FrmDownTime : Form
    {
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;

        private CommiTarget target;
        /// <summary>
        /// 读取或设置通讯目标位置
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }
        }
        private string deviceID;
        /// <summary>
        /// 读取或设置通讯设备地址
        /// </summary>
        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }
        private string deviceName;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }
        
        public FrmDownTime()
        {
            InitializeComponent();
        }

        private void FrmDownTime_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "时段设置");
            this.Text = this.unitItem.UnitName;
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            if (this.dsUnit.Tables.Contains("时段设置"))
            {
                DataTable tab = this.dsUnit.Tables["时段设置"];
                if (tab.Columns.Contains("选择"))
                    tab.Columns.Remove("选择");
                tab.Columns.Add("选择", typeof(bool));
            }
            bg.BindFld(this, this.dsUnit);
            //显示指定字段
            //string cols = "选择 40,timeid 编号 40,timename 时段名称 90,timesec1s 起始时间1 90,timesec1e 截止时间1 90,timesec2s 起始时间2 90,timesec2e 截止时间2 90,";
            //cols += "date1 起始日期 80,date2 截止日期 80,satrest 星期六 50,sunrest 星期天 50";
            //bg.SetGridCols(this.gdValList, cols);
            foreach (DataGridViewColumn c in this.gdValList.Columns)
                c.ReadOnly = !("选择" == c.Name);

            this.lbDeviceInfo.Text = "  " + this.deviceName + "(" + this.deviceID + ")";
        }

        /// <summary>
        /// 获取选择的行号数组
        /// </summary>
        /// <returns>返回选择行号索引序号</returns>
        private int[] getCheckedList()
        {
            List<int> list = new List<int>();
            string fld = "选择";
            if (!this.gdValList.Columns.Contains(fld))
                return list.ToArray();
            foreach (DataGridViewRow dr in this.gdValList.Rows)
            {
                if (DBNull.Value == dr.Cells[fld].Value)
                    continue;
                bool check = Convert.ToBoolean(dr.Cells[fld].Value);
                if (check) list.Add(dr.Index);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRefsh_Click(object sender, EventArgs e)
        {
           
        }
        /// <summary>
        /// 初始化时段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInit_Click(object sender, EventArgs e)
        {
                  }
        /// <summary>
        /// 通讯写入设备设置时段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDown_Click(object sender, EventArgs e)
        {
           

        }
        /// <summary>
        /// 通讯响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdP_ResponseHandle(object sender, ResponseEventArgs e)
        {
            MessageBox.Show(e.Success.ToString());
            CmdProtocol cmd = sender as CmdProtocol;
            if (null == cmd || string.IsNullOrEmpty(cmd.CmdId))
                return;
            MessageBox.Show(cmd.CmdId + "  " + basefun.valtag(cmd.ResponseFormat, "{状态}"));

        }
        /// <summary>
        /// 查询时段代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuery_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            if (indexlist.Length < 1)
            {
                MessageBox.Show("请选择一个时段", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string tpl = "停车场", cmd = "查询有效时段";
            string[,] colmap ={ { "{时段代码}", "timeid" } };
            DataTable tab = this.dsUnit.Tables["park_timese"];
            foreach (int i in indexlist)
            {
                DataRow dr = tab.Rows[i];
                string tagdata = "@设备地址=" + this.deviceID;
                for (int c = 0; c < colmap.GetLength(0); c++)
                {
                    object val = dr[colmap[c, 1]];
                    if (true.Equals(val)) val = "1";
                    if (false.Equals(val)) val = "0";
                    tagdata = basefun.setvaltag(tagdata, colmap[c, 0], Convert.ToString(val));
                }
                CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
                cmdP.setCommand(tpl, cmd, tagdata);
                CommiManager.GlobalManager.SendCommand(target, cmdP);
                if (!cmdP.EventWh.WaitOne(2000,false))
                {
                    MessageBox.Show("通讯超时失败", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                    MessageBox.Show(cmdP.ResponseFormat);
            }
            MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnInit_Click_1(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("初始化时段清除设备时段设置,确认是否执行？", "设备通讯", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;
            
            string tpl = "停车场", cmd = "初始化时段";
            CmdProtocol cmdP = new CmdProtocol(false);
            string tagdata = "@设备地址=" + this.deviceID;
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯超时失败", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void BtnRefsh_Click_1(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.dsUnit.Tables["时段设置"].Clear();
            query.FillDataSet("时段设置", this.paramwin, this.dsUnit);
        }

        private void BtnDown_Click_1(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            if (indexlist.Length < 1)
            {
                MessageBox.Show("请选择一个时段", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string tpl = "停车场", cmd = "下载有效时段";
            string[,] colmap ={ { "{时段编号}", "时段编号" }, { "{开始时间1}", "开始时间1" }, { "{截止时间1}", "截止时间1" }, { "{开始时间2}", "开始时间2" }, { "{截止时间2}", "截止时间2" },
                                { "{开始日期}", "开始日期" }, { "{截止日期}", "截止日期" }, { "{星期六}", "星期六" }, { "{星期日}", "星期日" } };
            DataTable tab = this.dsUnit.Tables["时段设置"];
            CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
            foreach (int i in indexlist)
            {
                DataRow dr = tab.Rows[i];
                string tagdata = "@设备地址=" + this.deviceID;
                for (int c = 0; c < colmap.GetLength(0); c++)
                {
                    object val = dr[colmap[c, 1]];
                    if (true.Equals(val)) val = "1";
                    if (false.Equals(val)) val = "0";
                    tagdata = basefun.setvaltag(tagdata, colmap[c, 0], Convert.ToString(val));
                }
                cmdP.setCommand(tpl, cmd, tagdata);
                CommiManager.GlobalManager.SendCommand(target, cmdP);
                if (!cmdP.EventWh.WaitOne(2000, false))
                {
                    MessageBox.Show("通讯超时失败", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            MessageBox.Show("下载时段成功！", "设备通讯", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}