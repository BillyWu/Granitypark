using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using Granity.CardOneCommi;
using Granity.communications;
namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmCarParkInfo : DevExpress.XtraEditors.XtraForm
    {
          /// <summary>
        /// 查询实例
        /// </summary>
        QueryDataRes Query = null;
        string DevIdS = "";//发行器地址
        /// <summary>
        /// 是否已经初始化读卡器
        /// </summary>
        bool isInited = false;
        /// <summary>
        /// 读卡器
        /// </summary>
        CmdCard cmdCard = new CmdCard();
        /// <summary>
        /// 当前卡号序列号
        /// </summary>
        string cardID = "";

        string unitName = "停车场卡信息";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmCarParkInfo()
        {
            InitializeComponent();
        }

        private void FrmCarParkInfo_Load(object sender, EventArgs e)
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
            DataTable tab = initDev();
            if (null == tab || tab.Rows.Count < 1)
                XtraMessageBox.Show("请设置发行器", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 初始化发行器
        /// </summary>
        /// <returns></returns>
        public DataTable initDev()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            string filter = "(电脑='{0}' or IP地址='{1}') and 通讯类别='发行器'";
            filter = string.Format(filter, Dns.GetHostName(), myip);
            DataTable tab = this.dsUnit.Tables["发行器"];
            tab.Clear();
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            query.FillDataSet("发行器", this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
            return tab;
        }

        /// <summary>
        /// 发行器信息
        /// </summary>
        public void initCmdCard()
        {
            DataTable tab = initDev();
            if (null == tab || tab.Rows.Count < 1)
            {
                this.cmdCard.SetTarget(null, -1, false);
                return;
            }
            DataRow dr = this.dsUnit.Tables["发行器"].Rows[0];
            string port = Convert.ToString(dr["串口"]);
            int baudrate = Convert.ToInt32(dr["波特率"]);
            Parity parity = Parity.None;
            int databits = Convert.ToInt32(dr["数据位"]);
            DevIdS = Convert.ToString(dr["站址"]);
            StopBits stopbits = StopBits.One;
            switch (Convert.ToString(dr["停止位"]))
            {
                case "1.5位": stopbits = StopBits.OnePointFive; break;
                case "2位": stopbits = StopBits.Two; break;
                default: stopbits = StopBits.One; break;
            }
            CommiTarget target = new CommiTarget(port, baudrate, parity, databits, stopbits);
            int addrst = Convert.ToInt32(dr["站址"]);
            bool success = this.cmdCard.SetTarget(target, addrst, false);
            if (success)
                this.cmdCard.Buzz(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.isInited)
            {
                this.initCmdCard();
                this.isInited = true;
                return;
            }
            string cardid = this.cmdCard.CardID;
            string cardsid = this.cmdCard.CardSID;
            string cardnum = this.cmdCard.CardNum;
            if (( string.IsNullOrEmpty(cardid)) || cardsid == "")
                return;

            this.cardID = cardid;
            for (int i = 0; i < this.gridviewCarInfo.RowCount; i++)
            {
                DataRow dr = this.gridviewCarInfo.GetDataRow(i);
                if (null == dr) continue;
                if (!cardnum.Equals(dr["卡号"]))
                    continue;
                this.gridviewCarInfo.SelectRow(i);
                this.gridviewCarInfo.FocusedRowHandle = i;
                break;
            }
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (!isSuccess)
            {
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.gridCarInfo.ShowPrintPreview();
        }
    }
}