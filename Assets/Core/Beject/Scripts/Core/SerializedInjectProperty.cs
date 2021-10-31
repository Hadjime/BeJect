using System;
using System.Reflection;
using UnityEngine;

namespace MBSCore.Beject
{
    [Serializable]
    internal struct SerializedInjectProperty
    {
        [SerializeField] private SerializedType _type;
        [SerializeField] private string _path;
        
        public SerializedInjectProperty(PropertyInfo propertyInfo)
        {
            _type = propertyInfo.DeclaringType;
            _path = propertyInfo.Name;
        } 

        public PropertyInfo GetPropertyInfo()
        {
            return _type.Type.GetProperty(_path, InjectUtility.FOUND_BINDING_FLAGS);
        }
    }
}
