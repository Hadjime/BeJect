using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MBSCore.Beject
{
	public partial class SceneContext
	{
#if UNITY_EDITOR
		private static readonly Type ComponentType = typeof(Component);
		
		[SerializeField] private InjectMap map;
		
		[ContextMenu("Find MonoBehaviours for InjectSystem")]
		private void FindMonoBehavioursForInjectSystem()
		{
			if (map == null)
			{
				throw new ArgumentException("Map is null");
			}
			
			map.UpdateInjectMap();
			InjectUtility.FindInjectData(map, FindObjectsInScene, out injectObjects, out fillingObjects);
			EditorUtility.SetDirty(this);
		}

		private UnityEngine.Object[] FindObjects(Type searchType)
		{
			Type unityType = typeof(UnityEngine.Object);
			Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
			List<Type> foundTypes = new List<Type>();
			for (int i = 0; i < allTypes.Length; i++)
			{
				Type type = allTypes[i];
				if (unityType.IsAssignableFrom(type) && searchType.IsAssignableFrom(type))
				{
					foundTypes.Add(type);
				}
			}

			List<UnityEngine.Object> foundObject = new List<UnityEngine.Object>();
			for (int i = 0; i < foundTypes.Count; i++)
			{
				UnityEngine.Object[] foundArray = FindObjectsInScene(foundTypes[i]);
				foundObject.AddRange(foundArray);
			}

			return foundObject.ToArray();
		}

		private static UnityEngine.Object[] FindObjectsInScene(Type searchType)
		{
			if (!searchType.IsInterface && ComponentType.IsAssignableFrom(searchType) == false)
			{
				return Array.Empty<UnityEngine.Object>();
			}

			List<UnityEngine.Object> foundList = new List<UnityEngine.Object>();
			List<GameObject> rootList = new List<GameObject>();
			UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			scene.GetRootGameObjects(rootList);
			for (int i = rootList.Count - 1; i >= 0; i--)
			{
				foundList.AddRange(rootList[i].GetComponentsInChildren(searchType, true));
			}

			return foundList.ToArray();
		}
#endif
	}
}