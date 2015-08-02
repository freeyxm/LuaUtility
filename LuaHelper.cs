using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LuaInterface;

namespace LuaUtility
{
    public class LuaHelper
    {
        private LuaState m_lua;
        private Dictionary<string, bool> m_loadedFiles = new Dictionary<string,bool>();
        private Dictionary<string, LuaFunction> m_functions = new Dictionary<string, LuaFunction>();

        public LuaHelper(bool useGlobal)
        {
            if (useGlobal)
                m_lua = LuaManager.GetSingleton().GetLua();
            else
                m_lua = new LuaState();

            RegisterFunctions();
        }

        public LuaHelper(LuaState lua)
        {
            m_lua = lua;
        }

        void RegisterFunctions()
        {
            RegisterFunction("RequireLua", this, this.GetType().GetMethod("RequireLua"));
        }

        public void RegisterFunction(string path, object target, System.Reflection.MethodBase function)
        {
            m_lua.RegisterFunction(path, target, function);
        }

        public void RegisterAttribute(string key, object value)
        {
            m_lua[key] = value;
        }

        public object GetAttribute(string key)
        {
            return m_lua[key];
        }

        public object[] DoString(string chunk)
        {
            return m_lua.DoString(chunk);
        }

        public bool DoFile(string fileName, bool reloadFlag)
        {
            if (!m_loadedFiles.ContainsKey(fileName))
            {
				LuaManager.DoCodeFile(m_lua, fileName);
                m_loadedFiles.Add(fileName, true);
                return true;
            }
            else if (reloadFlag)
            {
				LuaManager.DoCodeFile(m_lua, fileName);
                return true;
            }
            return false;
        }

        public bool DoFile(string fileName)
        {
            return DoFile(fileName, false);
        }

        public void RequireLua(string luaFileName)
        {
            DoFile(luaFileName, false);
        }

        public void CreateClassNewFunc(string className)
        {
            StringBuilder buff = new StringBuilder();
            //buff.Append(className).Append(" = {};").AppendLine();
            buff.Append("function ").Append(className).Append(":new()").AppendLine();
            buff.Append("    local o = {};").AppendLine();
            buff.Append("    setmetatable(o, self);").AppendLine();
            buff.Append("    self.__index = self;").AppendLine();
            buff.Append("    return o;").AppendLine();
            buff.Append("end").AppendLine();
            DoString(buff.ToString());
        }

        public LuaFunction GetFunction(string funcName)
        {
            if (m_functions.ContainsKey(funcName))
            {
                return m_functions[funcName];
            }
            else
            {
                LuaFunction func = m_lua.GetFunction(funcName);
                if (func != null)
                {
                    m_functions.Add(funcName, func);
                }
                return func;
            }
        }

        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = GetFunction(funcName);
            return func != null ? func.Call(args) : null;
        }

        /// <summary>
        /// Call lua function with expand last element(array) in args so that can auto expand in lua script.
        /// Notice such param list style (obj, array[]), know what will happen before you use it.
        /// </summary>
        public object[] CallFunctionEA(string funcName, params object[] args)
        {
            LuaFunction func = GetFunction(funcName);
            if (func == null)
            {
                return null;
            }
            else if (args.Length <= 1 || !(args[args.Length - 1] is Array))
            {
                return func.Call(args);
            }
            else
            {
                int preArgsNum = args.Length - 1;
                Array lastArray = args[preArgsNum] as Array;
                if (lastArray.Length == 0)
                {
                    return func.Call(args);
                }
                else
                {
                    object[] newArgs = new object[preArgsNum + lastArray.Length];
                    Array.Copy(args, newArgs, preArgsNum);
                    Array.Copy(lastArray, 0, newArgs, preArgsNum, lastArray.Length);
                    return func.Call(newArgs);
                }
            }
        }

        public static System.Action Action(LuaFunction func)
        {
            System.Action action = () =>
            {
                func.Call();
            };
            return action;
        }

        public static System.Action Action(LuaFunction func, params object[] args)
        {
            System.Action action = () =>
                {
                    func.Call(args);
                };
            return action;
        }

        public static UIEventListener.VoidDelegate VoidDelegate(LuaFunction func)
        {
            UIEventListener.VoidDelegate action = (go) =>
            {
                func.Call();
            };
            return action;
        }

        public static UIEventListener.VoidDelegate VoidDelegate(LuaFunction func, params object[] args)
        {
            UIEventListener.VoidDelegate action = (go) =>
                {
                    func.Call(args);
                };
            return action;
        }

        public static Transform FindTransform(Transform tran, string name)
        {
            Transform target = tran.transform.Find(name);
            if (target == null)
                throw new Exception("FindTransform: child not exist. " + tran.name + " -> " + name);
            return target;
        }

        public static Component FindComponent(GameObject go, string name, string compType)
        {
            Transform target = FindTransform(go.transform, name);
            Component component = target.gameObject.GetComponent(compType);
            if (component == null)
                throw new Exception("FindComponent: component not exist. " + go.name + " -> " + name + " -> " + compType);
            return component;
        }

        public static LuaBinder GetLuaBinder(GameObject obj, string name)
        {
            LuaBinder[] comps = obj.GetComponents<LuaBinder>();

            for (int i = 0; i < comps.Length; ++i)
            {
                if (comps[i].m_name.Equals(name))
                    return comps[i];
            }

            Debuger.LogError(string.Format("GetLuaBinder failed. obj = {0}, name = {1}", obj.name, name));
            return null;
        }

        public static List<object> CreateList()
        {
            return new List<object>();
        }

        public static Dictionary<object, object> CreateDictionary()
        {
            return new Dictionary<object, object>();
        }
    }
}
