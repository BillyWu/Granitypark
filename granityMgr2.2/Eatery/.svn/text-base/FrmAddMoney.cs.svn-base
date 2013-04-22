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
using DevExpress.XtraEditors;
using System.Text.RegularExpressions;

namespace Granity.granityMgr.Eatery
{
    /// <summary>
    /// 补助管理
    /// </summary>
    public partial class FrmAddMoney : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "基本资料";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        QueryDataRes Query = null;
        public FrmAddMoney()
        {
            InitializeComponent();
        }
        private void FrmAddMoney_Load(object sender, EventArgs e)
        {
            //读取业务单元和传递参数
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            LoadDept();
        }
        #region  加载部门
        public void LoadDept()
        {
            DataRow drDept = this.dsUnit.Tables["部门"].NewRow();
            drDept["名称"] = "全部";
            drDept["编号"] = string.Empty;
            this.dsUnit.Tables["部门"].Rows.Add(drDept);
            this.cboDept.Properties.DataSource = this.dsUnit.Tables["部门"];
            this.cboDept.Properties.DisplayMember = "名称";
            this.cboDept.Properties.ValueMember = "编号";
            this.cboDept.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
        }
        #endregion
        /// <summary>
        /// 根据部门检索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboDept_TextChanged(object sender, EventArgs e)
        {
            string filter = "c.名称 like '%{0}%'";
            if (cboDept.Text == "全部")
                filter = "";
            else
            {
                filter = string.Format(filter, this.cboDept.Text.Replace("'", "''"));
            }
            //得到数据源
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = dsUnit.Tables["补助管理"]; ;
            if (null == tab) return;
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
            detpList.Items.Clear();
            foreach (DataRow dr in tab.Rows)
            {
                this.detpList.Items.Add(dr["姓名"] + "(" + dr["卡号"] + ")");
            }
        }
        /// <summary>
        /// 全部向右移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRigthAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < detpList.Items.Count; i++)
            {
                this.moneyList.Items.Add(detpList.Items[i]);
            }
            detpList.Items.Clear();
        }
        /// <summary>
        /// 全部向左移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeftAll_Click(object sender, EventArgs e)
        {
            //添加
            for (int i = 0; i < this.moneyList.Items.Count; i++)
            {
                this.detpList.Items.Add(this.moneyList.Items[i]);
            }
            moneyList.Items.Clear();
        }
        
        /// <summary>
        /// 单个向右移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.detpList.SelectedItems.Count; i++)
            {
                this.moneyList.Items.Add(this.detpList.SelectedItems[i]);
            }
            for (int i = 0; i < this.detpList.SelectedItems.Count; i++)
            {
                this.detpList.Items.Remove(this.detpList.SelectedItems[i]);
            }
        }
        /// <summary>
        /// 单个向左移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.moneyList.SelectedItems.Count; i++)
            {
                this.detpList.Items.Add(this.moneyList.SelectedItems[i]);
            }
            for (int i = 0; i < this.moneyList.SelectedItems.Count; i++)
            {
                this.moneyList.Items.Remove(this.moneyList.SelectedItems[i]);
            }
        }
        /// <summary>
        /// 保存补助记录，修改卡片余额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMoney.Text) || moneyList.Items.Count < 1)
            {
                XtraMessageBox.Show("补助金额为空或者卡号为空!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #region 循环写入数据
            for (int i = 0; i < moneyList.Items.Count; i++)
            {
                string[] sArray = moneyList.Items[i].ToString().Split('(');
                foreach (string j in sArray)
                {
                    if (j.ToString().Contains(")"))
                    {
                        string[] ssArray = j.ToString().Split(')');
                        foreach (string h in ssArray)
                        {
                            if (!string.IsNullOrEmpty(h.ToString()))
                            {
                                string CardNo = h.ToString();
                                NameObjectList ps = new NameObjectList();
                                ParamManager.MergeParam(ps, this.paramwin);
                                ps["卡号"] = CardNo;

                                #region 查询当前记录
                                QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
                                DataTable tab = dsUnit.Tables["补助信息"];
                                if (null == tab) return;
                                tab.Clear();
                                query.FillDataSet(tab.TableName, ps, this.dsUnit);
                                #endregion

                                //保存充值记录并且更新卡片余额
                                DataRow dr = tab.Rows[0];
                                dr["卡号"] = CardNo;
                                dr["补助充值"] = txtMoney.Text;
                                dr["操作员"] = BindManager.getUser().UserName;
                                NameObjectList ps1 = ParamManager.createParam(dr);
                                ParamManager.MergeParam(ps1, this.paramwin, false);
                                this.Query.ExecuteNonQuery("补助信息", ps1, ps1, ps1);
                            }
                        }
                    }
                }
            }
            #endregion

            moneyList.Items.Clear();
            MessageBox.Show("补助成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        /// <summary>
        /// 金额和合法性验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMoney_TextChanged(object sender, EventArgs e)
        {
            Regex MoneyReg = new Regex(@"^\-??\d+\.??\d+$");
            if (!MoneyReg.IsMatch(txtMoney.Text))
                txtMoney.Text = "";
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnclose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
    }
}