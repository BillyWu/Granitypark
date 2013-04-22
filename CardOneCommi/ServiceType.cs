using System;
using System.Collections.Generic;
using System.Text;

namespace Granity.CardOneCommi
{
    /// <summary>
    /// ��չ��������
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// �����豸����,�޸������ݣ�service='updateparam'
        /// </summary>
        UpdatePmDevice,
        /// <summary>
        /// ���豸����Ѳ��,�������ݣ�service='monitor',deviceid
        /// </summary>
        MonitorDevice,
        /// <summary>
        /// ���Զ��豸��Ѳ��,�������ݣ�service='halt',deviceid,deviceall='true'
        /// </summary>
        HaltDevice,
        /// <summary>
        /// ��ȡ�豸��Ѳ����Ϣ,�������ݣ�service='readinfo',deviceid
        /// </summary>
        ReadInfodev
    }
}
