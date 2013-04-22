#region �汾˵��

/*
 * �������ݣ�   ���������
 *
 * ��    �ߣ�   ���ٲ�
 *
 * �� �� �ߣ�   ���ٲ�
 *
 * ��    �ڣ�   2010-07-27
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Granity.winTools
{
    /// <summary>
    /// ���������
    /// </summary>
    public class DataGridViewCalendarColumn : DataGridViewColumn
    {
        public DataGridViewCalendarColumn()
            : base(new CalendarCell())
        {
        }
        /// <summary>
        /// ��ȡ�����ô�����Ԫ���ģ��
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
                    throw new InvalidCastException("�������ڵ�Ԫ��CalendarCell");
                }
                base.CellTemplate = value;
            }
        }
    }
    /// <summary>
    /// ������ڵ�Ԫ��ģ��,Ĭ�ϳ����ڸ�ʽ
    /// </summary>
    public class CalendarCell : DataGridViewTextBoxCell
    {

        public CalendarCell()
            : base()
        {
            this.Style.Format = "D";
        }
        /// <summary>
        /// ���Ӳ���ʼ�����޵ı༭�ؼ�
        /// </summary>
        /// <param name="rowIndex">��������</param>
        /// <param name="initialFormattedValue">Ҫ�ڿؼ�����ʾ�ĳ�ʼֵ</param>
        /// <param name="dataGridViewCellStyle">ȷ�����޿ؼ������</param>
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
        /// ��ȡ��Ԫ��ļ��ޱ༭�ؼ�������
        /// </summary>
        public override Type EditType
        {
            get
            {
                return typeof(CalendarEditingControl);
            }
        }
        /// <summary>
        /// ��ȡ��Ԫ��ֵ����
        /// </summary>
        public override Type ValueType
        {
            get
            {
                return typeof(DateTime);
            }
        }
        /// <summary>
        /// ��ȡ��Ԫ��Ĭ��ֵ
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
    /// ��Ԫ��༭�ؼ�
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
        
        #region ʵ�ֽӿ� IDataGridViewEditingControl

        /// <summary>
        /// ��ȡ�����ð�����Ԫ��� DataGridView
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
        /// �ؼ�������
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
        /// ��ȡ�����ø�ʽ�����ֵ
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
        /// ��ȡ������һ��ֵ����ֵָʾ�༭�ؼ���ֵ�Ƿ�����ص�Ԫ���ֵ��ͬ
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
        /// ��ȡ������һ��ֵ����ֵָʾÿ��ֵ����ʱ���Ƿ���Ҫ���¶�λ��Ԫ�������
        /// </summary>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// ��ȡ�����ָ��λ�� DataGridView.EditingPanel �Ϸ�����λ�ڱ༭�ؼ��Ϸ�ʱ��ʹ�õĹ��
        /// </summary>
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        /// <summary>
        /// ��ȡ�ؼ���Textֵ
        /// </summary>
        /// <param name="context">����������</param>
        /// <returns></returns>
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }
        /// <summary>
        /// ������ʽ
        /// </summary>
        /// <param name="dataGridViewCellStyle">��Ԫ����ʽ</param>
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="key">���̼�</param>
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
        /// ׼����ǰѡ�еĵ�Ԫ���Խ��б༭
        /// </summary>
        /// <param name="selectAll">�Ƿ�ѡ��Ԫ���ȫ������/param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        #endregion

        /// <summary>
        /// �༭ֵ�ı䴥������¼�(��д����)
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
