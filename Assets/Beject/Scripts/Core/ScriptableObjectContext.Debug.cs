#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#endif

namespace MBSCore.Beject
{
    public partial class ScriptableObjectContext
    {
#if UNITY_EDITOR
        private const string AssetsFilter = "t:ScriptableObject";

        [SerializeField] private InjectMap map;

        [SerializeField] private DefaultAsset[] foldersForSearch;

        [ContextMenu("Find ScrptableObjects")]
        private void FindScrptableObjects()
        {
            List<ScriptableObject> foundScriptableObjects = GetScriptableObjectsInFolders();

            Func<Type, UnityEngine.Object[]> findObjectsFunction = (Type type) =>
            {
                List<UnityEngine.Object> foundObjects = new List<UnityEngine.Object>();
                for (int i = 0; i < foundScriptableObjects.Count; i++)
                {
                    ScriptableObject obj = foundScriptableObjects[i];
                    if (type.IsInstanceOfType(obj))
                    {
                        foundObjects.Add(obj);
                    }
                }

                return foundObjects.ToArray();
            };
            
            InjectUtility.FindInjectData(map, findObjectsFunction, out injectObjects, out fillingObjects);
        }

        private List<ScriptableObject> GetScriptableObjectsInFolders()
        {
            List<ScriptableObject> foundObjects = new List<ScriptableObject>();
            List<string> foundPaths = new List<string>();

            for (int i = 0; i < foldersForSearch.Length; i++)
            {
                DefaultAsset folderForSearch = foldersForSearch[i];
                if (folderForSearch == null)
                {
                    continue;
                }

                string searchPath = AssetDatabase.GetAssetPath(folderForSearch);

                if (!string.IsNullOrEmpty(searchPath) && !foundPaths.Contains(searchPath))
                {
                    foundPaths.Add(searchPath);
                }
            }

            string[] dataId = AssetDatabase.FindAssets(AssetsFilter, foundPaths.ToArray());
            for (int i = 0; i < dataId.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(dataId[i]);
                UnityEngine.Object[] data = AssetDatabase.LoadAllAssetsAtPath(path);

                for (int j = 0; j < data.Length; j++)
                {
                    if (data[j] is ScriptableObject scriptableObject && !foundObjects.Contains(scriptableObject))
                    {
                        foundObjects.Add(scriptableObject);
                    }
                }
            }

            return foundObjects;
        }
#endif
    }
}