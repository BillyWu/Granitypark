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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
namespace Granity.granityMgr.util
{
    public partial class FrmCardParam : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "系统管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmCardParam()
        {
            InitializeComponent();
        }
        private void FrmCardParam_Load(object sender, EventArgs e)
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
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(gridView1.GetDataRow(0)["停车场ID模式"]) != Convert.ToString(gridView1.GetDataRow(0)["门禁ID模式"]) || Convert.ToString(gridView1.GetDataRow(0)["停车场ID模式"]) != Convert.ToString(gridView1.GetDataRow(0)["消费ID模式"]) || Convert.ToString(gridView1.GetDataRow(0)["消费ID模式"]) != Convert.ToString(gridView1.GetDataRow(0)["门禁ID模式"]))
            {
                MessageBox.Show("IC,ID模式不能同时使用！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (!isSuccess)
                MessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 控制单选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (Convert.ToString(gridView1.GetDataRow(e.RowHandle)["消费ID模式"]) == "True" && Convert.ToString(gridView1.GetDataRow(e.RowHandle)["消费IC模式"]) == "True")
            {
                MessageBox.Show("消费IC,ID模式不能同时选择！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView1.GetDataRow(e.RowHandle)["消费ID模式"] = "False";
                gridView1.GetDataRow(e.RowHandle)["消费IC模式"] = "False";
            }
            if (Convert.ToString(gridView1.GetDataRow(e.RowHandle)["门禁ID模式"]) == "True" && Convert.ToString(gridView1.GetDataRow(e.RowHandle)["门禁IC模式"]) == "True")
            {
                MessageBox.Show("门禁IC,ID模式不能同时选择！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView1.GetDataRow(e.RowHandle)["门禁ID模式"] = "False";
                gridView1.GetDataRow(e.RowHandle)["门禁IC模式"] = "False";
            }
            if (Convert.ToString(gridView1.GetDataRow(e.RowHandle)["停车场ID模式"]) == "True" && Convert.ToString(gridView1.GetDataRow(e.RowHandle)["停车场IC模式"]) == "True")
            {
                MessageBox.Show("停车场IC,ID模式不能同时选择！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView1.GetDataRow(e.RowHandle)["停车场ID模式"] = "False";
                gridView1.GetDataRow(e.RowHandle)["停车场IC模式"] = "False";
            }
        }
    }
}