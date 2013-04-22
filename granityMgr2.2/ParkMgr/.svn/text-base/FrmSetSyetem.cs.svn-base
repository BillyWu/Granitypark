using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.parkStation.cardManager;
using System.Runtime.InteropServices;
using System.Threading;
using Granity.winTools;

using Estar.Business.DataManager;
using Estar.Common.Tools;
using System.Drawing.Printing;
using System.Data.SqlClient;
using Granity.communications;
using Granity.parkStation;
using Granity.CardOneCommi;


namespace Granity.parkStation
{
    public partial class FrmSetSyetem : Form
    {
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;

        public FrmSetSyetem()
        {
            InitializeComponent();
        }

        private void FrmSetSyetem_Load(object sender, EventArgs e)
        {
            DevListGrid.Hide();
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), Convert.ToString(pstrans["name"]));
            this.Text = this.unitItem.UnitName;

            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            if (this.dsUnit.Tables.Contains("设备列表"))
            {
                DataTable tab = this.dsUnit.Tables["设备列表"];
                if (tab.Columns.Contains("选择"))
                    tab.Columns.Remove("选择");
                tab.Columns.Add("选择", typeof(bool));
            }
            bg.BindFld(this, this.dsUnit);
            //显示指定字段
            // bg.SetGridCols(this.dbGrid, "选择 60,名称 设备名称,地址,端口,设备地址,访问方式,备注");
            foreach (DataGridViewColumn col in this.dbGrid.Columns)
            {
                if ("选择" == col.Name)
                    continue;
                col.ReadOnly = true;
            }
        }

        /// <summary>
        /// 获取选择的行号数组
        /// </summary>
        /// <returns>返回选择行号索引序号</returns>
        private int[] getCheckedList()
        {
            List<int> list = new List<int>();
            string fld = "选择";
            if (!this.dbGrid.Columns.Contains(fld))
                return list.ToArray();
            DataTable tab = this.dbGrid.DataSource as DataTable;
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow dr = tab.Rows[i];
                if (DBNull.Value == dr[fld])
                    continue;
                bool check = Convert.ToBoolean(dr[fld]);
                if (check) list.Add(i);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据行的设备目标位置参数
        /// 记录包含字段【访问方式】(TCP/UDP/SerialPort)、【端口】(60000/COM1)、【地址】(192.168.1.146)
        /// </summary>
        /// <param name="dr">数据记录</param>
        /// <returns></returns>
        private CommiTarget getTarget(DataRow dr)
        {
            if (null == dr) return null;
            CommiTarget target = new CommiTarget();
            CommiType commiType = CommiType.UDP;
            int typecommi = 0;
            if (dr["通讯类别"].ToString() == "485")
            {
                typecommi = 1;
            }
            else if (dr["通讯类别"].ToString() == @"TCP\IP")
            {
                typecommi = 2;
            }
            else
            {
                typecommi = 3;
            }
            if (DBNull.Value != dr["通讯类别"])
            {
                //int typecommi = Convert.ToInt16(dr["通讯类别"]);
                if (1 == typecommi) commiType = CommiType.SerialPort;
                if (2 == typecommi) commiType = CommiType.TCP;
            }
            if (CommiType.SerialPort == commiType)
            {
                string portname = "COM" + Convert.ToString(dr["端口"]);
                target.SetProtocolParam(portname, 19200);
            }
            else
            {
                string addr = Convert.ToString(dr["站址"]);
                int port = Convert.ToInt16(dr["端口"]);
                target.SetProtocolParam(addr, port, commiType);
            }
            target.setProtocol(Protocol.PTLPark);
            return target;
        }

        private void SerchBtn_Click(object sender, EventArgs e)
        {
            DevListGrid.Show();
            dbGrid.Hide();
            DevListGrid.Columns.Add("1", "名称");
            DevListGrid.Columns.Add("2", "IP地址");
            DevListGrid.Columns.Add("3", "型号");
            DevListGrid.Columns.Add("4", "Mac地址");
            DevListGrid.Columns.Add("5", "端口号");
            DevListGrid.Columns.Add("6", "波特率");
            DevListGrid.Columns.Add("7", "数据位");
            DevListGrid.Columns.Add("8", "停止位");
            DevListGrid.Columns.Add("9", "校验位");
            DevListGrid.Columns.Add("10", "修改");
            int count = DevListGrid.Rows.Count - 1;
            for (int i = 0; i < count; i++)
            {
                DevListGrid.Rows.RemoveAt(0);
            }
            //DevListGrid.Columns.Clear();
            UDPclient.sendrecv.ZN_SearchAll();
            int time = 0;
            byte[] szip = new byte[20];
            byte[] szver = new byte[20];
            byte[] szmac = new byte[30];
            byte devtype = 0;
            byte ipmode = 0;
            int tcpport = 0;
            int res = 0;
            for (; time < 20; time++)
            {
                while (UDPclient.sendrecv.ZN_GetSearchDev(ref szip[0], ref szver[0], ref szmac[0], ref devtype, ref ipmode, ref tcpport) == 1)
                {
                    DevListGrid.Rows.Add();
                    string IP = System.Text.Encoding.ASCII.GetString(szip);
                    DevListGrid.Rows[time].Cells[1].Value = IP;
                    DevListGrid.Rows[time].Cells[2].Value = devtype.ToString();
                    //item.SubItems.Add(devtype.ToString());
                    string mac = System.Text.Encoding.ASCII.GetString(szmac);
                    DevListGrid.Rows[time].Cells[3].Value = mac;
                    //res = Getinfo_UDP(IP, mac, devtype,time);
                    if (res != 1)
                    {
                        MessageBox.Show("获取信息失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //DevListGrid.Rows.Add();
                    //res=UDPclient.sendrecv
                }
                //if (DevListGrid.Rows[0].Cells[1].Value == "")
                //{
                //    DevListGrid.Rows.RemoveAt(0);
                //}
                System.Threading.Thread.Sleep(10);
            }
        }

        private void OnCloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 查询设备清单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevListBtn_Click(object sender, EventArgs e)
        {
            dbGrid.Show();
            DevListGrid.Hide();
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.dsUnit.Tables["compara"].Clear();
            query.FillDataSet("compara", this.paramwin, this.dsUnit);
        }

        /// <summary>
        /// 初始化硬件设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            if (indexlist.Length < 1)
            {
                MessageBox.Show("请选择一个设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (DialogResult.Yes != MessageBox.Show("初始化设备会清空设备存储的数据，是否确认需要初始化！", "设备初始化", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;
            string strdevIDs = "", msg = "", strdevNames = "";
            string tpl = "停车场", cmd = "格式化";
            DataTable tab = this.dbGrid.DataSource as DataTable;
            foreach (int i in indexlist)
            {
                DataRow dr = tab.Rows[i];
                CommiTarget target = this.getTarget(dr);
                CmdProtocol cmdP = new CmdProtocol(Convert.ToString(dr["名称"]), false);
                string devid = Convert.ToString(dr["站址"]);
                string tagdata = "@设备地址=" + devid;
                cmdP.setCommand(tpl, cmd, tagdata);
                CommiManager.GlobalManager.SendCommand(target, cmdP);

                if (!cmdP.EventWh.WaitOne(2000, false))
                    msg = cmdP.CmdId + "：通讯失败,请检查设备连接或通讯参数后重试！";
                string rpmsg = basefun.valtag(cmdP.ResponseFormat, "{状态}");
                if (string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(rpmsg) && "操作成功！" != rpmsg)
                    msg = cmdP.CmdId + "：" + rpmsg;
                if (!string.IsNullOrEmpty(msg))
                    break;
                strdevIDs += "," + devid;
                strdevNames += cmdP.CmdId + ",";
            }
            //形成参数
            string[] devids = strdevIDs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            NameObjectList[] psList = new NameObjectList[devids.Length];
            for (int i = 0; i < devids.Length; i++)
            {
                psList[i] = new NameObjectList();
                psList[i]["devID"] = devids[i];
            }
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            if (psList.Length > 0)
                query.ExecuteNonQuery("删除设备白名单", psList, psList, psList);

            if (string.IsNullOrEmpty(msg))
                MessageBox.Show("设备初始化成功！", "设备初始化", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                msg = string.IsNullOrEmpty(strdevNames) ? msg : strdevNames + "成功执行初始化，\r\n" + msg;
                MessageBox.Show(msg, "设备初始化", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 命令执行响应结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmd_ResponseHandle(object sender, ResponseEventArgs e)
        {
            CmdProtocol cmd = sender as CmdProtocol;
            if (null == cmd || string.IsNullOrEmpty(cmd.CmdId))
                return;

            MessageBox.Show(cmd.CmdId + "  " + basefun.valtag(cmd.ResponseFormat, "{状态}"));
        }

        /// <summary>
        /// 巡检硬件设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            string msg = "";
            if (indexlist.Length < 1)
                msg = "请选择一个设备";
            else if (indexlist.Length > 1)
                msg = "不能同时巡检多个设备";
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tab = this.dbGrid.DataSource as DataTable;
            FrmQueryState frmstate = new FrmQueryState();
            DataRow dr = tab.Rows[indexlist[0]];
            frmstate.deviceID = Convert.ToString(dr["站址"]);
            frmstate.target = this.getTarget(dr);
            frmstate.ShowDialog(this);
        }
        /// <summary>
        /// 加载屏显信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisInfoBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            if (indexlist.Length < 1)
            {
                MessageBox.Show("请选择一个设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FrmDisplay frmdis = new FrmDisplay();
            frmdis.Show();
        }

        /// <summary>
        /// 加载控制参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownComBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            if (indexlist.Length < 1)
            {
                MessageBox.Show("请选择一个设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataTable tab = this.dbGrid.DataSource as DataTable;
            DataRow drtarget = tab.Rows[indexlist[0]];

            UnitItem unitPark = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "设备控制维护");
            BindManager bg = new BindManager(this);
            NameObjectList ps = new NameObjectList();
            ps["code"] = drtarget["编号"];
            DataSet ds = bg.BuildDataset(unitPark, ps);

            FrmCommPara win = new FrmCommPara();
            win.DataSource = ds;
            win.Query = new QueryDataRes(unitPark.DataSrcFile);
            win.Position = 0;
            win.Target = this.getTarget(drtarget);
            win.FunType = "通讯";
            win.ShowDialog(this);
        }

        /// <summary>
        /// 加载系统时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetTimeBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            if (indexlist.Length < 1)
            {
                MessageBox.Show("请选择设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = "";
            string tpl = "停车场", cmd = "加载系统时间";
            DataTable tab = this.dbGrid.DataSource as DataTable;
            foreach (int i in indexlist)
            {
                DataRow dr = tab.Rows[i];
                CommiTarget target = this.getTarget(dr);
                CmdProtocol cmdP = new CmdProtocol(Convert.ToString(dr["名称"]), false);
                string tagdata = "@设备地址=" + Convert.ToString(dr["站址"]);
                cmdP.setCommand(tpl, cmd, tagdata);
                CommiManager.GlobalManager.SendCommand(target, cmdP);

                if (!cmdP.EventWh.WaitOne(2000, false))
                    msg = cmdP.CmdId + "：通讯失败,请检查设备连接或通讯参数后重试！";
                string rpmsg = basefun.valtag(cmdP.ResponseFormat, "{状态}");
                if (string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(rpmsg) && "操作成功！" != rpmsg)
                    msg = cmdP.CmdId + "：" + rpmsg;
                if (!string.IsNullOrEmpty(msg))
                    break;
            }
            if (string.IsNullOrEmpty(msg))
                MessageBox.Show("成功加载系统时间！", "设备通讯", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(msg, "设备通讯", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        /// <summary>
        /// 加载有效时段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetValTimeBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            string msg = "";
            if (indexlist.Length < 1)
                msg = "请选择一个设备";
            else if (indexlist.Length > 1)
                msg = "不能同时巡检多个设备";
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tab = this.dbGrid.DataSource as DataTable;
            DataRow dr = tab.Rows[indexlist[0]];
            BindManager.setTransParam(ParamManager.createParam(dr));

            FrmDownTime frmdowntime = new FrmDownTime();
            frmdowntime.DeviceID = Convert.ToString(dr["站址"]);
            frmdowntime.DeviceName = Convert.ToString(dr["名称"]);
            frmdowntime.Target = this.getTarget(dr);
            frmdowntime.ShowDialog(this);
        }

        /// <summary>
        /// 加载收费标准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetFeeBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            string msg = "";
            if (indexlist.Length < 1)
                msg = "请选择一个设备";
            else if (indexlist.Length > 1)
                msg = "不能同时巡检多个设备";
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tab = this.dbGrid.DataSource as DataTable;
            DataRow dr = tab.Rows[indexlist[0]];
            BindManager.setTransParam(ParamManager.createParam(dr));

            FrmFeePara win = new FrmFeePara();
            win.DeviceID = Convert.ToString(dr["站址"]);
            win.DeviceName = Convert.ToString(dr["名称"]);
            win.Target = this.getTarget(dr);
            win.ShowDialog(this);
        }

        /// <summary>
        /// 有效卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidCardBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            string msg = "";
            if (indexlist.Length < 1)
                msg = "请选择一个设备";
            else if (indexlist.Length > 1)
                msg = "不能同时下载多个设备的有效卡";
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tab = this.dbGrid.DataSource as DataTable;
            DataRow dr = tab.Rows[indexlist[0]];
            BindManager.setTransParam(ParamManager.createParam(dr));
            FrmValCard ValCard = new FrmValCard();
            ValCard.DeviceID = Convert.ToString(dr["站址"]);
            ValCard.DeviceName = Convert.ToString(dr["名称"]);
            ValCard.Target = this.getTarget(dr);
            ValCard.Query = new QueryDataRes("设备控制维护");
            ValCard.ShowDialog(this);
        }
        /// <summary>
        /// 读取记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadRecBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            string msg = "";
            if (indexlist.Length < 1)
                msg = "请选择一个设备";
            else if (indexlist.Length > 1)
                msg = "不能同时下载多个设备的有效卡";
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tab = this.dbGrid.DataSource as DataTable;
            DataRow dr = tab.Rows[indexlist[0]];

            FrmReadRecord win = new FrmReadRecord();
            win.DeviceID = Convert.ToString(dr["站址"]);
            win.DeviceName = Convert.ToString(dr["名称"]);
            win.Target = this.getTarget(dr);
            win.ShowDialog(this);
        }
        /// <summary>
        /// 设备监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoniDevBtn_Click(object sender, EventArgs e)
        {
            int[] indexlist = this.getCheckedList();
            string msg = "";
            if (indexlist.Length < 1)
                msg = "请选择一个设备";
            else if (indexlist.Length > 1)
                msg = "不能同时下载多个设备的有效卡";
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tab = this.dbGrid.DataSource as DataTable;
            DataRow dr = tab.Rows[indexlist[0]];
            FrmRemotControl win = new FrmRemotControl();
            win.DeviceID = Convert.ToString(dr["站址"]);
            win.DeviceName = Convert.ToString(dr["名称"]);
            win.Target = this.getTarget(dr);
            win.ShowDialog(this);
        }
    }
}