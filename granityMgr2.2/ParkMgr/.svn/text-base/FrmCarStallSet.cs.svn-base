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

namespace Granity.parkStation
{
    public partial class FrmCarStallSet : Form
    {
        string unitName = "";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;

        DataGridViewPrinter MyDataGridViewPrinter;
        public FrmCarStallSet()
        {
            InitializeComponent();
        }
        private void FrmCarStallSet_Load(object sender, EventArgs e)
        {
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitName = pstrans["name"].ToString();//单元
            this.Text = unitName;
            //获取当前单元名称
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("车位组设置", "@db=车位组设置");
            dict.Add("时段设置", "@db=时段设置");
            if (unitName == "车位组设置")
            {
                panel3.Visible = false;
                this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
                this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            }
            else if (unitName == "时段设置")
            {
                panel4.Visible = false;
                this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
                this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            }

            //数据源
            string datasource = dict[this.unitName];
            this.dbGrid.Tag = datasource;
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            ds = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, ds);
            //列名显示
            if (datasource == "@db=车位组设置")
            {
                string cols = "车组编号 100,名称 301, 数量 150, 备注 205";
                bg.SetGridCols(this.dbGrid, cols);
            }
            if (datasource == "@db=时段设置")
            {
                string cols = "时段编号 80,时段名称 85, 开始时间1 85,截止时间1 85,开始时间2 85,截止时间2 85,";
                cols += "开始日期 83,截止日期 83,星期六 50,星期日 50";
                bg.SetGridCols(this.dbGrid, cols);
            }
        }

        //打印报表
        private bool SetupThePrinting()
        {
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = false;
            MyPrintDialog.AllowSelection = false;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = false;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;
            if (MyPrintDialog.ShowDialog() != DialogResult.OK)
                return false;
            string title;
            MyPrintDocument.DocumentName = unitName;
            title = MyPrintDocument.DocumentName;  //标题
            MyPrintDocument.PrinterSettings = MyPrintDialog.PrinterSettings;
            MyPrintDocument.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
            MyPrintDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);

            MyDataGridViewPrinter = new DataGridViewPrinter(dbGrid, MyPrintDocument, true, true, title, new Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);
            return true;
        }
        private void MyPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbGrid.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();

            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbDelel_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbGrid.DataSource as DataTable;
            if (null == tab || tab.Rows.Count < 1)
                return;
            DialogResult result = MessageBox.Show("是否删除当前记录", "删除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result)
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                MessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (SetupThePrinting())
                {
                    PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                    MyPrintPreviewDialog.Document = MyPrintDocument;
                    MyPrintPreviewDialog.ShowDialog();
                }
            }
            catch
            {
                MessageBox.Show("打印出错，请检查打印机！！");
            }
        }
    }
}