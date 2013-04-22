using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Specialized;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;
using Estar.Common.Tools;

namespace Estar.Common.Tools
{
	/// <summary>
	/// IDataParameterCollection, IList, ICollection, IEnumerable
	/// </summary>
	public class NameParamCollection : IDataParameterCollection
	{
		private	object _this;	//对参数列表的引用

		#region 构造函数
		public NameParamCollection(IDataParameterCollection		parameterCollection)
		{
			this._this=parameterCollection;
		}
		#endregion

		#region 参数列表类型及获取引用参数列表
		/// <summary>
		/// 获取这个参数的引用参数类型
		/// </summary>
		/// <returns></returns>
		public DataAccessType  ParameterCollectionType()
		{
			if(null!=(this._this as SqlParameterCollection))
				return DataAccessType.SqlClient;
			if(null!=(this._this as OleDbParameterCollection))
				return DataAccessType.OLEDB;
			if(null!=(this._this as OracleParameterCollection))
				return DataAccessType.ODPNet;
			return DataAccessType.SqlClient;
		}

		public DBParameter this[string parameterName]
		{
			get
			{
				DBParameter param=null;
				switch(this.ParameterCollectionType())
				{
					case DataAccessType.SqlClient:
						param=new DBParameter((this._this as SqlParameterCollection)[parameterName]);break;
					case DataAccessType.OLEDB:
						param=new DBParameter((this._this as OleDbParameterCollection)[parameterName]);break;
					case DataAccessType.ODPNet:
						param=new DBParameter((this._this as OracleParameterCollection)[parameterName]);break;
				}
				return param;
			}
		}

		public DBParameter this[int index]
		{
			get
			{
				DBParameter param=null;
				switch(this.ParameterCollectionType())
				{
					case DataAccessType.SqlClient:
						param=new DBParameter((this._this as SqlParameterCollection)[index]);break;
					case DataAccessType.OLEDB:
						param=new DBParameter((this._this as OleDbParameterCollection)[index]);break;
					case DataAccessType.ODPNet:
						param=new DBParameter((this._this as OracleParameterCollection)[index]);break;
				}
				return param;
			}
		}


		public SqlParameterCollection SqlParameterCollection
		{
			get{ return (this._this as SqlParameterCollection);}
		}
		public OleDbParameterCollection OleDbParameterCollection
		{
			get{ return (this._this as OleDbParameterCollection);}
		}
		public OracleParameterCollection OracleParameterCollection
		{
			get{ return (this._this as OracleParameterCollection);}
		}
		#endregion

		#region IDataParameterCollection 成员

		object IDataParameterCollection.this[string parameterName]
		{
			get
			{
				return this[parameterName];
			}
			set
			{
				throw(new Exception("可以设置参数值,不能设置参数对象!"));
			}
		}

		public void RemoveAt(string parameterName)
		{
			(this._this as IDataParameterCollection).RemoveAt(parameterName);
		}

		public bool Contains(string parameterName)
		{
			return (this._this as IDataParameterCollection).Contains(parameterName);
		}

		public int IndexOf(string parameterName)
		{
			return (this._this as IDataParameterCollection).IndexOf(parameterName);
		}

		#endregion

		#region IList 成员

		public bool IsReadOnly
		{
			get
			{
				return (this._this as IList).IsReadOnly;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return (this._this as IList)[index];
			}
			set
			{
				throw(new Exception("可以设置参数值,不能设置参数对象!"));
			}
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			(this._this as IList).RemoveAt(index);
		}

		public void Insert(int index, object value)
		{
			throw(new Exception("可以设置参数值,不能设置参数对象!"));
		}

		public void Remove(object value)
		{
			(this._this as IList).Remove(value);
		}

		bool System.Collections.IList.Contains(object value)
		{
			return (this._this as IList).Contains(value);
		}

		public void Clear()
		{
			(this._this as IList).Clear();
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return (this._this as IList).IndexOf(value);
		}

		public int Add(object value)
		{
			throw(new Exception("可以设置参数值,不能设置参数对象!"));
		}

		public bool IsFixedSize
		{
			get
			{
				return (this._this as IList).IsFixedSize;
			}
		}

		#endregion

		#region ICollection 成员

