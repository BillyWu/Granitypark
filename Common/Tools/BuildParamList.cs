using System;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Xsl;
using System.Web;
using System.Web.SessionState;

#region 说明
/// <summary>
/// 管理系统共用参数:UnitName,StartDate,EndDate,UserAccounts,StartPreDate,EndNextDate
///					 UserName,DeptName,DeptCode,DWName,DeptSaleName,OPTUnitID,LimitDays
/// UnitName 存放位置:<PL t='P'><L t='D'><P n='UnitName' v='blank' t='s'/></L></PL>
/// 采用xmldocument结构
/// 参数分为三个类型:  系统参数,页面参数,局部段Band参数
/// 参数存放位置及机制:
///       系统参数只存放于session中,页面不传递这部分参数;session["sysparam"]
///       页面参数由页面传递,有三种传递情况: 1,页面间通过session传递; 2,XMLHttp直接传递给页面 3,页面自身回传(此种类型的参数是层次的)
///       局部段Band参数, 在客户端页面与后台的交互过程中,根据不同情况传递段Band当前行参数,默认是主段
/// 在客户端的参数结构:
///       系统参数(不进行传递使用),  只提供客户端js使用
///       全局参数: 页面接收的传递参数,   页面自定义参数     局部临时参数
///       局部段Band参数:   段Band当前行(隐含于数据段内,不在参数数据岛)     段自定义参数     段临时参数
/// 临时参数:
///       在每次传递后即可消失的参数,它不影响下一次使用
/// 参数数组:
///       在段参数数组 PL[@t="B"]/L[@t="D"]/List/P 形成参数数组
/// 参数优先级: 段->页->系 ;  临时->自定义->数据->传递
/// 在页间传递时,接收页面的客户端参数要做一转换:把前一页面的传递参数舍弃,留下其他参数形成接收页面的传递参数
/// 
/// <D>
///    <PL n="name" t="type(S|P|B :System|Page|Band)">
///       <L t="D|C|T|Ts :Data|Custom|Temp|Trans">
///          <P n="name" v="value" t="type(s|i|f|d|guid|b :string|int|float|datetime|guid|bool)" pt="M|E :Micro|Envir"/>
///          <List><P .... /><P ... />...</List>
///          <List><P .... /><P ... />...</List>
///          ...
///       </L>
///    </PL>
/// </D>
/// </summary>
#endregion

namespace Estar.Common.Tools
{
    
    #region  参数类别定义
    /// <summary>
    /// 参数有效范围类别:系统,页面(默认),段
    /// </summary>
    public enum ParamRangeType
    {
        /// <summary>
        /// 系统参数
        /// </summary>
        System,
        /// <summary>
        /// 页面参数
        /// </summary>
        Page,
        /// <summary>
        /// 段参数
        /// </summary>
        Band
    }

    /// <summary>
    /// 参数使用来源类别:局部临时,自定义,数据(默认)
    /// </summary>
    public enum ParamUseType
    {
        /// <summary>
        /// 局部临时
        /// </summary>
        Temp,
        /// <summary>
        /// 自定义
        /// </summary>
        CustomDefine,
        /// <summary>
        /// 数据
        /// </summary>
        Data,
        /// <summary>
        /// 传递来的
        /// </summary>
        Trans
    }

    #endregion

    /// <summary>
    /// 管理系统共用参数
    /// </summary>
    public class BuildParamList
	{
        //分组合计样式表
        static private XmlDocument     _xmlGroupSum = null;

        /// <summary>
        /// 参数来源类型,优先级 PL: B->P->S  二级 L: T->C->D
        /// </summary>
        static private string[] _typeuser ={ "T", "C", "D", "Ts" };
        /// <summary>
        /// 参数区域范围类型,
        /// </summary>
        static private string[] _typerange ={ "B", "P", "S" };
        
        #region 公共属性

        /// <summary>
        /// 传递参数的session名称
        /// </summary>
        static public string SessionNameTrans
        {
            get { return "PageParam"; }
        }

        #endregion

        public BuildParamList()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//

        }

        #region 读取参数

        /// <summary>
        /// 读取参数值
        /// </summary>
        /// <param name="xmldoc">参数列表</param>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        static public string getValue(XmlDocument xmldoc, string name)
        {
            return getValue(xmldoc, "", name);
        }

