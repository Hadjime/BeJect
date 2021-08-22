using System;
using System.Reflection;
using UnityEngine;

namespace MBSCore.Beject
{
    [Serializable]
    internal class SerializedFillingInfo
    {
        [SerializeField] private SerializedType fillingType;
        [SerializeField] private string[] fieldPaths;
        [SerializeField] private string[] propertyPaths;

        public SerializedFillingInfo(Type fillingType, FieldInfo[] injectFields, PropertyInfo[] injectProperties)
        {
            this.fillingType = fillingType;
            fieldPaths = SerializedTypeConverter.GetPaths(injectFields);
            propertyPaths = SerializedTypeConverter.GetPaths(injectProperties);
        }

        public FillingInfo GetFillingInfo()
        {
            FieldInfo[] fields = SerializedTypeConverter.GetFieldInfos(fillingType, fieldPaths);
            PropertyInfo[] properties = SerializedTypeConverter.GetPropertyInfos(fillingType, propertyPaths);
            return new FillingInfo(fillingType, fields, properties);
        }
    }
}