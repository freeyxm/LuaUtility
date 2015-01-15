using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaUtility.LuaPureMVC
{
    public class LuaModel
    {
        string m_name;
        Dictionary<string, object> m_datas;

        public LuaModel(string name)
        {
            m_name = name;
            m_datas = new Dictionary<string, object>();
        }

        public void AddData(string key, object data)
        {
            m_datas.Add(key, data);
        }

        public object GetData(string key)
        {
            if (m_datas.ContainsKey(key))
                return m_datas[key];
            else
                return null;
        }
    }
}
