using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Granity.granityMgr.Eatery
{
    public partial class FrmCashRegister : DevExpress.XtraEditors.XtraForm
    {
        public FrmCashRegister()
        {
            InitializeComponent();
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}