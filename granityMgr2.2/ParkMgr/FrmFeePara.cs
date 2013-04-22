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
using System.Drawing.Printing;
using System.Data.SqlClient;
using Granity.communications;
using Granity.CardOneCommi;

namespace Granity.parkStation
{
    public partial class FrmFeePara : Form
    {
        string unitName = "";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = new DataSet();
        BindManager bindMgr;
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

        public FrmFeePara()
        {
            InitializeComponent();
        }

        private void FrmFeePara_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.deviceName))
            {

                //读取业务单元和传递参数
                this.paramwin = BindManager.getSystemParam();
                NameObjectList pstrans = BindManager.getTransParam();
                ParamManager.MergeParam(this.paramwin, pstrans);
                unitName = pstrans["name"].ToString();//单元
                unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
                //绑定数据
                BindManager bg = new BindManager(this);
                this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
                bg.BindFld(this, this.dsUnit);
                if (unitName == "停车场收费标准")
                {
                    lbDeviceInfo.Visible = false;
                    this.DownBtn.Visible = false;
                    QueryBtn.Visible = false;
                    BtnClose.Visible = false;
                }
            }
            else
            {
                this.cbodev.Visible = false;
                this.btnsave.Visible = false;
                this.lbDeviceInfo.Text = "  " + this.deviceName + "(" + this.deviceID + ")";
            }
        }
        /// <summary>
        /// 设置容器内tag属性有@pm标记的,有@val取其值,是Radio取选中的值
        /// </summary>
        /// <param name="ct">容器控件</param>
        /// <param name="tagdata">tag格式数据</param>
        /// <returns></returns>
        private string getTagdata(Control ct, string tagdata)
        {
            if (null == ct || !ct.Visible)
                return tagdata;
            string tag = Convert.ToString(ct.Tag);
            string pm = basefun.valtag(tag, "pm");
            if (!string.IsNullOrEmpty(pm))
            {
                pm = "{" + pm + "}";
                string val = basefun.valtag(tag, "val");
                if (string.IsNullOrEmpty(val))
                    val = ct.Text;
                if (ct is RadioButton)
                {
                    if (((RadioButton)ct).Checked)
                        return basefun.setvaltag(tagdata, pm, val);
                    return tagdata;
                }
                return basefun.setvaltag(tagdata, pm, val);
            }
            foreach (Control child in ct.Controls)
                tagdata = getTagdata(child, tagdata);
            return tagdata;
        }

        /// <summary>
        /// 设置容器内tag属性有@pm标记的,有@val取其值,是Radio取选中的值
        /// </summary>
        /// <param name="ct">容器控件</param>
        /// <param name="tagdata">tag格式数据</param>
        /// <returns></returns>
        private string setTagdata(Control ct, string tagdata)
        {
            if (null == ct || !ct.Visible || string.IsNullOrEmpty(tagdata))
                return tagdata;
            string tag = Convert.ToString(ct.Tag);
            string pm = basefun.valtag(tag, "pm");
            if (!string.IsNullOrEmpty(pm))
            {
                pm = "{" + pm + "}";
                string val = basefun.valtag(tagdata, pm);
                if (ct is RadioButton)
                {
                    string v = basefun.valtag(tag, "val");
                    if (val == v)
                        ((RadioButton)ct).Checked = true;
                    else
                        ((RadioButton)ct).Checked = false;
                    return tagdata;
                }
                ct.Text = val;
                return tagdata;
            }
            foreach (Control child in ct.Controls)
                tagdata = setTagdata(child, tagdata);
            return tagdata;
        }

        /// <summary>
        /// 写入通讯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownBtn_Click(object sender, EventArgs e)
        {
            string tagdata = "@设备地址=" + this.deviceID;
            tagdata = this.getTagdata(this.grpCarType, tagdata);
            tagdata = this.getTagdata(this.grpFeeType, tagdata);
            tagdata = this.getTagdata(this.tabFeeStd.SelectedTab, tagdata);

            string cmd = "下载收费标准" + basefun.valtag(tagdata, "{方式代码}");
            string tpl = "停车场";
            CmdProtocol cmdP = new CmdProtocol(cmd, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(5000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "设备通讯", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("成功下载收费标准！", "设备通讯", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 读取收费标准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryBtn_Click(object sender, EventArgs e)
        {
            string tagdata = "@设备地址=" + this.deviceID;
            tagdata = this.getTagdata(this.grpCarType, tagdata);
            tagdata = this.getTagdata(this.grpFeeType, tagdata);
            tagdata = this.getTagdata(this.tabFeeStd.SelectedTab, tagdata);

            string cmd = "读取收费标准" + basefun.valtag(tagdata, "{方式代码}");
            string tpl = "停车场";
            CmdProtocol cmdP = new CmdProtocol(cmd, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(5000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                tagdata = cmdP.ResponseFormat;
                this.setTagdata(this.grpCarType, tagdata);
                this.setTagdata(this.Grpfeetp, tagdata);
                this.setTagdata(this.grpFeeType, tagdata);
                MessageBox.Show("成功检测出设备数据！", "设备通讯", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void cmdP_ResponseHandle(CmdProtocol cmdP)
        {
            if (null == cmdP || string.IsNullOrEmpty(cmdP.ResponseFormat))
                return;
            MessageBox.Show(cmdP.ResponseFormat);
        }

        private void RdoFee1_CheckedChanged(object sender, EventArgs e)
        {
            tabFeeStd.SelectedIndex = 0;
        }

        private void RdoFee2_CheckedChanged(object sender, EventArgs e)
        {
            tabFeeStd.SelectedIndex = 1;
        }

        private void RdoFee3_CheckedChanged(object sender, EventArgs e)
        {
            tabFeeStd.SelectedIndex = 2;
        }

        private void RdoFee4_CheckedChanged(object sender, EventArgs e)
        {
            tabFeeStd.SelectedIndex = 3;
        }

        private void RdoFee5_CheckedChanged(object sender, EventArgs e)
        {
            tabFeeStd.SelectedIndex = 4;
        }

        private void RdoFee6_CheckedChanged(object sender, EventArgs e)
        {
            tabFeeStd.SelectedIndex = 5;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}