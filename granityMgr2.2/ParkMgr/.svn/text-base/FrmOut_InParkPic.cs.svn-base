using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.communications;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using System.Net;
using System.Globalization;
using System.IO;
using Granity.CardOneCommi;
using Granity.winTools;
using System.IO.Ports;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;

namespace Granity.parkStation
{
    public partial class FrmOut_InParkPic : DevExpress.XtraEditors.XtraForm
    {

        public FrmOut_InParkPic()
        {
            InitializeComponent();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmOut_InParkPic_Load(object sender, EventArgs e)
        {
            getPic_Server(this.txtInpic.Text, this.pl_inpic);
            getPic_Server(this.txtOutPic.Text, this.pl_outPic);
        }

        /// <summary>
        /// 从服务器中获得图片
        /// </summary>
        /// <param name="savePath">图片保存相对路径</param>
        /// <param name="pl">显示图片的容器</param>
        public void getPic_Server(string filepath, Panel pl)
        {
            if (string.IsNullOrEmpty(filepath))
                return;
            int port = 2010;
            string ipsrv = "127.0.0.1";
            string sport = DataAccRes.AppSettings("Granity文件服务");
            string conn = DataAccRes.DefaultDataConnInfo.Value;
            if (!string.IsNullOrEmpty(sport))
                try { port = Convert.ToInt32(sport); }
                catch { return; }
            Regex regIP = new Regex(@"server=([\w.\(\)]*)(;|\\)");
            if (regIP.IsMatch(conn))
            {
                Match mt = regIP.Match(conn);
                if (mt.Groups.Count > 1)
                    ipsrv = mt.Groups[1].Value.ToLower();
                if ("(local)" == ipsrv || "127.0.0.1" == ipsrv)
                    ipsrv = Dns.GetHostName();
                ipsrv = Dns.GetHostAddresses(ipsrv)[0].ToString();
            }
            CommiTarget target = new CommiTarget(ipsrv, port, CommiType.TCP);
            target.setProtocol(CmdFileTrans.PTL);
            CmdFileTrans cmd = new CmdFileTrans(false);
            cmd.GetFile(filepath);
            CommiManager.GlobalManager.SendCommand(target, cmd);
            if (cmd.EventWh.WaitOne(new TimeSpan(0, 0, 15), false))
            {
                byte[] data = cmd.FileContext;
                if (data.Length < 1)
                    return;
                MemoryStream stream = new MemoryStream(data);
                pl.BackgroundImage = Image.FromStream(stream);
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}