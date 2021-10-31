using System;
using System.Collections.Generic;
using UnityEngine;

namespace MBSCore
{
    [Serializable]
    public class SerializedType : ISerializationCallbackReceiver
    {
        private const string EmptyClass = "(None)";
        private const string DeserializeExceptionMessage = "Serialized class cannot be deserialized";
        private const string Separator = ", ";
        
        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
        
        [SerializeField] private string serializedName;
        private Type _type;

        public SerializedType()
        {
        }

        public SerializedType(string assemblyQualifiedClassName)
        {
            Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
                ? Type.GetType(assemblyQualifiedClassName)
                : null;
        }

        public SerializedType(Type type)
        {
            Type = type;
        }

        public Type Type
        {
            get => _type;
            private set
            {
                if (value == null)
                {
                    throw  new NullReferenceException();
                }

                _type = value;
                serializedName = GetSerializedName(value);
                TypeMap[serializedName] = _type;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(serializedName))
            {
                if (TypeMap.TryGetValue(serializedName, out _type))
                {
                    return;
                }
                
                _type = Type.GetType(serializedName);
                if (_type == null)
                {
                    throw new ArgumentException(DeserializeExceptionMessage, serializedName);
                }

                TypeMap.Add(serializedName, _type);
            }
            else
            {
                _type = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        public static implicit operator string(SerializedType typeReference)
        {
            return typeReference.serializedName;
        }

        public static implicit operator Type(SerializedType typeReference)
        {
            return typeReference.Type;
        }

        public static implicit operator SerializedType(Type type)
        {
            return new SerializedType(type);
        }

        public override string ToString()
        {
            if (_type == null)
            {
                return EmptyClass;
            }

            var fullName = _type.FullName;

            return string.IsNullOrEmpty(fullName) ? EmptyClass : fullName;
        }

        private static string GetSerializedName(Type type)
        {
            return type != null
                ? type.FullName + Separator + type.Assembly.GetName().Name
                : string.Empty;
        }
    }
}