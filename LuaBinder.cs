﻿using System;
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
        public GameObject[] m_gameObjects;

        protected LuaHelper m_luaHelper;
        private Dictionary<string, object> m_objects;
        private bool m_luaLoaded = false;

        void Init()
        {
            m_luaHelper = new LuaHelper(false); // It's important to use a non global lua state, or lua function belong to different script will confused.
            m_objects = new Dictionary<string, object>();

            RegisterAttributes();

            for (int i = 0; i < m_gameObjects.Length; ++i)
            {
                GameObject gameObject = m_gameObjects[i];
                if (!m_objects.ContainsKey(gameObject.name))
                    m_objects.Add(gameObject.name, gameObject);
            }

            if (!string.IsNullOrEmpty(m_name))
            {
                LuaBinderMgr.Instance.AddObject(m_name, this);
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
                m_luaHelper.DoFile(m_luaFile);
                m_luaLoaded = true;
            }
        }

        void RegisterAttributes()
        {
            m_luaHelper.RegisterAttribute("gameObject", gameObject);
            m_luaHelper.RegisterAttribute("transform", transform);
            m_luaHelper.RegisterAttribute("this", this);
        }

        public object[] CallLuaFunction(string funcName, params object[] args)
        {
            return m_luaHelper.CallFunction(funcName, args);
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