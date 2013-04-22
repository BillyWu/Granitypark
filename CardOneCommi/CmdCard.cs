using System;
using System.Collections.Generic;
using System.Text;
using Granity.communications;
using System.Diagnostics;
using System.Threading;

namespace Granity.CardOneCommi
{
    /// <summary>
    /// 发行卡片,读写卡片信息
    /// </summary>
    public class CmdCard:CmdProtocol
    {
        #region 内部变量
        
        /// <summary>
        /// 发行器通讯目标
        /// </summary>
        private CommiTarget targetwr = null;

        public CommiTarget Targetwr
        {
            get { return targetwr; }
        }
        /// <summary>
        /// 通讯管理器
        /// </summary>
        private CommiManager commiMgr = CommiManager.GlobalManager;
        /// <summary>
        /// 发行器站址
        /// </summary>
        private string addrst = "3";
        /// <summary>
        /// 卡片巡检状态:0/初始或读写卡信息,1/脱机巡检,2/联机巡检
        /// </summary>
        private int stateChecking = 0;
        /// <summary>
        /// 同步等待时刻,默认1000毫秒
        /// </summary>
        private int waitTime = 1000;
        /// <summary>
        /// 是否IC卡
        /// </summary>
        private bool isCardIC = true;
        /// <summary>
        /// 通讯状态
        /// </summary>
        private string stateresponse = "";
        /// <summary>
        /// 卡号缓存瞬时定时器
        /// </summary>
        private Timer tmIDcache = null;
        /// <summary>
        /// 读取当前是否是IC卡
        /// </summary>
        public bool IsCardIC
        {
            get { return isCardIC; }
        }
        /// <summary>
        /// 读取发行器站址,默认3
        /// </summary>
        public string StationNum
        {
            get { return this.addrst; }
        }
        /// <summary>
        /// 读取发行器通讯状态
        /// </summary>
        public string StateResponse
        {
            get { return this.stateresponse; }
        }
        #endregion

        #region 当前卡片信息

        private string cardid;
        private string cardserno;
        /// <summary>
        /// 读取当前卡片序列号
        /// </summary>
        public string CardID
        {
            get { return cardserno; }
        }

        /// <summary>
        /// 读取当前卡片短序列号,
        /// IC卡取高三字节反序数字,ID卡直接取低三字节
        /// </summary>
        public string CardSID
        {
            get
            {
                if (string.IsNullOrEmpty(cardid))
                    return "";
                long num = Convert.ToInt64(cardid);
                string s = Convert.ToString(num, 16).PadLeft(8, '0');
                if (isCardIC)
                    num = Convert.ToInt64(s.Substring(4, 2) + s.Substring(2, 2) + s.Substring(0, 2), 16);
                else
                    num = Convert.ToInt64(s.Substring(s.Length - 6, 6), 16);
                return Convert.ToString(num);
            }
        }
        private string cardnum;
        /// <summary>
        /// 读取当前卡片编号,ID卡与短序列号SID相同
        /// </summary>
        public string CardNum
        {
            get { return cardnum; }
        }

        #endregion

