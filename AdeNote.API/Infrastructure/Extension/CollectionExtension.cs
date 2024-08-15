﻿namespace AdeNote.Infrastructure.Extension
{
    public static class CollectionExtension
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                return; 
            foreach(T item in collection)
            {
                action(item);
            }
           
        }
    }
}
