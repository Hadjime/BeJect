﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MBSCore.Editor
{
    public abstract class CustomEditorWindow : EditorWindow
    {
        private const string GUI_STYLE_BOX = "Box";

        private Dictionary<string, Vector2> ScrollViewPositions = new Dictionary<string, Vector2>();
        
        private SerializedObject targetSerializedObject;

        public SerializedObject TargetSerializedObject => targetSerializedObject;

        protected abstract void DrawGUI();

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

        private void CreateTarget()
        {
            ScriptableObject target = this;
            targetSerializedObject = new SerializedObject(target);
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
        
        private void OnGUI()
        {
            CreateTarget();
            DrawGUI();
        }
    }
}