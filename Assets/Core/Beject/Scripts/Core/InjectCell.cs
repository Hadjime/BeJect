using System;

namespace MBSCore.Beject
{
    public class InjectCell
    {
        public readonly object InjectObject;
        public readonly Type[] InjectTypes;

        public InjectCell(object injectObject, Type[] injectTypes)
        {
            InjectObject = injectObject;
            InjectTypes = injectTypes;
        }
    }
}