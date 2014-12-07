using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace PDStat
{
	public static class EnumHelper
	{
		//copied from http://blog.spontaneouspublicity.com/associating-strings-with-enums-in-c
		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes != null && attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}

		public static List<T> EnumToList<T>()
		{
			Type enumType = typeof(T);

			// Can't use generic type constraints on value types,
			// so have to do check like this
			if (enumType.BaseType != typeof(Enum))
				throw new ArgumentException("T must be of type System.Enum");

			Array enumValArray = Enum.GetValues(enumType);
			List<T> enumValList = new List<T>(enumValArray.Length);

			foreach (int val in enumValArray)
			{
				enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
			}

			return enumValList;
		}
	}

	public static class HelperMethods
	{
		//doesn't work?
		public static string GetDescription<T>(T value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			if (fi == null) return value.ToString();

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes != null && attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}
	}
}
