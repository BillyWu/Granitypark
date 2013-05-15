using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;



namespace ComLib
{
    //设备通讯通用信息
    public class devObj
    {
        public string ID = string.Empty;        //设备号
        public string Buss = string.Empty;      //设备应用业务：门禁、考勤等
        public int ControlNo = 0;               //控制号：应用于门禁
        public string IP = string.Empty;        //IP地址
        public string Port = string.Empty;      //端口
        public int Mode = 0;                    //通讯方式(0-485,1-自动IP,2-固定IP)
        public Boolean LH = false;              //是否交换高低位
        public string Paritymode = string.Empty;    //校验方式:0或空为异或校验,1为和校验
        public string Exchanges = string.Empty; //转义字符集
        public int Delay = 2000;                //延迟时间(ms)
        public string Command = "";             //命令说明
        public string WeekRule = "";            //星期取值规则
        public Boolean IsAsc = false;           //是否按Char发送
        public string PreCommand = "";          //前置操作命令字
        public string Return = "";              //返回状态,空为执行错，否则为执行成功

        public int baudRate = 9600;
        public Parity parity = Parity.None;
        public int DataBits = 8;
        public StopBits stopBits = StopBits.One;

    }

    //参数规则信息
    public class pmObj
    {
        public string Name = string.Empty;        //参数名称
        public int Len = 0;                       //位长(字符位长)
        public string DataType = string.Empty;    //数据类型
        public string Format = string.Empty;      //数据格式
        public string Formats = string.Empty;     //参数位格式
        public Double Scale = 0;                     //参数比例,对整数、浮点数有效
        public int Add = 0;                       //是否将字符值增值,增值量为Add
        public Boolean LH = false;                //是否交换高低位
        public string Finish = "";                //命令说明
        public string SubItems = string.Empty;    //参数位格式
        public string Value = string.Empty;       //参数位格式
        public string IsNull = string.Empty;      //转0字符，如刷卡状态，为FF时，是无记录值，应转为0
    }

    public class ComClass
    {
        //udp通讯类byLeo
        #region
        public string[] UdpGetInfoByIPPort(string ipAddress, int ipPort, string cmdstr, int idelay)
        {
            if (basefun.IsCorrenctIP(ipAddress) == false) return null;
            if (idelay == 0) idelay = 100;
            int timeout = idelay;
            byte[] bytOutBuffer = getByteBy16s(cmdstr);
            IPEndPoint RemoteIpEndPoint = SetIPEndPoint(ipAddress, ipPort);
            if (RemoteIpEndPoint == null) return null;
            byte[] bytReceiveBuffer;
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Send(bytOutBuffer, bytOutBuffer.Length, RemoteIpEndPoint);
                IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                IAsyncResult result = udpClient.BeginReceive(null, this);
                result.AsyncWaitHandle.WaitOne(timeout, false);
                if (!result.IsCompleted)
                {
                    //throw SharpTimeoutException.Create(ipAddress, timeout);
                    udpClient.Close();
                    return null;
                }

                bytReceiveBuffer = udpClient.EndReceive(result, ref from);
                string udpInfo = get16sByByte(bytReceiveBuffer);
                udpInfo = SpecialRestore(udpInfo, "dddb", "db");
                udpInfo = SpecialRestore(udpInfo, "dcdb", "c0");
                //取返回值16进制数组,并解析
                string[] infos = AnalysisEateryResults(udpInfo);
                udpClient.Close();
                return infos;
            }

        }


