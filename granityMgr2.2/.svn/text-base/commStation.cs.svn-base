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
        /// 汉字转内码
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
        /// 汉字转16进制
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
        /// 16进制转10进制
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
        /// BYTE[]转换成STRING型
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
        /// 16进制字符串转换成CHAR型数组，是为了得到ASCLL码。
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
        /// char型数组转换成byte型数组，因为发送给板子的是以byte型数组发送
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
        /// 先吧以16进制存储的字符串转换成ASCLL码（char型）再转换成byte
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
        /// string转换成byte[]
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
        /// 10进制转16进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sValue10to16(string str)
        {
            if (string.IsNullOrEmpty(str)) str = "00";
            string s = Convert.ToInt32(str).ToString("X");
            if (s.Length % 2 != 0)
            {
                s = "0" + s;//不到位则补，如则变位的格式
            }
            return s;
        }

        /// <summary>
        /// XOR校验
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
        /// 格式化字符串
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
        /// 无数据时的格式化字符串
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
        /// 16进制的长度Len的格式化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string initsLen(string str)
        {
            string iLen = str.PadLeft(4, '0');
            return iLen;
        }
        public string initLen(string str)//10进制的长度Len的格式化，字符串长度是4个字节
        {
            int L, H;
            L = Convert.ToInt32(str) % 256;//高位
            H = Convert.ToInt32(str) / 256;//低位
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
        public string cmd10to16(string cmd)//字符串10进制转16进制
        {
            int iLen = cmd.Length / 2;
            string cmdstr = "";
            for (int i = 0; i < iLen; i++)
            {
                cmdstr += sValue10to16(cmd.Substring(2 * i, 2));//每2位是一组比如08年8月8日则是09 08 08 不能090808一起转
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
        public string get16sbycharactor(string str)//把诸如汉字等字符串先转为char型在转换成int型方可得到汉字的内码，在转为string型。
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
        /// 返回信息
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
        /// 设备初始化
        /// </summary>
        /// <param name="sn">设备地址</param>
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 下载控制参数
        /// </summary>
        /// <param name="sn">设备地址</param>
        /// <param name="cmd">命令数据</param>
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }


        }
        /// <summary>
        /// 读取控制参数
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }

        }
        /// <summary>
        /// 下载当前系统时间
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return info;
            }
            else
            {
                return info = "通讯失败，请检查！";
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
        /// 下载收费标准
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 读取收费参数
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 计算收费金额
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 下载白名单
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
                        return "成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "88":
                        return "白名单溢出！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 查询ID白名单
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "89":
                        return "不存在的白名单！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 删除ID白名单
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
                        return "成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "89":
                        return "不存在的白名单！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }

        /// <summary>
        /// 下载ID黑名单
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
                        return "成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "84":
                        return "白名单溢出！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 删除黑名单
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
                        return "成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "83":
                        return "不存在的黑名单！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }


        /// <summary>
        /// 读取黑名单
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
                        return "成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "83":
                        return "不存在的黑名单！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 检测状态
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "83":
                        return "不存在的黑名单！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 收集当前记录
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";
                    case "82":
                        return "长度错误！";
                    case "85":
                        return "无新记录，初始化后没有记录！";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 收取当前记录
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";
                    case "82":
                        return "长度错误！";
                    case "85":
                        return "无新记录，初始化后没有记录！";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 读卡
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";
                    case "82":
                        return "长度错误！";
                    case "86":
                        return "无卡！";
                    case "89":
                        return "未认证的卡！";
                    case "8B":
                        return "模块故障！";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 下载显示屏信息
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "":
                        return "无数据！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 获取设备信息
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 初始化车位信息
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 下载车位组信息
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
        /// 查询车位组信息
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
        /// 联机显示屏和语音提示
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 初始化时段
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";


                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
            }
        }
        /// <summary>
        /// 下载时段
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
                        return "操作成功！";
                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "83":
                        return "非法数据！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 读取时段
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
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
        /// 调整场内停车
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
                        return "操作成功！";

                    case "80":
                        return "不支持命令！";
                    case "81":
                        return "未知错误！";

                    case "82":
                        return "长度错误！";
                    case "83":
                        return "操作成功！";
                    case "88":
                        return "找不到卡！";

                }
                return udpinfo;
            }
            else
            {
                return udpinfo = "通讯失败，请检查！";
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
                str = "操作成功";
                return str;
            }
            else if (result == "80")
            {
                str = "不支持命令";
                return str;
            }
            else if (result == "81")
            {
                str = "未知错误";
                return str;
            }
            else if (result == "82")
            {
                str = "长度错误";
                return str;
            }
            else if (result == "85")
            {
                str = "无新记录，初始化后没有记录";
                return str;
            }
            else
                return str;
        }
    }
}
