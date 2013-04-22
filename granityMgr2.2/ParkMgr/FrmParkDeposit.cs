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
using Granity.communications;
using Granity.CardOneCommi;
using System.Text.RegularExpressions;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmParkDeposit : Form
    {
        private string tag;
        /// <summary>
        /// 读取或设置收费数据
        /// </summary>
        public string DataTag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        private int addrst = -1;
        /// <summary>
        /// 通讯设备站址
        /// </summary>
        public int AddrStation
        {
            get { return this.addrst; }
            set { this.addrst = value; }
        }
        private CommiTarget target;
        /// <summary>
        /// 通讯目标
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }
        }
        private QueryDataRes query;
        /// <summary>
        /// 设置或读取当前数据查询实例
        /// </summary>
        public QueryDataRes Query
        {
            get { return query; }
            set { query = value; }
        }

        public FrmParkDeposit()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 更加tag标记值更新控件内控件值,有下拉框则添加对应“名称”值
        /// </summary>
        /// <param name="ct">控件容器</param>
        /// <param name="tag">tag格式数据</param>
        /// <param name="keyName">tag标记映射标记名称</param>
        /// <returns>返回tag格式数据</returns>
        private string setContainerTagValue(Control ct, string tag, string keyName)
        {
            if (null == ct || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(keyName))
                return tag;
            string t = Convert.ToString(ct.Tag);
            string pm = basefun.valtag(t, keyName);
            if (!string.IsNullOrEmpty(pm))
            {
                string val = basefun.valtag(tag, "{" + pm + "}");
                string v = val;
                if (string.IsNullOrEmpty(val))
                    v = basefun.valtag(tag, pm);
                if (!(ct is ListControl))
                    ct.Text = v;
                else
                {
                    ListControl cbb = ct as ListControl;
                    try { cbb.SelectedValue = val; }
                    catch { }
                    if (string.IsNullOrEmpty(val))
                        tag = basefun.setvaltag(tag, pm + "名称", cbb.Text);
                    else
                        tag = basefun.setvaltag(tag, "{" + pm + "名称" + "}", cbb.Text);
                }
            }
            foreach (Control child in ct.Controls)
                tag = this.setContainerTagValue(child, tag, keyName);
            return tag;
        }

        private void FrmParkDeposit_Load(object sender, EventArgs e)
        {
            this.setContainerTagValue(this.grpInfo, this.tag, "pm");
            //直接通讯获取停车费用
            if (this.addrst < 0 || null == this.target)
                return;
            string cardnum = basefun.valtag(this.tag, "卡号");
            string cardtype = basefun.valtag(this.tag, "卡类");
            string cartype = basefun.valtag(this.tag, "车型");
            string dtparkin = basefun.valtag(this.tag, "入场时间");
            if (string.IsNullOrEmpty(cardtype) || string.IsNullOrEmpty(cartype) || string.IsNullOrEmpty(dtparkin))
                return;
            string data="";
            NameObjectList ps = ParamManager.createParam(this.tag);
            foreach (string key in ps.AllKeys)
                data = basefun.setvaltag(data, "{" + key + "}", Convert.ToString(ps[key]));

            data = basefun.setvaltag(data, "设备地址", Convert.ToString(this.addrst));
            data = basefun.setvaltag(data, "{设备地址}", Convert.ToString(this.addrst));
            string dtparkout = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            data = basefun.setvaltag(data, "{出场时间}", dtparkout);
            data = basefun.setvaltag(data, "{出入场时间}", dtparkout);
            data = basefun.setvaltag(data, "{开闸方式}", "00");
            this.tag = data;
            CmdProtocol cmdP = new CmdProtocol(false);
            cmdP.setCommand("停车场", "计算收费金额", data);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                return;
            string money = basefun.valtag(cmdP.ResponseFormat, "{收费金额}");
            if (string.IsNullOrEmpty(money))
                return;
            this.tbPkmoney.Text = money;
            this.tag = basefun.setvaltag(this.tag, "{收费金额}", money);
            string cardmoney = basefun.valtag(this.tag, "{卡片余额}");
            try
            {
                money = Convert.ToString(Convert.ToDecimal(money) - Convert.ToDecimal(cardmoney));
                this.tbPkfee.Text = money;
                this.tag = basefun.setvaltag(this.tag, "{收费}", money);
            }
            catch { }
        }
        Regex regUperCase = new Regex("[A-Z]", RegexOptions.Compiled);
        /// <summary>
        /// 押金处理，小票打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCharge_Click(object sender, EventArgs e)
        {
            string province = this.cbbProvince.Text;
            if (!string.IsNullOrEmpty(province))
                province = regUperCase.Replace(province, "");
            string carnum = this.tbCarnum.Text;
            if (!string.IsNullOrEmpty(carnum))
                carnum = province + carnum;
            this.tag = basefun.setvaltag(this.tag, "{车牌号码}", carnum);
            //编号
            string code = BindManager.getCodeSn("P");
            this.tag = basefun.setvaltag(this.tag, "{打印编号}", code);
            //打印小票
            string printer = DataAccRes.AppSettings("押金打印机");
            try
            {
                if (!string.IsNullOrEmpty(printer))
                    this.printDeposit.PrinterSettings.PrinterName = printer;
                this.printDeposit.Print();
            }
            catch { }
            //押金处理
            NameObjectList ps = ParamManager.createParam(this.tag);
            ps["操作类别"] = "收取";
            ParamManager.MergeParam(ps, BindManager.getSystemParam(), false);
            this.Query.ExecuteNonQuery("押金处理", ps, ps, ps);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        /// <summary>
        /// 退还押金
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBack_Click(object sender, EventArgs e)
        {
            string province = this.cbbProvince.Text;
            if (!string.IsNullOrEmpty(province))
                province = regUperCase.Replace(province, "");
            string carnum = this.tbCarnum.Text;
            if (!string.IsNullOrEmpty(carnum))
                carnum = province + carnum;
            this.tag = basefun.setvaltag(this.tag, "{车牌号码}", carnum);
            //押金处理
            NameObjectList ps = ParamManager.createParam(this.tag);
            //ps["操作类别"] = "退费";
            ps["操作类别"] = "不退费再入场";
            this.tag = basefun.setvaltag(this.tag, "{收费操作}", "不退费再入场");
            ParamManager.MergeParam(ps, BindManager.getSystemParam(), false);
            this.Query.ExecuteNonQuery("押金处理", ps, ps, ps);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        /// <summary>
        /// 收取停车费用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFee_Click(object sender, EventArgs e)
        {
            //删除设备入场记录,并返回tag模拟的出入场tag标记值
            if (null == this.target || this.addrst < 0)
            {
                MessageBox.Show("请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string province = this.cbbProvince.Text;
            if (!string.IsNullOrEmpty(province))
                province = regUperCase.Replace(province, "");
            string carnum = this.tbCarnum.Text;
            if (!string.IsNullOrEmpty(carnum))
                carnum = province + carnum;
            this.tag = basefun.setvaltag(this.tag, "{车牌号码}", carnum);
            //打印小票
            //编号
            string code = this.query.ExecuteScalar("打印编号序列号", new NameObjectList()).ToString();
            this.tag = basefun.setvaltag(this.tag, "{打印编号}", code);
            string printer = DataAccRes.AppSettings("收费打印机");
            try
            {
                if (!string.IsNullOrEmpty(printer))
                    this.printFee.PrinterSettings.PrinterName = printer;
                this.printFee.Print();
            }
            catch { }
            //删除设备停车记录,保存出入场记录
            CmdProtocol cmdP = new CmdProtocol(false);
            cmdP.setCommand("停车场", "删除一条停车记录", this.tag);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
            {
                MessageBox.Show("请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btFree_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
        /// <summary>
        /// 打印收取押金
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDeposit_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (null == sender || null == e || null == e.Graphics || null == e.PageSettings)
                return;
            NameObjectList ps = ParamManager.createParam(this.tag);
            if (null == ps["车牌号码"])
                ps["车牌号码"] = "";
            string[] info ={ 
                "南方停车场",
                "hr",
                "卡号：{卡号}",
                "姓名：{姓名}",
                "车号：{车牌号码}",
                "进场时间：{入场时间}",
                "临时出场：{出场时间}",
                "押金到期：{到期时间}",
                "押金金额：{押金金额}",
                "hr"
            };
            Font font = new Font(new FontFamily("黑体"), 12);
            Brush brush = System.Drawing.Brushes.Blue;
            int height = 0;
            e.PageSettings.PaperSize = new PaperSize("B5", 250, 200);
            e.PageSettings.Margins = new Margins(0, 0, 0, 0);
            for (int i = 0; i < info.Length; i++)
            {
                if ("hr" == info[i])
                {
                    height += 25;
                    e.Graphics.DrawLine(Pens.Blue, 0, height, 250, height);
                    height += -20;
                    continue;
                }
                string val=info[i];
                foreach (string key in ps.AllKeys)
                    val = val.Replace("{" + key + "}", Convert.ToString(ps[key]));
                height += 25;
                if (0 == i)
                    e.Graphics.DrawString(val, font, brush, 80, height);
                else
                    e.Graphics.DrawString(val, font, brush, 0, height);
            }
        }

        private void printFee_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (null == sender || null == e || null == e.Graphics || null == e.PageSettings)
                return;
            NameObjectList ps = ParamManager.createParam(this.tag);
            if (null==ps["车牌号码"])
                ps["车牌号码"] = "";
            string[] info ={ 
                "南方停车场",
                "hr",
                "卡号：{卡号}",
                "姓名：{姓名}",
                "编号：{打印编号}",
                "车号：{车牌号码}",
                "进场时间：{入场时间}",
                "出场时间：{出场时间}",
                "停车时长：{停车时长}",
                "金    额：{收费金额}",
                "hr"
            };
            Font font = new Font(new FontFamily("黑体"), 12);
            Brush brush = System.Drawing.Brushes.Blue;
            int height = 0;
            e.PageSettings.PaperSize = new PaperSize("B5", 250, 200);
            e.PageSettings.Margins = new Margins(0, 0, 0, 0);
            for (int i = 0; i < info.Length; i++)
            {
                if ("hr" == info[i])
                {
                    height += 25;
                    e.Graphics.DrawLine(Pens.Blue, 0, height, 250, height);
                    height += -20;
                    continue;
                }
                string val = info[i];
                foreach (string key in ps.AllKeys)
                    val = val.Replace("{" + key + "}", Convert.ToString(ps[key]));
                height += 25;
                if (0 == i)
                    e.Graphics.DrawString(val, font, brush, 80, height);
                else
                    e.Graphics.DrawString(val, font, brush, 0, height);
            }
        }
    }
}