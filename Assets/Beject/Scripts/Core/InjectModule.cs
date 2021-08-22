using System;
using System.Collections.Generic;
using System.Reflection;

namespace MBSCore.Beject
{
    internal abstract class InjectModule
    {
        public abstract bool TryConvertToSimple(Type complexType, out Type simpleType);
        public abstract bool TrySetValueIntoField(object fillingObject, FieldInfo fieldInfo, List<object> values);
        public abstract bool TrySetValueIntoProperty(object fillingObject, PropertyInfo propertyInfo, List<object> values);
    }
}