using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using System.Globalization;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmDepositAll_del : DevExpress.XtraEditors.XtraForm
    {
        private DataTable tabSource;

        public DataTable TabData
        {
            get { return tabSource; }
            set { tabSource = value; }
        }
        public FrmDepositAll_del()
        {
            InitializeComponent();
        }

        private void btCharge_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void FrmDepositAll_del_Load(object sender, EventArgs e)
        {
            this.dgvlist.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dbgrid_RowPostPaint);
            dgvlist.DataSource = tabSource;
            BindManager mgr = new BindManager(this);
            mgr.SetGridCols(this.dgvlist, "卡号 80,姓名 80,卡类名称 卡类 60, 车型名称 车型 60,入场日期 110,进场日期 110, 停车时长 120,押金到期 110,卡片余额 60,停车费用 60");
        }

        /// <summary>
        /// 设置表格行标号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (null == grid) return;
            using (SolidBrush b = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(CultureInfo.CurrentCulture),
                        grid.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }
    }
}