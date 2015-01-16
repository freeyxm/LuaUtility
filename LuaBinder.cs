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
        public bool m_enableAwake = false;

        protected LuaHelper m_luaHelper;
        private Dictionary<string, object> m_objects;
        private bool m_luaLoaded = false;

        void Awake()
        {
            m_objects = new Dictionary<string, object>();
            m_luaHelper = new LuaHelper(false); // It's important to use a non global lua state, or lua function belong to diff script will confused.

            RegisterFunctions();

            if (m_enableAwake)
            {
                // If enable awake => Can't bind LuaBinder to gameObject in lua script.(Awake called befor property be setted.)
                CallLuaFunction("LLuaBinder_Awake");
            }

            if (!string.IsNullOrEmpty(m_name))
            {
                LuaBinderMgr.Instance.AddObject(m_name, this);
            }
        }

        void Start()
        {
            CallLuaFunction("LLuaBinder_Start");
        }

        void Destroy()
        {
            CallLuaFunction("LLuaBinder_Destroy");
        }

        void OnEnable()
        {
            CallLuaFunction("LLuaBinder_OnEnable");
        }

        void OnDisable()
        {
            CallLuaFunction("LLuaBinder_OnDisable");
        }

        void LoadLua()
        {
            if (!m_luaLoaded && !string.IsNullOrEmpty(m_luaFile))
            {
                m_luaHelper.DoFile(m_luaFile);
                m_luaLoaded = true;
            }
        }

        protected virtual void RegisterFunctions()
        {
            //mLuaHelper.RegisterFunction("LLuaBinder_XXX", this, this.GetType().GetMethod("XXX"));
        }

        public object[] CallLuaFunction(string funcName, params object[] args)
        {
            LoadLua(); // if !m_enableAwake, just after Instantiate gameObject may haven't load lua!
            return m_luaHelper.CallFunctionEA(funcName, this, args);
        }

        public LuaHelper GetLuaHelper()
        {
            return m_luaHelper;
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