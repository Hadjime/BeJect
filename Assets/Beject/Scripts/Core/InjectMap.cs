using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MBSCore.Beject
{
	public class InjectMap : ScriptableObject
	{
		[SerializeField] private SerializedType[] usingTypes;
		[SerializeField] private SerializedType[] injectTypes;
		[SerializeField] private SerializedFillingInfo[] fillingInfos;

		[NonSerialized] private Type[] _usingTypes;
		[NonSerialized] private HashSet<Type> _injectTypeHash;
		[NonSerialized] private HashSet<Type> _fillingTypeHash;
		[NonSerialized] private Dictionary<Type, InjectMembersContainer> _fillingDictionary;

		public Type[] UsingTypes
		{
			get
			{
				if (_usingTypes == null)
				{
					int count = usingTypes.Length;
					_usingTypes = new Type[count];
					for (int i = 0; i < count; i++)
					{
						_usingTypes[i] = usingTypes[i].Type;
					}
				}

				return _usingTypes;
			}
		}

		private Dictionary<Type, InjectMembersContainer> FillingDictionary
		{
			get
			{
				if (_fillingDictionary == null)
				{
					_fillingDictionary = new Dictionary<Type, InjectMembersContainer>();
					int cellCount = fillingInfos.Length;
					for (int i = 0; i < cellCount; i++)
					{
						FillingInfo fillingInfo = fillingInfos[i].GetFillingInfo();
						_fillingDictionary.Add(fillingInfo.FillingType, fillingInfo.GetMembers());
					}

					foreach (KeyValuePair<Type, InjectMembersContainer> pair in _fillingDictionary)
					{
						AddBaseTypeInjectMembers(pair.Value, pair.Key.BaseType, _fillingDictionary);
					}
				}

				return _fillingDictionary;
			}
		}

		private HashSet<Type> InjectTypeHash
		{
			get
			{
				if (_injectTypeHash == null)
				{
					int count = injectTypes.Length;
					_injectTypeHash = new HashSet<Type>();
					for (int i = 0; i < count; i++)
					{
						_injectTypeHash.Add(injectTypes[i].Type);
					}
				}

				return _injectTypeHash;
			}
		}

		private HashSet<Type> FillingTypeHash
		{
			get
			{
				if (_fillingTypeHash == null)
				{
					_fillingTypeHash = new HashSet<Type>();
					foreach (Type fillingType in FillingDictionary.Keys)
					{
						_fillingTypeHash.Add(fillingType);
					}
				}

				return _fillingTypeHash;
			}
		}

		public bool TryGetFieldInfos(Type fillingType, out InjectMembersContainer membersContainer)
		{
			return FillingDictionary.TryGetValue(fillingType, out membersContainer);
		}

		public bool TypeIsFilling(Type type)
		{
			return FillingTypeHash.Contains(type);
		}

		public bool TypeIsInject(Type type)
		{
			return InjectTypeHash.Contains(type);
		}
		
		private void AddBaseTypeInjectMembers(InjectMembersContainer container, Type baseType,
			IReadOnlyDictionary<Type, InjectMembersContainer> dictionary)
		{
			while (!ReferenceEquals(baseType, null))
			{
				if (dictionary.TryGetValue(baseType, out InjectMembersContainer baseContainer))
				{
					container.FieldInfos.AddRange(baseContainer.FieldInfos);
					container.PropertyInfos.AddRange(baseContainer.PropertyInfos);
				}

				baseType = baseType.BaseType;
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Update Inject Map")]
		public void UpdateInjectMap()
		{
			CalculateFillingInfos(out List<FillingInfo> tempInjectInfos);

			//ToDo Next method exist error
			CalculateMapCollections(tempInjectInfos, out List<Type> tempInjectTypes, out List<Type> tempUsingTypes,
				out SerializedFillingInfo[] newFillingInfos);
			
			fillingInfos = newFillingInfos;

			int injectTypesCount = tempInjectTypes.Count;
			injectTypes = new SerializedType[injectTypesCount];
			for (int i = 0; i < injectTypesCount; i++)
			{
				injectTypes[i] = new SerializedType(tempInjectTypes[i]);
			}

			int usingTypesCount = tempUsingTypes.Count;
			usingTypes = new SerializedType[usingTypesCount];
			for (int i = 0; i < usingTypesCount; i++)
			{
				usingTypes[i] = new SerializedType(tempUsingTypes[i]);
			}

			_usingTypes = null;
			_injectTypeHash = null;
			_fillingTypeHash = null;
			_fillingDictionary = null;
			EditorUtility.SetDirty(this);
		}
		
		private static void CalculateFillingInfos(out List<FillingInfo> fillingInfos)
		{
			fillingInfos = new List<FillingInfo>();
			List<FieldInfo> injectFields = new List<FieldInfo>();
			List<PropertyInfo> injectProperties = new List<PropertyInfo>();
			Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < allTypes.Length; i++)
			{
				Type type = allTypes[i];
				if (!type.IsClass || type.IsGenericType || type.IsAbstract)
				{
					continue;
				}

				injectFields.Clear();
				injectProperties.Clear();
				FoundFields(type, injectFields);
				FoundProperties(type, injectProperties);
				if (injectFields.Count > 0 || injectProperties.Count > 0)
				{
					fillingInfos.Add(new FillingInfo(type, injectFields.ToArray(), injectProperties.ToArray()));
				}
			}
		}

		private static void FoundFields(Type type, List<FieldInfo> fieldsList)
		{
			FieldInfo[] fields = type.GetFields(InjectUtility.FoundBindingFlags);
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo field = fields[j];
				InjectFieldAttribute fieldAttribute = field.GetCustomAttribute<InjectFieldAttribute>(true);
				if (fieldAttribute != null && !ContainsName(fieldsList, field))
				{
					fieldsList.Add(field);
				}
			}

			Type baseType = type.BaseType;
			if (baseType != null)
			{
				FoundFields(baseType, fieldsList);
			}
		}

		private static void FoundProperties(Type type, List<PropertyInfo> propertyList)
		{
			PropertyInfo[] properties = type.GetProperties(InjectUtility.FoundBindingFlags);
			for (int j = 0; j < properties.Length; j++)
			{
				PropertyInfo propertyInfo = properties[j];
				InjectPropertyAttribute propertyAttribute =
					propertyInfo.GetCustomAttribute<InjectPropertyAttribute>(true);
				if (propertyAttribute != null && !ContainsName(propertyList, propertyInfo))
				{
					propertyList.Add(propertyInfo);
				}
			}
			
			Type baseType = type.BaseType;
			if (baseType != null)
			{
				FoundProperties(baseType, propertyList);
			}
		}

		private static void CalculateMapCollections(List<FillingInfo> fillingInfos, out List<Type> injectTypes,
			out List<Type> usingTypes, out SerializedFillingInfo[] fillingArray)
		{
			int cellsCount = fillingInfos.Count;
			fillingArray = new SerializedFillingInfo[cellsCount];
			injectTypes = new List<Type>();
			usingTypes = new List<Type>();
			for (int i = 0; i < cellsCount; i++)
			{
				FieldInfo[] fieldInfos = fillingInfos[i].FieldInfos;
				PropertyInfo[] propertyInfos = fillingInfos[i].PropertyInfos;
				Type fillingType = fillingInfos[i].FillingType;
				fillingArray[i] = new SerializedFillingInfo(fillingType, fieldInfos, propertyInfos);

				if (!usingTypes.Contains(fillingType))
				{
					usingTypes.Add(fillingType);
				}

				for (int j = 0; j < fieldInfos.Length; j++)
				{
					CheckType(fieldInfos[j].FieldType, injectTypes, usingTypes);
				}

				for (int j = 0; j < propertyInfos.Length; j++)
				{
					CheckType(propertyInfos[j].PropertyType, injectTypes, usingTypes);
				}
			}
		}

		private static void CheckType(Type complexType, List<Type> injectTypes, List<Type> usingTypes)
		{
			if (!InjectUtility.TryGetSimpleType(complexType, out Type fieldType))
			{
				//ToDo Description
				throw new ArgumentException();
			}

			if (!injectTypes.Contains(fieldType))
			{
				injectTypes.Add(fieldType);
			}
					
			if (!usingTypes.Contains(fieldType))
			{
				usingTypes.Add(fieldType);
			}
		}

		private static bool ContainsName<T>(List<T> fieldsList, T fieldInfo) where T : MemberInfo
		{
			string fieldName = fieldInfo.Name;
			for (int i = fieldsList.Count - 1; i >= 0; i--)
			{
				if (fieldsList[i].Name == fieldName)
				{
					return true;
				}
			}

			return false;
		}
#endif
	}
}