using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using System.Drawing.Printing;
using Granity.winTools;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmTable : Form
    {
        string unitName = "";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;

        DataGridViewPrinter MyDataGridViewPrinter;
        public FrmTable()
        {
            InitializeComponent();
        }

        private void OnCloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmTable_Load(object sender, EventArgs e)
        {
            //读取业务单元和传递参数
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);

            unitName = pstrans["name"].ToString();//单元
            this.Text = this.CountRecordGroup.Text = unitName;
            //获取当前单元名称
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("入场车流量统计表", "@db=入场车流量统计");
            dict.Add("出场车流量统计表", "@db=出场车流量统计");
            dict.Add("停车场收费统计表", "@db=停车场收费统计");
            dict.Add("收费员收费统计表", "@db=收费员收费统计");
            dict.Add("远程控制记录统计表", "@db=stateremote");
            //数据源
            string datasource = dict[this.unitName];
            this.RecordGrid.Tag = datasource;
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            ps["dt"] = this.dtpDate.Value;
            ps["type"] = this.cbRecordTp.SelectedIndex + 1;
            ps["操作员"] = null;
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            ds = bg.BuildDataset(this.unitItem, ps);
            bg.BindFld(this, ds);
            //特殊业务处理
            this.lbcount.Text = "总记录: " + Convert.ToString(ds.Tables[0].Rows.Count) + " 条";



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

            MyDataGridViewPrinter = new DataGridViewPrinter(RecordGrid, MyPrintDocument, true, true, title, new Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);
            return true;
        }



        private void MyPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }

        private void OnCloseBtn_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void QueryBtn_Click(object sender, EventArgs e)
        {
            //设置参数
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            string s = Convert.ToDateTime(this.dtpDate.Value).ToString("yyyy-MM-dd").ToString();
            ps["dt"] = Convert.ToDateTime(this.dtpDate.Value).ToString("yyyy-MM-dd").ToString();
            ps["type"] = this.cbRecordTp.SelectedIndex + 1;

            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = this.RecordGrid.DataSource as DataTable;
            if (null == tab) return;
            tab.Clear();
            query.FillDataSet(tab.TableName, ps, this.ds);
            //特殊业务处理
            this.lbcount.Text = "总记录: " + Convert.ToString(ds.Tables[0].Rows.Count) + " 条";
        }

        private void PrintBtn_Click(object sender, EventArgs e)
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