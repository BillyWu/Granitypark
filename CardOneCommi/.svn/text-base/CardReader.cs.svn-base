using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Granity.communications;
using System.Threading;

namespace Granity.CardOneCommi
{
    /// <summary>
    /// 读卡器或卡发行器B/S插件
    /// </summary>
    [Guid("818A30FB-8903-4cd7-BAE1-00BD5C5B52AC")]
    public partial class CardReader : UserControl, IObjectSafety
    {
        /// <summary>
        /// 读卡器实例
        /// </summary>
        CmdCard cmdCard = new CmdCard();
        /// <summary>
        /// 协议列表
        /// </summary>
        Dictionary<string, string[]> tplpm = new Dictionary<string, string[]>();

        /// <summary>
        /// 读卡器通讯参数目标
        /// </summary>
        CommiTarget target = null;
        /// <summary>
        /// 读卡器站址
        /// </summary>
        int station = 3;
        /// <summary>
        /// 读卡器模式是否IC
        /// </summary>
        bool isCardIC = false;

        /// <summary>
        /// 定时器,到计时3s检查对设备离线
        /// </summary>
        static System.Threading.Timer tmCache = new System.Threading.Timer(new TimerCallback(tm_Callback), null, Timeout.Infinite, Timeout.Infinite);
        /// <summary>
        /// 当前在线发行器设备
        /// </summary>
        static CmdCard GlCard=null;

        public CardReader()
        {
            InitializeComponent();
            GlCard = this.cmdCard;
        }

        #region IObjectSafety 成员

        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";
        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";
        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";
        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";
        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";

        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;
        private const int S_OK = 0;
        private const int E_FAIL = unchecked((int)0x80004005);
        private const int E_NOINTERFACE = unchecked((int)0x80004002);

        private bool _fSafeForScripting = true;
        private bool _fSafeForInitializing = true;
        
