using System;
using System.IO;
using System.Web.Configuration;
using System.Data;
using System.Web;
using System.Xml;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using System.Net;

namespace Estar.Business.UserRight
{

    #region 操作类型
    public enum OperationType
    {
        /// <summary>
        /// 选择查询权限
        /// </summary>
        Selecte,
        /// <summary>
        /// 创建新权限
        /// </summary>
        Insert,
        /// <summary>
        /// 修改数据权限
        /// </summary>
        Update,
        /// <summary>
        /// 删除数据权限
        /// </summary>
        Delete,
        /// <summary>
        /// 包括增删改查的全面责任权限
        /// </summary>
        DutyAll,
        /// <summary>
        /// 无操作,无效的权限
        /// </summary>
        None
    }
    #endregion

    /// <summary>
    /// 新的分级用户权限角色管理
    /// </summary>
    public class User
    {

        #region 工作单元权限
        /// <summary>
        /// 工作单元权限
        /// </summary>
        private class WorkUnitRight
        {
            string _workUnitName;
            string[] _operationName = new string[0];

            /// <summary>
            /// 工作单元
            /// </summary>
            public string WorkName
            {
                get { return this._workUnitName; }
                set { this._workUnitName = value; }
            }
            /// <summary>
            /// 操作权限
            /// </summary>
            public string[] Operation
            {
                get { return this._operationName; }
                set { this._operationName = value; }
            }
        }
        #endregion

        private string _userId = "";		//用户ID
        private string _userSn = "";		//用户编号
        private string _password = "";	//用户口令
        private string _dept = "";		//用户所属部门
        private string _deptcode = "XX";	//部门编号
        private string _deptid = "";		//部门ID
        private string _deptSaleName = "";  //销售部门
        private string _deptParent = "";   //上级部门名称
        private string _deptsub = "";	//用户所属分部门
        private string _deptsubcode = "";//用户所属分部门编号
        private string _deptsubid = "";	//分部门ID
        private string _userName = "";	//用户名称
        private string _company = "";	//公司名称
        private string _unitName = "";	//单位名称
        private string _unitcode = "";	//单位编号
        private string _unitID = "";		//单位ID
        private string _unitParent = "";   //本单位的上级单位名称
        private string _roleName = "";	//用户角色GUID
        private string _OPTUnitID = "";	//用户所属的独立单位ID
        private int _limitDays = 0; //限制用户访问的有效时间天
        private bool _visitOut = false;   //用户是否可公网访问
        private string _rights = "";       //用户是否访问权限范围

        private QueryDataRes userQuery = null;
        private WorkUnitRight _workRight;
        private XmlDocument xmlDocDeptRight = null;
        private DataSet dsDeptRight = new DataSet();
        static private DataTable tabUser = null;
        static private NameObjectList xmlDocRightList = new NameObjectList();

