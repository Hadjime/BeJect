using MBSCore.Editor;

namespace MBSCore.Beject.Editor
{
    [UnityEditor.CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : CustomEditor
    {
        private const string FIND_BUTTON_TEXT = "Find MonoBehaviours for InjectSystem";
        private const string INVOKE_METHOD_NAME = "FindMonoBehavioursForInjectSystem";
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawButton(FIND_BUTTON_TEXT, InvokeSceneContext);
        }

        private void InvokeSceneContext()
        {
            SceneContext sceneContext = (SceneContext)target;
            ReflectionInvokeMethod<SceneContext>(INVOKE_METHOD_NAME, sceneContext, new object[] {});
        }
    }
}