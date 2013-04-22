using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Collections;
using System.Data.SqlClient;
using Granity.granityMgr.Eatery;
namespace Granity.granityMgr.Report
{
    public partial class viwReports : XtraReportsDemos.PreviewControl
    {
        public string StartTime = string.Empty;
        public string EndTime = string.Empty;
        public DataSet ds = new DataSet();
        public DataTable dt = new DataTable();
        public string ReportName;
        protected override XtraReport CreateReport()
        {
            try
            {
                switch (ReportName)
                {
                    case "刷卡报表":
                        dt.TableName = "刷卡记录";
                        Granity.granityMgr.Report.ReportOpenRecord OpenRecord = new ReportOpenRecord(dt);
                        OpenRecord.xrDateTime.Text = StartTime + "至" + EndTime;
                        return OpenRecord;
                        break;
                    case "消费统计":
                        granityMgr.Eatery.Reprot.ReportConsumeSum ConsumeSum = new Granity.granityMgr.Eatery.Reprot.ReportConsumeSum(ds);
                        ConsumeSum.xrDateTime.Text = StartTime + "至" + EndTime;
                        return ConsumeSum;
                        break;
                    case "消费机收入汇总":
                        granityMgr.Eatery.Reprot.ReportConsumeMacSum MacSum = new Granity.granityMgr.Eatery.Reprot.ReportConsumeMacSum(ds);
                        MacSum.xrDateTime.Text = StartTime + "至" + EndTime;
                        return MacSum;
                        break;
                    case "现金收入汇总":
                        granityMgr.Eatery.Reprot.ReportCashSum CashSumII = new Granity.granityMgr.Eatery.Reprot.ReportCashSum(ds);
                        CashSumII.xrDateTime.Text = StartTime + "至" + EndTime;
                        return CashSumII;
                        break;
                    case "考勤明细查询":
                        granityMgr.CheckWork.Report.ReportCheckWorklist CheckWorkList = new Granity.granityMgr.CheckWork.Report.ReportCheckWorklist(ds);
                        CheckWorkList.xrDateTime.Text = StartTime + "至" + EndTime;
                        return CheckWorkList;
                        break;
                    case "入场车流量统计表":
                        granityMgr.ParkMgr.Report.ReportIntVehicleNumberTotal IntVehicleNumberTotal = new Granity.granityMgr.ParkMgr.Report.ReportIntVehicleNumberTotal(ds);
                        IntVehicleNumberTotal.xrDateTime.Text = StartTime;
                        return IntVehicleNumberTotal;
                        break;

                    case "出场车流量统计":
                        granityMgr.ParkMgr.Report.ReportCarOutTotal CarOutTotal = new Granity.granityMgr.ParkMgr.Report.ReportCarOutTotal(ds);
                        CarOutTotal.xrDateTime.Text = StartTime;
                        return CarOutTotal;
                        break;
                    case "收费员收费统计":
                        granityMgr.ParkMgr.Report.ReportOperatorTotal OperatorTotal = new Granity.granityMgr.ParkMgr.Report.ReportOperatorTotal(ds);
                        OperatorTotal.xrDateTime.Text = StartTime;
                        return OperatorTotal;
                        break;

                    case "停车场收费统计":
                        granityMgr.ParkMgr.Report.ReportParkTotal ParkTotal = new Granity.granityMgr.ParkMgr.Report.ReportParkTotal(ds);
                        ParkTotal.xrDateTime.Text = StartTime;
                        return ParkTotal;
                        break;
                    default:
                        return null;
                        break;
                }
            }
            catch
            {
                return null;
            }
        }
        public override void Activate()
        {
            base.Activate();
        }
    }
}