		public bool IsSynchronized
		{
			get
			{
				return (this._this as ICollection).IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return (this._this as ICollection).Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			(this._this as ICollection).CopyTo(array,index);
		}

		public object SyncRoot
		{
			get
			{
				return (this._this as ICollection).SyncRoot;
			}
		}

		#endregion

		#region IEnumerable 成员

		public IEnumerator GetEnumerator()
		{
			return (this._this as IEnumerable).GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// 对Sql,OLEDB,Oracle各种方式的数据访问参数的适配器
	/// </summary>
	public class DBParameter : IDbDataParameter,IDataParameter,ICloneable
	{
		private object _this;	//实际参数的引用

		#region 构造函数
		public DBParameter(SqlParameter	sqlParameter)
		{
			this._this=sqlParameter;
		}

		public DBParameter(OleDbParameter	oleDbParameter)
		{
			this._this=oleDbParameter;
		}

		public DBParameter(OracleParameter	oracleParameter)
		{
			this._this=oracleParameter;
		}
		#endregion

		#region 参数类型及获取引用参数
		/// <summary>
		/// 获取这个参数的引用参数类型
		/// </summary>
		/// <returns></returns>
		public DataAccessType  ParameterType()
		{
			if(null!=(this._this as SqlParameter))
				return DataAccessType.SqlClient;
			if(null!=(this._this as OleDbParameter))
				return DataAccessType.OLEDB;
			if(null!=(this._this as OracleParameter))
				return DataAccessType.ODPNet;
			return DataAccessType.SqlClient;
		}

		/// <summary>
		/// 获取sql类型参数,如果不是就返回null;
		/// </summary>
		public SqlParameter SqlParameter
		{
			get{return (this._this as SqlParameter);}
		}
		/// <summary>
		/// 获取OleDb类型参数,如果不是就返回null;
		/// </summary>
		public OleDbParameter OleDbParameter
		{
			get{return (this._this as OleDbParameter);}
		}
		/// <summary>
		/// 获取oralce的ODP参数类型,如果不是就返回null;
		/// </summary>
		public OracleParameter OracleParameter
		{
			get{return (this._this as OracleParameter);}
		}
		#endregion

		#region IDbDataParameter 成员

		public byte Precision
		{
			get
			{
				return (this._this as IDbDataParameter).Precision;
			}
			set
			{
				(this._this as IDbDataParameter).Precision=value;
			}
		}

		public byte Scale
		{
			get
			{
				return (this._this as IDbDataParameter).Scale;
			}
			set
			{
				(this._this as IDbDataParameter).Scale=value;
			}
		}

		public int Size
		{
			get
			{
				return (this._this as IDbDataParameter).Size;
			}
			set
			{
				(this._this as IDbDataParameter).Size=value;
			}
		}

		#endregion

		#region IDataParameter 成员

		public System.Data.ParameterDirection Direction
		{
			get
			{
				return (this._this as IDataParameter).Direction;
			}
			set
			{
				(this._this as IDataParameter).Direction=value;
			}
		}

		public System.Data.DbType DbType
		{
			get
			{
				return (this._this as IDataParameter).DbType;
			}
			set
			{
				(this._this as IDataParameter).DbType=value;
			}
		}

		public object Value
		{
			get
			{
				return (this._this as IDataParameter).Value;
			}
			set
			{
				(this._this as IDataParameter).Value=value;
			}
		}

		public bool IsNullable
		{
			get
			{
				return (this._this as IDataParameter).IsNullable;
			}
		}

		public System.Data.DataRowVersion SourceVersion
		{
			get
			{
				return (this._this as IDataParameter).SourceVersion;
			}
			set
			{
				(this._this as IDataParameter).SourceVersion=value;
			}
		}

		public string ParameterName
		{
			get
			{
				string	strName=(this._this as IDataParameter).ParameterName;
				switch(this.ParameterType())
				{
					case DataAccessType.SqlClient:
						strName=strName.StartsWith("@")?strName.Remove(0,1):strName;
						break;
					default:
						break;
				}
				return strName;
			}
			set
			{
				(this._this as IDataParameter).ParameterName=value;
			}
		}

		public string SourceColumn
		{
			get
			{
				return (this._this as IDataParameter).SourceColumn;
			}
			set
			{
				(this._this as IDataParameter).SourceColumn=value;
			}
		}

		#endregion

		#region ICloneable 成员

		public object Clone()
		{
			return (this._this as ICloneable).Clone();
		}

		#endregion

	}
}
