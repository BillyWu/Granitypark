using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
namespace Granity.granityMgr.Eatery.Reprot
{
    public partial class ReportCashSum : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportCashSum()
        {
            InitializeComponent();
        }
        public ReportCashSum( DataSet ds)
        {
            InitializeComponent();
            this.dataSetCashSum1.Merge(ds);
        }
    }
}
