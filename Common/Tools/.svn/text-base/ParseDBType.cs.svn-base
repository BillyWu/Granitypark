using System;

namespace Estar.Common.Tools
{
	/// <summary>
	/// 通常的数据类型
	/// </summary>
	public enum DBTypeCommon
	{
		String,
		Int,
		Decimal,
		DateTime,
		Bit,
        Bool,
        Guid
	}


	/// <summary>
	/// 解析识别数据类型,辅助处理对应数据库的数据类型
	/// </summary>
	public class ParseDBType
	{
		public ParseDBType()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		public static DBTypeCommon	ParseTypeFromStr(string	type)
		{
			switch(type.ToLower())
			{
				case "string":
					return DBTypeCommon.String;
				case "int":
				case "int16":
				case "int32":
				case "int64":
					return DBTypeCommon.Int;
				case "decimal":
				case "double":
				case "float":
				case "real":
				case "single":
					return DBTypeCommon.Decimal;
				case "date":
				case "time":
				case "datetime":
					return DBTypeCommon.DateTime;
				case "bit":
				case "bool":
				case "boolean":
					return DBTypeCommon.Bit;
				default:
					return DBTypeCommon.String;
			}
		}
	}
}
