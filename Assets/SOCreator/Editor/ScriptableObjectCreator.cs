using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MBSCore.Editor
{
    public class ScriptableObjectCreator : CustomEditorWindow
    {
        private const string ENTER_MANAGER_NAME_LABEL = "Enter Manager Name:";
        private const string WIZARD_TITLE = "Scriptable Object Creator";
        private const string CREATE_BUTTON_NAME = "Create";
        private const string ASSET_EXPANSION = ".asset";
        private const int SEARCH_COUNT = 20;
        private const int MAX_VALIDATE_NAME_COUNT = 100;
        
        private static readonly MethodInfo GetActiveFolderPathMethod = typeof(ProjectWindowUtil).GetMethod(
            "TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly object[] GetActiveFolderPathMethodArguments = {null};

        private static GUIStyle BoldStile => EditorStyles.boldLabel;
        
        [SerializeField] private string searchType;
        
        private Type[] foundTypes = null;
        private Type createType;
        
        protected override void DrawGUI()
        {
            DrawHorizontalBox(DrawSearchPanel);
            DrawVerticalBox(TryDrawTypes);
        }

        [MenuItem("Tools/ScriptableObject/Creator")]
        private static void ShowWindow()
        {
            GetWindow<ScriptableObjectCreator>(WIZARD_TITLE);
        }
        
        private bool Validate(Type type)
        {
            string typeName = type.Name;
            string upperName = string.Concat(
                from x in typeName
                where char.IsUpper(x)
                select x);

            return typeof(ScriptableObject).IsAssignableFrom(type) &&
                   (typeName.StartsWith(searchType, StringComparison.OrdinalIgnoreCase) ||
                    upperName.StartsWith(searchType, StringComparison.OrdinalIgnoreCase));
        }
        
        private void FillWithValidTypes(ref int number)
        {
            IEnumerable<Type> types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where Validate(t)
                orderby t.Name
                select t;

            foreach (var type in types)
            {
                if (number >= SEARCH_COUNT)
                {
                    break;
                }

                foundTypes[number] = type;
                number++;
            }
        }
        
        private void FindTypes()
        {
            int number = 0;

            if (!string.IsNullOrEmpty(searchType))
            {
                FillWithValidTypes(ref number);
            }

            for (; number < SEARCH_COUNT; number++)
            {
                foundTypes[number] = null;
            }
        }
        
        private bool TryGetActiveFolderPath(out string path)
        {
            GetActiveFolderPathMethodArguments[0] = null;
            object result = GetActiveFolderPathMethod.Invoke(null, GetActiveFolderPathMethodArguments);
            bool boolResult = (bool) result;
            path = boolResult ? (string) GetActiveFolderPathMethodArguments[0] : default;
            return boolResult;
        }

        private bool TryGetValidateNameForCreateInstance(string path, string fileName, out string validateCreatePath)
        {
            bool validatePath = false;
            string checkPath = null;
            string fullPath = path + "/" + fileName;
            for (int i = 0; i < MAX_VALIDATE_NAME_COUNT && !validatePath; i++)
            {
                checkPath = fullPath + (i != 0 ? "(" + i + ")" : string.Empty) + ASSET_EXPANSION;
                validatePath = !AssetDatabase.LoadAssetAtPath(checkPath, typeof(UnityEngine.Object));
            }

            validateCreatePath = validatePath ? checkPath : null;
            return validatePath;
        }
        
        private void CreateType(Type type)
        {
            ScriptableObject asset = CreateInstance(type);

            if (TryGetActiveFolderPath(out string path) &&
                TryGetValidateNameForCreateInstance(path, type.Name, out string validateCreatePath))
            {
                AssetDatabase.CreateAsset(asset, validateCreatePath);
                AssetDatabase.Refresh();
            }
        }
        
        private void DrawType(Type type)
        {
            if (type == null)
            {
                EditorGUILayout.LabelField(string.Empty);
                return;
            }

            createType = type;
            DrawHorizontalBox(DrawCreatesType);
        }

        private void TryDrawTypes()
        {
            if (ReferenceEquals(foundTypes, null) ||
                foundTypes.Length == 0)
            {
                return;
            }
            
            for (int i = 0; i < SEARCH_COUNT; i++)
            {
                DrawType(foundTypes[i]);
            }
        }

        private void DrawSearchPanel()
        {
            EditorGUILayout.LabelField(ENTER_MANAGER_NAME_LABEL);
            string newSearchType = EditorGUILayout.TextField(searchType);
            bool changed = newSearchType != searchType;
            searchType = newSearchType;
            if (changed)
            {
                FindTypes();
            }
        }

        private void CreateScriptableObject()
        {
            CreateType(createType);
        }

        private void DrawCreatesType()
        {
            DrawButton(CREATE_BUTTON_NAME, CreateScriptableObject);
            GUILayout.Label(createType.Namespace + ".", GUILayout.ExpandWidth(false));
            GUILayout.Label(createType.Name, BoldStile, GUILayout.ExpandWidth(false));
        }

        private void OnEnable()
        {
            foundTypes = new Type[SEARCH_COUNT];
        }
    }
}