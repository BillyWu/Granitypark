#region 版本说明

/*
 * 功能内容：   可使用ZNetCom的设备UDP通讯口搜索工具类
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
using System.Net;
using Granity.communications;
using System.Threading;

namespace Granity.winTools
{
    /// <summary>
    /// 可使用ZNetCom的设备UDP通讯口搜索工具类
    ///     该工具通过广播8800和8801端口搜索设备
    /// </summary>
    public static class CmdUDPznetCom
    {
        /// <summary>
        /// 初始广播指令
        /// </summary>
        private const string initcmd = "A1 15 02 00 00 FF";
        /// <summary>
        /// 初始广播端口
        /// </summary>
        private const int initport = 8800;
        /// <summary>
        /// 获取信息指令,含UDP物理地址参数和指令号(A1 14 00 02 00/A1 14 00 02 02)
        /// </summary>
        private const string infocmd = "00 02 00 02 {0} {1} 00 FF";
        /// <summary>
        /// 搜索门禁设备
        /// </summary>
        private const string searcmd = "7E FF FF 01 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 02 0D";
        /// <summary>
        /// 广播门禁设备通讯端口号,60000
        /// </summary>
        private const int searport=60000;
        /// <summary>
        /// 获取信息广播端口
        /// </summary>
        private const int infoport = 8801;
        private static Protocol ptlznet = new Protocol();
        /// <summary>
        /// Znet协议规则
        /// </summary>
        private static Protocol ptlZnet
        {
            get
            {
                if (ptlznet.FrameHeader.Length > 0)
                    return ptlznet;
                ptlznet.FrameHeader = new byte[] { 0x00, 0x02, 0x00, 0x02 };
                ptlznet.FrameFoot = new byte[] { 0x00, 0xFF };
                ptlznet.KeyIndexStart = -1;
                ptlznet.KeyLength = 0;
                return ptlznet;
            }
        }

        /// <summary>
        /// 搜索网络内支持UDP通讯的设备信息,设备信息使用tag格式数据
        /// </summary>
        /// <returns>返回tag格式数据信息</returns>
        public static string[] SearchUDPnet()
        {
            IPEndPoint srv = new IPEndPoint(IPAddress.Broadcast, initport);
            CommiTarget target = new CommiTarget(srv, CommiType.UDP);
            target.setProtocol(ptlZnet);

            DateTime dtStart = DateTime.Now;
            CommandBase cmd = new CommandBase(false);
            cmd.TimeSendInv = new TimeSpan(0, 0, 50);
            cmd.TimeOut = new TimeSpan(0, 0, 2);
            cmd.TimeFailLimit = new TimeSpan(0, 0, 5);
            cmd.TimeLimit = new TimeSpan(0, 0, 20);
            cmd.IsResposeHandle = isRpsZnet;

            cmd.setCommand(initcmd, true);
            cmd.ResponseHandle += new EventHandler<ResponseEventArgs>(cmd_ResponseHandle);
            CommiManager.GlobalManager.SendCommand(target, cmd);
            //广播搜索，有多个设备返回信息，直到超时停止接收
            List<string> infolist = cmd.Tag as List<string>;
            while (cmd.EventWh.WaitOne(cmd.TimeLimit, false) && DateTime.Now - dtStart < cmd.TimeLimit)
            {
                infolist = cmd.Tag as List<string>;
                if (null == infolist || infolist.Count < 1)
                    continue;
                string[] infos = infolist.ToArray();
                bool iscomplete = true;
                for (int i = 0; i < infos.Length; i++)
                {
                    if (!string.IsNullOrEmpty(basefun.valtag(infos[i], "工作端口")))
                        continue;
                    iscomplete = false;
                    break;
                }
                if (iscomplete)
                    break;
            }
            cmd.TimeSendInv = new TimeSpan(0, 0, -10);
            CommiManager.GlobalManager.RemoveCommand(target, cmd);
            //返回搜索结果
            infolist = cmd.Tag as List<string>;
            cmd.Tag = null;
            if (null == infolist || infolist.Count < 1)
                return new string[0];
            return infolist.ToArray();
        }
        /// <summary>
        /// 接收广播反馈,需要多次交互获取完整信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void cmd_ResponseHandle(object sender, ResponseEventArgs e)
        {
            if (null == sender || !(sender is CommandBase) || null == e || !e.Success || null == e.Response || e.Response.Length < 1)
                return;
            CommandBase cmd = sender as CommandBase;
            string rsl = CommandBase.Parse(cmd.ResponseData, true);
            //检查功能码：搜索功能，获取网络信息，获取详细信息
            string codesearch  = "A1FD000922";
            string codedetailA = "A114010200";
            string codedetailB = "A114010202";
            string code = parseFunCode(rsl);
            string addr = parseAddrMac(rsl);
            if (codesearch != code && codedetailA != code && codedetailB != code)
                return;

            //格式参数：普通/物理地址/IP地址/文本/数字
            string[] formatinit ={ "功能码,10,4;", "物理地址,4,6", "IP地址,17,4;", "", "" };
            string[] formatdtlA ={"功能码,10,4;", "物理地址,4,6",
                                    "IP地址,47,4;子网掩码,51,4;网关IP,55,4;DNS服务器,59,4;",
                                    "设备名称,15,16;密码,31,5;",
                                    "网页端口,71,1;命令端口,72,2;"};
            string[] formatdtlB ={ "功能码,10,4;", "物理地址,4,6", "","",
                                    "工作方式,15,1;工作端口,16,2;超时断开时间,18,3;心跳时间,21,1;波特率,26,3;数据位,29,1;停止位,30,1;校验位,31,1;"};
            formatdtlB[4] += "分包长度,32,3;帧间隔,35,2;目标端口1,47,2;目标端口2,49,2;目标端口3,51,2;目标端口4,53,2;目标端口5,55,2;目标端口6,57,2;目标端口7,59,2;目标端口8,61,2;";

            //读取信息列表
            List<string> infolist = new List<string>();
            if (null == cmd.Tag)
                cmd.Tag = infolist;

            int index = -1;
            infolist = cmd.Tag as List<string>;
            for (int i = 0; i < infolist.Count; i++)
            {
                if (addr != basefun.valtag(infolist[i], "物理地址"))
                    continue;
                index = i;
                break;
            }
            string info = "";
            if (index > -1)
                info = infolist[index];
            if (codesearch == code)
                info = parseDetail(rsl, info, formatinit[0], formatinit[1], formatinit[2], formatinit[3], formatinit[4], false);
            else if (codedetailA == code)
                info = parseDetail(rsl, info, formatdtlA[0], formatdtlA[1], formatdtlA[2], formatdtlA[3], formatdtlA[4], false);
            else if (codedetailB == code)
                info = parseDetail(rsl, info, formatdtlB[0], formatdtlB[1], formatdtlB[2], formatdtlB[3], formatdtlB[4], false);
            if (index > -1)
                infolist[index] = info;
            else
                infolist.Add(info);

            //最后一步获取详细信息后结束
            if (codedetailB == code)
                return;
            //再获取网络信息，最后获取详细信息
            IPEndPoint srv = new IPEndPoint(IPAddress.Broadcast, infoport);
            CommiTarget target = new CommiTarget(srv, CommiType.UDP);
            target.setProtocol(ptlZnet);
            string strcmd = string.Format(infocmd, addr.Replace("-", " "), codesearch == code ? "A114000200" : "A114000202");
            CommandBase cmdNext = new CommandBase();
            //使用相同的同步事件，可动态检测UDP搜索反馈结果
            cmdNext.EventWh = cmd.EventWh;
            cmdNext.Tag = cmd.Tag;
            cmdNext.ResponseHandle += new EventHandler<ResponseEventArgs>(cmd_ResponseHandle);
            cmdNext.IsResposeHandle = isResponseCmd;
            cmdNext.setCommand(strcmd, true);
            CommiManager.GlobalManager.SendCommand(target, cmdNext);
        }
        /// <summary>
        /// 判断是否是指令的响应
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="response">响应字节</param>
        /// <returns>是否匹配</returns>
        private static bool isRpsZnet(CommandBase cmd, byte[] response)
        {
            if (response.Length < 25 || response.Length > 25)
                return false;
            byte[] rqs = new byte[] { 0xA1, 0xFD, 0x00, 0x09, 0x22 };
            for (int i = 10; i < 15; i++)
            {
                if (rqs[i-10] != response[i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 判断是否是指令的响应
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="response">响应字节</param>
        /// <returns>是否匹配</returns>
        private static bool isResponseCmd(CommandBase cmd, byte[] response)
        {
            byte[] request = cmd.getCommand();
            if (null == request || request.Length < 15 || response.Length < 15)
                return false;
            if (0 != request[12] || 1 != response[12])
                return false;
            for (int i = 0; i < 15; i++)
            {
                if (12 == i) continue;
                if (request[i] != response[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 解析响应结果,读取其中的物理地址
        /// </summary>
        /// <param name="response">通讯响应结果,16进制字符串</param>
        /// <returns>返回物理地址,非法数据则返回空字符</returns>
        private static string parseAddrMac(string response)
        {
            string header = "00020002", end = "00FF";
            if (string.IsNullOrEmpty(response) || !response.StartsWith(header) || !response.EndsWith(end))
                return "";
            //解析mac地址和IP地址
            string addrmac = response.Substring(header.Length, 12);
            for (int i = addrmac.Length - 2; i > 0; i -= 2)
                addrmac = addrmac.Substring(0, i) + "-" + addrmac.Substring(i);
            return addrmac;
        }

        /// <summary>
        /// 解析响应结果,读取其中的功能代码
        /// </summary>
        /// <param name="response">通讯响应结果,16进制字符串</param>
        /// <returns>返回功能代码</returns>
        private static string parseFunCode(string response)
        {
            string header = "00020002", end = "00FF";
            if (string.IsNullOrEmpty(response) || !response.StartsWith(header) || !response.EndsWith(end))
                return "";
            //解析功能代码
            string funcode = response.Substring(header.Length+12, 10);
            return funcode;
        }

        /// <summary>
        /// 搜索网络内支持UDP通讯的设备信息,设备信息使用tag格式数据
        /// </summary>
        /// <returns>返回tag格式数据信息</returns>
        public static string[] SearchUDPDoor()
        {
            IPEndPoint srv = new IPEndPoint(IPAddress.Broadcast, searport);
            CommiTarget target = new CommiTarget(srv, CommiType.UDP);
            target.setProtocol(Protocol.PTLDoor);

            DateTime dtStart = DateTime.Now;
            CommandBase cmd = new CommandBase(false);
            cmd.TimeSendInv = new TimeSpan(0, 0, 50);
            cmd.TimeOut = new TimeSpan(0, 0, 2);
            cmd.TimeFailLimit = new TimeSpan(0, 0, 5);
            cmd.TimeLimit = new TimeSpan(0, 0, 20);
            cmd.IsResposeHandle = isRpsDoor;

            cmd.setCommand(searcmd, true);
            cmd.ResponseHandle += new EventHandler<ResponseEventArgs>(cmd_RpsDoorHandle);
            CommiManager.GlobalManager.SendCommand(target, cmd);
            //广播搜索，有多个设备返回信息，直到超时停止接收
            List<string> infolist = cmd.Tag as List<string>;
            while (cmd.EventWh.WaitOne(cmd.TimeLimit, false) && DateTime.Now - dtStart < cmd.TimeLimit)
            {
                cmd.EventWh.Reset();
            }
            cmd.TimeSendInv = new TimeSpan(0, 0, -10);
            CommiManager.GlobalManager.RemoveCommand(target, cmd);
            //返回搜索结果
            infolist = cmd.Tag as List<string>;
            cmd.Tag = null;
            if (null == infolist || infolist.Count < 1)
                return new string[0];
            return infolist.ToArray();
        }
        /// <summary>
        /// 接收广播反馈,需要多次交互获取完整信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void cmd_RpsDoorHandle(object sender, ResponseEventArgs e)
        {
            if (null == sender || !(sender is CommandBase) || null == e || !e.Success || null == e.Response || e.Response.Length < 1)
                return;
            CommandBase cmd = sender as CommandBase;
            string rsl = CommandBase.Parse(cmd.ResponseData, true);
            //格式参数：普通/物理地址/IP地址/文本/数字
            string[] formatinfo ={ "功能码,3,2;",
                                     "物理地址,5,6", "IP地址,11,4;子网掩码,15,4;网关IP,19,4;",
                                     "",
                                    "站址,1,2;工作端口,23,2;"};
            //读取信息列表
            List<string> infolist = new List<string>();
            if (null == cmd.Tag)
                cmd.Tag = infolist;
            infolist = cmd.Tag as List<string>;
            string info = parseDetail(rsl, "", formatinfo[0], formatinfo[1], formatinfo[2], formatinfo[3], formatinfo[4], true);
            infolist.Add(info);
        }
        /// <summary>
        /// 判断是否是搜索门禁指令的响应
        /// </summary>
        /// <param name="cmd">门禁搜索指令</param>
        /// <param name="response">响应字节</param>
        /// <returns>是否匹配</returns>
        private static bool isRpsDoor(CommandBase cmd, byte[] response)
        {
            if (response.Length < 34 || response.Length > 34)
                return false;
            byte[] rqs = new byte[] { 0x01, 0x11 };
            for (int i = 3; i < 5; i++)
            {
                if (rqs[i - 3] != response[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 解析响应结果,生成tag格式的标记详细信息
        /// 格式参数用分号分割,其中名称|索引号|字节长度用逗号分割
        /// </summary>
        /// <param name="response">通讯响应结果,16进制字符串</param>
        /// <param name="taginfo">tag格式结果数据</param>
        /// <param name="format">格式参数,普通格式</param>
        /// <param name="addrmac">格式参数,物理地址类型</param>
        /// <param name="addrip">格式参数,IP地址类型</param>
        /// <param name="txt">格式参数,文本类型</param>
        /// <param name="num">格式参数,数字类型</param>
        /// <param name="chgLH">是否高地位转置</param>
        /// <returns>返回tag格式的详细信息</returns>
        private static string parseDetail(string response, string taginfo, string format, string addrmac, string addrip, string txt, string num,bool chgLH)
        {
            if (string.IsNullOrEmpty(response))
                return "";
            //参数名称,起始索引号,字节长度
            string info = taginfo;
            string[] infos = format.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < infos.Length; i++)
            {
                string[] p = infos[i].Split(",".ToCharArray());
                string val = response.Substring(Convert.ToInt16(p[1]) * 2, Convert.ToInt16(p[2]) * 2);
                info = basefun.setvaltag(info, p[0], val);
            }
            //物理地址格式参数
            infos = addrmac.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < infos.Length; i++)
            {
                string[] p = infos[i].Split(",".ToCharArray());
                string val = response.Substring(Convert.ToInt16(p[1]) * 2, Convert.ToInt16(p[2]) * 2);
                for (int k = val.Length - 2; k > 0; k -= 2)
                    val = val.Substring(0, k) + "-" + val.Substring(k);
                info = basefun.setvaltag(info, p[0], val);
            }
            //IP地址格式参数
            infos = addrip.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < infos.Length; i++)
            {
                string[] p = infos[i].Split(",".ToCharArray());
                string val = response.Substring(Convert.ToInt16(p[1]) * 2, Convert.ToInt16(p[2]) * 2);
                for (int k = val.Length - 2; k > -1; k -= 2)
                {
                    string v = Convert.ToInt16(val.Substring(k, 2), 16).ToString();
                    val = val.Substring(0, k) + "." + v + val.Substring(k + 2);
                }
                info = basefun.setvaltag(info, p[0], val.Substring(1));
            }
            //文本格式参数
            infos = txt.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < infos.Length; i++)
            {
                string[] p = infos[i].Split(",".ToCharArray());
                string val = response.Substring(Convert.ToInt16(p[1]) * 2, Convert.ToInt16(p[2]) * 2);
                byte[] buffer = new byte[val.Length / 2];
                for (int k = 0; k < val.Length; k += 2)
                    buffer[k / 2] = (byte)Convert.ToInt16(val.Substring(k, 2), 16);
                if (chgLH)
                    Array.Reverse(buffer);
                val = CommandBase.Parse(buffer);
                val = val.Replace("\0", "");
                info = basefun.setvaltag(info, p[0], val);
            }
            //数字格式参数
            infos = num.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < infos.Length; i++)
            {
                string[] p = infos[i].Split(",".ToCharArray());
                string val = response.Substring(Convert.ToInt16(p[1]) * 2, Convert.ToInt16(p[2]) * 2);
                if (chgLH)
                {
                    string temp = "";
                    for (int k = val.Length - 2; k > -1; k -= 2)
                        temp += val.Substring(k, 2);
                    val = temp;
                }
                val = Convert.ToInt64(val, 16).ToString();
                info = basefun.setvaltag(info, p[0], val);
            }

            return info;
        }
    }
}