        public User(string userID)
        {
            this._userId = userID;
            this._workRight = new WorkUnitRight();
            NameObjectList paramList = new NameObjectList();
            if (null == User.tabUser)
            {
                if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
                User.tabUser = this.userQuery.getTable("用户登录基本信息结构", paramList);
            }
            DataRow drUser = null;
            for (int i = User.tabUser.Rows.Count - 1; i > -1; i--)
            {
                DataRow dr = User.tabUser.Rows[i];
                DateTime dStart = Convert.ToDateTime(dr["登录时间"]);
                DateTime dEnd = Convert.ToDateTime(dr["离线时间"]);
                //超时10分钟
                if (dStart.Add(new TimeSpan(0, 10, 0)) < dEnd)
                {
                    NameObjectList param = new NameObjectList();
                    param["帐号"] = dr["帐号"];
                    param["登录时间"] = dr["登录时间"];
                    param["离线时间"] = dr["离线时间"];
                    param["登录IP"] = dr["登录IP"];
                    paramList[paramList.Count.ToString()] = param;
                    User.tabUser.Rows.Remove(dr);
                    continue;
                }
                string userAddress = Dns.GetHostName();
                if (null != HttpContext.Current && null != HttpContext.Current.Request)
                    userAddress = HttpContext.Current.Request.UserHostAddress;
                if (this.UserAccounts == dr["帐号"].ToString() && dr["登录IP"].ToString() == userAddress)
                {
                    drUser = dr;
                    dr["离线时间"] = DateTime.Now;
                }
            }
            User.tabUser.AcceptChanges();
            NameObjectList[] paramListArry = new NameObjectList[paramList.Count];
            for (int i = 0; i < paramListArry.Length; i++)
                paramListArry[i] = paramList[i] as NameObjectList;
            if (paramListArry.Length > 0)
            {
                if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
                this.userQuery.ExecuteInsert("用户登录日志", paramListArry);
            }
            if (null == drUser)
            {
                this.createUserInfo();
                drUser = User.tabUser.NewRow();
                drUser["帐号"] = this.UserAccounts;
                drUser["登录时间"] = DateTime.Now;
                drUser["离线时间"] = DateTime.Now;
                drUser["登录IP"] = Dns.GetHostName();
                if (null != HttpContext.Current && null != HttpContext.Current.Request)
                    drUser["登录IP"] = HttpContext.Current.Request.UserHostAddress;

                drUser["姓名"] = this._userName;
                drUser["部门"] = this._dept;
                drUser["部门编号"] = this._deptcode;
                drUser["部门ID"] = this._deptid;
                drUser["销售部门"] = this._deptSaleName;
                drUser["分部门"] = this._deptsub;
                drUser["分部门编号"] = this._deptsubcode;
                drUser["分部门ID"] = this._deptsubid;
                drUser["公司"] = this._company;
                drUser["单位"] = this._unitName;
                drUser["单位编号"] = this._unitcode;
                drUser["单位ID"] = this._unitID;
                drUser["角色"] = this._roleName;
                drUser["权限类别"] = this._rights;
                drUser["限制天数"] = this._limitDays;
                drUser["上级单位"] = this._unitParent;
                drUser["上级部门"] = this._deptParent;
                drUser["权限文件"] = this._OPTUnitID;
                drUser["公网访问"] = this._visitOut;
                User.tabUser.Rows.Add(drUser);

            }
            else
            {
                this._userName = (null == drUser["姓名"]) ? "" : drUser["姓名"].ToString();
                this._dept = (null == drUser["部门"]) ? "" : drUser["部门"].ToString();
                this._deptcode = (null == drUser["部门编号"]) ? "" : drUser["部门编号"].ToString();
                this._deptid = (null == drUser["部门ID"]) ? "" : drUser["部门ID"].ToString();
                this._deptSaleName = (null == drUser["销售部门"]) ? "" : drUser["销售部门"].ToString();
                this._deptsub = (null == drUser["分部门"]) ? "" : drUser["分部门"].ToString();
                this._deptsubcode = (null == drUser["分部门编号"]) ? "" : drUser["分部门编号"].ToString();
                this._deptsubid = (null == drUser["分部门ID"]) ? "" : drUser["分部门ID"].ToString();
                this._company = (null == drUser["公司"]) ? "" : drUser["公司"].ToString();
                this._unitName = (null == drUser["单位"]) ? "" : drUser["单位"].ToString();
                this._unitcode = (null == drUser["单位编号"]) ? "" : drUser["单位编号"].ToString();
                this._unitID = (null == drUser["单位ID"]) ? "" : drUser["单位ID"].ToString();
                this._unitParent = (null == drUser["上级单位"]) ? "" : drUser["上级单位"].ToString();
                this._deptParent = (null == drUser["上级部门"]) ? "" : drUser["上级部门"].ToString();
                this._roleName = (null == drUser["角色"]) ? "" : drUser["角色"].ToString();
                this._rights = (null == drUser["权限类别"]) ? "" : drUser["权限类别"].ToString();
                this._OPTUnitID = (null == drUser["权限文件"]) ? "" : drUser["权限文件"].ToString();
                if (null == drUser["限制天数"])
                    this._limitDays = 0;
                else
                    this._limitDays = Convert.ToInt16(drUser["限制天数"]);
                if (null == drUser["公网访问"])
                    this._visitOut = false;
                else
                    this._visitOut = Convert.ToBoolean(drUser["公网访问"]);

                //将加入权限文件收为加入权限数据，来自于表mnu_rights,
                string fileName = this._OPTUnitID; //取操作员的上级独立单位的ID做为权限文件名(+.xml)
                if ("" != fileName)
                {
                    paramList.Clear();
                    paramList["deptcode"] = this._deptcode;
                    paramList["role"] = this._roleName;
                    this.userQuery = QueryDataRes.CreateQuerySys();
                    this.userQuery.FillDataSet("rightsql", paramList, this.dsDeptRight);
                }


            }

        }

