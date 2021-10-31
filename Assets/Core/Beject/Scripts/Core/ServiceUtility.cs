using System;
using System.Collections.Generic;
using System.Reflection;

namespace MBSCore
{
    public static class ServiceUtility
    {
        private static readonly object[] ArgumentsInFunction = new object[1];
        
        public static void SortValues<T>(List<T> actualValues, IReadOnlyList<T> setValue, Action<T> newProcedure = null,
            Action<T> deleteProcedure = null)
        {
            for (int i = 0; i < setValue.Count; i++)
            {
                T value = setValue[i];
                if (!actualValues.Remove(value))
                {
                    newProcedure?.Invoke(value);
                }
            }

            for (int i = 0; i < actualValues.Count; i++)
            {
                deleteProcedure?.Invoke(actualValues[i]);
            }

            actualValues.Clear();
            actualValues.AddRange(setValue);
        }

        public static void ValidateGenericType(Type genericDefinition, Type checkedType, out Type[] genericTypes)
        {
            Type[] interfaceTypes = checkedType.GetInterfaces();
            for (int i = 0; i < interfaceTypes.Length; i++)
            {
                Type interfaceType = interfaceTypes[i];
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericDefinition)
                {
                    genericTypes = interfaceType.GetGenericArguments();
                    return;
                }
            }

            genericTypes = null;
        }

        public static void AutoGenericInvoke<T>(T argument, Type argumentGenericDefinition, MethodInfo actionGenericMethod, object invokerObject)
        {
            Type type = argument.GetType();
            ValidateGenericType(argumentGenericDefinition, type, out Type[] genericTypes);
            ArgumentsInFunction[0] = argument;
            for (int i = 0; i < genericTypes.Length; i++)
            {
                actionGenericMethod.MakeGenericMethod(genericTypes[i]).Invoke(invokerObject, ArgumentsInFunction);
            }
        }
    }
}
