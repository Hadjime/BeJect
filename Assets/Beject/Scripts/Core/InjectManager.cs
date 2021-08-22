using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBSCore.Beject
{
	[AddComponentMenu("Beject/SceneSystem/Inject Manager")]
	public class InjectManager : MonoBehaviour
	{
		[SerializeField] private ScriptableObjectContext scriptableObjectContext;
		
		private readonly Dictionary<Type, List<object>> injectDictionary = new Dictionary<Type, List<object>>();

		private readonly Dictionary<object, InjectMembersContainer> fillingDictionary =
			new Dictionary<object, InjectMembersContainer>();

		private void Awake()
		{
			ContextHandler.AddHandler += ContextAddHandler;
			ContextHandler.RemoveHandler += ContextRemoveHandler;

			IEnumerable<IContext> contextEnumerable = ContextHandler.GetCurrentContexts();
			foreach (IContext context in contextEnumerable)
			{
				AddInjectObject(context);
				AddFillingObject(context);
			}

			FillObjects();
		}

		private void OnDestroy()
		{
			ContextHandler.AddHandler -= ContextAddHandler;
			ContextHandler.RemoveHandler -= ContextRemoveHandler;
		}

		private void ContextAddHandler(IContext context)
		{
			AddInjectObject(context);
			AddFillingObject(context);
			FillObjects();
		}

		private void ContextRemoveHandler(IContext context)
		{
			RemoveInjectObject(context);
			RemoveFillingObject(context);
			FillObjects();
		}

		private void AddInjectObject(IContext context)
		{
			InjectCell[] contextInjectObject = context.GetInjectObjects();
			int contextInjectCount = contextInjectObject.Length;
			for (int i = 0; i < contextInjectCount; i++)
			{
				InjectCell injectCell = contextInjectObject[i];
				Type[] injectTypes = injectCell.InjectTypes;
				int injectTypeCount = injectTypes.Length;
				for (int j = 0; j < injectTypeCount; j++)
				{
					Type injectType = injectTypes[j];
					if (!injectDictionary.TryGetValue(injectType, out List<object> objects))
					{
						objects = new List<object>();
						injectDictionary.Add(injectType, objects);
					}

					objects.Add(injectCell.InjectObject);
				}
			}
		}

		private void RemoveInjectObject(IContext context)
		{
			InjectCell[] contextInjectObject = context.GetInjectObjects();
			int contextInjectCount = contextInjectObject.Length;
			for (int i = 0; i < contextInjectCount; i++)
			{
				InjectCell injectCell = contextInjectObject[i];
				Type[] injectTypes = injectCell.InjectTypes;
				int injectTypeCount = injectTypes.Length;
				for (int j = 0; j < injectTypeCount; j++)
				{
					Type injectType = injectTypes[j];
					if (!injectDictionary.TryGetValue(injectType, out List<object> objects))
					{
						continue;
					}

					objects.Remove(injectCell.InjectObject);
				}
			}
		}

		private void AddFillingObject(IContext context)
		{
			IEnumerable<KeyValuePair<Object, InjectMembersContainer>> contextFillingObjects = context.GetFillingObjects();
			foreach (KeyValuePair<Object, InjectMembersContainer> fillingCell in contextFillingObjects)
			{
				fillingDictionary.Add(fillingCell.Key, fillingCell.Value);
			}
		}

		private void RemoveFillingObject(IContext context)
		{
			IEnumerable<KeyValuePair<Object, InjectMembersContainer>> contextFillingObjects = context.GetFillingObjects();
			foreach (KeyValuePair<Object, InjectMembersContainer> fillingCell in contextFillingObjects)
			{
				fillingDictionary.Remove(fillingCell.Key);
			}
		}

		private void FillObjects()
		{
			foreach (KeyValuePair<object, InjectMembersContainer> fillingCell in fillingDictionary)
			{
				object fillingObject = fillingCell.Key;
				InjectMembersContainer membersContainer = fillingCell.Value;

				int injectFieldCount = membersContainer.FieldInfos.Count;
				for (int i = 0; i < injectFieldCount; i++)
				{
					FieldInfo injectField =  membersContainer.FieldInfos[i];
					InjectFieldProcedure(fillingObject, injectField, injectDictionary);
				}
				
				int injectPropertyCount = membersContainer.PropertyInfos.Count;
				for (int i = 0; i < injectPropertyCount; i++)
				{
					PropertyInfo injectProperty =  membersContainer.PropertyInfos[i];
					InjectPropertyProcedure(fillingObject, injectProperty, injectDictionary);
				}
			}
		}

		private static void InjectFieldProcedure(object fillingObject, FieldInfo injectField, Dictionary<Type, List<object>> injectDictionary)
		{
			Type fieldType = injectField.FieldType;
			if (!InjectUtility.TryGetSimpleType(fieldType, out Type injectType))
			{
				// ToDo Description
				throw new Exception();
			}

			if (!injectDictionary.TryGetValue(injectType, out List<object> injectObjects))
			{
				injectObjects = new List<object>();
				injectDictionary.Add(injectType, injectObjects);
			}
						
			if (!InjectUtility.TrySetValueIntoField(fillingObject, injectField, injectObjects))
			{
				// ToDo Description
				throw new Exception();
			}
		}
		
		private static void InjectPropertyProcedure(object fillingObject, PropertyInfo injectProperty, Dictionary<Type, List<object>> injectDictionary)
		{
			Type propertyType = injectProperty.PropertyType;
			if (!InjectUtility.TryGetSimpleType(propertyType, out Type injectType))
			{
				// ToDo Description
				throw new Exception();
			}

			if (!injectDictionary.TryGetValue(injectType, out List<object> injectObjects))
			{
				injectObjects = new List<object>();
				injectDictionary.Add(injectType, injectObjects);
			}
						
			if (!InjectUtility.TrySetValueIntoProperty(fillingObject, injectProperty, injectObjects))
			{
				// ToDo Description
				throw new Exception();
			}
		}
	}
}