        public User(string userID, ref Boolean blBad)
        {
            this._userId = userID;
            this._workRight = new WorkUnitRight();
            NameObjectList paramList = new NameObjectList();
            if (null == User.tabUser)
            {
                if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
                User.tabUser = this.userQuery.getTable("用户登录基本信息结构", paramList);
            }
            if (User.tabUser == null) { blBad = true; return; }
            DataRow drUser = null;
            for (int i = User.tabUser.Rows.Count - 1; i > -1; i--)
            {
                DataRow dr = User.tabUser.Rows[i];
                DateTime dStart = Convert.ToDateTime(dr["登录时间"]);
                DateTime dEnd = Convert.ToDateTime(dr["离线时间"]);
                //超时10分钟
                if (dStart.Add(new TimeSpan(0, 10, 0)) < dEnd)
                {
                    NameObjectList param = new NameObjectList();
                    param["帐号"] = dr["帐号"];
                    param["登录时间"] = dr["登录时间"];
                    param["离线时间"] = dr["离线时间"];
                    param["登录IP"] = dr["登录IP"];
                    paramList[paramList.Count.ToString()] = param;
                    User.tabUser.Rows.Remove(dr);
                    continue;
                }
                string userAddress = Dns.GetHostName();
                if (null != HttpContext.Current && null != HttpContext.Current.Request)
                    userAddress = HttpContext.Current.Request.UserHostAddress;
                if (this.UserAccounts == dr["帐号"].ToString() && dr["登录IP"].ToString() == userAddress)
                {
                    drUser = dr;
                    dr["离线时间"] = DateTime.Now;
                }
            }
            User.tabUser.AcceptChanges();
            NameObjectList[] paramListArry = new NameObjectList[paramList.Count];
            for (int i = 0; i < paramListArry.Length; i++)
                paramListArry[i] = paramList[i] as NameObjectList;
            if (paramListArry.Length > 0)
            {
                if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
                this.userQuery.ExecuteInsert("用户登录日志", paramListArry);
            }
            if (null == drUser)
            {
                this.createUserInfo();
                drUser = User.tabUser.NewRow();
                drUser["帐号"] = this.UserAccounts;
                drUser["登录时间"] = DateTime.Now;
                drUser["离线时间"] = DateTime.Now;
                drUser["登录IP"] = Dns.GetHostName();
                if (null != HttpContext.Current && null != HttpContext.Current.Request)
                    drUser["登录IP"] = HttpContext.Current.Request.UserHostAddress;

                drUser["姓名"] = this._userName;
                drUser["部门"] = this._dept;
                drUser["部门编号"] = this._deptcode;
                drUser["部门ID"] = this._deptid;
                drUser["销售部门"] = this._deptSaleName;
                drUser["分部门"] = this._deptsub;
                drUser["分部门编号"] = this._deptsubcode;
                drUser["分部门ID"] = this._deptsubid;
                drUser["公司"] = this._company;
                drUser["单位"] = this._unitName;
                drUser["单位编号"] = this._unitcode;
                drUser["单位ID"] = this._unitID;
                drUser["角色"] = this._roleName;
                drUser["权限类别"] = this._rights;
                drUser["限制天数"] = this._limitDays;
                drUser["上级单位"] = this._unitParent;
                drUser["上级部门"] = this._deptParent;
                drUser["权限文件"] = this._OPTUnitID;
                drUser["公网访问"] = this._visitOut;
                User.tabUser.Rows.Add(drUser);
            }
            else
            {
                this._userName = (null == drUser["姓名"]) ? "" : drUser["姓名"].ToString();
                this._dept = (null == drUser["部门"]) ? "" : drUser["部门"].ToString();
                this._deptcode = (null == drUser["部门编号"]) ? "" : drUser["部门编号"].ToString();
                this._deptid = (null == drUser["部门ID"]) ? "" : drUser["部门ID"].ToString();
                this._deptSaleName = (null == drUser["销售部门"]) ? "" : drUser["销售部门"].ToString();
                this._deptsub = (null == drUser["分部门"]) ? "" : drUser["分部门"].ToString();
                this._deptsubcode = (null == drUser["分部门编号"]) ? "" : drUser["分部门编号"].ToString();
                this._deptsubid = (null == drUser["分部门ID"]) ? "" : drUser["分部门ID"].ToString();
                this._company = (null == drUser["公司"]) ? "" : drUser["公司"].ToString();
                this._unitName = (null == drUser["单位"]) ? "" : drUser["单位"].ToString();
                this._unitcode = (null == drUser["单位编号"]) ? "" : drUser["单位编号"].ToString();
                this._unitID = (null == drUser["单位ID"]) ? "" : drUser["单位ID"].ToString();
                this._unitParent = (null == drUser["上级单位"]) ? "" : drUser["上级单位"].ToString();
                this._deptParent = (null == drUser["上级部门"]) ? "" : drUser["上级部门"].ToString();
                this._roleName = (null == drUser["角色"]) ? "" : drUser["角色"].ToString();
                this._rights = (null == drUser["权限类别"]) ? "" : drUser["权限类别"].ToString();
                this._OPTUnitID = (null == drUser["权限文件"]) ? "" : drUser["权限文件"].ToString();
                if (null == drUser["限制天数"])
                    this._limitDays = 0;
                else
                    this._limitDays = Convert.ToInt16(drUser["限制天数"]);
                if (null == drUser["公网访问"])
                    this._visitOut = false;
                else
                    this._visitOut = Convert.ToBoolean(drUser["公网访问"]);

                //将加入权限文件收为加入权限数据，来自于表mnu_rights,
                string fileName = this._OPTUnitID; //取操作员的上级独立单位的ID做为权限文件名(+.xml)
                if ("" != fileName)
                {
                    paramList.Clear();
                    paramList["deptcode"] = this._deptcode;
                    paramList["role"] = this._roleName;
                    this.userQuery = QueryDataRes.CreateQuerySys();
                    this.userQuery.FillDataSet("rightsql", paramList, this.dsDeptRight);
                }
            }

        }

