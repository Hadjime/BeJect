using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBSCore.Beject
{
    [Serializable]
    internal class SerializedInjectCell
    {
        [SerializeField] private Object injectObject;
        [SerializeField] private SerializedType[] injectTypes;
        
        public SerializedInjectCell(Object injectObject, Type[] injectTypes)
        {
            this.injectObject = injectObject;
            int injectTypeCount = injectTypes.Length;
            this.injectTypes = new SerializedType[injectTypeCount];
            for (int i = 0; i < injectTypeCount; i++)
            {
                this.injectTypes[i] = new SerializedType(injectTypes[i]);
            }
        }
        
        public InjectCell GetInjectCell()
        {
            int injectTypeCount = injectTypes.Length;
            Type[] types = new Type[injectTypeCount];
            for (int i = 0; i < injectTypeCount; i++)
            {
                types[i] = injectTypes[i].Type;
            }

            return new InjectCell(injectObject, types);
        }
    }
}