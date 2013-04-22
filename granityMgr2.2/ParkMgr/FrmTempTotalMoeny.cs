using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Common.Tools;
using Granity.winTools;
using Estar.Business.DataManager;
namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmTempTotalMoeny : DevExpress.XtraEditors.XtraForm
    {
        private string tag;
        /// <summary>
        /// 读取或设置收费数据
        /// </summary>
        public string DataTag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }
        private QueryDataRes query = null;
        /// <summary>
        /// 查询实例
        /// </summary>
        public QueryDataRes Query
        {
            get { return query; }
            set { query = value; }
        }
        public FrmTempTotalMoeny()
        {
            InitializeComponent();
        }

        private void FrmTempTotalMoeny_Load(object sender, EventArgs e)
        {
            string code = this.query.ExecuteScalar("打印编号序列号", new NameObjectList()).ToString();
            this.tag = basefun.setvaltag(this.tag, "{打印编号}", code);
            this.setContainerTagValue(this, this.tag, "pm");
            string realCar = basefun.valtag(tag, "{车牌号码}");
            string avtCar = basefun.valtag(tag, "{识别车牌号码}");
            string dtInTime = basefun.valtag(tag, "{入场时间}");
            if (string.IsNullOrEmpty(dtInTime))
                this.btCharge.Enabled = false;

        }

        /// <summary>
        /// 更加tag标记值更新控件内控件值,有下拉框则添加对应“名称”值
        /// </summary>
        /// <param name="ct">控件容器</param>
        /// <param name="tag">tag格式数据</param>
        /// <param name="keyName">tag标记映射标记名称</param>
        /// <returns>返回tag格式数据</returns>
        private string setContainerTagValue(Control ct, string tag, string keyName)
        {
            if (null == ct || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(keyName))
                return tag;
            string t = Convert.ToString(ct.Tag);
            string pm = basefun.valtag(t, keyName);
            if (!string.IsNullOrEmpty(pm))
            {
                string val = basefun.valtag(tag, "{" + pm + "}");
                string v = val;
                if (string.IsNullOrEmpty(val))
                    v = basefun.valtag(tag, pm);
                if (!(ct is DevExpress.XtraEditors.LookUpEdit))
                    ct.Text = v;
                else
                {
                    DevExpress.XtraEditors.LookUpEdit cbb = ct as DevExpress.XtraEditors.LookUpEdit;
                    try { cbb.EditValue = int.Parse(val); }
                    catch { }
                    if (string.IsNullOrEmpty(val))
                        tag = basefun.setvaltag(tag, pm + "名称", cbb.Text);
                    else
                        tag = basefun.setvaltag(tag, "{" + pm + "名称" + "}", cbb.Text);
                }
            }
            foreach (Control child in ct.Controls)
                tag = this.setContainerTagValue(child, tag, keyName);

            return tag;
        }

        /// <summary>
        /// 确定收费开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCharge_Click(object sender, EventArgs e)
        {

            string cardType = basefun.valtag(tag, "{卡类}");
            if (cardType == "9" && Convert.ToDecimal(this.txtLeaveMoney.Text) < Convert.ToDecimal(this.txtConsumeMoney.Text))
            {
                XtraMessageBox.Show("储值卡金额不足，请续费！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.tag = basefun.setvaltag(this.tag, "{收费}", this.txtConsumeMoney.Text.Trim());
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        /// <summary>
        /// 打开钱箱处理程序
        /// </summary>
        private void OpenCashbox()
        {
            //定义一个ProcessStartInfo实例
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            //设置启动进程的初始目录
            info.WorkingDirectory = Application.StartupPath;
            //设置启动进程的应用程序或文档名
            info.FileName = @"open.exe";
            //设置启动进程的参数
            info.Arguments = "";
            //启动由包含进程启动信息的进程资源
            try
            {
                System.Diagnostics.Process.Start(info);
            }
            catch (System.ComponentModel.Win32Exception we)
            {
                XtraMessageBox.Show(this, we.Message);
            }
        }

        private void BtClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}