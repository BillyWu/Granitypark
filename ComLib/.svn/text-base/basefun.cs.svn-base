using System;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

public class basefun
{
    /// <summary>
    /// 获得分组显示中字符串的最后一个值用于统计图标签名
    /// </summary>
    /// <param name="strGroup"></param>
    /// <returns></returns>
    public static string getLabelname(string strGroup)
    {
        if (strGroup.Trim().Length != 0)
        {
            char[] sep ={ ',' };
            string[] strArr = strGroup.Split(sep);
            return strArr[strArr.Length - 1];
        }
        else return "";
    }

    /// <summary>
    /// 从字符串获得相应数组
    /// </summary>
    /// <param name="strGroup"></param>
    /// <returns></returns>
    public static string[] getArrayFromString(string strGroup, string sep_symble)
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



    /// <summary> 
    /// 名称：IsNumberic 
    /// 功能：判断输入的是否是数字 
    /// 参数：string oText：源文本 
    /// 返回值：　bool true:是　false:否 
    /// </summary> 

    public static bool IsNumberic(string oText)
    {
        try
        {
            int var1 = Convert.ToInt32(oText);
            return true;
        }
        catch
        {
            return false;
        }
    }
    //获得字符串oString的实际长度 
    public static int StringLength(string oString)
    {
        byte[] strArray = System.Text.Encoding.Default.GetBytes(oString);
        int res = strArray.Length;
        return res;
    }
    //	///<summary> 
    //	///名称：redirect 
    //	///功能：子窗体返回主窗体 
    //	///参数：url 
    //	///返回值：空 
    //	///</summary> 
    //	public static void redirect(string url,System.Web.UI.Page page) 
    //	{ 
    //		if ( Session["IfDefault"]!=(object)"Default") 
    //		{ 
    //			page.RegisterStartupScript("","<script>window.top.document.location.href='"+url+"';</script>"); 
    //		} 
    //	} 


    /// <summary>
    /// 度量单位或字符串转为整型
    /// </summary>
    public static int toInt(string str)
    {
        int val = 0;
        if (str == null) { return 0; }
        if (str == "") { return 0; };
        str = str.Replace("px", "");
        str = str.Replace("%", "");
        val = int.Parse(str.Replace("px", ""));
        return val;

    }

    public static int toIntval(string str)
    {
        //if (!IsNumberic(str)) return 0;
        if (str.Trim() == "" || str.Trim() == "null") return 0;
        int x = 0;
        try
        {
            if (str.Contains("."))
                x = Convert.ToInt32(Convert.ToDouble(str));
            else
                x = Convert.ToInt32(str);
        }
        catch (Exception ex)
        {
            x = 0;
        }
        return x;
    }
    public static long toIntval64(string str)
    {
        //if (!IsNumberic(str)) return 0;
        if (str.Trim() == "" || str.Trim() == "null") return 0;
        long x = 0;
        try
        {
            if (str.Contains("."))
                x = Convert.ToInt64(Convert.ToDouble(str));
            else
                x = Convert.ToInt64(str);
        }
        catch
        {
            x = 0;
        }
        return x;
    }
    /// <summary>
    /// 整型转为度量单位或字符串
    /// </summary>
    public static string toUnit(int val) { return val.ToString() + "px"; }

    public static string RepType(string str)
    {
        str = str.Replace("string", "String");
        str = str.Replace("char", "String");
        str = str.Replace("datetime", "DateTime");
        str = str.Replace("date", "DateTime");
        str = str.Replace("int32", "Int32");
        str = str.Replace("int", "Int32");
        str = str.Replace("double", "Double");
        str = str.Replace("float", "Double");
        str = str.Replace("boolean", "Boolean");
        str = str.Replace("bool", "Boolean");
        return str;
    }

    public static string RepAS(string str)
    {
        str = str.Replace(" as ", "^");
        str = str.Replace(" As ", "^");
        str = str.Replace(" AS ", "^");
        return str;
    }


