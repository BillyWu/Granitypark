using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmRight : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "停车场权限设置";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        /// <summary>
        /// 所有的设备编号
        /// </summary>
        string equipmentAll = string.Empty;
        /// <summary>
        /// 所有的设备id
        /// </summary>
        string equipmentId = string.Empty;
        /// <summary>
        /// 所有的卡编号
        /// </summary>
        string cardAll = string.Empty;
        /// <summary>
        /// 控制器
        /// </summary>
        string controlAll = string.Empty;
        public FrmRight()
        {
            InitializeComponent();
        }

        private void FrmRight_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据0
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tabEquipment = this.ds.Tables["停车场设备信息"];
            this.bindMgr.BindTrv(this.treEquipmentAll, tabEquipment, "名称", "id", "PID", "@ID={ID},@PID={PID},@编号={编号}");
            this.treEquipmentAll.ExpandAll();
            DataTable tabDept = this.ds.Tables["停车场卡"];
            this.bindMgr.BindTrv(this.treDept, tabDept, "名称", "id", "PID", "@ID={ID},@PID={PID},@用户编号={用户编号},@卡号={卡号}");
            this.treDept.ExpandAll();
            this.treDept.AfterCheckNode += new NodeEventHandler(treDept_AfterCheckNode);
            this.treDept.FocusedNodeChanged += new FocusedNodeChangedEventHandler(treDept_FocusedNodeChanged);
            this.treDept.FocusedNode = this.treDept.Nodes.ParentNode;
            this.treEquipmentAll.AfterCheckNode += new NodeEventHandler(treEquipmentAll_AfterCheckNode);
        }

        void treEquipmentAll_AfterCheckNode(object sender, NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        /// <summary>
        /// 门卡任何一扇门都打不开时，就把树节点复选框的值置为FALSE
        /// </summary>
        private void NotRight(TreeList tre)
        {
            foreach (TreeListNode node in tre.Nodes)
            {
                Uncheck(node);
            }
        }

        /// <summary>
        /// 树节点复选框全部置为FALSE
        /// </summary>
        /// <param name="node"></param>
        private void Uncheck(TreeListNode node)
        {
            node.CheckState = CheckState.Unchecked;
            foreach (TreeListNode door in node.Nodes)
            {
                Uncheck(door);
            }
        }

        /// <summary>
        /// 选择停车场卡能打开的设备，如果设备可以打开，树节点复选框的值置为true 否则置为false
        /// </summary>
        /// <param name="Door">用户选择卡节点，卡能打开的停车场设备的集合</param>
        /// <param name="doorTab">所有的停车场设备</param>
        private void GetRight(string[] equipment, DataTable equipmentTab)
        {
            foreach (TreeListNode node in this.treEquipmentAll.Nodes)
            {
                GetRightCheck(node, equipment, equipmentTab);
            }
        }

        /// <summary>
        /// 选择停车场卡能打开的设备，如果设备可以打开，树节点复选框的值置为true 否则置为false
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="Door">用户选择卡节点，卡能打开的停车场设备的集合</param>
        /// <param name="doorTab">所有的停车场设备</param>
        private void GetRightCheck(TreeListNode node, string[] equipment, DataTable equipmentTab)
        {
            foreach (string s in equipment)
            {
                if (equipmentTab.Select("设备编号='" + s + "'").Length == 0)
                {
                    continue;
                }
                string equipmentTag = equipmentTab.Select("设备编号='" + s + "'")[0]["id"].ToString();
                string tag = basefun.valtag(Convert.ToString(node.Tag), "ID");
                if (tag == equipmentTag)
                {
                    node.Checked = true;
                    break;
                }
            }
            foreach (TreeListNode Dnode in node.Nodes)
            {
                GetRightCheck(Dnode, equipment, equipmentTab);
            }
        }

        void treDept_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            string userNo = string.Empty;
            if (e.Node == null)
                return;
            //绑定门之前，先把树所有复选框置为false;
            NotRight(this.treEquipmentAll);
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            if (e.Node.HasChildren)
            {
                foreach (DataRow dr in this.ds.Tables["停车场卡"].Select("pid='" + tag + "'"))
                {
                    if (userNo == string.Empty)
                    {
                        userNo = dr["用户编号"].ToString();
                    }
                    else
                    {
                        userNo += "," + dr["用户编号"].ToString();
                    }
                }
            }
            else
            {
                userNo = basefun.valtag(Convert.ToString(e.Node.Tag), "用户编号");
            }
            string[] checkUserNo = userNo.Split(',');
            foreach (string s in checkUserNo)
            {
                if (this.ds.Tables["卡权限设置"].Select("用户编号='" + s + "'").Length > 0)
                {
                    DataRow dr = this.ds.Tables["卡权限设置"].Select("用户编号='" + s + "'")[0];
                    string[] Right = dr["权限"].ToString().Split(',');
                    DataTable equipmentTab = this.ds.Tables["门岗设备"];
                    GetRight(Right, equipmentTab);
                }
            }
            this.treEquipmentAll.ExpandAll();
        }

        void treDept_AfterCheckNode(object sender, NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 遍历子节点，修改复选框的值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// 遍历父节点，修改复选框的值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        /// <summary>
        /// 保存卡权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSave_Click(object sender, EventArgs e)
        {
            cardAll = string.Empty;
            equipmentAll = string.Empty;
            equipmentId = string.Empty;
            #region 遍历选择的卡
            foreach (TreeListNode Card in this.treDept.Nodes)
            {
                checkCard(Card);
            }
            #endregion
            #region 遍历选择的设备
            foreach (TreeListNode Equipment in this.treEquipmentAll.Nodes)
            {
                checkEquipment(Equipment);
            }
            #endregion
            if (cardAll == string.Empty)
            {
                XtraMessageBox.Show("请选择部门或用户", "系统提示！");
                return;
            }
            //停车场卡权限
            string[] checkcardAll = cardAll.Split(',');
            string[] door = equipmentAll.Split(',');
            foreach (string c in checkcardAll)
            {
                foreach (DataRow dr in this.ds.Tables["卡权限设置"].Select("卡号 ='" + c + "'"))
                {
                    dr.BeginEdit();
                    dr["权限"] = equipmentAll;
                    dr.EndEdit();
                }
            }
            WhiteList(checkcardAll);
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
            {
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 添加白名单
        /// </summary>
        /// <param name="checkcardAll"></param>
        private void WhiteList(string[] checkcardAll)
        {
            //添加门禁的白名单记录
            DataTable tablist = this.ds.Tables["白名单"];
            foreach (string cardNo in checkcardAll)
            {
                DataRow[] drv = this.ds.Tables["卡权限设置"].Select(" 卡号 ='" + cardNo + "'");
                if (drv.Length == 0)      continue;
                string rightstate = Convert.ToString(drv[0]["状态"]);
                string devices = equipmentId;
                //对已经授权的设备进行权限更改
                DataRow[] drs = tablist.Select(string.Format("卡号='{0}'", cardNo));
                foreach (DataRow dr in drs)
                {
                    string dev = Convert.ToString(dr["设备"]);
                    dr["状态"] = devices.Contains(dev) ? rightstate : "清";
                    devices = devices.Replace(dev, "");
                }
                //本次新增加权限的设备
                foreach (string ctrl in devices.Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries))
                {
                    DataRow dr = tablist.NewRow();
                    dr["卡号"] = cardNo;
                    dr["设备"] = ctrl;
                    dr["状态"] = rightstate;
                    dr["业务类别"] = "停车场";
                    tablist.Rows.Add(dr);
                }
            }
        }

        /// <summary>
        /// 帅选勾选的用户编号
        /// </summary>
        /// <param name="node"></param>
        private void checkCard(TreeListNode node)
        {
            if (!this.ds.Tables["卡权限设置"].Columns.Contains("卡号"))
                return;
            string cardNo = basefun.valtag(Convert.ToString(node.Tag), "卡号");
            if (cardNo != string.Empty)
            {
                if (node.Checked == true && this.ds.Tables["卡权限设置"].Select("卡号='" + cardNo + "'").Length > 0)
                {
                    if (cardAll == string.Empty)
                    {
                        cardAll = basefun.valtag(Convert.ToString(node.Tag), "卡号");
                    }
                    else
                    {
                        cardAll += String.Format(",{0}", basefun.valtag(Convert.ToString(node.Tag), "卡号"));
                    }
                }
            }
            foreach (TreeListNode Card in node.Nodes)
            {
                checkCard(Card);
            }
        }

        /// <summary>
        /// 帅选勾选的设备信息
        /// </summary>
        /// <param name="node"></param>
        private void checkEquipment(TreeListNode node)
        {
            string tag = basefun.valtag(Convert.ToString(node.Tag), "ID");
            if (node.Checked == true && this.ds.Tables["门岗设备"].Select("id='" + tag + "'").Length > 0)
            {
                DataRow dr = this.ds.Tables["门岗设备"].Select("id='" + tag + "'")[0];
                if (equipmentAll == string.Empty)
                {
                    equipmentAll = dr["设备编号"].ToString();
                    equipmentId = dr["id"].ToString();
                }
                else
                {
                    equipmentAll += String.Format(",{0}", dr["设备编号"].ToString());
                    equipmentId += String.Format(",{0}", dr["id"].ToString());
                }
            }
            foreach (TreeListNode Door in node.Nodes)
            {
                checkEquipment(Door);
            }
        }
    }
}