using System.Reflection;
using MBSCore.Beject.SceneSystem;
using UnityEditor;
using UnityEngine;

namespace MBSCore.Beject.Editor
{
    public class BejectSceneCreator
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string INJECT_MANAGER_NAME = "Beject Manager";
        private const string SCENE_CONTEXT_NAME = "Scene Context";
        private const string SCENE_MANAGER_NAME = "Scene Manager";
        private const string SCRIPTABLE_OBJECT_CONTEXT = "scriptableObjectContext";
        private const string INJECT_MAP = "map";
        
        private static FieldInfo ReflectionGetField<TObject>(string fieldName)
        {
            return typeof(TObject).GetField(fieldName, BINDING_FLAGS);
        }

        private static void ReflectionSetField<TObject>(string fieldName, object instance, object value)
        {
            FieldInfo fieldInfo = ReflectionGetField<TObject>(fieldName);
            fieldInfo.SetValue(instance, value);
        }

        private static TObject CreateObject<TObject>(string name) where TObject : MonoBehaviour
        {
            return new GameObject(name).AddComponent<TObject>();
        }
        
        [MenuItem("Tools/BeJect/Add Manager")]
        private static void AddInjectManager()
        {
            InjectManager injectManager = CreateObject<InjectManager>(INJECT_MANAGER_NAME);
            ScriptableObjectContextMediator mediator =
                ScriptableObject.CreateInstance<ScriptableObjectContextMediator>();
            ReflectionSetField<InjectManager>(SCRIPTABLE_OBJECT_CONTEXT, injectManager,
                mediator.CurrentScriptableObject);
        }
        
        [MenuItem("Tools/BeJect/Add SceneContext")]
        private static void AddSceneContext()
        {
            SceneContext sceneContext = CreateObject<SceneContext>(SCENE_CONTEXT_NAME);
            InjectMapMediator mediator = ScriptableObject.CreateInstance<InjectMapMediator>();
            ReflectionSetField<SceneContext>(INJECT_MAP, sceneContext, mediator.CurrentScriptableObject);
        }

        [MenuItem("Tools/BeJect/Add SceneManager")]
        private static void AddSceneManager()
        {
            CreateObject<SceneManager>(SCENE_MANAGER_NAME);
        }
        
        [MenuItem("Tools/BeJect/All Modules")]
        private static void AddInjectSystemToScene()
        {
            AddInjectManager();
            AddSceneContext();
            AddSceneManager();
        }
    }
}