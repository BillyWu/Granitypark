using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
namespace Granity.granityMgr.BaseDicinfo
{
    /// <summary>
    /// 对于门权限设计的思想如下：用户点击部门时，需要把部门下面的员工所有的门累加起来，显示该部门所有员工的能打开的门；
    /// 点击单个用户时，只显示单个用户能打开的门
    /// （如 技术部门有001，002 员工 ，001能打开 1，2 门，002 能打开3，4门，点击技术部门，需要显示出1，2，3，4门;点击001时，显示1，2门）
    /// 1 绑定权限：（1）用户选择了门后，在保存时遍历用户选择了那些门，累加起来，保存到变量里
    ///           （2）写入数据库，一个用户可以对应多门，用户对应的门保存在表门字段权限里，以“，”为间隔
    /// （如 员工编号 001 选择了 1，2 门 001->1,2）
    /// 2 显示门：  （1）用户点击部门信息时，需统计部门下面的所有员工，然后把选择的部门下面的员工能打开的门累加
    /// （如 技术部门有001，002 员工 ，001能打开 1，2 门，002 能打开3，4门，点击技术部门，需要显示出1，2，3，4门;点击001时，显示1，2门）
    /// 3 具体实现过程略
    /// </summary>
    public partial class FrmRight : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门禁权限";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        /// <summary>
        /// 所有的门编号
        /// </summary>
        string DoorAll = string.Empty;
        /// <summary>
        /// 所有的卡编号
        /// </summary>
        string CardAll = string.Empty;
        /// <summary>
        /// 控制器读卡器号
        /// </summary>
        Dictionary<string, string> ctrlReadNo = new Dictionary<string, string>();
        /// <summary>
        /// 控制器
        /// </summary>
        string controlAll = string.Empty;
        public FrmRight()
        {
            InitializeComponent();
        }
 
