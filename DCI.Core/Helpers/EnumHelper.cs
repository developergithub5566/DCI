using DCI.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Core.Helpers
{
	public class EnumHelper
	{
		//EnumApplicantStatus status = EnumApplicantStatus.DoneInitialInterview;
		//string description = EnumHelper.GetEnumDescription(status);

		//public static string GetEnumDescription(Enum value)
		//{
		//	FieldInfo fi = value.GetType().GetField(value.ToString());

		//	DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

		//	if (attributes != null && attributes.Length > 0)
		//	{
		//		return attributes[0].Description;
		//	}
		//	else
		//	{
		//		return value.ToString();
		//	}
		//}

		public static string GetEnumDescriptionByTypeValue(Type enumType, object enumValue)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Provided type must be an enum.", nameof(enumType));
			}

			string enumName = Enum.GetName(enumType, enumValue);
			if (enumName == null)
			{
				return null; // Value not found in enum
			}

			FieldInfo fi = enumType.GetField(enumName);
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes != null && attributes.Length > 0)
			{
				return attributes[0].Description;
			}
			else
			{
				return enumName;
			}
		}

	}
}