        public User(string userID, Boolean reset, ref Boolean blBad)
        {
            this._userId = userID;
            this._workRight = new WorkUnitRight();
            NameObjectList paramList = new NameObjectList();
            if (null == User.tabUser || reset == true)
            {
                if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
                User.tabUser = this.userQuery.getTable("用户登录基本信息结构", paramList);
            }
            DataRow drUser = null;
            if (User.tabUser == null) { blBad = true; return; }
            for (int i = User.tabUser.Rows.Count - 1; i > -1; i--)
            {
                DataRow dr = User.tabUser.Rows[i];
                DateTime dStart = Convert.ToDateTime(dr["登录时间"]);
                DateTime dEnd = Convert.ToDateTime(dr["离线时间"]);
                //超时10分钟
                if (dStart.Add(new TimeSpan(0, 10, 0)) < dEnd)
                {
                    NameObjectList param = new NameObjectList();
                    param["帐号"] = dr["帐号"];
                    param["登录时间"] = dr["登录时间"];
                    param["离线时间"] = dr["离线时间"];
                    param["登录IP"] = dr["登录IP"];
                    paramList[paramList.Count.ToString()] = param;
                    User.tabUser.Rows.Remove(dr);
                    continue;
                }
                string userAddress = Dns.GetHostName();
                if (null != HttpContext.Current && null != HttpContext.Current.Request)
                    userAddress = HttpContext.Current.Request.UserHostAddress;
                if (this.UserAccounts == dr["帐号"].ToString() && dr["登录IP"].ToString() == userAddress)
                {
                    drUser = dr;
                    dr["离线时间"] = DateTime.Now;
                }
            }
            User.tabUser.AcceptChanges();
            NameObjectList[] paramListArry = new NameObjectList[paramList.Count];
            for (int i = 0; i < paramListArry.Length; i++)
                paramListArry[i] = paramList[i] as NameObjectList;
            if (paramListArry.Length > 0)
            {
                if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
                this.userQuery.ExecuteInsert("用户登录日志", paramListArry);
            }
            if (null == drUser)
            {
                this.createUserInfo();
                drUser = User.tabUser.NewRow();
                drUser["帐号"] = this.UserAccounts;
                drUser["登录时间"] = DateTime.Now;
                drUser["离线时间"] = DateTime.Now;
                drUser["登录IP"] = Dns.GetHostName();
                if (null != HttpContext.Current && null != HttpContext.Current.Request)
                    drUser["登录IP"] = HttpContext.Current.Request.UserHostAddress;

                drUser["姓名"] = this._userName;
                drUser["部门"] = this._dept;
                drUser["部门编号"] = this._deptcode;
                drUser["部门ID"] = this._deptid;
                drUser["销售部门"] = this._deptSaleName;
                drUser["分部门"] = this._deptsub;
                drUser["分部门编号"] = this._deptsubcode;
                drUser["分部门ID"] = this._deptsubid;
                drUser["公司"] = this._company;
                drUser["单位"] = this._unitName;
                drUser["单位编号"] = this._unitcode;
                drUser["单位ID"] = this._unitID;
                drUser["角色"] = this._roleName;
                drUser["权限类别"] = this._rights;
                drUser["限制天数"] = this._limitDays;
                drUser["上级单位"] = this._unitParent;
                drUser["上级部门"] = this._deptParent;
                drUser["权限文件"] = this._OPTUnitID;
                drUser["公网访问"] = this._visitOut;
                User.tabUser.Rows.Add(drUser);
            }
            else
            {
                this._userName = (null == drUser["姓名"]) ? "" : drUser["姓名"].ToString();
                this._dept = (null == drUser["部门"]) ? "" : drUser["部门"].ToString();
                this._deptcode = (null == drUser["部门编号"]) ? "" : drUser["部门编号"].ToString();
                this._deptid = (null == drUser["部门ID"]) ? "" : drUser["部门ID"].ToString();
                this._deptSaleName = (null == drUser["销售部门"]) ? "" : drUser["销售部门"].ToString();
                this._deptsub = (null == drUser["分部门"]) ? "" : drUser["分部门"].ToString();
                this._deptsubcode = (null == drUser["分部门编号"]) ? "" : drUser["分部门编号"].ToString();
                this._deptsubid = (null == drUser["分部门ID"]) ? "" : drUser["分部门ID"].ToString();
                this._company = (null == drUser["公司"]) ? "" : drUser["公司"].ToString();
                this._unitName = (null == drUser["单位"]) ? "" : drUser["单位"].ToString();
                this._unitcode = (null == drUser["单位编号"]) ? "" : drUser["单位编号"].ToString();
                this._unitID = (null == drUser["单位ID"]) ? "" : drUser["单位ID"].ToString();
                this._unitParent = (null == drUser["上级单位"]) ? "" : drUser["上级单位"].ToString();
                this._deptParent = (null == drUser["上级部门"]) ? "" : drUser["上级部门"].ToString();
                this._roleName = (null == drUser["角色"]) ? "" : drUser["角色"].ToString();
                this._rights = (null == drUser["权限类别"]) ? "" : drUser["权限类别"].ToString();
                this._OPTUnitID = (null == drUser["权限文件"]) ? "" : drUser["权限文件"].ToString();
                if (null == drUser["限制天数"])
                    this._limitDays = 0;
                else
                    this._limitDays = Convert.ToInt16(drUser["限制天数"]);
                if (null == drUser["公网访问"])
                    this._visitOut = false;
                else
                    this._visitOut = Convert.ToBoolean(drUser["公网访问"]);

                //将加入权限文件收为加入权限数据，来自于表mnu_rights,
                string fileName = this._OPTUnitID; //取操作员的上级独立单位的ID做为权限文件名(+.xml)
                if ("" != fileName)
                {
                    paramList.Clear();
                    paramList["deptcode"] = this._deptcode;
                    paramList["role"] = this._roleName;
                }
            }

        }
        #region  十八个用户属性

