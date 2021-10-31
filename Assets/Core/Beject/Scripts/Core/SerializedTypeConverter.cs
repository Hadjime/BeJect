using System;
using System.Collections.Generic;
using System.Reflection;

namespace MBSCore.Beject
{
	public static class SerializedTypeConverter
	{
		public static string[] GetPaths<T>(IReadOnlyList<T> members) where T : MemberInfo
		{
			int count = members.Count;
			string[] paths = new string[count];
			for (int i = 0; i < count; i++)
			{
				paths[i] = members[i].Name;
			}

			return paths;
		}

		public static FieldInfo[] GetFieldInfos(Type type, string[] fieldPaths)
		{
			int fieldCount = fieldPaths.Length;
			FieldInfo[] fieldInfos = new FieldInfo[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				FieldInfo fieldInfo = null;
				Type processingType = type;
				while (processingType != null && fieldInfo == null)
				{
					fieldInfo = processingType.GetField(fieldPaths[i], InjectUtility.FOUND_BINDING_FLAGS);
					processingType = processingType.BaseType;
				}

				fieldInfos[i] = fieldInfo;
			}

			return fieldInfos;
		}
		
		public static PropertyInfo[] GetPropertyInfos(Type type, string[] propertyPaths)
		{
			int propertyCount = propertyPaths.Length;
			PropertyInfo[] propertyInfos = new PropertyInfo[propertyCount];
			for (int i = 0; i < propertyCount; i++)
			{
				PropertyInfo propertyInfo = null;
				Type processingType = type;

				while (processingType != null && propertyInfo == null)
				{
					propertyInfo = processingType.GetProperty(propertyPaths[i], InjectUtility.FOUND_BINDING_FLAGS);
					processingType = processingType.BaseType;
				}

				propertyInfos[i] = propertyInfo;
			}

			return propertyInfos;
		}
	}
}