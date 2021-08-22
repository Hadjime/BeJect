using UnityEngine;

namespace MBSCore.Beject.Editor
{
    public class SOMediator<TSO> : ScriptableObject
        where TSO : ScriptableObject
    {
        [SerializeField] private TSO typeScriptableObject;

        public TSO CurrentScriptableObject => typeScriptableObject;
    }
}