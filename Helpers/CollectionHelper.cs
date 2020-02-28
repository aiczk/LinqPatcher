using Mono.Collections.Generic;

namespace LinqPatcher.Helpers
{
    public static class CollectionHelper
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this Collection<T> collection) =>
            new ReadOnlyCollection<T>(collection);
    }
}