        private void Frm_Right_Load(object sender, EventArgs e)
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
            DataTable tabDor = this.ds.Tables["门禁分组树"];
            this.bindMgr.BindTrv(this.treDoorAll, tabDor, "名称", "id", "PID", "@ID={ID},@PID={PID},@编号={编号},@站址={站址}");
             this.treDoorAll.ExpandAll();
            DataTable tabDept = this.ds.Tables["卡发行"];
            this.bindMgr.BindTrv(this.treDept, tabDept, "名称","id", "PID", "@ID={ID},@PID={PID},@用户编号={用户编号},@卡号={卡号}");
            this.treDept.ExpandAll();
            this.treDept.AfterCheckNode += new NodeEventHandler(treCardAll_AfterCheckNode);
            this.treDept.FocusedNodeChanged += new FocusedNodeChangedEventHandler(treDept_FocusedNodeChanged);
            this.treDept.FocusedNode = this.treDept.Nodes.ParentNode;
            this.treDoorAll.AfterCheckNode += new NodeEventHandler(treDoorAll_AfterCheckNode);
        }

        /// <summary>
        /// 当节点有子节点时，表名选中了该节点下所有子节点的用户编号，当没有子节点时，表名选中一个用户编号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treDept_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            string userNo = string.Empty;
            if (e.Node == null)
                return;
            //绑定门之前，先把树所有复选框置为false;
            NotRight(this.treDoorAll);
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            if (e.Node.HasChildren)
            {
                foreach (DataRow dr in this.ds.Tables["卡发行"].Select("pid='" + tag + "'"))
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
                if (this.ds.Tables["门禁权限设置"].Select("用户编号='" + s + "'").Length > 0)
                {
                    DataRow dr = this.ds.Tables["门禁权限设置"].Select("用户编号='" + s + "'")[0];
                    string[] Right = dr["权限"].ToString().Split(',');
                    DataTable doorTab = this.ds.Tables["门"];
                    GetRight(Right, doorTab);
                }
            }
            this.treDoorAll.ExpandAll();
        }

        void treDoorAll_AfterCheckNode(object sender, NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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
        private void Uncheck( TreeListNode node )
        {
            node.CheckState =CheckState.Unchecked;
            foreach (TreeListNode door in node.Nodes)
            {
                Uncheck(door);
            }
        }

        /// <summary>
        /// 选择门卡能打开的门，如果门可以打开，树节点复选框的值置为true 否则置为false
        /// </summary>
        /// <param name="Door">用户选择卡节点，卡能打开的门的集合</param>
        /// <param name="doorTab">所有的门</param>
        private void GetRight(string[] Door, DataTable doorTab)
        {
            foreach (TreeListNode node in this.treDoorAll.Nodes)
            {
                GetRightCheck(node, Door, doorTab);
            }
        }

        /// <summary>
        /// 选择门卡能打开的门，如果门可以打开，树节点复选框的值置为true 否则置为false
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="Door">用户选择卡节点，卡能打开的门的集合</param>
        /// <param name="doorTab">所有的门</param>
        private void GetRightCheck(TreeListNode node, string[] Door,DataTable doorTab)
        {
            foreach (string s in Door)
            {
                if (doorTab.Select("门编号='" + s + "'").Length ==0)
                {
                    continue;
                }
                string DoorTag = doorTab.Select("门编号='" + s + "'")[0]["id"].ToString();
                string tag = basefun.valtag(Convert.ToString(node.Tag), "ID");
                if (tag == DoorTag)
                {
                    node.Checked = true;
                    break; 
                }
            }
            foreach( TreeListNode Dnode in node.Nodes  )
            {
                GetRightCheck(Dnode, Door, doorTab);
            }
        }

        void treCardAll_AfterCheckNode(object sender, NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
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
            CardAll = string.Empty;
            DoorAll = string.Empty;
            controlAll = string.Empty;
            #region 遍历选择的卡
            foreach (TreeListNode Card in this.treDept.Nodes)
            {
                checkCard(Card);
            }
            #endregion
            #region 遍历选择的门
            foreach (TreeListNode Door in this.treDoorAll.Nodes)
            {
                checkDoor(Door);
            }
            #endregion
            if (CardAll == string.Empty)
            {
                XtraMessageBox.Show("请选择部门或用户","系统提示！");
                return;
            }
            //卡权限
            string[] checkCardAll = CardAll.Split(',');
            foreach (string c in checkCardAll)
            {
                foreach (DataRow dr in this.ds.Tables["门禁权限设置"].Select(" 用户编号 ='"+ c +"'"))
                {
                    dr.BeginEdit();
                    dr["权限"] = DoorAll;
                    dr.EndEdit();
                }
            }
            WhiteList(checkCardAll);
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
        /// <param name="checkCardAll"></param>
        private void WhiteList(string[] checkCardAll)
        {
            //控制器对应读卡器号
            string[] control = controlAll.Split(',');
            foreach (string str in control)
            {
                string readNo = string.Empty;
                foreach (DataRow drDoor in this.ds.Tables["门"].Select("控制器='" + str + "'"))
                {
                    if (readNo == string.Empty)
                    {
                        readNo = drDoor["读卡器号"].ToString();
                    }
                    else
                    {
                        readNo += string.Format(",{0}", drDoor["读卡器号"].ToString());
                    }
                }
                if (!ctrlReadNo.ContainsKey(str))
                    ctrlReadNo.Add(str, readNo);
            }
            //添加门禁的白名单记录
            foreach (string user in checkCardAll)
            {
                string cardNo = this.ds.Tables["门禁权限设置"].Select(" 用户编号 ='" + user + "'")[0]["卡号"].ToString();
                foreach (DataRow drCardNo in this.ds.Tables["白名单"].Select("业务类别='门禁' and 卡号='" + cardNo + "'"))
                {
                    drCardNo.Delete();
                }
                foreach (string ctrl in control)
                {
                    if (ctrl == string.Empty)
                        continue;
                    DataRow dr = this.ds.Tables["白名单"].NewRow();
                    dr["卡号"] = cardNo;
                    dr["设备"] = ctrl;
                    dr["状态"]= "白";
                    dr["业务类别"]= "门禁";
                    dr["附加"] = ctrlReadNo[ctrl].ToString();
                    this.ds.Tables["白名单"].Rows.Add(dr);
                }
            }
        }

       /// <summary>
       /// 帅选勾选的用户编号
       /// </summary>
       /// <param name="node"></param>
        private void checkCard(TreeListNode node)
        {
            if (!this.ds.Tables["门禁权限设置"].Columns.Contains("用户编号"))
                return;
            string userNo = basefun.valtag(Convert.ToString(node.Tag), "用户编号");
            string cardNo = basefun.valtag(Convert.ToString(node.Tag), "卡号");
            if (userNo != string.Empty)
            {
                if (node.Checked == true && this.ds.Tables["门禁权限设置"].Select("用户编号='" + userNo + "'").Length > 0)
                {
                    if (CardAll == string.Empty)
                    {
                        CardAll = basefun.valtag(Convert.ToString(node.Tag), "用户编号");
                    }
                    else
                    {
                        CardAll += String.Format(",{0}", basefun.valtag(Convert.ToString(node.Tag), "用户编号"));
                    }
                }
            }
            foreach (TreeListNode Card in node.Nodes)
            {
                checkCard(Card);
            }
        }

        /// <summary>
        /// 帅选勾选的门信息
        /// </summary>
        /// <param name="node"></param>
        private void checkDoor(TreeListNode node)
        {
            string tag = basefun.valtag(Convert.ToString(node.Tag), "ID");
            if (node.Checked == true&&this.ds.Tables["门"].Select("id='" + tag + "'").Length> 0)
            {
                DataRow dr = this.ds.Tables["门"].Select("id='" + tag + "'")[0];
                if (DoorAll == string.Empty)
                {
                    DoorAll = dr["门编号"].ToString();
                    controlAll = dr["控制器"].ToString();
                }
                else
                {
                    DoorAll += String.Format(",{0}", dr["门编号"].ToString());
                    if (!controlAll.Contains(dr["控制器"].ToString()))
                    {
                        controlAll +=string.Format(",{0}", dr["控制器"].ToString());
                    }
                }
            }
            foreach (TreeListNode Door in node.Nodes)
            {
                checkDoor(Door);
            }
        }
    }
}