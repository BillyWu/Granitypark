#region 版本说明

/*
 * 功能内容：   基础命令,实现简单通讯方式
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-22
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Threading;

namespace Granity.communications
{
    /// <summary>
    /// 通讯命令简单实现(默认失败时可重试3次,不重复执行超限40s)
    /// </summary>
    public class CommandBase
    {

        private string cmdId;
        /// <summary>
        /// 获取或设置指令Id(键值)
        /// </summary>
        public string CmdId
        {
            get
            {
                return this.cmdId;
            }
            set
            {
                this.cmdId = value;
            }
        }

        private object tag;
        /// <summary>
        /// 指令实例附加扩展属性
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        #region 命令交换通讯响应控制参数

        private TimeSpan sendInv=new TimeSpan(0,0,-10);
        /// <summary>
        /// 发送时间间隔,小于0表示不持续发送(默认-10秒,不重发)
        /// </summary>
        [XmlIgnore]
        public TimeSpan TimeSendInv
        {
            get
            {
                return this.sendInv;
            }
            set
            {
                this.sendInv = value;
            }
        }

        private TimeSpan timeLimit = new TimeSpan(0, 0, 3);
        /// <summary>
        /// 超限时间间隔,TimeSpan.MaxValue则永不超限(默认3秒超限退出命令)
        /// </summary>
        [XmlIgnore]
        public TimeSpan TimeLimit
        {
            get
            {
                return this.timeLimit;
            }
            set
            {
                this.timeLimit = value;
            }
        }

        private TimeSpan timeFailLimit = new TimeSpan(600 * 10000);
        /// <summary>
        /// 连续无响应时间间隔,TimeSpan.MaxValue则永不失败退出(默认600ms超限失败)
        /// </summary>
        [XmlIgnore]
        public TimeSpan TimeFailLimit
        {
            get
            {
                return this.timeFailLimit;
            }
            set
            {
                this.timeFailLimit = value;
            }
        }

        private TimeSpan timeOut = new TimeSpan(200 * 10000);
        /// <summary>
        /// 超时时间间隔,小于0表示不需要接收响应(默认200ms可接收到响应)
        /// </summary>
        [XmlIgnore]
        public TimeSpan TimeOut
        {
            get
            {
                return this.timeOut;
            }
            set
            {
                this.timeOut = value;
            }
        }

        private DateTime responseDatetime = DateTime.Now;
        /// <summary>
        /// 响应指令时间,初始值与创建时间相同
        /// </summary>
        [XmlIgnore]
        public DateTime ResponseDatetime
        {
            get
            {
                return this.responseDatetime;
            }
            set
            {
                this.responseDatetime = value;
            }
        }

        private DateTime sendDatetime = DateTime.Now;
        /// <summary>
        /// 发送指令时间,初始值与创建时间相同
        /// </summary>
        [XmlIgnore]
        public DateTime SendDatetime
        {
            get
            {
                return this.sendDatetime;
            }
            set
            {
                this.sendDatetime = value;
            }
        }

        private DateTime firstDatetime = DateTime.Now;
        /// <summary>
        /// 第一次发送指令时间,初始值与创建时间相同
        /// </summary>
        public DateTime FirstDatetime
        {
            get { return firstDatetime; }
            set { firstDatetime = value; }
        }


        private DateTime createDatetime = DateTime.Now;
        /// <summary>
        /// 实例化命令时间
        /// </summary>
        [XmlIgnore]
        public DateTime CreateDatetime
        {
            get
            {
                return this.createDatetime;
            }
            set
            {
                this.createDatetime = value;
            }
        }

        #endregion

        /// <summary>
        /// 子命令
        /// </summary>
        private IList<CommandBase> subCmdList = new List<CommandBase>();
        /// <summary>
        /// 交互时子级命令。
        /// </summary>
        [XmlIgnore]
        public IList<CommandBase> SubCmdList
        {
            get
            {
                return this.subCmdList;
            }
            set
            {
                this.subCmdList = value;
            }
        }

        #region 响应同步事件及句柄

        private byte[] responseData = new byte[0];
        /// <summary>
        /// 读取当前响应字节数据
        /// </summary>
        public byte[] ResponseData
        {
            get { return responseData; }
        }

        private FailAftPro failafpro=FailAftPro.Exit;
        /// <summary>
        /// 读取或设置失败超限后处理类别:重连,退出,忽略(默认退出)
        /// </summary>
        public FailAftPro FailProAf
        {
            get { return this.failafpro; }
            set { this.failafpro = value; }
        }

        /// <summary>
        /// 检查是否为响应的指令源
        /// </summary>
        public HlCheckResponse IsResposeHandle;

        /// <summary>
        /// 响应事件
        /// </summary>
        public event EventHandler<ResponseEventArgs> ResponseHandle;

        private EventWaitHandle eventWh;
        /// <summary>
        /// 获取或设置多个响应之间线程同步事件句柄(由需要同步的主线程分发赋值)
        /// 有实例时在指令响应触发事件后自动触发同步事件
        /// </summary>
        [XmlIgnore]
        public EventWaitHandle EventWh
        {
            get { return this.eventWh; }
            set { this.eventWh = value; }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="arg"></param>
        public virtual void RaiseResponse(ResponseEventArgs arg)
        {
            EventHandler<ResponseEventArgs> handle = this.ResponseHandle;
            this.responseData = arg.Response;
            if (null != handle)
                handle(this, arg);
            if (null != this.eventWh)
                this.eventWh.Set();
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认方式构造实例,不初始化同步事件句柄,无指令ID
        /// </summary>
        public CommandBase()
        {
            DateTime dtNow = DateTime.Now.AddMilliseconds(-1);
            this.createDatetime = this.responseDatetime = this.firstDatetime = this.sendDatetime = dtNow;
        }

        /// <summary>
        /// 构造函数指定构造指令ID(指令ID是可更改的)
        /// </summary>
        /// <param name="id">指令ID</param>
        public CommandBase(string id)
        {
            this.cmdId = id;
            DateTime dtNow = DateTime.Now.AddMilliseconds(-1);
            this.createDatetime = this.responseDatetime = this.firstDatetime = this.sendDatetime = dtNow;
        }

        /// <summary>
        /// 构造函数,初始化同步事件句柄状态
        /// </summary>
        /// <param name="ewhState">同步事件初始状态</param>
        public CommandBase(bool ewhState)
        {
            this.eventWh = new ManualResetEvent(ewhState);
            DateTime dtNow = DateTime.Now.AddMilliseconds(-1);
            this.createDatetime = this.responseDatetime = this.firstDatetime = this.sendDatetime = dtNow;
        }

        /// <summary>
        /// 构造函数,初始化指令ID,初始化同步事件句柄状态
        /// </summary>
        /// <param name="id">指令ID,使用中可更改</param>
        /// <param name="ewhState">同步事件初始状态</param>
        public CommandBase(string id, bool ewhState)
        {
            this.cmdId = id;
            this.eventWh = new ManualResetEvent(ewhState);
            DateTime dtNow = DateTime.Now.AddMilliseconds(-1);
            this.createDatetime = this.responseDatetime = this.firstDatetime = this.sendDatetime = dtNow;
        }

        #endregion

        /// <summary>
        /// 检查指令当前状态
        /// </summary>
        /// <returns>返回当前指令状态</returns>
        public CmdState CheckState()
        {
            DateTime dtNow = DateTime.Now;
            //当前时间－响应时间           > 超限失败时间     <超限失败>
            //        首先: 有超限限制的,并且已经发送命令
            if (TimeSpan.MaxValue > this.timeFailLimit && this.timeOut > TimeSpan.Zero && this.sendDatetime > this.responseDatetime )
                if (this.sendDatetime > this.createDatetime && this.timeFailLimit < dtNow - this.responseDatetime)
                    return CmdState.TimeFailLimit;
            //当前时间－响应时间－间隔时间 > 超限周期时间     <完成>
            if (TimeSpan.MaxValue > this.timeLimit && this.sendDatetime > this.createDatetime)
            {
                //其次: 可持续发送的命令,进入再次发送命令周期,则再判断是否超限,不可持续发送的命令,直接判断
                TimeSpan inv = this.sendInv <= TimeSpan.Zero ? TimeSpan.Zero : this.sendInv;
                if (this.TimeLimit < dtNow - this.firstDatetime - inv)
                    return CmdState.Completed;
            }
            // 发送时间等于响应时间<首次发送>
            if (this.sendDatetime == this.responseDatetime)
                return CmdState.Request;
            //发送时间大于响应时间 且 当前时间-发送时间>超时时间 <超时重发>
            else if (this.sendDatetime > this.responseDatetime && this.timeOut > TimeSpan.Zero
                        && (dtNow - this.sendDatetime) > this.timeOut)
                return CmdState.ReqTimeout;
            //发送时间大于响应时间 且 超时时间 < Zero 且 SendInv <= Zero <单次无响应指令完成>
            else if (this.sendDatetime > this.responseDatetime && this.timeOut <= TimeSpan.Zero && this.sendInv <= TimeSpan.Zero)
                return CmdState.Completed;
            //发送时间大于响应时间 且 超时时间 < Zero 且 SendInv > Zero <无响应指令持续发送>
            else if (this.sendDatetime > this.responseDatetime && this.timeOut <= TimeSpan.Zero && (dtNow - this.sendDatetime) >= this.sendInv)
                return CmdState.Request;
            //发送时间大于响应时间 且 超时时间 < Zero 且 SendInv > Zero <无响应指令等待发送>
            else if (this.sendDatetime > this.responseDatetime && this.timeOut <= TimeSpan.Zero)
                return CmdState.WaitRepeat;
            //发送时间小于响应时间 且 当前时间-发送时间>间隔时间 <持续发送>
            else if (this.sendDatetime < this.responseDatetime && this.sendInv > TimeSpan.Zero && (dtNow - this.sendDatetime) >= this.sendInv)
                return CmdState.Request;
            //发送时间小于响应时间 且 间隔时间 > Zero <等待持续发送>
            else if (this.sendDatetime < this.responseDatetime && this.sendInv > TimeSpan.Zero)
                return CmdState.WaitRepeat;
            //发送时间小于响应时间 且 间隔时间 <= Zero <单次指令完成>
            else if (this.sendDatetime < this.responseDatetime && this.sendInv <= TimeSpan.Zero)
                return CmdState.Completed;

            return CmdState.Response;
        }

        /// <summary>
        /// 重置指令创建时间/发送时间/响应时间,状态是最初可请求
        /// </summary>
        /// <returns>返回最初状态可请求</returns>
        public CmdState ResetState()
        {
            this.createDatetime = this.responseDatetime = this.firstDatetime = this.sendDatetime = DateTime.Now;
            if (null != this.eventWh)
                this.eventWh.Reset();
            return CmdState.Request;
        }

        private byte[] cmdbuffer = new byte[0];
        /// <summary>
        /// 生成命令通讯字符串
        /// </summary>
        public byte[] getCommand()
        {
            if (null == this.cmdbuffer)
                this.cmdbuffer = new byte[0];
            return this.cmdbuffer;
        }


        #region 设置命令指令

        /// <summary>
        /// 设置通讯指令
        /// </summary>
        /// <param name="cmd">指令字节</param>
        /// <returns>成功返回true,指令为空则false不改变原来指令</returns>
        public bool setCommand(byte[] cmd)
        {
            if (null == cmd || CmdState.Response == this.CheckState())
                return false;
            if (null != this.eventWh)
                this.eventWh.Reset();
            this.cmdbuffer = cmd;
            this.responseData = new byte[0];
            return true;
        }
        /// <summary>
        /// 设置通讯指令,指定是否16进制字节数据,否则按Gb312编码
        /// </summary>
        /// <param name="cmd">指令字符串</param>
        /// <param name="isHEX">是否是16进制字符串</param>
        /// <returns>成功返回true,指令为空则false不改变原来指令</returns>
        public bool setCommand(string cmd, bool isHEX)
        {
            if (string.IsNullOrEmpty(cmd))
                return false;
            if (isHEX)
            {
                cmd = cmd.ToUpper().Replace(" ", "");
                if (0 != cmd.Length % 2)
                    cmd = "0" + cmd;
                int len = cmd.Length / 2;
                byte[] buffer = new byte[len];
                for (int i = 0; i < len; i++)
                    buffer[i] = (byte)Convert.ToInt16(cmd.Substring(i * 2, 2), 16);
                return this.setCommand(buffer);
            }
            return this.setCommand(cmd, Encoding.GetEncoding("GB2312"));
        }

        /// <summary>
        /// 设置通讯指令
        /// </summary>
        /// <param name="cmd">指令字符串,默认按照ASCII编码字节</param>
        /// <returns>成功返回true,指令为空则false不改变原来指令</returns>
        public bool setCommand(string cmd)
        {
            return this.setCommand(cmd, Encoding.ASCII);
        }

        /// <summary>
        /// 设置通讯指令,以指定编码格式编码
        /// </summary>
        /// <param name="cmd">指令字符串</param>
        /// <param name="encoding">指令字符串,默认按照GB2312编码字节</param>
        /// <returns>成功返回true,指令为空则false不改变原来指令</returns>
        public bool setCommand(string cmd, Encoding encoding)
        {
            if (string.IsNullOrEmpty(cmd))
                return false;
            if (null == encoding)
                encoding = Encoding.GetEncoding("GB2312");
            byte[] buffer = encoding.GetBytes(cmd);
            return this.setCommand(buffer);
        }

        /// <summary>
        /// 设置通讯指令,以指定编码方法编码字符串
        /// </summary>
        /// <param name="cmd">指令字符串</param>
        /// <param name="coding">编码方法</param>
        /// <returns>成功返回true,指令为空或编码不成功则false不改变原来指令</returns>
        public bool setCommand(string cmd, HlCoding coding)
        {
            if (null == coding)
                return this.setCommand(cmd);
            return this.setCommand(coding(cmd));
        }

        #endregion

        #region 解析字节

        /// <summary>
        /// 字节结果转换成GB2312编码字符串
        /// </summary>
        /// <param name="result">字节数据</param>
        /// <param name="encoding">编码格式,默认GB312</param>
        /// <returns>解析后的字符串</returns>
        public static string Parse(byte[] result, Encoding encoding)
        {
            if (null == result || result.Length < 1)
                return "";
            if (null == encoding)
                encoding = Encoding.GetEncoding("GB2312");
            return encoding.GetString(result);
        }
        /// <summary>
        /// 字节结果转换成字符串
        /// </summary>
        /// <param name="result">字节数据</param>
        /// <param name="isHEX">是否以16进制格式解析</param>
        /// <returns>解析后的字符串</returns>
        public static string Parse(byte[] result, bool isHEX)
        {
            if (null == result || result.Length < 1)
                return "";
            if (isHEX)
            {
                string str = "";
                for (int i = 0; i < result.Length; i++)
                    str += Convert.ToString(result[i], 16).PadLeft(2, '0').ToUpper();
                return str;
            }
            return Parse(result, Encoding.GetEncoding("GB2312"));
        }
        /// <summary>
        /// 字节结果转换成字符串,默认以GB2312编码
        /// </summary>
        /// <param name="result">字节数据</param>
        /// <returns>解析后的字符串</returns>
        public static string Parse(byte[] result)
        {
            return Parse(result, Encoding.GetEncoding("GB2312"));
        }

        /// <summary>
        /// 字节结果转换成字符串,使用指定解码方法解码
        /// </summary>
        /// <param name="result">字节数据</param>
        /// <param name="decoding">解码方法,为空则使用默认编码格式处理</param>
        /// <returns>解析后的字符串</returns>
        public static string Parse(byte[] result, HlDecoding decoding)
        {
            if (null == decoding)
                return Parse(result);
            return decoding(result);
        }

        #endregion



    }//end CmdCtrl

    /// <summary>
    /// 指令状态
    /// </summary>
    public enum CmdState
    {
        /// <summary>
        /// 可发送请求指令(首次/重复发送指令)
        /// </summary>
        Request,
        /// <summary>
        /// 可发送请求指令(超时重发)
        /// </summary>
        ReqTimeout,
        /// <summary>
        /// 指令已经发送,等待响应
        /// </summary>
        Response,
        /// <summary>
        /// 已响应,等待重复发送指令
        /// </summary>
        WaitRepeat,
        /// <summary>
        /// 指令已经完成(单次或重复执行结束)
        /// </summary>
        Completed,
        /// <summary>
        /// 指令超限失败(连续重试无响应),超限失败后关闭重联
        /// </summary>
        TimeFailLimit
    }
    /// <summary>
    /// 在指令失败超限后处理进程类别
    /// </summary>
    public enum FailAftPro
    {
        /// <summary>
        /// 重新连接
        /// </summary>
        Reconn,
        /// <summary>
        /// 指令退出
        /// </summary>
        Exit,
        /// <summary>
        /// 忽略,不做处理
        /// </summary>
        Ignor
    }

    /// <summary>
    /// 对字符串编码为字节
    /// </summary>
    /// <param name="content">需要编码的字符串内容</param>
    /// <returns>返回编码后的字节数据</returns>
    public delegate byte[] HlCoding(string content);

    /// <summary>
    /// 对字节解码为字符串
    /// </summary>
    /// <param name="content">需要解码的字节内容</param>
    /// <returns>返回解码后的字符串数据</returns>
    public delegate string HlDecoding(byte[] content);

    /// <summary>
    /// 判断响应字节是否与指令实例匹配为指令源
    /// </summary>
    /// <param name="cmd">实例实例</param>
    /// <param name="response">通讯响应字节</param>
    /// <returns>返回是否是指令源</returns>
    public delegate bool HlCheckResponse(CommandBase cmd, byte[] response);

    /// <summary>
    /// 一帧分多次接收数据时，连接完整帧加入目标字节数组列表中,不完整帧放入临时上下文字节数组
    /// 合并帧数据，加入列表数据，以供业务应用。
    /// </summary>
    /// <param name="buffer">读取当前缓存字节流</param>
    /// <param name="len">字节流数据长度</param>
    /// <param name="temp">临时字节数组，或上次接收的不完整字节数组，或承接下次字节流的字节数组</param>
    /// <param name="destlist">目标字节数组列表</param>
    /// <returns>返回目标字节数组列表,如果参数没有定义列表则创建</returns>
    public delegate IList<byte[]> HdlMergeList(byte[] buffer, int len, ref byte[] temp, IList<byte[]> destlist);

}
