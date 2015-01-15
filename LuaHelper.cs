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
        private LuaState m_lua = null;
        private bool m_useGlobal = false;

        public LuaHelper(bool _useGlobal)
        {
            m_useGlobal = _useGlobal;
            if (m_useGlobal)
                m_lua = LuaManager.GetSingleton().GetLua();
            else
                m_lua = new LuaState();
        }

        public void RegisterFunction(string path, object target, System.Reflection.MethodBase function)
        {
            m_lua.RegisterFunction(path, target, function);
        }

        public object[] DoString(string chunk)
        {
            return m_lua.DoString(chunk);
        }

        public void DoFile(string fileName)
        {
            LuaManager.DoFile(m_lua, fileName);
        }

        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = m_lua.GetFunction(funcName);
            if (func != null)
                return func.Call(args);
            else
                return null;
        }

        /// <summary>
        /// Call lua function with expand last element(array) in args so that can auto expand in lua script.
        /// Notice such param list style (obj, array[]), know what will happen before you use it.
        /// </summary>
        public object[] CallFunctionEA(string funcName, params object[] args)
        {
            LuaFunction func = m_lua.GetFunction(funcName);
            if (func == null)
                return null;
            else if (args == null || args.Length <= 1 || !(args[args.Length - 1] is Array))
                return func.Call(args);
            else
            {
                int argsLen = args.Length - 1;
                Array lastArray = args[argsLen] as Array;
                if (lastArray.Length == 0)
                    return func.Call(args);
                else
                {
                    object[] newArgs = new object[argsLen + lastArray.Length];
                    Array.Copy(args, newArgs, argsLen);
                    Array.Copy(lastArray, 0, newArgs, argsLen, lastArray.Length);
                    return func.Call(newArgs);
                }
            }
        }

        public static Action Action(LuaFunction func, params object[] args)
        {
            Action action = () =>
                {
                    func.Call(args);
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
