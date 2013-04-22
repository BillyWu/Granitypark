using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace Estar.Common.Tools
{
	/// <summary>
	/// �о��ṩ�����ݷ��ʷ�ʽ
	/// </summary>
	public enum DataAccessType
	{
		SqlClient,OLEDB,ODPNet
	}

    /// <summary>
    /// ���ݿ����Ӷ���;type: SqlClient,OLEDB,ODPNet
    /// </summary>
    public class DataConnInfo : ConfigurationElement
    {

        public DataConnInfo()
        {
        }

        public DataConnInfo(string strname)
        {
            this.Name = strname;
        }

        public DataConnInfo(string strname, string strtype, string strconn)
        {
            this.Name = strname;
            this.DbType = strtype;
            this.Value = strconn;
        }

        #region ��������

        /// <summary>
        /// ���ݿ���������
        /// </summary>
        [ConfigurationProperty("name", DefaultValue="default", Options = ConfigurationPropertyOptions.None)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// ���ݿ���������
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "SqlClient", Options = ConfigurationPropertyOptions.None)]
        public string DbType
        {
            get { return this["type"] as string; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// ���ݿ������ַ���
        /// </summary>
        [ConfigurationProperty("value", DefaultValue = "server=(local);user id=sa;password=;", IsRequired=true, Options = ConfigurationPropertyOptions.IsRequired)]
        public string Value
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// ��ȡ���ݷ�������
        /// </summary>
        /// <param name="itemname">����Դ����</param>
        /// <returns>����Դ����</returns>
        public DataAccessType DAType
        {
            get
            {
                switch (this.Value)
                {
                    case "OdpNet":
                        return DataAccessType.ODPNet;
                    case "OleDb":
                        return DataAccessType.OLEDB;
                    case "SqlClient":
                        return DataAccessType.SqlClient;
                    default:
                        return DataAccessType.SqlClient;
                }
            }
        }

        #endregion

    }

    /// <summary>
    /// ��������Դ���Ӽ��ϵ����ýڹ���������
    /// </summary>
    [ConfigurationCollection(typeof(DataConnInfo), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class DataConnCollection : ConfigurationElementCollection
    {
        public DataConnCollection()
        {
        }

        #region ʵ��ConfigurationElementCollection�����⺯��

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return
                    ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new DataConnInfo();
        }
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new DataConnInfo(elementName);
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((DataConnInfo)element).Name;
        }

        protected override string ElementName
        {
            get{  return "add"; }
        }
           
        #endregion

        #region ���Ϻ���
        new public DataConnInfo this[string Name]
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;
                return BaseGet(Name) as DataConnInfo;
            }
        }

        public DataConnInfo this[int index]
        {
            get { return this.BaseGet(index) as DataConnInfo; }
            set
            {
                if (null != this.BaseGet(index))
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }


        public void Add(DataConnInfo dc)
        {
            if (null != this[dc.Name])
                this.BaseRemove(dc.Name);
            this.BaseAdd(dc);
        }
        public void Clear()
        {
            this.BaseClear();
        }
        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }
        public void Remove(string strname)
        {
            this.BaseRemove(strname);
        }

        public bool Contains(string strname)
        {
            if (string.IsNullOrEmpty(strname))
                return false;
            if (null == this.BaseGet(strname))
                return false;
            else
                return true;
        }
        public int IndexOf(string strname)
        {
            return this.BaseIndexOf(this.BaseGet(strname));
        }

        #endregion

    }

    /// <summary>
	/// WebConfig�Զ���ڵ�ǿ������
	/// </summary>
    public class DataConnSection : ConfigurationSection
	{

        public DataConnSection()
        {
        }

        [ConfigurationProperty("DataSource",IsDefaultCollection=true)]
        public DataConnCollection DataConnList
        {
            get { return (DataConnCollection)base["DataSource"]; }
            set { base["DataSource"] = value; }
        }

	}
	
	/// <summary>
	/// �ṩ���ݷ�����Դ����
	/// ������Դ�ķ��ʷ�ʽ��ʹ�ù����в��ܸı�
	/// Ĭ��Sql���ʷ�ʽ
    /// Ĭ��ȡ��Ӧ��ͬ������Ŀ¼�����ļ�,��Ŀ¼û�������ļ���ȡϵͳ�����ļ�,
    /// ��Ŀ¼��Ӧ������û�е�ȡӦ��ϵͳ������;
    /// </summary>
	public class DataAccRes
	{
		private DataAccessType	_DbType;	    //���ݷ�������
		private string			_sXmlFile;	    //���ݷ�����Դ�ļ�
		private string			_sConn="";		//���ݿ������ַ���

        static private string          _appName = "";  //Ӧ�ó�������

		#region ���캯��

		/// <summary>
		/// Ĭ�ϵ����ݷ�����Դ
		/// </summary>
		public DataAccRes():this("","")
		{
		}

		/// <summary>
		/// ����Ĭ������Դ����,ָ������Դ��Դ�ļ�
		/// </summary>
		/// <param name="xmlfile">����Դ��Դ�ļ�</param>
		public DataAccRes(string	xmlfile):this("",xmlfile)
		{
		}

		/// <summary>
		/// ָ�����ݿ�ķ��ʷ�ʽ,���ݷ�����Ŀ��xml�ļ�,���ݿ������ַ���
		/// </summary>
		/// <param name="pdbtype">���ݷ�������</param>
		/// <param name="xmlfile">XML������Դ�ļ�</param>
		/// <param name="strConn">���ݿ������ַ���</param>
		public DataAccRes(DataAccessType pdbtype,string xmlfile,string	strconn)
		{
			this._DbType=pdbtype;
			this._sXmlFile=xmlfile;
			this._sConn=strconn;
        }
		
		/// <summary>
		/// ָ������Դ��������,���ݷ�����Դ�ļ�����
		/// </summary>
		/// <param name="itemname">ָ������Դ��������</param>
		/// <param name="xmlfile">XML������Դ�ļ�</param>
		public DataAccRes(string	itemname,string	xmlfile)
		{
            DataConnSection conn = DefaultSection;
            DataConnInfo conninfo = null;
            if (!string.IsNullOrEmpty(itemname))
                conninfo = conn.DataConnList[itemname];
            if (null == conninfo)
                conninfo = DefaultDataConnInfo;
            this._DbType = conninfo.DAType;
            this._sConn = conninfo.Value;
			this._sXmlFile=xmlfile;
		}

		#endregion

        #region �ڲ�����
        /// <summary>
		/// ��ȡ���ݷ�������
		/// </summary>
		public DataAccessType DBAccessType
		{
			get{return this._DbType;}
		}

		/// <summary>
		/// ��ȡ���ݷ�����Դ�ļ�
		/// </summary>
		public string XmlFile
		{
			get{return _sXmlFile;}
		}

        /// <summary>
        /// ��ȡ���ݷ��������ַ���
        /// </summary>
        public string ConnStr
        {
            get
            {
                if ("" != this._sConn)
                    return this._sConn;
                else
                    switch (this.DBAccessType)
                    {
                        case DataAccessType.ODPNet:
                            return AppSettings("ODPNetConnectionStr");
                        case DataAccessType.OLEDB:
                            return AppSettings("OleDBConnectionStr");
                        case DataAccessType.SqlClient:
                            return AppSettings("SQLServerDBConnectionStr");
                        default:
                            throw new ExecutionEngineException("���ݷ�������û��");
                    }
            }
        }

        #endregion

        #region ��̬����
        /// <summary>
        /// ��ȡӦ�ó�������
        /// </summary>
        static public string AppName
        {
            get 
            {
                if ("" == _appName)
                {
                    if (HttpContext.Current != null)
                    {
                        string appname = HttpContext.Current.Request.ApplicationPath;
                        _appName = appname.Substring(appname.LastIndexOf("/") + 1);
                    }
                }
                return _appName; 
            }
        }

        /// <summary>
        /// ��ȡϵͳĬ�������ļ�,�Ự��������-> Ӧ��ͬ������-> Ӧ������
        /// ֧�ֻỰ����,û�лỰ����,�����Ӧ��վ������ѡ��ͬ��������,��Ӧ�����׵�ת�Ƶ�����������
        /// ���϶�û�е�ʹ��ϵͳĬ������
        /// </summary>
        static public Configuration DefaultConfiguration
        {
            get
            {
                //�Ự����ģ��
                Configuration config =null;
                object model = null;
                if (null != HttpContext.Current && null != HttpContext.Current.Session)
                    model = HttpContext.Current.Session["AppModel"];
                if (null != model)
                {
                    config = WebConfigurationManager.OpenWebConfiguration(model.ToString());
                    if (config.HasFile)
                        return config;
                }
                //Ӧ��ͬ������->  Ӧ������-> 
                //û��ͬ�����õ�ѡ��ϵͳ����
                if (null == HttpContext.Current)
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                else
                {
                    config = WebConfigurationManager.OpenWebConfiguration("~/" + DataAccRes.AppName);
                    if (!config.HasFile)
                        config = WebConfigurationManager.OpenWebConfiguration("~");
                }
                KeyValueConfigurationElement configElem = config.AppSettings.Settings["AppModel"];
                if (null != configElem)
                {
                    Configuration config2 = WebConfigurationManager.OpenWebConfiguration(configElem.Value);
                    if (config2.HasFile)
                        config = config2;
                }
                return config;
            }
        }

        /// <summary>
        /// ��ȡϵͳĬ�����ýڵ�
        /// </summary>
        static public DataConnSection DefaultSection
        {
            get
            {
                DataConnSection conn = DefaultConfiguration.GetSection("CustomSection") as DataConnSection;
                if (null == conn && null!=HttpContext.Current)
                    conn = WebConfigurationManager.GetSection("CustomSection") as DataConnSection;
                else if(null==conn)
                    conn = ConfigurationManager.GetSection("CustomSection") as DataConnSection;
                return conn;
            }
        }

        /// <summary>
        /// ϵͳĬ��������������Ϣ;��������ȡ�����ļ�Appsettings��DataConnInfo����,
        /// û�е�ȡӦ�����Ƶ�����,û�е�ȡdefault���Ƶ�����
        /// </summary>
        static public DataConnInfo DefaultDataConnInfo
        {
            get
            {
                DataConnSection conn = DefaultSection;
                DataConnInfo conninfo = conn.DataConnList[AppSettings("DataConnInfo")];
                if (null == conninfo)
                    conninfo = conn.DataConnList[AppName];
                if (null == conninfo)
                    conninfo = conn.DataConnList["default"];
                return conninfo;
            }
        }

        #endregion

        #region ��ȡ������appSettings
        /// <summary>
        /// ��ȡϵͳĬ������:��������Ӧ�ö�Ӧ���ļ����µ������ļ�,���û�оͶ�ȡӦ��Ĭ�ϵ�����
        /// </summary>
        /// <param name="strkey">���ü�</param>
        /// <returns>���ش������õ��ַ���</returns>
        static public string AppSettings(string strkey)
        {
            KeyValueConfigurationElement configElem = null;//DefaultConfiguration.AppSettings.Settings[strkey];
            if (null != configElem)
                return configElem.Value;
            if(null==HttpContext.Current)
                return ConfigurationManager.AppSettings[strkey];
            return WebConfigurationManager.AppSettings[strkey];
        }

        /// <summary>
        /// ����ϵͳĬ������:��������Ӧ�ö�Ӧ���ļ����µ������ļ�,���û�оͶ�ȡӦ��Ĭ�ϵ�����
        /// </summary>
        /// <param name="strkey">���ü�</param>
        /// <returns>���ش������õ��ַ���</returns>
        static public string AppSettings(string strkey,string strvalue)
        {
            KeyValueConfigurationElement configElem = DefaultConfiguration.AppSettings.Settings[strkey];
            if (null != configElem)
                configElem.Value = strvalue;
            else
                DefaultConfiguration.AppSettings.Settings.Add(strkey, strvalue);
            DefaultConfiguration.Save(ConfigurationSaveMode.Modified);
            return strvalue;
        }

        /// <summary>
        /// �л����׵�Ĭ��������;
        /// ��AppModel���Ե�,��������AppModel����ΪAppDemoֵ,������Ч
        /// </summary>
        /// <returns></returns>
        static public bool SwitchDefaultModel()
        {
            Configuration config = DefaultConfiguration;
            KeyValueConfigurationElement configElemDemo = config.AppSettings.Settings["AppDemo"];
            KeyValueConfigurationElement configElemModel=config.AppSettings.Settings["AppModel"];

            if(null!=configElemDemo)
            {
                if (null == configElemModel)
                    config.AppSettings.Settings.Add("AppModel", configElemDemo.Value);
                else
                    configElemModel.Value = configElemDemo.Value;
            }
            config.Save(ConfigurationSaveMode.Modified);
            if (null != HttpContext.Current && null != HttpContext.Current.Session["AppModel"])
                HttpContext.Current.Session.Remove("AppModel");
            return true;
        }

        /// <summary>
        /// �л������ļ�������ѡ��
        /// </summary>
        /// <param name="strpath">���������ļ����ļ���</param>
        /// <returns></returns>
        static public bool SwitchModelConfig(string strpath)
        {
            if (string.IsNullOrEmpty(strpath))
                return false;
            Configuration config = DefaultConfiguration;
            KeyValueConfigurationElement appElem = config.AppSettings.Settings["AppModel"];
            if (null == appElem)
                config.AppSettings.Settings.Add("AppModel", strpath);
            else
                appElem.Value = strpath;
            config.Save(ConfigurationSaveMode.Modified);
            return true;
        }

        #endregion

    }
}
