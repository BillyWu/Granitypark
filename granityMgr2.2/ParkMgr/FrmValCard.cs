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
//using Granity.winTools;
using Granity.CardOneCommi;

namespace Granity.parkStation
{
    public partial class FrmValCard : Form
    {
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;


        //private CommiTarget target;
        private string deviceID;
        private CommiTarget target;
        /// <summary>
        /// 读取或设置通讯目标位置
        /// </summary>
        public CommiTarget Target
        {
            get { return target; }
            set { target = value; }

        }
        /// <summary>
        /// 读取或设置设备地址
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
        private QueryDataRes query;
        /// <summary>
        /// 读取或设置数据查询
        /// </summary>
        public QueryDataRes Query
        {
            get { return query; }
            set { query = value; }
        }

        BindManager bindMgr = null;

        public FrmValCard()
        {
            InitializeComponent();
        }
        private void FrmValCard_Load(object sender, EventArgs e)
        {
            bindMgr = new BindManager(this);
            this.refresh();
        }

        /// <summary>
        /// 刷新当前信息
        /// </summary>
        private void refresh()
        {
            if (string.IsNullOrEmpty(this.deviceID) || string.IsNullOrEmpty(this.deviceName) || null == this.query)
            {
                this.gdValList.Rows.Clear();
                return;
            }
            this.lbInfo.Text = this.deviceName + " (" + this.deviceID + ")";
            string sql = "EXECUTE dbo.park_未下载 '" + this.deviceID + "'";
            if (this.rdUndel.Checked)
                sql = "EXECUTE dbo.park_已更新 '" + this.deviceID + "'";
            if (this.rdUnupdate.Checked)
                sql = "EXECUTE dbo.park_未更新 '" + this.deviceID + "'";
            if (this.rdDowned.Checked)
                sql = "EXECUTE dbo.park_已下载 '" + this.deviceID + "'";

            //绑定数据集
            DataTable tab = this.query.GetDataTableBySql(sql);
            tab.TableName = "有效卡";
            ds = new DataSet();
            ds.Tables.Add(tab);
            bindMgr.BindFld(this, ds);
            //设定操作功能按钮 
            BtnDownAll.Enabled = false;
            BtnDownOne.Enabled = false;
            BtnReadOne.Enabled = false;
            BtnDelAll.Enabled = false;
            BtnDelOne.Enabled = false;
            if (this.rdUndel.Checked || this.rdDowned.Checked)
            {
                BtnReadOne.Enabled = true;
                BtnDelAll.Enabled = true;
                BtnDelOne.Enabled = true;
            }
            else
            {
                BtnDownAll.Enabled = true;
                BtnDownOne.Enabled = true;
            }
            //未更新功能时开放全部功能用户自己确定操作
            if (this.rdUnupdate.Checked)
            {
                BtnReadOne.Enabled = true;
                BtnDelAll.Enabled = true;
                BtnDelOne.Enabled = true;
            }

            //显示指定字段
            bindMgr.SetGridCols(this.gdValList, "姓名,卡号,卡类,车类,时段名称,状态,有效日期");
        }


        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 下载全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownAll_Click(object sender, EventArgs e)
        {
            DataTable tab = this.gdValList.DataSource as DataTable;
            if (tab.Rows.Count < 1)
                return;
            DataRow[] drs = new DataRow[tab.Rows.Count];
            for (int i = 0; i < tab.Rows.Count; i++)
                drs[i] = tab.Rows[i];
            string tpl = "停车场", cmd = "下载ID白名单";
            string[,] colmap ={ { "{卡号}", "卡片序列号" }, { "{卡类}", "卡型" }, { "{车型}", "车型" }, { "{固定车位}", "固定车位" }, { "{时段}", "时段1" },
                                { "{有效日期}", "有效日期" }};
            string msg = "";// CmdProtocol.ExecuteDataRows(drs, colmap, tpl, cmd, this.target, this.deviceID, this.query, "白名单_单条");
            this.refresh();
            if (string.IsNullOrEmpty(msg))
                MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(basefun.valtag(msg, "{状态}"), "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 下载单条白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownOne_Click(object sender, EventArgs e)
        {
            //将DataGridView的值赋给table
            DataTable tab = this.gdValList.DataSource as DataTable;
            if (tab.Rows.Count < 1)
                return;
            BindingManagerBase bindmgr = this.BindingContext[tab];
            //得到所有行的数据，赋值给DataRow
            DataRow[] drs = new DataRow[1] { tab.Rows[bindmgr.Position] };
            string tpl = "停车场", cmd = "下载ID白名单";
            string[,] colmap ={ { "{卡号}", "卡片序列号" }, { "{卡类}", "卡型" }, { "{车型}", "车型" }, { "{固定车位}", "固定车位" }, { "{时段}", "时段1" },
                                { "{有效日期}", "有效日期" }};
            //向下位机发送指令并操作数据库
            string msg = "";// CmdProtocol.ExecuteDataRows(drs, colmap, tpl, cmd, this.target, this.deviceID, this.query, "白名单_单条");
            this.refresh();
            if (string.IsNullOrEmpty(msg))
                MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(basefun.valtag(msg, "{状态}"), "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 读取单条白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReadOne_Click(object sender, EventArgs e)
        {
            DataTable tab = this.gdValList.DataSource as DataTable;
            if (tab.Rows.Count < 1)
                return;
            BindingManagerBase bindmgr = this.BindingContext[tab];
            DataRow dr = tab.Rows[bindmgr.Position];
            string cardnum = Convert.ToString(dr["卡号"]);
            if (string.IsNullOrEmpty(cardnum))
            {
                MessageBox.Show("请确定具体卡号！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string tpl = "停车场", cmd = "查询ID白名单";
            CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
            string tagdata = "@设备地址=" + this.deviceID;
            tagdata = basefun.setvaltag(tagdata, "{卡号}", cardnum);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(target, cmdP);
            this.refresh();
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                //cmdP.ResponseFormat
                MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// 删除全部白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelAll_Click(object sender, EventArgs e)
        {

            DataTable tab = this.gdValList.DataSource as DataTable;
            if (tab.Rows.Count < 1)
                return;
            BindingManagerBase bindmgr = this.BindingContext[tab];
            DataRow dr = tab.Rows[bindmgr.Position];
            string cardnum = Convert.ToString(dr["卡号"]);
            string id = Convert.ToString(dr["id"]);
            if (string.IsNullOrEmpty(cardnum))
            {
                MessageBox.Show("请确定具体卡号！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("请确定是否要删除全部白名单？【是】将删除全部白名单，【否】则放弃操作。", "删除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (DialogResult.Yes != result)
                return;
            string tpl = "停车场", cmd = "初始化ID白名单";
            CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
            string tagdata = "@设备地址=" + this.deviceID;
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                deleteAll(id);
                this.refresh();
                MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void deleteOne(string devid, string cardno)
        {
            NameObjectList ps = this.buildParam(this, null);
            ps["设备"] = devid;
            ps["卡号"] = cardno;
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "设备控制维护");
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            query.ExecuteDelete("删除白名单_单条", ps);
        }
        public void deleteAll(string devid)
        {
            NameObjectList ps = this.buildParam(this, null);
            ps["设备"] = devid;
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "设备控制维护");
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            query.ExecuteDelete("删除白名单_全部", ps);
        }
        /// <summary>
        /// 删除单条白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelOne_Click(object sender, EventArgs e)
        {
            DataTable tab = this.gdValList.DataSource as DataTable;
            if (tab.Rows.Count < 1)
                return;
            BindingManagerBase bindmgr = this.BindingContext[tab];
            DataRow dr = tab.Rows[bindmgr.Position];
            string cardnum = Convert.ToString(dr["卡号"]);
            string id = Convert.ToString(dr["id"]);
            if (string.IsNullOrEmpty(cardnum))
            {
                MessageBox.Show("请确定具体卡号！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show("请确定是否要删除卡号：" + cardnum, "删除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (DialogResult.Yes != result)
                return;
            string tpl = "停车场", cmd = "删除ID白名单";
            CmdProtocol cmdP = new CmdProtocol(this.deviceID, false);
            string tagdata = "@设备地址=" + this.deviceID;
            tagdata = basefun.setvaltag(tagdata, "{卡号}", cardnum);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {

                deleteOne(id, cardnum);
                this.refresh();
                MessageBox.Show("执行成功！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private NameObjectList buildParam(Control ct, NameObjectList ps)
        {
            if (null == ps)
                ps = new NameObjectList();
            string tag = Convert.ToString(ct.Tag);
            if (!string.IsNullOrEmpty(basefun.valtag(tag, "fld")))
            {
                ps[basefun.valtag(tag, "fld")] = ct.Text;
                return ps;
            }
            foreach (Control child in ct.Controls)
                this.buildParam(child, ps);
            return ps;
        }

        private void rbUndown_CheckedChanged(object sender, EventArgs e)
        {
            this.refresh();
        }

        private void rdUndel_CheckedChanged(object sender, EventArgs e)
        {
            this.refresh();
        }

        private void rdUnupdate_CheckedChanged(object sender, EventArgs e)
        {
            this.refresh();
        }

        private void rdDowned_CheckedChanged(object sender, EventArgs e)
        {
            this.refresh();
        }


    }
}