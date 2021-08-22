﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MBSCore.Beject
{
	internal class InjectModuleList : InjectModule
	{
		private static readonly MethodInfo CopyListMethodInfo = typeof(InjectModuleList).GetMethod(nameof(CopyList),
			BindingFlags.Static | BindingFlags.NonPublic);

		public override bool TryConvertToSimple(Type complexType, out Type simpleType)
		{
			if (ValidateType(ref complexType))
			{
				simpleType = complexType.GetGenericArguments()[0];
				return true;
			}

			simpleType = default;
			return false;
		}

		public override bool TrySetValueIntoField(object fillingObject, FieldInfo fieldInfo, List<object> values)
		{
			if (!TryConvertToSimple(fieldInfo.FieldType, out Type genericArgument))
			{
				return false;
			}
  
			fieldInfo.SetValue(fillingObject,
				CopyListMethodInfo.MakeGenericMethod(genericArgument).Invoke(this, new object[] {values}));
			return true;
		}

		public override bool TrySetValueIntoProperty(object fillingObject, PropertyInfo propertyInfo, List<object> values)
		{
			if (!TryConvertToSimple(propertyInfo.PropertyType, out Type genericArgument))
			{
				return false;
			}
  
			propertyInfo.SetValue(fillingObject,
				CopyListMethodInfo.MakeGenericMethod(genericArgument).Invoke(this, new object[] {values}));
			return true;
		}

		private static bool ValidateType(ref Type type)
		{
			return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>) ||
			                                  type.GetGenericTypeDefinition() == typeof(List<>));
		}

		private static List<T> CopyList<T>(IList values)
		{
			List<T> copiedList = new List<T>();
			int valueCount = values.Count;
			for (int i = 0; i < valueCount; i++)
			{
				if (values[i] is T tValue)
				{
					copiedList.Add(tValue);
				}
			}

			return copiedList;
		}
	}
}