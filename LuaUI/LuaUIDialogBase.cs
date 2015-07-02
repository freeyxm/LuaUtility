using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LuaInterface;

namespace LuaUtility
{
    public class LuaUIDialogBase : UIDialogBase
    {
        public string UIName = "LuaUIDialogBase";
        public string m_luaFile;
        public GameObject[] m_gameObjects;

        protected LuaHelper m_luaHelper;
        private Dictionary<string, object> m_objects;
        private bool m_luaLoaded = false;

        void Initialize()
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
        }

        protected override void Init()
        {
            InitMediator(new LuaUIDialogBaseMediator(this));
        }

        public override string GetUIName()
        {
            return UIName;
        }

        new void Awake()
        {
            // Can't bind LuaBinder to gameObject in lua script --> Awake called befor property be setted.
            Initialize();
            
            LoadLua();

            base.Awake();

            CallLuaFunction("Awake");
        }

        void Start()
        {
            CallLuaFunction("Start");
        }

        new void OnDestroy()
        {
            base.OnDestroy();
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

        public override void RefreshUI()
        {
            base.RefreshUI();
            CallLuaFunction("RefreshUI");
        }

        protected override void OnPlayOpenAnimBegine()
        {
            base.OnPlayOpenAnimBegine();
            CallLuaFunction("OnPlayOpenAnimBegine");
        }

        protected override void OnPlayOpenAnimEnd()
        {
            base.OnPlayOpenAnimEnd();
            CallLuaFunction("OnPlayOpenAnimEnd");
        }

        protected override void OnPlayCloseAnimBegine()
        {
            base.OnPlayCloseAnimBegine();
            CallLuaFunction("OnPlayCloseAnimBegine");
        }

        protected override void OnPlayCloseAnimEnd()
        {
            base.OnPlayCloseAnimEnd();
            CallLuaFunction("OnPlayCloseAnimEnd");
        }

        public override void OnFocusStateChange(bool gotFocus, UIPanelBase ui)
        {
            base.OnFocusStateChange(gotFocus, ui);
            CallLuaFunction("OnFocusStateChange", gotFocus, ui);
        }

        void LoadLua()
        {
            if (!m_luaLoaded && !string.IsNullOrEmpty(m_luaFile))
            {
                m_luaHelper.DoFile("CommonImport.lua");

                //StringBuilder buff = new StringBuilder();
                //buff.Append(UIName).Append(" = {};").AppendLine();
                //buff.Append("function ").Append(UIName).Append(":new()").AppendLine();
                //buff.Append("    local o = {};").AppendLine();
                //buff.Append("    setmetatable(o, self);").AppendLine();
                //buff.Append("    self.__index = self;").AppendLine();
                //buff.Append("    return o;").AppendLine();
                //buff.Append("end").AppendLine();
                //m_luaHelper.DoString(buff.ToString());

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

        public void SendNotification(string name, object value)
        {
            GameManager.Instance.SendNotification(name, value);
        }
    }
}