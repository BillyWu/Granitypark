using System;
using System.Collections.Generic;
using System.Text;
using Granity.communications;

namespace Granity.commiServer
{
    /// <summary>
    /// 通讯设备
    /// </summary>
    public abstract class DeviceBase
    {
        /// <summary>
        /// 通讯目标
        /// </summary>
        protected CommiTarget target=null;
        /// <summary>
        /// 设备ID
        /// </summary>
        protected string devid = "";
        /// <summary>
        /// 读取设备ID
        /// </summary>
        public string DevID
        {
            get { return devid; }
        }
        private bool isPatrol = false;
        /// <summary>
        /// 是否属巡检设备
        /// </summary>
        public bool IsPatrol
        {
            get { return isPatrol; }
            set { isPatrol = value;}
        }
        private DateTime dtbeat = DateTime.MinValue;
        /// <summary>
        /// 读取或设置设备巡检时心跳时间
        /// </summary>
        public DateTime dtBeat
        {
            get { return this.dtbeat; }
            set { this.dtbeat = value; }
        }
        /// <summary>
        /// 设备站址
        /// </summary>
        protected int station = 0;

        private List<RecordDev> rows = new List<RecordDev>();
        /// <summary>
        /// 读取记录集
        /// </summary>
        public List<RecordDev> Rows
        {
            get { return this.rows; }
        }

        private RecordDev signaldev = new RecordDev();
        /// <summary>
        /// 读取信号状态
        /// </summary>
        public RecordDev Signal
        {
            get { return this.signaldev; }
        }

        private RecordDev alarmdev = new RecordDev();
        /// <summary>
        /// 读取告警状态
        /// </summary>
        public RecordDev Alarm
        {
            get { return this.alarmdev; }
        }

        /// <summary>
        /// 通讯管理器
        /// </summary>
        protected CommiManager commimgr = null;

        /// <summary>
        /// 响应事件，有采集新记录时触发
        /// </summary>
        public event EventHandler<DvRecordEventArgs> RecordHandle;
        /// <summary>
        /// 响应事件，有信号变位时触发
        /// </summary>
        public event EventHandler<DvSignalEventArgs> SignalHandle;
        /// <summary>
        /// 响应事件，有告警发生或结束时触发
        /// </summary>
        public event EventHandler<DvAlarmEventArgs> AlarmHandle;

