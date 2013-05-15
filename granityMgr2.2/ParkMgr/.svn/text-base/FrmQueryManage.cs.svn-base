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
using System.Data.SqlClient;
using System.IO;
using DevExpress.XtraEditors;

namespace Granity.granityMgr.ParkMgr
{
    /// <summary>
    /// 出场纪录和查询
    /// </summary>
    public partial class FrmQueryManage : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "停车记录";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmQueryManage()
        {
            InitializeComponent();
        }
        private void FrmQueryManage_Load(object sender, EventArgs e)
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
            DataTable tab = this.dsUnit.Tables["停车记录"];
            if (null == tab || tab.Rows.Count < 1)
                return;
            LoadCardTyep();
            LoadDev();

        }
        /// <summary>
        /// 加载卡片类型
        /// </summary>
        public void LoadCardTyep()
        {
            DataRow drCardType = this.dsUnit.Tables["卡片类型"].NewRow();
            drCardType["卡类"] = "卡类";
            drCardType["编号"] = 1;
            this.dsUnit.Tables["卡片类型"].Rows.Add(drCardType);
            this.CbCardtype.Properties.DataSource = this.dsUnit.Tables["卡片类型"];
            this.CbCardtype.Properties.DisplayMember = "卡类";
            this.CbCardtype.Properties.ValueMember = "编号";
            this.CbCardtype.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("卡类", "卡类", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
        }
        /// <summary>
        /// 加载卡片类型
        /// </summary>
        public void LoadDev()
        {
            DataRow drCardType = this.dsUnit.Tables["设备"].NewRow();
            drCardType["名称"] = "名称";
            drCardType["名称"] = 1;
            this.dsUnit.Tables["设备"].Rows.Add(drCardType);
            this.dtDev.Properties.DataSource = this.dsUnit.Tables["设备"];
            this.dtDev.Properties.DisplayMember = "名称";
            this.dtDev.Properties.ValueMember = "名称";
            this.dtDev.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtQuery_Click(object sender, EventArgs e)
        {
            string StartDtTime = this.StartDate.Text + " " + this.cboStartTime.Text;
            string EndDtTime = this.EndDate.Text + " " + this.cboEndTime.Text;
            //获取当前单元名称
            DataTable tab = this.dbGridIn.DataSource as DataTable;
            if (null == tab) return;
            string filter = "";
            filter = " a.日期 between (SELECT convert(datetime,'{0}',121)) and (SELECT convert(datetime,'{1}',121)+1)  and a.用户编号 like '%{2}%' and b.name like '%{3}%' and c.卡类 like '%{4}%' and (a.车牌 like '%{5}%' or a.车牌 is null) and a.卡号 like '%{6}%' and p.名称 like '%{7}%'";
            filter = string.Format(filter, StartDtTime.Replace("'", "''"), EndDtTime.Replace("'", "''"), txtuserID.Text.Replace("'", "''"), this.txtuserName.Text.Replace("'", "''"), this.CbCardtype.Text.Replace("'", "''"), TxtCarNo.Text.Replace("'", "''"), TxtCardNo.Text.Replace("'", "''"), this.dtDev.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);

        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtClose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 打印记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtPrint_Click(object sender, EventArgs e)
        {
            this.dbGridIn.ShowPrintPreview();
        }


        private void dbGrid_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = e.RowHandle.ToString();
            }
        }

        private void dbGrid_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow drv = this.dbGrid.GetDataRow(e.RowHandle);
            FrmInParkPic frmIn = new FrmInParkPic();
            string img = this.dbGrid.GetDataRow(e.RowHandle)["入场图片"].ToString();

            //Billy
            byte[] byData1 = new byte[100];
            char[] charData1 = new char[1000];
            FileStream sFile1 = null;

            try
            {
                sFile1 = new FileStream(img, FileMode.Open, FileAccess.Read);
                sFile1.Seek(55, SeekOrigin.Begin);
                sFile1.Read(byData1, 0, 100); //第一个参数是被传进来的字节数组,用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
            }
            catch
            {
                MessageBox.Show("读取图片失败");
                return;
            }
            Decoder d1 = Encoding.UTF8.GetDecoder();
            d1.GetChars(byData1, 0, byData1.Length, charData1, 0);
            picpark.BackgroundImage = Image.FromStream(sFile1);

            return;

            string server = DataAccRes.AppSettings("服务器");

            if (server != "127.0.0.1")
            {
                string pic = img;
                if (string.IsNullOrEmpty(pic)) return;
                string[] sArray = pic.Split('\\');
                string path = "\\\\" + server + @"\D$" + @"\images";


                string[] test = Directory.GetFiles(path);
                string strResult = string.Empty;
                string sss = "\\\\" + server + @"\D$" + @"\images\" + sArray[2].ToString();
                if (Array.IndexOf(test, sss) != -1)
                {
                    byte[] byData = new byte[100];
                    char[] charData = new char[1000];
                    FileStream sFile = null;

                    try
                    {
                        sFile = new FileStream(sss, FileMode.Open, FileAccess.Read);
                        sFile.Seek(55, SeekOrigin.Begin);
                        sFile.Read(byData, 0, 100); //第一个参数是被传进来的字节数组,用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
                    }
                    catch
                    {
                        MessageBox.Show("读取图片失败");
                        return;
                    }
                    Decoder d = Encoding.UTF8.GetDecoder();
                    d.GetChars(byData, 0, byData.Length, charData, 0);
                    picpark.BackgroundImage = Image.FromStream(sFile);
                }
            }
        }

    }
}