using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace MBSCore.Editor
{
    [CustomPropertyDrawer(typeof(SerializedType))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        private const BindingFlags FOUND_BINDING_FLAGS =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string ERROR_MESSAGE = "Error";
        private const string ARRAY_INDICATOR = "Array";
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string typeLabel;

            string[] typePaths = property.propertyPath.Split('.');
            object mediatorMember = property.serializedObject.targetObject;
            Type mediatorType = mediatorMember.GetType();
            bool mediatorIsArray = false;
            foreach (var typePath in typePaths)
            {
                if (typePath == ARRAY_INDICATOR)
                {
                    mediatorIsArray = true;
                    continue;
                }

                if (mediatorIsArray)
                {
                    int beginNumber = typePath.IndexOf('[') + 1;
                    int numberLength = typePath.Length - beginNumber - 1;
                    string indexString = typePath.Substring(beginNumber, numberLength);
                    int index = int.Parse(indexString);
                    if (mediatorMember is IList listMember)
                    {
                        mediatorMember = listMember[index];
                    }

                    mediatorIsArray = false;
                }
                else
                {
                    FieldInfo fInfo = mediatorType.GetField(typePath, FOUND_BINDING_FLAGS);
                    if (fInfo != null)
                    {
                        mediatorMember = fInfo.GetValue(mediatorMember);
                    }
                    else
                    {
                        mediatorMember = null;
                        break;
                    }
                }

                mediatorType = mediatorMember.GetType();
            }

            if (mediatorMember is SerializedType serializedType)
            {
                typeLabel = serializedType.ToString();
            }
            else
            {
                typeLabel = ERROR_MESSAGE;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.TextField(position, label, typeLabel);
            }
        }
    }
}