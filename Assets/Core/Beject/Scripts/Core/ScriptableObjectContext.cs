using System.Collections.Generic;
using UnityEngine;

namespace MBSCore.Beject
{
	public partial class ScriptableObjectContext : ScriptableObject, IContext
	{
		[SerializeField] private SerializedInjectCell[] injectObjects;
		[SerializeField] private SerializedFillingCell[] fillingObjects;
		
		public void OnEnable()
		{
			ContextHandler.Add(this);
		}

		public void OnDisable()
		{
			ContextHandler.Remove(this);
		}

		public InjectCell[] GetInjectObjects()
		{
			int count = injectObjects.Length; 
			InjectCell[] result = new InjectCell[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = injectObjects[i].GetInjectCell();
			}
			
			return result;
		}

		public IEnumerable<KeyValuePair<Object, InjectMembersContainer>>  GetFillingObjects()
		{
			int count = fillingObjects.Length; 
			KeyValuePair<Object, InjectMembersContainer>[] result = new KeyValuePair<Object, InjectMembersContainer>[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = fillingObjects[i].GetMembersContainer();
			}
			
			return result;
		}
	}
}
