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
    public partial class FrmCommPara : Form
    {

        private DataSet ds;
        /// <summary>
        /// 绑定数据源
        /// </summary>
        public DataSet DataSource
        {
            get { return this.ds; }
            set { this.ds = value; }
        }
        private int position = -1;
        /// <summary>
        /// 绑定数据源当前行索引号
        /// </summary>
        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                if (this.ds.Tables.Count < 0)
                    return;
                DataTable tab = this.ds.Tables[0];
                if (null == this.BindingContext[tab])
                    return;
                this.BindingContext[tab].Position = position;
            }
        }

        private string funType;
        /// <summary>
        /// 读取或设置模式窗口功能类别：管理|通讯
        /// </summary>
        public string FunType
        {
            get { return funType; }
            set { funType = value; }
        }

        private CommiTarget target;
        /// <summary>
        /// 读取或设置通讯目标,为null不通讯
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }
        }

        private QueryDataRes query;
        /// <summary>
        /// 读取或设置数据操作
        /// </summary>
        public QueryDataRes Query
        {
            get { return query; }
            set { query = value; }
        }

        public FrmCommPara()
        {
            InitializeComponent();
        }

        private void FrmCommPara_Load(object sender, EventArgs e)
        {
            if (null == this.ds || !this.ds.Tables.Contains("设备列表"))
                return;
            this.refresh();
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        private void refresh()
        {
            BindManager bindmgr = new BindManager(this);
            bindmgr.BindFld(this, this.ds);
            DataTable tab = DataSource.Tables["设备列表"];
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position < 0 || tab.Rows.Count < 1)
                return;
            if (position > 0)
                frmmgr.Position = position;
            //通道类型
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            bool channelbycard = this.CardTpRadio.Checked = Convert.ToBoolean(dr["通道选择类型"]);
            Panel plchannel = this.plCarType;
            if (channelbycard)
            {
                this.plCarType.Hide();
                this.plCardType.Show();
                plchannel = this.plCardType;
            }
            this.CarTpRadio.Checked = !channelbycard;
            //通道,卡类允许,放行控制 三项分组选配内容
            this.setCheckedsgrp(this.grpbChannel, "通道选择内容", Convert.ToString(dr["通道选择内容"]));
            this.setCheckedsgrp(this.grpbCardType, "卡类允许", Convert.ToString(dr["卡类允许"]));
            this.setCheckedsgrp(this.grpbAccept, "放行控制", Convert.ToString(dr["放行控制"]));

            if ("通讯" == this.funType)
                this.plBtngrp.Show();
            else
                this.plBtngrp.Hide();
        }
        /// <summary>
        /// 计算容器内分组项的复选框内容，结果以逗号分割的值
        /// 在checkbox控件的tag属性上设置val和grp标记值,val是需要设置的代表的结果值
        /// </summary>
        /// <param name="ct">控件容器</param>
        /// <param name="grp">分组值，tag标记值的grp值对比</param>
        /// <returns>选中的复选框内容(逗号分割)</returns>
        private string getCheckedsgrp(Control ct, string grp)
        {
            if (null == ct || string.IsNullOrEmpty(grp))
                return "";
            if (ct is CheckBox)
            {
                string tag = Convert.ToString(ct.Tag);
                if (grp == basefun.valtag(tag, "grp") && ((CheckBox)ct).Checked)
                    return basefun.valtag(tag, "val");
                return "";
            }
            string strchk = "";
            foreach (Control child in ct.Controls)
            {
                string val = getCheckedsgrp(child, grp);
                if (!string.IsNullOrEmpty(val))
                    strchk += "," + val;
            }
            if (strchk.StartsWith(","))
                strchk = strchk.Substring(1);
            return strchk;
        }

        /// <summary>
        /// 设置容器内分组项的复选框内容，选项值以逗号分割
        /// </summary>
        /// <param name="ct">控件容器</param>
        /// <param name="grp">分组值</param>
        /// <param name="vals">选项值，以逗号分割</param>
        private void setCheckedsgrp(Control ct, string grp, string vals)
        {
            if (null == ct || !ct.Visible || string.IsNullOrEmpty(grp))
                return;
            if (ct is CheckBox)
            {
                string tag = Convert.ToString(ct.Tag);
                string val = basefun.valtag(tag, "val");
                if (string.IsNullOrEmpty(val) || grp != basefun.valtag(tag, "grp"))
                    return;
                if (vals.Contains("," + val + ",") || vals.StartsWith(val + ",") || vals.EndsWith("," + val) || val == vals)
                    ((CheckBox)ct).Checked = true;
                else
                    ((CheckBox)ct).Checked = false;
                return;
            }
            foreach (Control child in ct.Controls)
                setCheckedsgrp(child, grp, vals);
        }

        /// <summary>
        /// 按照车型选配通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarTpRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.plCarType.Visible)
                return;
            DataTable tab = DataSource.Tables["park"];
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position < 0 || tab.Rows.Count < 1)
                return;
            if (position > 0)
                frmmgr.Position = position;
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            dr["通道选择类型"] = false;
            this.plCardType.Hide();
            this.plCarType.Show();
            foreach (Control ctrl in this.plCardType.Controls)
            {
                if (!(ctrl is CheckBox))
                    continue;
                ((CheckBox)ctrl).Checked = false;
            }
        }
        /// <summary>
        /// 按照卡类型选配通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CardTpRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.plCardType.Visible)
                return;
            DataTable tab = DataSource.Tables["park"];
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position < 0 || tab.Rows.Count < 1)
                return;
            if (position > 0)
                frmmgr.Position = position;
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            dr["通道选择类型"] = true;
            this.plCarType.Hide();
            this.plCardType.Show();
            foreach (Control ctrl in this.plCarType.Controls)
            {
                if (!(ctrl is CheckBox))
                    continue;
                ((CheckBox)ctrl).Checked = false;
            }
        }

        /// <summary>
        /// 确定返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseBtn_Click(object sender, EventArgs e)
        {
            if (null == this.ds || !this.ds.Tables.Contains("park") || this.ds.Tables["park"].Rows.Count < 1)
            {
                this.Close();
                return;
            }
            DataTable tab = this.ds.Tables["park"];
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position > 0)
                frmmgr.Position = position;
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            string strchk = this.getCheckedsgrp(this.grpbChannel, "通道选择内容");
            dr["通道选择内容"] = strchk;
            strchk = this.getCheckedsgrp(this.grpbCardType, "卡类允许");
            dr["卡类允许"] = strchk;
            strchk = this.getCheckedsgrp(this.grpbAccept, "放行控制");
            dr["放行控制"] = strchk;
            this.Close();
        }
        /// <summary>
        /// 读取设备控制参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRead_Click(object sender, EventArgs e)
        {
            if (null == this.ds || !this.ds.Tables.Contains("park")
                || this.ds.Tables["park"].Rows.Count < 1 || !this.ds.Tables["park"].Columns.Contains("dev_id"))
            {
                return;
            }
            DataTable tab = this.ds.Tables["park"];
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position > 0)
                frmmgr.Position = position;
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            string addr = Convert.ToString(dr["dev_id"]);
            if (string.IsNullOrEmpty(addr))
            {
                MessageBox.Show("没有维护设备地址,不能通讯！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string tpl = "停车场", cmd = "读取控制参数";
            CmdProtocol cmdP = new CmdProtocol("读取控制参数" + addr, false);
            string tagdata = "@设备地址=" + addr;
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(5000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                this.cmdP_ResponseHandle(cmdP);
        }
        /// <summary>
        /// 响应指令结果
        /// </summary>
        /// <param name="cmdP"></param>
        private void cmdP_ResponseHandle(CmdProtocol cmd)
        {
            DataTable tab = DataSource.Tables["park"];
            if (null == cmd || string.IsNullOrEmpty(cmd.CmdId) || string.IsNullOrEmpty(cmd.ResponseFormat)
                || null == tab || tab.Rows.Count <= this.position)
                return;
            string info = cmd.ResponseFormat;
            string[,] colmap ={ { "{有效期报警门限}", "有效报警" }, { "{卡余额报警门警}", "卡余额报警" },
                                { "{场内场逻辑控制}", "内场逻辑" },{"脱机处理","脱机"},{"脱机时间","脱机时间"}, {"{车位占用卡处理}","车位占用处理"},
                                {"车型","按键默认车型"},{"方式","临时卡方式"},{"{场内场逻辑控制}","内场逻辑"}};
            // 灯饰设置s/e,脱机,放行控制(3期卡/4时段卡/5临时卡/6免费卡/7临免卡/8贵宾卡/9储值卡/10一卡通),
            //卡类允许(),通道选择内容(卡/车: 1大车/2中型车/3小车/4摩托车),满位允许()

            //@{帧头}=02,@{设备地址}=1,@{状态}=操作成功！,@{命令长度}=0040,@{本机地址}=1,@{系统标识符}=2102803,@{通信密码}=0,@{系统密码}=0,@{用户密码}=1467233176,
            //@{出卡机欠量报警门限}=0,@{总车位数}=1000,@脱机处理=1,@脱机时间=8.0,@车型=5,@方式=1,@{货币单位}=01,@{入口满位处理}=10101010,@{外设配置}=00000010,
            //@{放行控制}=00010100,@{场内场逻辑控制}=01,@{车位占用卡处理}=00000000,@{卡类允许}=11111111,@{通道选择}=00000000,@{有效期报警门限}=30,
            //@{卡余额报警门警}=0,@中央收费=0,@有效时间=10,@{进出逻辑控制}=86,@灯1=043520,@灯2=00:23,@{保留车位}=15104,@{场内场编号}=10,
            //@{卡片使用区}=0,@{最高余额}=2047,@{最低余额}=65280,@{保留字}=0,@{校验字}=DD,@{帧尾}=03
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position > 0)
                frmmgr.Position = position;
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            for (int i = 0; i < colmap.GetLength(0); i++)
            {
                if (typeof(bool) == tab.Columns[colmap[i, 1]].DataType)
                    dr[colmap[i, 1]] = "1" == basefun.valtag(info, colmap[i, 0]) ? true : false;
                else
                    dr[colmap[i, 1]] = basefun.valtag(info, colmap[i, 0]);
            }
            dr["按键默认车型"] = Convert.ToString(Convert.ToInt64(dr["按键默认车型"]), 16).ToUpper();
            dr["进出逻辑控制"] = "AA" == basefun.valtag(info, "{进出逻辑控制}");
            colmap = new string[,] { { "灯1", "灯饰设置s" }, { "灯2", "灯饰设置e" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
            {
                string val = basefun.valtag(info, colmap[i, 0]);
                if (!val.Contains(":"))
                    val = "00:00";
                dr[colmap[i, 1]] = val;
            }

            string v = basefun.valtag(info, "{入口满位处理}");
            dr["满位允许"] = "1010" == v.Substring(0, 4);
            dr["满位临时卡"] = "1010" == v.Substring(4, 4);
            v = basefun.valtag(info, "{外设配置}");
            string[] cols ={ "地感", "车位屏", "中文屏1", "中文屏2", "读头1", "读头2" };
            for (int i = 0; i < cols.Length; i++)
                dr[cols[i]] = "1" == v.Substring(i, 1);

            v = basefun.valtag(info, "{放行控制}");
            string vals = "";
            for (int i = 0; i < v.Length; i++)
            {
                if ("1" == v.Substring(i, 1))
                    vals += "," + Convert.ToString(7 - i + 3);
            }
            dr["放行控制"] = vals;
            v = basefun.valtag(info, "{卡类允许}");
            vals = "";
            for (int i = 0; i < 8; i++)
            {
                if ("1" == v.Substring(i, 1))
                    vals += "," + Convert.ToString(i + 3);
            }
            dr["卡类允许"] = vals;

            v = basefun.valtag(info, "{通道选择}");
            dr["通道选择类型"] = "1" == v.Substring(0, 1);
            vals = "";
            if (true.Equals(dr["通道选择类型"]))
                cols = new string[] { "10", "9", "8", "6", "4", "3" };
            else
                cols = new string[] { "4", "3", "2", "1" };
            for (int i = 0; i < cols.Length; i++)
                if ("1" == v.Substring(v.Length - i - 1, 1))
                    vals += "," + cols[i];
            dr["通道选择内容"] = vals;
            bool isCenter = "1" == basefun.valtag(info, "中央收费");
            dr["中央收费"] = isCenter;
            dr["有效时间"] = !isCenter ? "0" : basefun.valtag(info, "有效时间");

            this.refresh();
            MessageBox.Show("成功检测设备控制参数！");
        }
        /// <summary>
        /// 写入控制参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btWrite_Click(object sender, EventArgs e)
        {
            if (null == this.ds || !this.ds.Tables.Contains("park")
                || this.ds.Tables["park"].Rows.Count < 1 || !this.ds.Tables["park"].Columns.Contains("dev_id"))
            {
                this.Close();
                return;
            }

            DataTable tab = this.ds.Tables["park"];
            BindingManagerBase frmmgr = this.BindingContext[tab];
            if (position > 0)
                frmmgr.Position = position;
            DataRow dr = ((DataRowView)frmmgr.Current).Row;
            string addr = Convert.ToString(dr["dev_id"]);
            if (string.IsNullOrEmpty(addr))
            {
                MessageBox.Show("没有维护设备地址,不能通讯！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string strchk = this.getCheckedsgrp(this.grpbChannel, "通道选择内容");
            dr["通道选择内容"] = strchk;
            strchk = this.getCheckedsgrp(this.grpbCardType, "卡类允许");
            dr["卡类允许"] = strchk;
            strchk = this.getCheckedsgrp(this.grpbAccept, "放行控制");
            dr["放行控制"] = strchk;

            //@{出卡机欠量报警门限}=0,@{总车位数}=1000,@{脱机时间}.{脱机处理}=1,@{脱机时间}.{脱机时间}=25,
            //@{临时卡出卡选择}.{车型}=C,@{临时卡出卡选择}.{方式}=1,@{货币单位}=01,@{入口满位处理}.{入场}=0,@{入口满位处理}.{临时卡}=0,
            //@{外设配置}=00011001,@{放行控制}=00101001,@{场内场逻辑控制}=aa,@{车位占用卡处理}=01010000,
            //@{卡类允许}=11111111,@{通道选择}=00000110,@{有效期报警门限}=20,@{卡余额报警门警}=30,
            //@{中央收费}.{中央收费}=1,@{中央收费}.{有效时间}=30,@{进出逻辑控制}=AA,@{灯饰设置}.{h1}=10,
            //@{灯饰设置}.{m1}=30,@{灯饰设置}.{h2}=08,@{灯饰设置}.{m2}=30,@{保留车位}=10,@{场内场编号}=10,@{卡片使用区}=7,@{最高余额}=65535,@{最低余额}=20

            //@{帧头}=02,@{设备地址}=1,@{状态}=操作成功！,@{命令长度}=0040,@{本机地址}=1,@{系统标识符}=2102803,@{通信密码}=0,@{系统密码}=0,
            //@{用户密码}=1467233176,@{出卡机欠量报警门限}=0,@{总车位数}=0,@脱机处理=1,@脱机时间=80,@车型=0,@方式=6,@{货币单位}=0A,
            //@{入口满位处理}=00000000,@{外设配置}=10100100,@{放行控制}=00010100,@{场内场逻辑控制}=3,@{车位占用卡处理}=00010000,@{卡类允许}=11111111,
            //@{通道选择}=00001100,@{有效期报警门限}=30,@{卡余额报警门警}=80,@中央收费=0,@有效时间=0,@{进出逻辑控制}=AA,@灯1=00:00,@灯2=00:00,
            //@{保留车位}=0,@{场内场编号}=0,@{卡片使用区}=7,@{最高余额}=0,@{最低余额}=0,@{保留字}=0,@{校验字}=82,@{帧尾}=03

            string[,] colmap ={ { "{有效期报警门限}", "有效报警" }, { "{卡余额报警门警}", "卡余额报警" },{"{总车位数}","allstall"},
                                { "{场内场逻辑控制}", "内场逻辑" }, {"{脱机时间}.{脱机处理}","脱机"},{"{脱机时间}.{脱机时间}","脱机时间"}, {"{车位占用卡处理}","车位占用处理"},
                                {"{临时卡出卡选择}.{方式}","临时卡方式"}, {"{场内场逻辑控制}","内场逻辑"},
                                {"{中央收费}.{中央收费}","中央收费"}, {"{中央收费}.{有效时间}","有效时间"},
                                {"{入口满位处理}.{入场}", "满位允许" }, { "{入口满位处理}.{临时卡}", "满位临时卡" }
                             };

            string tagdata = "@设备地址=" + addr;
            tagdata = basefun.setvaltag(tagdata, "{货币单位}", "01");
            tagdata = basefun.setvaltag(tagdata, "{卡片使用区}", "7");
            for (int i = 0; i < colmap.GetLength(0); i++)
            {
                string val = Convert.ToString(dr[colmap[i, 1]]);
                if (true.Equals(dr[colmap[i, 1]])) val = "1";
                if (false.Equals(dr[colmap[i, 1]])) val = "0";
                if ("1" == val && ("满位允许" == colmap[i, 1] || "满位临时卡" == colmap[i, 1]))
                    val = "10";
                tagdata = basefun.setvaltag(tagdata, colmap[i, 0], val);
            }
            string v = Convert.ToString(dr["按键默认车型"]);
            tagdata = basefun.setvaltag(tagdata, "{临时卡出卡选择}.{车型}", Convert.ToString(Convert.ToInt64(v, 16)));
            if (true.Equals(dr["进出逻辑控制"]))
                tagdata = basefun.setvaltag(tagdata, "{进出逻辑控制}", "AA");
            else
                tagdata = basefun.setvaltag(tagdata, "{进出逻辑控制}", "86");
            v = Convert.ToString(dr["灯饰设置s"]) + ":" + Convert.ToString(dr["灯饰设置e"]);
            string[] vals = v.Split(":".ToCharArray(), 4);
            if (4 != vals.Length) vals = new string[] { "00", "00", "00", "00" };
            string[] cols ={ "{灯饰设置}.{h1}", "{灯饰设置}.{m1}", "{灯饰设置}.{h2}", "{灯饰设置}.{m2}" };
            for (int i = 0; i < cols.Length; i++)
                tagdata = basefun.setvaltag(tagdata, cols[i], vals[i]);
            cols = new string[] { "地感", "车位屏", "中文屏1", "中文屏2", "读头1", "读头2" };
            v = "";
            for (int i = 0; i < cols.Length; i++)
                v += true.Equals(dr[cols[i]]) ? "1" : "0";
            tagdata = basefun.setvaltag(tagdata, "{外设配置}", v + "00");

            string valk = "";
            colmap = new string[,] { { "{放行控制}", "放行控制" }, { "{卡类允许}", "卡类允许" } };
            for (int k = 0; k < colmap.GetLength(0); k++)
            {
                valk = "," + Convert.ToString(dr[colmap[k, 1]]) + ",";
                v = "";
                for (int i = 0; i < 8; i++)
                {
                    if (valk.Contains("," + Convert.ToString(i + 3) + ","))
                        v = "1" + v;
                    else
                        v = "0" + v;
                }
                tagdata = basefun.setvaltag(tagdata, colmap[k, 0], v);
            }

            valk = "," + Convert.ToString(dr["通道选择内容"]) + ",";
            v = "";
            if (true.Equals(dr["通道选择类型"]))
                cols = new string[] { "3", "4", "6", "8", "9", "10" };
            else
                cols = new string[] { "1", "2", "3", "4" };
            for (int i = 0; i < cols.Length; i++)
                if (valk.Contains("," + cols[i] + ","))
                    v += "1";
                else
                    v += "0";
            if (true.Equals(dr["通道选择类型"]))
                v = "10" + v;
            else
                v = "0000" + v;
            tagdata = basefun.setvaltag(tagdata, "{通道选择}", v);

            string tpl = "停车场", cmd = "下载控制参数";
            CmdProtocol cmdP = new CmdProtocol(addr, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(5000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                this.cmdP_ResponseWritedHandle(cmdP);
        }

        /// <summary>
        /// 写入指令响应
        /// </summary>
        /// <param name="cmd">指令对象</param>
        private void cmdP_ResponseWritedHandle(CmdProtocol cmd)
        {
            if (null == cmd || string.IsNullOrEmpty(cmd.ResponseFormat))
            {
                MessageBox.Show("通讯失败！检查指令", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (null != this.ds && null != this.Query && this.ds.Tables.Contains("park") && this.ds.Tables["park"].Rows.Count > 0)
            {
                DataTable tab = this.ds.Tables["park"];
                DataRow dr = this.ds.Tables["park"].Rows[0];
                if (DataRowState.Modified == dr.RowState)
                {
                    NameObjectList ps = ParamManager.createParam(dr);
                    this.Query.ExecuteUpdate(tab.TableName, ps);
                }
            }
            MessageBox.Show("成功下载设备控制参数！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}