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
    /// ��÷�����ʾ���ַ��������һ��ֵ����ͳ��ͼ��ǩ��
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
    /// ���ַ��������Ӧ����
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
    /// ���ƣ�IsNumberic 
    /// ���ܣ��ж�������Ƿ������� 
    /// ������string oText��Դ�ı� 
    /// ����ֵ����bool true:�ǡ�false:�� 
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
    //����ַ���oString��ʵ�ʳ��� 
    public static int StringLength(string oString)
    {
        byte[] strArray = System.Text.Encoding.Default.GetBytes(oString);
        int res = strArray.Length;
        return res;
    }
    //	///<summary> 
    //	///���ƣ�redirect 
    //	///���ܣ��Ӵ��巵�������� 
    //	///������url 
    //	///����ֵ���� 
    //	///</summary> 
    //	public static void redirect(string url,System.Web.UI.Page page) 
    //	{ 
    //		if ( Session["IfDefault"]!=(object)"Default") 
    //		{ 
    //			page.RegisterStartupScript("","<script>window.top.document.location.href='"+url+"';</script>"); 
    //		} 
    //	} 


    /// <summary>
    /// ������λ���ַ���תΪ����
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
    /// ����תΪ������λ���ַ���
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
            case "1��": strmm = "һ��";
                break;
            case "2��": strmm = "����";
                break;
            case "3��": strmm = "����";
                break;
            case "4��": strmm = "����";
                break;
            case "5��": strmm = "����";
                break;
            case "6��": strmm = "����";
                break;
            case "7��": strmm = "����";
                break;
            case "8��": strmm = "����";
                break;
            case "9��": strmm = "����";
                break;
            case "10��": strmm = "ʮ��";
                break;
            case "11��": strmm = "ʮһ��";
                break;
            case "12��": strmm = "ʮ����";
                break;
        }
        return strmm;
    }

    //ȡtree�γɵ�XML���Զ�������:����ַ�����ʽ:@key=value,@key=value,@key=value
    //��svalueֵ���ж��ŵ�ִ��ת��Ϊ˫����,�ڽ���ʱ�ٶ�˫����ת��Ϊ:*#$#*
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


    //����tree�γɵ�XML���Զ���tag����
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
    /// C#ʵ��javascript��escape ,��javascript��unescape��Ӧ
    /// ����ַ��������ڷ������ͻ���ת������
    /// </summary>
    /// <param name="s">��ҪASCII������ַ���</param>
    /// <returns>������ַ���</returns>
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

    //�ж�IP��ַ�ĺϷ���
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
    /// ��������ģʽƥ��
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
    /// BCD�ַ�ת��Ϊ����ʱ��,BCD��ֻ�ܰ��ֽڽ���
    /// </summary>
    /// <param name="valHex">BCD�ַ�ֵ</param>
    /// <param name="format">����ʱ���ʽ,Ĭ��yyMMdd����ʽ��ֺż�BCD��β</param>
    /// <returns>����ת������ʱ��ֵ,ʧ�ܷ�����Сֵ</returns>
    public static string toDatetimeBCD(string valBCD, string format)
    {
        //����BCD������16���ƴ���
        //����4λ��Ĵ���,������2λ��
        if (string.IsNullOrEmpty(format) || !format.EndsWith("~BCD"))
            return basefun.toDatetime(valBCD, format);
        format = format.Replace("~BCD", "");
        string dtformat = DateTime.MinValue.ToString();
        if (string.IsNullOrEmpty(valBCD) || valBCD.Length < 4 || pattDt["00"].IsMatch(valBCD) || pattDt["FF"].IsMatch(valBCD))
            return dtformat;
        if (valBCD.Length % 2 > 0)
            valBCD = "0" + valBCD;
        //������ʽ
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
        //���ÿ�ֽڱ�ʾһ��ֵ,���ֽڽ���
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
    /// ����ʱ��ת��Ϊָ����ʽ��BCD���ַ�
    /// </summary>
    /// <param name="dtval">����ʱ��ֵ</param>
    /// <param name="format">����ʱ���ʽ,��ʽ��ֺż�BCD</param>
    /// <returns>����ת����16����ֵ</returns>
    public static string toBCDDatetime(string dtval, string format)
    {
        //������
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

        //������ʽ
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
        //BCD��ֻ�ܰ��ֽڽ���
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
    /// 16�����ַ�ת��Ϊ����ʱ��
    /// </summary>
    /// <param name="valHex">16�����ַ�ֵ</param>
    /// <param name="format">����ʱ���ʽ,Ĭ��yyMMdd����ʽ��ֺż�����#�ֶν����������������</param>
    /// <returns>����ת������ʱ��ֵ,ʧ�ܷ�����Сֵ</returns>
    public static string toDatetime(string valHex, string format)
    {
        //����4λ��Ĵ���,������2λ��
        //���������ֽ������
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
        //������ʽ
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
        //���ÿ�ֽڱ�ʾһ��ֵ,���ֽڽ���
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
        //�����ֽ�û�и��Ӹ�ʽ����,�����
        if (string.IsNullOrEmpty(attr))
            return dtformat;

        //�����λ�������ڸ�ʽ,����������Ӳ���,��ʽ��yyMMdd;7#4#5   hhmmss~5#6#5&2
        // ���Ӳ����ԷֺŸ���,#�ŷֶ�,ÿ�α�ʾλ���Ⱥ�ϵ��(���Ⱥ�ϵ����&�ֿ�)
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
    /// ����ʱ��ת��Ϊָ����ʽ��16�����ַ�
    /// </summary>
    /// <param name="dtval">����ʱ��ֵ</param>
    /// <param name="format">����ʱ���ʽ,Ĭ��yyMMdd����ʽ��ֺż�����#�ֶν����������������</param>
    /// <returns>����ת����16����ֵ</returns>
    public static string toHexDatetime(string dtval, string format)
    {
        //������
        if (string.IsNullOrEmpty(dtval))
            dtval = DateTime.Today.ToString();
        DateTime dt = DateTime.Today;
        try
        {
            dt = Convert.ToDateTime(dtval);
        }
        catch { return ""; }

        //������ʽ
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
        
        //û�и��Ӹ�ʽ,���ֽڽ���
        //�и���λ���ȸ�ʽ,��λ���Ƚ���
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