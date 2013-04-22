using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using Granity.communications;
using Granity.CardOneCommi;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmParkStallSet : Form
    {
        string unitName = "停车场管理";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        /// <summary>
        public FrmParkStallSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParkStallSetFrm_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
            //对树的操作
            DataTable tab = this.ds.Tables["停车场管理"];
            this.bindMgr.BindTrv(this.trvParkStall, tab, "名称", "ID", "PID", "@ID={ID},@PID={PID},@编号={编号},@站址={站址}");
            this.trvParkStall.ExpandAll();
            tab.ColumnChanged += new DataColumnChangeEventHandler(tab_ColumnChanged);
            if (this.trvParkStall.Nodes.Count > 0)
                this.trvParkStall.SelectedNode = this.trvParkStall.Nodes[0];
            this.panGrpParam.Hide();
            this.panGrpFee.Dock = DockStyle.Fill;
            this.panGrpParam.Dock = DockStyle.Fill;
            //初始化收费参数编辑控件事件
            this.setTextChanged(this.tabFeeStd);
            this.setCheckedChanged(this.grpbAccept);
            this.setCheckedChanged(this.grpbCardType);
            this.setCheckedChanged(this.grpbChannel);
        }
        /// <summary>
        /// 数据改变时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tab_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable tab = sender as DataTable;
            if (null == tab || null == e || "名称" != e.Column.ColumnName)
                return;
            string id = Convert.ToString(e.Row["ID"]);
            if (this.trvParkStall.Nodes.Count < 1)
                return;
            TreeNode[] trns = this.trvParkStall.Nodes.Find(id, true);
            if (trns.Length > 0)
                trns[0].Text = Convert.ToString(e.ProposedValue);
        }

        /// <summary>
        /// 是否正在对复选框赋值,正在赋值则不改写DataTable字段数据
        /// </summary>
        bool isValSetting = false;

        #region 内部函数
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
            bool islocal = !isValSetting;
            isValSetting = true;
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
            if (islocal)
                isValSetting = false;
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
        /// 设置容器内复选框改变事件
        /// </summary>
        /// <param name="ct">容器</param>
        private void setCheckedChanged(Control ct)
        {
            foreach (Control c in ct.Controls)
            {
                if (c is CheckBox)
                    (c as CheckBox).CheckedChanged += new EventHandler(chkGrp_CheckedChanged);
                else if (c.Controls.Count > 0)
                    setCheckedChanged(c);
            }
        }
        /// <summary>
        /// 设置容器内编辑控件编辑值改变事件
        /// </summary>
        /// <param name="ct">容器</param>
        private void setTextChanged(Control ct)
        {
            foreach (Control c in ct.Controls)
            {
                if (c is TextBoxBase || c is DateTimePicker)
                    c.TextChanged += new EventHandler(txtFee_textChanged);
                else if (c.Controls.Count > 0)
                    setTextChanged(c);
            }
        }
        /// <summary>
        /// 设置容器内编辑控件编辑值改变事件
        /// </summary>
        /// <param name="ct">容器</param>
        private void setTextBlank(Control ct)
        {
            foreach (Control c in ct.Controls)
            {
                if (c is TextBoxBase || c is DateTimePicker)
                    c.Text = "";
                else if (c is CheckBox)
                    (c as CheckBox).Checked = false;
                else if (c.Controls.Count > 0)
                    setTextBlank(c);
            }
        }
        /// <summary>
        /// 设置收费标准,依据tag标记的pm和num值规则
        /// </summary>
        /// <param name="parkid">场ID</param>
        /// <param name="cartype">车型代码</param>
        /// <param name="tabdetail">收费标准明细</param>
        /// <param name="tabpg">标签页</param>
        private void setFeeParam(DataTable tabdetail, Control ct)
        {
            if (null == tabdetail || null == ct)
                return;
            DataView dvfee = this.ds.Tables["收费标准"].DefaultView;
            DataView dvdetail = tabdetail.DefaultView;
            if (dvfee.Count < 1 || dvdetail.Count < 1)
                return;
            //依据tag标记pm值(字段名称)和num值(序号值)读取映射值
            DataRowView drFee = dvfee[0];
            DataRowView drdetail = dvdetail[0];
            foreach (Control c in ct.Controls)
            {
                if (!(c is TextBoxBase || c is DateTimePicker))
                {
                    if (c.Controls.Count > 0)
                        setFeeParam(tabdetail, c);
                    continue;
                }
                //计费标准参数值
                string tag = Convert.ToString(c.Tag);
                string col = basefun.valtag(tag, "pm");
                string num = basefun.valtag(tag, "num");
                DataColumn dbcol = null;
                DataRowView drcol = null;
                //计费标准明细参数值,无num序号则读取第一行字段值
                if (string.IsNullOrEmpty(num))
                {
                    if (dvfee.Table.Columns.Contains(col))
                    {
                        dbcol = dvfee.Table.Columns[col];
                        drcol = drFee;
                    }
                    else if (tabdetail.Columns.Contains(col))
                    {
                        dbcol = tabdetail.Columns[col];
                        drcol = drdetail;
                    }
                }
                else if (tabdetail.Columns.Contains(col))
                {
                    dbcol = tabdetail.Columns[col];
                    foreach (DataRowView dr in dvdetail)
                    {
                        if (num != Convert.ToString(dr["序号"]))
                            continue;
                        drcol = dr;
                        break;
                    }
                }
                if (null == dbcol || null == drcol)
                    continue;
                if (typeof(DateTime) == dbcol.DataType && DBNull.Value != drcol[col])
                    c.Text = Convert.ToDateTime(drcol[col]).ToString("HH:mm");
                else
                    c.Text = Convert.ToString(drcol[col]);
            }
        }
        /// <summary>
        /// 获取设置参数，更新DataTable数据,依据Textbox的tag规则更新
        /// </summary>
        /// <param name="parkid">停车场ID</param>
        /// <param name="cartype">车型</param>
        /// <param name="feetype">消费方式类型</param>
        /// <param name="txt">编辑数值的控件</param>
        /// <param name="tabfee">收费标准数据</param>
        /// <param name="tabdetail">时段或时刻明细</param>
        private void getFeeParam(Control txt, DataTable tabfee, DataTable tabdetail)
        {
            if (null == txt || null == tabfee || null == tabdetail)
                return;
            DataView dvfee = tabfee.DefaultView;
            DataView dvdetail = tabdetail.DefaultView;
            if (dvfee.Count < 1 || dvdetail.Count < 1)
                return;

            DataRowView drfee = dvfee[0];
            DataRowView drdetail = dvdetail[0];
            string tag = Convert.ToString(txt.Tag);
            string col = basefun.valtag(tag, "pm");
            string num = basefun.valtag(tag, "num");

            //序号为空,直接对第一行赋值
            DataColumn dbcol = null;
            DataRow drcol = null;
            if (string.IsNullOrEmpty(num))
            {
                if (tabfee.Columns.Contains(col))
                {
                    dbcol = tabfee.Columns[col];
                    drcol = drfee.Row;
                }
                else if (tabdetail.Columns.Contains(col))
                {
                    dbcol = tabdetail.Columns[col];
                    drcol = drdetail.Row;
                }
            }
            else
            {
                //设置明细值,没有明细行则增加记录
                if (!tabdetail.Columns.Contains(col))
                    return;
                dbcol = tabdetail.Columns[col];
                foreach (DataRowView dr in dvdetail)
                {
                    if (num != Convert.ToString(dr["序号"]))
                        continue;
                    drcol = dr.Row;
                    break;
                }
                if (null == drcol)
                {
                    drcol = tabdetail.NewRow();
                    drcol["ID"] = Guid.NewGuid().ToString();
                    drcol["标准ID"] = drdetail["标准ID"];
                    drcol["序号"] = Convert.ToInt32(num);
                    tabdetail.Rows.Add(drcol);
                }
            }
            if (null == dbcol) return;
            if (string.IsNullOrEmpty(txt.Text))
                drcol[col] = DBNull.Value;
            else if (typeof(DateTime) == dbcol.DataType)
            {
                string val = txt.Text.Trim();
                if (val.StartsWith(":") || val.EndsWith(":"))
                    drcol[col] = DBNull.Value;
                else
                    drcol[col] = Convert.ToDateTime("1900-01-01 " + txt.Text + ":00");
            }
            else if (typeof(int) == dbcol.DataType)
                drcol[col] = Convert.ToInt32(txt.Text);
            else
                drcol[col] = txt.Text;
        }
        /// <summary>
        /// 检查收费标准明细,1:删除序号中断的明细,2:删除时刻或时段不用的明细
        /// </summary>
        private void validateFeeTab()
        {
            DataTable tabfee = this.ds.Tables["收费标准"];
            DataTable tabdspan = this.ds.Tables["收费标准时段"];
            DataTable tabdtime = this.ds.Tables["收费标准时刻"];
            foreach (DataRow dr in tabfee.Rows)
            {
                //删除不需要的时段明细
                if (DataRowState.Deleted == dr.RowState)
                    continue;
                DataRow[] drsdetail = tabdspan.Select(string.Format("标准ID='{0}'", dr["ID"]), "序号");
                string feetype = Convert.ToString(dr["计费方式"]);
                int num = 1;
                if ("消费方式4" == feetype || "消费方式5" == feetype)
                    foreach (DataRow d in drsdetail)
                        d.Delete();
                if ("消费方式3" == feetype)
                    foreach (DataRow d in drsdetail)
                    {
                        int sernum = Convert.ToInt32(d["序号"]);
                        string tslen = Convert.ToString(d["时长"]);
                        string money = Convert.ToString(d["金额"]);
                        if (num == sernum && !string.IsNullOrEmpty(tslen) && !string.IsNullOrEmpty(money))
                        {
                            num++;
                            continue;
                        }
                        d.Delete();
                    }
                //删除不需要的时刻明细
                drsdetail = tabdtime.Select(string.Format("标准ID='{0}'", dr["ID"]), "序号");
                num = 1;
                if ("消费方式4" != feetype && "消费方式5" != feetype)
                    foreach (DataRow d in drsdetail)
                        d.Delete();
                if ("消费方式4" == feetype)
                {
                    string dtbegin = "";
                    foreach (DataRow d in drsdetail)
                    {
                        int sernum = Convert.ToInt32(d["序号"]);
                        string dtend = Convert.ToString(d["截止"]);
                        string money = Convert.ToString(d["金额"]);
                        if (num == sernum && !string.IsNullOrEmpty(dtend) && !string.IsNullOrEmpty(money))
                        {
                            if (string.IsNullOrEmpty(dtbegin))
                                d["起始"] = DBNull.Value;
                            else
                                d["起始"] = Convert.ToDateTime(dtbegin);
                            dtbegin = dtend;
                            num++;
                            continue;
                        }
                        d.Delete();
                    }
                }
            }//foreach (DataRow dr in tabfee.Rows)
        }

        #endregion

        /// <summary>
        /// 增加车厂
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAddPark_Click(object sender, EventArgs e)
        {
            DataTable tab = this.ds.Tables["停车场管理"];
            //增加大场
            DataRow drnew = tab.NewRow();
            string code = BindManager.getCodeSn("Park");
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["名称"] = "新增大场" + code;
            drnew["类型"] = "大场";
            drnew["编号"] = code;
            tab.Rows.Add(drnew);
            TreeNode trn = BindManager.SetTreeNode(this.trvParkStall, drnew, "名称", "ID", "PID", "@ID={ID},@PID={PID},@编号={编号}");
            this.trvParkStall.SelectedNode = trn;
        }

        /// <summary>
        /// 增加内场
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAddInside_Click(object sender, EventArgs e)
        {
            if (null == this.trvParkStall.SelectedNode)
                return;
            TreeNode trnsel = this.trvParkStall.SelectedNode;

            if (TypeCombo.Text == "门岗")
            {
                MessageBox.Show("请在根节点上添加场内场!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            if ("场内场" == TypeCombo.Text)
                pid = basefun.valtag(Convert.ToString(trnsel.Parent.Tag), "ID");
            DataTable tab = this.ds.Tables["停车场管理"];
            //增加内场
            DataRow drnew = tab.NewRow();
            string code = BindManager.getCodeSn("Park");
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["PID"] = pid;
            drnew["名称"] = "新增场内场" + code;
            drnew["类型"] = "场内场";
            drnew["编号"] = code;
            tab.Rows.Add(drnew);
            TreeNode trn = BindManager.SetTreeNode(this.trvParkStall, drnew, "名称", "ID", "PID", "@ID={ID},@PID={PID},@编号={编号}");
            this.trvParkStall.SelectedNode = trn;
        }
        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAddDevice_Click(object sender, EventArgs e)
        {
            if (null == this.trvParkStall.SelectedNode)
                return;
            TreeNode trnsel = this.trvParkStall.SelectedNode;
            string pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            if ("门岗" == TypeCombo.Text)
                try
                {
                    pid = basefun.valtag(Convert.ToString(trnsel.Parent.Tag), "ID");
                }
                catch
                {
                    MessageBox.Show("节点选择错误!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            DataTable tab = this.ds.Tables["停车场管理"];
            //增加内场
            DataRow drnew = tab.NewRow();
            string code = BindManager.getCodeSn("Dr");
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["PID"] = pid;
            drnew["名称"] = "新增设备" + code;
            drnew["类型"] = "门岗";
            drnew["编号"] = code;
            tab.Rows.Add(drnew);
            TreeNode trn = BindManager.SetTreeNode(this.trvParkStall, drnew, "名称", "ID", "PID", "@ID={ID},@PID={PID},@编号={编号}");
            this.trvParkStall.SelectedNode = trn;
        }
        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbDelete_Click(object sender, EventArgs e)
        {
            TreeNode trn = this.trvParkStall.SelectedNode;
            if (null == trn)
                return;
            string tip = "确定是否删除当前节点";
            if (trn.Nodes.Count > 0)
                tip += ",同时将删除该节点的下级关联节点！";
            DialogResult msgquet = MessageBox.Show(tip, "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            //递归删除子级节点
            TreeNode trncurrent = trn;
            TreeNode trnnext = trn.PrevNode;
            DataTable tab = this.ds.Tables["停车场管理"];
            DataTable tabfee = this.ds.Tables["收费标准"];
            DataTable tabdtime = this.ds.Tables["收费标准时刻"];
            DataTable tabdspan = this.ds.Tables["收费标准时段"];
            string strfind = "ID='{0}'";
            if (trn.Nodes.Count > 0)
                trn = trn.Nodes[trn.Nodes.Count - 1];
            this.trvParkStall.SelectedNode = null;
            while (null != trn)
            {
                if (trn.Nodes.Count > 0)
                {
                    trn = trn.Nodes[trn.Nodes.Count - 1];
                    continue;
                }
                string id = Convert.ToString(trn.Tag);
                id = basefun.valtag(id, "ID");
                DataRow[] drs = tab.Select(string.Format(strfind, id));
                if (drs.Length > 0)
                {
                    //删除计费标准和停车场管理
                    DataRow[] drsfee = tabfee.Select(string.Format("场ID='{0}'", id));
                    foreach (DataRow drf in drsfee)
                    {
                        DataRow[] drsdetail = tabdspan.Select(string.Format("标准ID='{0}'", drf["ID"]));
                        foreach (DataRow d in drsdetail)
                            d.Delete();
                        drsdetail = tabdtime.Select(string.Format("标准ID='{0}'", drf["ID"]));
                        foreach (DataRow d in drsdetail)
                            d.Delete();
                        drf.Delete();
                    }
                    drs[0].Delete();
                }
                trnnext = trn.PrevNode;
                if (null == trnnext)
                    trnnext = trn.Parent;
                trn.Remove();
                if (trncurrent != trn)
                    trn = trnnext;
                else
                    trn = null;
            }
            this.trvParkStall.SelectedNode = trnnext;
        }
        /// <summary>-
        /// 保存设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSave_Click(object sender, EventArgs e)
        {
            this.validateFeeTab();
            DataTable tabpark = this.ds.Tables["停车场管理"];
            DataTable tabfee = this.ds.Tables["收费标准"];
            DataTable tabdspan = this.ds.Tables["收费标准时段"];
            DataTable tabdtime = this.ds.Tables["收费标准时刻"];
            DataTable[] tablist ={ tabpark, tabfee, tabdspan, tabdtime };
            bool isSuccess = this.Query.BeginTransaction();
            string msg = "";
            if (!isSuccess)
                msg = "保存失败，无法打开事务！";
            else
            {
                foreach (DataTable tab in tablist)
                {
                    NameObjectList[] psins = ParamManager.createParam(tab, DataRowState.Added);
                    NameObjectList[] psupt = ParamManager.createParam(tab, DataRowState.Modified);
                    NameObjectList[] psdel = ParamManager.createParam(tab, DataRowState.Deleted);
                    isSuccess = this.Query.ExecuteNonQuery(tab.TableName, psins, psupt, psdel);
                    if (!isSuccess)
                    {
                        msg = "保存失败，请检查数据是否合法！";
                        break;
                    }
                }
            }
            if (!isSuccess)
                this.Query.RollbackAndClose();
            else
            {
                this.Query.Commit();
                this.Query.Close();
                foreach (DataTable tab in tablist)
                    tab.AcceptChanges();
                //发出通讯更新设备
                int port = 2010;
                string sport = DataAccRes.AppSettings("Granity文件服务");
                if (!string.IsNullOrEmpty(sport))
                    try { port = Convert.ToInt32(sport); }
                    catch { return; }
                string conn = DataAccRes.DefaultDataConnInfo.Value;
                Regex regIP = new Regex(@"server=([\w.\(\)]*)(;|\\)");
                string ipsrv = "127.0.0.1";
                if (regIP.IsMatch(conn))
                {
                    Match mt = regIP.Match(conn);
                    if (mt.Groups.Count > 1)
                        ipsrv = mt.Groups[1].Value.ToLower();
                    if ("(local)" == ipsrv || "127.0.0.1" == ipsrv)
                        ipsrv = Dns.GetHostName();
                    ipsrv = Dns.GetHostAddresses(ipsrv)[0].ToString();
                }
                CmdFileTrans cmd = new CmdFileTrans(false);
                cmd.ExtRefreshDevice();
                CommiTarget target = new CommiTarget(ipsrv, port, CommiType.TCP);
                CommiManager.GlobalManager.SendCommand(target, cmd);
            }
            if (!isSuccess)
                MessageBox.Show(msg, "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 导航树节点改变联动当前数据表当前行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParkStallTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (null == e.Node)
                return;
            string tag = Convert.ToString(e.Node.Tag);
            string id = basefun.valtag(tag, "ID");
            DataTable tab = this.ds.Tables["停车场管理"];
            BindingManagerBase bindMgrbase = this.BindingContext[tab];
            if (null == tab || null == bindMgrbase)
                return;
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                if (DataRowState.Deleted == tab.Rows[i].RowState || Convert.ToString(tab.Rows[i]["ID"]) != id)
                    continue;
                bindMgrbase.Position = i;
                break;
            }
            if (bindMgrbase.Position < 0)
                return;
            DataRowView dr = bindMgrbase.Current as DataRowView;
            bool isGate = "门岗" == Convert.ToString(dr["类型"]);
            this.panGrpFee.Visible = !isGate;
            this.AllNoTxt.Visible = this.AllNoLab.Visible = this.RetainStaTxt.Visible = this.RetainStaLab.Visible = !isGate;
            this.panGrpParam.Visible = isGate;
            this.cbodevname.Visible = this.DevTypeCombo.Visible = this.label2.Visible = this.DevTypeLab.Visible = isGate;
        
            //停车场默认打开大车收费标准
            if (!isGate)
            {
                this.btnCar_Click(this.btnbigCar, null);
                return;
            }
            //  通道,卡类允许,放行控制 三项分组选配内容
            bool isChannelcard = true;
            if (DBNull.Value != dr["通道卡类别"])
                isChannelcard = Convert.ToBoolean(dr["通道卡类别"]);
            this.isValSetting = true;
            this.radChannelCar.Checked = !isChannelcard;
            this.radChannelCard.Checked = isChannelcard;
            this.isValSetting = false;
            this.setCheckedsgrp(this.plCardType, "通道内容", Convert.ToString(dr["通道内容"]));
            this.setCheckedsgrp(this.plCarType, "通道内容", Convert.ToString(dr["通道内容"]));
            this.setCheckedsgrp(this.grpbCardType, "卡类允许", Convert.ToString(dr["卡类允许"]));
            this.setCheckedsgrp(this.grpbAccept, "放行控制", Convert.ToString(dr["放行控制"]));
        }
        /// <summary>
        /// 分组内复选框的选择切换,对数据表字段赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkGrp_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            DataTable tab = this.ds.Tables["停车场管理"];
            BindingManagerBase bindMgrbase = this.BindingContext[tab];
            DataRowView dr = bindMgrbase.Current as DataRowView;
            if (isValSetting || null == dr || null == chk)
                return;

            string tag = Convert.ToString(chk.Tag);
            string grp = basefun.valtag(tag, "grp");
            if (string.IsNullOrEmpty(grp) || !tab.Columns.Contains(grp))
                return;
            string v = Convert.ToString(dr[grp]);
            string val = this.getCheckedsgrp(chk.Parent, grp);
            if (v == val)
                return;
            dr.Row[grp] = val;
        }
        /// <summary>
        /// 通道选择，显示不同面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radChannel_CheckedChanged(object sender, EventArgs e)
        {
            bool iscard = false;
            if (this.radChannelCard == sender)
                iscard = true;
            this.plCardType.Visible = iscard;
            this.plCarType.Visible = !iscard;
            if (this.isValSetting)
                return;
            //切换通道选择时清空选项
            this.isValSetting = true;
            this.setTextBlank(this.grpbChannel);
            DataTable tab = this.ds.Tables["停车场管理"];
            BindingManagerBase bindMgrbase = this.BindingContext[tab];
            DataRowView dr = bindMgrbase.Current as DataRowView;
            if (null == dr)
            {
                this.isValSetting = false;
                return;
            }
            dr.Row["通道卡类别"] = iscard;
            dr.Row["通道内容"] = "";
            this.isValSetting = false;
        }
        /// <summary>
        /// 设置不同车型的收费标准,按钮tag属性有val值标记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCar_Click(object sender, EventArgs e)
        {
            Button bt = sender as Button;
            if (null == bt) return;
            //车型按钮状态切换,模拟tab标签页方式
            Button[] bts ={ this.btnbigCar, this.btnMiCar, this.btnTimeCar, this.btnSmallCar, this.btnMoCar };
            foreach (Button b in bts)
            {
                if (bt == b)
                    b.FlatStyle = FlatStyle.Popup;
                else
                    b.FlatStyle = FlatStyle.Standard;
                b.Enabled = bt != b;
            }
            string cartype = basefun.valtag(Convert.ToString(bt.Tag), "val");
            string tag = Convert.ToString(this.grpFee.Tag);
            this.grpFee.Tag = basefun.setvaltag(tag, "车型", cartype);
            //清空收费标准
            this.isValSetting = true;
            this.setTextBlank(this.tabFeeStd);
            this.isValSetting = false;
            //查询收费标准,默认增加一条标准
            DataTable tab = this.ds.Tables["停车场管理"];
            BindingManagerBase bindMgrbase = this.BindingContext[tab];
            DataRowView drv = bindMgrbase.Current as DataRowView;
            if (null == drv || "门岗" == Convert.ToString(drv["类型"]))
                return;
            string parkid = Convert.ToString(drv["ID"]);
            DataTable tabFee = this.ds.Tables["收费标准"];
            string strfilter = string.Format("场ID='{0}'", parkid);
            DataRow[] drs = tabFee.Select(strfilter);
            if (drs.Length < 1)
            {
                NameObjectList ps = new NameObjectList();
                ps["场ID"] = parkid;
                this.Query.FillDataSet("收费标准", ps, ds);
                this.Query.FillDataSet("收费标准时段", ps, ds);
                this.Query.FillDataSet("收费标准时刻", ps, ds);
            }
            strfilter = string.Format("场ID='{0}' and 车型='{1}'", parkid, cartype);
            tabFee.DefaultView.RowFilter = strfilter;
            DataView dv = tabFee.DefaultView;
            DataRowView drvfee = dv.Count > 0 ? dv[0] : null;
            if (null == drvfee)
            {
                DataRow dr = tabFee.NewRow();
                dr["ID"] = Guid.NewGuid().ToString();
                dr["场ID"] = parkid;
                dr["车型"] = cartype;
                dr["计费方式"] = "消费方式1";
                tabFee.Rows.Add(dr);
                drvfee = dv[0];
            }
            //显示对应车型收费标准
            string feetype = Convert.ToString(drvfee["计费方式"]);
            int index = -1 + Convert.ToInt16(feetype.Replace("消费方式", ""));
            RadioButton[] rad ={ this.radfee1, this.radfee2, this.radfee3, this.radfee4, this.radfee5, this.radfee6 };
            if (index > rad.Length) index = 0;
            if (rad[index].Checked)
                this.radfee_CheckedChanged(rad[index], null);
            else
                rad[index].Checked = true;
        }
        /// <summary>
        /// 选择不同收费标准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radfee_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton bt = sender as RadioButton;
            if (null == bt||!bt.Checked)
                return;
            int index = Convert.ToInt16(basefun.valtag(Convert.ToString(bt.Tag), "val"));
            //查询收费标准,默认增加一条明细
            DataTable tabpark = this.ds.Tables["停车场管理"];
            DataView dvfee = this.ds.Tables["收费标准"].DefaultView;
            BindingManagerBase bindMgrbase = this.BindingContext[tabpark];
            DataRowView drpark = bindMgrbase.Current as DataRowView;
            if (null == drpark || "门岗" == Convert.ToString(drpark["类型"]) || dvfee.Count < 1)
                return;
            DataTable tabdetail = this.ds.Tables["收费标准时段"];
            if (4 == index || 5 == index)
                tabdetail = this.ds.Tables["收费标准时刻"];
            DataRowView drvFee = dvfee[0];
            string feetype = "消费方式" + Convert.ToString(index);
            if (feetype != Convert.ToString(drvFee["计费方式"]))
                drvFee["计费方式"] = feetype;
            tabdetail.DefaultView.RowFilter = string.Format("标准ID='{0}'", drvFee["ID"]);
            tabdetail.DefaultView.Sort = "序号";
            DataRowView drvdetail = null;
            if (tabdetail.DefaultView.Count > 0)
                drvdetail = tabdetail.DefaultView[0];
            if (null == drvdetail)
            {
                DataRow dr = tabdetail.NewRow();
                dr["ID"] = Guid.NewGuid().ToString();
                dr["标准ID"] = drvFee["ID"];
                dr["序号"] = 1;
                tabdetail.Rows.Add(dr);
            }
            //刷新编辑控件值
            this.tabFeeStd.SelectedIndex = -1 + index;
            this.isValSetting = true;
            this.setFeeParam(tabdetail, this.tabFeeStd.SelectedTab);
            this.isValSetting = false;
        }

        /// <summary>
        /// 收费参数改变时更新DataTable数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFee_textChanged(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (null == ctrl || this.isValSetting)
                return;

            string tag = Convert.ToString(ctrl.Tag);
            string col = basefun.valtag(tag, "pm");
            if (string.IsNullOrEmpty(col))
                return;
            //查询收费标准
            DataTable tabpark = this.ds.Tables["停车场管理"];
            DataView dvfee = this.ds.Tables["收费标准"].DefaultView;
            BindingManagerBase bindMgrbase = this.BindingContext[tabpark];
            DataRowView drpark = bindMgrbase.Current as DataRowView;
            if (null == drpark || "门岗" == Convert.ToString(drpark["类型"]) || dvfee.Count < 1)
                return;
            int index = 1 + this.tabFeeStd.SelectedIndex;
            DataTable tabdetail = this.ds.Tables["收费标准时段"];
            if (4 == index || 5 == index)
                tabdetail = this.ds.Tables["收费标准时刻"];
            //更新数据
            this.getFeeParam(ctrl, dvfee.Table, tabdetail);
        }
        
    }
}




