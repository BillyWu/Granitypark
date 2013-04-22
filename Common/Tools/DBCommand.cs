using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Data.OleDb;
using Oracle.DataAccess.Client;

namespace Estar.Common.Tools
{
	/// <summary>
	/// DBCommand ��ժҪ˵����
	/// </summary>
	public class DBCommand : IDbCommand
	{
		private IDbCommand	_this=null;

		public DBCommand(IDbCommand		cmd)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this._this=cmd;
		}


		#region �������ͼ���ȡ���ò���
		
		/// <summary>
		/// ��ȡ������������ò�������
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
				// TODO:  ��� DBCommand.Parameters getter ʵ��
				return new NameParamCollection(this._this.Parameters);
			}
		}

        /// <summary>
        /// �� DBCommand.CommandText ���͵� DBCommand.Connection ������һ��System.Xml.XmlReader����
        /// </summary>
        /// <returns>����һ��System.Xml.XmlReader����</returns>
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
		/// ��ȡsql���Ͳ���,������Ǿͷ���null;
		/// </summary>
		public SqlCommand SqlCommand
		{
			get{return (this._this as SqlCommand);}
		}
		/// <summary>
		/// ��ȡOleDb���Ͳ���,������Ǿͷ���null;
		/// </summary>
		public OleDbCommand OleDbCommand
		{
			get{return (this._this as OleDbCommand);}
		}
		/// <summary>
		/// ��ȡoralce��ODP��������,������Ǿͷ���null;
		/// </summary>
		public OracleCommand OracleCommand
		{
			get{return (this._this as OracleCommand);}
		}

		#endregion

		#region IDbCommand ��Ա

		public void Cancel()
		{
			// TODO:  ��� DBCommand.Cancel ʵ��
			this._this.Cancel();
		}

		public void Prepare()
		{
			// TODO:  ��� DBCommand.Prepare ʵ��
			this._this.Prepare();
		}

		public System.Data.CommandType CommandType
		{
			get
			{
				// TODO:  ��� DBCommand.CommandType getter ʵ��
				return this._this.CommandType;
			}
			set
			{
				// TODO:  ��� DBCommand.CommandType setter ʵ��
				this._this.CommandType=value;
			}
		}

		public IDataReader ExecuteReader(System.Data.CommandBehavior behavior)
		{
			// TODO:  ��� DBCommand.ExecuteReader ʵ��
			return this._this.ExecuteReader(behavior);
		}

		IDataReader System.Data.IDbCommand.ExecuteReader()
		{
			// TODO:  ��� DBCommand.System.Data.IDbCommand.ExecuteReader ʵ��
			return this._this.ExecuteReader();
		}

		public object ExecuteScalar()
		{
			// TODO:  ��� DBCommand.ExecuteScalar ʵ��
			return this._this.ExecuteScalar();
		}

		public int ExecuteNonQuery()
		{
			// TODO:  ��� DBCommand.ExecuteNonQuery ʵ��
			return this._this.ExecuteNonQuery();
		}

		public int CommandTimeout
		{
			get
			{
				// TODO:  ��� DBCommand.CommandTimeout getter ʵ��
				return this._this.CommandTimeout;
			}
			set
			{
				// TODO:  ��� DBCommand.CommandTimeout setter ʵ��
				this._this.CommandTimeout=value;
			}
		}

		/// <summary>
		/// ���غ���
		/// </summary>
		/// <returns></returns>
		public IDbDataParameter CreateParameter()
		{
			// TODO:  ��� DBCommand.CreateParameter ʵ��
			return this._this.CreateParameter();
		}

		public IDbConnection Connection
		{
			get
			{
				// TODO:  ��� DBCommand.Connection getter ʵ��
				return this._this.Connection;
			}
			set
			{
				// TODO:  ��� DBCommand.Connection setter ʵ��
				this._this.Connection=value;
			}
		}

		public System.Data.UpdateRowSource UpdatedRowSource
		{
			get
			{
				// TODO:  ��� DBCommand.UpdatedRowSource getter ʵ��
				return this._this.UpdatedRowSource;
			}
			set
			{
				// TODO:  ��� DBCommand.UpdatedRowSource setter ʵ��
				this._this.UpdatedRowSource=value;
			}
		}

		public string CommandText
		{
			get
			{
				// TODO:  ��� DBCommand.CommandText getter ʵ��
				return this._this.CommandText;
			}
			set
			{
				// TODO:  ��� DBCommand.CommandText setter ʵ��
				this._this.CommandText=value;
			}
		}

		/// <summary>
		/// �ض�������
		/// </summary>
		public IDataParameterCollection Parameters
		{
			get
			{
				// TODO:  ��� DBCommand.Parameters getter ʵ��
				return this._this.Parameters;
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				// TODO:  ��� DBCommand.Transaction getter ʵ��
				return this._this.Transaction;
			}
			set
			{
				// TODO:  ��� DBCommand.Transaction setter ʵ��
				this._this.Transaction=value;
			}
		}

		#endregion

		#region IDisposable ��Ա

		public void Dispose()
		{
			// TODO:  ��� DBCommand.Dispose ʵ��
			this._this.Dispose();
		}

		#endregion

	}
}
