using System.Collections.Generic;
using System.Reflection;

namespace MBSCore.Beject
{
    public class InjectMembersContainer
    {
        public readonly List<FieldInfo> FieldInfos = new List<FieldInfo>();
        public readonly List<PropertyInfo> PropertyInfos = new List<PropertyInfo>();
        
        public InjectMembersContainer(FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos)
        {
            FieldInfos.AddRange(fieldInfos);
            PropertyInfos.AddRange(propertyInfos);
        }
    }
}
