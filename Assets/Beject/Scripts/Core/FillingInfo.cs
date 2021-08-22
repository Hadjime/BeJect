using System;
using System.Reflection;

namespace MBSCore.Beject
{
    internal class FillingInfo
    {
        public readonly Type FillingType;
        public readonly FieldInfo[] FieldInfos;
        public readonly PropertyInfo[] PropertyInfos;
        
        public FillingInfo(Type type, FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos)
        {
            FillingType = type;
            FieldInfos = fieldInfos;
            PropertyInfos = propertyInfos;
        }

        public InjectMembersContainer GetMembers()
        {
            return new InjectMembersContainer(FieldInfos, PropertyInfos);
        }
    }
}