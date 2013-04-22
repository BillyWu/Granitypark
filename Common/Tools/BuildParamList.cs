using System;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Xsl;
using System.Web;
using System.Web.SessionState;

#region ˵��
/// <summary>
/// ����ϵͳ���ò���:UnitName,StartDate,EndDate,UserAccounts,StartPreDate,EndNextDate
///					 UserName,DeptName,DeptCode,DWName,DeptSaleName,OPTUnitID,LimitDays
/// UnitName ���λ��:<PL t='P'><L t='D'><P n='UnitName' v='blank' t='s'/></L></PL>
/// ����xmldocument�ṹ
/// ������Ϊ��������:  ϵͳ����,ҳ�����,�ֲ���Band����
/// �������λ�ü�����:
///       ϵͳ����ֻ�����session��,ҳ�治�����ⲿ�ֲ���;session["sysparam"]
///       ҳ�������ҳ�洫��,�����ִ������: 1,ҳ���ͨ��session����; 2,XMLHttpֱ�Ӵ��ݸ�ҳ�� 3,ҳ������ش�(�������͵Ĳ����ǲ�ε�)
///       �ֲ���Band����, �ڿͻ���ҳ�����̨�Ľ���������,���ݲ�ͬ������ݶ�Band��ǰ�в���,Ĭ��������
/// �ڿͻ��˵Ĳ����ṹ:
///       ϵͳ����(�����д���ʹ��),  ֻ�ṩ�ͻ���jsʹ��
///       ȫ�ֲ���: ҳ����յĴ��ݲ���,   ҳ���Զ������     �ֲ���ʱ����
///       �ֲ���Band����:   ��Band��ǰ��(���������ݶ���,���ڲ������ݵ�)     ���Զ������     ����ʱ����
/// ��ʱ����:
///       ��ÿ�δ��ݺ󼴿���ʧ�Ĳ���,����Ӱ����һ��ʹ��
/// ��������:
///       �ڶβ������� PL[@t="B"]/L[@t="D"]/List/P �γɲ�������
/// �������ȼ�: ��->ҳ->ϵ ;  ��ʱ->�Զ���->����->����
/// ��ҳ�䴫��ʱ,����ҳ��Ŀͻ��˲���Ҫ��һת��:��ǰһҳ��Ĵ��ݲ�������,�������������γɽ���ҳ��Ĵ��ݲ���
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
    
    #region  ���������
    /// <summary>
    /// ������Ч��Χ���:ϵͳ,ҳ��(Ĭ��),��
    /// </summary>
    public enum ParamRangeType
    {
        /// <summary>
        /// ϵͳ����
        /// </summary>
        System,
        /// <summary>
        /// ҳ�����
        /// </summary>
        Page,
        /// <summary>
        /// �β���
        /// </summary>
        Band
    }

    /// <summary>
    /// ����ʹ����Դ���:�ֲ���ʱ,�Զ���,����(Ĭ��)
    /// </summary>
    public enum ParamUseType
    {
        /// <summary>
        /// �ֲ���ʱ
        /// </summary>
        Temp,
        /// <summary>
        /// �Զ���
        /// </summary>
        CustomDefine,
        /// <summary>
        /// ����
        /// </summary>
        Data,
        /// <summary>
        /// ��������
        /// </summary>
        Trans
    }

    #endregion

    /// <summary>
    /// ����ϵͳ���ò���
    /// </summary>
    public class BuildParamList
	{
        //����ϼ���ʽ��
        static private XmlDocument     _xmlGroupSum = null;

        /// <summary>
        /// ������Դ����,���ȼ� PL: B->P->S  ���� L: T->C->D
        /// </summary>
        static private string[] _typeuser ={ "T", "C", "D", "Ts" };
        /// <summary>
        /// ��������Χ����,
        /// </summary>
        static private string[] _typerange ={ "B", "P", "S" };
        
        #region ��������

        /// <summary>
        /// ���ݲ�����session����
        /// </summary>
        static public string SessionNameTrans
        {
            get { return "PageParam"; }
        }

        #endregion

        public BuildParamList()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//

        }

        #region ��ȡ����

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="xmldoc">�����б�</param>
        /// <param name="name">��������</param>
        /// <returns>����ֵ</returns>
        static public string getValue(XmlDocument xmldoc, string name)
        {
            return getValue(xmldoc, "", name);
        }

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="xmldoc">�����б�</param>
        /// <param name="bandName">���ݶ�,��Item����</param>
        /// <param name="name">������</param>
        /// <returns>����ֵ</returns>
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
        /// ��ȡ�����ڵ�
        /// </summary>
        /// <param name="xmldoc">�����б�</param>
        /// <param name="name">��������</param>
        /// <returns>����XmlNode�ڵ�</returns>
        static public XmlNode getXmlNode(XmlDocument xmldoc, string name)
        {
            return getXmlNode(xmldoc, "", name);
        }

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="xmldoc">�����б�</param>
        /// <param name="bandName">���ݶ�,��Item����</param>
        /// <param name="name">������</param>
        /// <returns>����XmlNode�ڵ�</returns>
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
            // ��һ���ҳ����е� <P n="Command" v="Save" t="s" />��
            // ���ҵ������ڵ������а���workitem.datasrc��state=��Ӧ״̬��ֵ
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
            // ��һ���ҳ����е� <P n="Command" v="Save" t="s" />��
            // ���ҵ������ڵ������а���workitem.datasrc��state=��Ӧ״̬��ֵ
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

        #region  ���ò���
        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="xmldoc">�����б��ĵ�</param>
        /// <param name="key">��������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>���ز����ڵ�</returns>
        static public XmlElement setValue(XmlDocument xmldoc, string key, string value)
        {
            return setValue(xmldoc, ParamRangeType.Page, "", ParamUseType.Temp, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="xmldoc">�����б��ĵ�</param>
        /// <param name="PR">�����������</param>
        /// <param name="key">��������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>���ز����ڵ�</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string key, string value)
        {
            return setValue(xmldoc, PR, "", ParamUseType.Temp, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="xmldoc">�����б��ĵ�</param>
        /// <param name="PR">�����������</param>
        /// <param name="key">��������</param>
        /// <param name="value">����ֵ</param>
        /// <param name="dbtype">��������</param>
        /// <returns>���ز����ڵ�</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string key, string value, DBTypeCommon dbtype)
        {
            return setValue(xmldoc, PR, "", ParamUseType.Temp, key, value, dbtype);
        }

        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="xmldoc">�����б��ĵ�</param>
        /// <param name="PR">�����������</param>
        /// <param name="L">����ʹ����Դ���</param>
        /// <param name="key">��������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>���ز����ڵ�</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, ParamUseType L, string key, string value)
        {
            return setValue(xmldoc, PR, "", L, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="xmldoc">�����б��ĵ�</param>
        /// <param name="PR">�����������</param>
        /// <param name="rangeName">������������</param>
        /// <param name="L">����ʹ����Դ���</param>
        /// <param name="key">��������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>���ز����ڵ�</returns>
        static public XmlElement setValue(XmlDocument xmldoc, ParamRangeType PR, string rangeName, ParamUseType L, string key, string value)
        {
            return setValue(xmldoc, PR, rangeName, L, key, value, DBTypeCommon.String);
        }

        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="xmldoc">�����б��ĵ�</param>
        /// <param name="PR">�����������</param>
        /// <param name="rangeName">������������</param>
        /// <param name="L">����ʹ����Դ���</param>
        /// <param name="key">��������</param>
        /// <param name="value">����ֵ</param>
        /// <param name="dbtype">������������</param>
        /// <returns>���ز����ڵ�</returns>
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
            //����ϵͳ������ҳ�����,ֻ��һ������,���ƺ�߸���ǰ��
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

        #region ���ɲ���

        /// <summary>
        /// ���ݲ���Xml�ĵ��б����ɲ���:
        /// </summary>
        /// <param name="xmldoc">�ĵ�����</param>
        /// <returns>���ز���</returns>
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
                        //���ӵ������б�
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        /// <summary>
        /// ���ݲ���Xml�ĵ��б�����ָ�������Ĳ���:
        /// </summary>
        /// <param name="xmldoc">�ĵ�����</param>
        /// <param name="bandName">������</param>
        /// <returns>���ز���</returns>
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
                        //���ӵ������б�
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        /// <summary>
        /// ���ݲ���Xml�ĵ��б����ɲ���:����Listֻ�޶���PL[@t='B']/L[@='D']/List
        /// </summary>
        /// <param name="xmldoc">�ĵ�����</param>
        /// <returns>���ز�������</returns>
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
                        //���ӵ������б�
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
        /// ���ݲ���Xml�ĵ��б�����ָ���ζ�Ӧ����:����Listֻ�޶���PL[@t='B']/L[@='D']/List
        /// </summary>
        /// <param name="xmldoc">�ĵ�����</param>
        /// <param name="bandName">������</param>
        /// <returns>���ز�������</returns>
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
                        //���ӵ������б�
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
        /// ���ݲ���Xml�ĵ��б����ɺ��������:
        /// </summary>
        /// <param name="xmldoc">�ĵ�����</param>
        /// <returns>���ز���</returns>
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
                        //���ӵ������б�
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        /// <summary>
        /// ���ݲ���Xml�ĵ��б�����ָ�������ĺ��������:
        /// </summary>
        /// <param name="xmldoc">�ĵ�����</param>
        /// <param name="bandName">������</param>
        /// <returns>���ز���</returns>
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
                        //���ӵ������б�
                        if (isParam)
                            BuildParamList.setParamByParamNode(paramList, xnP);
                    }//foreach (XmlNode xnP in xnlist)
                }
            return paramList;
        }

        #endregion

        #region �ڲ�����

		/// <summary>
		/// ����<Param name="" value="" type=""></Param>�ڵ����ò���
		/// </summary>
		/// <param name="paramList">�����б�</param>
		/// <param name="xmlParam">xml�Ĳ���ֵ</param>
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
						default:	//������Guid
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

        #region ʶ��ȫ��
        /// <summary>
		/// �Ƿ���������Ϣ,������Ϣ���ܴ�����ͼ״̬��
		/// </summary>
		/// <param name="key">sessionֵ����</param>
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

        #region ����XmlDoc����

        /// <summary>
        /// ��ȡ���������ĵ����ܷ�����ܵ�XSLT��ʽ��
        /// </summary>
        /// <returns>���ط���ϼ���ʽ��</returns>
        static private XmlDocument getXSLGroupSum()
        {
            if (null != _xmlGroupSum) return _xmlGroupSum;
	        string xslSumHTML="<XML> "
		        +"	<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'> "
		        +"		<xsl:key name='pkname' match='DataRow' use='���' /> "
		        +"		<xsl:template match='/'> "
		        +"			<xsl:for-each select='*'> "
		        +"				<xsl:element name='{name()}'> "
		        +"					<xsl:call-template name='tempsum'> "
		        +"						<xsl:with-param name='groupby'>���</xsl:with-param> "
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
        /// ��xmlData�����ĵ�����strFld�ֶν��з���ϼ�;xmlSchema�����ĵ��Ľṹ
        /// </summary>
        /// <param name="xmlData">�����ĵ�</param>
        /// <param name="xmlSchema">�����ĵ��ṹ</param>
        /// <param name="strFld">�����ĵ�����ϼƵ��ֶ�</param>
        /// <returns>û�����ݷ���null,�ɹ�����null</returns>
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
            //��ӻ�����ģ��������������;
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