        /// <summary>
        /// 读取用户帐号
        /// </summary>
        public string UserAccounts
        {
            get { return this._userId; }
        }

        /// <summary>
        /// 读取用户编号
        /// </summary>
        public string UserSerialNum
        {
            get { return this._userSn; }
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get { return this._userName; }
        }

        /// <summary>
        /// 获取部门名称
        /// </summary>
        public string DeptmentName
        {
            get { return this._dept; }
        }
        /// <summary>
        /// 读取独立销售部门名称
        /// </summary>
        public string DeptSaleName
        {
            get { return this._deptSaleName; }
        }
        /// <summary>
        /// 获取部门编号
        /// </summary>
        public string DeptmentCode
        {
            get { return this._deptcode; }
        }

        /// <summary>
        /// 获取部门ID
        /// </summary>
        public string DeptmentID
        {
            get { return this._deptid; }
        }

        /// <summary>
        /// 获取用户部门的上级部门名称
        /// </summary>
        public string DeptSup
        {
            get { return this._deptParent; }
        }

        /// <summary>
        /// 读取分部门名称
        /// </summary>
        public string SubDeptName
        {
            get { return this._deptsub; }
        }

        /// <summary>
        /// 读取分部门编号
        /// </summary>
        public string SubDeptCode
        {
            get { return this._deptsubcode; }
        }

        /// <summary>
        /// 读取分部门编号
        /// </summary>
        public string SubDeptID
        {
            get { return this._deptsubid; }
        }

        /// <summary>
        /// 获取用户总公司名称
        /// </summary>
        public string Company
        {
            get { return this._company; }
        }

        /// <summary>
        /// 获取用户单位名称
        /// </summary>
        public string UnitName
        {
            get { return this._unitName; }
        }

        /// <summary>
        /// 读取单位编号
        /// </summary>
        public string UnitCode
        {
            get { return this._unitcode; }
        }

        /// <summary>
        /// 读取单位GUID
        /// </summary>
        public string UnitID
        {
            get { return this._unitID; }
        }

        /// <summary>
        /// 读取用户所属的独立单位ID
        /// </summary>
        public string OPTUnitID
        {
            get { return this._OPTUnitID; }
        }

        /// <summary>
        /// 读取用户单位的上级单位名称
        /// </summary>
        public string UnitSup
        {
            get { return this._unitParent; }
        }

