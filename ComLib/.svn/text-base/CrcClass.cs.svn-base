using System;
using System.Collections.Generic;
using System.Text;

namespace ComLib
{
    public class CrcClass
    {
        /// <summary>
        /// 对命令字符串FCS和校验
        /// </summary>
        /// <param name="str">需要进行FCS和校验的字符串</param>
        /// <returns>校验字:16进制数</returns>
        public string ValidateCodeFCS(string str)
        {
            long d_fcs = 0;
            for (int i = 0; i < str.Length; i += 2)
                d_fcs += Convert.ToInt32(str.Substring(i, 2), 16);
            d_fcs = d_fcs % (0x100);
            string h_fcs = Convert.ToString(d_fcs, 16);
            h_fcs = h_fcs.ToUpper();
            if (h_fcs.Length > 2)
                h_fcs = h_fcs.Substring(h_fcs.Length - 2, 2);
            h_fcs = h_fcs.PadLeft(2, '0');
            return h_fcs;
        }

        /// <summary>
        /// Modbus ASC模式的LRC校验
        /// </summary>
        /// <param name="str">命令字符串</param>
        /// <returns>校验字:16进制数</returns>
        public string ValidateCodeLRC(string str)
        {
            string _str = "";
            long d_lrc = 0;
            for (int i = 0; i < str.Length; i += 2)
            {
                try
                {
                    if (str.Length > i + 1)
                        _str = str.Substring(i, 2);
                }
                catch
                {
                }
                if (this.IsNumberic16(_str) == true)
                    d_lrc += Convert.ToInt32(_str, 16);
                else
                    continue;
            }
            if (d_lrc > 0xFF)
                d_lrc = d_lrc % (0x100);
            string h_lrc = Convert.ToString(0xFF - d_lrc + 1, 16);
            h_lrc = h_lrc.ToUpper();
            if (h_lrc.Length > 2)
                h_lrc = h_lrc.Substring(h_lrc.Length - 2, 2);
            h_lrc = h_lrc.PadLeft(2, '0');
            return h_lrc;
        }

        /// <summary>
        /// Modbus RTU模式的RTU校验
        /// </summary>
        /// <param name="str">命令字符串</param>
        /// <returns>校验字:双字节16进制数</returns>
        public string ValidateCodeCRC(string str)
        {
            UInt16 niCRC = 0xFFFF;
            for (int i = 0; i < str.Length; i += 2)
            {
                niCRC ^= Convert.ToUInt16(str.Substring(i, 2), 16);
                for (int j = 0; j < 8; j++)
                {
                    if (0 != (niCRC & 0x01))
                        niCRC = Convert.ToUInt16(((niCRC >> 1) & 0x7FFF) ^ 0xA001);
                    else
                        niCRC = Convert.ToUInt16((niCRC >> 1) & 0x7FFF);
                    niCRC &= 0xFFFF;
                }
            }
            string strCRC = Convert.ToString(niCRC, 16).PadLeft(4, '0');
            return strCRC.Substring(2, 2) + strCRC.Substring(0, 2);
        }

        /// <summary>
        /// 对命令字符串进行四位异或校验
        /// </summary>
        /// <param name="str">需要进行四位异或校验的字符串</param>
        /// <returns>校验字:16进制数</returns>
        public string CRCXOR4(string cmd)
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
            return Convert.ToString(result, 16);
        }

        public string CRCXOR(string cmd)//XOR校验
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
        /// 字节和校验：目前应用于门禁、考勤通讯协议
        /// </summary>
        /// <param name="strCmd">16进制字符串</param>
        /// <returns></returns>
        public string CRCSUM(string strCmd)
        {
            if (strCmd == "") return "";
            strCmd = strCmd.ToUpper();
            long lCheckSum = 0;
            byte[] Buffer = new byte[strCmd.Length / 2];
            for (int i = 0; i < strCmd.Length / 2; i++)
            {
                Buffer[i] = Convert.ToByte(Convert.ToInt32(strCmd.Substring(2 * i, 2), 16));
                lCheckSum = Buffer[i] + lCheckSum;
            }
            strCmd = sVal10To16(lCheckSum.ToString());
            return strCmd;
        }

        /// <summary>
        /// 将10进制字符串转化为16进制字符串(2位转)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sVal10To16(string str)
        {
            if (str == "") return "";
            string s = Convert.ToString(Convert.ToInt32(str), 16);
            if (s.Length % 2 != 0)
                s = "0" + s;
            return s;
        }

        /// <summary> 
        /// 名称：IsNumberic 
        /// 功能：判断输入的是否是数字 
        /// 参数：string oText：源文本 
        /// 返回值：　bool true:是　false:否 
        /// </summary> 
        public bool IsNumberic16(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText, 16);
                return true;
            }
            catch
            {
                return false;
            }
        } 

    }
}
