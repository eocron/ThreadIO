namespace ThreadIO
{
    public interface ILockScopeGraph<TKey> : ILockScopeDictionary<TKey>
    {
        /// <summary>
        /// Adds depenedncy from one key to another. 
        /// Cycles allowed, but be aware - logicaly they considered as singular node and same type of lock will be shared among them.
        /// Good practice is not to use cycles at all.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool TryAddEdge(TKey from, TKey to);

        /// <summary>
        /// Removes dependency from one key to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool TryRemoveEdge(TKey from, TKey to);
    }
}