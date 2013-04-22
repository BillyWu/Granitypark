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
        /// ��ȡ������ͨѶĿ��,Ϊnull��ͨѶ
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }
        }
        private string deviceID;
        /// <summary>
        /// Ѳ���豸��ַ
        /// </summary>
        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }

        private string deviceName;
        /// <summary>
        /// �豸����
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
        /// ִ��ָ��,ָ������tag����@pm,@valֵ
        /// </summary>
        /// <param name="sender">����ָ�ť</param>
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
            
            //ִ��ָ��
            string cmd = "����ϵͳԶ��բ����";
            string tpl = "ͣ����";
            string tagdata = "@�豸��ַ=" + this.deviceID;
            tagdata = basefun.setvaltag(tagdata, "{" + pm + "}", val);
            CmdProtocol cmdP = new CmdProtocol(cmd, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("ͨѶʧ��,�����豸���ӻ�ͨѶ���������ԣ�", "ͨѶ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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