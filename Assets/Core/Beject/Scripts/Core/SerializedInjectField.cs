using System;
using System.Reflection;
using UnityEngine;

namespace MBSCore.Beject
{
    [Serializable]
    internal struct SerializedInjectField
    {
        [SerializeField] private SerializedType _type;
        [SerializeField] private string _path;

        public SerializedInjectField(FieldInfo fieldInfo)
        {
            _type = fieldInfo.DeclaringType;
            _path = fieldInfo.Name;
        } 

        public FieldInfo GetFieldInfo()
        {
            return _type.Type.GetField(_path, InjectUtility.FOUND_BINDING_FLAGS);
        }
    }
}