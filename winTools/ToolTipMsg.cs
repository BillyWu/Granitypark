#region �汾˵��

/*
 * �������ݣ�   �ؼ��Ŀ����ʾ
 *
 * ��    �ߣ�   ���ٲ�
 *
 * �� �� �ߣ�   ���ٲ�
 *
 * ��    �ڣ�   2010-05-27
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Granity.winTools
{
    /// <summary>
    /// �ؼ��Ŀ����ʾ
    /// </summary>
    public class ToolTipMsg:ToolTip
    {
        /// <summary>
        /// �ؼ��Ŀ����ʾ
        /// </summary>
        public ToolTipMsg()
            : base()
        {
            this.StopTimer();
            this.ShowAlways = true;
        }
        /// <summary>
        /// �ؼ��Ŀ����ʾ
        /// </summary>
        /// <param name="ct">�󶨿ؼ�</param>
        public ToolTipMsg(IContainer ct)
            : base(ct)
        {
            this.StopTimer();
            this.ShowAlways = true;
        }
    }
}
