using System.Collections.Generic;

namespace ThreadIO
{
    public interface IGraph<TKey> : IReadOnlyCollection<TKey>
    {
        IEnumerable<TKey> GetAllAscendants(IEnumerable<TKey> keys);
        IEnumerable<TKey> GetAllDescendants(IEnumerable<TKey> keys);

        IEnumerable<TKey> GetAscendants(TKey key);
        IEnumerable<TKey> GetDescendants(TKey key);

        bool TryAddEdge(TKey from, TKey to);
        bool TryAddNode(TKey key);
        bool TryRemoveEdge(TKey from, TKey to);
        bool TryRemoveNode(TKey key);
    }
}