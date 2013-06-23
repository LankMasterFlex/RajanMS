using System;
using System.Collections.Generic;

namespace Common.Collections
{
    public sealed class SafeList<T> : IDisposable
    {
        private List<T> m_list;
        private object m_locker;

        public SafeList()
        {
            m_list = new List<T>();
            m_locker = new object();
        }

        public void Add(T value)
        {
            lock (m_locker)
            {
                m_list.Add(value);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (m_locker)
                {
                    return m_list[index];
                }
            }
        }

        public void Remove(T value)
        {
            lock (m_locker)
            {
                m_list.Remove(value);
            }
        }

        public void Clear()
        {
            lock (m_locker)
            {
                m_list.Clear();
            }
        }

        public void ForAll(Action<T> action)
        {
            lock (m_locker)
            {
                foreach (T value in m_list)
                {
                    action(value);
                }
            }
        }

        public void Dispose()
        {
            Clear();
            m_list = null;
            m_locker = null;
        }
    }
}
