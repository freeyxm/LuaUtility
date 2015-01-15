using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaUtility
{
    public class LuaObjectMgr<T>
    {
        public const bool DEFAULT_DUPLICATE = false;
        Dictionary<string, T> m_container;
        protected bool m_allowDuplicate;

        private static LuaObjectMgr<T> m_instance;
        public static LuaObjectMgr<T> Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new LuaObjectMgr<T>();
                return m_instance;
            }
        }

        public LuaObjectMgr(bool allowDuplicate)
        {
            m_allowDuplicate = allowDuplicate;
            m_container = new Dictionary<string, T>();
        }

        public LuaObjectMgr()
            : this(DEFAULT_DUPLICATE)
        {
        }

        public void SetAllowDuplicate(bool allow)
        {
            m_allowDuplicate = allow;
        }

        public T GetObject(string key)
        {
            if (m_container.ContainsKey(key))
                return m_container[key];
            else
                return default(T);
        }

        // Can't use virtual when class is a generic class (ios not support it!).
        public void AddObject(string key, T obj)
        {
            if (m_container.ContainsKey(key))
            {
                if (!m_allowDuplicate)
                    throw new Exception("LuaObjectMgr: Object's key duplicated. key = " + key);
                else
                    m_container[key] = obj;
            }
            else
            {
                m_container.Add(key, obj);
            }
        }

        public void RemoveObject(string key)
        {
            if (m_container.ContainsKey(key))
                m_container.Remove(key);
        }
    }
}
