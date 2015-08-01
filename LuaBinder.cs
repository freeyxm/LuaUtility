using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LuaInterface;

namespace LuaUtility
{
    public class LuaBinder : MonoBehaviour
    {
        public string m_name;
        public string m_luaFile;
        public bool m_single = false; // 是否在当前Lua虚拟机内是唯一的？
        public MonoBehaviour m_parentBinder; // 当绑定了该值时，此实例将共享m_parentBinder的Lua虚拟机。
        public GameObject[] m_gameObjects;

        protected LuaHelper m_luaHelper;
        private Dictionary<string, object> m_objects; // 缓存。
        private bool m_luaLoaded = false;

        protected static int m_globalInstanceIndex = 0; // 当在同个Lua虚拟机内创建多个同脚本的对象时(如复制Item)，每个对象拥有唯一的索引，以相互区别。
        protected string m_luaInstanceName; // 绑定的Lua对象的名称。
        protected object m_luaInstance; // 绑定的Lua对象，调用Lua函数时，需要传递该值。（class:func()调用形式）

        void Init()
        {
            m_luaInstanceName = m_single ? m_name : string.Format("{0}_{1}", m_name, ++m_globalInstanceIndex); // 注意命名方式，防止重复。

            m_luaHelper = null;
            if (m_parentBinder != null)
            {
                do
                {
                    // 由于C#没有多重继承，LuaBinder与LuaUIDialogBase不能实现为继承关系，只能逐一判断类型。
                    LuaBinder binder = m_parentBinder as LuaBinder;
                    if (binder != null)
                    {
                        m_luaHelper = binder.GetLuaHelper();
                        break;
                    }
                    LuaUIDialogBase dialog = m_parentBinder as LuaUIDialogBase;
                    if (dialog != null)
                    {
                        m_luaHelper = dialog.GetLuaHelper();
                        break;
                    }
                } while (false);
            }
            if (m_luaHelper == null)
            {
                m_luaHelper = new LuaHelper(false);
            }

            m_objects = new Dictionary<string, object>();
            for (int i = 0; i < m_gameObjects.Length; ++i)
            {
                GameObject gameObject = m_gameObjects[i];
                if (!m_objects.ContainsKey(gameObject.name))
                    m_objects.Add(gameObject.name, gameObject);
            }

            if (m_single)
            {
                LuaBinderMgr.Instance.AddObject(m_name, this);
            }
        }

        void Clear()
        {
            m_objects.Clear();

            if (m_single)
            {
                LuaBinderMgr.Instance.RemoveObject(m_name);
            }
        }

        void Awake()
        {
            // Can't bind LuaBinder to gameObject in lua script --> Awake called befor property be setted.
            Init();
            
            LoadLua();

            CallLuaFunction("Awake");
        }

        void Start()
        {
            CallLuaFunction("Start");
        }

        void OnDestroy()
        {
            CallLuaFunction("OnDestroy");
            Clear();
        }

        void OnEnable()
        {
            CallLuaFunction("OnEnable");
        }

        void OnDisable()
        {
            CallLuaFunction("OnDisable");
        }

        void LoadLua()
        {
            if (!m_luaLoaded && !string.IsNullOrEmpty(m_luaFile))
            {
                bool loadFlag = m_luaHelper.DoFile(m_luaFile);
                if (!m_single)
                {
                    if (loadFlag)
                    {
                        m_luaHelper.CreateClassNewFunc(m_name);
                    }
                    // 创建Lua类实例
                    m_luaHelper.DoString(string.Format("{0} = {1}:new();", m_luaInstanceName, m_name));
                }
                m_luaInstance = m_luaHelper.GetAttribute(m_luaInstanceName);

                RegisterAttributes();

                m_luaLoaded = true;
            }
        }

        void RegisterAttributes()
        {
            RegisterAttribute("gameObject", gameObject);
            RegisterAttribute("transform", transform);
            RegisterAttribute("binder", this);
        }

        void RegisterAttribute(string name, object value)
        {
            m_luaHelper.RegisterAttribute(string.Format("{0}.{1}", m_luaInstanceName, name), value);
        }

        public object[] CallLuaFunction(string funcName, params object[] args)
        {
            funcName = string.Format("{0}.{1}", m_luaInstanceName, funcName);

            if (args.Length == 0)
            {
                return m_luaHelper.CallFunction(funcName, m_luaInstance);
            }
            else
            {
                // 采用class:func()调用形式，需要传递class实例。
                object[] tempArgs = new object[args.Length + 1];
                tempArgs[0] = m_luaInstance;
                args.CopyTo(tempArgs, 1);
                return m_luaHelper.CallFunction(funcName, tempArgs);
            }
        }

        public LuaHelper GetLuaHelper()
        {
            return m_luaHelper;
        }

        public string GetLuaInstanceName()
        {
            return m_luaInstanceName;
        }

        // used to cache object for lua script.
        public void SetObject(string key, object obj)
        {
            if (obj == null)
                m_objects.Remove(key);
            else
                m_objects[key] = obj;
        }

        public object GetObject(string key)
        {
            if (m_objects.ContainsKey(key))
                return m_objects[key];
            else
                return null;
        }
    }
}