        /// <summary>
        /// 启动采集
        /// </summary>
        public abstract void StartGather();
        /// <summary>
        /// 停止采集
        /// </summary>
        public abstract void StopGather();
        /// <summary>
        /// 直接采集一条数据,并自动存入数据库,返回获取数据的记录
        /// 没有记录时恢复巡检
        /// </summary>
        /// <param name="isfirst">是否首次提取,首次会补充采集当前记录以防漏采</param>
        public abstract string GatherData(bool isfirst);
        /// <summary>
        /// 设置通讯设备
        /// </summary>
        /// <param name="devid">设备ID</param>
        /// <param name="target">通讯目标</param>
        /// <param name="station">站址</param>
        public virtual void SetDevice(CommiManager mgr, CommiTarget target, string devid, int station)
        {
            if (null == mgr || string.IsNullOrEmpty(devid) || null == target || station < 1)
                return;
            this.commimgr = mgr;
            this.target = target;
            this.devid = devid;
            this.station = station;
        }
        /// <summary>
        /// 设置数据,保留2分钟内数据或50条记录,2分钟内有多余50条记录则保留最近100条记录
        /// </summary>
        /// <param name="tag"></param>
        public void AddRecord(string tag)
        {
            int min = 50, max = 100;
            TimeSpan ts = new TimeSpan(0, 2, 0);
            int count = this.rows.Count;
            if (count < min)
            {
                RecordDev rdnew = new RecordDev();
                rdnew.tag = tag;
                this.rows.Add(rdnew);
                return;
            }
            if (count > max)
            {
                this.rows.RemoveRange(0, count - max);
                count = this.rows.Count;
            }
            DateTime dtmi = DateTime.Now.AddMinutes(-2);
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                if (this.rows[i].dtReceive > dtmi)
                    break;
                index++;
            }
            if (index > 0)
                this.rows.RemoveRange(0, index);
            RecordDev rd = null;
            if (max == this.rows.Count)
            {
                rd = this.rows[0];
                this.rows.RemoveAt(0);
                rd.dtReceive = DateTime.Now;
            }
            else
                rd = new RecordDev();
            rd.tag = tag;
            this.rows.Add(rd);
        }
        /// <summary>
        /// 触发记录事件
        /// </summary>
        /// <param name="arg"></param>
        public virtual void RaiseRecord(DvRecordEventArgs arg)
        {
            EventHandler<DvRecordEventArgs> handle = this.RecordHandle;
            if (null == handle || null == arg)
                return;
            if (string.IsNullOrEmpty(arg.DeviceID) || string.IsNullOrEmpty(arg.TagInfo))
                return;
            handle(this, arg);
        }
        /// <summary>
        /// 触发信号变位事件
        /// </summary>
        /// <param name="arg"></param>
        public virtual void RaiseSignal(DvSignalEventArgs arg)
        {
            EventHandler<DvSignalEventArgs> handle = this.SignalHandle;
            if (null == handle || null == arg)
                return;
            if (string.IsNullOrEmpty(arg.DeviceID) || string.IsNullOrEmpty(arg.TagInfo) || string.IsNullOrEmpty(arg.TagSignal))
                return;
            handle(this, arg);
        }
        /// <summary>
        /// 触发告警发生或结束事件
        /// </summary>
        /// <param name="arg"></param>
        public virtual void RaiseAlarm(DvAlarmEventArgs arg)
        {
            EventHandler<DvAlarmEventArgs> handle = this.AlarmHandle;
            if (null == handle || null == arg)
                return;
            if (string.IsNullOrEmpty(arg.DeviceID) || string.IsNullOrEmpty(arg.TagInfo) || string.IsNullOrEmpty(arg.TagAlarm))
                return;
            handle(this, arg);
        }
    }

    /// <summary>
    /// 设备接收数据记录
    /// </summary>
    public class RecordDev
    {
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime dtReceive = DateTime.Now;
        /// <summary>
        /// 接收数据,可以是状态,告警或数据记录,tag格式
        /// </summary>
        public string tag = "";
    }
    /// <summary>
    /// 设备采集响应记录
    /// </summary>
    public class DvRecordEventArgs : EventArgs
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID;
        /// <summary>
        /// 设备站址
        /// </summary>
        public int Station;
        /// <summary>
        /// 记录的设备时间
        /// </summary>
        public DateTime DtDevice = DateTime.Now;
        /// <summary>
        /// 记录内容,tag格式数据
        /// </summary>
        public string TagInfo;
        /// <summary>
        /// 设备采集记录参数构造函数,初始化参数值
        /// </summary>
        /// <param name="deviceid">设备ID</param>
        /// <param name="station">设备站址</param>
        /// <param name="dt">设备记录时间</param>
        /// <param name="tag">记录内容tag格式信息</param>
        public DvRecordEventArgs(string deviceid, int station, DateTime dt, string tag):base()
        {
            this.DeviceID = deviceid;
            this.Station = station;
            this.DtDevice = dt;
            this.TagInfo = tag;
        }
    }

    /// <summary>
    /// 设备采集响应信号
    /// </summary>
    public class DvSignalEventArgs : EventArgs
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID;
        /// <summary>
        /// 设备站址
        /// </summary>
        public int Station;
        /// <summary>
        /// 记录的设备时间
        /// </summary>
        public DateTime DtDevice = DateTime.Now;
        /// <summary>
        /// 信号状态全信息,tag格式数据
        /// </summary>
        public string TagInfo;
        /// <summary>
        /// 信号变位
        /// </summary>
        public string TagSignal;
        /// <summary>
        /// 设备采集记录参数构造函数,初始化参数值
        /// </summary>
        /// <param name="deviceid">设备ID</param>
        /// <param name="station">设备站址</param>
        /// <param name="dt">设备记录时间</param>
        /// <param name="tag">信号状态信息tag格式值</param>
        public DvSignalEventArgs(string deviceid, int station, DateTime dt, string tag) : base()
        {
            this.DeviceID = deviceid;
            this.Station = station;
            this.DtDevice = dt;
            this.TagInfo = tag;
        }
    }
    /// <summary>
    /// 设备巡检告警状态
    /// </summary>
    public class DvAlarmEventArgs : EventArgs
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID;
        /// <summary>
        /// 设备站址
        /// </summary>
        public int Station;
        /// <summary>
        /// 记录的设备时间
        /// </summary>
        public DateTime DtDevice = DateTime.Now;
        /// <summary>
        /// 告警状态全信息,tag格式数据
        /// </summary>
        public string TagInfo;
        /// <summary>
        /// 告警发生或结束
        /// </summary>
        public string TagAlarm;
        /// <summary>
        /// 设备采集记录参数构造函数,初始化参数值
        /// </summary>
        /// <param name="deviceid">设备ID</param>
        /// <param name="station">设备站址</param>
        /// <param name="dt">设备记录时间</param>
        /// <param name="tag">告警状态信息tag格式值</param>
        public DvAlarmEventArgs(string deviceid, int station, DateTime dt, string tag) : base()
        {
            this.DeviceID = deviceid;
            this.Station = station;
            this.DtDevice = dt;
            this.TagInfo = tag;
        }
    }
}
