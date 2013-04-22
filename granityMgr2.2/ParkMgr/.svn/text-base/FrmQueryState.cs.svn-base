using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.communications;
using Granity.winTools;
using Estar.Business.UserRight;
//using Granity.winTools;
using Granity.CardOneCommi;

namespace Granity.parkStation
{
    public partial class FrmQueryState : Form
    {
        /// <summary>
        /// 巡检设备地址
        /// </summary>
        public string deviceID;
        /// <summary>
        /// 设备通讯目标参数
        /// </summary>
        public CommiTarget target;

        public FrmQueryState()
        {
            InitializeComponent();
        }

        private void FrmQueryState_Load(object sender, EventArgs e)
        {
            this.lbInfonew.Text = "";
            this.lbDate.Text = "";
            this.lbWorker.Text = "";

            string tpl = "停车场", cmd = "检测状态";
            CmdProtocol cmdP = new CmdProtocol("巡检设备" + this.deviceID, false);
            string tagdata = "@设备地址=" + this.deviceID;
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000,false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                this.cmdP_ResponseHandle(cmdP);
        }

        /// <summary>
        /// 巡检响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdP_ResponseHandle(CmdProtocol cmd)
        {
            if (null == cmd || string.IsNullOrEmpty(cmd.CmdId))
                return;
            User user = BindManager.getUser();
            this.lbWorker.Text = user.UserName;
            string info = cmd.ResponseFormat;

            this.lbDate.Text = basefun.valtag(info, "{当前时间}").Replace(" ", "\n\r");
            this.lbInfonew.Text = basefun.valtag(info, "{新记录}");
            string state = basefun.valtag(info, "{内部状态}");
            for (int i = 0; i < state.Length; i++)
            {
                CheckBox chk = this.findCheckBox(this.grpInState, "rdfld", "xc" + Convert.ToString(i));
                if (null == chk) continue;
                chk.Checked = '1' == state[i] ? true : false;
            }
            state = basefun.valtag(info, "{输入输出状态}");
            for (int i = 0; i < state.Length; i++)
            {
                CheckBox chk = this.findCheckBox(this.grpInState, "rdfld", "yc" + Convert.ToString(i));
                if (null == chk) continue;
                chk.Checked = '1' == state[i] ? true : false;
            }
        }

        /// <summary>
        /// 根据tag标记name的值val找到复选框
        /// </summary>
        /// <param name="name">设定标记名称</param>
        /// <param name="val">标记值</param>
        /// <returns></returns>
        private CheckBox findCheckBox(Control ct, string name, string val)
        {
            if (null == ct || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(val))
                return null;
            if (ct is CheckBox)
            {
                string tag = Convert.ToString(ct.Tag);
                if (val == basefun.valtag(tag, name))
                    return ct as CheckBox;
            }
            foreach (Control child in ct.Controls)
            {
                CheckBox chk = this.findCheckBox(child, name, val);
                if (null != chk) return chk;
            }
            return null;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}