namespace AdeNote.Infrastructure.Extension
{
    public static class CollectionExtension
    {
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                return; 
            foreach(T item in collection)
            {
                action(item);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return false;

            return collection.Any();
        }
    }
}
