using System;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Estar.Common.Tools
{
    #region WorkUnitType 业务单元类型属性;WorkItemType 单项目分类;AppendFunType 附加项功能分类;CalculateType 计算类型;FCalculateType 强制计算类型;SaveType 保存类型
    /// <summary>
	/// 业务功能项的类型
	/// </summary>
	public enum WorkUnitType
	{
        /// <summary>
        /// 最简单的空白页类型
        /// </summary>
        SimpleBank,
        /// <summary>
        /// 简单模板的左工作区类型
        /// </summary>
        SimpleBankLeft,
        /// <summary>
        /// 自定义模板的左工作区类型
        /// </summary>
        HtmlBankLeft,
        /// <summary>
        /// 自定义模板的弹出窗口类型
        /// </summary>
        HtmlBankLeftPop,
        /// <summary>
        /// 报表类型
        /// </summary>
        ReportItem,
        /// <summary>
        /// 含有Vml图表类型的模板窗口
        /// </summary>
        VmlChartTp,
        /// <summary>
        /// 修改密码
        /// </summary>
        Updatekey,
        /// <summary>
        /// 初始预警页面
        /// </summary>
        NavPage,
		/// <summary>
		/// VML地图窗口类型
		/// </summary>
		VmlMapPage,

		/// <summary>
		/// 其他特殊格式类型
		/// </summary>
		OtherItem,
		/// <summary>
		/// 系统菜单定义
		/// </summary>
		SysItem,
		/// <summary>
		/// 权限设置
		/// </summary>
		SysOptRight,
		/// <summary>
		/// 组织机构管理
		/// </summary>
		SysOrganize,
		/// <summary>
		/// 权限设置
		/// </summary>
		SysOptions,
		/// <summary>
		/// 分配操作集
		/// </summary>
		SysAssign,
		/// <summary>
		/// 工作单元中的附加单元
		/// </summary>
		AppendUnit,

		/// <summary>
		/// 初始页
		/// </summary>
		HomePage
		
	}

	/// <summary>
	/// 工作单元的单项目分类
	/// </summary>
	public enum WorkItemType
	{
		/// <summary>
		/// 主数据
		/// </summary>
		MasterData,
		/// <summary>
		/// 从数据或明细数据
		/// </summary>
		DetailData,
		/// <summary>
		/// 常规类型
		/// </summary>
		GeneralData,
		/// <summary>
		/// 报表数据
		/// </summary>
		ReportData,
		/// <summary>
		/// 是否为数据插入项
		/// </summary>
	}

	/// <summary>
	/// 附加项功能分类
	/// </summary>
	public enum AppendFunType
	{
		/// <summary>
		/// 浏览
		/// </summary>
		Browse,
                
        /// <summary>
		/// 导入数据
		/// </summary>
		Import,
		/// <summary>
		/// 输出报表
		/// </summary>
		Report,
		/// <summary>
		/// 直接执行定义的Sql语句
		/// </summary>
		SqlCmd,
        /// <summary>
        /// 列表参数执行定义的Sql语句
        /// </summary>
        SqlCmdList,
		/// <summary>
		/// 从Excel文件导入数据到数据库中
		/// </summary>
		ImportFromExecel,
        /// <summary>
        /// 配置系统
        /// </summary>
        ConfigSystem
	}


	/// <summary>
	/// 列的计算类型
	/// </summary>
	public enum CalculateType
	{
		/// <summary>
		/// 不设置计算
		/// </summary>
		NotSet,
		/// <summary>
		/// 初始化计算
		/// </summary>
		Init,
		/// <summary>
		/// 动态计算
		/// </summary>
		Dynamic
	}

    public enum FCalculateType
    {
        /// <summary>
        /// 不设置计算
        /// </summary>
        NotSet,
        /// <summary>
        /// 初始化计算
        /// </summary>
        Init,
        /// <summary>
        /// 动态计算
        /// </summary>
        Dynamic
    }

	/// <summary>
	/// 保存执行类型
	/// </summary>
	public enum SaveType
	{
		/// <summary>
		/// 通常类保存
		/// </summary>
		GenerallySave,
		/// <summary>
		/// 转换类保存;把一个表数据转换到另一个表中
		/// </summary>
		TransSave,
		/// <summary>
		/// 保存后,页面重新进行数据绑定
		/// </summary>
		SaveAndReload
	}

	/// <summary>
	/// 打印类别
	/// </summary>
	public enum PrintType
	{
		/// <summary>
		/// Word类别
		/// </summary>
		Word,
		/// <summary>
		/// Excel类别
		/// </summary>
		Excel,
		/// <summary>
		/// 网页直接打印
		/// </summary>
		HTML
	}


	#endregion

	#region DictColumn 列类型
	/// <summary>
	/// 每个单项目的列定义
	/// </summary>
	public class DictColumn
	{
		public string	ColumnName="";
        public string   Title = "";
        public string   DataSrc = "";	//字典列数据源
		public string	TextCol="";	//字典列显示名字段
		public string	ValueCol="";	//字段列值列字段
		public string	FilterItem=string.Empty;	//过虑结果对应字段项
		public string	FilterData=string.Empty;	//过虑数据的数据集名称
		public string	ColType="";
		public bool		Visible=true;
        public bool     EVisible = true;    //在编辑板上不可见

		/// <summary>
		/// 是否只读属性
		/// </summary>
		public bool		IsReadOnly=false;
		/// <summary>
		/// 列的显示格式
		/// </summary>
		public string   Formate=string.Empty;
		/// <summary>
		/// 计算列表达式
		/// </summary>
		public string	Expression;
		
		/// <summary>
		/// 脚注,常数，sum,avg,max,min,count
		/// </summary>
		public string	Footer;
		/// <summary>
		/// 计算类型
		/// </summary>
		public CalculateType	CalType=CalculateType.NotSet;
        /// <summary>
        /// 强制计算类型
        /// </summary>
        public FCalculateType FCalType = FCalculateType.NotSet;
        /// <summary>
		/// 是否可空
		/// </summary>
		public bool		IsNeed=true;
		/// <summary>
		/// 验证单元格输入的合法性
		/// </summary>
		public string	ValidateCell="";
		/// <summary>
		/// 是否合并
		/// </summary>
		public bool	MergeCell;

		/// <summary>
		/// 地图该列最大值柱子高度
		/// </summary>
		public int		BarHeight=50;

		/// <summary>
		/// 地图该列的柱子宽度
		/// </summary>
		public int		BarWidth=20;

		/// <summary>
		/// 地图该列柱子的颜色
		/// </summary>
		public string	BarColor="red";
		/// <summary>
		/// 地图该列柱子的提示
		/// </summary>
		public string	BarTitle="";

        /// <summary>
        /// 红字条件表达式
        /// </summary>
        public string RedWord="";
        /// <summary>
        /// 列宽
        /// </summary>
        public int Width = 80;
        /// <summary>
        /// 列宽
        /// </summary>
        public int Height = 0;
        /// 编码规则
        /// </summary>
        public String BHRule = "";
        /// 编码规则清零标志
        /// </summary>
        public String ZeroFlag = "";
    }
	#endregion

	#region Validity 行校验
	/// <summary>
	/// 行校验定义
	/// </summary>
	public class Validity
	{
		public string	Comment="";
		public string	Expression="";
		public string	AlterMsg="";
	}
	#endregion

	#region WorkItem 工作单元内工作项目
	/// <summary>
	/// 工作单元的单个项目
	/// </summary>
	public class WorkItem
	{
        public NameValueCollection AliasList = new NameValueCollection();
		public string	    ItemName=string.Empty;
		public string	    DataSrc=string.Empty;
		public string	    CountDataSrc=string.Empty;
		public string	    DataSrcPage=string.Empty;	//分页数据源,分页控件显示分页数据使用
		public WorkItemType	ItemType=WorkItemType.DetailData;
		public PrintType	PrintType=PrintType.HTML;
		public string	    LinkCol=string.Empty;
		public string	    ColumnKey=string.Empty;
		public string	    TemplateEdit=string.Empty;
		public string	    TemplateHead=string.Empty;
		public string	    HeadHeight=string.Empty;
		public string	    PageSize=string.Empty;
		public string	    Group=string.Empty;
		public string	    BillType=string.Empty;
		//new fixed
		public string	    SumCol=string.Empty;
		public string	    Where=string.Empty;
		public DictColumn[]	DictCol;
		public Validity[]	Validities;

		public string	    printitem="";
		public string	    printname="";
		public string	    printtype="";
        public string       printcountmin = "";
		public bool		    print=false;
		//设置chart的title及数值显示类别(百分号或普通数值)
		public string	    ChartTitleLeft="";
		public string	    ChartTitleBottom="";
		public string	    ChartTitleTop="";
		public string	    ChartValueType="";
		public int		    ExtentX=80;
		public int		    ExtentY=80;
        public string       TempId = string.Empty;
        public Boolean      ManualRefresh = false; //子项目是否手动过滤

        public string       IDField = string.Empty;
        public string       PIDField = string.Empty;
        public string       TxtField = string.Empty;
        public string       NameField = string.Empty;
        public string       SIDField = string.Empty;
        public string       KEYField = string.Empty;
        public string       ValueField = string.Empty;
        public string       TypeField = string.Empty;
        public string       OrderField = string.Empty;
        public string       Ntag = string.Empty;
        public Boolean       NoExpand = false;

		/// <summary>
		/// 地图柱子的间隔
		/// </summary>
		public int		BarStep=8;
		/// <summary>
		/// 地图柱子的x偏移量
		/// </summary>
		public int		BarOffsetX=8;
		/// <summary>
		/// 地图柱子的y偏移量
		/// </summary>
		public int		BarOffsetY=12;
		public bool		IsImport=false;
        public string   InitFilter = ""; //初始过滤字符串
    }
	#endregion

	#region		CommandItem 命令附加项目

	/// <summary>
	/// 命令附加项目
	/// </summary>
	public class CommandItem
	{
		public string	ItemName=string.Empty;
		public string	DataSrc=string.Empty;
		public string	Topic=string.Empty;
		public AppendFunType	FunType=AppendFunType.SqlCmd;
	}

	#endregion

	#region		AppendItem 工作类

	/// <summary>
	/// 命令附加项目
	/// </summary>
	public class AppendItem
	{
		public string	ItemName=string.Empty;
		public string	DataSrc=string.Empty;

		/// <summary>
		/// 项目分组
		/// </summary>
		public string				UnitGroup=string.Empty;
		/// <summary>
		/// 指定该项显示的窗口，1为主窗，0或空为表单窗。若为单窗体，该值不起作用
		/// </summary>
		public	string				ShowPos=string.Empty;

		/// <summary>
		/// 指定调用的UNITITEM，有值时，不考虑DATAITEM
		/// </summary>
		public string				UnitName=string.Empty;


		/// <summary>
		/// 指定插入执行时的ITEM
		/// </summary>
		public	string				CmdItem=string.Empty;

		/// <summary>
		/// 功能类型
		/// </summary>
		public	AppendFunType		FunType=AppendFunType.Browse;

		public  string				PrintItem=string.Empty;
		public  string				PrintTpName=string.Empty;
		public PrintType			PrintType=PrintType.Excel;
        public string               PrintCountMin = "";
		/// <summary>
		/// 弹出窗口对话框宽度
		/// </summary>
		public	string				DialogWidth="800px";

		/// <summary>
		/// 弹出窗口对话框高度
		/// </summary>
		public	string				DialogHeight="500px";

        /// <summary>
        /// 当模板类型为HTML时的URL
        /// </summary>
        public string HTMLURL = "";
        
	}
	#endregion

	#region UnitItem 工作单元类
	/// <summary>
	/// 工作单元的实例
	/// </summary>
	public class UnitItem
	{
		private	string				_unitFile=string.Empty;
		private	XmlDocument			_xmldoc=null;
		private XmlNode				_xmlnode=null;				//当前与_sworkUnit一致的节点
        private DataRow _drnode;
        private DataTable dtWorkUnit    = new DataTable();
        private DataTable dtWorkItem    = new DataTable();
        private DataTable dtAppendItem  = new DataTable();
        private DataTable dtComItem     = new DataTable();
        private DataTable dtColumn      = new DataTable();
        private SqlConnection myConnection=new SqlConnection();
		/// <summary>
		/// 单元名称
		/// </summary>
		private	string				_unitName=string.Empty;
		/// <summary>
		/// 单元内项目列表
		/// </summary>
		public	WorkItem[]			WorkItemList=new WorkItem[0];
		/// <summary>
		/// 命令附加项目
		/// </summary>
		public CommandItem[]		CommandItemList=new CommandItem[0];

		/// <summary>
		/// 附加项目名称列表
		/// </summary>
		public	AppendItem[]		AppendItemList=new AppendItem[0];

		/// <summary>
		/// 单元类型
		/// </summary>
		public	WorkUnitType		UnitType;
        /// <summary>
        /// 是否在左导航栏可见,默认可现
        /// </summary>
        public bool IsVisibleNav = true;
		/// <summary>
		/// 数据源文件,虚拟站点路径
		/// </summary>
		private	string				_dataSrcFile=string.Empty;
		/// <summary>
		/// 页面模板文件,虚拟站点路径
		/// </summary>
		public	string				FileEditTp=string.Empty;

        /// <summary>
        /// 打印模板文件,虚拟站点路径
        /// </summary>
        public string               FilePrnTp = string.Empty;

		/// <summary>
		/// 单据类型简称
		/// </summary>
		public	string				BillType=string.Empty;
		/// <summary>
		/// 保存方式
		/// </summary>
		public	SaveType			SaveOPType=SaveType.GenerallySave;
		/// <summary>
		/// 打印类别
		/// </summary>
		public  PrintType			PrintType=PrintType.HTML;
		/// <summary>
		/// 字典数据源文件,虚拟站点路径
		/// </summary>
		private	string				_dictColSrcFile=string.Empty;
		/// <summary>
		/// 快捷过虑
		/// </summary>
		public	ShortcutFilter		StFilter=null;

		/// <summary>
		/// 树形过滤
		/// </summary>
		public	TreeFilter		treeFilter=null;

        /// <summary>
        /// 缺省工作流
        /// </summary>
        public string WorkFlow = string.Empty;

		#region  公共属性
			/// <summary>
			/// 工作单元对应的xmlNode节点配置
			/// </summary>
            public XmlNode UnitNode
            {
                get { return this._xmlnode; }
            }

            public DataTable DtUnit
            {
                get { return this.dtWorkUnit; }
            }

            public DataTable DtWorkItem
            {
                get { return this.dtWorkItem; }
            }

            public DataTable DtAppendItem
            {
                get { return this.dtAppendItem; }
            }

            public DataTable DtComItem
            {
                get { return this.dtComItem; }
            }

            public DataTable DtColumn
            {
                get { return this.dtColumn; }
            }

            /// <summary>
			/// 读取或设置单元名称
			/// </summary>
			public	string				UnitName
			{
				get{return this._unitName;}
				set
				{
					this._unitName=value;
                    this._drnode["name"]=value;
				}
			}
			/// <summary>
			/// 读取或设置数据源文件
			/// </summary>
			public	string				DataSrcFile
			{
				get{return	this._dataSrcFile;}
				set
				{
					this._dataSrcFile=value;
                    if (this._drnode == null) return;
                    this._drnode["datasrcfile"] = value;
				}
			}
			/// <summary>
			/// 读取或设置字典数据源文件,虚拟站点路径
			/// </summary>
			public	string				DictColSrcFile
			{
				get{return	this._dictColSrcFile;}
				set
				{
					this._dictColSrcFile=value;
					if(null==this._drnode["dictcolfile"])
                        this._drnode["dictcolfile"] = value;
				}
			}
		#endregion

        private void OpenDatabase()
        {
            string strConn = DataAccRes.DefaultDataConnInfo.Value;
            myConnection = new SqlConnection(strConn);
            myConnection.Open();
        }

        public DataTable BindGrid(string strsql)
        {
            SqlDataAdapter da = new SqlDataAdapter(strsql, myConnection);
            if (da == null) return null;
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                return dt;
            }
            catch
            {
                //leofun.Alert("生成失败，请检查！", this);
                return null;
            }

        }

        private DataTable bindsubTable(string sqltext,string linkname)
        {
            string itemcode = "";
            DataRow[] drs = this.dtWorkUnit.Select("ntype='" + linkname + "'");
            for (int i = 0; i < drs.Length; i++)
                itemcode = itemcode + "'" + drs[i]["ID"] + "',";
            if (drs.Length > 0)
            {
                itemcode = itemcode.Substring(0, itemcode.Length - 1);
                return BindGrid(sqltext.Replace("@itemcode",itemcode));
            }
            else
                return null;
        }

        private void CreateAppTable(string workUnitFile, string workUnitName)
        {
            OpenDatabase();
            string systemdb = DataAccRes.AppSettings("SystemDB");
            if (systemdb == "" || systemdb == null) systemdb = "hmsys";
            string sqltext = "execute [" + systemdb + "].[dbo].[proc_sys_workitem] '" + workUnitFile + "','" + workUnitName + "'";
            this.dtWorkUnit = BindGrid(sqltext);
            if (this.dtWorkUnit == null) return;
            sqltext = "SELECT * FROM [" + systemdb + "].[dbo].[fn_sys_items] () WHERE PID in(@itemcode) order by serial";
            this.dtWorkItem     = this.bindsubTable(sqltext,"Item");
            sqltext = "SELECT * FROM [" + systemdb + "].[dbo].[fn_sys_appenditems] () WHERE PID in(@itemcode)";
            this.dtAppendItem   = this.bindsubTable(sqltext, "AppendItem");

            sqltext = "SELECT * FROM [" + systemdb + "].[dbo].[fn_sys_commanditems] () WHERE PID in(@itemcode)";
            this.dtComItem = this.bindsubTable(sqltext, "CommandItem");
            if(myConnection.State== ConnectionState.Open)
                myConnection.Close();
        }

        // 通过workUnitName得到其中的数据源dataitem,再通过dataitem查到对应的sqltext,最后得到table
        // workUnitFile原用于打开指定操作集的文件名，即workitem.xml，目前workitem.xml由hmsys数据库替代，因此，可以不再考虑
        // 此时，workUnitName为单元名称，只需在hmsys的unititem中找到workUnitName集合即可,然后再取出workUnitName单元的所有属性
		public UnitItem(string	workUnitFile,string	workUnitName)
		{
            if (workUnitName == "") return;
            this._unitFile = workUnitFile;
            CreateAppTable(workUnitFile,workUnitName);
            if (this.dtWorkUnit == null) return;

            DataRow[] _drnodes = this.dtWorkUnit.Select("ntype='UnitItem'");
             if (_drnodes.Length == 0) return;
             this._drnode = _drnodes[0];
			this.SetWorkUnit(workUnitName);

			this.WorkItemList		=	this.GetWorkItemList();
			this.CommandItemList	=	this.GetCommandItemList();
			this.AppendItemList		=	this.GetAppendItemList();
			this.UnitType			=	this.GetWorkUnitType();
			this.DataSrcFile		=	this.GetDataSrcFile();
			this.FileEditTp			=	this.GetFileEditTp();
            this.WorkFlow           =   this.GetWorkFlow();
            
            this.FilePrnTp          =   this.GetFilePrintTp();
			this.SaveOPType			=	this.GetSaveType();
			this.DictColSrcFile		=	this.GetDictColSrcFile();
			this.StFilter			=	this.GetShortcutFilter();
			this.treeFilter			=	this.GetTreeFilter();
			
			this.BillType			=	this.GetBillType();
            this.PrintType          =   this.GetPrnType();
            this.IsVisibleNav       =   this.GetVisibleNav();
		}

        // 通过传入的xmldoc获得相关unit参数
        public UnitItem(NameObjectList paramlist, string workUnitName)
        {
              
            if (workUnitName == "") return;
            this.UnitName = workUnitName;
            //this.WorkItemList = null;
            //this.CommandItemList = null;
            //this.AppendItemList = null;
            //this.UnitType = null;
            //this.DataSrcFile = null;
            //this.FileEditTp = null;
            //this.WorkFlow = null;

            //this.FilePrnTp = null;
            //this.SaveOPType = null;
            //this.DictColSrcFile = null;
            //this.StFilter = null;
            //this.treeFilter = null;

            //this.BillType = null;
            //this.PrintType = null;
            //this.IsVisibleNav = null;
        }

		public UnitItem()
		{
			this._unitFile	=	"";
			this._xmldoc	=	new XmlDocument();
			this._xmldoc.LoadXml("<BusinessSource name='hmsys'><UnitItem name='' templatetype='SPT' datasrcfile='' dictcolfile='' gridtemplate='' billtype='' savetype='' workflow=''/></BusinessSource>");
			this._xmlnode=this._xmldoc.DocumentElement.SelectSingleNode("UnitItem");
		}

		/// <summary>
		/// 设置当前的工作单元
		/// </summary>
		/// <param name="workUnit">单元名称</param>
		private	void SetWorkUnit(string	workUnit)
		{
            string strXml = "<UnitItem></UnitItem>";
            this._xmldoc = new XmlDocument();
            this._xmldoc.LoadXml(strXml);
            if (null == dtWorkUnit)
                throw (new Exception("系统资源没有提供该功能单元：" + workUnit));
            CreateXmlNode();
            strXml = "<BusinessSource name='hmsys'>" + this._xmldoc.InnerXml + "</BusinessSource>";
            this._xmldoc.LoadXml(strXml);
            this._xmlnode = this._xmldoc.DocumentElement.SelectSingleNode("UnitItem");
            this.UnitName=workUnit;
		}

        private void CreateXmlNode()
        {
            DataRow[] drUnit= this.dtWorkUnit.Select("ntype='UnitItem'");
            for (int i = 0; i < drUnit[0].Table.Columns.Count; i++)
            {
                if (drUnit[0].Table.Columns[i].ColumnName == "ID" || drUnit[0].Table.Columns[i].ColumnName == "text"
                     || drUnit[0].Table.Columns[i].ColumnName == "PID" || drUnit[0].Table.Columns[i].ColumnName == "ntype")
                    continue;

                if ((drUnit[0][i].ToString() == "") ||
                    (drUnit[0].Table.Columns[i].DataType.FullName == "System.Boolean" && drUnit[0][i].ToString() == "False"))
                        continue;
                this._xmldoc.DocumentElement.Attributes.Append(this._xmldoc.CreateAttribute(drUnit[0].Table.Columns[i].ColumnName));
                this._xmldoc.DocumentElement.Attributes[drUnit[0].Table.Columns[i].ColumnName].Value = drUnit[0][i].ToString();
            }

            XmlNode root = this._xmldoc.DocumentElement;
            DataRow[] drItem;
            if (this.dtWorkItem != null)
            {
                drItem = this.dtWorkItem.Select();
                for (int i = 0; i < drItem.Length; i++)
                {
                    XmlElement elem = this._xmldoc.CreateElement("Item");
                    root.AppendChild(elem);
                    for (int j = 0; j < drItem[i].Table.Columns.Count; j++)
                    {
                        if (drItem[i].Table.Columns[j].ColumnName == "ID" || drItem[i].Table.Columns[j].ColumnName == "text"
                             || drItem[i].Table.Columns[j].ColumnName == "PID" || drItem[i].Table.Columns[j].ColumnName == "ntype")
                            continue;

                        if ((drItem[i][j].ToString() == "") ||
                            (drItem[i].Table.Columns[j].DataType.FullName == "System.Boolean" && drItem[i][j].ToString() == "False"))
                            continue;
                        elem.Attributes.Append(this._xmldoc.CreateAttribute(drItem[i].Table.Columns[j].ColumnName));
                        elem.Attributes[drItem[i].Table.Columns[j].ColumnName].Value = drItem[i][j].ToString();
                    }
                    //增加columns节点及属性
                    if (myConnection.State == ConnectionState.Closed)
                        myConnection.Open();
                    string systemdb = DataAccRes.AppSettings("SystemDB");
                    if (systemdb == "" || systemdb == null) systemdb = "hmsys";
                    string sqltext = "execute "+systemdb+".[dbo].[proc_sys_columns] '" + drItem[i]["pid"].ToString() + "'";

                    this.dtColumn = BindGrid(sqltext);
                    DataRow[] drcols=this.dtColumn.Select();
                    for (int k = 0; k < drcols.Length; k++)
                    {
                        elem = this._xmldoc.CreateElement("Column");
                        for (int n = 0; n < drcols[k].Table.Columns.Count; n++)
                        {
                            if (drcols[k].Table.Columns[n].ColumnName == "ID" || drcols[k].Table.Columns[n].ColumnName == "text"
                                 || drcols[k].Table.Columns[n].ColumnName == "PID" || drcols[k].Table.Columns[n].ColumnName == "ntype")
                                continue;
                            if ((drcols[k][n].ToString() == "") ||
                                (drcols[k].Table.Columns[n].DataType.FullName == "System.Boolean" && drcols[k][n].ToString()=="False"))
                                continue;
                            XmlNode itemnode = root.SelectSingleNode("Item[@name='" + drItem[i]["name"] + "']");
                            elem.Attributes.Append(this._xmldoc.CreateAttribute(drcols[k].Table.Columns[n].ColumnName));
                            string cVal = drcols[k][n].ToString();
                            if (drcols[k].Table.Columns[n].DataType.FullName == "System.Boolean" && drcols[k][n].ToString() == "True")
                                cVal = "1";
                            elem.Attributes[drcols[k].Table.Columns[n].ColumnName].Value = cVal;
                            itemnode.AppendChild(elem);
                        }
                    }
                }
            }

            if (this.dtAppendItem != null)
            {
                drItem = this.dtAppendItem.Select();
                for (int i = 0; i < drItem.Length; i++)
                {
                    XmlElement elem = this._xmldoc.CreateElement("AppendItem");
                    root.AppendChild(elem);
                    for (int j = 0; j < drItem[i].Table.Columns.Count; j++)
                    {
                        if (drItem[i].Table.Columns[j].ColumnName == "ID" || drItem[i].Table.Columns[j].ColumnName == "text"
                             || drItem[i].Table.Columns[j].ColumnName == "PID" || drItem[i].Table.Columns[j].ColumnName == "ntype")
                            continue;

                        if ((drItem[i][j].ToString() == "") ||
                            (drItem[i].Table.Columns[j].DataType.FullName == "System.Boolean" && drItem[i][j].ToString() == "False"))
                            continue;
                        elem.Attributes.Append(this._xmldoc.CreateAttribute(drItem[i].Table.Columns[j].ColumnName));
                        elem.Attributes[drItem[i].Table.Columns[j].ColumnName].Value = drItem[i][j].ToString();
                    }
                }
            }

            if (this.dtComItem != null)
            {
                drItem = this.dtComItem.Select();
                for (int i = 0; i < drItem.Length; i++)
                {
                    XmlElement elem = this._xmldoc.CreateElement("CommandItem");
                    root.AppendChild(elem);
                    for (int j = 0; j < drItem[i].Table.Columns.Count; j++)
                    {
                        if (drItem[i].Table.Columns[j].ColumnName == "ID" || drItem[i].Table.Columns[j].ColumnName == "text"
                             || drItem[i].Table.Columns[j].ColumnName == "PID" || drItem[i].Table.Columns[j].ColumnName == "ntype")
                            continue;

                        if ((drItem[i][j].ToString() == "") ||
                            (drItem[i].Table.Columns[j].DataType.FullName == "System.Boolean" && drItem[i][j].ToString() == "False"))
                            continue;
                        elem.Attributes.Append(this._xmldoc.CreateAttribute(drItem[i].Table.Columns[j].ColumnName));
                        elem.Attributes[drItem[i].Table.Columns[j].ColumnName].Value = drItem[i][j].ToString();
                    }
                }
            }
        }


		#region 内部属性函数

		/// <summary>
		/// 获取业务单元的明细项目
		/// </summary>
		/// <returns>返回明细项目数组</returns>
        private WorkItem[] GetWorkItemList()
        {
            if (null == this._xmlnode) return (new WorkItem[0]);
            XmlNodeList itemlist = this._xmlnode.SelectNodes("Item");
            WorkItem[] workItemList = new WorkItem[itemlist.Count];

            for (int i = 0; i < itemlist.Count; i++)
            {

                WorkItem item = new WorkItem();
                item.ItemName = itemlist[i].Attributes["name"].Value;
                workItemList[i] = item;
                if (null != itemlist[i].Attributes["alias"] && "" != itemlist[i].Attributes["alias"].Value)
                {
                    string[] aliasList = itemlist[i].Attributes["alias"].Value.Split(";".ToCharArray());
                    for (int ialias = 0; ialias < aliasList.Length; ialias++)
                        if ("" != aliasList[ialias] && null != aliasList[ialias])
                            item.AliasList[aliasList[ialias]] = aliasList[ialias];
                }

                #region 关系类型

                string strRlt = "";
                if (null == itemlist[i].Attributes["relation"] || "" == itemlist[i].Attributes["relation"].Value)
                    strRlt = "G";
                else
                    strRlt = itemlist[i].Attributes["relation"].Value.ToUpper();
                switch (strRlt)
                {
                    case "M":
                        item.ItemType = WorkItemType.MasterData;
                        if (null != this._xmlnode.Attributes["billtype"])
                            item.BillType = this._xmlnode.Attributes["billtype"].Value;
                        break;
                    case "D":
                        item.ItemType = WorkItemType.DetailData;
                        break;
                    case "G":
                        item.ItemType = WorkItemType.GeneralData;
                        break;
                    default:
                        throw (new Exception("明细项目类型设置有误：" + this.UnitName));
                }

                #endregion

                #region 打印类型

                string strPrintType = "";
                if (null != itemlist[i].Attributes["printtype"])
                    strPrintType = itemlist[i].Attributes["printtype"].Value.ToUpper();
                switch (strPrintType)
                {
                    case "WORD":
                        item.PrintType = PrintType.Word;
                        break;
                    case "EXCEL":
                        item.PrintType = PrintType.Excel;
                        break;
                    case "HTML":
                        item.PrintType = PrintType.HTML;
                        break;
                    default:
                        item.PrintType = PrintType.HTML;
                        break;
                }
                if (null != itemlist[i].Attributes["printitem"])
                    item.printitem = itemlist[i].Attributes["printitem"].Value;
                if (null != itemlist[i].Attributes["printname"])
                    item.printname = itemlist[i].Attributes["printname"].Value;
                if (null != itemlist[i].Attributes["printcount"] && null != itemlist[i].Attributes["printcount"].Value)
                    item.printcountmin = itemlist[i].Attributes["printcount"].Value;
                if (itemlist[i].Attributes["print"] != null && itemlist[i].Attributes["print"].Value.ToLower() == "true")
                    item.print = true;
                else
                    item.print = false;
                #endregion

                #region Chart图表类型

                string strCharType = "";
                if (null != itemlist[i].Attributes["charttype"])
                    strCharType = itemlist[i].Attributes["charttype"].Value.ToLower();
                if (itemlist[i].Attributes["chartleft"] != null)
                    item.ChartTitleLeft = itemlist[i].Attributes["chartleft"].Value;
                if (itemlist[i].Attributes["chartbottom"] != null)
                    item.ChartTitleBottom = itemlist[i].Attributes["chartbottom"].Value;
                if (itemlist[i].Attributes["charttop"] != null)
                    item.ChartTitleTop = itemlist[i].Attributes["charttop"].Value;
                if (itemlist[i].Attributes["chartvaluetype"] != null)
                    item.ChartValueType = itemlist[i].Attributes["chartvaluetype"].Value;

                #endregion

                if (null != itemlist[i].Attributes["extentx"] && null != itemlist[i].Attributes["extentx"].Value
                    && "" != itemlist[i].Attributes["extentx"].Value)
                    try
                    {
                        item.ExtentX = int.Parse(itemlist[i].Attributes["extentx"].Value);
                    }
                    catch { }
                if (null != itemlist[i].Attributes["extenty"] && null != itemlist[i].Attributes["extenty"].Value
                    && "" != itemlist[i].Attributes["extenty"].Value)
                    try
                    {
                        item.ExtentY = int.Parse(itemlist[i].Attributes["extenty"].Value);
                    }
                    catch { }

                #region 工作项目数据源属性
                if (null != itemlist[i].Attributes["dataitem"])
                    item.DataSrc = itemlist[i].Attributes["dataitem"].Value;
                if (itemlist[i].Attributes["dataitempage"] != null)
                    item.DataSrcPage = itemlist[i].Attributes["dataitempage"].Value;
                if (itemlist[i].Attributes["countdataitem"] != null)
                    item.CountDataSrc = itemlist[i].Attributes["countdataitem"].Value;
                if (null != itemlist[i].Attributes["linkcol"])
                    item.LinkCol = itemlist[i].Attributes["linkcol"].Value;
                if (null != itemlist[i].Attributes["columnkey"])
                    item.ColumnKey = itemlist[i].Attributes["columnkey"].Value;
                if (null != itemlist[i].Attributes["gridtemplate"])
                    item.TemplateEdit = itemlist[i].Attributes["gridtemplate"].Value;
                if (null != itemlist[i].Attributes["headtemplate"])
                    item.TemplateHead = itemlist[i].Attributes["headtemplate"].Value;
                if (null != itemlist[i].Attributes["headheight"])
                    item.HeadHeight = itemlist[i].Attributes["headheight"].Value;
                if (null != itemlist[i].Attributes["pagesize"])
                    item.PageSize = itemlist[i].Attributes["pagesize"].Value;
                if (null != itemlist[i].Attributes["group"])
                    item.Group = itemlist[i].Attributes["group"].Value;
                if (null != itemlist[i].Attributes["sumcol"])
                    item.SumCol = itemlist[i].Attributes["sumcol"].Value;
                if (null != itemlist[i].Attributes["where"])
                    item.Where = itemlist[i].Attributes["where"].Value;
                if (null != itemlist[i].Attributes["filter"])
                    item.InitFilter = itemlist[i].Attributes["filter"].Value;

                if (null != itemlist[i].Attributes["idfld"])
                    item.IDField = itemlist[i].Attributes["idfld"].Value;

                if (null != itemlist[i].Attributes["pidfld"])
                    item.PIDField = itemlist[i].Attributes["pidfld"].Value;

                if (null != itemlist[i].Attributes["txtfld"])
                    item.TxtField = itemlist[i].Attributes["txtfld"].Value;

                if (null != itemlist[i].Attributes["namefld"])
                    item.NameField = itemlist[i].Attributes["namefld"].Value;

                if (null != itemlist[i].Attributes["selfid"])
                    item.SIDField = itemlist[i].Attributes["selfid"].Value;

                if (null != itemlist[i].Attributes["keyfid"])
                    item.KEYField = itemlist[i].Attributes["keyfid"].Value;

                if (null != itemlist[i].Attributes["valuefld"])
                    item.ValueField = itemlist[i].Attributes["valuefld"].Value;

                if (null != itemlist[i].Attributes["typefld"])
                    item.TypeField = itemlist[i].Attributes["typefld"].Value;

                if (null != itemlist[i].Attributes["orderfld"])
                    item.OrderField = itemlist[i].Attributes["orderfld"].Value;

                if (null != itemlist[i].Attributes["ntag"])
                    item.Ntag = itemlist[i].Attributes["ntag"].Value;

                if (itemlist[i].Attributes["noexpand"] != null && itemlist[i].Attributes["noexpand"].Value.ToLower() == "true")
                    item.NoExpand = true;

                if (itemlist[i].Attributes["import"] != null && itemlist[i].Attributes["import"].Value == "1")
                    item.IsImport = true;


                if (itemlist[i].Attributes["manualrefresh"] != null && itemlist[i].Attributes["manualrefresh"].Value.ToLower() == "true")
                    item.ManualRefresh = true;
                
                if (null != itemlist[i].Attributes["tpid"])
                    item.TempId = itemlist[i].Attributes["tpid"].Value;

                #endregion

                #region 地图数据柱属性
                if (null != itemlist[i].Attributes["barstep"] && "" != itemlist[i].Attributes["barstep"].Value)
                    try
                    {
                        item.BarStep = int.Parse(itemlist[i].Attributes["barstep"].Value);
                    }
                    catch { }
                if (null != itemlist[i].Attributes["baroffsetx"] && "" != itemlist[i].Attributes["baroffsetx"].Value)
                    try
                    {
                        item.BarOffsetX = int.Parse(itemlist[i].Attributes["baroffsetx"].Value);
                    }
                    catch { }
                if (null != itemlist[i].Attributes["baroffsety"] && "" != itemlist[i].Attributes["baroffsety"].Value)
                    try
                    {
                        item.BarOffsetY = int.Parse(itemlist[i].Attributes["baroffsety"].Value);
                    }
                    catch { }
                #endregion

                XmlNodeList colnodeList = itemlist[i].SelectNodes("Column");
                item.DictCol = new DictColumn[colnodeList.Count];
                for (int j = 0; j < colnodeList.Count; j++)
                {
                    item.DictCol[j] = new DictColumn();

                    #region 字典列数据源

                    item.DictCol[j].ColumnName = colnodeList[j].Attributes["name"].Value;
                    item.DictCol[j].Title = item.DictCol[j].ColumnName;
                    if (null != colnodeList[j].Attributes["dataitem"] && null != colnodeList[j].Attributes["textcol"]
                            && null != colnodeList[j].Attributes["valuecol"])
                    {
                        item.DictCol[j].DataSrc = colnodeList[j].Attributes["dataitem"].Value;
                        item.DictCol[j].TextCol = colnodeList[j].Attributes["textcol"].Value;
                        item.DictCol[j].ValueCol = colnodeList[j].Attributes["valuecol"].Value;
                    }
                    if (null != colnodeList[j].Attributes["filteritem"] && null != colnodeList[j].Attributes["filterdata"])
                        item.DictCol[j].FilterItem = colnodeList[j].Attributes["filteritem"].Value;
                    if (null != colnodeList[j].Attributes["filterdata"])
                        item.DictCol[j].FilterData = colnodeList[j].Attributes["filterdata"].Value;
                    if (null != colnodeList[j].Attributes["title"] && !string.IsNullOrEmpty(colnodeList[j].Attributes["title"].Value))
                        item.DictCol[j].Title = colnodeList[j].Attributes["title"].Value;
                    #endregion

                    #region 列计算,校验,编辑,格式,可视属性
                    //计算列,列是否必填,列的单元格验证
                    if (null != colnodeList[j].Attributes["expression"])
                        item.DictCol[j].Expression = colnodeList[j].Attributes["expression"].Value;

                    if (null != colnodeList[j].Attributes["redword"])
                        item.DictCol[j].RedWord = colnodeList[j].Attributes["redword"].Value;

                    if (null != colnodeList[j].Attributes["bhrule"])
                        item.DictCol[j].BHRule = colnodeList[j].Attributes["bhrule"].Value;
                    if (null != colnodeList[j].Attributes["zeroflag"])
                        item.DictCol[j].ZeroFlag = colnodeList[j].Attributes["zeroflag"].Value;

                    if (null != colnodeList[j].Attributes["validity"])
                        item.DictCol[j].ValidateCell = colnodeList[j].Attributes["validity"].Value;

                    if (null != colnodeList[j].Attributes["calcol"] && "1" == colnodeList[j].Attributes["calcol"].Value)
                        item.DictCol[j].CalType = CalculateType.Dynamic;
                    else
                        item.DictCol[j].CalType = CalculateType.Init;

                    if (null != colnodeList[j].Attributes["fcalcol"] && "1" == colnodeList[j].Attributes["fcalcol"].Value)
                        item.DictCol[j].FCalType = FCalculateType.Dynamic;
                    else
                        item.DictCol[j].FCalType = FCalculateType.Init;

                    if (null != colnodeList[j].Attributes["width"])
                        item.DictCol[j].Width = int.Parse(colnodeList[j].Attributes["width"].Value);

                    if (null != colnodeList[j].Attributes["height"])
                        item.DictCol[j].Height = int.Parse(colnodeList[j].Attributes["height"].Value);
                    //脚注
                    if (null != colnodeList[j].Attributes["footer"])
                        item.DictCol[j].Footer = colnodeList[j].Attributes["footer"].Value;
                    // 0 - 非限制输入, 1- 表示不能为空
                    if (null != colnodeList[j].Attributes["chkcol"] && "1" == colnodeList[j].Attributes["chkcol"].Value)
                        item.DictCol[j].IsNeed = true;
                    else
                        item.DictCol[j].IsNeed = false;

                    if (null != colnodeList[j].Attributes["merge"] && "1" == colnodeList[j].Attributes["merge"].Value)
                        item.DictCol[j].MergeCell = true;
                    else
                        item.DictCol[j].MergeCell = false;

                    if (null != colnodeList[j].Attributes["isreadonly"] && "1" == colnodeList[j].Attributes["isreadonly"].Value)
                        item.DictCol[j].IsReadOnly = true;
                    else
                        item.DictCol[j].IsReadOnly = false;
                    if (null != colnodeList[j].Attributes["format"] && string.Empty != colnodeList[j].Attributes["format"].Value &&
                        "" != colnodeList[j].Attributes["format"].Value.ToLower())
                        item.DictCol[j].Formate = colnodeList[j].Attributes["format"].Value;

                    //1 - 不可见，0，空为正常显示 
                    if (null != colnodeList[j].Attributes["visible"] && "1" == colnodeList[j].Attributes["visible"].Value)
                        item.DictCol[j].Visible = false;
                    else
                        item.DictCol[j].Visible = true;

                    //1 - 不可见，0，空为正常显示 
                    if (null != colnodeList[j].Attributes["evisible"] && "1" == colnodeList[j].Attributes["evisible"].Value)
                        item.DictCol[j].EVisible = false;
                    else
                        item.DictCol[j].EVisible = true;

                #endregion

                    #region 地图属性
                    if (null != colnodeList[j].Attributes["barheight"] && "" != colnodeList[j].Attributes["barheight"].Value)
                        try
                        {
                            item.DictCol[j].BarHeight = int.Parse(colnodeList[i].Attributes["barheight"].Value);
                        }
                        catch { }
                    if (null != colnodeList[j].Attributes["barwidth"] && "" != colnodeList[j].Attributes["barwidth"].Value)
                        try
                        {
                            item.DictCol[j].BarWidth = int.Parse(colnodeList[i].Attributes["barwidth"].Value);
                        }
                        catch { }
                    if (null != colnodeList[j].Attributes["barcolor"] && "" != colnodeList[j].Attributes["barcolor"].Value)
                        try
                        {
                            item.DictCol[j].BarColor = colnodeList[i].Attributes["barcolor"].Value;
                        }
                        catch { }
                    if (null != colnodeList[j].Attributes["bartitle"] && "" != colnodeList[j].Attributes["bartitle"].Value)
                        try
                        {
                            item.DictCol[j].BarTitle = colnodeList[i].Attributes["bartitle"].Value;
                        }
                        catch { }
                    #endregion

                }

                //行数据校验
                XmlNodeList valiNodeList = itemlist[i].SelectNodes("Validity");
                item.Validities = new Validity[valiNodeList.Count];
                for (int j = 0; j < valiNodeList.Count; j++)
                {
                    item.Validities[j] = new Validity();
                    if (null != valiNodeList[j].Attributes["comment"])
                        item.Validities[j].Comment = valiNodeList[j].Attributes["comment"].Value;
                    if (null != valiNodeList[j].Attributes["expression"])
                        item.Validities[j].Expression = valiNodeList[j].Attributes["expression"].Value;
                    if (null != valiNodeList[j].Attributes["alertmsg"])
                        item.Validities[j].AlterMsg = valiNodeList[j].Attributes["alertmsg"].Value;
                }
            }
            return workItemList;
        }


		/// <summary>
		/// 获取业务单元的附加子功能单元
		/// </summary>
		/// <returns>返回明细项目数组</returns>
        private AppendItem[] GetAppendItemList()
        {
            if (null == this._xmlnode) return new AppendItem[0];
            XmlNodeList itemlist = this._xmlnode.SelectNodes("AppendItem");
            AppendItem[] appendItemList = new AppendItem[itemlist.Count];

            for (int i = 0; i < itemlist.Count; i++)
            {
                appendItemList[i] = new AppendItem();
                appendItemList[i].ItemName = itemlist[i].Attributes["name"].Value;

                if (null != itemlist[i].Attributes["dataitem"])
                    appendItemList[i].DataSrc = itemlist[i].Attributes["dataitem"].Value;

                if (null != itemlist[i].Attributes["cmditem"] && null != itemlist[i].Attributes["cmditem"].Value)
                    appendItemList[i].CmdItem = itemlist[i].Attributes["cmditem"].Value;

                if (null == itemlist[i].Attributes["funtype"] || "" == itemlist[i].Attributes["funtype"].Value
                    || "browse" == itemlist[i].Attributes["funtype"].Value.ToLower())
                    appendItemList[i].FunType = AppendFunType.Browse;
                else if ("import" == itemlist[i].Attributes["funtype"].Value.ToLower()
                    || "checkin" == itemlist[i].Attributes["funtype"].Value.ToLower()
                    || "importpost" == itemlist[i].Attributes["funtype"].Value.ToLower())
                    appendItemList[i].FunType = AppendFunType.Import;
                else if ("report" == itemlist[i].Attributes["funtype"].Value.ToLower())
                    appendItemList[i].FunType = AppendFunType.Report;
                else
                    appendItemList[i].FunType = AppendFunType.Browse;

                if (null != itemlist[i].Attributes["unitgroup"] && null != itemlist[i].Attributes["unitgroup"].Value)
                    appendItemList[i].UnitGroup = itemlist[i].Attributes["unitgroup"].Value;

                if (null != itemlist[i].Attributes["unitname"] && null != itemlist[i].Attributes["unitname"].Value)
                    appendItemList[i].UnitName = itemlist[i].Attributes["unitname"].Value;

                if (null != itemlist[i].Attributes["showpos"])
                    appendItemList[i].ShowPos = itemlist[i].Attributes["showpos"].Value;

                if (null != itemlist[i].Attributes["dialogheight"] && null != itemlist[i].Attributes["dialogheight"].Value)
                {
                    string dh = itemlist[i].Attributes["dialogheight"].Value.ToLower();
                    dh = dh.Replace("px", "")+"px";
                    appendItemList[i].DialogHeight = dh;
                }

                if (null != itemlist[i].Attributes["dialogwidth"] && null != itemlist[i].Attributes["dialogwidth"].Value)
                {
                    string dw = itemlist[i].Attributes["dialogwidth"].Value.ToLower();
                    dw = dw.Replace("px", "") + "px";
                    appendItemList[i].DialogWidth = dw;
                }

                if (null != itemlist[i].Attributes["printitem"] && null != itemlist[i].Attributes["printitem"].Value)
                    appendItemList[i].PrintItem = itemlist[i].Attributes["printitem"].Value;
                if (null != itemlist[i].Attributes["printname"] && null != itemlist[i].Attributes["printname"].Value)
                    appendItemList[i].PrintTpName = itemlist[i].Attributes["printname"].Value;
                if (null != itemlist[i].Attributes["printcount"] && null != itemlist[i].Attributes["printcount"].Value)
                    appendItemList[i].PrintCountMin = itemlist[i].Attributes["printcount"].Value;

                if (null != itemlist[i].Attributes["printtype"] && null != itemlist[i].Attributes["printtype"].Value
                    && "WORD" == itemlist[i].Attributes["printtype"].Value.ToUpper())
                    appendItemList[i].PrintType = PrintType.Word;

                if (null != itemlist[i].Attributes["templatetype"] && null != itemlist[i].Attributes["templatetype"].Value)
                {
                    if (null != itemlist[i].Attributes["gridtemplate"] && null != itemlist[i].Attributes["gridtemplate"].Value)
                    {
                        if (itemlist[i].Attributes["templatetype"].Value.ToLower() == "html")
                            appendItemList[i].HTMLURL = itemlist[i].Attributes["gridtemplate"].Value;
                    }
                }
            }
            return appendItemList;
        }


		/// <summary>
		/// 获取业务单元的附加子功能单元
		/// </summary>
		/// <returns>返回明细项目数组</returns>
		public AppendItem GetAppendItem(string	itemName)
		{
            if (this.AppendItemList.Length == 0) return null;
			AppendItem		appendItem=null;
			for(int i=0;i<this.AppendItemList.Length;i++)
			{
				appendItem=this.AppendItemList[i];
				if(itemName.ToLower()==appendItem.ItemName.ToLower())
					return appendItem;
			}
 
            // 测试方法:修正，如果指定的itemName与appendItem集合中不相配，
            // 则用itemName更换掉其中的appendItem.unitName, 前题是必须配一个appendItem
            appendItem.UnitName = itemName;
            //---
            OpenDatabase();
            string systemdb = DataAccRes.AppSettings("SystemDB");
            if (systemdb == "" || systemdb == null) systemdb = "hmsys";
            string unitfile = DataAccRes.AppSettings("WorkConfig");
            string sqltext = "select top 1 templatetype,gridtemplate,workflow from " + systemdb + ".dbo.unititem where [name]='" + appendItem.UnitName + "' and ntype='UnitItem'";
            DataTable tab    = new DataTable();
            tab = BindGrid(sqltext);
            if (tab.Rows.Count > 0)
            {
                DataRow[] _dr = tab.Select();
                if (_dr[0][0].ToString().ToLower() == "html")
                    appendItem.HTMLURL = _dr[0][1].ToString();
                else
                    appendItem.HTMLURL = "";

            }
			return appendItem;
		}


		/// <summary>
		/// 获取指定单元的命令项列表
		/// </summary>
		/// <returns>返回命令项目数组</returns>
        private CommandItem[] GetCommandItemList()
        {
            if (null == this._xmlnode) return new CommandItem[0];
            XmlNodeList itemlist = this._xmlnode.SelectNodes("CommandItem");
            CommandItem[] cmdItemList = new CommandItem[itemlist.Count];

            for (int i = 0; i < itemlist.Count; i++)
            {
                cmdItemList[i] = new CommandItem();
                cmdItemList[i].ItemName = itemlist[i].Attributes["name"].Value;
                if (null != itemlist[i].Attributes["dataitem"])
                    cmdItemList[i].DataSrc = itemlist[i].Attributes["dataitem"].Value;
                if (null != itemlist[i].Attributes["topic"])
                    cmdItemList[i].Topic = itemlist[i].Attributes["topic"].Value;
                if (null == itemlist[i].Attributes["funtype"] || "" == itemlist[i].Attributes["funtype"].Value
                    || "sqlcmd" == itemlist[i].Attributes["funtype"].Value.ToLower())
                    cmdItemList[i].FunType = AppendFunType.SqlCmd;
                else if ("xmltodb" == itemlist[i].Attributes["funtype"].Value.ToLower())
                    cmdItemList[i].FunType = AppendFunType.ImportFromExecel;
                else if ("sqlcmdlist" == itemlist[i].Attributes["funtype"].Value.ToLower())
                    cmdItemList[i].FunType = AppendFunType.SqlCmdList;
                else
                    cmdItemList[i].FunType = AppendFunType.SqlCmd;
            }
            return cmdItemList;
        }


		/// <summary>
		/// 获取业务单元类型
		/// </summary>
		/// <returns></returns>
        private WorkUnitType GetWorkUnitType()
        {
            WorkUnitType workType = WorkUnitType.OtherItem;
            if (null == this._xmlnode) return workType;

            string strTPType = "";
            if (null != this._xmlnode.Attributes["templatetype"])
                strTPType = this._xmlnode.Attributes["templatetype"].Value.ToUpper();
            switch (strTPType)
            {

                case "SIMP":
                    workType = WorkUnitType.SimpleBank;
                    break;
                case "SIMPL":
                    workType = WorkUnitType.SimpleBankLeft;
                    break;
                case "HTML":
                    workType = WorkUnitType.HtmlBankLeft;
                    break;
                case "HTMLPOP":
                    workType = WorkUnitType.HtmlBankLeftPop;
                    break;
                case "R":
                    workType = WorkUnitType.ReportItem;
                    break;
                case "VCTP":
                    workType = WorkUnitType.VmlChartTp;
                    break;
                case "UP":
                    workType = WorkUnitType.Updatekey;
                    break;
                case "NAV":
                    workType = WorkUnitType.NavPage;
                    break;
                case "VML":
                    workType = WorkUnitType.VmlMapPage;
                    break;

                case "权限管理":
                    workType = WorkUnitType.SysOptRight;
                    break;
                case "组织机构":
                    workType = WorkUnitType.SysOrganize;
                    break;
                case "分配操作集":
                    workType = WorkUnitType.SysAssign;
                    break;
                case "操作集分组":
                    workType = WorkUnitType.SysOptions;
                    break;
                case "操作集设计":
                    workType = WorkUnitType.SysItem;
                    break;
                default:
                    workType = WorkUnitType.OtherItem;
                    break;
            }
            return workType;
        }


		/// <summary>
		/// 获取业务单元分组
		/// </summary>
		/// <returns></returns>
		private string			GetUnitGroup()
		{
            if (null == _drnode || null == this._drnode["unitgroup"])
				return string.Empty;
            return this._drnode["unitgroup"].ToString();

		}

		/// <summary>
		/// 获取业务数据的配设资源文件,虚拟站点路径
		/// </summary>
		/// <returns></returns>
        private string GetDataSrcFile()
        {
            if (null == this._xmlnode || null == this._xmlnode.Attributes["datasrcfile"])
                return string.Empty;
            return this._xmlnode.Attributes["datasrcfile"].Value;
        }

        /// <summary>
        /// 获取业务工作流
        /// </summary>
        /// <returns></returns>
        private string GetWorkFlow()
        {
            if (null == this._xmlnode || null == this._xmlnode.Attributes["workflow"])
                return string.Empty;
            return this._xmlnode.Attributes["workflow"].Value;
        }

		/// <summary>
		/// 获取单元的整体编辑模板,会对应多个项目数据源
		/// </summary>
		/// <returns></returns>
        private string GetFileEditTp()
        {
            if (null == this._xmlnode || null == this._xmlnode.Attributes["gridtemplate"])
                return string.Empty;
            return this._xmlnode.Attributes["gridtemplate"].Value;
        }

        /// <summary>
        /// 获取单元的打印模板,会对应多个项目数据源
        /// </summary>
        /// <returns></returns>
        private string GetFilePrintTp()
        {
            if (null == this._xmlnode || null == this._xmlnode.Attributes["printtemplate"])
                return string.Empty;
            return this._xmlnode.Attributes["printtemplate"].Value;
        }


		/// <summary>
		/// 获取单元的保存类型;
		/// </summary>
		/// <returns></returns>
        private SaveType GetSaveType()
        {
            if (null == this._xmlnode) return SaveType.GenerallySave;
            string saveType = "";
            if (null != this._xmlnode.Attributes["savetype"] && null != this._xmlnode.Attributes["savetype"].Value
                && "" != this._xmlnode.Attributes["savetype"].Value)
                saveType = this._xmlnode.Attributes["savetype"].Value;
            if ("trans" == saveType.ToLower())
                return SaveType.TransSave;
            else if ("saverefresh" == saveType.ToLower())
                return SaveType.SaveAndReload;
            else
                return SaveType.GenerallySave;
        }

		/// <summary>
		/// 获取字典列的配设资源文件,虚拟站点路径
		/// </summary>
		/// <returns></returns>
        private string GetDictColSrcFile()
        {
            if (null == this._xmlnode || null == this._xmlnode.Attributes["dictcolfile"]
                || null == this._xmlnode.Attributes["dictcolfile"].Value
                || "" == this._xmlnode.Attributes["dictcolfile"].Value)
                return string.Empty;
            return this._xmlnode.Attributes["dictcolfile"].Value;
        }


		/// <summary>
		/// 快速过滤
		/// </summary>
		/// <returns></returns>
        private ShortcutFilter GetShortcutFilter()
        {
            ShortcutFilter shFilter = new ShortcutFilter();
            if (null == this._xmlnode) return shFilter;
            XmlNode xmlnode = this._xmlnode.SelectSingleNode("ShortcutFilter");
            if (null == xmlnode) return shFilter;
            if (null != xmlnode.Attributes["linkcol"])
                shFilter.ColumnName = xmlnode.Attributes["linkcol"].Value;
            if (null != xmlnode.Attributes["dataitem"] && null != xmlnode.Attributes["valuecol"]
                && null != xmlnode.Attributes["textcol"])
            {
                shFilter.DataSrc = xmlnode.Attributes["dataitem"].Value;
                shFilter.TxtCol = xmlnode.Attributes["textcol"].Value;
                shFilter.ValueCol = xmlnode.Attributes["valuecol"].Value;
            }
            if (null != xmlnode.Attributes["coltype"])
                shFilter.ColType = xmlnode.Attributes["coltype"].Value;
            if (shFilter.ColumnName == "")
            {
                if (shFilter.TxtCol != "") { shFilter.ColumnName = shFilter.TxtCol; }
                else { shFilter.ColumnName = shFilter.ValueCol; }
            }

            if (shFilter.ColType == "")
            {
                shFilter.ColType = "char";
            }
            return shFilter;
        }

		/// <summary>
		/// 树形过滤
		/// </summary>
		/// <returns></returns>
        private TreeFilter GetTreeFilter()
        {
            TreeFilter treeFilter = new TreeFilter();
            if (null == this._xmlnode) return treeFilter;
            XmlNode xmlnode = this._xmlnode.SelectSingleNode("TreeFilter");

            if (null == xmlnode) return treeFilter;

            #region  数据源属性

            if (null != xmlnode.Attributes["dataitem"])
            {
                treeFilter.DataSrc = xmlnode.Attributes["dataitem"].Value;
                if (null != xmlnode.Attributes["linkcol"])
                    treeFilter.ColumnName = xmlnode.Attributes["linkcol"].Value;
                if (null != xmlnode.Attributes["level"])
                    treeFilter.Level = xmlnode.Attributes["level"].Value;
            }

            #endregion

            #region  其他显示属性
            if (xmlnode.Attributes["alert"] != null)
                treeFilter.Alertmsg = xmlnode.Attributes["alert"].Value;
            if (xmlnode.Attributes["gounititem"] != null)
                treeFilter.GoUnitItem = xmlnode.Attributes["gounititem"].Value;

            if (xmlnode.Attributes["treetype"] != null)
                treeFilter.TreeType = xmlnode.Attributes["treetype"].Value;

            if (xmlnode.Attributes["readtype"] != null)
                treeFilter.ReadType = xmlnode.Attributes["readtype"].Value;

            if (xmlnode.Attributes["treefun"] != null)
                treeFilter.TreeFun = xmlnode.Attributes["treefun"].Value;

            if (xmlnode.Attributes["hasall"] != null)
                treeFilter.HasAll = xmlnode.Attributes["hasall"].Value;

            if (xmlnode.Attributes["isfirst"] != null
                    && ("1" == xmlnode.Attributes["isfirst"].Value || "true" == xmlnode.Attributes["isfirst"].Value.ToLower()))
                treeFilter.IsFirst = true;
            else
                treeFilter.IsFirst = false;

            if (xmlnode.Attributes["firsthide"] != null
                 && ("1" == xmlnode.Attributes["firsthide"].Value || xmlnode.Attributes["firsthide"].Value.ToLower() == "true"))
                treeFilter.FirstHide = true;
            else
                treeFilter.FirstHide = false;

            if (xmlnode.Attributes["refreshed"] != null
                    && ("1" == xmlnode.Attributes["refreshed"].Value || xmlnode.Attributes["refreshed"].Value.ToLower() == "true"))
                treeFilter.EditFreshed = true;
            else
                treeFilter.EditFreshed = false;

            if (xmlnode.Attributes["tree2edit"] != null
                    && ("1" == xmlnode.Attributes["tree2edit"].Value || xmlnode.Attributes["tree2edit"].Value.ToLower() == "true"))
                treeFilter.TreeToEdit = true;
            else
                treeFilter.TreeToEdit = false;

            if (xmlnode.Attributes["tree2edit"] != null)
                treeFilter.TreeToEditFields = xmlnode.Attributes["tree2edit"].Value;
            #endregion

            return treeFilter;
        }


		/// <summary>
		/// 单元单据类型
		/// </summary>
		/// <returns></returns>
        private string GetBillType()
        {
            if (null == this._xmlnode || null == this._xmlnode.Attributes["billtype"]
                || "" == this._xmlnode.Attributes["billtype"].Value)
                return string.Empty;
            return this._xmlnode.Attributes["billtype"].Value;
        }

        /// <summary>
        /// 单元单据类型
        /// </summary>
        /// <returns></returns>
        private PrintType GetPrnType()
        {
            PrintType printtype = PrintType.HTML;
            if (null == this._xmlnode) return printtype;


            string strPrintType = "";
            if (null == this._xmlnode || null == this._xmlnode.Attributes["printtype"]
               || "" == this._xmlnode.Attributes["printtype"].Value) strPrintType = "HTML";
            else
                strPrintType = this._xmlnode.Attributes["printtype"].Value;
            switch (strPrintType.ToUpper())
            {
                case "WORD":
                    printtype = PrintType.Word;
                    break;
                case "EXCEL":
                    printtype = PrintType.Excel;
                    break;
                case "HTML":
                    printtype = PrintType.HTML;
                    break;
                default:
                    printtype = PrintType.HTML;
                    break;
            }
            return printtype;
        }


        /// <summary>
        /// 功能单元是否可视
        /// </summary>
        /// <returns>返回布尔值,默认True</returns>
        private bool GetVisibleNav()
        {
            if (null != this._xmlnode && null != this._xmlnode.Attributes["visiblenav"]
                && ("false" == this._xmlnode.Attributes["visiblenav"].Value.ToLower() || "0" == this._xmlnode.Attributes["visiblenav"].Value))
                return false;
            return true;
        }

        #region 自定义私有函数
        //_drnode._columns[0].ColumnName
        private bool IfExistCloumnName(DataRow dr,string colname)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
                if (dr.Table.Columns[i].ColumnName == colname)
                {
                    return true;
                    break;
                }
            return false;
        }
        #endregion

        #endregion

    }

    #endregion


    #region ShortcutFilter 快捷过虑
    /// <summary>
    /// 快捷过滤
    /// </summary>
    public class ShortcutFilter
	{
		public string	ColumnName="";
		public string	DataSrc="";
		public string	TxtCol="";
		public string	ValueCol="";
		public string	ColType="";
	}
	#endregion

	#region TreeFilter 树形过虑
	/// <summary>
	/// 快捷过滤
	/// </summary>
	public class TreeFilter
	{
		public string	ColumnName="";
		public string	DataSrc="";
		public string	Level="";
		public bool		IsFirst=true;
		public string	Alertmsg="";
		public string	GoUnitItem="";
		public string	TreeType="";
		public string	TreeFun="filter";
		public string	HasAll="";
		public bool 	FirstHide=false; //初始时是否隐藏
		//leo fixed 20050730
		public bool 	EditFreshed = false; //编辑正文后，树同步更新数据(ture or false)
		public bool     TreeToEdit  = false; //在新增节后，是否由树节点信息编辑正文信息(ture or false), 在初始增加时，该项不起作用，直接由树修改正文
		public string     TreeToEditFields  = ""; //在treetoedit=true时，设置需要从树中附值的字段名称，以“，”分割。为空时，则缺省取树各级的tag或text做为字段名
        public string ReadType = ""; //定义读取树的方式，“递归”表示以递归的方式来形成树，否则为平辅方式
    }
	#endregion


}
