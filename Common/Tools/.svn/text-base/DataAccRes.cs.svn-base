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
	/// 列举提供的数据访问方式
	/// </summary>
	public enum DataAccessType
	{
		SqlClient,OLEDB,ODPNet
	}

    /// <summary>
    /// 数据库链接定义;type: SqlClient,OLEDB,ODPNet
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

        #region 公共属性

        /// <summary>
        /// 数据库链接名称
        /// </summary>
        [ConfigurationProperty("name", DefaultValue="default", Options = ConfigurationPropertyOptions.None)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 数据库链接类型
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "SqlClient", Options = ConfigurationPropertyOptions.None)]
        public string DbType
        {
            get { return this["type"] as string; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        [ConfigurationProperty("value", DefaultValue = "server=(local);user id=sa;password=;", IsRequired=true, Options = ConfigurationPropertyOptions.IsRequired)]
        public string Value
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// 读取数据访问类型
        /// </summary>
        /// <param name="itemname">数据源名称</param>
        /// <returns>数据源类型</returns>
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
    /// 定义数据源连接集合的配置节关联解析类
    /// </summary>
    [ConfigurationCollection(typeof(DataConnInfo), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class DataConnCollection : ConfigurationElementCollection
    {
        public DataConnCollection()
        {
        }

        #region 实现ConfigurationElementCollection的虚拟函数

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

        #region 集合函数
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
	/// WebConfig自定义节点强类型类
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
	/// 提供数据访问资源类型
	/// 数据资源的访问方式在使用过程中不能改变
	/// 默认Sql访问方式
    /// 默认取与应用同名的子目录配置文件,子目录没有配置文件的取系统配置文件,
    /// 子目录对应配置项没有的取应用系统配置项;
    /// </summary>
	public class DataAccRes
	{
		private DataAccessType	_DbType;	    //数据访问类型
		private string			_sXmlFile;	    //数据访问资源文件
		private string			_sConn="";		//数据库连接字符串

        static private string          _appName = "";  //应用程序名称

		#region 构造函数

		/// <summary>
		/// 默认的数据访问资源
		/// </summary>
		public DataAccRes():this("","")
		{
		}

		/// <summary>
		/// 依据默认数据源连接,指定数据源资源文件
		/// </summary>
		/// <param name="xmlfile">数据源资源文件</param>
		public DataAccRes(string	xmlfile):this("",xmlfile)
		{
		}

		/// <summary>
		/// 指定数据库的访问方式,数据访问项目的xml文件,数据库连接字符串
		/// </summary>
		/// <param name="pdbtype">数据访问类型</param>
		/// <param name="xmlfile">XML数据资源文件</param>
		/// <param name="strConn">数据库连接字符串</param>
		public DataAccRes(DataAccessType pdbtype,string xmlfile,string	strconn)
		{
			this._DbType=pdbtype;
			this._sXmlFile=xmlfile;
			this._sConn=strconn;
        }
		
		/// <summary>
		/// 指定数据源连接名称,数据访问资源文件名称
		/// </summary>
		/// <param name="itemname">指定数据源连接名称</param>
		/// <param name="xmlfile">XML数据资源文件</param>
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

        #region 内部属性
        /// <summary>
		/// 读取数据访问类型
		/// </summary>
		public DataAccessType DBAccessType
		{
			get{return this._DbType;}
		}

		/// <summary>
		/// 读取数据访问资源文件
		/// </summary>
		public string XmlFile
		{
			get{return _sXmlFile;}
		}

        /// <summary>
        /// 读取数据访问连接字符串
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
                            throw new ExecutionEngineException("数据访问类型没有");
                    }
            }
        }

        #endregion

        #region 静态属性
        /// <summary>
        /// 读取应用程序名称
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
        /// 读取系统默认配置文件,会话帐套配置-> 应用同名配置-> 应用帐套
        /// 支持会话帐套,没有会话帐套,则根据应用站点名称选择同名的配置,有应用帐套的转移到帐套配置中
        /// 以上都没有的使用系统默认配置
        /// </summary>
        static public Configuration DefaultConfiguration
        {
            get
            {
                //会话帐套模型
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
                //应用同名配置->  应用帐套-> 
                //没有同名配置的选择系统配置
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
        /// 读取系统默认配置节点
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
        /// 系统默认配置数据链信息;链接名称取配置文件Appsettings的DataConnInfo设置,
        /// 没有的取应用名称的配置,没有的取default名称的配置
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

        #region 读取和配置appSettings
        /// <summary>
        /// 读取系统默认配置:首先找与应用对应的文件夹下的配置文件,如果没有就读取应用默认的配置
        /// </summary>
        /// <param name="strkey">配置键</param>
        /// <returns>返回此项配置的字符串</returns>
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
        /// 设置系统默认配置:首先找与应用对应的文件夹下的配置文件,如果没有就读取应用默认的配置
        /// </summary>
        /// <param name="strkey">配置键</param>
        /// <returns>返回此项配置的字符串</returns>
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
        /// 切换帐套到默认配置项;
        /// 有AppModel属性的,设置它的AppModel属性为AppDemo值,否则无效
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
        /// 切换配置文件的帐套选项
        /// </summary>
        /// <param name="strpath">帐套配置文件的文件夹</param>
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
