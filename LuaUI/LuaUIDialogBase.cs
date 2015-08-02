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
        public LuaUIDialogBase m_parentBinder1; // 当绑定了该值时，此实例将共享m_parentBinder的Lua环境。
        public LuaBinder m_parentBinder2; // 当绑定了该值时，此实例将共享m_parentBinder的Lua环境。
        public GameObject[] m_gameObjects;

        protected LuaHelper m_luaHelper;
        private Dictionary<string, object> m_objects;
        private bool m_luaLoaded = false;

        protected string m_luaInstanceName;
        protected object m_luaInstance;

        void Initialize()
        {
            m_luaInstanceName = UIName;

            // 由于C#没有多重继承，LuaBinder与LuaUIDialogBase不能实现为继承关系，只能逐一判断类型。
            if (m_parentBinder1 != null)
            {
                m_luaHelper = m_parentBinder1.GetLuaHelper();
            }
            else if (m_parentBinder2 != null)
            {
                m_luaHelper = m_parentBinder2.GetLuaHelper();
            }
            else
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
                m_luaHelper.DoFile(m_luaFile);

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
                return m_luaHelper.CallFunction(funcName, m_luaInstance);
            else
                return m_luaHelper.CallFunctionEA(funcName, m_luaInstance, args);
        }

        public LuaHelper GetLuaHelper()
        {
            return m_luaHelper;
        }

        public string GetLuaInstanceName()
        {
            return m_luaInstanceName;
        }

        public object GetLuaInstance()
        {
            return m_luaInstance;
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