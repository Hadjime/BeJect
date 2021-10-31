using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBSCore.Beject
{
    [Serializable]
    internal class SerializedFillingCell
    {
        [SerializeField] private Object fillingObject;
        [SerializeField] private string[] fieldPaths;
        [SerializeField] private string[] propertyPaths;
        
        public SerializedFillingCell(Object fillingObject, InjectMembersContainer membersContainer)
        {
            this.fillingObject = fillingObject;
            fieldPaths = SerializedTypeConverter.GetPaths(membersContainer.FieldInfos);
            propertyPaths = SerializedTypeConverter.GetPaths(membersContainer.PropertyInfos);
        }

        public KeyValuePair<Object, InjectMembersContainer> GetMembersContainer()
        {
            Type fillingType = fillingObject.GetType();
            FieldInfo[] fields = SerializedTypeConverter.GetFieldInfos(fillingType, fieldPaths);
            PropertyInfo[] properties = SerializedTypeConverter.GetPropertyInfos(fillingType, propertyPaths);
            InjectMembersContainer membersContainer = new InjectMembersContainer(fields, properties);
            return new KeyValuePair<Object, InjectMembersContainer>(fillingObject, membersContainer);
        }
    }
}