        /// <summary>
        /// 发行管理构造函数
        /// </summary>
        /// <param name="target">发行器通讯地址</param>
        /// <param name="stationNum">发行器站址</param>
        public CmdCard():base(false)
        {
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = TimeSpan.MaxValue;
            this.FailProAf = FailAftPro.Reconn;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(CmdCard_ResponseHandle);
            this.tmIDcache = new Timer(new TimerCallback(this.tm_Callback), null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 设置发行器
        /// </summary>
        /// <param name="target">发行器地址</param>
        /// <param name="stationNum">发行器站址</param>
        /// <param name="isCardIC">是否IC卡</param>
        /// <returns>是否成功设置发行器</returns>
        public bool SetTarget(CommiTarget target, int stationNum, bool isCardIC)
        {
            //重新设置则保持当前状态
            if (null != target && stationNum > 0 && stationNum < 255 && null!=this.targetwr)
                if (this.addrst == Convert.ToString(stationNum) && this.isCardIC == isCardIC
                    && ((null != target.SrvEndPoint && null != this.targetwr.SrvEndPoint && this.targetwr.SrvEndPoint.Port == target.SrvEndPoint.Port && this.targetwr.SrvEndPoint.Address.ToString() == target.SrvEndPoint.Address.ToString())
                    || (this.targetwr.PortName == target.PortName && this.targetwr.BaudRate == target.BaudRate)))
                {
                    if (!string.IsNullOrEmpty(this.cardid) || 0 == this.stateChecking)
                        return true;
                    this.reChecking(1);
                    return true;
                }
            this.TimeSendInv = new TimeSpan(24, 0, 0);
            this.stateChecking = 0;
            this.cardid = "";
            this.cardserno = "";
            this.cardnum = "";

            string tagdata = "@设备地址=" + this.addrst;
            //原发行器脱机
            string[,] cmds ={ { "卡务中心", "脱机" }, { "卡务中心", "蜂鸣" } };
            if (null != this.targetwr && !string.IsNullOrEmpty(this.addrst))
            {
                for (int i = 0; i < cmds.GetLength(0); i++)
                {
                    this.setCommand(cmds[i, 0], cmds[i, 1], tagdata);
                    this.ResetState();
                    this.commiMgr.SendCommand(this.targetwr, this);
                    this.EventWh.WaitOne(this.waitTime, false);
                }
                this.commiMgr.RemoveCommand(this.targetwr, this);
            }

            //置发行器脱机并蜂鸣提示
            this.isCardIC = isCardIC;
            this.targetwr = null;
            this.addrst = "3";
            if (null == target || stationNum < 1 || stationNum > 255)
                return false;
            this.targetwr = target;
            this.addrst = Convert.ToString(stationNum);
            target.setProtocol(Protocol.PTLCard);
            tagdata = "@设备地址=" + this.addrst;
            cmds = new string[,] { { "卡务中心", "联机" }, { "卡务中心", "防冲突" } };
            if (!isCardIC)
                cmds = new string[,] { { "卡务中心", "读卡" } };
            for (int i = 0; i < cmds.GetLength(0); i++)
            {
                this.setCommand(cmds[i, 0], cmds[i, 1], tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                    return false;
            }
            this.tmIDcache.Change(Timeout.Infinite, Timeout.Infinite);
            this.TimeSendInv = new TimeSpan(3000000);
            this.stateChecking = isCardIC ? 2 : 1;
            return true;
        }

        /// <summary>
        /// 蜂鸣提示,成功提示1声,失败提示3声
        /// </summary>
        /// <param name="isSuccess">是否成功提示</param>
        public void Buzz(bool isSuccess)
        {
            //蜂鸣提示:取不到卡号蜂鸣三次提示
            this.reChecking(0);
            int times = isSuccess ? 1 : 3;
            string tagdata = "@设备地址=" + this.addrst;
            for (int i = 0; i < times; i++)
            {
                this.setCommand("卡务中心", "蜂鸣", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (this.EventWh.WaitOne(this.waitTime, false))
                    Thread.Sleep(300);
            }
            this.reChecking(1);
        }

        /// <summary>
        /// 响应请求,巡检
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdCard_ResponseHandle(object sender, ResponseEventArgs e)
        {
            this.stateresponse = (null == e || !e.Success) ? "通讯中断" : "";
            if (null == sender || null == e || !e.Success || !(sender is CmdCard))
                return;
            CmdCard card = sender as CmdCard;
            if (0 == card.stateChecking)
                return;
            string tag = card.ResponseFormat;
            string cardid = basefun.valtag(tag, "{卡号}");
            string state = basefun.valtag(tag, "{状态}");
            if (card.cardid == cardid)
            {
                if (!string.IsNullOrEmpty(cardid))
                    card.tmIDcache.Change(3000, Timeout.Infinite);
                return;
            }
            if (string.IsNullOrEmpty(cardid) && card.isCardIC)
            {
                card.cardnum = "";
                card.cardserno = "";
                card.cardid = cardid;
                return;
            } if (string.IsNullOrEmpty(cardid))
                return;
            card.cardid = cardid;
            card.tmIDcache.Change(3000, Timeout.Infinite);
            //ID卡则卡号/卡编号相同,IC卡读取写入编号
            if (card.isCardIC)
            {
                //读取卡编号
                string data = card.ReadCardNum();
                card.cardnum = basefun.valtag(data, "{卡编号}");
            }else
                card.cardnum = card.CardSID;
            card.cardserno = cardid;
            if (card.isCardIC)
                card.Buzz(true);
        }

        /// <summary>
        /// 缓存卡号3秒,在非巡检状态时延后执行
        /// </summary>
        /// <param name="obj"></param>
        private void tm_Callback(object obj)
        {
            if (string.IsNullOrEmpty(this.cardid))
                return;
            if (0 == this.stateChecking)
            {
                this.tmIDcache.Change(3000, Timeout.Infinite);
                return;
            }
            this.cardid = "";
            this.cardnum = "";
            this.cardserno = "";
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="vali">一卡通协议认证指令名称</param>
        /// <param name="pwddefault">认证默认密码</param>
        /// <returns>响应结果</returns>
        private string validatePwd(string vali,string validefault)
        {
            if (null == this.targetwr || string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无发行器,@Success=false";
            if (0 != this.stateChecking)
                return "@{状态}=状态错误,@Success=false";
            string failtmsg = "@{状态}=通讯失败,@Success=false";
            this.TimeSendInv = new TimeSpan(24, 0, 0);
            this.ResetState();

            string tagdata = "@设备地址=" + this.addrst;
            tagdata = basefun.setvaltag(tagdata, "{卡号}", this.cardid);
            string[,] cmds = { { "卡务中心", "选择卡" }, { "一卡通", vali } };
            string[,] cmdbak = { { "卡务中心", "选择卡" }, { "一卡通", validefault } };
            string msg = "false";
            while ("true" != msg)
            {
                for (int i = 0; i < cmds.GetLength(0); i++)
                {
                    this.setCommand(cmds[i, 0], cmds[i, 1], tagdata);
                    this.ResetState();
                    this.commiMgr.SendCommand(this.targetwr, this);
                    if (!this.EventWh.WaitOne(this.waitTime, false))
                        return failtmsg;
                    msg = basefun.valtag(this.ResponseFormat, "Success");
                    if ("true" != msg)
                        break;
                }
                //密码认证失败,则使用默认密码
                if (cmdbak == cmds)
                    break;
                if ("true" != msg)
                    cmds = cmdbak;
            }
            return this.ResponseFormat;
        }
        /// <summary>
        /// 切换巡检状态：0/停止巡检，1/启动巡检
        /// </summary>
        /// <param name="stateChk">巡检类型,0停止巡检,其他值会内部自动识别恢复巡检</param>
        private void reChecking(int stateChk)
        {
            if (0 != stateChk)
                stateChk = this.isCardIC ? 2 : 1;
            CmdState st = this.CheckState();
            if (CmdState.Response == st || CmdState.Request == st || CmdState.ReqTimeout == st)
            {
                this.EventWh.Reset();
                this.EventWh.WaitOne(this.TimeOut, false);
            }
            this.TimeSendInv = new TimeSpan(24, 0, 0);
            this.commiMgr.ClearBuffer(this.targetwr);
            this.stateChecking = stateChk;
            string tagdata = "@设备地址=" + this.addrst;
            if (1 == stateChk)
            {
                this.setCommand("卡务中心", "脱机", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                this.EventWh.WaitOne(this.waitTime, false);
                this.setCommand("卡务中心", "读卡", tagdata);
            }
            else if (2 == stateChk)
                this.setCommand("卡务中心", "防冲突", tagdata);
            else
                this.setCommand(new byte[0]);
            this.ResetState();
            if (this.getCommand().Length < 1)
                this.ResponseDatetime = this.ResponseDatetime.AddMilliseconds(1);
            if (0 < stateChk)
            {
                this.TimeSendInv = new TimeSpan(3000000);
                this.commiMgr.SendCommand(this.targetwr, this);
            }
        }
        /// <summary>
        /// 暂停或恢复巡检
        /// </summary>
        /// <param name="ispause">是否暂停</param>
        public void Pause(bool ispause)
        {
            if (null == this.targetwr || string.IsNullOrEmpty(this.addrst))
                return;
            int station = Convert.ToInt32(this.addrst);
            if (station < 1 || station > 255)
                return;
            this.reChecking(ispause ? 0 : 1);
        }

        /// <summary>
        /// 发防冲突指令,再次确认卡号,不能锁定时返回提示信息
        /// </summary>
        /// <returns>返回失败提示信息</returns>
        private string lockCardNum()
        {
            string tagdata = "@设备地址=" + this.addrst;
            this.setCommand("卡务中心", "防冲突", tagdata);
            string cardid = "";
            for (int m = 0; m < 3; m++)
            {
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                    return "@{状态}=通讯失败,@Success=false";
                if (string.IsNullOrEmpty(this.ResponseFormat))
                    continue;
                cardid = basefun.valtag(this.ResponseFormat, "{卡号}");
                if (!string.IsNullOrEmpty(cardid))
                    break;
            }
            if (this.cardid != cardid && !string.IsNullOrEmpty(cardid))
                return "@{状态}=写过程不能更换IC卡,@Success=false";
            if (string.IsNullOrEmpty(cardid))
                this.commiMgr.ClearBuffer(this.targetwr);
            return "";
        }
        /// <summary>
        /// 可靠执行,可重复执行3次尝试
        /// </summary>
        /// <returns>返回响应结果,tag格式</returns>
        private string execReliable()
        {
            string msg = "";
            for (int m = 0; m < 3; m++)
            {
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                {
                    msg = "@{状态}=通讯失败,@Success=false";
                    break;
                }
                msg = this.ResponseFormat;
                if ("true" == basefun.valtag(msg, "Success"))
                    break;
            }
            return msg;
        }

        /// <summary>
        /// 读取认证卡片发行信息
        /// </summary>
        /// <returns>返回tag格式值,卡编号,停车场,门禁,消费,考勤,电梯,巡更,Success</returns>
        private string ReadCardNum()
        {
            this.reChecking(0);
            string msg = "";
            //string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            //if ("true" == basefun.valtag(msg, "Success"))
            string tagdata = "@设备地址=" + this.addrst;
            this.setCommand("一卡通", "读取发行", tagdata);
            for (int m = 0; m < 3; m++)
            {
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                {
                    msg = "@{状态}=通讯失败,@Success=false";
                    break;
                }
                msg = this.ResponseFormat;
                if ("true" == basefun.valtag(msg, "Success"))
                    break;
                if (string.IsNullOrEmpty(msg))
                    this.commiMgr.ClearBuffer(this.targetwr);
            }
            this.reChecking(1);
            return msg;
        }

        /// <summary>
        /// 读取卡消费时效信息
        /// </summary>
        /// <param name="cardID">卡片序列号</param>
        /// <returns>返回tag格式值:卡类,期限日期,历史金额,Success</returns>
        public string ReadEateryDtLimit()
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";

            this.reChecking(0);
            string msg = "";
            //string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                string tagdata = "@设备地址=" + this.addrst;
                this.setCommand("一卡通", "读取消费权限", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                    msg = "@{状态}=通讯失败,@Success=false";
                else
                    msg = this.ResponseFormat;
            }
            this.reChecking(1);
            return msg;
        }

        /// <summary>
        /// 读取卡消费记录
        /// </summary>
        /// <param name="cardID">卡片序列号</param>
        /// <returns>返回tag格式值:卡类,期限日期,历史金额,Success</returns>
        public string ReadEateryInfo()
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";

            this.reChecking(0);
            string msg = "";
            //string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                string tagdata = "@设备地址=" + this.addrst;
                this.setCommand("一卡通", "读取消费金额", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                    msg = "@{状态}=通讯失败,@Success=false";
                else
                    msg = this.ResponseFormat;
            }
            this.reChecking(1);
            return msg;
        }

        /// <summary>
        /// 读取卡停车场时效信息
        /// </summary>
        /// <param name="cardID">卡片序列号</param>
        /// <returns>返回tag格式值,通讯失败返回空:卡类,车型,期限日期,车牌</returns>
        public string ReadParkDtLimit()
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";

            this.reChecking(0);
            string msg = "";
            //string msg = this.validatePwd("认证大场写密码", "认证大场默认写密码");
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                string tagdata = "@设备地址=" + this.addrst;
                this.setCommand("一卡通", "读取大场权限", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                    msg = "@{状态}=通讯失败,@Success=false";
                else
                    msg = this.ResponseFormat;
            }
            this.reChecking(1);
            return msg;
        }

        /// <summary>
        /// 卡片停机
        /// </summary>
        /// <returns></returns>
        public void CardHalt()
        {
            this.TimeSendInv = new TimeSpan(24, 0, 0);
            this.ResetState();
            this.reChecking(0);
            string tagdata = "@设备地址=" + this.addrst;

            string[,] cmds ={ { "卡务中心", "卡片停机" }};
            for (int i = 0; i < cmds.GetLength(0); i++)
            {
                this.setCommand(cmds[i, 0], cmds[i, 1], tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                this.EventWh.WaitOne(this.waitTime, false);
            }
            this.reChecking(1);
        }
        /// <summary>
        /// 置发行器脱机停止巡检
        /// </summary>
        public void TrunOffLine()
        {
            this.reChecking(0);
            this.SetTarget(null, -1, false);
        }

        /// <summary>
        /// 写入卡号及发行
        /// </summary>
        /// <param name="cardnum">卡编号</param>
        /// <param name="isEatery">是否消费有效</param>
        /// <param name="isPark">是否停车场有效</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string WriteCardNum(string cardnum, bool isEatery, bool isPark)
        {
            return this.WriteCardNum(cardnum, isEatery, isPark, false);
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
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            this.reChecking(0);
            string msg = this.lockCardNum();
            if (!string.IsNullOrEmpty(msg))
            {
                this.reChecking(1);
                return msg;
            }
            //string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            string tagdata = "@设备地址=" + this.addrst;
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                tagdata = basefun.setvaltag(tagdata, "{卡编号}", cardnum);
                tagdata = basefun.setvaltag(tagdata, "{发行标志}.{消费}", isEatery ? "1" : "0");
                tagdata = basefun.setvaltag(tagdata, "{发行标志}.{停车场}", isPark ? "1" : "0");
                tagdata = basefun.setvaltag(tagdata, "{发行标志}.{门禁}", isDoor ? "1" : "0");
                tagdata = basefun.setvaltag(tagdata, "{发行标志}.{考勤}", isDoor ? "1" : "0");
                string[] mk ={ "{发行标志}.{电梯}", "{发行标志}.{巡更}" };
                for (int i = 0; i < mk.Length; i++)
                    tagdata = basefun.setvaltag(tagdata, mk[i], "0");
                this.setCommand("一卡通", "写入发行", tagdata);
                msg = this.execReliable();
            }
            this.reChecking(1);
            return msg;
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
        public string WriteEateryDtLimit(int cardType, DateTime dtStart, DateTime dtEnd, int level, string psw, double money,double subsidy)
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            int st = this.stateChecking;
            this.reChecking(0);
            string msg = this.lockCardNum();
            if (!string.IsNullOrEmpty(msg))
            {
                this.reChecking(1);
                return msg;
            }
            //string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            string tagdata = "@设备地址=" + this.addrst;
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                this.ResetState();
                //if ("true" == basefun.valtag(this.ResponseFormat, "Success"))
                //    msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
                //写入
                tagdata = basefun.setvaltag(tagdata, "{卡类}", Convert.ToString(cardType));
                tagdata = basefun.setvaltag(tagdata, "{启用日期}", dtStart.ToShortDateString());
                tagdata = basefun.setvaltag(tagdata, "{有效日期}", dtEnd.ToShortDateString());
                tagdata = basefun.setvaltag(tagdata, "{级别}", Convert.ToString(level));
                tagdata = basefun.setvaltag(tagdata, "{验证码}", psw);
                tagdata = basefun.setvaltag(tagdata, "{历史金额}", Convert.ToString(money));
                this.setCommand("一卡通", "写入消费权限", tagdata);
                msg = this.execReliable();
                if ("true" == basefun.valtag(msg, "Success"))
                {
                    string[,] vals ={ { "{累计消费额}", "0" }, { "{消费时间}", "1900-01-01" }, { "{日累计}", "0" }, { "{时段累计}", "0" }, { "{时段次数}", "0" } };
                    tagdata = "@设备地址=" + this.addrst;
                    for (int i = 0; i < vals.GetLength(0); i++)
                        tagdata = basefun.setvaltag(tagdata, vals[i, 0], vals[i, 1]);
                    tagdata = basefun.setvaltag(tagdata, "{余额}", Convert.ToString(money));
                    tagdata = basefun.setvaltag(tagdata, "{累计补助金额}", Convert.ToString(subsidy));
                    tagdata = basefun.setvaltag(tagdata, "{消费时间}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    this.setCommand("一卡通", "写入消费金额", tagdata);
                    msg = this.execReliable();
                }
            }
            this.reChecking(1);
            return msg;
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
        public string WriteEateryDtLimit(DateTime dtStart, DateTime dtEnd, double addMoney, double subsidy, bool isSubsidyAdd)
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            this.reChecking(0);
            string msg = this.lockCardNum();
            if (!string.IsNullOrEmpty(msg))
            {
                this.reChecking(1);
                return msg;
            }
            //string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            string tagdata = "@设备地址=" + this.addrst;
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                //读取发行信息
                string[,] info ={ { "{卡类}", "5" }, { "{启用日期}", "1900-01-01" }, { "{有效日期}", "1900-01-01" }, { "{级别}", "0" }, { "{验证码}", "666666" }, { "{历史金额}", "0" } };
                this.setCommand("一卡通", "读取消费权限", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (!this.EventWh.WaitOne(this.waitTime, false))
                {
                    this.reChecking(1);
                    return "@{状态}=通讯失败,@Success=false";
                }
                string tag = msg = this.ResponseFormat;
                if ("true" != basefun.valtag(msg, "Success"))
                {
                    this.reChecking(1);
                    return msg;
                }
                for (int i = 0; i < info.GetLength(0); i++)
                {
                    string val = basefun.valtag(tag, info[i, 0]);
                    if (!string.IsNullOrEmpty(val))
                        info[i, 1] = val;
                }
                if (dtEnd > dtStart)
                {
                    info[1, 1] = dtStart.ToShortDateString();
                    info[2, 1] = dtEnd.ToShortDateString();
                }
                info[5, 1] = Convert.ToString(addMoney + Convert.ToDouble(info[5, 1]));
                //累加余额及补助
                tag = msg = "";
                string[,] vals ={ { "{余额}", "0" }, { "{累计补助金额}","0" }, { "{累计消费额}", "0" }, { "{消费时间}", "1900-01-01" }, { "{日累计}", "0" }, { "{时段累计}", "0" }, { "{时段次数}", "0" } };
                this.setCommand("一卡通", "读取消费金额", tagdata);
                this.ResetState();
                this.commiMgr.SendCommand(this.targetwr, this);
                if (this.EventWh.WaitOne(this.waitTime, false))
                {
                    tag = msg = this.ResponseFormat;
                    for (int i = 0; i < vals.GetLength(0); i++)
                    {
                        string val = basefun.valtag(tag, vals[i, 0]);
                        if (!string.IsNullOrEmpty(val))
                            vals[i, 1] = val;
                    }
                    if (!string.IsNullOrEmpty(vals[0, 1]) && !string.IsNullOrEmpty(vals[1, 1]))
                    {
                        vals[0, 1] = Convert.ToString(addMoney + Convert.ToDouble(vals[0, 1]));
                        vals[1, 1] = Convert.ToString(subsidy + Convert.ToDouble(vals[1, 1]));
                    }
                }
                if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(vals[0, 1]) || string.IsNullOrEmpty(vals[1, 1]))
                {
                    vals[0, 1] = Convert.ToString(addMoney);
                    vals[1, 1] = Convert.ToString(subsidy);
                }
                if (!isSubsidyAdd) vals[1, 1] = Convert.ToString(subsidy);
                vals[3, 1] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //写入
                tagdata = "@设备地址=" + this.addrst;
                for (int i = 0; i < info.GetLength(0); i++)
                    tagdata = basefun.setvaltag(tagdata, info[i, 0], info[i, 1]);
                this.setCommand("一卡通", "写入消费权限", tagdata);
                msg = this.execReliable();
                if ("true" != basefun.valtag(msg, "Success"))
                {
                    this.reChecking(1);
                    return msg;
                }
                tagdata = "@设备地址=" + this.addrst;
                for (int i = 0; i < vals.GetLength(0); i++)
                    tagdata = basefun.setvaltag(tagdata, vals[i, 0], vals[i, 1]);
                this.setCommand("一卡通", "写入消费金额", tagdata);
                msg = this.execReliable();
            }
            this.reChecking(1);
            return msg;
        }

        /// <summary>
        /// 写入消费时效和充值金额
        /// </summary>
        /// <param name="dtStart">启用日期</param>
        /// <param name="dtEnd">有效日期</param>
        /// <param name="addMoney">充值金额</param>
        /// <returns></returns>
        public string WriteEateryDtLimit(DateTime dtStart, DateTime dtEnd, double addMoney)
        {
            return this.WriteEateryDtLimit(dtStart, dtEnd, addMoney, 0, true);
        }
        /// <summary>
        /// 写入消费时效和充值金额和补助金额
        /// </summary>
        /// <param name="dtStart">启用日期</param>
        /// <param name="dtEnd">有效日期</param>
        /// <param name="addMoney">充值金额</param>
        /// <param name="subsidy">补助金额,默认累加补助</param>
        /// <returns></returns>
        public string WriteEateryDtLimit(DateTime dtStart, DateTime dtEnd, double addMoney,double subsidy)
        {
            return this.WriteEateryDtLimit(dtStart, dtEnd, addMoney, subsidy, true);
        }
        /// <summary>
        /// 写入消费时效和充值金额
        /// </summary>
        /// <param name="addMoney">充值金额</param>
        /// <returns></returns>
        public string WriteEateryDtLimit(double addMoney)
        {
            return this.WriteEateryDtLimit(DateTime.MinValue, DateTime.MinValue, addMoney, 0, true);
        }
        /// <summary>
        /// 写入消费时效和充值金额和补助金额
        /// </summary>
        /// <param name="addMoney">充值金额</param>
        /// <param name="subsidy">补助金额,默认累加补助</param>
        /// <returns></returns>
        public string WriteEateryDtLimit(double addMoney, double subsidy)
        {
            return this.WriteEateryDtLimit(DateTime.MinValue, DateTime.MinValue, addMoney, subsidy, true);
        }
        /// <summary>
        /// 写入消费时效和充值金额和补助金额
        /// </summary>
        /// <param name="addMoney">充值金额</param>
        /// <param name="subsidy">补助金额</param>
        /// <param name="isSubsidyAdd">补助是否累加</param>
        /// <returns></returns>
        public string WriteEateryDtLimit(double addMoney, double subsidy, bool isSubsidyAdd)
        {
            return this.WriteEateryDtLimit(DateTime.MinValue, DateTime.MinValue, addMoney, subsidy, isSubsidyAdd);
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
        public string WriteParkDtLimit(int cardType, int cartype, DateTime dtStart, DateTime dtEnd, string carNo)
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            this.reChecking(0);
            string msg = this.lockCardNum();
            if (!string.IsNullOrEmpty(msg))
            {
                this.reChecking(1);
                return msg;
            }
            //string msg = this.validatePwd("认证大场写密码", "认证大场默认写密码");
            string tagdata = "@设备地址=" + this.addrst;
            //if ("true" == basefun.valtag(msg, "Success"))
            {
                tagdata = basefun.setvaltag(tagdata, "{卡类}.{卡类}", Convert.ToString(cardType));
                tagdata = basefun.setvaltag(tagdata, "{卡类}.{车型}", Convert.ToString(cartype));
                tagdata = basefun.setvaltag(tagdata, "{启用日期}", dtStart.ToShortDateString());
                tagdata = basefun.setvaltag(tagdata, "{有效日期}", dtEnd.ToShortDateString());
                tagdata = basefun.setvaltag(tagdata, "{车牌}", carNo);
                this.setCommand("一卡通", "写入大场权限", tagdata);
                msg = this.execReliable();
            }
            this.reChecking(1);
            return msg;
        }
        /// <summary>
        /// 清除指定区域数据
        /// </summary>
        /// <param name="area">卡片区域类型</param>
        /// <returns>返回发行结果tag格式：Success</returns>
        public string ClearData(CardArea area)
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            int st = this.stateChecking;
            this.reChecking(0);
            string[] valipwd = { "认证消费写密码", "认证消费默认写密码" };
            if (CardArea.Park == area)
                valipwd = new string[] { "认证大场写密码", "认证大场默认写密码" };

            string msg = "";
            //string msg = this.validatePwd(valipwd[0], valipwd[1]);
            string tagdata = "@设备地址=" + this.addrst;
            //if ("true" == basefun.valtag(msg, "Success"))
            int blockindex = CardArea.Eatery==area ? 20 : 28;
            for (int i = 0; i < 3;i++ )
            {
                tagdata = basefun.setvaltag(tagdata, "{块地址}", Convert.ToString(blockindex++));
                this.setCommand("卡务中心", "写数据", tagdata);
                msg = this.execReliable();
            }
            this.reChecking(1);
            return msg;
        }
        /// <summary>
        /// 设定消费密码
        /// </summary>
        /// <returns></returns>
        public string SetPwdEatery()
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            this.reChecking(0);
            string msg = this.validatePwd("认证消费写密码", "认证消费默认写密码");
            string tagdata = "@设备地址=" + this.addrst;
            if ("true" == basefun.valtag(msg, "Success"))
            {
                this.setCommand("一卡通", "设定消费密码", tagdata);
                msg = this.execReliable();
            }
            this.reChecking(1);
            return msg;
        }
        /// <summary>
        /// 设定大场密码
        /// </summary>
        /// <returns></returns>
        public string SetPwdPark()
        {
            if (string.IsNullOrEmpty(this.cardid))
                return "@{状态}=无卡或不能识别的卡,@Success=false";
            if (!this.isCardIC)
                return "@{状态}=ID卡模式不能写入,@Success=false";

            this.reChecking(0);
            string msg = this.validatePwd("认证大场写密码", "认证大场默认写密码");
            string tagdata = "@设备地址=" + this.addrst;
            if ("true" == basefun.valtag(msg, "Success"))
            {
                this.setCommand("一卡通", "设定大场密码", tagdata);
                msg = this.execReliable();
            }
            this.reChecking(1);
            return msg;
        }
    }

    /// <summary>
    /// 卡片区域分类：消费/停车场
    /// </summary>
    public enum CardArea
    {
        /// <summary>
        /// 消费区
        /// </summary>
        Eatery,
        /// <summary>
        /// 停车场
        /// </summary>
        Park
    }
}
