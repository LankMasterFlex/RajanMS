using System;
using System.Collections.Generic;

namespace Common.Collections
{
    public sealed class SafeDictionMap<TKey, TValue> : IDisposable
    {
        private Dictionary<TKey, TValue> m_dictionary;
        private object m_locker;

        public SafeDictionMap()
        {
            m_dictionary = new Dictionary<TKey, TValue>();
            m_locker = new object();
        }

        public void Add(TKey key, TValue value)
        {
            lock (m_locker)
            {
                m_dictionary.Add(key, value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (m_locker)
                {
                    return m_dictionary[key];
                }
            }
        }

        public void Remove(TKey key)
        {
            lock (m_locker)
            {
                m_dictionary.Remove(key);
            }
        }

        public void Clear()
        {
            lock (m_locker)
            {
                m_dictionary.Clear();
            }
        }

        public void ForAll(Action<KeyValuePair<TKey, TValue>> action)
        {
            lock (m_locker)
            {
                foreach (KeyValuePair<TKey, TValue> pair in m_dictionary)
                {
                    action(pair);
                }
            }
        }

        public void Dispose()
        {
            Clear();
            m_dictionary = null;
            m_locker = null;
        }
    }
}