using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Granity.communications;
using Granity.CardOneCommi;

namespace Granity.parkStation.cardManager
{
    public partial class FrmRemotControl : Form
    {
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
        /// 巡检设备地址
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

        public FrmRemotControl()
        {
            InitializeComponent();
        }

        private void FrmRemotControl_Load(object sender, EventArgs e)
        {
            this.lbDeviceInfo.Text = "  " + this.deviceName + "(" + this.deviceID + ")";
        }

        /// <summary>
        /// 执行指令,指令标记在tag属性@pm,@val值
        /// </summary>
        /// <param name="sender">触发指令按钮</param>
        /// <param name="e"></param>
        private void btControl_Click(object sender, EventArgs e)
        {
            if (null == sender || !(sender is Control))
                return;
            string tag = Convert.ToString(((Control)sender).Tag);
            string pm = basefun.valtag(tag, "pm");
            if (string.IsNullOrEmpty(pm))
                return;
            string val = basefun.valtag(tag, "val");
            if (string.IsNullOrEmpty(val))
                return;
            
            //执行指令
            string cmd = "管理系统远程闸控制";
            string tpl = "停车场";
            string tagdata = "@设备地址=" + this.deviceID;
            tagdata = basefun.setvaltag(tagdata, "{" + pm + "}", val);
            CmdProtocol cmdP = new CmdProtocol(cmd, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                this.cmdP_ResponseHandle(cmdP);
        }

        private void cmdP_ResponseHandle(CmdProtocol cmdP)
        {
            if (null == cmdP || string.IsNullOrEmpty(cmdP.ResponseFormat))
                return;
            MessageBox.Show(cmdP.ResponseFormat);
        }
    }
}