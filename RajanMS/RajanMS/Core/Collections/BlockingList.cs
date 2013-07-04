using System;
using System.Collections.Generic;

namespace RajanMS.Core.Collections
{
    class BlockingList<T>
    {
        private List<T> m_list;
        private object m_locker;
     
        public BlockingList()
        {
            m_list = new List<T>();
            m_locker = new object();
        }

        public void Add(T item, bool block = true)
        {
            if (block)
            {
                lock (m_locker)
                    m_list.Add(item);
            }
            else
            {
                m_list.Add(item);
            }
        }

        public void Remove(T item, bool block = true)
        {
            if (block)
            {
                lock (m_locker)
                    m_list.Remove(item);
            }
            else
            {
                m_list.Remove(item);
            }
        }

        public void ForEach(Action<T> action,bool block = true)
        {
            if (block)
            {
                lock (m_locker)
                {
                    foreach (T obj in m_list)
                        action(obj);
                }
            }
            else
            {
                foreach (T obj in m_list)
                    action(obj);
            }
        }
    }
}
