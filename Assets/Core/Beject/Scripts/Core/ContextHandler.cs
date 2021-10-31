using System;
using System.Collections.Generic;

namespace MBSCore.Beject
{
    internal static class ContextHandler
    {
        private static List<IContext> contextList = new List<IContext>();
        
        public static event Action<IContext> AddHandler;
        public static event Action<IContext> RemoveHandler;

        public static void Add(IContext context)
        {
            if (contextList.Contains(context))
            {
                return;
            }
            
            contextList.Add(context);
            AddHandler?.Invoke(context);
        }

        public static void Remove(IContext context)
        {
            if (!contextList.Contains(context))
            {
                return;
            }
            
            contextList.Remove(context);
            RemoveHandler?.Invoke(context);
        }

        public static IEnumerable<IContext> GetCurrentContexts()
        {
            return contextList.ToArray();
        }
    }
}