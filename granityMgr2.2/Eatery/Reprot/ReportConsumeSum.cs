using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
namespace Granity.granityMgr.Eatery.Reprot
{
    public partial class ReportConsumeSum : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportConsumeSum()
        {
            InitializeComponent();
        }
        public ReportConsumeSum(DataSet ds)
        {
            InitializeComponent();
            this.dataSetConsumeSum1.Merge(ds);
        }
    }
}