    public static string covMonth(string strmm)
    {
        switch (strmm)
        {
            case "1月": strmm = "一月";
                break;
            case "2月": strmm = "二月";
                break;
            case "3月": strmm = "三月";
                break;
            case "4月": strmm = "四月";
                break;
            case "5月": strmm = "五月";
                break;
            case "6月": strmm = "六月";
                break;
            case "7月": strmm = "七月";
                break;
            case "8月": strmm = "八月";
                break;
            case "9月": strmm = "九月";
                break;
            case "10月": strmm = "十月";
                break;
            case "11月": strmm = "十一月";
                break;
            case "12月": strmm = "十二月";
                break;
        }
        return strmm;
    }

    //取tree形成的XML的自定义属性:标记字符串样式:@key=value,@key=value,@key=value
    //对svalue值含有逗号的执行转义为双逗号,在解析时再对双逗号转义为:*#$#*
    public static string valtag(string stag, string varname)
    {
        if (stag == null) return "";
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


    //设置tree形成的XML的自定义tag属性
    public static string setvaltag(string stagvalue, string skey, string svalue)
    {
        if (string.IsNullOrEmpty(skey))
            return stagvalue;
        if (string.IsNullOrEmpty(stagvalue))
            stagvalue = "";
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
    /// C#实现javascript的escape ,与javascript的unescape呼应
    /// 解决字符串编码在服务端与客户端转换问题
    /// </summary>
    /// <param name="s">需要ASCII编码的字符串</param>
    /// <returns>编码后字符串</returns>
    public static string Escape(string s)
    {
        StringBuilder sb = new StringBuilder();
        byte[] ba = System.Text.Encoding.Unicode.GetBytes(s);
        for (int i = 0; i < ba.Length; i += 2)
        {    /**///// BE SURE 2's 
            sb.Append("%u");
            sb.Append(ba[i + 1].ToString("X2"));
            sb.Append(ba[i].ToString("X2"));
        }
        return sb.ToString();
    }

    public static string false2null(string blval)
    {
        if (blval.ToLower() == "false")
            return "";
        else return blval;
    }

    //判断IP地址的合法性
    public static Boolean IsCorrenctIP(string ip)
    {
        if (ip == "") return true;
        string pattrn = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])";
        if (System.Text.RegularExpressions.Regex.IsMatch(ip, pattrn))
        {
            return true;
        }
        else
        {
            return false;

        }
    }
    public static string Empty2Null(string s)
    {
        if (s == "") s = "null";
        return s;
    }

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

    /// <summary>
    /// 常用日期模式匹配
    /// </summary>
    static Dictionary<string, Regex> pattDt = new Dictionary<string, Regex>();

