using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using System.Data.SqlClient;
using System.Collections;
using System.Xml;

namespace Granity.granityMgr.BaseDicinfo
{
    /// <summary>
    /// 设备信息维护
    /// </summary>
    public partial class FrmDoorControlBaseinfo : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门禁分组管理";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        public FrmDoorControlBaseinfo()
        {
            InitializeComponent();
        }

        private void Uc_DoorControlBaseinfo_Load(object sender, EventArgs e)
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
        //    this.bindMgr.BindFld(this, ds, true);
            //对树的操作
            DataTable tab = this.ds.Tables["门禁管理"];
            this.bindMgr.BindTrv(this.TreeGroup, tab, "名称", "id", "PID", "@ID={ID},@PID={PID},@编号={编号},@站址={站址}");
            this.TreeGroup.ExpandAll();
            this.txtDoorName.EditValueChanged += new EventHandler(txtEditValueChanged);
            this.txtGroupName.EditValueChanged += new EventHandler(txtEditValueChanged);
            this.txtControlDor.EditValueChanged += new EventHandler(txtEditValueChanged);
            BandCheckChangeEventargs(this.xtraTabExtendPara);
            GetEffTime();
        }

        /// <summary>
        /// 递归寻找CheckEdit 控件
        /// </summary>
        /// <param name="c"></param>
        private void BandCheckChangeEventargs(Control c)
        {
            if (c is DevExpress.XtraEditors.CheckEdit)
            {
                CheckEdit ck = c as CheckEdit;
                ck.Leave += new EventHandler(ck_Leave);
                return;
            }
            foreach (Control cl in c.Controls)
            {
                BandCheckChangeEventargs(cl);
            }
        }

        /// <summary>
        /// checkedit 焦点离开后触发，修改相关值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ck_Leave(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeGroup.FocusedNode;
            if (trnsel == null)
                return;
            string tag = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            CheckEdit ck = sender as CheckEdit;
            foreach (DataRow dr in this.ds.Tables["扩展板参数"].Select("id='" + ck.Parent.Tag.ToString() + "'"))
            {
                foreach (CheckEdit c in ck.Parent.Controls)
                {
                    dr.BeginEdit();
                    dr["编号"] = c.Parent.Parent.Tag.ToString();
                    switch (c.Text.Trim())
                    {
                        case "胁迫报警":
                            dr["胁迫报警"] = c.Checked;
                            break;
                        case "超时开门告警":
                            dr["超时开门告警"] = c.Checked;
                            break;
                        case "非法开门报警":
                            dr["非法开门报警"] = c.Checked;
                            break;
                        case "非法刷卡告警":
                            dr["非法刷卡告警"] = c.Checked;
                            break;
                        case "火警告警":
                            dr["火警告警"] = c.Checked;
                            break;
                        case "联动告警":
                            dr["联动告警"] = c.Checked;
                            break;
                        case "一号门":
                            dr["一号门"] = c.Checked;
                            break;
                        case "二号门":
                            dr["二号门"] = c.Checked;
                            break;
                        case "三号门":
                            dr["三号门"] = c.Checked;
                            break;
                        case "四号门":
                            dr["四号门"] = c.Checked;
                            break;
                        default:
                            break;
                    }
                    dr.EndEdit();
                }
            }
        }

        /// <summary>
        /// 增加门禁分组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDoorGroupAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.ds.Tables["门禁分组"];
            //增加门禁组
            DataRow drnew = tab.NewRow();
            string code = BindManager.getCodeSn("Grp");
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["名称"] = "门组" + code;
            drnew["类别"] = "门组";
            drnew["编号"] = code;
            drnew["控制参数id"] = Guid.NewGuid().ToString();
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.TreeGroup.AppendNode(new object[] { drnew["名称"].ToString(), drnew["ID"].ToString() }, null);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@编号=" + drnew["名称"].ToString() + "";
            node.ImageIndex = 1;
            node.SelectImageIndex = 4;
            this.TreeGroup.FocusedNode = node;
        }

        /// <summary>
        /// 增加门禁控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDoorControlAdd_Click(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeGroup.FocusedNode;
            if (trnsel == null)
                return;
            string pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            DataRow[] drcheck = this.ds.Tables["门禁分组"].Select("id='"+pid+"'");
            if (drcheck.Length == 0)
            {
                XtraMessageBox.Show("请在门禁组下面添加门禁","系统提示！");
                return;
            }

            DataTable tab = this.ds.Tables["门禁"];
            //增加门禁
            DataRow drnew = tab.NewRow();
            string code = BindManager.getCodeSn("门禁");
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["分组id"] = pid;
            drnew["名称"] = "门禁" + code;
            drnew["通讯协议"] = "门禁";
            tab.Rows.Add(drnew);

            TreeListNode node;
            node = this.TreeGroup.AppendNode(new object[] { drnew["名称"].ToString(), drnew["ID"].ToString() }, this.TreeGroup.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@编号=" + drnew["名称"].ToString() + "";
            node.ImageIndex = 2;
            node.SelectImageIndex = 2;
            this.TreeGroup.FocusedNode = node;
        }

        /// <summary>
        /// 判断控制的类型，现在读卡器的数量
        /// </summary>
        /// <param name="trnsel">树</param>
        /// <param name="Type">控制器类型</param>
        /// <returns>bool</returns>
        private bool ReadCardCout(TreeListNode trnsel, string Type)
        {
            if (trnsel == null || Type == string.Empty)
                return false;
            switch ((ControlType)Enum.Parse(typeof(ControlType), Type))
            {
                case ControlType.单门双向:
                    if (trnsel.Nodes.Count >= 1)
                    {
                        XtraMessageBox.Show(ControlType.单门双向.ToString() + "控制器只能添加1扇门", "系统提示！");
                        return false;
                    }
                    break;
                case ControlType.双门双向:
                    if (trnsel.Nodes.Count >= 2)
                    {
                        XtraMessageBox.Show(ControlType.双门双向.ToString() + "控制器只能添加2扇门", "系统提示！");
                        return false;
                    }
                    break;
                case ControlType.四门单向:
                    if (trnsel.Nodes.Count >= 4)
                    {
                        XtraMessageBox.Show(ControlType.四门单向.ToString() + "控制器只能添加4扇门", "系统提示！");
                        return false;
                    }
                    break;
                case ControlType.考勤机:
                    if (trnsel.Nodes.Count >= 1)
                    {
                        XtraMessageBox.Show(ControlType.考勤机.ToString() + "控制器只能添加1扇门", "系统提示！");
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// 增加门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDoorAdd_Click(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeGroup.FocusedNode;
            if (trnsel == null)
                return;
            string id = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            DataTable dtcontrol = this.ds.Tables["门禁"];
            DataRow[] drcheck = dtcontrol.Select("id='" + id + "'");
            if (drcheck.Length == 0)
            {
                XtraMessageBox.Show("请在门禁下面添加门", "系统提示！");
                return;
            }
            //没有控制输入之前暂时这样控制，方便程序调试
            if (drcheck[0]["控制器类型"].ToString()==string.Empty)
            {
                XtraMessageBox.Show("门禁控制没有选择控制器类型,选择后才添加门", "系统提示！");
                return;
            }
            bool readCardCount = ReadCardCout(trnsel, drcheck[0]["控制器类型"].ToString());
            if (!readCardCount)
                return;
            int readCardNo = this.TreeGroup.FocusedNode.Nodes.Count;
            #region 注销 汪 功能更改 2011-3-24
            //if (this.ds.Tables["门"].Select("控制器='" + id + "'").Length > 0)
            //{
            //    readCardNo = this.ds.Tables["门"].Select("控制器='" + id + "'", " 读卡器号 desc ")[0]["读卡器号"].ToString();
              
            //}
            //if (readCardNo == string.Empty)
            //{
            //    readCardNo = "0";
            //}
            //else
            //{
            //    int count = Int32.Parse(readCardNo) + 1;
            //    readCardNo = count.ToString();
            //}
            #endregion
            DataTable tab = this.ds.Tables["门"];
            //增加门
            DataRow drnew = tab.NewRow();
            string code = BindManager.getCodeSn("Dor");
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["控制器"] = id;
            drnew["名称"] = "门" + code;
            drnew["读卡器号"] = readCardNo;
            drnew["门编号"] = code;
            tab.Rows.Add(drnew);

            TreeListNode node ;
            node = this.TreeGroup.AppendNode(new object[] { drnew["名称"].ToString(), drnew["ID"].ToString() }, this.TreeGroup.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@编号=" + drnew["名称"].ToString() + "";
            node.ImageIndex = 2;
            node.SelectImageIndex = 2;
            this.TreeGroup.FocusedNode = node;
        }

        /// <summary>
        /// 检查收费标准明细,1:删除序号中断的明细,2:删除时刻或时段不用的明细
        /// </summary>
        private void validateFeeTab()
        {
            DataTable tabfee = this.ds.Tables["门"];
            DataTable tabdspan = this.ds.Tables["门禁"];
            DataTable tabdtime = this.ds.Tables["门禁分组"];
            DataTable[] tablist ={ tabfee, tabdspan, tabdtime };
        }

        /// <summary>
        /// 保存增加门，增加门禁设备，增加门组信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.validateFeeTab();
                DataTable tabfee = this.ds.Tables["门"];
                DataTable tabdspan = this.ds.Tables["门禁"];
                DataTable tabdtime = this.ds.Tables["门禁分组"];
                DataTable tabExtend= this.ds.Tables["扩展板参数"];
                DataTable tabEff = this.ds.Tables["门禁有效时段"];
                DataTable tabTask = this.ds.Tables["定时任务"];
                DataTable[] tablist ={ tabfee, tabdspan, tabdtime, tabEff, tabExtend,tabTask };
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
                    System.Text.RegularExpressions.Regex regIP = new System.Text.RegularExpressions.Regex(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}");
                    string ipsrv = "127.0.0.1";
                    if (regIP.IsMatch(conn))
                        ipsrv = regIP.Match(conn).Value;
                    //CmdFileTrans cmd = new CmdFileTrans(false);
                    //cmd.ExtRefreshDevice();
                    //CommiTarget target = new CommiTarget(ipsrv, port, CommiType.TCP);
                    //CommiManager.GlobalManager.SendCommand(target, cmd);
                }
                if (!isSuccess)
                    XtraMessageBox.Show(msg, "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                XtraMessageBox.Show("保存失败", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Query.Close();
            }
        }

        /// <summary>
        /// 删除 节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDel_Click(object sender, EventArgs e)
        {
            TreeListNode  trn = this.TreeGroup.FocusedNode;
            if (null == trn)
                return;
            string tip = "确定是否删除当前节点";
            if (trn.Nodes.Count > 0)
                tip += ",同时将删除该节点的下级关联节点！";
            DialogResult msgquet = MessageBox.Show(tip, "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            TreeListNode  trncurrent = trn;
            DataTable tabGroup = this.ds.Tables["门禁分组"];
            DataTable tabDoor = this.ds.Tables["门"];
            DataTable tabDoorControl = this.ds.Tables["门禁"];
            string tag = basefun.valtag(Convert.ToString(trn.Tag), "ID");

            #region 删除子级节点 暂时没有采用递归的方式删除 先这样，有时间写成递归
            foreach (DataRow drGroup in tabGroup.Select("id='" + tag + "'"))
            {
                foreach (DataRow drdoorcontrol in tabDoorControl.Select("分组id='" + drGroup["id"].ToString() + "'"))
                {
                    foreach (DataRow drdoor in tabDoor.Select("控制器='" + drdoorcontrol["id"].ToString() + "'"))
                    {
                        if (this.TreeGroup.FindNodeByFieldValue("id",drdoor["id"].ToString())!=null)
                        {
                            TreeListNode tn1= this.TreeGroup.FindNodeByFieldValue("id",drdoor["id"].ToString());
                            this.TreeGroup.Nodes.Remove(tn1);
                        }
                        drdoor.Delete();
                    }
                    if (this.TreeGroup.FindNodeByFieldValue("id", drdoorcontrol["id"].ToString()) != null)
                        {
                            TreeListNode tn1 = this.TreeGroup.FindNodeByFieldValue("id", drdoorcontrol["id"].ToString());
                            this.TreeGroup.Nodes.Remove(tn1);
                        }
                    drdoorcontrol.Delete();
                }
                if (this.TreeGroup.FindNodeByFieldValue("id", drGroup["id"].ToString()) != null)
                {
                    TreeListNode tn1 = this.TreeGroup.FindNodeByFieldValue("id", drGroup["id"].ToString());
                    this.TreeGroup.Nodes.Remove(tn1);
                }
                drGroup.Delete();
            }

            foreach (DataRow drdoorcontrol in tabDoorControl.Select("id='" + tag + "'"))
            {
                foreach (DataRow drdoor in tabDoor.Select("控制器='" + drdoorcontrol["id"].ToString() + "'"))
                {
                    if (this.TreeGroup.FindNodeByFieldValue("id", drdoor["id"].ToString()) != null)
                    {
                        TreeListNode tn1 = this.TreeGroup.FindNodeByFieldValue("id", drdoor["id"].ToString());
                        this.TreeGroup.Nodes.Remove(tn1);
                    }
                    drdoor.Delete();
                }
                if (this.TreeGroup.FindNodeByFieldValue("id", drdoorcontrol["id"].ToString()) != null)
                {
                    TreeListNode tn1 = this.TreeGroup.FindNodeByFieldValue("id", drdoorcontrol["id"].ToString());
                    this.TreeGroup.Nodes.Remove(tn1);
                }
                drdoorcontrol.Delete();
            }

            foreach (DataRow drdoor in tabDoor.Select("id='" + tag + "'"))
            {
                if (this.TreeGroup.FindNodeByFieldValue("id", drdoor["id"].ToString()) != null)
                {
                    int  readCardNum =0;
                    TreeListNode tn1 = this.TreeGroup.FindNodeByFieldValue("id", drdoor["id"].ToString());
                    TreeListNode tn1Parenet= tn1.ParentNode;
                    string doorId = basefun.valtag(tn1.Tag.ToString(), "ID");
                    string controlId = basefun.valtag(tn1Parenet.Tag.ToString(), "ID");
                    if (tabDoor.Select("id='" + doorId + "'").Length > 0)
                    {
                        DataRow dr = tabDoor.Select("id='" + doorId + "'")[0];
                        readCardNum = int.Parse(dr["读卡器号"].ToString());
                    }
                    foreach (DataRow drCard in tabDoor.Select("控制器='" + controlId + "' and 读卡器号>" + readCardNum))
                    {
                        drCard.BeginEdit();
                        drCard["读卡器号"] = int.Parse(drCard["读卡器号"].ToString()) - 1;
                        drCard.EndEdit();
                    }
                    this.TreeGroup.Nodes.Remove(tn1);
                }
                drdoor.Delete();
            }
            #endregion
        }

        /// <summary>
        /// 点击树节点时触发
        /// </summary>
        /// <param name="tag">树的Tag值</param>
        private void GetExpend(string tag)
        {
            if (this.ds.Tables["扩展板参数"].Select("分组id='" + tag + "'").Length == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    DataRow dr = this.ds.Tables["扩展板参数"].NewRow();
                    dr["id"] = Guid.NewGuid().ToString();
                    dr["分组id"] = tag;
                    this.ds.Tables["扩展板参数"].Rows.Add(dr);
                }
            }

            //只有两层不需要用递归
            int index = -1;
            DataRow[] drpnl = this.ds.Tables["扩展板参数"].Select("分组id='" + tag + "'", "id desc");
            foreach (Control c in xtraTabExtendPara.TabPages)
            {
                index++;
                foreach (Control cp in c.Controls)
                {
                    DevExpress.XtraEditors.XtraPanel Pnl = cp as XtraPanel;
                    Pnl.Tag = drpnl[index]["id"].ToString();
                }
            }

            if (this.ds.Tables["扩展板参数"].Select("分组id='" + tag + "'").Length == 0)
            {
                BandCheck(this.xtraTabExtendPara);
            }
            else
            {
                foreach (DataRow dr in this.ds.Tables["扩展板参数"].Select("分组id='" + tag + "'"," id desc"))
                {
                    BandCheck(this.xtraTabExtendPara);
                }
            }
        }

        /// <summary>
        /// 递归寻找CheckEdit 并给控件赋值
        /// </summary>
        /// <param name="c"></param>
        private void BandCheck(Control c)
        {
            if (c is DevExpress.XtraEditors.CheckEdit)
            {
                DevExpress.XtraEditors.CheckEdit ck = c as CheckEdit;
                DataRow dr = this.ds.Tables["扩展板参数"].Select("id= '" + ck.Parent.Tag.ToString() + "'")[0];
                if (dr[ck.Text].ToString() == string.Empty || dr[ck.Text].ToString()=="0")
                {
                    ck.Checked = false;
                    return;
                }
                if (dr[ck.Text].ToString() == "1")
                {
                    ck.Checked = true;
                    return;
                }
                ck.Checked = bool.Parse(dr[ck.Text].ToString());
                return;
            }
            foreach (Control cl in c.Controls)
            {
                BandCheck(cl);
            }
        }

        /// <summary>
        /// 获取分组的有效时段
        /// </summary>
        /// <param name="tag"></param>
        private void GetGrpEffTime(string tag)
        {
            foreach (Control cl in this.xtraTabEffTime.Controls)
            {
                CheckEdit ck = cl as CheckEdit;
                if (ck == null)
                    continue;
                DataRow[] row = this.ds.Tables["门禁有效时段"].Select(string.Format("分组id='{0}' and 有效时段id='{1}'", tag, Convert.ToString(ck.Name)));
                if (row.Length>0)
                {
                    ck.Checked = true;
                }
                else
                {
                    ck.Checked = false;
                }
            }
        }

        /// <summary>
        /// 获取分组的定时任务
        /// </summary>
        private void GetTask(string tag)
        {
            if(string.IsNullOrEmpty(tag))
                return ;
            DataTable tab = this.ds.Tables["定时任务"] as DataTable;
            if (tab == null)
                return;
            DataTable temp = new DataTable();
            temp = tab.Clone();
            temp = FunShare.GetTable(temp, tab.Select(string.Format("分组ID='{0}'", tag)));
            this.gridTask.DataSource = temp;
        }

        /// <summary>
        /// 获取门禁分组的有效时段,并绘制控件位置
        /// </summary>
        /// <param name="Tag">门禁分组表id</param>
        private void GetEffTime()
        {
            int x = 5;
            int y = 5;
            foreach (DataRow dr in ds.Tables["有效时段"].Rows)
            {
                CheckEdit chk = new CheckEdit();
                chk.Name = Convert.ToString(dr["id"]);
                chk.Text = Convert.ToString(dr["名称"]);
                chk.Leave += new EventHandler(chk_Leave);
                chk.Location = new Point(x, y);
                x = x + 100;
                if (x >= this.xtraTabEffTime.Width - 100)
                {
                    x = 5;
                    y = y + 30;
                }
                this.xtraTabEffTime.Controls.Add(chk);
            }
        }

        /// <summary>
        /// 离开有效时段复选框，触发复选框点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chk_Leave(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeGroup.FocusedNode;
            if (trnsel == null)
                return;
            CheckEdit ck = (sender as CheckEdit);
            string tag = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            string id = Convert.ToString(ck.Name);
            DataRow[] row = ds.Tables["门禁有效时段"].Select(string.Format("分组id='{0}' and 有效时段id='{1}'", tag, id));
            if (ck.Checked == false)
            {
                if (row.Length == 1)
                {
                    ds.Tables["门禁有效时段"].Select(string.Format("分组id='{0}' and 有效时段id='{1}'", tag, id))[0].Delete();
                }
            }
            else
            {
                if (row.Length == 1)
                {
                    ds.Tables["门禁有效时段"].Select(string.Format("分组id='{0}' and 有效时段id='{1}'", tag, id))[0].Delete();
                }
                DataRow drAdd = ds.Tables["门禁有效时段"].NewRow();
                drAdd["id"] = Guid.NewGuid().ToString();
                drAdd["分组id"] = tag;
                drAdd["有效时段id"] = id;
                ds.Tables["门禁有效时段"].Rows.Add(drAdd);
            }
        }

        /// <summary>
        /// 根据树节点获取门禁的有效时段，扩展版
        /// </summary>
        /// <param name="tag"></param>
        private void GetGroupInfo(TreeListNode tn)
        {
            if (tn == null)
                return;
            int x = 5;
            int y = 5;
            string tag = basefun.valtag(Convert.ToString(tn.Tag), "ID");
            if (this.ds.Tables["门禁分组"].Select("id='" + tag + "'").Length > 0)
            {
                GetGrpEffTime(tag);
                GetTask(tag);
                GetExpend(tag);
            }
        }

       /// <summary>
        /// 根据树节点获取或设置Group,Pnl的显示或隐藏(暂时这样写，没想到封装的方法 ),通过控件的Name属性来控制是否显示
       /// </summary>
       /// <param name="ColName">查询表结构指定的列名</param>
       /// <param name="Tag">指定列的值</param>
       /// <param name="Ctl">控件容器</param>
       /// <param name="dt">查询用到的表</param>
        private void SetGroupVisble( DevExpress.XtraEditors.XtraPanel Pnl, DataTable dt)
        {
            switch (dt.TableName)
            {
                case "门禁分组":
                    foreach (Control cl in Pnl.Controls)
                    {
                        if (cl.Name == "GroupBaseInfo" || cl.Name == "GroupDoorGroupInfo" || cl.Name == "GroupExtendInfo"
                            || cl.Name == "panelEffTask" || cl.Name == "grpTimeTask")
                        {
                            cl.Visible = true;
                        }
                        else
                        {
                            cl.Visible = false;
                        }
                    }
                    break;
                case "门禁":
                    foreach (Control cl in Pnl.Controls)
                    {
                        if (cl.Name == "GroupControlParm")
                        {
                            cl.Visible = true;
                        }
                        else
                        {
                            cl.Visible = false;
                        }
                    }
                    break;
                case "门":
                    foreach (Control cl in Pnl.Controls)
                    {
                        if (cl.Name == "GroupDoorInfo")
                        {
                            cl.Visible = true;
                        }
                        else
                        {
                            cl.Visible = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            bool fag = FunShare.CheckDataState(this.ds);
            if (!fag)
            {
                this.Close();
            }
            else
            {
                DialogResult dr = XtraMessageBox.Show("您还有修改的数据未保存，确定退出吗？", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (DialogResult.OK == dr)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        ///  点击树节点触发节点事件，主要是在右边显示出节点的相关信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeGroup_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node == null||!Convert.ToString(e.Node.Tag).Contains(","))
            {
                return;
            }
            int rowIndex = 0;
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            string text = basefun.valtag(Convert.ToString(e.Node.Tag), "编号");
            foreach (DataTable dtTemp in ds.Tables)
            {
                rowIndex = 0;
                if (!dtTemp.Columns.Contains("id"))
                {
                    continue;
                }
                foreach (DataRow dr in dtTemp.Rows)
                {
                    rowIndex++;
                    if (DataRowState.Deleted != dr.RowState && dr["id"].ToString() == tag)
                    {
                        BindingManagerBase bindMgrbase = this.BindingContext[dtTemp];
                        if (bindMgrbase != null)
                        {
                            bindMgrbase.Position = rowIndex - 1;
                            GetGroupInfo(e.Node);
                            SetGroupVisble(PnlGroupInfo, dtTemp);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 修改树节点的text值
        /// </summary>
        /// <param name="trv">树</param>
        /// <param name="colName">树节点列名</param>
        /// <param name="colValue">列值</param>
        private void setTreeNodeText(TreeList trv,string colName,string colValue)
        {
            try
            {
                if (trv == null || colName == null || colValue == string.Empty)
                    return;
                if (trv.Nodes.Count == 0)
                    return;
                trv.FocusedNode.SetValue(trv.Columns[colName], colValue.Trim());
            }
            catch
            { }
        }

        /// <summary>
        /// 修改树节点text事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt= sender as DevExpress.XtraEditors.TextEdit;
            setTreeNodeText(this.TreeGroup, "名称", txt.Text.Trim());
        }

        /// <summary>
        /// 提示控制器下面添加的门数量不能超过门禁控制器类型允许的数量 备注
        /// 跟ReadCardCount()方法不同处在于是否等于控制器类型规定的门数量
        /// </summary>
        /// <param name="trnsel"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        private bool LimitReadCardCount(TreeListNode trnsel, string Type)
        {
            if (trnsel == null || Type == string.Empty)
                return false ;
            switch ((ControlType)Enum.Parse(typeof(ControlType), Type))
            {
                case ControlType.单门双向:
                    if (trnsel.Nodes.Count >1)
                    {
                        XtraMessageBox.Show(ControlType.单门双向.ToString() + "控制器只能添加1扇门", "系统提示！");
                        return false;
                    }
                    break;
                case ControlType.双门双向:
                    if (trnsel.Nodes.Count > 2)
                    {
                        XtraMessageBox.Show(ControlType.双门双向.ToString() + "控制器只能添加2扇门", "系统提示！");
                        return false;
                    }
                    break;
                case ControlType.四门单向:
                    if (trnsel.Nodes.Count > 4)
                    {
                        XtraMessageBox.Show(ControlType.四门单向.ToString() + "控制器只能添加4扇门", "系统提示！");
                        return false;
                    }
                    break;
                case ControlType.考勤机:
                    if (trnsel.Nodes.Count > 1)
                    {
                        XtraMessageBox.Show(ControlType.考勤机.ToString() + "控制器只能添加1扇门", "系统提示！");
                        return false;
                    }
                    break;
                default:
                    return false;
                    break;
            }
            return true;
        }
        /// <summary>
        /// 提示控制器下面添加的门数量不能超过门禁控制器类型允许的数量 备注
        /// 跟ReadCardCount()方法不同处在于是否等于控制器类型规定的门数量
        /// </summary>
        private void LimitDoorCount()
        {
            if (this.TreeGroup.FocusedNode == null || lookUpEdit7.EditValue == null)
                return;
            bool fag = LimitReadCardCount(this.TreeGroup.FocusedNode, lookUpEdit7.EditValue.ToString());
            if (!fag)
                this.lookUpEdit7.Focus();
        }

        private void lookUpEdit7_EditValueChanged(object sender, EventArgs e)
        {
            LimitDoorCount();
        }

        private void lookUpEdit7_Leave(object sender, EventArgs e)
        {
            LimitDoorCount();
        }

        /// <summary>
        /// 增加定时任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtTaskAdd_Click(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeGroup.FocusedNode;
            if (trnsel == null)
                return;
            string pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            int count = this.gridViewTask.RowCount;
            DataTable tab = this.gridTask.DataSource as DataTable;
            DataRow dr = tab.NewRow();
            dr["ID"] = Guid.NewGuid().ToString();
            dr["分组ID"] = pid;
            dr["索引号"] = count;
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
            this.gridTask.DataSource = tab;
        }

        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btTaskDel_Click(object sender, EventArgs e)
        {
            if (this.gridViewTask.RowCount == 0)
                return;
            DataTable tab = this.gridTask.DataSource as DataTable;
            if (tab == null || tab.Rows.Count == 0)
                return;
            System.Windows.Forms.DialogResult dr = XtraMessageBox.Show("您确定要删除当前选中的记录？", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel  == dr)
                return;
            DataRowView dv = this.BindingContext[tab].Current as DataRowView;
            dv.Row.Delete();
            this.gridTask.DataSource = tab;
        }

        private void gridViewTask_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
             string tag = basefun.valtag(Convert.ToString(this.TreeGroup.FocusedNode.Tag), "分组ID");
             EditTabColValue(this.ds.Tables["定时任务"], "id", e.Value.ToString(), tag);
             this.gridTask.DataSource = this.ds.Tables["定时任务"];
        }

        /// <summary>
        /// 根据记录Tag,修改虚表记录的值(传递的表里一定有id字段，否则该方法无效)
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="ColName">列名</param>
        /// <param name="ColValue">列值</param>
        /// <param name="Tag">记录id</param>
        private void EditTabColValue(DataTable dt, string ColName, string ColValue, string Tag)
        {
            if (!dt.Columns.Contains("id"))
                return;
            foreach (DataRow dr in dt.Select("id='" + Tag + "'"))
            {
                dr.BeginEdit();
                dr[ColName] = ColValue;
                dr.EndEdit();
            }
        }
    }
    public enum TreeNodeType
    {
        AllDoorGruopInfo,//门分组
        ControlDoor,//控制器，扩展信息，有效时间
        Door//门

    }
    public enum ControlType
    {
        单门双向,
        双门双向,
        四门单向,
        考勤机
    }
}