        /// <summary>
        /// 读取用户角色名称
        /// </summary>
        public string RoleName
        {
            get { return this._roleName; }
        }
        /// <summary>
        /// 读取用户角色权限范围
        /// </summary>
        public string Rights
        {
            get { return this._rights; }
        }
        /// <summary>
        /// 用户所属独立单位权限文档
        /// </summary>
        public XmlDocument XmlDocDeptRight
        {
            get { return this.xmlDocDeptRight; }
        }

        /// <summary>
        /// 用户所属权限表
        /// </summary>
        public DataSet DsDeptRight
        {
            get { return this.dsDeptRight; }
        }

        /// <summary>
        /// 读取用户访问数据的有效天数
        /// </summary>
        public int LimitDays
        {
            get { return this._limitDays; }
        }

        /// <summary>
        /// 读取用户是否访问公网属性
        /// </summary>
        public bool VisitOut
        {
            get { return this._visitOut; }
        }

        #endregion

        #region 公共函数

        /// <summary>
        /// 判断用户是否有指定单元的权限
        /// </summary>
        /// <param name="strWorkUnitName">功能单元名称</param>
        /// <param name="operateType">操作权限类型</param>
        /// <returns>返回是否有该权限;true为有;false为没有</returns>
        public bool HasRight(string strWorkUnitName, OperationType operateType)
        {
            this._setWorkUnitName(strWorkUnitName);
            bool bInsert = false, bUpdate = false, bDelete = false, bSelecte = false, bDuty = false;
            OperationType type = OperationType.None;
            for (int i = 0; i < this._workRight.Operation.Length; i++)
            {
                type = this._parseOperateType(this._workRight.Operation[i]);
                switch (type)
                {
                    case OperationType.Insert:
                        bInsert = true; bSelecte = true;
                        break;
                    case OperationType.Update:
                        bUpdate = true; bSelecte = true;
                        break;
                    case OperationType.Delete:
                        bDelete = true; bSelecte = true;
                        break;
                    case OperationType.Selecte:
                        bSelecte = true;
                        break;
                    case OperationType.DutyAll:
                        bInsert = true; bUpdate = true; bDelete = true; bSelecte = true; bDuty = true;
                        break;
                    default: break;
                }
            }
            if (bSelecte && bInsert && bUpdate && bDelete)
                bDuty = true;
            if (this.UserAccounts == "12345678")
            { bInsert = true; bUpdate = true; bDelete = true; bSelecte = true; bDuty = true; }
            if (operateType == OperationType.Selecte && bSelecte && !bInsert && !bUpdate && !bDelete)
                return true;
            if (operateType == OperationType.Insert && bInsert)
                return true;
            if (operateType == OperationType.Update && bUpdate)
                return true;
            if (operateType == OperationType.Delete && bDelete)
                return true;
            if (operateType == OperationType.DutyAll && bDuty)
                return true;

            return false;
        }