        /// <summary>
        /// 读取参数值
        /// </summary>
        /// <param name="xmldoc">参数列表</param>
        /// <param name="bandName">数据段,即Item名称</param>
        /// <param name="name">参数名</param>
        /// <returns>参数值</returns>
        static public string getValue(XmlDocument xmldoc, string bandName, string name)
        {
            if (null == xmldoc || string.IsNullOrEmpty(name))
                return "";
            for (int n = 0; n < _typerange.Length; n++)
            {
                XmlNodeList xnPLlist = null;
                if (0 == n && !string.IsNullOrEmpty(bandName))
                    xnPLlist = xmldoc.SelectNodes("//PL[@t='B' and @n='" + bandName + "']");
                else
                    xnPLlist = xmldoc.SelectNodes("//PL[@t='"+_typerange[n]+"']");
                for (int i = 0; i < xnPLlist.Count; i++)
                    for (int k = 0; k < _typeuser.Length; k++)
                    {
                        XmlNode xn = xnPLlist[i].SelectSingleNode("L[@t='" + _typeuser[k] + "']/P[@n='" + name + "']");
                        if (null == xn) continue;
                        if (null == xn.Attributes["v"])
                            return "";
                        else
                            return xn.Attributes["v"].Value;
                    }
            }
            return "";
        }

        /// <summary>
        /// 读取参数节点
        /// </summary>
        /// <param name="xmldoc">参数列表</param>
        /// <param name="name">参数名称</param>
        /// <returns>参数XmlNode节点</returns>
        static public XmlNode getXmlNode(XmlDocument xmldoc, string name)
        {
            return getXmlNode(xmldoc, "", name);
        }

        /// <summary>
        /// 读取参数值
        /// </summary>
        /// <param name="xmldoc">参数列表</param>
        /// <param name="bandName">数据段,即Item名称</param>
        /// <param name="name">参数名</param>
        /// <returns>参数XmlNode节点</returns>
        static public XmlNode getXmlNode(XmlDocument xmldoc, string bandName, string name)
        {
            if (null == xmldoc || string.IsNullOrEmpty(name))
                return null;
            for (int n = 0; n < _typerange.Length; n++)
            {
                XmlNodeList xnPLlist = null;
                if (0 == n && !string.IsNullOrEmpty(bandName))
                    xnPLlist = xmldoc.SelectNodes("//PL[@t='B' and @n='" + bandName + "']");
                else
                    xnPLlist = xmldoc.SelectNodes("//PL[@t='" + _typerange[n] + "']");
                for (int i = 0; i < xnPLlist.Count; i++)
                    for (int k = 0; k < _typeuser.Length; k++)
                    {
                        XmlNode xn = xnPLlist[i].SelectSingleNode("L[@t='" + _typeuser[k] + "']/P[@n='" + name + "']");
                        if (null == xn) continue;
                        return xn;
                    }
            }
            return null;
        }
/*
        static public XmlNode getXmlComNode(XmlDocument xmldoc, string name,string datasrc)
        {
            if (null == xmldoc || string.IsNullOrEmpty(name))
                return null;
            // 第一步找出所有的 <P n="Command" v="Save" t="s" />，
            // 再找到上述节点中所有包含workitem.datasrc及state=对应状态的值
            // <items keyvalue="6f6f8949-5667-4fdb-a5ba-235d2e4cb0c0" state="modify">
            
            XmlNodeList xnPLlist = xmldoc.SelectNodes("//P[@n='" + name + "' and @v='Save']");
            XmlNodeList xns = null;
            XmlNode xn = null;
            for (int n = 0; n < xnPLlist.Count; n++)
            {
                xn = xnPLlist[n].ParentNode;
                xns = xn.SelectNodes(datasrc);
                if (xns != null && xns.Count>0) break;
            }
            if (xns==null || xns.Count==0) return null;
            return xn;
        }
*/
        static public XmlNodeList getXmlComNode(XmlDocument xmldoc, string name, string datasrc, string OPType)
        {
            if (null == xmldoc || string.IsNullOrEmpty(name))
                return null;
            // 第一步找出所有的 <P n="Command" v="Save" t="s" />，
            // 再找到上述节点中所有包含workitem.datasrc及state=对应状态的值
            // <items keyvalue="6f6f8949-5667-4fdb-a5ba-235d2e4cb0c0" state="modify">
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmldoc.NameTable);
            string namedrc = "";
            if (datasrc.IndexOf(":") > -1)
            {
                namedrc = datasrc.Substring(0, datasrc.IndexOf(":"));
                nsmgr.AddNamespace(namedrc, "urn:schemas-microsoft-com:xml-diffgram-v1");
            }
            //XmlNode book = doc.SelectSingleNode("//ab:book", nsmgr); 

