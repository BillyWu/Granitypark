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
namespace Granity.granityMgr.BaseDicinfo
{
    /// <summary>
    /// 设计思想：用户点击门是门时，显示出该门所有的用户；用户点击门禁或门分组时，显示门禁或门禁组下面所有的门的用户
    /// 如门组 Granity 下面有门禁 Ctr001，门禁 Ctr001下面有 1，2，3，4 ，用户点击门组Granity 时显示 1，2，3，4 ，用户点击Ctr001
    /// 时显示 1，2，3，4 ；用户点击具体的门时，只显示出选择门对应的用户
    /// 具体实现过程 略
    /// </summary>
    public partial class FrmDorUsers : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门用户";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        /// <summary>
        /// 用户选择的门，当选择的门组或控制器时，就是控制或门组下面所有门
        /// </summary>
        private string doorAll = string.Empty;
        public FrmDorUsers()
        {
            InitializeComponent();
        }

        private void FrmDorUsers_Load(object sender, EventArgs e)
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
            DataTable tabDor = this.ds.Tables["门禁分组"];
            this.bindMgr.BindTrv(this.treDoorAll, tabDor, "名称", "id", "PID", "@ID={ID},@PID={PID},@编号={编号},@站址={站址}");
            this.treDoorAll.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(treDoorAll_FocusedNodeChanged);
            this.treDoorAll.FocusedNode = null;
            this.treDoorAll.AfterCheckNode += new NodeEventHandler(treDoorAll_AfterCheckNode);
            this.treDoorAll.ExpandAll();
            DataTable tabDept = this.ds.Tables["部门用户信息"];
            this.bindMgr.BindTrv(this.treDeptUser, tabDept, "名称", "id", "PID", "@ID={ID},@PID={PID},@用户编号={用户编号},@站址={站址}");
            this.treDeptUser.ExpandAll();
        }

        void treDoorAll_AfterCheckNode(object sender, NodeEventArgs e)
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

        void treDoorAll_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            TreeListNode trv = e.Node;
            if (trv == null||e.Node.TreeList.Nodes.Count == 0)
                return;
            NotRight(this.treDeptUser);
            DataTable dtDor = DorRight(this.ds.Tables["用户门权限"]);
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            this.doorAll = string.Empty;
            string[] dor = GetDor(trv,this.doorAll).Split(',');
            if (dor ==null)
                return;
            foreach (string s in dor)
            {
                foreach (TreeListNode tn in this.treDeptUser.Nodes)
                {
                    GetDorUsers(tn, s, dtDor);
                }
            }
        }

        /// <summary>
        /// 递归找出当前节点下的节点（门），找出的门记录在全局变量doorAll
        /// 没有采用门禁权限里的方法来获取当前节点下的门，递归更直观，维护性强
        /// </summary>
        /// <param name="tn">树节点</param>
        /// <param name="allDor">全局变量doorAll</param>
        /// <returns></returns>
        private string GetDor(TreeListNode tn, string allDor)
        {
            string dorNo = basefun.valtag(Convert.ToString(tn.Tag), "编号");
            if (this.ds.Tables["门"].Select("门编号='" + dorNo + "'").Length > 0)
            {
                if (this.doorAll == string.Empty)
                {
                    this.doorAll = dorNo;
                }
                else
                {
                    this.doorAll += "," + dorNo;
                }
            }
            foreach (TreeListNode trn in tn.Nodes)
            {
                GetDor(trn, this.doorAll);
            }
            return doorAll;
        }

        /// <summary>
        /// 门所具有的用户
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="dor">门</param>
        /// <param name="userDorAll">所有用户信息</param>
        /// <returns></returns>
        private void GetDorUsers(TreeListNode tn, string dor, DataTable userDorAll)
        {
            if (tn == null || dor == string.Empty|| userDorAll == null)
                return;
            string userNo = basefun.valtag(Convert.ToString(tn.Tag), "用户编号");
            DataRow[] userAllNo = userDorAll.Select("门='" + dor + "' and 用户编号='" + userNo + "'");
            if (userAllNo.Length > 0)
            {
                tn.CheckState = CheckState.Checked;
            }
            foreach (TreeListNode stn in tn.Nodes)
            {
                GetDorUsers(stn, dor, userDorAll);
            }
        }

        /// <summary>
        /// 得到门权限，然后通过转换把行记录格式转换成（用户编号，门）格式放到内存
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable DorRight( DataTable dt )
        {
            if (dt == null)
                return null ;
            DataTable dtDor = new DataTable();
            dtDor.Columns.Add("用户编号");
            dtDor.Columns.Add("门");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["权限"].ToString() != string.Empty)
                {
                    string[] dor = dr["权限"].ToString().Split(',');
                    foreach (string sdor in dor)
                    {
                        DataRow drRight = dtDor.NewRow();
                        drRight["用户编号"] = dr["用户编号"].ToString();
                        drRight["门"] = sdor;
                        dtDor.Rows.Add(drRight);
                    }
                }
            }
            return dtDor;
        }

        /// <summary>
        /// 树复选框节点全部置为false
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

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}