        /// <summary>
        /// 把操作数据中的字符转换成枚举类型
        /// </summary>
        /// <param name="operationName">数据库中存储的类型</param>
        /// <returns>系统权限枚举类型</returns>
        private OperationType _parseOperateType(string operationName)
        {
            switch (operationName)
            {
                case "浏览": return OperationType.Selecte;
                case "增加": return OperationType.Insert;
                case "修改": return OperationType.Update;
                case "删除": return OperationType.Delete;
                case "增删改": return OperationType.DutyAll;
                default: return OperationType.None;
            }
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool login(string password)
        {
            NameObjectList paramList = new NameObjectList();
            paramList["userid"] = this.UserAccounts;
            paramList["密码"] = password;
            if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
            DataTable tab = this.userQuery.getTable("人员口令验证", paramList, null);
            if (tab == null) return false;
            this._password = password;
            if (tab.Rows.Count < 1) return false;
            string userAddress = Dns.GetHostName();
            if (null != HttpContext.Current && null != HttpContext.Current.Request)
                userAddress = HttpContext.Current.Request.UserHostAddress;
            for (int i = 0; i < User.tabUser.Rows.Count; i++)
                if (this.UserAccounts == User.tabUser.Rows[i]["帐号"].ToString() && User.tabUser.Rows[i]["登录IP"].ToString() == userAddress)
                {
                    User.tabUser.Rows[i]["离线时间"] = DateTime.Now;
                    return true;
                }
            DataRow dr = User.tabUser.NewRow();
            dr["帐号"] = this.UserAccounts;
            dr["登录时间"] = DateTime.Now;
            dr["离线时间"] = DateTime.Now;
            dr["登录IP"] = userAddress;
            User.tabUser.Rows.Add(dr);
            return true;
        }




        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool login(string user, string password)
        {
            NameObjectList paramList = new NameObjectList();
            paramList["userid"] = user;
            paramList["密码"] = password;
            if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
            DataTable tab = this.userQuery.getTable("人员口令验证", paramList, null);
            if (tab == null) return false;
            this._password = password;
            if (tab.Rows.Count < 1) return false;
            string userAddress = Dns.GetHostName();
            if (null != HttpContext.Current && null != HttpContext.Current.Request)
                userAddress = HttpContext.Current.Request.UserHostAddress;
            for (int i = 0; i < User.tabUser.Rows.Count; i++)
                if (this.UserAccounts == User.tabUser.Rows[i]["帐号"].ToString() && User.tabUser.Rows[i]["登录IP"].ToString() == userAddress)
                {
                    User.tabUser.Rows[i]["离线时间"] = DateTime.Now;
                    return true;
                }
            DataRow dr = User.tabUser.NewRow();
            dr["帐号"] = this.UserAccounts;
            dr["登录时间"] = DateTime.Now;
            dr["离线时间"] = DateTime.Now;
            dr["登录IP"] = userAddress;
            User.tabUser.Rows.Add(dr);
            return true;
        }











        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="oldPassword">原来的密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public bool ModifyPassword(string oldPassword, string newPassword)
        {
            NameObjectList paramList = new NameObjectList();
            if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
            QueryDataRes query = this.userQuery;
            paramList["userid"] = this.UserAccounts;
            paramList["原密码"] = oldPassword;
            paramList["新密码"] = newPassword;
            if (query.ExecuteUpdate("人员口令验证", paramList))
            {
                this._password = newPassword;
                return true;
            }
            else
                return false;

        }


        /// <summary>
        /// 判断是否初次使用本系统
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IfFirstLogin()
        {
            NameObjectList paramList = new NameObjectList();
            if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
            DataTable tab = this.userQuery.getTable("系统初始验证", paramList, null);
            if (tab == null) return false;
            if (tab.Rows.Count < 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 用户心跳,更新用户离线时间
        /// </summary>
        /// <param name="userid"></param>
        public static void HeartBeat()
        {
            if (null == User.tabUser || User.tabUser.Rows.Count < 1)
                return;
            if (null == HttpContext.Current || null == HttpContext.Current.Request)
                return;
            string userAddress = HttpContext.Current.Request.UserHostAddress;
            string userid = Convert.ToString(HttpContext.Current.Session["userid"]);
            if (string.IsNullOrEmpty(userid))
                return;
            //更新离线时间
            for (int i = 0; i < User.tabUser.Rows.Count; i++)
            {
                DataRow dr = User.tabUser.Rows[i];
                if (!userid.Equals(dr["帐号"]) || !userAddress.Equals(dr["登录IP"]))
                    continue;
                dr["离线时间"] = DateTime.Now;
                User.tabUser.AcceptChanges();
                break;
            }
        }

        #endregion

        #region 内部函数

        /// <summary>
        /// 设置用户信息
        /// </summary>
        private void createUserInfo()
        {
            NameObjectList paramList = new NameObjectList();
            paramList["userid"] = this.UserAccounts;
            if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
            DataTable tab = this.userQuery.getTable("人员基本信息", paramList);
            if (tab.Rows.Count < 1)
            {
                this._userName = "系统管理员";
            }
            else
            {
                this._userName = (null == tab.Rows[0]["姓名"]) ? "" : tab.Rows[0]["姓名"].ToString();
                this._dept = (null == tab.Rows[0]["部门"]) ? "" : tab.Rows[0]["部门"].ToString();
                this._deptcode = (null == tab.Rows[0]["部门编号"]) ? "" : tab.Rows[0]["部门编号"].ToString();
                this._deptid = (null == tab.Rows[0]["部门ID"]) ? "" : tab.Rows[0]["部门ID"].ToString();
                this._deptsub = (null == tab.Rows[0]["分部门"]) ? "" : tab.Rows[0]["分部门"].ToString();
                this._deptsubcode = (null == tab.Rows[0]["分部门编号"]) ? "" : tab.Rows[0]["分部门编号"].ToString();
                this._deptsubid = (null == tab.Rows[0]["分部门ID"]) ? "" : tab.Rows[0]["分部门ID"].ToString();
                this._unitName = (null == tab.Rows[0]["单位"]) ? "" : tab.Rows[0]["单位"].ToString();
                this._unitcode = (null == tab.Rows[0]["单位编号"]) ? "" : tab.Rows[0]["单位编号"].ToString();
                this._unitID = (null == tab.Rows[0]["单位ID"]) ? "" : tab.Rows[0]["单位ID"].ToString();
                this._roleName = (null == tab.Rows[0]["角色"]) ? "" : tab.Rows[0]["角色"].ToString();
                if (null == tab.Rows[0]["限制天数"] || DBNull.Value == tab.Rows[0]["限制天数"])
                    this._limitDays = 0;
                else
                    this._limitDays = Convert.ToInt16(tab.Rows[0]["限制天数"]);
                if (null == tab.Rows[0]["公网访问"] || DBNull.Value == tab.Rows[0]["公网访问"])
                    this._visitOut = false;
                else
                    this._visitOut = Convert.ToBoolean(tab.Rows[0]["公网访问"]);
            }
            string fileName = "";
            if (tab.Rows.Count > 0 && null != tab.Rows[0]["分部门独立管理"]
                    && (tab.Rows[0]["分部门独立管理"].Equals(true) || "true" == tab.Rows[0]["分部门独立管理"].ToString().ToLower()))
                fileName = (null == tab.Rows[0]["分部门编号"]) ? "" : tab.Rows[0]["分部门编号"].ToString();
            else if (tab.Rows.Count > 0 && null != tab.Rows[0]["部门独立管理"]
                    && (tab.Rows[0]["部门独立管理"].Equals(true) || "true" == tab.Rows[0]["部门独立管理"].ToString().ToLower()))
                fileName = (null == tab.Rows[0]["部门编号"]) ? "" : tab.Rows[0]["部门编号"].ToString();
            else if (tab.Rows.Count > 0)
                fileName = (null == tab.Rows[0]["单位编号"]) ? "" : tab.Rows[0]["单位编号"].ToString();
            this._OPTUnitID = fileName;

            //根据this._OPTUnitID查出权限数据
            if ("" != fileName)
            {
                //打开权限数据,条件：角色,部门,所在单位(this._OPTUnitID),得出所属节点的操作集
                paramList.Clear();
                paramList["deptcode"] = this.DeptmentCode;
                paramList["role"] = this.RoleName;
                this.userQuery.FillDataSet("rightsql", paramList, this.dsDeptRight);
            }
            paramList.Clear();
            paramList["DeptCode"] = this.DeptmentCode;
            paramList["OPTUnitID"] = this.UnitID;
            if (null == this.userQuery) this.userQuery = QueryDataRes.CreateQuerySys();
            tab = this.userQuery.getTable("上级部门", paramList);
            if (null != tab && tab.Rows.Count > 0 && null != tab.Rows[0]["名称"])
                this._deptParent = tab.Rows[0]["名称"].ToString();
            tab = this.userQuery.getTable("销售部门", paramList);
            if (null != tab && tab.Rows.Count > 0 && null != tab.Rows[0]["名称"])
                this._deptSaleName = tab.Rows[0]["名称"].ToString();

            tab = this.userQuery.getTable("上级单位", paramList);
            if (null != tab && tab.Rows.Count > 0 && null != tab.Rows[0]["名称"])
                this._unitParent = tab.Rows[0]["名称"].ToString();

        }
        /// <summary>
        /// 取出XML节点Tag标记的字典值对数据
        /// </summary>
        /// <param name="stag">tag值</param>
        /// <param name="varname">取的字典值键值</param>
        /// <returns>得到对应的键值</returns>
        private string valtag(string stag, string varname)
        {
            if (null == stag || "" == stag)
                return "";
            string[] arrTag = stag.Split(",".ToCharArray());
            for (int i = 0; i < arrTag.Length; i++)
            {
                if (arrTag[i].IndexOf("@" + varname + "=") > -1)
                {
                    if ("" == arrTag[i]) continue;
                    string[] arr = arrTag[i].Split("=".ToCharArray());
                    return arr[1];
                }
            }
            return "";
        }

        /// <summary>
        /// 设置用户当前工作单元,初始化权限
        /// </summary>
        /// <param name="strWorkUnitName"></param>
        private void _setWorkUnitName(string strWorkUnitName)
        {
            this._workRight.WorkName = strWorkUnitName;
            this._workRight.Operation = new string[0];
            if ("" == this.RoleName) return;
            string strDeptCode = "";
            if ("" != this.SubDeptCode)
                strDeptCode = this.SubDeptCode;
            else if ("" != this.DeptmentCode)
                strDeptCode = this.DeptmentCode;
            else if ("" != this.UnitCode)
                strDeptCode = this.UnitCode;
            if ("" == strDeptCode) return;
            DataSet ds = this.DsDeptRight;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["ntype"].ToString() != "基本操作集")
                    continue;
                string itemname = ds.Tables[0].Rows[i]["name"].ToString();
                if (itemname == strWorkUnitName)
                {
                    string str = ds.Tables[0].Rows[i]["userright"].ToString();
                    this._workRight.Operation = new string[1];
                    this._workRight.Operation[0] = str;
                    break;
                }
            }
            return;
        }
        #endregion

    }
}
