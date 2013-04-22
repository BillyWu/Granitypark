using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using DevExpress.XtraEditors.Repository;
using System.Text.RegularExpressions;

namespace Granity.granityMgr.CardMgr
{
    public partial class FrmDevSet : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 发行器管理
        /// </summary>
        string unitName = "卡片管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmDevSet()
        {
            InitializeComponent();
        }
        private void FrmDevSet_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
            //加载串口
            string[] Com ={
              "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10"};
            LoadComBox("串口", Com);
            //加载波特率
            string[] BTL ={ "4800", "9600", "19200", "38400", "56000" };
            LoadComBox("波特率", BTL);
            //加载数据位
            string[] DataW ={ "5", "6", "7", "8" };
            LoadComBox("数据位", DataW);
            //加载停止位
            string[] DopW ={ "1", "2" };
            LoadComBox("停止位", DopW);
        }
        /// <summary>
        /// 加载ComBox
        /// </summary>
        public void LoadComBox(string Name, string[] str)
        {
            RepositoryItemComboBox Combo = new RepositoryItemComboBox();
            for (int i = 0; i < str.Length; i++)
            {
                Combo.Items.Add(str[i].ToString());
            }
            Combo.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dbDev.RepositoryItems.Add(Combo);
            this.gridView1.Columns[Name].ColumnEdit = Combo;
        }
        /// <summary>
        //添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbDev.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();
            dr["波特率"] = "19200";
            dr["数据位"] = 8;
            dr["停止位"] = 1;
            dr["串口"] = "COM1";
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbDev.DataSource as DataTable;
            if (null == tab || tab.Rows.Count < 1)
                return;
            DialogResult result = MessageBox.Show("是否删除当前记录", "删除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result)
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (!isSuccess)
                MessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// IP地址合法性验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            Regex IpReg = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
            string IpVal = this.gridView1.GetDataRow(e.RowHandle)["IP地址"].ToString();
            if (!IpReg.IsMatch(IpVal))
                this.gridView1.GetDataRow(e.RowHandle)["IP地址"] = "";
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {

            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
    }
}