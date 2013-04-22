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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Granity.communications;
using Granity.CardOneCommi;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.Text.RegularExpressions;

namespace Granity.granityMgr.Eatery
{
    /// <summary>
    /// 餐厅维护
    /// </summary>
    public partial class FrmRestaurant : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "基本资料";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        string DinName = "";
        public FrmRestaurant()
        {
            InitializeComponent();
        }
        private void FrmRestaurant_Load(object sender, EventArgs e)
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
            DataTable tab = this.ds.Tables["餐厅维护"];
            this.bindMgr.BindTrv(this.TreeDin, tab, "名称", "id", "PID", "@ID={ID},@PID={PID},@编号={编号},@名称={名称}");
            this.TreeDin.ExpandAll();
            LoadTypeandCOM();
            this.txtDinName.EditValueChanged += new EventHandler(txtEditValueChanged);
        }
        /// <summary>
        /// 修改树节点text事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = sender as DevExpress.XtraEditors.TextEdit;
            setTreeNodeText(this.TreeDin, "名称", txt.Text.Trim());
        }
        /// <summary>
        /// 修改树节点的text值
        /// </summary>
        /// <param name="trv">树</param>
        /// <param name="colName">树节点列名</param>
        /// <param name="colValue">列值</param>
        private void setTreeNodeText(TreeList trv, string colName, string colValue)
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
        /// 加载下拉框
        /// </summary>
        public void LoadTypeandCOM()
        {
            //加载通讯类别
            string[] type ={
              "TCP/IP(局域网)","UDP/IP(局域网)","串口"};
            LoadComboBox(gridView4, type, "通讯类别");
            //加载串口
            string[] Com ={
              "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10"};
            LoadComboBox(gridView4, Com, "串口");
            //加载区号
            string[] Area ={
              "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15"};
            LoadComboBox(gridView4, Area, "卡片区号");
            string[] Week ={
              "星期一","星期二","星期三","星期四","星期五","星期六","星期日"};
            LoadComboBox(gridView1, Week, "星期");
            //加载波特率
            string[] BTL ={ "4800", "9600", "19200", "38400", "56000" };
            LoadComboBox(this.gridView4, BTL, "波特率");
            //加载端口
            string[] DK ={ "30000", "60000" };
            LoadComboBox(this.gridView4, DK, "端口");
            //加载数据位
            string[] DataW ={ "5", "6", "7", "8" };
            LoadComboBox(gridView4, DataW, "数据位");
            //加载停止位
            string[] DopW ={ "1", "2" };
            LoadComboBox(gridView4, DopW, "停止位");
        }
        /// <summary>
        /// 加载Gridview的下拉列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="str"></param>
        /// <param name="ColuName"></param>
        public void LoadComboBox(DevExpress.XtraGrid.Views.Grid.GridView db, string[] str, string ColuName)
        {
            RepositoryItemComboBox riCombo = new RepositoryItemComboBox();
            if (ColuName == "通讯类别")
            {
                riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            }
            for (int i = 0; i < str.Length; i++)
            {
                riCombo.Items.Add(str[i].ToString());
            }
            riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            riCombo.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dbDev.RepositoryItems.Add(riCombo);
            db.Columns[ColuName].ColumnEdit = riCombo;
        }
        /// <summary>
        /// 添加数据 消费时段,菜单,定额,消费机
        /// </summary>
        /// <param name="db">GridControl</param>
        public void AddNewDate(DevExpress.XtraGrid.GridControl db, string flag)
        {
            DataTable tab = db.DataSource as DataTable;
            if (null == tab) return;
            #region 检测是否指定了餐厅
            if (string.IsNullOrEmpty(DinName))
            {
                XtraMessageBox.Show("请选择餐厅!", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            DataRow dr = tab.NewRow();
            switch (flag)
            {
                case "1":
                    if (gridView1.RowCount > 0)
                    {
                        dr["早餐开始"] = gridView1.GetDataRow(0)["早餐开始"];
                        dr["午餐开始"] = gridView1.GetDataRow(0)["午餐开始"];
                        dr["晚餐开始"] = gridView1.GetDataRow(0)["晚餐开始"];
                        dr["夜宵开始"] = gridView1.GetDataRow(0)["夜宵开始"];
                        dr["早餐结束"] = gridView1.GetDataRow(0)["早餐结束"];
                        dr["午餐结束"] = gridView1.GetDataRow(0)["午餐结束"];
                        dr["晚餐结束"] = gridView1.GetDataRow(0)["晚餐结束"];
                        dr["夜宵结束"] = gridView1.GetDataRow(0)["夜宵结束"];
                    }
                    dr["餐厅ID"] = DinName;
                    dr["ID"] = Guid.NewGuid();
                    break;
                case "2":
                    dr["餐厅ID"] = DinName;
                    dr["ID"] = Guid.NewGuid();
                    break;
                case "3":
                    dr["定额餐厅"] = DinName;
                    break;
                case "4":
                    dr["ID"] = Guid.NewGuid();
                    dr["餐厅名称"] = DinName;
                    dr["波特率"] = "19200";
                    dr["数据位"] = 8;
                    dr["停止位"] = 1;
                    dr["端口"] = 30000;
                    dr["串口"] = "COM1";
                    break;
            }
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// 删除数据 消费时段,菜单,定额,消费机
        /// </summary>
        /// <param name="db">GridControl</param>
        public void DelDate(DevExpress.XtraGrid.GridControl db, DevExpress.XtraGrid.Views.Grid.GridView grid)
        {
            if (grid.RowCount == 0)
                return;
            DataTable tab = db.DataSource as DataTable;
            if (null == tab || tab.Rows.Count < 1)
                return;
            DialogResult result = XtraMessageBox.Show("是否删除当前记录", "系统提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result)
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }
        /// <summary>
        /// 根据餐厅检索 消费时段,菜单,消费机ddd
        /// </summary>
        /// <param name="db">GridControl</param>
        /// <param name="flag">类别标示</param>
        /// <param name="ID">编号</param>
        public void CheckByDin(DevExpress.XtraGrid.GridControl db, string flag, string ID)
        {
            try
            {
                DataTable tab = db.DataSource as DataTable;
                if (null == tab) return;
                if (string.IsNullOrEmpty(ID)) return;
                string filter = "";
                switch (flag)
                {
                    case "1":
                        filter = "餐厅ID='{0}' order by 星期序号";
                        break;
                    case "2":
                        filter = "餐厅ID='{0}'";
                        break;
                    case "4":
                        filter = "a.餐厅ID='{0}'";
                        break;
                    case "3":
                        filter = "餐厅ID='{0}'";
                        break;
                    default:
                        break;
                }
                filter = string.Format(filter, ID.Replace("'", "''"));
                QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
                tab.Clear();
                query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.ds);
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 添加 时段，消费机，定额，菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            string flag = dbEatery.SelectedTabPageIndex.ToString();
            switch (flag)
            {
                case "0":
                    AddNewDate(dbEateyTime, "1");
                    break;
                case "1":
                    AddNewDate(dbMenu, "2");
                    break;
                case "2":
                    AddNewDate(dbMoney, "3");
                    break;
                default:
                    AddNewDate(dbDev, "4");
                    break;
            }
        }

        /// <summary>
        /// 生成唯一的餐厅编号
        /// </summary>
        /// <returns></returns>
        public string DinNum()
        {
            Guid tempGuid = Guid.NewGuid();
            byte[] bytes = tempGuid.ToByteArray();
            int i = Math.Abs(((int)bytes[1] << 6));
            return i.ToString();
        }
        /// <summary>
        /// 添加节点餐厅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        TreeListNode nodeid;
        private void btnDinAdd_Click_1(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeDin.FocusedNode;
            string pid = "";
            pid = Guid.NewGuid().ToString();
            DataTable tab = this.ds.Tables["餐厅维护"];
            DataRow drnew = tab.NewRow();
            drnew["名称"] = "餐厅";
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["PID"] = pid;
            drnew["编号"] = DinNum();
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.TreeDin.AppendNode(new object[] { drnew["名称"].ToString(), drnew["ID"].ToString() }, this.TreeDin.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@编号=" + drnew["名称"].ToString() + "";
            node.ImageIndex = 1;
            node.SelectImageIndex = 4;
            this.TreeDin.FocusedNode = node;
            nodeid = this.TreeDin.FocusedNode;
        }
        /// <summary>
        /// 删除节点 餐厅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void btnDinDel_Click_1(object sender, EventArgs e)
        {
            TreeListNode trn = this.TreeDin.FocusedNode;
            if (null == trn)
                return;
            string tip = "确定是否删除当前节点";
            if (trn.Nodes.Count > 0)
                tip += ",同时将删除该节点的下级关联节点！";
            DialogResult msgquet = XtraMessageBox.Show(tip, "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            //递归删除节点
            TreeListNode trncurrent = trn;
            DataTable tabGroup = this.ds.Tables["餐厅维护"];
            string tag = basefun.valtag(Convert.ToString(trn.Tag), "ID");
            foreach (DataRow drGroup in tabGroup.Select("id='" + tag + "'"))
            {
                if (this.TreeDin.FindNodeByFieldValue("id", drGroup["id"].ToString()) != null)
                {
                    TreeListNode tn1 = this.TreeDin.FindNodeByFieldValue("id", drGroup["id"].ToString());
                    this.TreeDin.Nodes.Remove(tn1);
                }
                drGroup.Delete();
            }
        }
        /// <summary>
        /// 保存节点 餐厅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDinSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataTable tab = this.ds.Tables["餐厅维护"];
            this.bindMgr.BindTrv(this.TreeDin, tab, "名称", "id", "PID", "@ID={ID},@PID={PID},@编号={编号},@名称={名称}");
            this.TreeDin.ExpandAll();
            this.TreeDin.FocusedNode = nodeid;
        }
        /// <summary>
        /// 删除时段，菜单，定额，消费机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            string flag = dbEatery.SelectedTabPageIndex.ToString();
            switch (flag)
            {
                case "0":
                    DelDate(dbEateyTime, this.gridView1);
                    break;
                case "1":
                    DelDate(dbMenu, this.gridView2);
                    break;
                case "2":
                    DelDate(dbMoney, this.gridView3);
                    break;
                default:
                    DelDate(dbDev, this.gridView4);
                    break;
            }
        }
        /// <summary>
        /// 保存 时段，菜单，定额，消费机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //邦定到指定的餐厅
            CheckByDin(dbEateyTime, "1", DinName);
            CheckByDin(dbMenu, "2", DinName);
            CheckByDin(dbDev, "4", DinName);
            CheckByDin(dbMoney, "3", DinName);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtCancel_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();

        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeDin.FocusedNode;
            string pid = "";
            if (trnsel == null)
            {
                pid = Guid.NewGuid().ToString();
            }
            else
            {
                pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            }
            DataTable tab = this.ds.Tables["餐厅维护"];
            DataRow drnew = tab.NewRow();
            drnew["名称"] = "餐厅";
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["PID"] = pid;
            drnew["编号"] = DinNum();
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.TreeDin.AppendNode(new object[] { drnew["名称"].ToString(), drnew["ID"].ToString() }, this.TreeDin.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@编号=" + drnew["名称"].ToString() + "";
            node.ImageIndex = 1;
            node.SelectImageIndex = 4;
            this.TreeDin.FocusedNode = node;
            nodeid = this.TreeDin.FocusedNode;
        }
        /// <summary>
        /// 节点切换改变内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeDin_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            int rowIndex = 0;
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            if (string.IsNullOrEmpty(tag))
            {
                tag = Convert.ToString(e.Node.Tag);
                DinName = tag;
            }
            else
            {
                tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
                DinName = tag;
            }
            DataTable tab = ds.Tables["餐厅维护"];
            rowIndex = 0;
            foreach (DataRow dr in tab.Rows)
            {
                rowIndex++;
                if (DataRowState.Deleted != dr.RowState && dr["id"].ToString() == tag)
                {
                    BindingManagerBase bindMgrbase = this.BindingContext[tab];
                    if (bindMgrbase != null)
                    {
                        bindMgrbase.Position = rowIndex - 1;
                    }
                    break;
                }
            }

            #region 刷新纪录
            CheckByDin(dbEateyTime, "1", tag);
            CheckByDin(dbMenu, "2", tag);
            CheckByDin(dbDev, "4", tag);
            CheckByDin(dbMoney, "3", tag);
            this.txtDinName.TabIndex = 1;
            #endregion
        }
        /// <summary>s
        /// 数据合法性验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView4_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //IP地址
            Regex IpReg = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
            string IpVal = this.gridView4.GetDataRow(e.RowHandle)["IP地址"].ToString();
            if (!IpReg.IsMatch(IpVal))
                this.gridView4.GetDataRow(e.RowHandle)["IP地址"] = "";
            //正整数
            Regex NumReg = new Regex(@"^(0|[1-9]\d*)$");
            string NumVal = this.gridView4.GetDataRow(e.RowHandle)["编号"].ToString();
            if (!NumReg.IsMatch(NumVal))
                this.gridView4.GetDataRow(e.RowHandle)["编号"] = "";
        }
        /// <summary>
        /// 菜单数据合法性验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string[] str = new string[this.gridView2.RowCount];
            if (this.gridView2.RowCount < 1) return;
            for (int i = 0; i < this.gridView2.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView2.GetDataRow(i)["编号"]);
                if (Convert.ToString(this.gridView2.GetDataRow(e.RowHandle)["编号"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("此编号已经使用，请重新写入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView2.GetDataRow(e.RowHandle)["编号"] = "0";
                    return;
                }
            }
        }
        /// <summary>
        /// 验证定额明细合法性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            string[] str = new string[this.gridView3.RowCount];
            if (this.gridView3.RowCount < 1) return;
            for (int i = 0; i < this.gridView3.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView3.GetDataRow(i)["序号"]);
                if (Convert.ToString(this.gridView3.GetDataRow(e.RowHandle)["序号"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("此编号已经使用，请重新写入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView3.GetDataRow(e.RowHandle)["序号"] = "0";
                    return;
                }
            }
        }
        /// <summary>
        /// 验证时间是否唯一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string[] str = new string[this.gridView1.RowCount];
            if (this.gridView1.RowCount < 1) return;
            for (int i = 0; i < this.gridView1.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView1.GetDataRow(i)["星期"]);
                if (Convert.ToString(this.gridView1.GetDataRow(e.RowHandle)["星期"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("此编号已经使用，请重新写入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView1.GetDataRow(e.RowHandle)["星期"] = "0";
                    return;
                }
            }
        }
    }
}