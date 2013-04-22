using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Data.OleDb;
using Oracle.DataAccess.Client;

namespace Estar.Common.Tools
{
	/// <summary>
	/// DBCommand 的摘要说明。
	/// </summary>
	public class DBCommand : IDbCommand
	{
		private IDbCommand	_this=null;

		public DBCommand(IDbCommand		cmd)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this._this=cmd;
		}


		#region 参数类型及获取引用参数
		
		/// <summary>
		/// 获取这个参数的引用参数类型
		/// </summary>
		/// <returns></returns>
		public DataAccessType  DBCommandType()
		{
			if(null!=(this._this as SqlCommand))
				return DataAccessType.SqlClient;
			if(null!=(this._this as OleDbCommand))
				return DataAccessType.OLEDB;
			if(null!=(this._this as OracleCommand))
				return DataAccessType.ODPNet;
			return DataAccessType.SqlClient;
		}

		public NameParamCollection	DBParameters
		{
			get
			{
				// TODO:  添加 DBCommand.Parameters getter 实现
				return new NameParamCollection(this._this.Parameters);
			}
		}

        /// <summary>
        /// 将 DBCommand.CommandText 发送到 DBCommand.Connection 并生成一个System.Xml.XmlReader对象。
        /// </summary>
        /// <returns>生成一个System.Xml.XmlReader对象</returns>
        public XmlReader ExecuteXmlReader()
        {
            switch (this.DBCommandType())
            {
                case DataAccessType.SqlClient:
                    return this.SqlCommand.ExecuteXmlReader();
                case DataAccessType.OLEDB:
                    return null;
                case DataAccessType.ODPNet:
                    return this.OracleCommand.ExecuteXmlReader();
            }
            return null;
        }

		/// <summary>
		/// 获取sql类型参数,如果不是就返回null;
		/// </summary>
		public SqlCommand SqlCommand
		{
			get{return (this._this as SqlCommand);}
		}
		/// <summary>
		/// 获取OleDb类型参数,如果不是就返回null;
		/// </summary>
		public OleDbCommand OleDbCommand
		{
			get{return (this._this as OleDbCommand);}
		}
		/// <summary>
		/// 获取oralce的ODP参数类型,如果不是就返回null;
		/// </summary>
		public OracleCommand OracleCommand
		{
			get{return (this._this as OracleCommand);}
		}

		#endregion

		#region IDbCommand 成员

		public void Cancel()
		{
			// TODO:  添加 DBCommand.Cancel 实现
			this._this.Cancel();
		}

		public void Prepare()
		{
			// TODO:  添加 DBCommand.Prepare 实现
			this._this.Prepare();
		}

		public System.Data.CommandType CommandType
		{
			get
			{
				// TODO:  添加 DBCommand.CommandType getter 实现
				return this._this.CommandType;
			}
			set
			{
				// TODO:  添加 DBCommand.CommandType setter 实现
				this._this.CommandType=value;
			}
		}

		public IDataReader ExecuteReader(System.Data.CommandBehavior behavior)
		{
			// TODO:  添加 DBCommand.ExecuteReader 实现
			return this._this.ExecuteReader(behavior);
		}

		IDataReader System.Data.IDbCommand.ExecuteReader()
		{
			// TODO:  添加 DBCommand.System.Data.IDbCommand.ExecuteReader 实现
			return this._this.ExecuteReader();
		}

		public object ExecuteScalar()
		{
			// TODO:  添加 DBCommand.ExecuteScalar 实现
			return this._this.ExecuteScalar();
		}

		public int ExecuteNonQuery()
		{
			// TODO:  添加 DBCommand.ExecuteNonQuery 实现
			return this._this.ExecuteNonQuery();
		}

		public int CommandTimeout
		{
			get
			{
				// TODO:  添加 DBCommand.CommandTimeout getter 实现
				return this._this.CommandTimeout;
			}
			set
			{
				// TODO:  添加 DBCommand.CommandTimeout setter 实现
				this._this.CommandTimeout=value;
			}
		}

		/// <summary>
		/// 隐藏函数
		/// </summary>
		/// <returns></returns>
		public IDbDataParameter CreateParameter()
		{
			// TODO:  添加 DBCommand.CreateParameter 实现
			return this._this.CreateParameter();
		}

		public IDbConnection Connection
		{
			get
			{
				// TODO:  添加 DBCommand.Connection getter 实现
				return this._this.Connection;
			}
			set
			{
				// TODO:  添加 DBCommand.Connection setter 实现
				this._this.Connection=value;
			}
		}

		public System.Data.UpdateRowSource UpdatedRowSource
		{
			get
			{
				// TODO:  添加 DBCommand.UpdatedRowSource getter 实现
				return this._this.UpdatedRowSource;
			}
			set
			{
				// TODO:  添加 DBCommand.UpdatedRowSource setter 实现
				this._this.UpdatedRowSource=value;
			}
		}

		public string CommandText
		{
			get
			{
				// TODO:  添加 DBCommand.CommandText getter 实现
				return this._this.CommandText;
			}
			set
			{
				// TODO:  添加 DBCommand.CommandText setter 实现
				this._this.CommandText=value;
			}
		}

		/// <summary>
		/// 重定义特性
		/// </summary>
		public IDataParameterCollection Parameters
		{
			get
			{
				// TODO:  添加 DBCommand.Parameters getter 实现
				return this._this.Parameters;
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				// TODO:  添加 DBCommand.Transaction getter 实现
				return this._this.Transaction;
			}
			set
			{
				// TODO:  添加 DBCommand.Transaction setter 实现
				this._this.Transaction=value;
			}
		}

		#endregion

		#region IDisposable 成员

		public void Dispose()
		{
			// TODO:  添加 DBCommand.Dispose 实现
			this._this.Dispose();
		}

		#endregion

	}
}
