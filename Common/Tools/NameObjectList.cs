using System;
using System.Collections;
using System.Collections.Specialized;


namespace Estar.Common.Tools
{
	/// <summary>
	/// string����object�б�
	/// </summary>
	public class NameObjectList : NameObjectCollectionBase  
	{

		private DictionaryEntry _de = new DictionaryEntry();

		#region ���캯��
		public NameObjectList()  
		{
		}

		/// <summary>
		/// ���������ֵ佨���б�
		/// </summary>
		/// <param name="d"></param>
		/// <param name="bReadOnly"></param>
		public NameObjectList( IDictionary d, Boolean bReadOnly )  
		{
			foreach ( DictionaryEntry de in d )  
			{
				this.BaseAdd( (String) de.Key, de.Value );
			}
			this.IsReadOnly = bReadOnly;
		}
		#endregion

		#region �ӿ�ʵ��
		/// <summary>
		/// ��ȡָ��λ�õļ�ֵ��
		/// </summary>
		public Object this[ int index ]  
		{
			get  
			{
				_de.Key = this.BaseGetKey(index);
				_de.Value = this.BaseGet(index);
				return _de.Value;
			}
			set{this.BaseSet(index,value);}
		}

		/// <summary>
		/// ʹ���ַ���ֱֵ�Ӷ�д��Ӧֵ
		/// </summary>
		public Object this[ String key ]  
		{
			get  
			{
				return( this.BaseGet( key ) );
			}
			set  
			{
				this.BaseSet( key, value );
			}
		}

		/// <summary>
		/// �������м�ֵ
		/// </summary>
		public String[] AllKeys  
		{
			get  
			{
				return( this.BaseGetAllKeys() );
			}
		}

		/// <summary>
		/// �������е�ֵ
		/// </summary>
		public Array AllValues  
		{
			get  
			{
				return( this.BaseGetAllValues() );
			}
		}

		/// <summary>
		/// ���������ַ����͵�ֵ
		/// </summary>
		public String[] AllStringValues  
		{
			get  
			{
				return( (String[]) this.BaseGetAllValues( Type.GetType( "System.String" ) ) );
			}
		}

		public Boolean HasKeys  
		{
			get  
			{
				return( this.BaseHasKeys() );
			}
		}

		public void Add( String key, Object value )  
		{
			this.BaseAdd( key, value );
		}

		public void Remove( String key )  
		{
			this.BaseRemove( key );
		}

		public void Remove( int index )  
		{
			this.BaseRemoveAt( index );
		}

		public void Clear()  
		{
			this.BaseClear();
		}

		#endregion

	}
}