        public string[] UdpGetInfoByIPPort(string ipAddress, int ipPort, string cmdstr, int idelay, ref string state)
        {
            if (basefun.IsCorrenctIP(ipAddress) == false) return null;
            if (idelay == 0) idelay = 100;
            int timeout = idelay;
            byte[] bytOutBuffer = getByteBy16s(cmdstr);
            IPEndPoint RemoteIpEndPoint = SetIPEndPoint(ipAddress, ipPort);
            byte[] bytReceiveBuffer;
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Send(bytOutBuffer, bytOutBuffer.Length, RemoteIpEndPoint);
                IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                IAsyncResult result = udpClient.BeginReceive(null, this);
                result.AsyncWaitHandle.WaitOne(timeout, false);
                if (!result.IsCompleted)
                {
                    //throw SharpTimeoutException.Create(ipAddress, timeout);
                    udpClient.Close();
                    return null;
                }

                bytReceiveBuffer = udpClient.EndReceive(result, ref from);
                string udpInfo = get16sByByte(bytReceiveBuffer);
                udpInfo = SpecialRestore(udpInfo, "dddb", "db");
                udpInfo = SpecialRestore(udpInfo, "dcdb", "c0");
                //取返回值16进制数组,并解析
                string[] infos = AnalysisEateryResults(udpInfo, ref state);
                udpClient.Close();
                return infos;
            }

        }
        public byte[] getAscByteBy16s(string str, string strhead, string strend)//先吧以16进制存储的字符串转换成ASCLL码（char型）再转换成byte
        {
            int ihead = basefun.toIntval(strhead);
            int iend = basefun.toIntval(strend);
            int iLen = str.Length;
            char[] buff = new char[iLen + 2];
            buff[0] = (char)ihead;
            buff[iLen + 1] = (char)iend;
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

        public string GetInfo(string devpms, string cmdpms, string datapms, string OutString, ref string state)
        {
            devObj devobj = new devObj();
            string cmdstr = CommandString(devpms, cmdpms, datapms, ref devobj);
            if (basefun.IsCorrenctIP(devobj.IP) == false) return null;
            if (devobj.Delay == 0) devobj.Delay = 2000;
            byte[] bytOutBuffer;
            if (devobj.IsAsc)
            {
                string strhead = cmdstr.Substring(0, 2);
                string strend = cmdstr.Substring(cmdstr.Length - 2, 2);
                cmdstr = cmdstr.Substring(2, cmdstr.Length - 2);
                cmdstr = cmdstr.Substring(0, cmdstr.Length - 2);
                bytOutBuffer = getAscByteBy16s(cmdstr, strhead, strend);
            }
            else
                bytOutBuffer = getByteBy16s(cmdstr);
            //string strasc = "";
            //for (int i = 0; i < bytOutBuffer.Length; i++)
            //{
            //    strasc = bytOutBuffer[i].
            //}
            byte[] bytReceiveBuffer;

            //----- UDP ------//
            if (devobj.Mode == 0)
            {
                SerialCommi serialcom = new SerialCommi();
                serialcom.SetProtocolParam("COM1", devobj.baudRate, devobj.Delay);
                bytReceiveBuffer = serialcom.SendCommand(cmdstr, true);
            }
            else
            {
                IPEndPoint RemoteIpEndPoint = SetIPEndPoint(devobj.IP, basefun.toIntval(devobj.Port), devobj.Mode);
                UdpClient udpClient = new UdpClient();
                udpClient.Send(bytOutBuffer, bytOutBuffer.Length, RemoteIpEndPoint);

                IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                IAsyncResult result = udpClient.BeginReceive(null, this);
                result.AsyncWaitHandle.WaitOne(devobj.Delay, false);
                if (!result.IsCompleted)
                {
                    udpClient.Close();
                    return null;
                }
                bytReceiveBuffer = udpClient.EndReceive(result, ref from);
                udpClient.Close();
            }
            //-----------------


            string udpInfo = "";
            if (devobj.IsAsc)
            {
                string info = "";
                char[] cbuff = new char[bytReceiveBuffer.Length - 2];
                for (int i = 1; i < bytReceiveBuffer.Length - 1; i++)
                {
                    cbuff[i - 1] = (char)bytReceiveBuffer[i];
                    info += Convert.ToString(cbuff[i - 1]);
                }
                //去掉帧头和帧尾
                udpInfo = info;
            }
            else
                udpInfo = get16sByByte(bytReceiveBuffer, true);

            //恢复转义字符 
            if (devobj.Exchanges != "")
            {
                string[] strExs = devobj.Exchanges.Split('#');
                for (int i = 0; i < strExs.Length; i++)
                {
                    string[] _a = strExs[i].Split('/');
                    udpInfo = SpecialRestore(udpInfo, _a[1], _a[0]);
                }
            }
            //取返回值16进制数组,并解析
            string infos = AnalysisEateryResults(udpInfo, OutString, devobj, true, ref state);
            return infos;

        }

        public string GetInfo(string devpms, string cmdpms, string datapms, string OutString, ref string state, ref string cmdstring)
        {
            devObj devobj = new devObj();
            string cmdstr = CommandString(devpms, cmdpms, datapms, ref devobj);
            if (basefun.IsCorrenctIP(devobj.IP) == false) return null;
            if (devobj.Delay == 0) devobj.Delay = 2000;
            byte[] bytOutBuffer;
            if (devobj.IsAsc)
            {
                string strhead = cmdstr.Substring(0, 2);
                string strend = cmdstr.Substring(cmdstr.Length - 2, 2);
                cmdstr = cmdstr.Substring(2, cmdstr.Length - 2);
                cmdstr = cmdstr.Substring(0, cmdstr.Length - 2);
                bytOutBuffer = getAscByteBy16s(cmdstr, strhead, strend);
            }
            else
                bytOutBuffer = getByteBy16s(cmdstr);
            IPEndPoint RemoteIpEndPoint = SetIPEndPoint(devobj.IP, basefun.toIntval(devobj.Port), devobj.Mode);
            byte[] bytReceiveBuffer;
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Send(bytOutBuffer, bytOutBuffer.Length, RemoteIpEndPoint);
                IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                IAsyncResult result = udpClient.BeginReceive(null, this);
                result.AsyncWaitHandle.WaitOne(devobj.Delay, false);
                if (!result.IsCompleted)
                {
                    udpClient.Close();
                    return null;
                }
                bytReceiveBuffer = udpClient.EndReceive(result, ref from);
                string udpInfo = "";
                if (devobj.IsAsc)
                {
                    string info = "";
                    char[] cbuff = new char[bytReceiveBuffer.Length - 2];
                    for (int i = 1; i < bytReceiveBuffer.Length - 1; i++)
                    {
                        cbuff[i - 1] = (char)bytReceiveBuffer[i];
                        info += Convert.ToString(cbuff[i - 1]);
                    }
                    //去掉帧头和帧尾
                    udpInfo = info;
                }
                else
                    udpInfo = get16sByByte(bytReceiveBuffer, true);

                //恢复转义字符 
                if (devobj.Exchanges != "")
                {
                    string[] strExs = devobj.Exchanges.Split('#');
                    for (int i = 0; i < strExs.Length; i++)
                    {
                        string[] _a = strExs[i].Split('/');
                        udpInfo = SpecialRestore(udpInfo, _a[1], _a[0]);
                    }
                }
                //取返回值16进制数组,并解析
                //udpInfo = "7E7AF08110091227000012200100000000DCDE0F909B137D0100FF00000000F4060D";

                string infos = AnalysisEateryResults(udpInfo, OutString, devobj, true, ref state);
                udpClient.Close();
                return infos;
            }

        }

        public string GetCmdStr(string devpms, string cmdpms, string datapms, string OutString, ref string state)
        {
            devObj devobj = new devObj();
            return CommandString(devpms, cmdpms, datapms, ref devobj);
        }


        private devObj setDevObj(string devpms)
        {
            devObj devobj = new devObj();
            devobj.ID = basefun.valtag(devpms, "devid");
            devobj.ControlNo = basefun.toIntval(basefun.valtag(devpms, "controlno"));
            devobj.Buss = basefun.valtag(devpms, "buss");
            devobj.IP = basefun.valtag(devpms, "ip");
            devobj.Mode = basefun.toIntval(basefun.valtag(devpms, "mode")) - 1;
            devobj.Port = basefun.valtag(devpms, "port");
            if (devobj.Mode == 0)
                devobj.Port = ComPort(devobj.Port);
            devobj.Paritymode = basefun.valtag(devpms, "parity");
            devobj.LH = basefun.valtag(devpms, "LH") == "1" ? true : false;
            devobj.Exchanges = basefun.valtag(devpms, "exchange");
            devobj.Command = basefun.valtag(devpms, "cmd");
            devobj.WeekRule = basefun.valtag(devpms, "weekrule");
            devobj.IsAsc = basefun.valtag(devpms, "isasc") == "1" ? true : false;
            devobj.PreCommand = basefun.valtag(devpms, "precmd");
            devobj.Return = basefun.valtag(devpms, "return");
            devobj.parity = paritypm(basefun.valtag(devpms, "paritycom"));
            devobj.baudRate = basefun.toIntval((basefun.valtag(devpms, "baudrate")));
            int dbit = basefun.toIntval(basefun.valtag(devpms, "databits"));
            devobj.DataBits = (dbit == 0) ? 8 : dbit;
            devobj.stopBits = stopbitpm(basefun.valtag(devpms, "stopbits"));
            //串口参数
            return devobj;
        }

        private string ComPort(String s)
        {
            switch (s.ToLower())
            {
                case "1": return "COM1"; break;
                case "2": return "COM2"; break;
                case "3": return "COM3"; break;
                case "4": return "COM4"; break;
                case "5": return "COM5"; break;
                case "6": return "COM6"; break;
                case "7": return "COM7"; break;
                case "8": return "COM8"; break;
                case "9": return "COM9"; break;
                case "10": return "COM10"; break;
                case "11": return "COM11"; break;
                case "12": return "COM12"; break;
            }
            return "COM1";
        }
        private Parity paritypm(String s)
        {
            switch (s.ToLower())
            {
                case "none": return Parity.None; break;
                case "even": return Parity.Even; break;
                case "mark": return Parity.Mark; break;
                case "odd": return Parity.Odd; break;
                case "space": return Parity.Space; break;
            }
            return Parity.None;
        }
        private StopBits stopbitpm(String s)
        {
            switch (s.ToLower())
            {
                case "none": return StopBits.None; break;
                case "one": return StopBits.One; break;
                case "onepoint": return StopBits.OnePointFive; break;
                case "two": return StopBits.Two; break;
            }
            return StopBits.One;
        }
        /// <summary>
        /// 修补汉字字符串中的单字节字符
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private string trHzChar(string src)
        {
            string s = "";
            string[] strs = new string[src.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = src.Substring(i, 1);
                string schar = Regex.Match(strs[i], "([[\\u4e00-\\u9fa5|\\s](&nbsp;)?)*").Value;
                if (schar == "") strs[i] = strs[i] + " ";
                s = s + strs[i];
            }
            return s;
        }
       




        /// <summary>
        /// 根据设备参数和命令参数计算通讯命令字符串
        /// </summary>
        /// <param name="devpms">设备参数李氏集合</param>
        /// <param name="cmdpms">指令参数李氏集合</param>
        /// <returns>通讯字符串</returns>
        public string CommandString(string devpms, string cmdpms, string datapms, ref devObj devobj)
        {
            //定义设备属性
            string[] datapmss = datapms.Split(',');
            devobj = setDevObj(devpms);
            string strpm = "";
            string[] arrpm = cmdpms.Split(';');
            string str = "";
            int ilength = 0;
            pmObj pmXorObj = new pmObj();
            for (int i = 0; i < arrpm.Length; i++)
            {

                string[] arr = arrpm[i].Split(',');

                NameObjectList lst = NameValueTag(arrpm[i]);
                pmObj pmobj = new pmObj();
                pmobj.Name = getVarNameByHmStr(arr[0]);
                pmobj.Len = toIntval(valtag(arrpm[i], "{len}"));
                pmobj.DataType = valtag(arrpm[i], "{datatype}");
                pmobj.Format = valtag(arrpm[i], "{format}");
                if (pmobj.Format.StartsWith("±"))
                    pmobj.Format = pmobj.Format.Substring(1);
                pmobj.Formats = valtag(arrpm[i], "{formats}");
                pmobj.Scale = toFloatValue(valtag(arrpm[i], "{scale}")) == 0 ? 1 : toFloatValue(valtag(arrpm[i], "{scale}"));
                //pmobj.Scale = toIntval(valtag(arrpm[i], "{scale}")) == 0 ? 1 : toIntval(valtag(arrpm[i], "{scale}"));
                pmobj.Add = toIntval(valtag(arrpm[i], "{add}"));
                pmobj.SubItems = valtag(arrpm[i], "{subitems}");
                pmobj.Value = lst.AllStringValues[0].ToLower();



                string strlh = valtag(arrpm[i], "{lh}");
                if (strlh == "") pmobj.LH = devobj.LH;
                else
                {
                    if (strlh == "0") pmobj.LH = false;
                    else pmobj.LH = true;
                }
                if (lst.Keys[0] == "{设备地址}")
                {
                    if (pmobj.DataType == "word")
                        str = str + exchangeLH(devobj.ID, pmobj.LH);
                    else
                        str = str + exchangeLH(sVal10To16(devobj.ID, pmobj.Len / 8), pmobj.LH);
                    continue;
                }
                string svalue = pmobj.Value; //值
                ilength = basefun.toIntval(lst.AllStringValues[1].ToLower()); //位数
                string datetype = lst.AllStringValues[2].ToLower(); //数据类型
                string format = lst.AllStringValues[3]; //格式
                //int icale = basefun.toIntval(lst.AllStringValues[5].ToLower()); //比例因子
                //if (icale == 0) icale = 1;
                switch (datetype)
                {
                    case "string":
                        if (pmobj.Value == "" || pmobj.Value == "?")
                            svalue = basefun.valtag(datapms, pmobj.Name);

                        long _iv = basefun.toIntval(svalue);
                        svalue = this.sVal10To16(_iv.ToString());
                        break;
                    case "asc":
                        if (pmobj.Value == "" || pmobj.Value == "?")
                            svalue = (basefun.valtag(datapms, pmobj.Name)).Trim();
                        svalue = AscToCharactor(svalue);
                        int _lenerr = (pmobj.Len / 8 - svalue.Length / 2);
                        if (_lenerr % 2 == 0)
                        {
                            string strsign = "";
                            for (int ib = 0; ib < _lenerr; ib++)
                            {
                                strsign = strsign + "20";
                            }
                            svalue = strsign + svalue;
                        }
                        break;
                    case "asc2":
                        if (pmobj.Value == "" || pmobj.Value == "?")
                            svalue = (basefun.valtag(datapms, pmobj.Name)).Trim();
                        svalue = AscToCharactor(svalue);
                        _lenerr = (pmobj.Len / 8 - svalue.Length / 2);
                        if (_lenerr % 2 == 0)
                        {
                            string strsign = "";
                            for (int ib = 0; ib < _lenerr; ib++)
                            {
                                strsign = strsign + "20";
                            }
                            svalue = strsign + svalue;
                        }
                        if (svalue.Length > 16)
                            svalue = svalue.Substring(0, 16);
                        break;
                    case "int":
                    case "long":
                    case "integer":
                        if (pmobj.SubItems != "")
                        {
                            string[] subs = pmobj.SubItems.Split('+');
                            int isubs = 0;
                            string _sbit = "";
                            for (int m = 0; m < subs.Length; m++)
                            {
                                string[] asubs = subs[m].Split('=');
                                string[] asubpms = asubs[1].Split('#');
                                string subValue = asubpms[0];
                                if (subValue == "" || subValue == "?")
                                    subValue = basefun.valtag(datapms, asubs[0]);
                                if (string.IsNullOrEmpty(subValue))
                                    subValue = "0";
                                int iSublen = basefun.toIntval(asubpms[1]);
                                string subDataType = asubpms[2];
                                string subFormat = asubpms[3];
                                string subScale = asubpms[4];
                                switch (subDataType.ToLower())
                                {
                                    case "double":
                                        //转为二进制
                                        if (subFormat == "") subFormat = "{0:.00}";
                                        if (subFormat.IndexOf("{") > -1)
                                        {
                                            subValue = string.Format(subFormat, Convert.ToDouble(subValue) * pmobj.Scale);
                                        }
                                        else
                                            subValue = string.Format("{" + subFormat + "}", Convert.ToDouble(subValue) * pmobj.Scale);
                                        subValue = basefun.toIntval(subValue).ToString();
                                        subValue = sVal10To2(subValue, iSublen);
                                        break;
                                    case "integer":
                                    case "int":
                                        subValue = sVal10To2(subValue, iSublen);
                                        break;
                                    default:
                                        subValue = subValue;
                                        break;
                                }
                                //rtns = basefun.setvaltag(rtns, asubs[0], subValue);
                                _sbit = _sbit + subValue;
                            }
                            svalue = this.sVal10To16(Convert.ToInt32(_sbit, 2).ToString());
                        }
                        else
                        {
                            if (pmobj.Value == "" || pmobj.Value == "?")
                                svalue = basefun.valtag(datapms, pmobj.Name);
                            if (string.IsNullOrEmpty(svalue))
                                svalue = "0";
                            long _lv = Convert.ToInt64(basefun.toIntval64(svalue) * pmobj.Scale);
                            svalue = this.sVal10To16(_lv.ToString());
                            if (pmobj.Format == "db")
                            {
                                svalue = basefun.toIntval64(svalue).ToString() + basefun.toIntval64(svalue).ToString();
                            }
                        }
                        break;
                    case "float":
                    case "double":
                        if (pmobj.Value == "" || pmobj.Value == "?")
                            svalue = basefun.valtag(datapms, pmobj.Name);
                        if (string.IsNullOrEmpty(svalue))
                            svalue = "0";
                        if (pmobj.Format.IndexOf("{") > -1)
                            svalue = string.Format(pmobj.Format, Convert.ToDouble(svalue) * pmobj.Scale);
                        else if (!string.IsNullOrEmpty(pmobj.Format))
                            svalue = string.Format("{" + pmobj.Format + "}", Convert.ToDouble(svalue) * pmobj.Scale);
                        else
                            svalue = Convert.ToInt32(Convert.ToDouble(svalue) * pmobj.Scale).ToString();
                        svalue = basefun.toIntval(svalue).ToString();
                        svalue = this.sVal10To16(svalue, "F".PadLeft(pmobj.Len / 4, 'F'));
                        break;
                    case "bit":
                        //脱机处理=1#1#String##+脱机时间=0.1#15#Double#{0:.0}#10;
                        if (pmobj.SubItems != "")
                        {
                            string[] subs = pmobj.SubItems.Split('+');
                            int isubs = 0;
                            string _sbit = "";
                            for (int m = 0; m < subs.Length; m++)
                            {
                                string[] asubs = subs[m].Split('=');
                                string[] asubpms = asubs[1].Split('#');
                                string subValue = asubpms[0];
                                if (subValue == "" || subValue == "?")
                                    subValue = basefun.valtag(datapms, pmobj.Name + ".{" + asubs[0] + "}");
                                int iSublen = basefun.toIntval(asubpms[1]);
                                string subDataType = asubpms[2];
                                string subFormat = asubpms[3];
                                string subScale = asubpms[4];
                                switch (subDataType.ToLower())
                                {
                                    case "double":
                                        //转为二进制
                                        if (string.IsNullOrEmpty(subValue))
                                            subValue = "0";
                                        if (subFormat == "") subFormat = "{0:.00}";
                                        if (subFormat.IndexOf("{") > -1)
                                        {
                                            subValue = string.Format(subFormat, Convert.ToDouble(subValue) * 10);
                                        }
                                        else
                                            subValue = string.Format("{" + subFormat + "}", Convert.ToDouble(subValue) * pmobj.Scale);
                                        subValue = sVal10To2(subValue, iSublen);
                                        break;
                                    case "integer":
                                    case "int":
                                        if (string.IsNullOrEmpty(subValue))
                                            subValue = "0";
                                        subValue = sVal10To2(subValue, iSublen);
                                        break;
                                    default:
                                        subValue = subValue;
                                        break;
                                }
                                //rtns = basefun.setvaltag(rtns, asubs[0], subValue);
                                _sbit = _sbit + subValue;
                            }
                            try
                            {
                                svalue = this.sVal10To16(Convert.ToInt32(_sbit, 2).ToString());
                            }
                            catch (Exception ex) { svalue = "00"; };
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(svalue)) svalue = "0";
                            if (pmobj.Value == "" || pmobj.Value == "?")
                                svalue = basefun.valtag(datapms, pmobj.Name);
                            if (string.IsNullOrEmpty(svalue))
                            {
                                return "";
                            }
                            svalue = this.sVal10To16(Convert.ToInt32(svalue, 2).ToString());
                        }
                        break;
                    case "date":
                    case "datetime":
                        if (pmobj.Value == "" || pmobj.Value == "?")
                            svalue = basefun.valtag(datapms, pmobj.Name);
                        if (string.IsNullOrEmpty(svalue))
                            svalue = DateTime.Now.ToString();
                        if (format.EndsWith("~BCD"))
                            svalue = basefun.toBCDDatetime(svalue, format);
                        else
                            svalue = basefun.toHexDatetime(svalue, format);
                        break;
                    default:
                        if (pmobj.Value == "" || pmobj.Value == "?")
                            svalue = basefun.valtag(datapms, pmobj.Name);

                        break;
                }

                if (ilength == 0) ilength = 8 * svalue.Length / 2;
                if (lst.Keys[0] == "{校验字}")
                {
                    pmXorObj = pmobj;
                    break;
                }
                if (svalue == "") svalue = "0";
                svalue = ComplementingBits(svalue, ilength);
                if (!format.EndsWith("~BCD"))
                    svalue = exchangeLH(svalue, pmobj.LH);
                str = str + svalue;
                Debug.WriteLine(str);
            }

            // -- 末尾计算校验码 --
            CrcClass crc = new CrcClass();
            string xor = "";
            switch (devobj.Paritymode)
            {
                case "0":
                    xor = crc.CRCXOR(str.Substring(2, str.Length - 2));
                    break;
                case "1":
                    xor = crc.CRCSUM(str.Substring(2, str.Length - 2));
                    break;
            }
            string vstr = exchangeLH(ComplementingBits(xor, ilength), pmXorObj.LH);
            //处理转义字符：对不包括帧头、帧尾的字节进行转义处理
            str = str.Substring(2, str.Length - 2) + vstr;
            str = transferWords(str, devobj.Exchanges);

            NameObjectList lst0 = NameValueTag(arrpm[0]);
            NameObjectList lst1 = NameValueTag(arrpm[arrpm.Length - 1]);
            string strCmd = lst0.AllStringValues[0] + str + lst1.AllStringValues[0];
            strCmd = strCmd.ToUpper();
            return strCmd;
        }


        /// <summary>
        /// 处理转义字符：对不包括帧头、帧尾的字节进行转义处理(按字节转义)
        /// </summary>
        /// <param name="str">需要转义的原字符串</param>
        /// <param name="exWords">转义字符集</param>
        /// <returns></returns>0000173b
        private string transferWords(string str, string exWords)
        {

            if (exWords == "") return str;
            string[] strExs = exWords.Split('#');
            str = str.ToUpper();
            string[] bytes = String16ToArry(str);
            str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < strExs.Length; j++)
                {
                    string[] strwords = strExs[j].Split('/');
                    if (bytes[i].ToUpper() == strwords[0].ToUpper())
                    {
                        bytes[i] = strwords[1];
                        break;
                    }
                }
                str = str + bytes[i];
            }
            return str;
        }

        private IPEndPoint SetIPEndPoint(string sip, int port, int mode)
        {

            IPEndPoint RemoteIpEndPoint;
            try
            {
                if (mode == 2)
                {
                    RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(sip), port);
                }
                else
                {
                    if (port == 0) port = 60000;
                    RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
                }
            }
            catch (Exception ex)
            {
                return null;
            };
            return RemoteIpEndPoint;
        }

        private IPEndPoint SetIPEndPoint(string sip, int port)
        {
            IPEndPoint RemoteIpEndPoint;
            if (sip != "")
            {
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(sip), port);
            }
            else
            {
                if (port == 0) port = 60000;
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
            }
            return RemoteIpEndPoint;
        }

        private byte[] getByteBy16s(string cmdString)
        {
            int ilen = cmdString.Length / 2;
            byte[] bytOutBuffer = new byte[ilen];
            for (int i = 0; i < ilen; i++)
                bytOutBuffer[i] = Convert.ToByte(Convert.ToInt32(cmdString.Substring(2 * i, 2), 16));
            return bytOutBuffer;
        }

        private string get16sByByte(byte[] buf)
        {
            string returndata = "";
            int ilen = buf.Length;
            for (int i = 0; i < ilen; i++)
                returndata = returndata + Convert.ToInt32(buf[i]).ToString("X").PadLeft(2, '0');
            return returndata;
        }

        /// <summary>
        /// 去除头尾后的16进制字符串
        /// </summary>
        /// <param name="buf">字节数组</param>
        /// <param name="noHE">是否去掉头尾</param>
        /// <returns></returns>
        private string get16sByByte(byte[] buf, Boolean noHE)
        {
            string returndata = "";
            int ilen = buf.Length;
            if (noHE)
            {
                for (int i = 1; i < ilen - 1; i++)
                    returndata = returndata + Convert.ToInt32(buf[i]).ToString("X").PadLeft(2, '0');
            }
            else
            {
                for (int i = 0; i < ilen; i++)
                    returndata = returndata + Convert.ToInt32(buf[i]).ToString("X").PadLeft(2, '0');
            }
            return returndata;
        }

        public void Delay(double t)
        {
            long tickEnd;

            tickEnd = DateTime.Now.AddSeconds(t).Ticks;
            while (DateTime.Now.Ticks < tickEnd)
            {
                ;
            }
        }

        /// <summary>
        /// 对标记形成字符串数组string={{"key","value"},{"key","value"}}
        /// </summary>
        /// <param name="stag"></param>
        /// <returns></returns>
        public NameObjectList NameValueTag(string stag)
        {
            NameObjectList tag = new NameObjectList();
            stag = stag.Replace(",,", "*#$#*");
            string[] arrTag = basefun.getArrayFromString(stag, ",");
            if (null == arrTag || arrTag.Length < 1)
                return tag;
            for (int i = 0; i < arrTag.Length; i++)
                if (arrTag[i].StartsWith("@") && arrTag[i].Contains("="))
                {
                    string strkey = arrTag[i].Substring(1, arrTag[i].IndexOf("=") - 1);
                    string strvalue = arrTag[i].Substring(arrTag[i].IndexOf("=") + 1);
                    strvalue = strvalue.Replace("*#$#*", ",").Trim();
                    tag[strkey] = strvalue;
                }
            return tag;
        }


        #endregion

        #region
        /// <summary>
        /// 解析结果值,含多重校验
        /// </summary>
        /// <param name="strResult">消费机返回字符串</param>
        /// <returns></returns>
        public string[] AnalysisEateryResults(string strResult)
        {
            //strResult = "C00101120010000001000000010101010104090000000FC0";
            //返回数据区：C0 01 01 12(以下为通讯字节数) 00（返回状态字） 10(--16进制数，实际应用数据) 00 00 01 00 00 00 01 01 01 01 01 04 09 00 00 00 -- 0F C0
            if (string.IsNullOrEmpty(strResult))
                return null;
            if (strResult.Length <= 8) return null;                             //如果返回字符串长度不大于8，则无效返回
            int rtnlen = Convert.ToInt32(strResult.Substring(6, 2), 16);        //获得取字符串中返回通讯字节数的理论长度
            //校验字,如果不对，则返回空
            string str = strResult.Substring(2, strResult.Length - 6);
            if (strResult.Substring(strResult.Length - 4, 2).ToLower() != this.CRCXOR(str).ToLower())
                return null;

            //取实际返回的通讯字节长度,如果小于理论长度，则无效    
            int reallength = Convert.ToInt32(strResult.Substring(8, strResult.Length - 12).Length) / 2;
            if (reallength != rtnlen) return null;
            string strRtnData = strResult.Substring(8, strResult.Length - 12);
            string rstate = strRtnData.Substring(0, 2);
            string rdata = strRtnData.Substring(4, strRtnData.Length - 4);
            if (rstate == "00") return String16ToArry(rdata);
            else return null;
        }

        /// <summary>
        /// 解析结果值,含多重校验,并返回状态值
        /// </summary>
        /// <param name="strResult">消费机返回字符串</param>
        /// <returns></returns>
        public string[] AnalysisEateryResults(string strResult, ref string state)
        {
            //strResult = "C00101120010000001000000010101010104090000000FC0";
            //返回数据区：C0 01 01 12(以下为通讯字节数) 00（返回状态字） 10(--16进制数，实际应用数据) 00 00 01 00 00 00 01 01 01 01 01 04 09 00 00 00 -- 0F C0
            if (string.IsNullOrEmpty(strResult))
                return null;
            if (strResult.Length <= 8) return null;                             //如果返回字符串长度不大于8，则无效返回
            int rtnlen = Convert.ToInt32(strResult.Substring(6, 2), 16);        //获得取字符串中返回通讯字节数的理论长度

            //校验字,如果不对，则返回空
            string str = strResult.Substring(2, strResult.Length - 6);
            if (strResult.Substring(strResult.Length - 4, 2).ToLower() != this.CRCXOR(str).ToLower())
                return null;

            //取实际返回的通讯字节长度,如果小于理论长度，则无效    
            int reallength = Convert.ToInt32(strResult.Substring(8, strResult.Length - 12).Length) / 2;
            if (reallength != rtnlen) return null;
            string strRtnData = strResult.Substring(8, strResult.Length - 12);
            state = strRtnData.Substring(0, 2);
            string rdata = strRtnData.Substring(4, strRtnData.Length - 4);
            if (state == "00") return String16ToArry(rdata);
            else return null;
        }

        /// <summary>
        /// 根据返回值的参数结构解析结果值,含多重校验,并返回状态值
        /// </summary>
        /// <param name="strResult">消费机返回字符串</param>
        /// <returns></returns>
        public string[] AnalysisEateryResults(string strResult, string strpms, ref string state)
        {
            if (string.IsNullOrEmpty(strResult))
                return null;
            //取返回结果的参数结构

            if (strResult.Length <= 8) return null;                             //如果返回字符串长度不大于8，则无效返回
            int rtnlen = Convert.ToInt32(strResult.Substring(6, 2), 16);        //获得取字符串中返回通讯字节数的理论长度

            //校验字,如果不对，则返回空
            string str = strResult.Substring(2, strResult.Length - 6);
            if (strResult.Substring(strResult.Length - 4, 2).ToLower() != this.CRCXOR(str).ToLower())
                return null;

            //取实际返回的通讯字节长度,如果小于理论长度，则无效    
            int reallength = Convert.ToInt32(strResult.Substring(8, strResult.Length - 12).Length) / 2;
            if (reallength != rtnlen) return null;
            string strRtnData = strResult.Substring(8, strResult.Length - 12);
            state = strRtnData.Substring(0, 2);
            string rdata = strRtnData.Substring(4, strRtnData.Length - 4);
            if (state == "00") return String16ToArry(rdata);
            else return null;
        }

        /// <summary>
        /// 处理时，结果字符串示noHE的定义而确定是否已包含或未包含帧头帧尾。根据返回值的参数结构解析结果值,含多重校验,并返回状态值
        /// </summary>
        /// <param name="strResult">待解析的结果字符串</param>
        /// <param name="strpms">参数配置集合</param>
        /// <param name="devobj">设备参数</param>
        /// <param name="noHE">包含帧头帧尾的状态字</param>
        /// <param name="state">结果集中的返回状态值</param>
        /// <returns></returns>
        public string AnalysisEateryResults(string strResult, string strpms, devObj devobj, Boolean noHE, ref string state)
        {
            if (string.IsNullOrEmpty(strResult))
                return null;
            //取返回结果的参数结构
            //计算返回值的理论长度
            string[] arrpm = strpms.Split(';');
            int ilength = 0;
            int verlength = 0;  //校验字的字节数
            int bit0len = 0;
            int bit1len = 0;
            for (int i = 0; i < arrpm.Length; i++)
            {
                if (noHE)
                {
                    if (i == 0 || i == arrpm.Length - 1) continue;
                }
                NameObjectList lst = NameValueTag(arrpm[i]);
                if (i == 0) bit0len = basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8;
                if (i == arrpm.Length - 1) bit1len = basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8;
                if (lst.Keys[0] == "{校验字}") verlength = basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8;
                ilength = ilength + basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8; //字节数
            }
            if (strResult.Length / 2 < ilength && 14 == strResult.Length)
            {
                strpms = "@{帧头}=02,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{设备地址}=,@{len}=8,@{datatype}=string,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{状态}=,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{命令长度}=0,@{len}=16,@{datatype}=long,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{校验字}=,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{帧尾}=03,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=";
                return AnalysisEateryResults(strResult, strpms, devobj, noHE, ref state);
            }
            if (strResult.Length / 2 < ilength && 16 == strResult.Length)
            {
                strpms = "@{帧头}=02,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{设备地址}=,@{len}=8,@{datatype}=string,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{控制场}=01,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{长度场}=2,@{len}=8,@{datatype}=int,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{状态}=,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{命令长度}=0,@{len}=8,@{datatype}=long,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{校验字}=,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=;@{帧尾}=03,@{len}=8,@{datatype}=word,@{format}=,@{formats}=,@{scale}=,@{subitems}=";
                return AnalysisEateryResults(strResult, strpms, devobj, noHE, ref state);
            }
            if (strResult.Length / 2 != ilength) return null;                             //如果返回字符串长度不大于8，则无效返回
            //校验字,如果不对，则返回空

            CrcClass crc = new CrcClass();
            string xor = "";
            switch (devobj.Paritymode)
            {
                case "0":
                    xor = crc.CRCXOR(strResult.Substring(bit0len * 2, strResult.Length - bit0len * 2 - bit1len * 2 - verlength * 2));
                    break;
                case "1":
                    xor = crc.CRCSUM(strResult.Substring(bit0len * 2, strResult.Length - bit0len * 2 - bit1len * 2 - verlength * 2));
                    break;
            }
            //修正校验字长度未补位错误,暂不对数据校验码判断
            string _xor = this.ComplementingBits(xor, verlength * 2, true, true);
            string vstr = exchangeLH(_xor, devobj.LH);
            //if (strResult.Substring(strResult.Length - bit1len * 2 - verlength * 2, verlength * 2).ToLower() != vstr.ToLower())
            //    return null;

            string[] strResults = String16ToArry(strResult);
            //根据转出的格式要求处理返回值集合
            string svalue = getValByHmFormat(strpms, strResults, devobj, noHE, ref state);
            //string rdata = strRtnData.Substring(4, strRtnData.Length - 4);
            //if (state == "00") return String16ToArry(rdata);
            return svalue;
        }

        /// <summary>
        /// 根据返回值的参数结构解析结果值,含多重校验,并返回状态值
        /// </summary>
        /// <param name="strResult">消费机返回字符串</param>
        /// <returns></returns>
        public string AnalysisEateryResults(string strResult, string strpms, devObj devobj, ref string state)
        {
            if (string.IsNullOrEmpty(strResult))
                return null;
            //取返回结果的参数结构
            //计算返回值的理论长度
            string[] arrpm = strpms.Split(';');
            int ilength = 0;
            int verlength = 0;  //校验字的字节数
            int bit0len = 0;    //校验字的字节数
            int bit1len = 0;    //校验字的字节数
            for (int i = 0; i < arrpm.Length; i++)
            {
                NameObjectList lst = NameValueTag(arrpm[i]);
                if (i == 0) bit0len = basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8;
                if (i == arrpm.Length - 1) bit1len = basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8;
                if (lst.Keys[0] == "{校验字}") verlength = basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8;
                ilength = ilength + basefun.toIntval(lst.AllStringValues[1].ToLower()) / 8; //字节数
            }
            if (strResult.Length / 2 != ilength) return null;                             //如果返回字符串长度不大于8，则无效返回
            //校验字,如果不对，则返回空

            CrcClass crc = new CrcClass();
            string vstr = strResult.Substring(bit0len * 2, strResult.Length - bit0len * 2 - bit1len * 2 - verlength * 2);
            if ("0" == devobj.Paritymode)
                vstr = exchangeLH(crc.CRCXOR(vstr), devobj.LH);
            else
                vstr = exchangeLH(crc.CRCSUM(vstr), devobj.LH);
            if (strResult.Substring(strResult.Length - bit1len * 2 - verlength * 2, verlength * 2).ToLower() != vstr.ToLower())
                return null;

            string[] strResults = String16ToArry(strResult);
            //根据转出的格式要求处理返回值集合
            string svalue = getValByHmFormat(strpms, strResults, devobj, ref state);
            //string rdata = strRtnData.Substring(4, strRtnData.Length - 4);
            //if (state == "00") return String16ToArry(rdata);
            return svalue;
        }

        /// <summary>
        /// 对命令字符串进行四位异或校验
        /// </summary>
        /// <param name="str">需要进行FCS和校验的字符串</param>
        /// <returns>校验字:16进制数</returns>
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
            string s = Convert.ToString(result, 16);
            if (s.Length == 1) s = "0" + s;
            return s;
        }

        /// <summary>
        /// 由日期格式得到16进制日期字符串
        /// </summary>
        /// <param name="date"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public string GetFormatDateOf16(string sdate, string Format)
        {
            string s = "";
            if (sdate == "") return "";
            DateTime _date;
            try
            {
                _date = Convert.ToDateTime(sdate);
            }
            catch (Exception ex) { return ""; }
            switch (Format)
            {
                case "yyyyMMddhhmmss":
                case "yyMMddhhmmss":
                case "yyMMdd":
                case "hhmmss":
                case "hhmm":
                    //string y = _date.Year.ToString().Substring(2);
                    //string m = _date.Month.ToString();
                    //string d = _date.Day.ToString();
                    //string hour = _date.Hour.ToString();
                    //string min = _date.Minute.ToString();
                    //string sec = _date.Second.ToString();
                    Format = Format.Replace("hh", "HH"); //变为24进制
                    s = _date.ToString(Format);

                    s = String10To16(s);
                    break;
                default:
                    break;

            }
            return s;
        }

        /// <summary>
        /// 带格式串的日期
        /// </summary>
        /// <param name="sdate"></param>
        /// <param name="Format"></param>
        /// <param name="Formats"></param>
        /// <returns></returns>
        public string GetFormatDateOf16(string sdate, string Format, pmObj pmobj)
        {
            string s = "";
            if (sdate == "") return "";
            DateTime _date;
            try
            {
                _date = Convert.ToDateTime(sdate);
            }
            catch (Exception ex) { return ""; }
            switch (Format)
            {
                case "yyyyMMddhhmmss":
                case "yyMMddhhmmss":
                case "yyyyMMdd":
                    Format = Format.Replace("hh", "HH"); //变为24进制
                    s = _date.ToString(Format);
                    if (pmobj.Formats.IndexOf("#") > -1)
                    {
                        int n = 0;
                        string[] srcformat = sdate.Split('-');
                        string[] dstformat = pmobj.Formats.Split('#');
                        string sv = "";
                        //将每个单独元素转为2进制
                        for (int m = 0; m < srcformat.Length; m++)
                        {
                            string s2 = Convert.ToString(Convert.ToInt32(srcformat[m]), 2);
                            s2 = ComplementingBits(s2, basefun.toIntval(dstformat[m]), true, true);
                            sv = sv + s2;
                        }
                        sv = ComplementingBits(sv, pmobj.Len, true, true);
                        s = this.sVal2To16(sv, pmobj.Len / 8);
                    }
                    else
                        s = String10To16(s);
                    break;
                case "yyMMdd":
                    s = _date.ToString(Format);
                    if (pmobj.Formats.IndexOf("#") > -1)
                    {
                        int n = 0;
                        string[] srcformat = sdate.Split('-');
                        string[] dstformat = pmobj.Formats.Split('#');
                        string sv = "";
                        if (srcformat[0].Length > 2) srcformat[0] = srcformat[0].Substring(2, 2);
                        //将每个单独元素转为2进制
                        for (int m = 0; m < srcformat.Length; m++)
                        {
                            string s2 = Convert.ToString(Convert.ToInt32(srcformat[m]), 2);
                            s2 = ComplementingBits(s2, basefun.toIntval(dstformat[m]), true, true);
                            sv = sv + s2;
                        }
                        sv = ComplementingBits(sv, pmobj.Len, true, true);
                        s = this.sVal2To16(sv, pmobj.Len / 8);
                    }
                    else
                        s = String10To16(s);
                    break;
                case "hhmmss":
                case "hhmm":
                    //string y = _date.Year.ToString().Substring(2);
                    //string m = _date.Month.ToString();
                    //string d = _date.Day.ToString();
                    //string hour = _date.Hour.ToString();
                    //string min = _date.Minute.ToString();
                    //string sec = _date.Second.ToString();
                    Format = Format.Replace("hh", "HH"); //变为24进制
                    if (pmobj.Formats.IndexOf("#") > -1)
                    {
                        int n = 0;
                        string[] srcformat = sdate.Split(':');
                        string[] dstformat = pmobj.Formats.Split('#');
                        string sv = "";
                        if (pmobj.Len == 16 && dstformat[2] == "5" && basefun.toIntval(srcformat[2]) > 29)
                            srcformat[2] = "29";
                        //将每个单独元素转为2进制
                        for (int m = 0; m < srcformat.Length; m++)
                        {
                            string s2 = Convert.ToString(Convert.ToInt32(srcformat[m]), 2);
                            s2 = ComplementingBits(s2, basefun.toIntval(dstformat[m]), true, true);
                            sv = sv + s2;
                        }
                        sv = ComplementingBits(sv, pmobj.Len, true, true);
                        s = this.sVal2To16(sv, pmobj.Len / 8);
                    }
                    else
                        s = String10To16(s);
                    break;
                default:
                    break;
            }
            return s;
        }

        public string GetWeekByHZ(string strWeek)
        {
            string ret = "";
            switch (strWeek)
            {
                case "07":
                    ret = "星期日";
                    break;
                case "01":
                    ret = "星期一";
                    break;
                case "02":
                    ret = "星期二";
                    break;
                case "03":
                    ret = "星期三";
                    break;
                case "04":
                    ret = "星期四";
                    break;
                case "05":
                    ret = "星期五";
                    break;
                case "06":
                    ret = "星期六";
                    break;
            }
            return (ret);
        }

        public string GetWeekBy1(string strWeek)
        {
            string ret = "";
            switch (strWeek)
            {
                case "00":
                    ret = "星期日";
                    break;
                case "01":
                    ret = "星期一";
                    break;
                case "02":
                    ret = "星期二";
                    break;
                case "03":
                    ret = "星期三";
                    break;
                case "04":
                    ret = "星期四";
                    break;
                case "05":
                    ret = "星期五";
                    break;
                case "06":
                    ret = "星期六";
                    break;
            }
            return (ret);
        }

        public string GetWeekByEn(string strWeek)
        {
            string ret = "";
            switch (strWeek)
            {
                case "Sunday":
                    ret = "07";
                    break;
                case "Monday":
                    ret = "01";
                    break;
                case "Tuesday":
                    ret = "02";
                    break;
                case "Wednesday":
                    ret = "03";
                    break;
                case "Thursday":
                    ret = "04";
                    break;
                case "Friday":
                    ret = "05";
                    break;
                case "Saturday":
                    ret = "06";
                    break;
            }
            return (ret);
        }

        /// <summary>
        /// 字符串补位,默认高位字节补齐
        /// </summary>
        /// <param name="str">需补位的16进制字符串</param>
        /// <param name="ilen">位长度,内部做字节长度转换</param>
        /// <returns></returns>
        private string ComplementingBits(string str16, int ilen)
        {
            return ComplementingBits(str16, ilen, true, false);
        }

        /// <summary>
        /// 字符串补位,默认高位补齐
        /// </summary>
        /// <param name="str">需补位的16进制字符串</param>
        /// <param name="ilen">需要的位长度,字节时内部做字节长度转换</param>
        /// <param name="isbits">以位直接填充，否则字节对齐填充</param>
        /// <returns></returns>
        private string ComplementingBits(string str16, int ilen, Boolean isbits)
        {
            return ComplementingBits(str16, ilen, true, isbits);
        }

        /// <summary>
        /// 字符串补位
        /// </summary>
        /// <param name="str">需补位的16进制字符串</param>
        /// <param name="ilen">需要的位长度</param>
        /// <param name="direct">补位方向(true是前导、false是后置)</param>
        /// <param name="isbits">以位直接填充，否则字节对齐填充</param>
        /// <returns>填充长度后的字符串</returns>
        private string ComplementingBits(string str16, int ilen, Boolean direct, Boolean isbits)
        {
            int strlen = str16.Length;
            string space = "0";
            if (!isbits)
            {
                space = "00";
                ilen = Convert.ToInt32(Math.Ceiling(ilen / 8.0));
                if (str16.Length % 2 != 0) str16 = "0" + str16;
                strlen = str16.Length / 2;
            }
            for (int i = 0, len = ilen - strlen; i < len; i++)
            {
                if (direct)
                    str16 = space + str16;
                else
                    str16 = str16 + space;
            }
            return str16;
        }

        /// <summary>
        /// 将10进制字符串转化为16进制字符串(2位转)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string String10To16(string str)
        {
            int ilen = str.Length / 2;
            byte[] buf = new byte[ilen];
            string s = "";
            for (int i = 0; i < ilen; i++)
            {
                buf[i] = Convert.ToByte(Convert.ToInt32(str.Substring(2 * i, 2), 10));
                string x = Convert.ToString(Convert.ToInt16(buf[i].ToString()), 16);
                if (x.Length == 1) x = "0" + x;
                s = s + x;
            }
            return s;//090b050c011e
        }

        /// <summary>
        /// 将10进制字符串转化为16进制字符串(2位转)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sVal10To16(string str)
        {
            if (str == "") return "";
            string s = Convert.ToString(Convert.ToInt64(str), 16);
            if (s.Length % 2 != 0)
                s = "0" + s;
            return s;
        }
        /// <summary>
        /// 将10进制字符串转化为16进制字符串(2位转)
        /// </summary>
        /// <param name="str">10进制数字</param>
        /// <param name="len">转换字节长度,一个字节2个字符</param>
        /// <returns>转换后结果,超长去除高位,不足补0</returns>
        public string sVal10To16(string str, int len)
        {
            if (str == "") return "";
            string s = "";
            if (str.IndexOf(".") < 0)
                s = Convert.ToString(Convert.ToInt64(str), 16);
            else
            {
                long lg = Convert.ToInt64(Convert.ToDouble(str));
                s = Convert.ToString(lg, 16);
            }
            if (s.Length % 2 != 0)
                s = "0" + s;
            if (len < 1) return s;
            len *= 2;
            s = s.PadLeft(len, '0');
            return s.Substring(s.Length - len, len);
        }

        /// <summary>
        /// 高低位取反
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string exchangeLH(string str)
        {
            if (str == "") return "";
            if (str.Length % 2 == 1)
                str = "0" + str;
            int ilen = str.Length / 2;
            string s = "";
            string strbyte = "";
            for (int i = ilen - 1; i >= 0; i--)
            {
                string sx = str.Substring(2 * i, 2);
                strbyte = strbyte + "," + sx;
                s = s + sx;
            }
            return s;
        }

        /// <summary>
        /// 根据高低位转换要求进行高低位取反
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isLH"></param>
        /// <returns></returns>
        public string exchangeLH(string str, Boolean isLH)
        {
            if (!isLH) return str;
            if (str == "") return "";
            if (str.Length % 2 == 1)
                str = "0" + str;
            int ilen = str.Length / 2;
            string s = "";
            string strbyte = "";
            for (int i = ilen - 1; i >= 0; i--)
            {
                string sx = str.Substring(2 * i, 2);
                strbyte = strbyte + "," + sx;
                s = s + sx;
            }
            return s;
        }

        /// <summary>
        /// 有符号10进制字节数据str,以max最大值(负数取反正数原值)转换为16进制字节数据
        /// </summary>
        /// <param name="str">有符号10进制数据</param>
        /// <param name="max">高位符号16进制数据,如一个字节最大值FF</param>
        /// <returns></returns>
        public string sVal10To16(string str, string max)
        {
            if (str == "?") return "";
            if (string.IsNullOrEmpty(max))
                return sVal10To16(str);
            if (str == "") str = "0";
            long v = 0;
            if (str.IndexOf(".") > -1)
                v = Convert.ToInt64(Convert.ToDouble(str));
            else
                v = Convert.ToInt64(str);
            string s = "";
            if (v >= 0)
                s = Convert.ToString(v, 16).PadLeft(max.Length, '0');
            else
            {
                s = Convert.ToString(v, 2).PadLeft(max.Length * 4 - 1, '1');
                s = "1" + s.Substring(s.Length - max.Length * 4 + 1);
                s = Convert.ToString(Convert.ToInt64(s, 2), 16).PadLeft(max.Length, '0');
            }
            return s;
        }

        /// <summary>
        /// 按字节转换为16进制字符串(2位转)
        /// </summary>
        /// <param name="str">十进制字符串</param>
        /// <param name="len">字节长度</param>
        /// <returns></returns>
        public string sVal2To16(string str, int len)
        {
            if (str == "") str = "0";
            string s = Convert.ToString(Convert.ToInt32(str, 2), 16);
            if (s.Length % 2 != 0)
                s = "0" + s;
            int ilen = s.Length;
            if (ilen != len * 2)
            {
                for (int i = 0; i < len * 2 - ilen; i++)
                {
                    s = "0" + s;
                }
            }
            return s;
        }

        /// <summary>
        /// 将十进制字符串转为2进制
        /// </summary>
        /// <param name="str">输入的数值字符串</param>
        /// <returns></returns>
        public string sVal10To2(string str)
        {
            int iv = basefun.toIntval(str);
            return Convert.ToString(iv, 2);
        }

        /// <summary>
        /// 将十进制字符串转为2进制,并根据要求的字节数补位
        /// </summary>
        /// <param name="str">输入的数值字符串</param>
        /// <returns></returns>
        public string sVal10To2(string str, int bytelen)
        {
            int iv = basefun.toIntval(str);
            string s = Convert.ToString(iv, 2);
            int ierr = bytelen - s.Length;
            if (ierr <= 0) return s;
            string sx = "";
            for (int i = 0; i < ierr; i++)
                sx = sx + "0";
            return sx + s;
        }

        /// <summary>
        /// 将十六进制字符串转为十进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sVal16To10(string str16)
        {
            if (str16 == "") return "00";
            string s = Convert.ToInt64(str16, 16).ToString();
            if (s.Length % 2 != 0)
                s = "0" + s;
            return s;
        }

        public string String16To10(string str)
        {
            int ilen = str.Length / 2;
            byte[] buf = new byte[ilen];
            string s = "";
            for (int i = 0; i < ilen; i++)
            {
                buf[i] = Convert.ToByte(Convert.ToInt32(str.Substring(2 * i, 2), 16));
                string x = buf[i].ToString();
                if (x.Length == 1) x = "0" + x;
                s = s + x;
            }
            return s;//091105120130
        }

        /// <summary>
        /// 将16进制字符串转换为16进制字符数组
        /// </summary>
        /// <param name="str">16进制字符串</param>
        /// <returns></returns>
        public string[] String16ToArry(string str)
        {
            int ilen = str.Length / 2;
            string[] arr = new string[ilen];
            for (int i = 0; i < ilen; i++)
            {
                arr[i] = str.Substring(2 * i, 2);
                if (arr[i].Length == 1) arr[i] = "0" + arr[i];
            }
            return arr;
        }

        /// <summary>
        /// 将16进制字符串转换为以制式参数规定的字符数组
        /// </summary>
        /// <param name="str">16进制字符串</param>
        /// <param name="IFormat">进制</param>
        /// <returns>指定制式的字符串数组</returns>
        public string[] String16ToArry(string str, int IFormat)
        {
            if (IFormat == 0) IFormat = 10;
            int ilen = str.Length / 2;
            string[] arr = new string[ilen];
            byte[] buf = new byte[ilen];

            for (int i = 0; i < ilen; i++)
            {
                buf[i] = Convert.ToByte(Convert.ToInt32(str.Substring(2 * i, 2), IFormat));
                arr[i] = buf[i].ToString();
                if (arr[i].Length == 1) arr[i] = "0" + arr[i];
            }
            return arr;
        }

        public string StringToStrings(string str, string sign)
        {
            int ilen = str.Length / 2;
            string s = "";
            for (int i = 0; i < ilen; i++)
            {
                string sx = str.Substring(2 * i, 2);
                if (sx.Length == 1) sx = "0" + sx;
                s = s + sign + sx;
            }
            return s;
        }

        public string StringToStrings(string str, string sign, string pre, string sback)
        {
            int ilen = str.Length / 2;
            string s = "";
            for (int i = 0; i < ilen; i++)
            {
                string sx = str.Substring(2 * i, 2);
                if (sx.Length == 1) sx = "0" + sx;
                sx = pre + sx + sback;
                s = s + sign + sx;
            }
            return s;
        }

        #endregion


        #region

        /// <summary>
        /// 更换特殊字符
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="srcsign">原字符</param>
        /// <param name="destsign">需更换的字符</param>
        /// <returns>输出更换后字符串</returns>
        public string SpecialExchange(string str, string srcsign, string destsign)
        {
            int ilen = str.Length / 2;
            string s = "";
            string strbyte = "";
            for (int i = 0; i < ilen; i++)
            {
                strbyte = str.Substring(2 * i, 2);
                strbyte = strbyte.Replace(srcsign.ToLower(), destsign.ToLower());
                s += strbyte;
            }
            return s;
        }
        /// <summary>
        /// 替换转义字符,忽略大小写
        /// </summary>
        /// <param name="data">需要处理的数据</param>
        /// <param name="srcsign">需替换的样例字符</param>
        /// <param name="destsign">更换的实际字符</param>
        /// <returns>返回转义处理后的字符</returns>
        public string SpecialRestore(string data, string srcsign, string destsign)
        {
            if (string.IsNullOrEmpty(data))
                return data;
            data = data.Replace(srcsign.ToLower(), destsign.ToLower());
            data = data.Replace(srcsign.ToUpper(), destsign.ToUpper());
            return data;
        }

        public string EateryInitCmd(string sn, string strcmd)
        {
            string stype = "01";
            string cmd = strcmd;
            string cmdlen = "00";
            string slen = sVal10To16(Convert.ToString(Convert.ToInt32(cmdlen, 16) + 2));
            string value = slen + cmd + cmdlen;
            string xor = CRCXOR(value);
            return string.Format("c0{0}{1}{2}{3}c0", sn, stype, value, xor);
        }

        /// <summary>
        /// 根据设备ID,命令字,命令长度，数据字符获得消费命令字符串,含限制位数
        /// </summary>
        /// <param name="sn">设备ID</param>
        /// <param name="cmd">执行命令</param>
        /// <param name="stype">控制类型</param>
        /// <param name="cmdlen">命令字长</param>
        /// <param name="ilmtbytes">限制字数</param>
        /// <param name="str">数据串</param>
        /// <returns>消费命令字符串</returns>
        public string EateryCmdStrintg(string sn, string cmd, string stype, string cmdlen, int ilmtbytes, string str)
        {
            string slen = sVal10To16(Convert.ToString(Convert.ToInt32(cmdlen, 16) + 2));
            string cmddata = "";
            string[] pm = str.Split(';');
            for (int i = 0; i < pm.Length; i++)
            {
                cmddata += setValByHmFormat(pm[i]);
            }
            if (str == "") cmddata = "";
            int ilen = cmddata.Length;
            if (cmddata.Length < ilmtbytes)
            {
                for (int i = 0; i < ilmtbytes - ilen; i++)
                {
                    cmddata = cmddata + "0";
                }
            }
            //所有原始数据做异或校验
            string value = slen + cmd + cmdlen + cmddata;
            string xor = this.CRCXOR(sn + stype + value);

            //处理转义字符
            sn = SpecialExchange(sn, "db", "dddb"); sn = SpecialExchange(sn, "c0", "dcdb");
            stype = SpecialExchange(stype, "db", "dddb"); stype = SpecialExchange(stype, "c0", "dcdb");
            slen = SpecialExchange(slen, "db", "dddb"); slen = SpecialExchange(slen, "c0", "dcdb");
            cmd = SpecialExchange(cmd, "db", "dddb"); cmd = SpecialExchange(cmd, "c0", "dcdb");
            cmdlen = SpecialExchange(cmdlen, "db", "dddb"); cmdlen = SpecialExchange(cmdlen, "c0", "dcdb");
            xor = SpecialExchange(xor, "db", "dddb"); xor = SpecialExchange(xor, "c0", "dcdb");

            for (int i = 0; i < pm.Length; i++)
            {
                string sv = setValByHmFormat(pm[i]);
                sv = SpecialExchange(sv, "db", "dddb");
                sv = SpecialExchange(sv, "c0", "dcdb");
                cmddata += sv;
            }
            if (str == "") cmddata = "";
            value = slen + cmd + cmdlen + cmddata;
            string strcmd = string.Format("c0{0}{1}{2}{3}c0", sn, stype, value, xor);
            return strcmd;
        }

        /// <summary>
        /// 根据设备ID,命令字,命令长度，数据字符获得消费命令字符串,不含限制位数
        /// </summary>
        /// <param name="sn">设备ID</param>
        /// <param name="cmd">执行命令</param>
        /// <param name="stype">控制类型</param>
        /// <param name="cmdlen">命令字长</param>
        /// <param name="str">数据串</param>
        /// <returns>消费命令字符串</returns>
        public string EateryCmdStrintg(string sn, string cmd, string stype, string cmdlen, string str)
        {
            string slen = sVal10To16(Convert.ToString(Convert.ToInt32(cmdlen, 16) + 2));
            string cmddata = "";
            string[] pm = str.Split(';');
            for (int i = 0; i < pm.Length; i++)
            {
                cmddata += setValByHmFormat(pm[i]);
            }
            if (str == "") cmddata = "";

            //所有原始数据做异或校验
            string value = slen + cmd + cmdlen + cmddata;
            string xor = this.CRCXOR(sn + stype + value);

            //处理转义字符
            sn = SpecialExchange(sn, "db", "dddb"); sn = SpecialExchange(sn, "c0", "dcdb");
            stype = SpecialExchange(stype, "db", "dddb"); stype = SpecialExchange(stype, "c0", "dcdb");
            slen = SpecialExchange(slen, "db", "dddb"); slen = SpecialExchange(slen, "c0", "dcdb");
            cmd = SpecialExchange(cmd, "db", "dddb"); cmd = SpecialExchange(cmd, "c0", "dcdb");
            cmdlen = SpecialExchange(cmdlen, "db", "dddb"); cmdlen = SpecialExchange(cmdlen, "c0", "dcdb");
            xor = SpecialExchange(xor, "db", "dddb"); xor = SpecialExchange(xor, "c0", "dcdb");

            cmddata = "";
            for (int i = 0; i < pm.Length; i++)
            {
                string sv = setValByHmFormat(pm[i]);
                sv = SpecialExchange(sv, "db", "dddb");
                sv = SpecialExchange(sv, "c0", "dcdb");
                cmddata += sv;
            }
            if (str == "") cmddata = "";

            value = slen + cmd + cmdlen + cmddata;
            string strcmd = string.Format("c0{0}{1}{2}{3}c0", sn, stype, value, xor);
            return strcmd;
        }


        /// <summary>
        /// 根据设备ID,命令格式文档构建命令字符串
        /// </summary>
        /// <param name="sn">设备ID</param>
        /// <param name="cmd">执行命令</param>
        /// <param name="stype">控制类型</param>
        /// <param name="cmdlen">命令字长</param>
        /// <param name="ilmtbytes">限制字数</param>
        /// <param name="str">数据串</param>
        /// <returns>消费命令字符串</returns>
        public string ComCmdStrintg(string sn, string cmd, string stype, string cmdlen, int ilmtbytes, string str)
        {
            string slen = sVal10To16(Convert.ToString(Convert.ToInt32(cmdlen, 16) + 2));
            string cmddata = "";
            string[] pm = str.Split(';');
            for (int i = 0; i < pm.Length; i++)
            {
                cmddata += setValByHmFormat(pm[i]);
            }
            if (str == "") cmddata = "";
            int ilen = cmddata.Length;
            if (cmddata.Length < ilmtbytes)
            {
                for (int i = 0; i < ilmtbytes - ilen; i++)
                {
                    cmddata = cmddata + "0";
                }
            }
            //所有原始数据做异或校验
            string value = slen + cmd + cmdlen + cmddata;
            string xor = this.CRCXOR(sn + stype + value);

            //处理转义字符
            sn = SpecialExchange(sn, "db", "dddb"); sn = SpecialExchange(sn, "c0", "dcdb");
            stype = SpecialExchange(stype, "db", "dddb"); stype = SpecialExchange(stype, "c0", "dcdb");
            slen = SpecialExchange(slen, "db", "dddb"); slen = SpecialExchange(slen, "c0", "dcdb");
            cmd = SpecialExchange(cmd, "db", "dddb"); cmd = SpecialExchange(cmd, "c0", "dcdb");
            cmdlen = SpecialExchange(cmdlen, "db", "dddb"); cmdlen = SpecialExchange(cmdlen, "c0", "dcdb");
            xor = SpecialExchange(xor, "db", "dddb"); xor = SpecialExchange(xor, "c0", "dcdb");

            for (int i = 0; i < pm.Length; i++)
            {
                string sv = setValByHmFormat(pm[i]);
                sv = SpecialExchange(sv, "db", "dddb");
                sv = SpecialExchange(sv, "c0", "dcdb");
                cmddata += sv;
            }
            if (str == "") cmddata = "";
            value = slen + cmd + cmdlen + cmddata;
            string strcmd = string.Format("c0{0}{1}{2}{3}c0", sn, stype, value, xor);
            return strcmd;
        }

        /// <summary>
        /// 由串口返回的字符串，按规划的模式赋值
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="udpinfos">串口返回的数据</param>
        /// <returns></returns>
        public string getValByHmFormat(string formats, string[] udpinfos)
        {
            if (udpinfos == null || formats == null) return null;
            string rtn = "";
            string[] xformats = formats.Split(';');
            int ipos = 0;
            for (int i = 0; i < xformats.Length; i++)
            {
                string[] arr = xformats[i].Split(',');
                string vname = getVarNameByHmStr(arr[0]);
                int istart = toIntval(valtag(xformats[i], "start"));
                int ilen = toIntval(valtag(xformats[i], "len"));

                string format = valtag(xformats[i], "sFormat");
                int ibytes = ilen / 8;
                string datatype = valtag(xformats[i], "datatype");
                int icale = basefun.toIntval(valtag(xformats[i], "scale"));
                if (icale == 0) icale = 1;
                string s = getValueByUpdInfos(udpinfos, ipos, ibytes, datatype);
                ipos = ipos + ilen;
                switch (datatype.ToLower())
                {
                    case "datetime":
                        string[] a = String16ToArry(s);
                        switch (format)
                        {
                            case "hhmm":
                                if (a.Length == 2)
                                    s = sVal16To10(a[0]) + ":" + sVal16To10(a[1]);
                                break;
                            case "yyyyMMddhhmmss":
                            case "yyMMddhhmmss":
                                if (a.Length > 6)
                                    s = sVal16To10(a[0]) + "-" + sVal16To10(a[1]) + "-" + sVal16To10(a[2]) +
                                        " " + sVal16To10(a[3]) + ":" + sVal16To10(a[4]) + ":" + sVal16To10(a[5]);
                                break;
                            case "yyyyMMddWWhhmmss":
                            case "yyMMddWWhhmmss":
                                if (a.Length > 6)
                                    s = sVal16To10(a[0]) + "-" + sVal16To10(a[1]) + "-" + sVal16To10(a[2]) +
                                        " " + sVal16To10(a[4]) + ":" + sVal16To10(a[5]) + ":" + sVal16To10(a[6])
                                    + " " + GetWeekByHZ(sVal16To10(a[3]));
                                break;
                            case "yyMMdd":
                                if (a.Length == 3)
                                    s = sVal16To10(a[0]) + "-" + sVal16To10(a[1]) + "-" + sVal16To10(a[2]);
                                break;
                            case "hhmmss":
                                if (a.Length == 3)
                                    s = sVal16To10(a[0]) + ":" + sVal16To10(a[1]) + ":" + sVal16To10(a[3]);
                                break;
                        }
                        break;
                    case "float":
                    case "double":
                        s = Convert.ToInt32(sVal16To10(s)).ToString();
                        if (format == "") format = "{0:.00}";
                        if (format.IndexOf("{") > -1)
                            s = string.Format(format, Convert.ToDouble(s) / icale);
                        else
                            s = string.Format("{" + format + "}", Convert.ToDouble(s) / icale);
                        break;
                    case "bit":
                        s = sVal16To10(s);
                        s = Convert.ToString(Convert.ToInt32(s), 2);
                        break;
                    case "integer":
                    case "int":
                        int _iv = Convert.ToInt32(sVal16To10(s)) / icale;
                        s = _iv.ToString();
                        break;
                    default:
                        s = Convert.ToInt32(sVal16To10(s)).ToString();
                        break;
                }
                xformats[i] = setvaltag(xformats[i], vname, s);
                rtn = rtn + ";" + xformats[i];
            }
            if (rtn.Length > 0)
                rtn = rtn.Substring(1);
            return rtn;
        }

        /// <summary>
        /// 由串口返回的字符串及hmtag字符串，按规划的模式赋值.由布尔值noHE分辨是否考虑帧头和帧尾
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="udpinfos">串口返回的数据</param>
        /// <param name="devobj">设备参数类</param>
        /// <param name="noHE">布尔值，true- 不包含帧头和帧尾</param>
        /// <param name="vtag">返回的状态值</param>
        /// <returns>返回的数据字符串</returns>
        public string getValByHmFormat(string formats, string[] udpinfos, devObj devobj, Boolean noHE, ref string vtag)
        {
            if (udpinfos == null || formats == null) return null;
            string rtn = "";
            string[] xformats = formats.Split(';');
            int ipos = 0;
            string rtns = "";
            for (int i = 0; i < xformats.Length; i++)
            {
                if (noHE)
                    if (i == 0 || i == xformats.Length - 1) continue;
                string[] arr = xformats[i].Split(',');
                string vname = getVarNameByHmStr(arr[0]);
                pmObj pmobj = new pmObj();
                pmobj.Name = getVarNameByHmStr(arr[0]);
                pmobj.Len = toIntval(valtag(xformats[i], "{len}"));
                pmobj.DataType = valtag(xformats[i], "{datatype}");
                pmobj.Format = valtag(xformats[i], "{format}");
                pmobj.Formats = valtag(xformats[i], "{formats}");
                pmobj.Scale = toFloatValue(valtag(xformats[i], "{scale}")) == 0.0 ? 1.0 : toFloatValue(valtag(xformats[i], "{scale}"));
                pmobj.Add = toIntval(valtag(xformats[i], "{add}"));
                pmobj.SubItems = valtag(xformats[i], "{subitems}");
                string strlh = valtag(xformats[i], "{lh}");
                if (strlh == "") pmobj.LH = devobj.LH;

                /*
                int ilen = toIntval(valtag(xformats[i], "{len}"));
                string format = valtag(xformats[i], "{format}");
                string aformats = valtag(xformats[i], "{formats}");
                int ibytes = ilen / 8;
                string datatype = valtag(xformats[i], "{datatype}");
                int icale = basefun.toIntval(valtag(xformats[i], "{scale}"));
                if (icale == 0) icale = 1;
                 */
                string s = getValueByUpdInfos(udpinfos, ipos, pmobj);
                ipos = ipos + pmobj.Len;
                switch (pmobj.DataType.ToLower())
                {
                    case "datetime":
                    case "date":
                    case "dateword":
                        if (pmobj.Format.EndsWith("~BCD"))
                            s = basefun.toDatetimeBCD(s, pmobj.Format);
                        else
                        {
                            s = this.exchangeLH(s, pmobj.LH);
                            s = basefun.toDatetime(s, pmobj.Format);
                        }
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        break;
                    case "float":
                    case "double":
                        s = this.exchangeLH(s, pmobj.LH);
                        if (pmobj.Format.StartsWith("±"))
                        {
                            pmobj.Format = pmobj.Format.Substring(1);
                            int h = Convert.ToInt16(s.Substring(0, 1), 16);
                            if (h > 7)
                                s = s.PadLeft(16, 'F');
                        }
                        s = Convert.ToInt64(sVal16To10(s)).ToString();
                        if (pmobj.Format == "") pmobj.Format = "{0:.00}";
                        if (pmobj.Format.IndexOf("{") > -1)
                            s = string.Format(pmobj.Format, Convert.ToDouble(s) * pmobj.Scale);
                        else
                            s = string.Format("{" + pmobj.Format + "}", Convert.ToDouble(s) * pmobj.Scale);
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        break;
                    case "float16":
                    case "double16":
                        s = this.exchangeLH(s, pmobj.LH);
                        s = Convert.ToInt32(sVal16To10(s)).ToString();
                        if (pmobj.Format == "") pmobj.Format = "{0:.00}";
                        if (pmobj.Format.IndexOf("{") > -1)
                            s = string.Format(pmobj.Format, Convert.ToDouble(s) * pmobj.Scale);
                        else
                            s = string.Format("{" + pmobj.Format + "}", Convert.ToDouble(s) * pmobj.Scale);
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        break;
                    case "bit":

                        s = this.exchangeLH(s, pmobj.LH);
                        s = sVal16To10(s);
                        s = ComplementingBits(Convert.ToString(Convert.ToInt64(s), 2), pmobj.Len, true, true);
                        //Billy
                        /*
                        if (s == "10000001" || s == "10000000")
                        {
                            s = "00000000";
                        }
                        */
                        //脱机处理=1#String#+脱机时间=15#Double#{0:.0}
                        if (pmobj.SubItems != "")
                        {
                            string[] subs = pmobj.SubItems.Split('+');
                            int isubs = 0;
                            for (int m = 0; m < subs.Length; m++)
                            {
                                string[] asubs = subs[m].Split('=');
                                string[] asubpms = asubs[1].Split('#');
                                int iSublen = basefun.toIntval(asubpms[1]);
                                string subDataType = asubpms[2];
                                string subFormat = asubpms[3];
                                string subValue = s.Substring(isubs, iSublen);
                                switch (subDataType.ToLower())
                                {
                                    case "datetime":
                                    case "date":
                                        subValue = Convert.ToString(Convert.ToInt64(subValue, 2), 16);
                                        subValue = ComplementingBits(subValue, 16);
                                        if (subFormat == "") subFormat = "hhmm";
                                        subValue = InfoTomyDate(subValue, subFormat, true);
                                        break;
                                    case "double":
                                        subValue = Convert.ToInt32(subValue, 2).ToString();
                                        if (subFormat == "") subFormat = "{0:.00}";
                                        //此处未加比例，有风险
                                        if (subFormat.IndexOf("{") > -1)
                                        {
                                            subValue = string.Format(subFormat, Convert.ToDouble(subValue) / 10);
                                        }
                                        else
                                            subValue = string.Format("{" + subFormat + "}", Convert.ToDouble(subValue) * pmobj.Scale);

                                        break;
                                    case "integer":
                                    case "int":
                                        subValue = Convert.ToInt32(subValue, 2).ToString();
                                        break;
                                    default:
                                        subValue = Convert.ToInt32(subValue, 2).ToString();
                                        break;
                                }
                                rtns = basefun.setvaltag(rtns, asubs[0], subValue);
                                isubs = isubs + iSublen;
                            }
                        }
                        else
                        {
                            rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        }
                        break;
                    case "integer":
                    case "int":
                        s = this.exchangeLH(s, pmobj.LH);
                        long _iv = Convert.ToInt64(Convert.ToInt64(sVal16To10(s)) * pmobj.Scale);
                        s = _iv.ToString();
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        break;
                    case "asc":
                        s = this.exchangeLH(s, pmobj.LH);
                        byte[] bs = new byte[s.Length / 2];
                        for (int p = 0; p < bs.Length; p++)
                            bs[p] = Convert.ToByte(s.Substring(p * 2, 2), 16);
                        s = Encoding.GetEncoding("GB2312").GetString(bs);
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s.Trim());
                        break;
                    case "word":
                        s = this.exchangeLH(s, pmobj.LH);
                        if (vname == "{状态}")
                        {
                            s = ResKeywords(devobj, s);
                            vtag = s;
                            if (s.Contains("操作成功") || s.Contains("无新记录"))
                                rtns = basefun.setvaltag(rtns, "Success", "true");
                            else
                                rtns = basefun.setvaltag(rtns, "Success", "false");
                        }
                        else
                        {
                            switch (pmobj.Format)
                            {
                                case "{0}-{1}-{2}-{3}-{4}":
                                    string[] arrip = this.String16ToArry(s, 16);
                                    string str = "";
                                    for (int m = 0; m < arrip.Length; m++)
                                    {
                                        str = str + "-" + this.sVal10To16(basefun.toIntval(arrip[m]).ToString());
                                    }
                                    s = str.Substring(1, str.Length - 1);
                                    break;
                                case "db":
                                    s = s.Substring(1);
                                    break;
                            }
                        }
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        break;
                    default:
                        s = this.exchangeLH(s, pmobj.LH);
                        if (pmobj.Format == "{0}.{1}.{2}.{3}")
                        {
                            string[] arrip = this.String16ToArry(s, 16);
                            string str = "";
                            for (int m = 0; m < arrip.Length; m++)
                            {
                                str = str + "." + basefun.toIntval(arrip[m]).ToString();
                            }
                            s = str.Substring(1, str.Length - 1);
                        }
                        else
                            s = Convert.ToInt64(sVal16To10(s)).ToString();
                        rtns = basefun.setvaltag(rtns, pmobj.Name, s);
                        break;
                }
                xformats[i] = setvaltag(xformats[i], vname, s);
                rtn = rtn + ";" + xformats[i];
            }
            //门禁时有返回则成功
            if (!string.IsNullOrEmpty(rtns) && "门禁" == devobj.Buss)
                rtns = basefun.setvaltag(rtns, "Success", "true");
            if (rtn.Length > 0)
                rtn = rtn.Substring(1);
            rtns = rtns.Trim();
            return rtns;
        }

        /// <summary>
        /// 由串口返回的字符串及hmtag字符串，按规划的模式赋值
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="udpinfos">串口返回的数据</param>
        /// <returns></returns>
        public string getValByHmFormat(string formats, string[] udpinfos, devObj devobj, ref string vtag)
        {
            if (udpinfos == null || formats == null) return null;
            string rtn = "";
            string[] xformats = formats.Split(';');
            int ipos = 0;
            for (int i = 0; i < xformats.Length; i++)
            {
                string[] arr = xformats[i].Split(',');
                string vname = getVarNameByHmStr(arr[0]);
                pmObj pmobj = new pmObj();
                pmobj.Name = getVarNameByHmStr(arr[0]);
                pmobj.Len = toIntval(valtag(xformats[i], "{len}"));
                pmobj.DataType = valtag(xformats[i], "{datatype}");
                pmobj.Format = valtag(xformats[i], "{format}");
                pmobj.Formats = valtag(xformats[i], "{formats}");
                pmobj.Scale = toFloatValue(valtag(xformats[i], "{scale}")) == 0 ? 1 : toFloatValue(valtag(xformats[i], "{scale}"));
                pmobj.Add = toIntval(valtag(xformats[i], "{add}"));

                /*
                int ilen = toIntval(valtag(xformats[i], "{len}"));
                string format = valtag(xformats[i], "{format}");
                string aformats = valtag(xformats[i], "{formats}");
                int ibytes = ilen / 8;
                string datatype = valtag(xformats[i], "{datatype}");
                int icale = basefun.toIntval(valtag(xformats[i], "{scale}"));
                if (icale == 0) icale = 1;
                 */
                string s = getValueByUpdInfos(udpinfos, ipos, pmobj);

                ipos = ipos + pmobj.Len;
                switch (pmobj.DataType.ToLower())
                {
                    case "datetime":
                        s = InfoTomyDate(s, pmobj, devobj);
                        break;
                    case "float":
                    case "double":
                        s = this.exchangeLH(s, devobj.LH);
                        s = Convert.ToInt32(sVal16To10(s)).ToString();
                        if (pmobj.Format == "") pmobj.Format = "{0:.00}";
                        if (pmobj.Format.IndexOf("{") > -1)
                            s = string.Format(pmobj.Format, Convert.ToDouble(s) * pmobj.Scale);
                        else
                            s = string.Format("{" + pmobj.Format + "}", Convert.ToDouble(s) * pmobj.Scale);
                        break;
                    case "bit":
                        s = this.exchangeLH(s, devobj.LH);
                        s = sVal16To10(s);
                        s = ComplementingBits(Convert.ToString(Convert.ToInt64(s), 2), pmobj.Len, true);
                        break;
                    case "integer":
                    case "int":
                        s = this.exchangeLH(s, devobj.LH);
                        long _iv = Convert.ToInt64(Convert.ToInt64(sVal16To10(s)) * pmobj.Scale);
                        s = _iv.ToString();
                        break;
                    case "string":
                        s = this.exchangeLH(s, devobj.LH);
                        break;
                    default:
                        s = this.exchangeLH(s, devobj.LH);
                        s = Convert.ToInt64(sVal16To10(s)).ToString();
                        break;
                }
                xformats[i] = setvaltag(xformats[i], vname, s);
                vtag = setvaltag(vtag, vname, s);
                rtn = rtn + ";" + xformats[i];
            }
            if (rtn.Length > 0)
                rtn = rtn.Substring(1);
            return rtn;
        }

        /// <summary>
        /// 16进制字符串转日期
        /// </summary>
        /// <param name="s">16进制字符串</param>
        /// <param name="format">格式</param>
        /// <returns>日期</returns>
        private string InfoTomyDate(string s, string format)
        {
            DateTime mydate = System.DateTime.Now;
            string yy = mydate.Year.ToString().Substring(0, 2);
            string[] a = String16ToArry(s, 16);
            switch (format)
            {
                case "yyMMdd":
                    if (a.Length == 3)
                        s = a[0] + "-" + a[1] + "-" + a[2];
                    break;
                case "yyyyMMdd":
                    if (a.Length == 3)
                        s = yy + a[0] + "-" + a[1] + "-" + a[2];
                    break;
                case "hhmmss":
                    if (a.Length == 3)
                        s = a[0] + ":" + a[1] + ":" + a[3];
                    break;
                case "hhmm":
                    if (a.Length == 2)
                        s = a[0] + ":" + a[1];
                    break;
                case "yyMMddhhmmss":
                    if (a.Length > 5)
                        s = a[0] + "-" + a[1] + "-" + a[2] + " " + a[3] + ":" + a[4] + ":" + a[5];
                    break;
                case "yyyyMMddhhmmss":
                    if (a.Length > 5)
                        s = yy + a[0] + "-" + a[1] + "-" + a[2] + " " + a[3] + ":" + a[4] + ":" + a[5];
                    break;
            }
            return s;
        }
        /// <summary>
        /// 16进制字符串转日期
        /// </summary>
        /// <param name="s">16进制字符串</param>
        /// <param name="format">格式</param>
        /// /// <param name="Is16">是否16进制</param>
        /// <returns>日期</returns>
        private string InfoTomyDate(string s, string format, Boolean Is16)
        {
            DateTime mydate = System.DateTime.Now;
            string yy = mydate.Year.ToString().Substring(0, 2);
            string[] a = String16ToArry(s, Is16 ? 16 : 10);
            switch (format)
            {
                case "yyMMdd":
                    if (a.Length == 3)
                        s = a[0] + "-" + a[1] + "-" + a[2];
                    break;
                case "yyyyMMdd":
                    if (a.Length == 3)
                        s = yy + a[0] + "-" + a[1] + "-" + a[2];
                    break;
                case "hhmmss":
                    if (a.Length == 3)
                        s = a[0] + ":" + a[1] + ":" + a[3];
                    break;
                case "hhmm":
                    if (a.Length == 2)
                        s = a[0] + ":" + a[1];
                    break;
                case "yyMMddhhmmss":
                    if (a.Length > 5)
                        s = a[0] + "-" + a[1] + "-" + a[2] + " " + a[3] + ":" + a[4] + ":" + a[5];
                    break;
                case "yyyyMMddhhmmss":
                    if (a.Length > 5)
                        s = yy + a[0] + "-" + a[1] + "-" + a[2] + " " + a[3] + ":" + a[4] + ":" + a[5];
                    break;
            }
            return s;
        }

        private string InfoTomyDate(string s, pmObj pmobj, devObj devobj)
        {
            DateTime mydate = System.DateTime.Now;
            string yy = mydate.Year.ToString().Substring(0, 2);
            string[] a = String16ToArry(s, 16);
            switch (pmobj.Format.Trim())
            {
                case "yyMMdd":
                case "yyyyMMdd":
                    a = String16ToArry(s);
                    if (pmobj.Formats != "")
                    {
                        int n = 0;
                        string[] yformat = pmobj.Formats.Split('#');
                        for (int m = 0; m < yformat.Length; m++)
                            n = n + basefun.toIntval(yformat[m]);
                        n = n / 8;
                        s = "";
                        for (int m = 0; m < n; m++) s = s + a[m];
                        s = this.exchangeLH(s, devobj.LH);       //转置
                        string[] ax = String16ToArry(s);
                        //转换为二进制
                        s = "";
                        for (int m = 0; m < ax.Length; m++)
                        {
                            string s2 = Convert.ToString(Convert.ToInt32(sVal16To10(ax[m])), 2);
                            s2 = ComplementingBits(s2, 8, true, true);
                            s = s + s2;
                        }
                        //处理格式位数据
                        string[] datas = new string[yformat.Length];
                        string strdata = "";
                        int im = 0;
                        for (int m = 0; m < yformat.Length; m++)
                        {
                            string str0 = s.Substring(im, basefun.toIntval(yformat[m]));
                            datas[m] = this.sVal16To10(this.sVal2To16(str0, 1));
                            im = im + basefun.toIntval(yformat[m]);
                            strdata = strdata + "-" + datas[m];
                        }
                        s = strdata.Substring(1);
                    }
                    else
                    {
                        if (a.Length == 3)
                        {
                            s = "";
                            for (int i = 0; i < 3; i++)
                                s += "-" + Convert.ToInt16(a[i], 16).ToString();
                            s = s.Substring(1);
                        }
                    }
                    if (pmobj.Format.Trim() == "yyyyMMdd")
                        s = yy + s;
                    break;
                case "hhmmss":
                    a = String16ToArry(s);
                    if (pmobj.Formats != "")
                    {
                        int n = 0;
                        string[] yformat = pmobj.Formats.Split('#');
                        for (int m = 0; m < yformat.Length; m++)
                            n = n + basefun.toIntval(yformat[m]);
                        n = n / 8;
                        s = "";
                        for (int m = 0; m < n; m++) s = s + a[m];
                        s = this.exchangeLH(s, devobj.LH);       //转置
                        string[] ax = String16ToArry(s);
                        //转换为二进制
                        s = "";
                        for (int m = 0; m < ax.Length; m++)
                        {
                            string s2 = Convert.ToString(Convert.ToInt32(sVal16To10(ax[m])), 2);
                            s2 = ComplementingBits(s2, 8, true, true);
                            s = s + s2;
                        }
                        //处理格式位数据
                        string[] datas = new string[yformat.Length];
                        string strdata = "";
                        int im = 0;
                        for (int m = 0; m < yformat.Length; m++)
                        {
                            string str0 = s.Substring(im, basefun.toIntval(yformat[m]));
                            datas[m] = this.sVal16To10(this.sVal2To16(str0, 1));
                            im = im + basefun.toIntval(yformat[m]);
                            strdata = strdata + ":" + datas[m];
                        }
                        s = strdata.Substring(1);
                    }
                    else
                    {
                        if (a.Length == 3)
                            s = a[0] + ":" + a[1] + ":" + a[3];
                    }
                    break;
                case "hhmm":
                    if (a.Length == 2)
                        s = a[0] + ":" + a[1];
                    break;
                case "yyMMddhhmmss":
                    if (a.Length > 5)
                        s = a[0] + "-" + a[1] + "-" + a[2] + " " + a[3] + ":" + a[4] + ":" + a[5];
                    break;
                case "yyyyMMDD":
                case "YYYYMMDD":
                    if (a.Length > 2)
                        s = yy + a[0] + "-" + a[1] + "-" + a[2];
                    break;
                case "yyyyMMddhhmmss":
                    if (a.Length > 5)
                        s = yy + a[0] + "-" + a[1] + "-" + a[2] + " " + a[3] + ":" + a[4] + ":" + a[5];
                    break;
                case "yyyyMMdd+hhmmss":   //日期+时间型，需联合位格式进行处理,例如位格式为 7#4#5+5#6#5
                    string[] xformat = null;
                    string[] tformat = null;
                    a = String16ToArry(s);
                    if (pmobj.Formats.IndexOf("+") > -1)
                    {
                        xformat = pmobj.Formats.Split('+');
                        tformat = pmobj.Format.Split('+');
                    }
                    string[] arrDatas = new string[tformat.Length];
                    int mn = 0;
                    for (int ix = 0; ix < xformat.Length; ix++)
                    {
                        int n = 0;
                        string[] yformat = xformat[ix].Split('#');

                        for (int m = 0; m < yformat.Length; m++) n = n + basefun.toIntval(yformat[m]);
                        n = n / 8; s = "";
                        for (int m = mn; m < n + mn; m++) s = s + a[m];
                        mn = mn + n;
                        s = this.exchangeLH(s, devobj.LH);       //转置
                        string[] ax = String16ToArry(s);
                        //转换为二进制
                        s = "";
                        for (int m = 0; m < ax.Length; m++)
                        {
                            string s2 = Convert.ToString(Convert.ToInt32(sVal16To10(ax[m])), 2);
                            s2 = ComplementingBits(s2, 8, true, true);
                            s = s + s2;
                        }
                        //处理格式位数据
                        string[] datas = new string[yformat.Length];
                        string strdata = "";
                        int im = 0;
                        for (int m = 0; m < yformat.Length; m++)
                        {
                            string str0 = s.Substring(im, basefun.toIntval(yformat[m]));
                            datas[m] = this.sVal16To10(this.sVal2To16(str0, 1));
                            im = im + basefun.toIntval(yformat[m]);
                            if (tformat[ix] == "yyyyMMdd") strdata = strdata + "-" + datas[m];
                            else strdata = strdata + ":" + datas[m];
                        }
                        arrDatas[ix] = strdata.Substring(1, strdata.Length - 1);
                    }
                    s = yy + arrDatas[0] + " " + arrDatas[1];
                    break;
                case "yyyyMMddWWhhmmss":
                    if (pmobj.DataType == "dateword")
                    {
                        string[] a1 = String16ToArry(s);
                        switch (devobj.WeekRule)
                        {
                            case "1":
                                if (a1.Length > 6)
                                    s = yy + a1[0] + "-" + a1[1] + "-" + a1[2] + " " + a1[4] + ":" + a1[5] + ":" + a1[6]
                                    + " " + GetWeekBy1(a1[3]);
                                break;
                            default:
                                if (a.Length > 6)
                                    s = yy + a1[0] + "-" + a1[1] + "-" + a1[2] + " " + a1[4] + ":" + a1[5] + ":" + a1[6]
                                    + " " + this.GetWeekByHZ(a1[3]);
                                break;
                        }
                    }
                    else
                    {
                        switch (devobj.WeekRule)
                        {
                            case "1":
                                if (a.Length > 6)
                                    s = yy + a[0] + "-" + a[1] + "-" + a[2] + " " + a[4] + ":" + a[5] + ":" + a[6]
                                    + " " + GetWeekBy1(a[3]);
                                break;
                            default:
                                if (a.Length > 6)
                                    s = yy + a[0] + "-" + a[1] + "-" + a[2] + " " + a[4] + ":" + a[5] + ":" + a[6]
                                    + " " + this.GetWeekByHZ(a[3]);
                                break;
                        }
                    }
                    break;
                case "yyMMddWWhhmmss":
                    switch (devobj.WeekRule)
                    {
                        case "1":
                            if (a.Length > 6)
                                s = a[0] + "-" + a[1] + "-" + a[2] + " " + a[4] + ":" + a[5] + ":" + a[6]
                                + " " + GetWeekBy1(a[3]);
                            break;
                        case "2":
                            if (a.Length > 6)
                                s = a[0] + "-" + a[1] + "-" + a[2] + " " + a[4] + ":" + a[5] + ":" + a[6]
                                + " " + this.GetWeekByHZ(a[3]);
                            break;
                    }
                    break;
            }
            return s;
        }

        public string getValueByUpdInfos(string[] udpinfos, int ipos, int ibyte, string datatype)
        {

            string s = "";
            ipos = ipos / 8;

            for (int i = ipos; i < ipos + ibyte; i++)
            {
                if (i >= udpinfos.Length) return "";
                s = s + udpinfos[i];
            }
            return s;
        }

        public string getValueByUpdInfos(string[] udpinfos, int ipos, pmObj pmobj)
        {

            string s = "";
            int ibyte = pmobj.Len / 8;
            ipos = ipos / 8;

            for (int i = ipos; i < ipos + ibyte; i++)
            {
                if (i >= udpinfos.Length) return "";
                s = s + udpinfos[i];
            }
            return s;
        }

        /// <summary>
        /// 由指定的格式化字符串返回结果字符串
        /// </summary>
        /// <param name="sformat">格式化字符串</param>
        /// <returns>结果字符串</returns>
        public string setValByHmFormat(string sformat)
        {
            //@inparkno=?,@start=0,@len=8,@datatype=Integer,@sFormat=,@sFlag=;
            //第一个字符为变量
            //sformat = "@inparkno=100,@start=0,@len=8,@datatype=DateTime,@sFormat=yyyyMMddhhmmss,@sFlag=";
            string[] xformats = sformat.Split(',');
            string vname = getVarNameByHmStr(xformats[0]);
            string value = valtag(sformat, vname);
            int istart = toIntval(valtag(sformat, "start"));
            int ilen = toIntval(valtag(sformat, "len"));
            string format = valtag(sformat, "sFormat");
            int ibytes = ilen / 8;
            string datatype = valtag(sformat, "datatype");
            int icale = basefun.toIntval(valtag(sformat, "scale"));
            if (icale == 0) icale = 1;
            string s = "";
            switch (datatype.ToLower())
            {
                case "datetime":
                    s = GetFormatDateOf16(value, format);
                    break;
                case "float":
                case "double":
                    if (format == "") format = "{0:.00}";
                    if (format.IndexOf("{") > -1)
                        s = string.Format(format, Convert.ToDouble(value));
                    else
                        s = string.Format("{" + format + "}", Convert.ToDouble(value));
                    int vs = Convert.ToInt32(Convert.ToDouble(s)) * icale;
                    s = vs.ToString();
                    s = sVal10To16(s, ibytes);
                    break;
                case "integer":
                    vs = basefun.toIntval(value) * icale;
                    s = vs.ToString();
                    s = sVal10To16(s, ibytes);
                    break;
                case "bit":
                    s = sVal2To16(value, ibytes);
                    break;
                default:
                    s = sVal10To16(value, ibytes);
                    break;
            }
            return s;
        }


        /// <summary>
        /// 由指定的格式化字符串返回结果字符串
        /// </summary>
        /// <param name="sformat">格式化字符串</param>
        /// <param name="srcsign">需更换的原16进制字符</param>
        /// <param name="destsign">被更换的16进制字符</param>
        /// <returns>结果字符串</returns>
        public string setValByHmFormat(string sformat, string srcsign, string destsign)
        {
            //@inparkno=?,@start=0,@len=8,@datatype=Integer,@sFormat=,@sFlag=;
            //第一个字符为变量
            //sformat = "@inparkno=100,@start=0,@len=8,@datatype=DateTime,@sFormat=yyyyMMddhhmmss,@sFlag=";
            string[] xformats = sformat.Split(',');
            string vname = getVarNameByHmStr(xformats[0]);
            string value = valtag(sformat, vname);
            int istart = toIntval(valtag(sformat, "start"));
            int ilen = toIntval(valtag(sformat, "len"));
            string format = valtag(sformat, "sFormat");
            int ibytes = ilen / 8;
            string datatype = valtag(sformat, "datatype");
            int icale = basefun.toIntval(valtag(sformat, "scale"));
            if (icale == 0) icale = 1;
            string s = "";
            switch (datatype.ToLower())
            {
                case "datetime":
                    s = GetFormatDateOf16(value, format);
                    break;
                case "float":
                case "double":
                    if (format == "") format = "{0:.00}";
                    if (format.IndexOf("{") > -1)
                        s = string.Format(format, Convert.ToDouble(value));
                    else
                        s = string.Format("{" + format + "}", Convert.ToDouble(value));
                    int vs = Convert.ToInt32(Convert.ToDouble(s)) * icale;
                    s = vs.ToString();
                    s = sVal10To16(s, ibytes);
                    break;
                case "bit":
                    s = sVal2To16(value, ibytes);
                    break;
                default:
                    s = sVal10To16(value, ibytes);
                    break;
            }
            return s;
        }

        public string AscToCharactor(string s)
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
        /// 解析单个变量名
        /// </summary>
        /// <param name="s">格式化字符@xx = y</param>
        /// <returns>变量名称</returns>
        public string getVarNameByHmStr(string s)
        {
            if (s.IndexOf("@") > -1) s = s.Replace("@", "");
            string[] arr = s.Split('=');
            if (arr.Length != 2) return "";
            return arr[0];
        }

        /// <summary>
        /// 由字符串取十进制整数值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回十进制整数</returns>
        public static int toIntval(string str)
        {
            if (str.Trim() == "") return 0;
            else return int.Parse(str);
        }
        /// <summary>
        /// 由字符串取十进制浮点值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回十进制整数</returns>
        public static Double toFloatValue(string str)
        {
            //if (!IsNumberic(str)) return 0;
            if (str.Trim() == "" || str.Trim() == "null") return 0;
            Double x = 0;
            try
            {
                x = Convert.ToDouble(str);
            }
            catch (Exception ex)
            {
                x = 0;
            }
            return x;
        }

        public string valtag(string stag, string varname)
        {
            stag = stag.Replace(",,", "*#$#*");
            string[] arrTag = getArrayFromString(stag, ",");
            if (null != arrTag)
                for (int i = 0; i < arrTag.Length; i++)
                    if (arrTag[i].StartsWith("@" + varname + "="))
                    {
                        string strFind = arrTag[i].Remove(0, varname.Length + 2);
                        strFind = strFind.Replace("*#$#*", ",").Trim();
                        return strFind;
                    }
            return "";
        }
        public string setvaltag(string stagvalue, string skey, string svalue)
        {
            if (null == skey || "" == skey)
                return stagvalue;
            stagvalue = stagvalue.Replace(",,", "*#$#*");
            svalue = svalue.Replace(",", ",,");
            string[] arrTag = stagvalue.Split(",".ToCharArray());
            string strTag = "";
            bool bfind = false;
            if (null != arrTag)
                for (int i = 0; i < arrTag.Length; i++)
                {
                    if ("" == arrTag[i]) continue;
                    if (false == bfind && arrTag[i].StartsWith("@" + skey + "="))
                    {
                        bfind = true;
                        if (null != svalue && "" != svalue)
                        {
                            if (i > 0) strTag += ",@" + skey + "=" + svalue;
                            else strTag += "@" + skey + "=" + svalue;
                        }
                    }
                    else
                    {
                        if (i > 0) strTag += "," + arrTag[i];
                        else strTag += arrTag[i];
                    }
                }//for(int i=0;i<arrTag.length;i++)
            if (false == bfind && null != svalue && "" != svalue)
            {
                if ("" == strTag)
                    strTag += "@" + skey + "=" + svalue;
                else
                    strTag += ",@" + skey + "=" + svalue;
            }
            strTag = strTag.Replace("*#$#*", ",,");
            return strTag;
        }

        /// <summary>
        /// 从字符串获得相应数组
        /// </summary>
        /// <param name="strGroup"></param>
        /// <returns></returns>
        public string[] getArrayFromString(string strGroup, string sep_symble)
        {
            if (strGroup == null) return null;
            if (strGroup.Trim().Length != 0)
            {
                char[] sep = sep_symble.ToCharArray();//{','};
                string[] strArr = strGroup.Split(sep);
                return strArr;
            }
            else return null;
        }
        #endregion

        #region
        private string ResKeywords(devObj devobj, string state)
        {
            string str = "数据错误！";
            if ("卡务中心" == devobj.Buss || "一卡通" == devobj.Buss)
            {
                switch (state)
                {
                    case "00": str = "操作成功！"; break;
                    case "80": str = "通讯命令错误！"; break;
                    case "81": str = "未知错误！"; break;
                    case "82": str = "长度错误！"; break;
                    case "83": str = "非法数据！"; break;
                    case "86": str = "无卡！"; break;
                    case "87": str = "密码错误！"; break;
                    case "88": str = "未初始化的卡！"; break;
                    case "89": str = "未认证的卡！"; break;
                    case "8A": str = "写错误！"; break;
                    case "8B": str = "模块错误！"; break;
                }
                return str;
            }
            else if ("停车场" == devobj.Buss)
            {
                switch (state)
                {
                    case "00": str = "操作成功！"; break;
                    case "80": str = "通讯命令错误！"; break;
                    case "81": str = "未知错误！"; break;
                    case "82": str = "长度错误！"; break;
                    case "83": str = "无效数据！"; break;
                    case "85": str = "无新记录！"; break;
                    case "88": str = "白名单溢出！"; break;
                    case "89": str = "不存在的白名单！"; break;
                }
                return str;
            }
            else if ("门禁" == devobj.Buss)
            {
                switch (state)
                {
                    case "00": str = "操作成功！"; break;
                    case "80": str = "通讯命令错误！"; break;
                    case "81": str = "未知错误！"; break;
                    case "82": str = "长度错误！"; break;
                    case "83": str = "无效数据！"; break;
                    case "84": str = "黑名单溢出！"; break;
                    case "85": str = "无新记录！"; break;
                    case "88": str = "白名单溢出！"; break;
                    case "89": str = "不存在的白名单！"; break;
                    case "90": str = "无效时段！"; break;
                }
                return str;
            }
            else if ("消费" == devobj.Buss)
            {
                switch (state)
                {
                    case "00": str = "操作成功！"; break;
                    case "80": str = "通讯命令错误！"; break;
                    case "81": str = "未知错误！"; break;
                    case "82": str = "长度错误！"; break;
                    case "83": str = "无效数据！"; break;
                    case "84": str = "黑名单溢出！"; break;
                    case "85": str = "无新记录！"; break;
                    case "86": str = "卡号非法！"; break;
                    case "87": str = "白名单错误！"; break;
                    case "88": str = "白名单溢出！"; break;
                    case "89": str = "不存在的白名单！"; break;
                    case "90": str = "无效的消费时段！"; break;
                }
                return str;
            }
            switch (devobj.Command)
            {
                case "初始化":
                case "加载系统时间":
                case "检测状态":
                    switch (state)
                    {
                        case "00": str = "操作成功！"; break;
                        case "80": str = "通讯命令错误！"; break;
                        case "81": str = "未知错误！"; break;
                        case "82": str = "长度错误！"; break;
                        case "83": str = "无效数据！"; break;
                        case "85": str = "无新记录！"; break;
                        case "88": str = "白名单溢出！"; break;
                        case "89": str = "不存在的白名单！"; break;
                    }
                    break;
                default:
                    switch (state)
                    {
                        case "00": str = "操作成功！"; break;
                        case "80": str = "通讯命令错误！"; break;
                        case "81": str = "未知错误！"; break;
                        case "82": str = "长度错误！"; break;
                        case "83": str = "无效数据！"; break;
                        case "85": str = "无新记录！"; break;
                        case "88": str = "白名单溢出！"; break;
                        case "89": str = "不存在的白名单！"; break;
                    }
                    break;
            }
            return str;
        }
        #endregion
    }

    public class NameObjectList : NameObjectCollectionBase
    {

        private DictionaryEntry _de = new DictionaryEntry();

        #region 构造函数
        public NameObjectList()
        {
        }

        /// <summary>
        /// 根据已有字典建立列表
        /// </summary>
        /// <param name="d"></param>
        /// <param name="bReadOnly"></param>
        public NameObjectList(IDictionary d, Boolean bReadOnly)
        {
            foreach (DictionaryEntry de in d)
            {
                this.BaseAdd((String)de.Key, de.Value);
            }
            this.IsReadOnly = bReadOnly;
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 读取指定位置的键值对
        /// </summary>
        public Object this[int index]
        {
            get
            {
                _de.Key = this.BaseGetKey(index);
                _de.Value = this.BaseGet(index);
                return _de.Value;
            }
            set { this.BaseSet(index, value); }
        }

        /// <summary>
        /// 使用字符键值直接读写对应值
        /// </summary>
        public Object this[String key]
        {
            get
            {
                return (this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        /// <summary>
        /// 返回所有键值
        /// </summary>
        public String[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        /// <summary>
        /// 返回所有的值
        /// </summary>
        public Array AllValues
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }

        /// <summary>
        /// 返回所有字符类型的值
        /// </summary>
        public String[] AllStringValues
        {
            get
            {
                return ((String[])this.BaseGetAllValues(Type.GetType("System.String")));
            }
        }

        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        public void Add(String key, Object value)
        {
            this.BaseAdd(key, value);
        }

        public void Remove(String key)
        {
            this.BaseRemove(key);
        }

        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        #endregion




    }
}
