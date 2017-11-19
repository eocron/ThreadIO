using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ThreadIO
{
    public class Graph<TKey> : IGraph<TKey>
    {
        private readonly Dictionary<TKey, BiDirNode> _nodes;
        public Graph()
        {
            _nodes = new Dictionary<TKey, BiDirNode>();
        }

        public Graph(IEnumerable<TKey> enumerable)
        {
            _nodes = enumerable.ToDictionary(x => x, x => new BiDirNode(x));
        }

        public bool TryAddNode(TKey key)
        {
            if (!_nodes.ContainsKey(key))
            {
                _nodes.Add(key, new BiDirNode(key));
                return true;
            }
            return false;
        }

        public bool TryAddEdge(TKey from, TKey to)
        {
            BiDirNode toNode;
            BiDirNode fromNode;
            if (!_nodes.TryGetValue(to, out toNode) || !_nodes.TryGetValue(from, out fromNode))//not present
            {
                return false;
            }
            if (fromNode.Outputs.Contains(toNode))//already contains
            {
                return false;
            }
            fromNode.Outputs.Add(toNode);
            toNode.Inputs.Add(fromNode);
            return true;
        }

        public bool TryRemoveNode(TKey key)
        {
            if (_nodes.ContainsKey(key))
            {
                var node = _nodes[key];
                foreach (var i in node.Inputs)
                {
                    i.Outputs.Remove(node);
                }
                foreach (var o in node.Outputs)
                {
                    o.Inputs.Remove(node);
                }
                _nodes.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryRemoveEdge(TKey from, TKey to)
        {
            BiDirNode toNode;
            BiDirNode fromNode;
            if (!_nodes.TryGetValue(to, out toNode) || !_nodes.TryGetValue(from, out fromNode))//not present
            {
                return false;
            }
            if (!fromNode.Outputs.Contains(toNode))//no edge
            {
                return false;
            }
            fromNode.Outputs.Remove(toNode);
            toNode.Inputs.Remove(fromNode);
            return true;
        }

        /// <summary>
        /// Get dependand nodes
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllAscendants(IEnumerable<TKey> keys)
        {
            return TraverseDistinct(keys, GetAscendants);
        }

        /// <summary>
        /// Get dependency nodes
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllDescendants(IEnumerable<TKey> keys)
        {
            return TraverseDistinct(keys, GetDescendants);
        }

        public IEnumerable<TKey> GetAscendants(TKey key)
        {
            return _nodes[key].Inputs.Select(x => x.Key);
        }

        public IEnumerable<TKey> GetDescendants(TKey key)
        {
            return _nodes[key].Outputs.Select(x => x.Key);
        }

        private static IEnumerable<T> TraverseDistinct<T>(IEnumerable<T> keys, Func<T, IEnumerable<T>> selector)
        {
            if (keys == null)
                yield break;

            var set = new HashSet<T>();
            var stack = new Stack<T>();
            foreach (var key in keys)
            {
                stack.Push(key);
            }
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                set.Add(item);
                yield return item;
                var children = selector(item);
                if (children != null)
                {
                    foreach (var c in children)
                    {
                        if (!set.Contains(c))
                        {
                            stack.Push(c);
                        }
                    }
                }
            }
        }

        private sealed class BiDirNode
        {
            public readonly ISet<BiDirNode> Inputs;

            public readonly TKey Key;

            public readonly ISet<BiDirNode> Outputs;

            public BiDirNode(TKey key)
            {
                Inputs = new HashSet<BiDirNode>();
                Outputs = new HashSet<BiDirNode>();
                Key = key;
            }

            public override bool Equals(object obj)
            {
                var item = obj as BiDirNode;

                if (item == null)
                {
                    return false;
                }

                return this.Key.Equals(item.Key);
            }

            public override int GetHashCode()
            {
                return this.Key.GetHashCode();
            }
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return _nodes.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _nodes.Count;
    }
}