    static basefun()
    {
        Regex regY = new Regex("^y{2,4}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex regM = new Regex("^M{2}", RegexOptions.Compiled);
        Regex regD = new Regex("^d{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex regW = new Regex("^w{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex regh = new Regex("^h{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex regm = new Regex("^m{2}", RegexOptions.Compiled);
        Regex regs = new Regex("^s{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex reg0 = new Regex("^0*$", RegexOptions.Compiled);
        Regex regF = new Regex("^F*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        pattDt.Add("yy", regY);
        pattDt.Add("MM", regM);
        pattDt.Add("dd", regD);
        pattDt.Add("ww", regW);
        pattDt.Add("HH", regh);
        pattDt.Add("mm", regm);
        pattDt.Add("ss", regs);
        pattDt.Add("00", reg0);
        pattDt.Add("FF", regF);
    }
    /// <summary>
    /// BCD字符转换为日期时间,BCD码只能按字节解析
    /// </summary>
    /// <param name="valHex">BCD字符值</param>
    /// <param name="format">日期时间格式,默认yyMMdd，格式后分号加BCD结尾</param>
    /// <returns>返回转换日期时间值,失败返回最小值</returns>
    public static string toDatetimeBCD(string valBCD, string format)
    {
        //不是BCD码则按照16进制处理
        //忽略4位年的处理,都按照2位年
        if (string.IsNullOrEmpty(format) || !format.EndsWith("~BCD"))
            return basefun.toDatetime(valBCD, format);
        format = format.Replace("~BCD", "");
        string dtformat = DateTime.MinValue.ToString();
        if (string.IsNullOrEmpty(valBCD) || valBCD.Length < 4 || pattDt["00"].IsMatch(valBCD) || pattDt["FF"].IsMatch(valBCD))
            return dtformat;
        if (valBCD.Length % 2 > 0)
            valBCD = "0" + valBCD;
        //解析格式
        string ft = format;
        List<string> fv = new List<string>();
        while (!string.IsNullOrEmpty(ft))
        {
            bool isMatch = false;
            foreach (string key in pattDt.Keys)
            {
                Match mt = pattDt[key].Match(ft);
                if (null == mt || !mt.Success)
                    continue;
                fv.Add(key);
                isMatch = true;
                ft = ft.Substring(mt.Length);
                break;
            }
            if (isMatch || string.IsNullOrEmpty(ft))
                continue;
            ft = ft.Substring(1);
        }
        if (fv.Count < 1 || fv.Count * 2 != valBCD.Length)
            return dtformat;

        string stdvalue = "yy-MM-dd HH:mm:ss";
        string[] ptts ={ "yy", "MM", "dd", "HH", "mm", "ss" };
        //如果每字节表示一个值,则按字节解析
        for (int i = 0; i < fv.Count; i++)
        {
            string v = Convert.ToString(Convert.ToInt16(valBCD.Substring(i * 2, 2), 10));
            stdvalue = stdvalue.Replace(fv[i], v);
        }
        stdvalue = stdvalue.Replace("yy", "0001");
        stdvalue = stdvalue.Replace("MM", "01");
        stdvalue = stdvalue.Replace("dd", "01");
        for (int i = 0; i < ptts.Length; i++)
            stdvalue = stdvalue.Replace(ptts[i], "00");
        try
        {
            DateTime dt = Convert.ToDateTime(stdvalue);
            dtformat = dt.ToString();
        }
        catch { }
        dtformat = dtformat.Replace("ww", "");
        return dtformat;
    }
    /// <summary>
    /// 日期时间转换为指定格式的BCD码字符
    /// </summary>
    /// <param name="dtval">日期时间值</param>
    /// <param name="format">日期时间格式,格式后分号加BCD</param>
    /// <returns>返回转换的16进制值</returns>
    public static string toBCDDatetime(string dtval, string format)
    {
        //检查参数
        if (string.IsNullOrEmpty(format) || !format.EndsWith("~BCD"))
            return basefun.toHexDatetime(dtval, format);
        format = format.Replace("~BCD", "");
        if (string.IsNullOrEmpty(dtval))
            dtval = DateTime.Today.ToString();
        DateTime dt = DateTime.Today;
        try
        {
            dt = Convert.ToDateTime(dtval);
        }
        catch { return ""; }

        //解析格式
        string ft = format;
        List<string> fv = new List<string>();
        while (!string.IsNullOrEmpty(ft))
        {
            bool isMatch = false;
            foreach (string key in pattDt.Keys)
            {
                Match mt = pattDt[key].Match(ft);
                if (null == mt || !mt.Success)
                    continue;
                fv.Add(key);
                isMatch = true;
                ft = ft.Substring(mt.Length);
                break;
            }
            if (isMatch || string.IsNullOrEmpty(ft))
                continue;
            ft = ft.Substring(1);
        }
        if (fv.Count < 1)
            return "";
        //BCD码只能按字节解析
        string stdvalue = "";
        for (int i = 0; i < fv.Count; i++)
        {
            string v = dt.ToString(fv[i]);
            if ("ww" == fv[i])
                v = 0 == dt.DayOfWeek ? "7" : Convert.ToInt16(dt.DayOfWeek).ToString();
            else
                v = Convert.ToString(Convert.ToInt16(v), 10);
            stdvalue += v.PadLeft(2, '0');
        }
        return stdvalue;
    }

    /// <summary>
    /// 16进制字符转换为日期时间
    /// </summary>
    /// <param name="valHex">16进制字符值</param>
    /// <param name="format">日期时间格式,默认yyMMdd，格式后分号加数字#分段解析特殊需求的日期</param>
    /// <returns>返回转换日期时间值,失败返回最小值</returns>
    public static string toDatetime(string valHex, string format)
    {
        //忽略4位年的处理,都按照2位年
        //不够两个字节则忽略
        if (string.IsNullOrEmpty(format))
            format = "yy-MM-dd";
        int index = format.IndexOf("~");
        string attr = "";
        if (index > -1)
        {
            attr = format.Substring(index + 1);
            format = format.Substring(0, index);
        }
        string dtformat = DateTime.MinValue.ToString();
        if (string.IsNullOrEmpty(valHex) || valHex.Length < 4 || pattDt["00"].IsMatch(valHex) || pattDt["FF"].IsMatch(valHex))
            return dtformat;
        if (valHex.Length % 2 > 0)
            valHex = "0" + valHex;
        //解析格式
        string ft = format;
        List<string> fv = new List<string>();
        while (!string.IsNullOrEmpty(ft))
        {
            bool isMatch = false;
            foreach (string key in pattDt.Keys)
            {
                Match mt = pattDt[key].Match(ft);
                if (null == mt||!mt.Success)
                    continue;
                fv.Add(key);
                isMatch = true;
                ft = ft.Substring(mt.Length);
                break;
            }
            if (isMatch || string.IsNullOrEmpty(ft))
                continue;
            ft = ft.Substring(1);
        }
        if (fv.Count < 1)
            return dtformat;

        string stdvalue = "yy-MM-dd HH:mm:ss";
        string[] ptts ={ "yy", "MM", "dd", "HH", "mm", "ss" };
        //如果每字节表示一个值,则按字节解析
        if (valHex.Length == fv.Count * 2 && string.IsNullOrEmpty(attr))
        {
            for (int i = 0; i < fv.Count; i++)
            {
                string v = Convert.ToString(Convert.ToInt16(valHex.Substring(i * 2, 2), 16));
                stdvalue = stdvalue.Replace(fv[i], v);
            }
            stdvalue = stdvalue.Replace("yy", "0001");
            stdvalue = stdvalue.Replace("MM", "01");
            stdvalue = stdvalue.Replace("dd", "01");
            for (int i = 0; i < ptts.Length; i++)
                stdvalue = stdvalue.Replace(ptts[i], "00");
            try
            {
                DateTime dt = Convert.ToDateTime(stdvalue);
                dtformat = dt.ToString(format);
            }
            catch { }
            dtformat = dtformat.Replace("ww", "");
            return dtformat; 
        }
        //不按字节没有附加格式参数,则忽略
        if (string.IsNullOrEmpty(attr))
            return dtformat;

        //如果按位计算日期格式,则需解析附加参数,格式：yyMMdd;7#4#5   hhmmss~5#6#5&2
        // 附加参数以分号隔离,#号分段,每段表示位长度和系数(长度和系数用&分开)
        int len = valHex.Length;
        for (int i = 0; i < len; i += 2)
            valHex += Convert.ToString(Convert.ToInt16(valHex.Substring(i, 2), 16), 2).PadLeft(8,'0');
        valHex = valHex.Substring(len);
        index = 0;
        string[] ftband = attr.Split("#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < fv.Count; i++)
        {
            decimal scal = 1;
            string blen = "8";
            if (ftband.Length > i)
                blen = ftband[i];
            string[] band ={ blen };
            if (blen.Contains("&"))
                band = blen.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            len = Convert.ToInt16(band[0]);
            if (band.Length > 1)
                scal = Convert.ToDecimal(band[1]);
            if (valHex.Length <= index)
                break;
            string v = Convert.ToString(Convert.ToInt64(scal * Convert.ToInt64(valHex.Substring(index, len), 2)));
            stdvalue = stdvalue.Replace(fv[i], v);
            index += len;
        }
        stdvalue = stdvalue.Replace("yy", "0001");
        stdvalue = stdvalue.Replace("MM", "01");
        stdvalue = stdvalue.Replace("dd", "01");
        for (int i = 0; i < ptts.Length; i++)
            stdvalue = stdvalue.Replace(ptts[i], "00");
        try
        {
            DateTime dt = Convert.ToDateTime(stdvalue);
            dtformat = dt.ToString();
            dtformat = dtformat.Replace("ww", "");
        }
        catch { }
        return dtformat;
    }

    /// <summary>
    /// 日期时间转换为指定格式的16进制字符
    /// </summary>
    /// <param name="dtval">日期时间值</param>
    /// <param name="format">日期时间格式,默认yyMMdd，格式后分号加数字#分段解析特殊需求的日期</param>
    /// <returns>返回转换的16进制值</returns>
    public static string toHexDatetime(string dtval, string format)
    {
        //检查参数
        if (string.IsNullOrEmpty(dtval))
            dtval = DateTime.Today.ToString();
        DateTime dt = DateTime.Today;
        try
        {
            dt = Convert.ToDateTime(dtval);
        }
        catch { return ""; }

        //解析格式
        if (string.IsNullOrEmpty(format))
            format = "yy-MM-dd";
        int index = format.IndexOf("~");
        string attr = "";
        if (index > -1)
        {
            attr = format.Substring(index + 1);
            format = format.Substring(0, index);
        }
        string ft = format;
        List<string> fv = new List<string>();
        while (!string.IsNullOrEmpty(ft))
        {
            bool isMatch = false;
            foreach (string key in pattDt.Keys)
            {
                Match mt = pattDt[key].Match(ft);
                if (null == mt || !mt.Success)
                    continue;
                fv.Add(key);
                isMatch = true;
                ft = ft.Substring(mt.Length);
                break;
            }
            if (isMatch || string.IsNullOrEmpty(ft))
                continue;
            ft = ft.Substring(1);
        }
        if (fv.Count < 1)
            return "";
        
        //没有附加格式,按字节解析
        //有附加位长度格式,按位长度解析
        string stdvalue = "";
        if (string.IsNullOrEmpty(attr))
        {
            for (int i = 0; i < fv.Count; i++)
            {
                string v = dt.ToString(fv[i]);
                if ("ww" == fv[i])
                    v = 0 == dt.DayOfWeek ? "7" : Convert.ToInt16(dt.DayOfWeek).ToString();
                else
                    v = Convert.ToString(Convert.ToInt16(v), 16);
                stdvalue += v.PadLeft(2, '0');
            }
            return stdvalue;
        }
        string[] ftband = attr.Split("#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < fv.Count; i++)
        {
            decimal scal = 1;
            string blen = "8";
            if (ftband.Length > i)
                blen = ftband[i];
            string[] band ={ blen };
            if (blen.Contains("&"))
                band = blen.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int len = Convert.ToInt16(band[0]);
            if (band.Length > 1)
                scal = Convert.ToDecimal(band[1]);
            int vi = 1;
            if ("ww" != fv[i])
                vi = Convert.ToInt16(Convert.ToInt16(dt.ToString(fv[i])) / scal);
            else
            {
                vi = 0 == dt.DayOfWeek ? 7 : Convert.ToInt32(dt.DayOfWeek);
                vi = Convert.ToInt16(vi / scal);
            }
            stdvalue += Convert.ToString(vi, 2).PadLeft(len, '0');
        }
        index = Convert.ToInt16(decimal.Ceiling(Convert.ToDecimal(stdvalue.Length / 8.0)));
        stdvalue = stdvalue.PadLeft(index * 8, '0');
        index = stdvalue.Length;
        for (int i = 0; i < index; i += 8)
            stdvalue += Convert.ToString(Convert.ToInt16(stdvalue.Substring(i, 8), 2), 16).PadLeft(2,'0');
        stdvalue = stdvalue.Substring(index);
        return stdvalue;
    }
}