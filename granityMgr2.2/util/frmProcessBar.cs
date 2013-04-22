using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Granity.granityMgr.util
{
    public partial class frmProcessBar : Form
    {
        /// <summary>
        /// 读取或设置进程条位置,100为满
        /// </summary>
        public int Position
        {
            get { return this.prb.Value; }
            set
            {
                if (value > this.prb.Maximum)
                    this.prb.Value = this.prb.Maximum;
                else if (value < this.prb.Minimum)
                    this.prb.Value = this.prb.Minimum;
                else
                    this.prb.Value = value;
            }
        }

        public frmProcessBar()
        {
            InitializeComponent();
        }
    }
}