        /// <summary>
        /// 获取浏览器安全接口参数设置
        /// </summary>
        /// <param name="riid"></param>
        /// <param name="pdwSupportedOptions"></param>
        /// <param name="pdwEnabledOptions"></param>
        /// <returns></returns>
        public int GetInterfaceSafetyOptions(ref Guid riid, ref int pdwSupportedOptions, ref int pdwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForScripting == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForInitializing == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }
        /// <summary>
        /// 设置浏览器安全接口参数值
        /// </summary>
        /// <param name="riid"></param>
        /// <param name="dwOptionSetMask"></param>
        /// <param name="dwEnabledOptions"></param>
        /// <returns></returns>
        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            int Rslt = E_FAIL;
            string strGUID = riid.ToString("B");
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_CALLER) && (_fSafeForScripting == true))
                        Rslt = S_OK;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_DATA) && (_fSafeForInitializing == true))
                        Rslt = S_OK;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }

        #endregion

        #region 内部成员

        /// <summary>
        /// 检查是否完成协议初始化
        /// </summary>
        /// <returns>成功返回空,没有初始化时提示</returns>
        private string isInited()
        {
            string[,] cmds = new string[,] { { "卡务中心", "联机" }, { "卡务中心", "防冲突" }, { "卡务中心", "读卡" }, { "卡务中心", "卡片停机" },
                            { "卡务中心", "脱机" }, { "卡务中心", "蜂鸣" }, { "卡务中心", "选择卡" }, {"卡务中心", "防冲突"},
                            {"一卡通", "写入发行"}, {"一卡通", "读取发行"}, {"一卡通", "写入消费权限"}, {"一卡通", "读取消费权限"},
                            {"一卡通", "写入大场权限"}, {"一卡通", "读取大场权限"}, 
                            {"一卡通", "读取消费金额"}, {"一卡通", "写入消费金额"},
                            {"卡务中心", "写数据"} };

            if (null == this.target)
                return "没有设置通讯设备";
            for (int i = 0; i < cmds.GetLength(0); i++)
            {
                if (!this.tplpm.ContainsKey(cmds[i, 0] + ":" + cmds[i, 1]))
                    return "没有配置协议： "+cmds[i, 0] + ":" + cmds[i, 1];
            }
            return "";
        }

        /// <summary>
        /// 定时器回调函数,5s没有激发就中断巡检读卡器
        /// </summary>
        /// <param name="obj"></param>
        private static void tm_Callback(object obj)
        {
            if (null != GlCard)
                GlCard.TrunOffLine();
            CommiManager.GlobalManager.ClearCommand();
        }
        #endregion

        #region 公开可供脚本调用函数
        /// <summary>
        /// 设置通讯协议定义
        /// </summary>
        /// <param name="tpl">协议名称</param>
        /// <param name="cmd">指令名称</param>
        /// <param name="tagdevice">协议设备定义</param>
        /// <param name="taginput">协议输入参数定义</param>
        /// <param name="tagoutput">协议输出参数定义</param>
        /// <returns>是否设置成功</returns>
        public bool setTpl(string tpl, string cmd, string tagdevice, string taginput, string tagoutput)
        {
            bool rtn = this.cmdCard.setTpl(tpl, cmd, tagdevice, taginput, tagoutput);
            if (!rtn) return false;
            string key = tpl + ":" + cmd;
            if (this.tplpm.ContainsKey(key))
                this.tplpm[key] = new string[] { tagdevice, taginput, tagoutput };
            else
                this.tplpm.Add(key, new string[] { tagdevice, taginput, tagoutput });
            return true;
        }

        /// <summary>
        /// 设置通讯参数,port为空或station范围不正确则通讯置空
        /// </summary>
        /// <param name="port">通讯端口</param>
        /// <param name="baudRate">通讯波特率</param>
        /// <param name="station">通讯站址</param>
        /// <param name="isCardIC">是否IC卡</param>
        /// <returns>是否成功设置通讯参数</returns>
        public bool setTarget(string port, int baudRate, int station, bool isCardIC)
        {
            CommiManager.GlobalManager.ClearCommand();
            if (string.IsNullOrEmpty(port) || station < 1 || station > 255)
            {
                this.target = null;
                this.cmdCard.SetTarget(null, -1, false);
                return false;
            }
            try
            {
                this.Beat();
                this.target = new CommiTarget(port, baudRate);
                this.station = station;
                this.isCardIC = isCardIC;
                return this.cmdCard.SetTarget(target, station, isCardIC);
            }
            catch (Exception ex)
            {
                this.target = null;
                this.cmdCard.SetTarget(null, -1, false);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 客户端每3s内激发一次
        /// </summary>
        public void Beat()
        {
            if (null != this.target)
                this.cmdCard.SetTarget(target, station, isCardIC);
            tmCache.Change(3000, Timeout.Infinite);
        }

        /// <summary>
        /// 读取当前卡片序列号
        /// </summary>
        /// <returns></returns>
        public string getCardSN()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.CardID;
        }
        /// <summary>
        /// 读取当前卡片序列号
        /// </summary>
        /// <returns></returns>
        public string getCardSID()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.CardSID;
        }
        /// <summary>
        /// 读取当前卡片编号
        /// </summary>
        /// <returns></returns>
        public string getCardNum()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.CardNum;
        }
        /// <summary>
        /// 蜂鸣提示,成功提示1声,失败提示3声
        /// </summary>
        /// <param name="isSuccess">是否成功提示</param>
        public string Buzz(bool isSuccess)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            this.cmdCard.Buzz(isSuccess);
            return "";
        }
        /// <summary>
        /// 读取消费时效信息,返回tag格式数据
        /// </summary>
        /// <returns>没有初始化,则返回空,返回tag格式数据:卡类,期限日期,历史金额,Success</returns>
        public string ReadEateryDtLimit()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.ReadEateryDtLimit();
        }
        /// <summary>
        /// 读取卡消费记录
        /// </summary>
        /// <param name="cardID">卡片序列号</param>
        /// <returns>返回tag格式值:卡类,期限日期,历史金额,Success</returns>
        public string ReadEateryInfo()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.ReadEateryInfo();
        }
        /// <summary>
        /// 读取卡停车场时效信息
        /// </summary>
        /// <param name="cardID">卡片序列号</param>
        /// <returns>返回tag格式值,通讯失败返回空:卡类,车型,期限日期,车牌</returns>
        public string ReadParkDtLimit()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.ReadParkDtLimit();
        }
        /// <summary>
        /// 卡片停机
        /// </summary>
        /// <returns></returns>
        public void CardHalt()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return;
            this.cmdCard.CardHalt();
        }
        /// <summary>
        /// 置发行器脱机巡检读卡
        /// </summary>
        public void TrunOffLine()
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return;
            this.cmdCard.TrunOffLine();
        }
        /// <summary>
        /// 写入卡号及发行
        /// </summary>
        /// <param name="cardnum">卡编号</param>
        /// <param name="isEatery">是否消费有效</param>
        /// <param name="isPark">是否停车场有效</param>
        /// <param name="isDoor">是否门禁有效</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string WriteCardNum(string cardnum, bool isEatery, bool isPark, bool isDoor)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.WriteCardNum(cardnum, isEatery, isPark, isDoor);
        }
        /// <summary>
        /// 初始化消费时效和充值金额
        /// </summary>
        /// <param name="cardType">卡类型</param>
        /// <param name="dtStart">启用日期</param>
        /// <param name="dtEnd">有效日期</param>
        /// <param name="level">级别</param>
        /// <param name="psw">用户密码</param>
        /// <param name="money">初始化充值金额</param>
        /// <param name="subsidy">初始化补助金额</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string WriteEateryDtLimit(int cardType, string dtStart, string dtEnd, int level, string psw, double money, double subsidy)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            DateTime ds = Convert.ToDateTime(dtStart);
            DateTime de = Convert.ToDateTime(dtEnd);
            return this.cmdCard.WriteEateryDtLimit(cardType, ds, de, level, psw, money, subsidy);
        }
        /// <summary>
        /// 写入消费时效和充值金额和补助金额
        /// </summary>
        /// <param name="dtStart">启用日期</param>
        /// <param name="dtEnd">有效日期,日期区间不对则保留原日期区间</param>
        /// <param name="addMoney">充值金额</param>
        /// <param name="subsidy">补助金额</param>
        /// <param name="isSubsidyAdd">补助是否累加,false时原补助清0再补助</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string WriteEateryDtLimit2(string dtStart, string dtEnd, double addMoney, double subsidy, bool isSubsidyAdd)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            DateTime ds = Convert.ToDateTime(dtStart);
            DateTime de = Convert.ToDateTime(dtEnd);
            return this.cmdCard.WriteEateryDtLimit(ds, de, addMoney, subsidy);
        }

        /// <summary>
        /// 充钱或发补助
        /// </summary>
        /// <param name="addMoney">充值金额</param>
        /// <param name="subsidy">补助金额</param>
        /// <param name="isSubsidyAdd">补助是否累加</param>
        /// <returns></returns>
        public string WriteEateryMoney(double addMoney, double subsidy, bool isSubsidyAdd)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            return this.cmdCard.WriteEateryDtLimit(DateTime.MinValue, DateTime.MinValue, addMoney, subsidy, isSubsidyAdd);
        }
        /// <summary>
        /// 写入停车场时效
        /// </summary>
        /// <param name="cardType">卡类型</param>
        /// <param name="cartype">车型</param>
        /// <param name="dtStart">启用日期</param>
        /// <param name="dtEnd">有效日期</param>
        /// <param name="carNo">车牌</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string WriteParkDtLimit(int cardType, int cartype, string dtStart, string dtEnd, string carNo)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            DateTime ds = Convert.ToDateTime(dtStart);
            DateTime de = Convert.ToDateTime(dtEnd);
            return this.cmdCard.WriteParkDtLimit(cardType, cartype, ds, de, carNo);
        }
        /// <summary>
        /// 退卡,清除指定区域数据
        /// </summary>
        /// <param name="area">卡片区域类型:0/消费,1/停车场</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string ClearData(int areatype)
        {
            string msg = this.isInited();
            if (!string.IsNullOrEmpty(msg))
                return msg;
            CardArea area = CardArea.Eatery;
            switch (areatype)
            {
                case 0: area = CardArea.Eatery; break;
                case 1: area = CardArea.Park; break;
            }
            return this.cmdCard.ClearData(area);
        }
        
        #endregion
    }
}
