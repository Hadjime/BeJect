using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MBSCore.Editor
{
    public class CustomEditor : UnityEditor.Editor
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string GUI_STYLE_BOX = "Box";
        
        private Dictionary<string, Vector2> ScrollViewPositions = new Dictionary<string, Vector2>();
        
        private SerializedObject targetSerializedObject;

        public SerializedObject TargetSerializedObject => targetSerializedObject;
        
        protected void CreateTarget()
        {
            ScriptableObject target = this;
            targetSerializedObject = new SerializedObject(target);
        }

        protected void DrawProperty(string lable, SerializedProperty serializedProperty)
        {
            GUILayout.Label(lable);
            EditorGUILayout.PropertyField(serializedProperty, true);
            targetSerializedObject.ApplyModifiedProperties();
        }
        
        protected void DrawVerticalBox(Action content, bool style = true)
        {
            if (style)
            {
                EditorGUILayout.BeginVertical(GUI_STYLE_BOX);   
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }
        
        protected void DrawHorizontalBox(Action content, bool style = true)
        {
            if (style)
            {
                EditorGUILayout.BeginHorizontal(GUI_STYLE_BOX);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
            }
            
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }
        
        protected int DrawDropdown(string lable, int selectedIndex, params string[] options)
        {
            EditorGUILayout.BeginHorizontal();
            if (lable != String.Empty)
            {
                GUILayout.Label(lable);
            }
            
            selectedIndex = EditorGUILayout.Popup(selectedIndex, options);
            EditorGUILayout.EndHorizontal();
            return selectedIndex;
        }
        
        protected void DrawButton(string text, Action callback)
        {
            if (GUILayout.Button(text))
            {
                callback?.Invoke();
            }
        }

        protected void DrawScrollView(string key, Action content, bool style = true)
        {
            if (style)
            {
                EditorGUILayout.BeginHorizontal(GUI_STYLE_BOX);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
            }

            if (!ScrollViewPositions.TryGetValue(key, out Vector2 scrollViewPosition))
            {
                scrollViewPosition = Vector2.zero;
                ScrollViewPositions.Add(key, scrollViewPosition);
            }
            
            scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);
            content?.Invoke();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        protected FieldInfo ReflectionGetField<TObject>(string fieldName)
        {
            return typeof(TObject).GetField(fieldName, BINDING_FLAGS);
        }

        protected PropertyInfo ReflectionGetProperty<TObject>(string propertyName)
        {
            return typeof(TObject).GetProperty(propertyName, BINDING_FLAGS);
        }

        protected MethodInfo ReflectionGetMethod<TObject>(string methodName)
        {
            return typeof(TObject).GetMethod(methodName, BINDING_FLAGS);
        }

        protected void ReflectionSetField<TObject>(string fieldName, object instance, object value)
        {
            FieldInfo fieldInfo = ReflectionGetField<TObject>(fieldName);
            fieldInfo.SetValue(instance, value);
        }

        protected void ReflectionSetProperty<TObject>(string propertyName, object instance, object value)
        {
            PropertyInfo propertyInfo = ReflectionGetProperty<TObject>(propertyName);
            propertyInfo.SetMethod?.Invoke(instance, new [] { value });
        }

        protected void ReflectionInvokeMethod<TObject>(string methodName, object instance, params object[] arg)
        {
            MethodInfo methodInfo = ReflectionGetMethod<TObject>(methodName);
            methodInfo?.Invoke(instance, arg);
        }
    }
}