            XmlNodeList xnPLlist = xmldoc.SelectNodes("//P[@n='" + name + "' and @v='Save']");
            XmlNodeList xns = null;
            for (int n = 0; n < xnPLlist.Count; n++)
            {
                XmlNode xn = xnPLlist[n].ParentNode;
                if (datasrc.IndexOf(":") > -1)
                    xns = xn.SelectNodes("//"+datasrc + "[@state='" + OPType + "']",nsmgr);
                else
                    xns = xn.SelectNodes("//" + datasrc + "[@state='" + OPType + "']");
                if (xns != null && xns.Count > 0) break;
            }
            if (xns == null || xns.Count == 0) return null;
            return xns;
        }
        

        #endregion

        #region  设置参数
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xmldoc">参数列表文档</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>返回参数节点</returns>
        static public XmlElement setValue(XmlDocument xmldoc, string key, string value)
        {
            return setValue(xmldoc, ParamRangeType.Page, "", ParamUseType.Temp, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xmldoc">参数列表文档</param>
        /// <param name="PR">参数区域类别</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>返回参数节点</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string key, string value)
        {
            return setValue(xmldoc, PR, "", ParamUseType.Temp, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xmldoc">参数列表文档</param>
        /// <param name="PR">参数区域类别</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="dbtype">参数类型</param>
        /// <returns>返回参数节点</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string key, string value, DBTypeCommon dbtype)
        {
            return setValue(xmldoc, PR, "", ParamUseType.Temp, key, value, dbtype);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xmldoc">参数列表文档</param>
        /// <param name="PR">参数区域类别</param>
        /// <param name="L">参数使用来源类别</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>返回参数节点</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, ParamUseType L, string key, string value)
        {
            return setValue(xmldoc, PR, "", L, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xmldoc">参数列表文档</param>
        /// <param name="PR">参数区域类别</param>
        /// <param name="rangeName">参数区域名称</param>
        /// <param name="L">参数使用来源类别</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>返回参数节点</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string rangeName, ParamUseType L, string key, string value)
        {
            return setValue(xmldoc, PR, rangeName, L, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xmldoc">参数列表文档</param>
        /// <param name="PR">参数区域类别</param>
        /// <param name="rangeName">参数区域名称</param>
        /// <param name="L">参数使用来源类别</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="dbtype">参数数据类型</param>
        /// <returns>返回参数节点</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string rangeName, ParamUseType L, string key, string value, DBTypeCommon  dbtype)
        {
            if (xmldoc == null || string.IsNullOrEmpty(key))
                return null;
            if (xmldoc.DocumentElement == null)
                try
                {
                    xmldoc.LoadXml("<D></D>");
                }
                catch
                {
                   
                }
            //对于系统参数和页面参数,只有一个区域,名称后边覆盖前边
            string strPRName = "P";
            if (ParamRangeType.System == PR) strPRName = "S";
            if (ParamRangeType.Page == PR) strPRName = "P";
            if (ParamRangeType.Band == PR) strPRName = "B";
            XmlNode xnPR = xmldoc.SelectSingleNode("//PL[@t='" + strPRName + "']");
            if (ParamRangeType.Band == PR && !string.IsNullOrEmpty(rangeName))
            {
                xnPR = xmldoc.SelectSingleNode("//PL[@t='B' and @n='" + rangeName + "']");
                if (null == xnPR)
                    xnPR = xmldoc.SelectSingleNode("//PL[@t='B' and not(@n)]");
            }
            if (null == xnPR)
            {
                xnPR = xmldoc.CreateElement("PL");
                xnPR = xmldoc.DocumentElement.AppendChild(xnPR);
                ((XmlElement)xnPR).SetAttribute("t", strPRName);
            }
            if (!string.IsNullOrEmpty(rangeName))
                ((XmlElement)xnPR).SetAttribute("n", rangeName);

            string strLName="T";
            if (ParamUseType.Temp == L) strLName = "T";
            if (ParamUseType.CustomDefine == L) strLName = "C";
            if (ParamUseType.Data == L) strLName = "D";
            if (ParamUseType.Trans == L) strLName = "Ts";
            XmlNode xnL = xnPR.SelectSingleNode("L[@t='"+strLName+"']");
            if (null == xnL)
            {
                xnL = xmldoc.CreateElement("L");
                xnL = xnPR.AppendChild(xnL);
                ((XmlElement)xnL).SetAttribute("t", strLName);
            }
            
            XmlElement xeParam = xnL.SelectSingleNode("P[@n='"+key+"']") as XmlElement;
            if (null == xeParam)
            {
                xeParam = xmldoc.CreateElement("P");
                xeParam = xnL.AppendChild(xeParam) as XmlElement;
                xeParam.SetAttribute("n", key);
            }
            xeParam.SetAttribute("v", value);
            switch (dbtype)
            {
                case DBTypeCommon.String:
                    xeParam.SetAttribute("t", "s");
                    break;
                case DBTypeCommon.Int:
                    xeParam.SetAttribute("t", "i");
                    break;
                case DBTypeCommon.Decimal:
                    xeParam.SetAttribute("t", "f");
                    break;
                case DBTypeCommon.DateTime:
                    xeParam.SetAttribute("t", "d");
                    break;
                case DBTypeCommon.Bool:
                    xeParam.SetAttribute("t", "b");
                    break;
                case DBTypeCommon.Guid:
                    xeParam.SetAttribute("t", "guid");
                    break;
                default:
                    xeParam.SetAttribute("t", "s");
                    break;
            }
            return xeParam;
        }

        #endregion

        #region 生成参数

        /// <summary>
        /// 根据参数Xml文档列表生成参数:
        /// </summary>
        /// <param name="xmldoc">文档参数</param>
        /// <returns>返回参数</returns>
        static public NameObjectList BuildParams(XmlDocument xmldoc)
        {
            string strPath = "/*/PL[@t='{0}']/L[@t='{1}']/P[not(@pt) or @pt!='M']";
            string strPass = "/*/PL[@t='{0}']/L[@t='{1}']/P[@n='{2}']";
            NameObjectList paramList = new NameObjectList();
            if (xmldoc == null) return paramList;
            for(int i=_typerange.Length-1;i>-1;i--)
                for (int j = _typeuser.Length - 1; j > -1; j--)
                {
                    string strFind = string.Format(strPath, _typerange[i], _typeuser[j]);
                    XmlNodeList xnlist = xmldoc.SelectNodes(strFind);
                    foreach (XmlNode xnP in xnlist)
                    {
                        if (null == xnP.Attributes["n"]) continue;
                        string key = xnP.Attributes["n"].Value;
                        bool isParam = true;
                        for (int m = i ; m > -1; m--)
                        {
                            for (int n = j - 1; n > -1; n--)
                            {
                                string strFindPass = string.Format(strPass, _typerange[m], _typeuser[n], key);
                                XmlNode xnPtemp = xnP.SelectSingleNode(strFindPass);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                            }
                            if (!isParam) break;
                        }
                        //增加到参数列表
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        /// <summary>
        /// 根据参数Xml文档列表生成指定段名的参数:
        /// </summary>
        /// <param name="xmldoc">文档参数</param>
        /// <param name="bandName">段名称</param>
        /// <returns>返回参数</returns>
        static public NameObjectList BuildParams(XmlDocument xmldoc, string bandName)
        {
            if (string.IsNullOrEmpty(bandName))
                return BuildParams(xmldoc);
            string strPath = "/*/PL[(@t='{0}' and @t!='B')or(@t='B' and @n='{2}')]/L[@t='{1}']/P[not(@pt) or @pt!='M']";
            string strPass = "/*/PL[(@t='{0}' and @t!='B')or(@t='B' and @n='{3}')]/L[@t='{1}']/P[@n='{2}']";
            NameObjectList paramList = new NameObjectList();
            for (int i = _typerange.Length - 1; i > -1; i--)
                for (int j = _typeuser.Length - 1; j > -1; j--)
                {
                    string strFind = string.Format(strPath, _typerange[i], _typeuser[j], bandName);
                    XmlNodeList xnlist = xmldoc.SelectNodes(strFind);
                    foreach (XmlNode xnP in xnlist)
                    {
                        if (null == xnP.Attributes["n"]) continue;
                        string key = xnP.Attributes["n"].Value;
                        bool isParam = true;
                        for (int m = i; m > -1; m--)
                        {
                            for (int n = j - 1; n > -1; n--)
                            {
                                string strFindPass = string.Format(strPass, _typerange[m], _typeuser[n], key, bandName);
                                XmlNode xnPtemp = xnP.SelectSingleNode(strFindPass);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                            }
                            if (!isParam) break;
                        }
                        //增加到参数列表
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        /// <summary>
        /// 根据参数Xml文档列表生成参数:数组List只限定于PL[@t='B']/L[@='D']/List
        /// </summary>
        /// <param name="xmldoc">文档参数</param>
        /// <returns>返回参数数组</returns>
        static public NameObjectList[] BuildParamsList(XmlDocument xmldoc)
        {
            string strPath = "/*/PL[@t='{0}']/L[@t='{1}']/P[not(@pt) or @pt!='M']";
            string strPath2 = "/*/PL[@t='{0}']/L[@t='{1}']/List/P";
            string strPass = "/*/PL[@t='{0}']/L[@t='{1}']/P[@n='{2}']";
            string strPass2 = "/*/PL[@t='{0}']/L[@t='{1}']/List/P[@n='{2}']";
            NameObjectList paramList = new NameObjectList();
            for (int i = _typerange.Length - 1; i > -1; i--)
                for (int j = _typeuser.Length - 1; j > -1; j--)
                {
                    string strFind = string.Format(strPath, _typerange[i], _typeuser[j]);
                    if (null != xmldoc.SelectSingleNode(string.Format(strPath2, _typerange[i], _typeuser[j])))
                        continue;
                    XmlNodeList xnlist = xmldoc.SelectNodes(strFind);
                    foreach (XmlNode xnP in xnlist)
                    {
                        if (null == xnP.Attributes["n"]) continue;
                        string key = xnP.Attributes["n"].Value;
                        bool isParam = true;
                        for (int m = i; m > -1; m--)
                        {
                            for (int n = j - 1; n > -1; n--)
                            {
                                string strFindPass = string.Format(strPass, _typerange[m], _typeuser[n], key);
                                XmlNode xnPtemp = xnP.SelectSingleNode(strFindPass);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                                string strFindPass2 = string.Format(strPass2, _typerange[m], _typeuser[n], key);
                                xnPtemp = xnP.SelectSingleNode(strFindPass2);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                            }
                            if (!isParam) break;
                        }
                        //增加到参数列表
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }

            strPath2 = "/*/PL[@t='{0}']/L[@t='{1}']/List";
            string strFind2 = string.Format(strPath2, "B", "D");
            XmlNodeList xnlistList = xmldoc.SelectNodes(strFind2);
            NameObjectList[] param = new NameObjectList[xnlistList.Count];
            for (int i = 0; i < xnlistList.Count; i++)
            {
                param[i] = new NameObjectList();
                for (int k = 0; k < paramList.Count; k++)
                    param[i][paramList.Keys[k]] = paramList[k];
                XmlNodeList xnlist = xnlistList[i].SelectNodes("P");
                foreach (XmlNode xnP in xnlist)
                    BuildParamList.setParamByParamNode(param[i], xnP);
            }
            return param;
        }

        /// <summary>
        /// 根据参数Xml文档列表生成指定段对应参数:数组List只限定于PL[@t='B']/L[@='D']/List
        /// </summary>
        /// <param name="xmldoc">文档参数</param>
        /// <param name="bandName">段名称</param>
        /// <returns>返回参数数组</returns>
        static public NameObjectList[] BuildParamsList(XmlDocument xmldoc, string   bandName)
        {
            if (string.IsNullOrEmpty(bandName))
                return BuildParamsList(xmldoc);
            string strPath  = "/*/PL[(@t='{0}'and @t!='B')or(@t='B' and @n='{2}')]/L[@t='{1}']/P[not(@pt) or @pt!='M']";
            string strPath2 = "/*/PL[(@t='{0}'and @t!='B')or(@t='B' and @n='{2}')]/L[@t='{1}']/List/P";
            string strPass  = "/*/PL[(@t='{0}'and @t!='B')or(@t='B' and @n='{3}')]/L[@t='{1}']/P[@n='{2}']";
            string strPass2 = "/*/PL[(@t='{0}'and @t!='B')or(@t='B' and @n='{3}')]/L[@t='{1}']/List/P[@n='{2}']";
            NameObjectList paramList = new NameObjectList();
            for (int i = _typerange.Length - 1; i > -1; i--)
                for (int j = _typeuser.Length - 1; j > -1; j--)
                {
                    string strFind = string.Format(strPath, _typerange[i], _typeuser[j], bandName);
                    if (null != xmldoc.SelectSingleNode(string.Format(strPath2, _typerange[i], _typeuser[j], bandName)))
                        continue;
                    XmlNodeList xnlist = xmldoc.SelectNodes(strFind);
                    foreach (XmlNode xnP in xnlist)
                    {
                        if (null == xnP.Attributes["n"]) continue;
                        string key = xnP.Attributes["n"].Value;
                        bool isParam = true;
                        for (int m = i; m > -1; m--)
                        {
                            for (int n = j - 1; n > -1; n--)
                            {
                                string strFindPass = string.Format(strPass, _typerange[m], _typeuser[n], key, bandName);
                                XmlNode xnPtemp = xnP.SelectSingleNode(strFindPass);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                                string strFindPass2 = string.Format(strPass2, _typerange[m], _typeuser[n], key, bandName);
                                xnPtemp = xnP.SelectSingleNode(strFindPass2);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                            }
                            if (!isParam) break;
                        }
                        //增加到参数列表
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }

            strPath2 = "/*/PL[(@t='{0}'and @t!='B')or(@t='B' and @n='{2}')]/L[@t='{1}']/List";
            string strFind2 = string.Format(strPath2, "B", "D",bandName);
            XmlNodeList xnlistList = xmldoc.SelectNodes(strFind2);
            NameObjectList[] param = new NameObjectList[xnlistList.Count];
            for (int i = 0; i < xnlistList.Count; i++)
            {
                param[i] = new NameObjectList();
                for (int k = 0; k < paramList.Count; k++)
                    param[i][paramList.Keys[k]] = paramList[k];
                XmlNodeList xnlist = xnlistList[i].SelectNodes("P");
                foreach (XmlNode xnP in xnlist)
                    BuildParamList.setParamByParamNode(param[i], xnP);
            }
            return param;
        }

        /// <summary>
        /// 根据参数Xml文档列表生成宏变量参数:
        /// </summary>
        /// <param name="xmldoc">文档参数</param>
        /// <returns>返回参数</returns>
        static public NameObjectList BuildParamMacro(XmlDocument xmldoc)
        {
       
            string strPath = "/*/PL[@t='{0}']/L[@t='{1}']/P[@pt='M']";
            string strPass = "/*/PL[@t='{0}']/L[@t='{1}']/P[@n='{2}']";
            NameObjectList paramList = new NameObjectList();
            for (int i = _typerange.Length - 1; i > -1; i--)
                for (int j = _typeuser.Length - 1; j > -1; j--)
                {
                    string strFind = string.Format(strPath, _typerange[i], _typeuser[j]);
                    XmlNodeList xnlist = xmldoc.SelectNodes(strFind);
                    foreach (XmlNode xnP in xnlist)
                    {
                        if (null == xnP.Attributes["n"]) continue;
                        string key = xnP.Attributes["n"].Value;
                        bool isParam = true;
                        for (int m = i; m > -1; m--)
                        {
                            for (int n = j - 1; n > -1; n--)
                            {
                                string strFindPass = string.Format(strPass, _typerange[m], _typeuser[n], key);
                                XmlNode xnPtemp = xnP.SelectSingleNode(strFindPass);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                            }
                            if (!isParam) break;
                        }
                        //增加到参数列表
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        /// <summary>
        /// 根据参数Xml文档列表生成指定段名的宏变量参数:
        /// </summary>
        /// <param name="xmldoc">文档参数</param>
        /// <param name="bandName">段名称</param>
        /// <returns>返回参数</returns>
        static public NameObjectList BuildParamMacro(XmlDocument xmldoc, string bandName)
        {
            if (string.IsNullOrEmpty(bandName))
                return BuildParams(xmldoc);
            string strPath = "/*/PL[(@t='{0}' and @t!='B')or(@t='B' and @n='{2}')]/L[@t='{1}']/P[@pt='M']";
            string strPass = "/*/PL[(@t='{0}' and @t!='B')or(@t='B' and @n='{3}')]/L[@t='{1}']/P[@n='{2}']";
            NameObjectList paramList = new NameObjectList();
            for (int i = _typerange.Length - 1; i > -1; i--)
                for (int j = _typeuser.Length - 1; j > -1; j--)
                {
                    string strFind = string.Format(strPath, _typerange[i], _typeuser[j], bandName);
                    XmlNodeList xnlist = xmldoc.SelectNodes(strFind);
                    foreach (XmlNode xnP in xnlist)
                    {
                        if (null == xnP.Attributes["n"]) continue;
                        string key = xnP.Attributes["n"].Value;
                        bool isParam = true;
                        for (int m = i; m > -1; m--)
                        {
                            for (int n = j - 1; n > -1; n--)
                            {
                                string strFindPass = string.Format(strPass, _typerange[m], _typeuser[n], key, bandName);
                                XmlNode xnPtemp = xnP.SelectSingleNode(strFindPass);
                                if (null != xnPtemp)
                                {
                                    isParam = false;
                                    break;
                                }
                            }
                            if (!isParam) break;
                        }
                        //增加到参数列表
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        #endregion

        #region 内部函数

		/// <summary>
		/// 根据<Param name="" value="" type=""></Param>节点设置参数
		/// </summary>
		/// <param name="paramList">参数列表</param>
		/// <param name="xmlParam">xml的参数值</param>
		static private void		setParamByParamNode(NameObjectList	paramList,XmlNode	xmlParam)
		{
            string strName = ""; string strValue = ""; string strtype = "";
            if (null == xmlParam.Attributes["name"] && null == xmlParam.Attributes["n"])
                return;
            if (null != xmlParam.Attributes["name"])
                strName = xmlParam.Attributes["name"].Value;
            else
                strName = xmlParam.Attributes["n"].Value;
            if (null != xmlParam.Attributes["value"])
                strValue = xmlParam.Attributes["value"].Value;
            else if (null != xmlParam.Attributes["v"])
                strValue = xmlParam.Attributes["v"].Value;
            if (null != xmlParam.Attributes["type"])
                strtype = xmlParam.Attributes["type"].Value;
            else if (null != xmlParam.Attributes["t"])
                strtype = xmlParam.Attributes["t"].Value;

			try
			{
                if (string.IsNullOrEmpty(strValue) && null == paramList[strName])
                {
                    paramList[strName] = DBNull.Value;
                    return;
                }
                //if (paramList[strName].ToString() != "" && strValue=="") return; 
				if(string.IsNullOrEmpty(strtype))
				{
					try
					{
						Guid	guid=new Guid(strValue);
						paramList[strName]=guid;
					}
					catch
					{
						paramList[strName]=strValue;
					}
				}else
					switch(strtype.ToLower())
					{
						case "int":
                        case "i":
							paramList[strName]=int.Parse(strValue);
							break;
						case "decimal":
                        case "f":
							paramList[strName]=decimal.Parse(strValue);
							break;
						case "bool":
                        case "b":
							paramList[strName]=bool.Parse(strValue);
							break;
						case "datetime":
                        case "date":
                        case "d":
							paramList[strName]=DateTime.Parse(strValue);
							break;
						case "string":
                        case "s":
								paramList[strName]=strValue;
							break;
						case "guid":
							paramList[strName]=new Guid(strValue);
							break;
                        case "macro":
                            break;
						default:	//可能是Guid
							paramList[strName]=strValue;
							break;
					}
			}
			catch
			{
				if(null==paramList[strName])
					paramList[strName]=DBNull.Value;
			}
        }

        #endregion

        #region 识别安全性
        /// <summary>
		/// 是否是敏感信息,敏感信息不能存入视图状态中
		/// </summary>
		/// <param name="key">session值名称</param>
		/// <returns></returns>
		static public bool		IsSensitivity(string  key)
		{
            if ("" == key || string.Empty == key)
                return false;
            key = key.ToLower();
			if("userid"==key)	    return true;
            if ("userkey" == key) return true;
            if ("appmodel" == key) return true;
            if ("sysparam" == key) return true;
			return false;
        }

        #endregion

        #region 数据XmlDoc计算

        /// <summary>
        /// 获取进行数据文档汇总分组汇总的XSLT样式表
        /// </summary>
        /// <returns>返回分组合计样式表</returns>
        static private XmlDocument getXSLGroupSum()
        {
            if (null != _xmlGroupSum) return _xmlGroupSum;
	        string xslSumHTML="<XML> "
		        +"	<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'> "
		        +"		<xsl:key name='pkname' match='DataRow' use='年份' /> "
		        +"		<xsl:template match='/'> "
		        +"			<xsl:for-each select='*'> "
		        +"				<xsl:element name='{name()}'> "
		        +"					<xsl:call-template name='tempsum'> "
		        +"						<xsl:with-param name='groupby'>年份</xsl:with-param> "
		        +"					</xsl:call-template> "
		        +"				</xsl:element> "
		        +"			</xsl:for-each> "
		        +"		</xsl:template> "
		        +"		<xsl:template name='tempsum'> "
		        +"			<xsl:param name='groupby' /> "
		        +"			<xsl:for-each select='*[generate-id()=generate-id(key(\"pkname\",*[name()=$groupby])[1])]'> "
		        +"				<xsl:element name='{name()}'> "
		        +"					<xsl:variable name='keyvalue' select='*[name()=$groupby]' /> "
		        +"					<xsl:element name='{$groupby}' ><xsl:value-of select='$keyvalue' /></xsl:element> "
		        +"				</xsl:element> "
		        +"			</xsl:for-each> "
		        +"		</xsl:template> "
		        +"	</xsl:stylesheet> "
		        +"</XML> ";
            _xmlGroupSum.LoadXml(xslSumHTML);
            return _xmlGroupSum;
        }

        /// <summary>
        /// 对xmlData数据文档按照strFld字段进行分组合计;xmlSchema数据文档的结构
        /// </summary>
        /// <param name="xmlData">数据文档</param>
        /// <param name="xmlSchema">数据文档结构</param>
        /// <param name="strFld">数据文档分组合计的字段</param>
        /// <returns>没有数据返回null,成功返回null</returns>
        static public XmlReader XmlGroupSum(XmlDocument xmlData, XmlDocument xmlSchema, string strFld)
        {
            if (null==xmlData || null==xmlSchema || ""==strFld || string.Empty==strFld)
                return null;
            XmlNode nodeRow = xmlData.SelectSingleNode("/*/*");
            if (null == nodeRow) return null;
            XmlNamespaceManager xmlNsMgl = new XmlNamespaceManager(xmlSchema.NameTable);
            XmlNode xmlRootEle = xmlSchema.DocumentElement;
            for (int i = 0; i < xmlRootEle.Attributes.Count; i++)
            {
                string strPrefix = xmlRootEle.Attributes[i].Prefix;
                string strLocalName = xmlRootEle.Attributes[i].LocalName;
                string strURI = xmlRootEle.Attributes[i].Value;
                if ("xmlns" == strLocalName)
                    xmlNsMgl.AddNamespace(string.Empty, strURI);
                if ("xmlns" != strPrefix) continue;
                xmlNsMgl.AddNamespace(strLocalName, strURI);
            }
            //添加汇总列模板参数分组的列名;
            XmlDocument XSLGroupSum = BuildParamList.getXSLGroupSum();
            XmlNamespaceManager xmlNsMglGS = new XmlNamespaceManager(XSLGroupSum.NameTable);
            xmlRootEle = XSLGroupSum.DocumentElement;
            for (int i = 0; i < xmlRootEle.Attributes.Count; i++)
            {
                string strPrefix = xmlRootEle.Attributes[i].Prefix;
                string strLocalName = xmlRootEle.Attributes[i].LocalName;
                string strURI = xmlRootEle.Attributes[i].Value;
                if ("xmlns" == strLocalName)
                    xmlNsMglGS.AddNamespace(string.Empty, strURI);
                if ("xmlns" != strPrefix) continue;
                xmlNsMglGS.AddNamespace(strLocalName, strURI);
            }
            XmlElement eleKey = XSLGroupSum.SelectSingleNode("//xsl:key", xmlNsMglGS) as XmlElement;
            XmlElement eleTpCall = XSLGroupSum.SelectSingleNode("//xsl:call-template", xmlNsMglGS) as XmlElement;
            XmlElement eleTp = XSLGroupSum.SelectSingleNode("//xsl:template[@name='tempsum']", xmlNsMglGS) as XmlElement;
            XmlElement  eleTpCopy = eleTp.CloneNode(true) as XmlElement;
            XmlElement  eleTpCallCopy = eleTpCall.CloneNode(true) as XmlElement;
            XmlElement eleParam = eleTpCall.SelectSingleNode("xsl:with-param[@name='groupby']", xmlNsMglGS) as XmlElement;
            string strXML = XSLGroupSum.OuterXml;
            
            eleParam.InnerText = strFld;
            eleKey.SetAttribute("match",nodeRow.LocalName);
            eleKey.SetAttribute("use",strFld);

            XmlElement eleRowGroup = eleTp.SelectSingleNode("xsl:for-each/xsl:element",xmlNsMglGS) as XmlElement;
            XmlNodeList colList = xmlSchema.SelectNodes("//xs:sequence//xs:element[@footer!='' and ( @type='xs:int' or @type='xs:number' or type='xs:float' or type='xs:decimal' or type='xs:double' )]", xmlNsMgl);
            for (int i = 0; i < colList.Count; i++)
            {
                XmlElement colEle = colList[i] as XmlElement;
                string colname = colEle.GetAttribute("name");
                string sumfoot = colEle.GetAttribute("footer");
                eleParam = eleTpCall.InsertBefore(eleTpCall.OwnerDocument.CreateElement("xsl:with-param"), eleTpCall.FirstChild) as XmlElement;
                eleParam.SetAttribute("name", "col" + i);
                eleParam.InnerText = colname;
                eleParam = eleTp.InsertBefore(eleTp.OwnerDocument.CreateElement("xsl:param"), eleTp.FirstChild) as XmlElement;
                eleParam.SetAttribute("name", "col" + i);

                XmlElement eleCol = eleRowGroup.AppendChild(eleRowGroup.OwnerDocument.CreateElement("xsl:element")) as XmlElement;
                eleCol.SetAttribute("name", "{$col" + i + "}");
                XmlElement eleValue = eleCol.AppendChild(eleCol.OwnerDocument.CreateElement("xsl:value-of")) as XmlElement;
                eleValue.SetAttribute("select", "sum(key('pkname',$keyvalue)/*[name()=$col" + i + "])");
            }
            if (colList.Count < 1)
            {
                XSLGroupSum.LoadXml(strXML);
                return null;
            }
            XslTransform xsltGS = new XslTransform();
            xsltGS.Load(XSLGroupSum);
            XmlReader xmlrd = xsltGS.Transform(xmlData, null);
            XSLGroupSum.LoadXml(strXML);
            return xmlrd;

        }

        #endregion


    }
}
