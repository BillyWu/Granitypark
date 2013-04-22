using System;
using System.Collections;
using System.Collections.Specialized;


namespace Estar.Common.Tools
{
	/// <summary>
	/// string键的object列表
	/// </summary>
	public class NameObjectList : NameObjectCollectionBase  
	{

		private DictionaryEntry _de = new DictionaryEntry();

		#region 构造函数
		public NameObjectList()  
		{
		}

		/// <summary>
		/// 根据已有字典建立列表
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

		#region 接口实现
		/// <summary>
		/// 读取指定位置的键值对
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
		/// 使用字符键值直接读写对应值
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
		/// 返回所有键值
		/// </summary>
		public String[] AllKeys  
		{
			get  
			{
				return( this.BaseGetAllKeys() );
			}
		}

		/// <summary>
		/// 返回所有的值
		/// </summary>
		public Array AllValues  
		{
			get  
			{
				return( this.BaseGetAllValues() );
			}
		}

		/// <summary>
		/// 返回所有字符类型的值
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
