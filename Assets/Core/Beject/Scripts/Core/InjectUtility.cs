using System;
using System.Collections.Generic;
using System.Reflection;

namespace MBSCore.Beject
{
	internal static class InjectUtility
	{
		public const BindingFlags FOUND_BINDING_FLAGS =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		
		private static readonly InjectModule[] Modules =
		{
			new InjectModuleArray(),
			new InjectModuleList(),
		};

		private static readonly int ModulesCount = Modules.Length;

		public static bool TryGetSimpleType(Type complexType, out Type simpleType)
		{
			for (int i = 0; i < ModulesCount; i++)
			{
				if (Modules[i].TryConvertToSimple(complexType, out simpleType))
				{
					return true;
				}
			}

			if (complexType.IsClass || complexType.IsInterface)
			{
				simpleType = complexType;
				return true;
			}

			simpleType = default;
			return false;
		}

		public static bool TrySetValueIntoField(object fillingObject, FieldInfo fieldInfo, List<object> values)
		{
			Type fieldType = fieldInfo.FieldType;

			for (int i = 0; i < ModulesCount; i++)
			{
				if (Modules[i].TrySetValueIntoField(fillingObject, fieldInfo, values))
				{
					return true;
				}
			}

			if (fieldType.IsClass || fieldType.IsInterface)
			{
				fieldInfo.SetValue(fillingObject, values.Count > 0 ? values[0] : default);
				return true;
			}

			return false;
		}
		
		public static bool TrySetValueIntoProperty(object fillingObject, PropertyInfo propertyInfo, List<object> values)
		{
			Type propertyType = propertyInfo.PropertyType;

			for (int i = 0; i < ModulesCount; i++)
			{
				if (Modules[i].TrySetValueIntoProperty(fillingObject, propertyInfo, values))
				{
					return true;
				}
			}

			if (propertyType.IsClass || propertyType.IsInterface)
			{
				propertyInfo.SetValue(fillingObject, values.Count > 0 ? values[0] : default);
				return true;
			}

			return false;
		}

#if UNITY_EDITOR
		public static void FindInjectData(InjectMap map, Func<Type, UnityEngine.Object[]> findObjectsFunction,
			out SerializedInjectCell[] tempInjectObjects, out SerializedFillingCell[] tempFillingObject)
		{
			if (map == null)
			{
				throw new ArgumentException("Map is null");
			}
			
			map.UpdateInjectMap();

			Dictionary<UnityEngine.Object, List<Type>> injectDictionary =
				new Dictionary<UnityEngine.Object, List<Type>>();
			Dictionary<UnityEngine.Object, InjectMembersContainer> tempFillingDictionary =
				new Dictionary<UnityEngine.Object, InjectMembersContainer>();
			Type[] usingTypes = map.UsingTypes;
			for (int i = 0; i < usingTypes.Length; i++)
			{
				Type usingType = usingTypes[i];
				bool isInject = map.TypeIsInject(usingType);
				bool isFilling = map.TypeIsFilling(usingType);
				UnityEngine.Object[] foundObjects = findObjectsFunction.Invoke(usingType);
				for (int j = 0; j < foundObjects.Length; j++)
				{
					UnityEngine.Object foundObject = foundObjects[j];
					if (isInject)
					{
						if (!injectDictionary.TryGetValue(foundObject, out List<Type> injectTypes))
						{
							injectTypes = new List<Type>();
							injectDictionary.Add(foundObject, injectTypes);
						}

						if (!injectTypes.Contains(usingType))
						{
							injectTypes.Add(usingType);
						}
					}

					if (isFilling && !tempFillingDictionary.ContainsKey(foundObject) &&
					    map.TryGetFieldInfos(foundObject.GetType(), out InjectMembersContainer membersContainer))
					{
						tempFillingDictionary.Add(foundObject, membersContainer);
					}
				}
			}

			int k = -1;
			int injectCount = injectDictionary.Count;
			tempInjectObjects = new SerializedInjectCell[injectCount];
			foreach (KeyValuePair<UnityEngine.Object, List<Type>> injectPair in injectDictionary)
			{
				k++;
				tempInjectObjects[k] = new SerializedInjectCell(injectPair.Key, injectPair.Value.ToArray());
			}

			k = -1;
			int fillingCount = tempFillingDictionary.Count;
			tempFillingObject = new SerializedFillingCell[fillingCount];
			foreach (KeyValuePair<UnityEngine.Object, InjectMembersContainer> fillingPair in tempFillingDictionary)
			{
				k++;
				tempFillingObject[k] = new SerializedFillingCell(fillingPair.Key, fillingPair.Value);
			}
		}
#endif
	}
}