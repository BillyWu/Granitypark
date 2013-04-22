using System;
using System.Collections.Generic;
using System.Text;

namespace Granity.CardOneCommi
{
    /// <summary>
    /// 扩展服务类型
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// 更新设备参数,无附加数据：service='updateparam'
        /// </summary>
        UpdatePmDevice,
        /// <summary>
        /// 对设备启动巡检,附加数据：service='monitor',deviceid
        /// </summary>
        MonitorDevice,
        /// <summary>
        /// 忽略对设备的巡检,附加数据：service='halt',deviceid,deviceall='true'
        /// </summary>
        HaltDevice,
        /// <summary>
        /// 读取设备的巡检信息,附加数据：service='readinfo',deviceid
        /// </summary>
        ReadInfodev
    }
}
