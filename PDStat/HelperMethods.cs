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

	public static class Helpers
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

		public static bool IsOfFFamily(string game)
		{
			return (game == PDFV || game == PDFP || game == PDF2V || game == PDF2P);
		}

		public static readonly string PD1 = "Project Diva (1)";
		public static readonly string PD2 = "Project Diva 2nd";
		public static readonly string PDX = "Project Diva Extend";

		public static readonly string PDDT = "Project Diva DT";
		public static readonly string PDDT2 = "Project Diva DT 2nd";
		public static readonly string PDDTX = "Project Diva DT Extend";

		public static readonly string PDFV = "Project Diva f (Vita)";
		public static readonly string PDFP = "Project Diva F (PS3)";
		public static readonly string PDF2V = "Project Diva f 2nd (Vita)";
		public static readonly string PDF2P = "Project Diva F 2nd (PS3)";
	}
}
