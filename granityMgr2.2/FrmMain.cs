using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.UserRight;
using Estar.Common.Tools;
using Granity.winTools;
using Granity.granityMgr.CardMgr;
using Granity.granityMgr.CheckWork;
using Granity.granityMgr.DoorMgr;
using Granity.granityMgr.Eatery;
using Granity.granityMgr.UserManager;
using DevExpress.XtraNavBar;
using Granity.communications;
using Granity.granityMgr.util;
using Granity.parkStation;
using DevExpress.XtraBars;
namespace Granity.granityMgr
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        private delegate void BarItemFirstItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e);
        User user = null;
        public FrmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载主页面，动态生成主菜单（门禁系统，考勤系统，停车场，消费系统）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            user = BindManager.getUser();
            if (null == user.DsDeptRight || user.DsDeptRight.Tables.Count < 1)
                return;
            DataTable tab = user.DsDeptRight.Tables[0];
            DataRow[] drs = tab.Select("ntype='操作集组' and level=2");
            int indexPic = -1;
            foreach (DataRow dr in drs)
            {
                if ("系统配置".Equals(dr["name"]))
                    continue;
                indexPic++;
                DevExpress.XtraBars.BarLargeButtonItem bar = new DevExpress.XtraBars.BarLargeButtonItem();
                bar.Caption = Convert.ToString(dr["text"]);
                bar.CaptionAlignment = DevExpress.XtraBars.BarItemCaptionAlignment.Right;
                bar.Name = basefun.valtag(Convert.ToString(dr["ntag"]), "name");
                bar.Tag = "@id=" + Convert.ToString(dr["ID"]) + "," + Convert.ToString(dr["ntag"]);
                bar.Id = this.barMgr.GetNewItemId();
                bar.LargeGlyph = this.imageBar.Images[indexPic];
                bar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(barMenu_Click);
                this.barMain.AddItem(bar);
                this.barMgr.Items.Add(bar);
            }
            if (this.barMain.ItemLinks.Count > 0)
            {
                BarItem item = this.barMain.ItemLinks[0].Item;
                item.Enabled = false;
                BarMenuClick(item);
            }
        }
        /// <summary>
        /// 用户点击不同主菜单想，动态生成不同的其在菜单项目 比如点击停车场，将生成停车场下面所有的菜单项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> 主菜单项 </param>
        void barMenu_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (null == sender || null == e.Item)
                return;
            //通过更改子项边框来区别当前选中的项
            foreach (DevExpress.XtraBars.BarButtonItemLink itemLinks in barMain.ItemLinks)
                itemLinks.Item.Enabled = true;
            e.Item.Enabled = false;
            string tag = Convert.ToString(e.Item.Tag);
            string item = basefun.valtag(tag, "name");
            if ("退出系统" == item)
            {
                this.Close();
                Application.Exit();
            }
            else
                this.BarMenuClick(e.Item);
        }

        /// <summary>
        ///  用户点击不同主菜单项，动态生成不同的其在菜单项目 比如点击停车场，将生成停车场下面所有的菜单项目
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id"></param>
        /// <param name="Name"></param>
        private void BarMenuClick(BarItem item)
        {
            if (null == item) return;
            string tag = Convert.ToString(item.Tag);
            string id = basefun.valtag(tag, "id");
            if (string.IsNullOrEmpty(id))
                return;
            foreach (Form fr in this.MdiChildren)
                fr.Close();
            this.navSystemFun.Groups.Clear();
            DataRow[] drs = user.DsDeptRight.Tables[0].Select(string.Format("PID='{0}' and hide='false'", id));
            if (null == drs || drs.Length < 1)
                return;
            int picgrp = -1;
            int picitem = -1;
            foreach (DataRow dr in drs)
            {
                picgrp++;
                string grpname = basefun.valtag(Convert.ToString(dr["ntag"]), "name");
                DevExpress.XtraNavBar.NavBarGroup group = new NavBarGroup();
                group.Appearance.Options.UseTextOptions = true;
                group.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                group.AppearanceBackground.Options.UseTextOptions = true;
                group.AppearanceBackground.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                group.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsList;
                group.Caption = grpname;
                group.Tag = Convert.ToString(dr["id"]);
                group.LargeImage = this.imageGroup.Images[picgrp];
                DataRow[] drSunnode = user.DsDeptRight.Tables[0].Select(string.Format("PID='{0}' and hide='false'", group.Tag));
                foreach (DataRow drv in drSunnode)
                {
                    picitem++;
                    NavBarItem navitem = this.navSystemFun.Items.Add();
                    navitem.Caption = Convert.ToString(drv["text"]);
                    navitem.Tag = Convert.ToString(drv["ntag"]);
                    navitem.LargeImage = this.imageItem.Images[picitem];
                    navitem.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(navBarItemLinkCliced);
                    group.ItemLinks.Add(navitem);
                }
                this.navSystemFun.Groups.Add(group);
            }
        }
        
        /// <summary>
        /// 在激活的Group里打开第一条记录的窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavSystemFun_ActiveGroupChanged(object sender, NavBarGroupEventArgs e)
        {
            if (null == sender || null == e || null == e.Group)
                return;
            foreach (Form fr in this.MdiChildren)
                fr.Close();
            if (e.Group.ItemLinks.Count > 0)
                this.OpenFrm(e.Group.ItemLinks[0]);
        }
        /// <summary>
        /// 点击菜单子项，根据点击的项目加载相应的用户控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navBarItemLinkCliced(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            OpenFrm(e.Link);
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="itemlink"></param>
        private void OpenFrm(NavBarItemLink itemlink)
        {
            if (null == itemlink||string.IsNullOrEmpty(itemlink.Caption))
                return;
            foreach (Form fr in this.MdiChildren)
            {
                fr.Close();
            }
            NameObjectList pstrans = new NameObjectList();
            pstrans["功能单元"] = itemlink.Caption;
            BindManager.setTransParam(pstrans);
            switch (itemlink.Caption)
            {
                //系统配置
                case "卡片参数配置":
                    FrmCardParam CardParam = new FrmCardParam();
                    CardParam.MdiParent = this;
                    CardParam.WindowState = FormWindowState.Maximized;
                    CardParam.Show();
                    break;
                //停车场
                case "入场记录和图像":
                    ParkMgr.FrmInQueryManage InQueryManage = new Granity.granityMgr.ParkMgr.FrmInQueryManage();
                    InQueryManage.MdiParent = this;
                    InQueryManage.WindowState = FormWindowState.Maximized;
                    InQueryManage.Show();
                    break;

                case "出场记录和图像":
                    FrmOutQueryManage OutQueryManage = new FrmOutQueryManage();
                    OutQueryManage.MdiParent = this;
                    OutQueryManage.WindowState = FormWindowState.Maximized;
                    OutQueryManage.Show();
                    break;

                case "场内停车记录":
                    ParkMgr.FrmQueryManage QueryManage = new Granity.granityMgr.ParkMgr.FrmQueryManage();
                    QueryManage.MdiParent = this;
                    QueryManage.WindowState = FormWindowState.Maximized;
                    QueryManage.Show();
                    break;
                case "操作员交接班记录":
                    FrmOper Oper = new FrmOper();
                    Oper.MdiParent = this;
                    Oper.WindowState = FormWindowState.Maximized;
                    Oper.Show();
                    break;
                case "停车场管理":
                    FrmParkStallSet ParkStallSet = new FrmParkStallSet();
                    ParkStallSet.MdiParent = this;
                    ParkStallSet.WindowState = FormWindowState.Maximized;
                    ParkStallSet.Show();
                    break;
                case "参数下载":
                    FrmDownParam EateryManage = new FrmDownParam();
                    EateryManage.MdiParent = this;
                    EateryManage.WindowState = FormWindowState.Maximized;
                    EateryManage.Show();
                    break;
                case "入场车流量统计表":
                    ParkMgr.Report.FrmIntVehicleNumberTotal VehicleNumberTotal = new Granity.granityMgr.ParkMgr.Report.FrmIntVehicleNumberTotal();
                    VehicleNumberTotal.MdiParent = this;
                    VehicleNumberTotal.WindowState = FormWindowState.Maximized;
                    VehicleNumberTotal.Show();
                    break;
                case "出场车流量统计表":
                    ParkMgr.Report.FrmCarOutTotal CarOutTotal = new Granity.granityMgr.ParkMgr.Report.FrmCarOutTotal();
                    CarOutTotal.MdiParent = this;
                    CarOutTotal.WindowState = FormWindowState.Maximized;
                    CarOutTotal.Show();
                    break;

                case "停车场收费统计表":
                    ParkMgr.Report.FrmParkTotal ParkTotal = new Granity.granityMgr.ParkMgr.Report.FrmParkTotal();
                    ParkTotal.MdiParent = this;
                    ParkTotal.WindowState = FormWindowState.Maximized;
                    ParkTotal.Show();
                    break;

                case "收费员收费统计表":
                    ParkMgr.Report.FrmOperatorTotal OperatorTotal = new Granity.granityMgr.ParkMgr.Report.FrmOperatorTotal();
                    OperatorTotal.MdiParent = this;
                    OperatorTotal.WindowState = FormWindowState.Maximized;
                    OperatorTotal.Show();
                    break;

                case "视频监控":
                    ParkMgr.FrmStationWatchingII StationWatching = new Granity.granityMgr.ParkMgr.FrmStationWatchingII();
                    //ParkMgr.FrmStatonWatchingStandard StationWatching = new Granity.granityMgr.ParkMgr.FrmStatonWatchingStandard();
                    StationWatching.WindowState = FormWindowState.Maximized;
                    StationWatching.Show();
                    break;
                case "权限设置":
                    ParkMgr.FrmRight ParkRight = new Granity.granityMgr.ParkMgr.FrmRight();
                    ParkRight.MdiParent = this;
                    ParkRight.WindowState = FormWindowState.Maximized;
                    ParkRight.Show();
                    break;
                //门禁系统
                case "有效时段":
                    BaseDicinfo.FrmEffectiveTimeList EffTime = new Granity.granityMgr.BaseDicinfo.FrmEffectiveTimeList();
                    EffTime.MdiParent = this;
                    EffTime.WindowState = FormWindowState.Maximized;
                    EffTime.Show();
                    break;
                case "设备资料":
                    Granity.granityMgr.BaseDicinfo.FrmDoorControlBaseinfo BaseInfo = new Granity.granityMgr.BaseDicinfo.FrmDoorControlBaseinfo();
                    BaseInfo.MdiParent = this;
                    BaseInfo.WindowState = FormWindowState.Maximized;
                    BaseInfo.Show();
                    break;
                case "开门记录查询":
                    Report.FrmOpenDoorRecord OpenRecord = new Report.FrmOpenDoorRecord();
                    OpenRecord.MdiParent = this;
                    OpenRecord.WindowState = FormWindowState.Maximized;
                    OpenRecord.Show();
                    break;
                case "门禁卡信息":
                    BaseDicinfo.FrmDoorCardInfo CardInfo = new Granity.granityMgr.BaseDicinfo.FrmDoorCardInfo();
                    CardInfo.MdiParent = this;
                    CardInfo.WindowState = FormWindowState.Maximized;
                    CardInfo.Show();
                    break;
                case "门禁监控":
                    FrmDoorMonitor DoorMonitor = new FrmDoorMonitor();
                    DoorMonitor.MdiParent = this;
                    DoorMonitor.WindowState = FormWindowState.Maximized;
                    DoorMonitor.Show();
                    break;
                case "门禁权限":
                    BaseDicinfo.FrmRight Right = new Granity.granityMgr.BaseDicinfo.FrmRight();
                    Right.MdiParent = this;
                    Right.WindowState = FormWindowState.Maximized;
                    Right.Show();
                    break;
                case "门用户":
                    BaseDicinfo.FrmDorUsers dorUser = new Granity.granityMgr.BaseDicinfo.FrmDorUsers();
                    dorUser.MdiParent = this;
                    dorUser.WindowState = FormWindowState.Maximized;
                    dorUser.Show();
                    break;
                case "门禁管理":
                    FrmDownParam DoorManager = new FrmDownParam();
                    DoorManager.MdiParent = this;
                    DoorManager.WindowState = FormWindowState.Maximized;
                    DoorManager.Show();
                    break;
                case "停车场卡信息":
                    ParkMgr.FrmCarParkInfo parkCard = new Granity.granityMgr.ParkMgr.FrmCarParkInfo();
                    parkCard.MdiParent = this;
                    parkCard.WindowState = FormWindowState.Maximized;
                    parkCard.Show();
                    break; 
                //卡务中心
                case "发行管理":
                    FrmCardMakeS CardMakeS = new FrmCardMakeS();
                    CardMakeS.MdiParent = this;
                    CardMakeS.WindowState = FormWindowState.Maximized;
                    CardMakeS.Show();
                    break;
                case "卡片管理":
                    FrmCardManager CardManager = new FrmCardManager();
                    CardManager.MdiParent = this;
                    CardManager.WindowState = FormWindowState.Maximized;
                    CardManager.Show();
                    break;
                case "发行器设置":
                    FrmDevSet DevSet = new FrmDevSet();
                    DevSet.MdiParent = this;
                    DevSet.WindowState = FormWindowState.Maximized;
                    DevSet.Show();
                    break;
                case "卡片发行记录":
                    FrmCardMakeRpt CardMakeRpt = new FrmCardMakeRpt();
                    CardMakeRpt.MdiParent = this;
                    CardMakeRpt.WindowState = FormWindowState.Maximized;
                    CardMakeRpt.Show();
                    break;
                case "卡片充值记录":
                    FrmCardPayRep CardPayRpt = new FrmCardPayRep();
                    CardPayRpt.MdiParent = this;
                    CardPayRpt.WindowState = FormWindowState.Maximized;
                    CardPayRpt.Show();
                    break;
                case "卡片延期记录":
                    FrmCardDelayRpt CardDelayRpt = new FrmCardDelayRpt();
                    CardDelayRpt.MdiParent = this;
                    CardDelayRpt.WindowState = FormWindowState.Maximized;
                    CardDelayRpt.Show();
                    break;
                case "退卡记录":
                    FrmCardQuitRtp CardQuitRpt = new FrmCardQuitRtp();
                    CardQuitRpt.MdiParent = this;
                    CardQuitRpt.WindowState = FormWindowState.Maximized;
                    CardQuitRpt.Show();
                    break;
                case "卡片挂失解挂记录":
                    FrmCardLostRpt CardLostRpt = new FrmCardLostRpt();
                    CardLostRpt.MdiParent = this;
                    CardLostRpt.WindowState = FormWindowState.Maximized;
                    CardLostRpt.Show();
                    break;
                //考勤系统
                case "部门排班查询":
                    FrmDeptScheduleRpt DeptScheduleRpt = new FrmDeptScheduleRpt();
                    DeptScheduleRpt.MdiParent = this;
                    DeptScheduleRpt.WindowState = FormWindowState.Maximized;
                    DeptScheduleRpt.Show();
                    break;
                case "员工打卡查询":
                    FrmHitCardRpt HitCardRpt = new FrmHitCardRpt();
                    HitCardRpt.MdiParent = this;
                    HitCardRpt.WindowState = FormWindowState.Maximized;
                    HitCardRpt.Show();
                    break;
                case "法定假日维护":
                    FrmHolidaySet HolidaySet = new FrmHolidaySet();
                    HolidaySet.MdiParent = this;
                    HolidaySet.WindowState = FormWindowState.Maximized;
                    HolidaySet.Show();
                    break;
                case "部门休息日维护":
                    FrmDeptRest DeptRest = new FrmDeptRest();
                    DeptRest.MdiParent = this;
                    DeptRest.WindowState = FormWindowState.Maximized;
                    DeptRest.Show();
                    break;
                case "班制定义":
                    FrmClass Class = new FrmClass();
                    Class.MdiParent = this;
                    Class.WindowState = FormWindowState.Maximized;
                    Class.Show();
                    break;
                case "部门班制":
                    FrmDeptClass DeptClass = new FrmDeptClass();
                    DeptClass.MdiParent = this;
                    DeptClass.WindowState = FormWindowState.Maximized;
                    DeptClass.Show();
                    break;
                case "考勤登记":
                    FrmEmployeeRegister Register = new FrmEmployeeRegister();
                    Register.MdiParent = this;
                    Register.WindowState = FormWindowState.Maximized;
                    Register.Show();
                    break;
                case "部门排班":
                    FrmMakeClass makeClass = new FrmMakeClass();
                    makeClass.MdiParent = this;
                    makeClass.WindowState = FormWindowState.Maximized;
                    makeClass.Show();
                    break;
                case "考勤明细查询":
                    Granity.granityMgr.CheckWork.Report.FrmCheckWorkList CheckWork = new Granity.granityMgr.CheckWork.Report.FrmCheckWorkList();
                    CheckWork.MdiParent = this;
                    CheckWork.WindowState = FormWindowState.Maximized;
                    CheckWork.Show();
                    break;
                //消费
                case "餐厅维护":
                    FrmRestaurant Restaurant = new FrmRestaurant();
                    Restaurant.MdiParent = this;
                    Restaurant.WindowState = FormWindowState.Maximized;
                    Restaurant.Show();
                    break;
                case "补助管理":
                    FrmAddMoney AddMoney = new FrmAddMoney();
                    AddMoney.MdiParent = this;
                    AddMoney.WindowState = FormWindowState.Maximized;
                    AddMoney.Show();
                    break;
                case "现金收入登记":
                    FrmCashRegisterII cashReg = new FrmCashRegisterII();
                    cashReg.MdiParent = this;
                    cashReg.WindowState = FormWindowState.Maximized;
                    cashReg.Show();
                    break;
                case "消费查询":
                    FrmConsumeRecord ConsumeRecord = new FrmConsumeRecord();
                    ConsumeRecord.MdiParent = this;
                    ConsumeRecord.WindowState = FormWindowState.Maximized;
                    ConsumeRecord.Show();
                    break;
                case "充值补助退款查询":
                    FrmAddReduceMoney AddReduceMoney = new FrmAddReduceMoney();
                    AddReduceMoney.MdiParent = this;
                    AddReduceMoney.WindowState = FormWindowState.Maximized;
                    AddReduceMoney.Show();
                    break;
                case "消费统计查询":
                    FrmConsumeSum ConsumeSum = new FrmConsumeSum();
                    ConsumeSum.MdiParent = this;
                    ConsumeSum.WindowState = FormWindowState.Maximized;
                    ConsumeSum.Show();
                    break;
                case "消费机收入汇总":
                    FrmCosumeMac ConsumeMac = new FrmCosumeMac();
                    ConsumeMac.MdiParent = this;
                    ConsumeMac.WindowState = FormWindowState.Maximized;
                    ConsumeMac.Show();
                    break;
                case "现金报表":
                    FrmCashSum CashSum = new FrmCashSum();
                    CashSum.MdiParent = this;
                    CashSum.WindowState = FormWindowState.Maximized;
                    CashSum.Show();
                    break;
                case "消费机管理":
                    FrmDownParam eateryManage = new FrmDownParam();
                    eateryManage.MdiParent = this;
                    eateryManage.WindowState = FormWindowState.Maximized;
                    eateryManage.Show();
                    break;
                case "消费监控":
                    FrmEateryMonitor frmeaterymtr = new FrmEateryMonitor();
                    frmeaterymtr.MdiParent = this;
                    frmeaterymtr.WindowState = FormWindowState.Maximized;
                    frmeaterymtr.Show();
                    break;
                // 用户管理
                case "用户信息":
                    FrmUserManager UserManager = new FrmUserManager();
                    UserManager.MdiParent = this;
                    UserManager.WindowState = FormWindowState.Maximized;
                    UserManager.Show();
                    break;
                case "公司员工":
                    FrmEmployees Employees = new FrmEmployees();
                    Employees.MdiParent = this;
                    Employees.WindowState = FormWindowState.Maximized;
                    Employees.Show();
                    break;
                case "角色定义":
                    FrmSysRoles role = new FrmSysRoles();
                    role.MdiParent = this;
                    role.WindowState = FormWindowState.Maximized;
                    role.Show();
                    break;
                case "修改密码":

                    FrmUpdatePass pass = new FrmUpdatePass();
                    pass.ShowDialog();
                    break;
                //case "用户信息":
                //    FrmSysUsers ps= new FrmSysUsers();
                //    ps.MdiParent = this;
                //    ps.WindowState = FormWindowState.Maximized;
                //    ps.Show();
                //    break;
            }

        }

        private void splitterControl1_LocationChanged(object sender, EventArgs e)
        {
            CheckNavPaneState();
        }

        bool isProcessingLayout = false;
        /// <summary>
        /// 控制NavPnl的宽度，可以由用户调大小
        /// </summary>
        protected void CheckNavPaneState()
        {
            if (this.navSystemFun.OptionsNavPane.IsAnimationInProgress) return;
            //if (!isProcessingLayout)
            //{
            //    try
            //    {
            //        if (this.navSystemFun.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Expanded)
            //        {
            //            if (this.navSystemFun.Width < this.navSystemFun.OptionsNavPane.ExpandedWidth)
            //            {
            //                isProcessingLayout = true;
            //                this.navSystemFun.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            //                return;
            //            }
            //        }
            //        if (this.navSystemFun.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
            //        {
            //            if (this.navSystemFun.Width > this.navSystemFun.CalcCollapsedPaneWidth())
            //            {
            //                isProcessingLayout = true;
            //                this.navSystemFun.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            //                return;
            //            }
            //        }
            //    }
            //    finally
            //    {
            //        isProcessingLayout = false;
            //    }
            //}
        }

        /// <summary>
        /// 关闭窗口关闭通讯线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            CommiServer.GlobalServer.Stop();
            CommiManager.GlobalManager.ClearCommand();
            ThreadManager.AbortAll();
            CommiManager.GlobalManager.ResetClient();
        }
    }
}