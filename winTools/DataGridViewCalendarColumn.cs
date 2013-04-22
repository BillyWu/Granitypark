#region 版本说明

/*
 * 功能内容：   表格日期列
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-07-27
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Granity.winTools
{
    /// <summary>
    /// 表格日期列
    /// </summary>
    public class DataGridViewCalendarColumn : DataGridViewColumn
    {
        public DataGridViewCalendarColumn()
            : base(new CalendarCell())
        {
        }
        /// <summary>
        /// 获取或设置创建单元格的模板
        /// </summary>
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
                {
                    throw new InvalidCastException("不是日期单元格CalendarCell");
                }
                base.CellTemplate = value;
            }
        }
    }
    /// <summary>
    /// 表格日期单元格模板,默认长日期格式
    /// </summary>
    public class CalendarCell : DataGridViewTextBoxCell
    {

        public CalendarCell()
            : base()
        {
            this.Style.Format = "D";
        }
        /// <summary>
        /// 附加并初始化寄宿的编辑控件
        /// </summary>
        /// <param name="rowIndex">行索引号</param>
        /// <param name="initialFormattedValue">要在控件中显示的初始值</param>
        /// <param name="dataGridViewCellStyle">确定寄宿控件的外观</param>
        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            CalendarEditingControl ctl =
                DataGridView.EditingControl as CalendarEditingControl;
            ctl.Value = (DateTime)this.Value;
        }
        /// <summary>
        /// 获取单元格的寄宿编辑控件的类型
        /// </summary>
        public override Type EditType
        {
            get
            {
                return typeof(CalendarEditingControl);
            }
        }
        /// <summary>
        /// 获取单元格值类型
        /// </summary>
        public override Type ValueType
        {
            get
            {
                return typeof(DateTime);
            }
        }
        /// <summary>
        /// 获取单元格默认值
        /// </summary>
        public override object DefaultNewRowValue
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
    /// <summary>
    /// 单元格编辑控件
    /// </summary>
    class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public CalendarEditingControl()
        {
            this.Format = DateTimePickerFormat.Short;
        }
        
        #region 实现接口 IDataGridViewEditingControl

        /// <summary>
        /// 获取或设置包含单元格的 DataGridView
        /// </summary>
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }
        /// <summary>
        /// 控件所在行
        /// </summary>
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }
        /// <summary>
        /// 获取或设置格式化后的值
        /// </summary>
        public object EditingControlFormattedValue
        {
            get
            {
                return this.Value.ToShortDateString();
            }
            set
            {
                String newValue = value as String;
                if (newValue != null)
                {
                    this.Value = DateTime.Parse(newValue);
                }
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示编辑控件的值是否与承载单元格的值不同
        /// </summary>
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示每当值更改时，是否需要重新定位单元格的内容
        /// </summary>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 获取当鼠标指针位于 DataGridView.EditingPanel 上方但不位于编辑控件上方时所使用的光标
        /// </summary>
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        /// <summary>
        /// 获取控件的Text值
        /// </summary>
        /// <param name="context">错误上下文</param>
        /// <returns></returns>
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }
        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="dataGridViewCellStyle">单元格样式</param>
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }
        /// <summary>
        /// 编辑键盘
        /// </summary>
        /// <param name="key">键盘键</param>
        /// <param name="dataGridViewWantsInputKey"></param>
        /// <returns></returns>
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 准备当前选中的单元格以进行编辑
        /// </summary>
        /// <param name="selectAll">是否选择单元格的全部内容/param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        #endregion

        /// <summary>
        /// 编辑值改变触发表格事件(重写基类)
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnValueChanged(EventArgs eventargs)
        {
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }

}
