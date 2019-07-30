using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Database
{
    public class LRUCache<K, V>
    {
        private readonly int _capacity;
        private readonly IDictionary<K, LinkedListNode<LRUCacheItem<K, V>>> _cacheMap = new ConcurrentDictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        private readonly LinkedList<LRUCacheItem<K, V>> _lruList = new LinkedList<LRUCacheItem<K, V>>();

        public LRUCache(int capacity)
        {
            _capacity = capacity;
        }

        public V Get(K key)
        {
            LinkedListNode<LRUCacheItem<K, V>> node;
            if (_cacheMap.TryGetValue(key, out node))
            {
                var value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddLast(node);
                return value;
            }
            return default(V);
        }
        
        public void Add(K key, V val)
        {
            if (_cacheMap.Count >= _capacity)
            {
                RemoveFirst();
            }

            var cacheItem = new LRUCacheItem<K, V>(key, val);
            var node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);
            _lruList.AddLast(node);
            _cacheMap.Add(key, node);
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            var node = _lruList.First;
            _lruList.RemoveFirst();

            // Remove from cache
            _cacheMap.Remove(node.Value.Key);
        }
    }

    class LRUCacheItem<K, V>
    {
        public LRUCacheItem(K k, V v)
        {
            Key = k;
            Value = v;
        }

        public readonly K Key;
        public readonly V Value;
    }
}