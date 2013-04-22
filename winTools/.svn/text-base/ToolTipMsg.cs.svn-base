#region 版本说明

/*
 * 功能内容：   控件的快捷提示
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-27
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
    /// 控件的快捷提示
    /// </summary>
    public class ToolTipMsg:ToolTip
    {
        /// <summary>
        /// 控件的快捷提示
        /// </summary>
        public ToolTipMsg()
            : base()
        {
            this.StopTimer();
            this.ShowAlways = true;
        }
        /// <summary>
        /// 控件的快捷提示
        /// </summary>
        /// <param name="ct">绑定控件</param>
        public ToolTipMsg(IContainer ct)
            : base(ct)
        {
            this.StopTimer();
            this.ShowAlways = true;
        }
    }
}
