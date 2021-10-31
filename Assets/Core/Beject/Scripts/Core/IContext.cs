using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MBSCore.Beject
{
    public interface IContext
    {
        InjectCell[] GetInjectObjects();
        IEnumerable<KeyValuePair<Object, InjectMembersContainer>> GetFillingObjects();
    }
}