using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;


namespace Granity.park.commStation
{
    public class commStation
    {
        int time = 1000;
        SerialPort sp = new SerialPort();
        public string port;
        public int baud;
        public bool init(string port, int baud)
        {
            if (sp.IsOpen)
            {
                sp.Close();
            }
            sp.PortName = port;
            sp.BaudRate = baud;
            if (!sp.IsOpen)
            {
                try
                {
                    sp.Open();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        public void closecom()
        {
            sp.Close();
        }
        public void opencom()
        {
            sp.ReadTimeout = time;
            sp.WriteTimeout = time;
            sp.Open();
        }
        /// <summary>
        /// ����ת����
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string Internal_code(string values)
        {
            string newValue = "";
            string st = string.Empty;
            byte[] array = System.Text.Encoding.Default.GetBytes(values);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] >= 128 && array[i] <= 247)
                {
                    st += string.Format("{0},{1},", array[i], array[i + 1]);
                    i++;
                }
                else
                {
                    st = st + string.Format("{0},", array[i]);
                }
            }
            string[] str2 = st.Split(',');
            foreach (string i in str2)
            {
                newValue = sValue10to16(i.ToString());
            }
            return newValue;
        }
        /// <summary>
        /// ����ת16����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sValueChineseto16(string str)
        {
            string values = "";
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            string[] strArr = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                strArr[i] = bytes[i].ToString("x");

                values += strArr[i].ToString();
            }
            return values;
        }
        /// <summary>
        /// 16����ת10����
        /// </summary>
        /// <param name="strvalue"></param>
        /// <param name="strstart"></param>
        /// <param name="strend"></param>
        /// <returns></returns>
        public string sValue16to10(string strvalue, int strstart, int strend)
        {
            return strvalue = Convert.ToString(Convert.ToInt64(strvalue.Substring(strstart, strend), 16));
        }
        /// <summary>
        /// BYTE[]ת����STRING��
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] getByteby16s(string str)
        {
            int iLen = str.Length / 2;
            byte[] arr = new byte[iLen];
            for (int i = 0; i < iLen; i++)
            {
                arr[i] = Convert.ToByte(Convert.ToInt32(str.Substring(2 * i, 2), 16));
            }
            return arr;
        }
        /// <summary>
        /// 16�����ַ���ת����CHAR�����飬��Ϊ�˵õ�ASCLL�롣
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public char[] getCharby16s(string str)
        {
            int iLen = str.Length;
            char[] buff = new char[iLen];
            for (int i = 0; i < iLen; i++)
            {
                buff[i] = Convert.ToChar(str.Substring(i, 1));
            }
            return buff;
        }
        /// <summary>
        /// char������ת����byte�����飬��Ϊ���͸����ӵ�����byte�����鷢��
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public byte[] getBytebychar(char[] buff)
        {
            int iLen = buff.Length;
            byte[] arr = new byte[iLen];
            for (int i = 0; i < iLen; i++)
            {
                arr[i] = (byte)buff[i];
            }
            return arr;
        }
        /// <summary>
        /// �Ȱ���16���ƴ洢���ַ���ת����ASCLL�루char�ͣ���ת����byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] getBytebystringtochar(string str)
        {
            int iLen = str.Length;
            char[] buff = new char[iLen + 2];
            buff[0] = (char)0x02;
            buff[iLen + 1] = (char)0x03;
            byte[] arr = new byte[iLen + 2];
            for (int i = 0; i < iLen; i++)
            {
                buff[i + 1] = Convert.ToChar(str.Substring(i, 1));
            }
            for (int j = 0; j < iLen + 2; j++)
            {
                arr[j] = (byte)buff[j];
            }
            return arr;
        }
        /// <summary>
        /// stringת����byte[]
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public string get16sbyByte(byte[] buf)
        {
            string data = "";
            int ilen = buf.Length;
            for (int i = 0; i < ilen; i++)
            {
                data = data + Convert.ToInt32(buf[i]).ToString("X").PadLeft(2, '0');
            }
            return data;
        }
        /// <summary>
        /// 10����ת16����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sValue10to16(string str)
        {
            if (string.IsNullOrEmpty(str)) str = "00";
            string s = Convert.ToInt32(str).ToString("X");
            if (s.Length % 2 != 0)
            {
                s = "0" + s;//����λ�򲹣������λ�ĸ�ʽ
            }
            return s;
        }

        /// <summary>
        /// XORУ��
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string CRCXOR(string cmd)
        {
            cmd = cmd.Trim();
            byte[] arrstr = new byte[cmd.Length / 2];
            for (int i = 0; i < arrstr.Length; i++)
                arrstr[i] = Convert.ToByte(Convert.ToInt16(cmd.Substring(i * 2, 2), 16));
            byte result = (byte)0;
            for (int index = 0; index < arrstr.Length; index++)
            {
                result ^= (byte)arrstr[index];
            }
            string s = result.ToString("X");
            if (s.Length == 1) s = "0" + s;
            return s;
        }
        /// <summary>
        /// ��ʽ���ַ���
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="com"></param>
        /// <param name="sLen"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string cmdString(string sn, string com, string sLen, string cmd)
        {
            sLen = initsLen(sLen);
            sn = sValue10to16(sn);
            //cmd = cmd10to16(cmd);
            string cmdstr = string.Format("{0}{1}{2}{3}", sn, com, sLen, cmd);
            string xor = CRCXOR(cmdstr);
            string cmdstring = string.Format("{0}{1}", cmdstr, xor);
            return cmdstring;
        }
        /// <summary>
        /// ������ʱ�ĸ�ʽ���ַ���
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="com"></param>
        /// <param name="sLen"></param>
        /// <returns></returns>
        public string cmdString(string sn, string com, string sLen)
        {
            sLen = initsLen(sLen);
            sn = sValue10to16(sn);
            string cmd = string.Format("{0}{1}{2}", sn, com, sLen);
            string xor = this.CRCXOR(cmd);
            string cmdstring = string.Format("{0}{1}", cmd, xor); ;
            return cmdstring;
        }
        /// <summary>
        /// 16���Ƶĳ���Len�ĸ�ʽ��
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string initsLen(string str)
        {
            string iLen = str.PadLeft(4, '0');
            return iLen;
        }
        public string initLen(string str)//10���Ƶĳ���Len�ĸ�ʽ�����ַ���������4���ֽ�
        {
            int L, H;
            L = Convert.ToInt32(str) % 256;//��λ
            H = Convert.ToInt32(str) / 256;//��λ
            string s = sValue10to16(L.ToString());
            s = sValue10to16(H.ToString()) + s;
            return s;
        }

        ///
        public string card10to16(string card)
        {
            string s = Convert.ToInt32(card).ToString("X").PadLeft(8, '0');
            return s;
        }
        public string cmd10to16(string cmd)//�ַ���10����ת16����
        {
            int iLen = cmd.Length / 2;
            string cmdstr = "";
            for (int i = 0; i < iLen; i++)
            {
                cmdstr += sValue10to16(cmd.Substring(2 * i, 2));//ÿ2λ��һ�����08��8��8������09 08 08 ����090808һ��ת
            }
            return cmdstr;
        }
        public string getstringbychar(char[] buf)
        {
            string cmd;
            cmd = "";
            for (int i = 0; i < buf.Length; i++)
            {
                cmd = cmd + Convert.ToString(buf[i]);
            }
            return cmd;
        }
        public string get16sbycharactor(string str)//�����纺�ֵ��ַ�����תΪchar����ת����int�ͷ��ɵõ����ֵ����룬��תΪstring�͡�
        {
            int iLen = str.Length;
            int sum = 0;
            string cmd = "";
            char[] buff = new char[iLen];
            for (int i = 0; i < iLen; i++)
            {
                buff[i] = Convert.ToChar(str.Substring(i, 1));
                sum = Convert.ToInt32(buff[i]);
                cmd = cmd + sum.ToString("X");
            }
            return cmd;
        }
        public string get16sbygb2312(string s)
        {
            string str = "";
            string high, low;
            for (int i = 0; i < s.Length; i++)
            {
                byte[] buf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(s.Substring(i, 1));
                high = Convert.ToInt32(buf[0]).ToString("X");
                if (buf.Length < 2)
                {
                    low = "";
                }
                else
                {
                    low = Convert.ToInt32(buf[1]).ToString("X");
                }
                str = str + high + low;
            }
            return str;
        }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="cmdstring"></param>
        /// <returns></returns>
        public string getcominfo(string cmdstring)
        {

            sp.WriteTimeout = time;
            sp.ReadTimeout = time;
            byte[] outbuf;
            outbuf = getBytebystringtochar(cmdstring);
            sp.Write(outbuf, 0, outbuf.Length);
            Thread.Sleep(200);
            string info = "";
            try
            {
                char[] buf = new char[512];
                sp.Read(buf, 0, buf.Length);
                info = getstringbychar(buf);

                return info;
            }
            catch (System.Exception ex)
            {

                info = ex.Message;
            }
            return info;
        }
        /// <summary>
        /// �豸��ʼ��
        /// </summary>
        /// <param name="sn">�豸��ַ</param>
        /// <returns></returns>
        public string System_Initialize(string sn)
        {
            string com = "01";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ���ؿ��Ʋ���
        /// </summary>
        /// <param name="sn">�豸��ַ</param>
        /// <param name="cmd">��������</param>
        /// <returns></returns>
        public string Modify_System_Parameter(string sn, string cmd)
        {
            string com = "02";
            string sLen = "40";
            string cmdstring = cmdString(sn, com, sLen, cmd.PadRight(128, '0'));
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }


        }
        /// <summary>
        /// ��ȡ���Ʋ���
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Read_System_Parameter(string sn)
        {
            string com = "03";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return getcominfo(cmdstring);
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }

        }
        /// <summary>
        /// ���ص�ǰϵͳʱ��
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Set_Time(string sn)
        {
            string com = "04";
            string sLen = "07";
            string time = DateTime.Now.ToString();
            string year = DateTime.Now.Year.ToString();
            year = year.Substring(2, 2);
            string month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            string day = DateTime.Now.Day.ToString().PadLeft(2, '0');
            string weekday = DateTime.Now.DayOfWeek.ToString().PadLeft(2, '0');
            if (weekday == "Monday")
            {
                weekday = "01";
            }
            else if (weekday == "Tuesday")
            {
                weekday = "02";
            }
            else if (weekday == "Wednesday")
            {
                weekday = "03";
            }
            else if (weekday == "Thursday")
            {
                weekday = "04";
            }
            else if (weekday == "Friday")
            {
                weekday = "05";
            }
            else if (weekday == "Saturday")
            {
                weekday = "06";
            }
            else if (weekday == "Sunday")
            {
                weekday = "07";
            }
            string hour = DateTime.Now.Hour.ToString().PadLeft(2, '0');
            string minute = DateTime.Now.Minute.ToString().PadLeft(2, '0');
            string second = DateTime.Now.Second.ToString().PadLeft(2, '0');
            string cmd;
            cmd = sValue10to16(year) + sValue10to16(month) + sValue10to16(day) + sValue10to16(weekday) + sValue10to16(hour) + sValue10to16(minute) + sValue10to16(second);

            string cmdstring = cmdString(sn, com, sLen, cmd);
            byte[] outbuf;
            outbuf = getBytebystringtochar(cmdstring);
            sp.Write(outbuf, 0, outbuf.Length);
            Thread.Sleep(200);
            char[] buf = new char[512];

            sp.Read(buf, 0, 512);

            string info = getstringbychar(buf);
            if (info.Contains("") && info.Contains(""))
            {
                info = info.Substring(3, 2);
                switch (info)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return info;
            }
            else
            {
                return info = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Initial_Fee_Parameter(string sn)
        {
            string com = "05";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// �����շѱ�׼
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Modify_Fee_Parameter(string sn, string cmd)
        {
            string com = "06";
            string sLen = "41";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);

            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ��ȡ�շѲ���
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Query_Fee_Parameter(string sn, string cmd)
        {
            string com = "07";
            string sLen = "01";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return getcominfo(cmdstring);
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// �����շѽ��
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Caculate_Fee(string sn, string cmd)
        {
            string com = "08";
            string sLen = "0E";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            string returninfo = "";
            udpinfo = getcominfo(cmdstring);
            returninfo = udpinfo;
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return returninfo;
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Initial_Card_List(string sn, string cmd)
        {
            string com = "09";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ���ذ�����
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Download_Card_List(string sn, string cmd)
        {
            string com = "0A";
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "88":
                        return "�����������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ��ѯID������
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Query_Card_List(string sn, string cmd)
        {
            string returninfo = "";
            string com = "0B";
            string sLen = "04";
            cmd = card10to16(cmd);
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            returninfo = udpinfo;
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return returninfo;
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "89":
                        return "�����ڵİ�������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ɾ��ID������
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Delete_Card_List(string sn, string cmd)
        {
            string com = "0C";
            string sLen = "04";
            cmd = card10to16(cmd);
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "89":
                        return "�����ڵİ�������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }

        /// <summary>
        /// ����ID������
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Download_Blacklist(string sn, string cmd)
        {
            string com = "5D";
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "84":
                        return "�����������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ɾ��������
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Resume_Card(string sn, string cmd)
        {
            string com = "5E";
            string sLen = "04";
            cmd = card10to16(cmd);
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "83":
                        return "�����ڵĺ�������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Query_Black_Card(string sn, string cmd)
        {
            string com = "5F";
            string sLen = "04";
            cmd = card10to16(cmd);
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "83":
                        return "�����ڵĺ�������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ���״̬
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Query_State(string sn, string cmd)
        {
            string returninfo = "";
            string com = "0D";
            string sLen = "02";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            returninfo = udpinfo;
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return returninfo;
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "83":
                        return "�����ڵĺ�������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Query_State(string sn)
        {
            string com = "0D";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// �ռ���ǰ��¼
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Get_Current_Recorder(string sn)
        {
            string returninfo = "";
            string com = "0E";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            returninfo = udpinfo;
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return udpinfo;
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";
                    case "82":
                        return "���ȴ���";
                    case "85":
                        return "���¼�¼����ʼ����û�м�¼��";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ��ȡ��ǰ��¼
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Get_Next_Reacorder(string sn)
        {
            string returninfo = "";
            string com = "0F";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            returninfo = udpinfo;
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return returninfo;
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";
                    case "82":
                        return "���ȴ���";
                    case "85":
                        return "���¼�¼����ʼ����û�м�¼��";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Issue_Card(string sn)
        {
            string com = "10";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Read_Card(string sn)
        {
            string returninfo = "";
            string com = "60";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            returninfo = udpinfo;
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return sValue16to10(returninfo, 9, 8);
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";
                    case "82":
                        return "���ȴ���";
                    case "86":
                        return "�޿���";
                    case "89":
                        return "δ��֤�Ŀ���";
                    case "8B":
                        return "ģ����ϣ�";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Remote_Control_Gate(string sn, string cmd)
        {
            string com = "12";
            string sLen = "01";
            cmd = sValue10to16(cmd);
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ������ʾ����Ϣ
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Download_Display(string sn, string cmd)
        {
            string com = "13";
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "":
                        return "�����ݣ�";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Remote_Reset(string sn)
        {
            string com = "14";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Get_Pictrue(string sn)
        {
            string com = "15";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Write_Device_Infomation(string sn, string cmd)
        {
            string com = "16";
            string sLen = "34";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ��ȡ�豸��Ϣ
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Read_Device_Infomation(string sn)
        {
            string com = "17";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return getcominfo(cmdstring);
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }

        }
        public string Write_Factory_Config(string sn, string cmd)
        {
            string com = "18";
            string sLen = "40";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Read_Factory_Config(string sn, string cmd)
        {
            string com = "19";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Remote_Start_Work(string sn, string cmd)
        {
            string com = "1A";
            string sLen = "04";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Remote_Beep(string sn)
        {
            string com = "1B";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ��ʼ����λ��Ϣ
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Initial_Private_Seat(string sn)
        {
            string com = "1C";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ���س�λ����Ϣ
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Download_Private_Using(string sn, string cmd)
        {
            string com = "1D";
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ��ѯ��λ����Ϣ
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Query_Private_Using(string sn, string cmd)
        {
            string com = "1E";
            string sLen = "02";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ������ʾ����������ʾ
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Host_Down_Cue(string sn, string cmd)
        {
            string com = "1F";
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Initial_Card_Indepent_Manage(string sn)
        {
            string com = "50";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Update_Card_Indepent_Manage(string sn, string cmd)
        {
            string com = "51";
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Query_Card_Indepent_Manage(string sn, string cmd)
        {
            string com = "52";
            string sLen = "04";
            cmd = card10to16(cmd);
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Enable_Dev_Offline_Work(string sn)
        {
            string com = "53";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Enable_Dev_Online_Work(string sn)
        {
            string com = "54";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ��ʼ��ʱ��
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string Initial_Common_duration(string sn)
        {
            string com = "55";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";


                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Modify_Common_duration(string sn, string cmd)
        {
            string com = "56";
            string sLen = "11";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);

            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "83":
                        return "�Ƿ����ݣ�";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Delete_Common_duration(string sn, string cmd)
        {
            string com = "57";
            string sLen = "01";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ��ȡʱ��
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Query_Common_duration(string sn, string cmd)
        {
            string com = "58";
            string sLen = "01";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return getcominfo(cmdstring);
                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }

        }
        public string Get_Current_Code(string sn)
        {
            string com = "59";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Get_Next_Code(string sn)
        {
            string com = "5A";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        /// <summary>
        /// ��������ͣ��
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Modi_Vech_Stop(string sn, string cmd)
        {
            string com = "5B";
            string sLen = "06";

            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            if (udpinfo.Contains("") && udpinfo.Contains(""))
            {
                udpinfo = udpinfo.Substring(3, 2);
                switch (udpinfo)
                {
                    case "00":
                        return "�����ɹ���";

                    case "80":
                        return "��֧�����";
                    case "81":
                        return "δ֪����";

                    case "82":
                        return "���ȴ���";
                    case "83":
                        return "�����ɹ���";
                    case "88":
                        return "�Ҳ�������";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "ͨѶʧ�ܣ����飡";
            }
        }
        public string Add_Vech_Stop(string sn, string cmd)
        {
            string com = "71";
            string sLen = "06";
            string card = card10to16(cmd.Substring(0, 8));
            string seat = Convert.ToString(Convert.ToInt32(cmd.Substring(6, 2)), 16).PadLeft(4, '0');
            cmd = card + seat;
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Read_Vech_Stop(string sn, string cmd)
        {
            string com = "72";
            string sLen = "06";
            string card = card10to16(cmd.Substring(0, 8));
            string seat = Convert.ToString(Convert.ToInt32(cmd.Substring(6, 2)), 16).PadLeft(4, '0');
            cmd = card + seat;
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Init_ParkStop_Inf(string sn, string cmd)
        {
            string com = "73";
            string sLen = "06";
            string card = card10to16(cmd.Substring(0, 8));
            string seat = Convert.ToString(Convert.ToInt32(cmd.Substring(6, 2)), 16).PadLeft(4, '0');
            cmd = card + seat;
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Send_LED_Display(string sn, int delay, string cmd)
        {
            string dtime, cmdstr;
            string com = "74";
            dtime = delay.ToString("X");
            cmdstr = get16sbygb2312(cmd);
            cmd = dtime + cmdstr;
            string sLen = (cmd.Length / 2).ToString("X");
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;

        }
        public string Update_SoftWare(string sn)
        {
            string com = "88";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Download_SoftWare(string sn, string cmd)
        {
            string com = "89";
            string sLen = "519";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string In_Check(string sn)
        {
            string com = "8A";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Update_SoftWare_Finish(string sn)
        {
            string com = "8B";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Set_Online(string sn)
        {
            string com = "20";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }


        public void Online(string sn)
        {
            string com = "20";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);

        }


        public string Set_Offline(string sn)
        {
            string com = "21";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Load_Key(string sn, string cmd)
        {
            string com = "22";
            string sLen = "08";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Request(string sn)
        {
            string com = "23";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Anticollision(string sn)
        {
            string com = "24";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Select_Card(string sn, string cmd)
        {
            string com = "25";
            string sLen = "04";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Authentication(string sn, string cmd)
        {
            string com = "26";
            string sLen = "02";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Lookfor_Card(string sn, string cmd)
        {
            string com = "27";
            string sLen = "01";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Read_Block(string sn, string cmd)
        {
            string com = "28";
            string sLen = "01";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Write_Block(string sn, string cmd)
        {
            string com = "29";
            string sLen = "11";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Set_Access_Control(string sn, string cmd)
        {
            string com = "2A";
            string sLen = "11";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Carf_Halt(string sn)
        {
            string com = "2B";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Authentication_Super_Card(string sn, string cmd)
        {
            string com = "2C";
            string sLen = "02";
            string cmdstring = cmdString(sn, com, sLen, cmd);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Read_Inpark_Info(string sn)
        {
            string com = "2D";
            string sLen = "00";
            string cmdstring = cmdString(sn, com, sLen);
            string udpinfo;
            udpinfo = getcominfo(cmdstring);
            return udpinfo;
        }
        public string Get_Result(string result)
        {
            string str = "";
            if (result == "00")
            {
                str = "�����ɹ�";
                return str;
            }
            else if (result == "80")
            {
                str = "��֧������";
                return str;
            }
            else if (result == "81")
            {
                str = "δ֪����";
                return str;
            }
            else if (result == "82")
            {
                str = "���ȴ���";
                return str;
            }
            else if (result == "85")
            {
                str = "���¼�¼����ʼ����û�м�¼";
                return str;
            }
            else
                return str;
        }
    }
}
