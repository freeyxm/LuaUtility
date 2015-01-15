using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Patterns;

namespace LuaUtility.LuaPureMVC
{
    public class LuaProxy : Proxy
    {
        LuaHelper m_luaHelper;

        public LuaProxy(string name, string luaFile, LuaHelper luaHelper)
            : base(name)
        {
            if (luaHelper == null)
                m_luaHelper = new LuaHelper(false);
            else
                m_luaHelper = luaHelper;
            m_luaHelper.DoFile(luaFile);
            CallLuaFunction("LLuaProxy_Init");
        }

        public LuaProxy(string name, string luaFile)
            : this(name, luaFile, null)
        {
        }

        public override void OnRegister()
        {
            CallLuaFunction("LLuaProxy_OnRegister");
        }

        public override void OnRemove()
        {
            CallLuaFunction("LLuaProxy_OnRemove");
        }

        public object[] CallLuaFunction(string funcName, params object[] args)
        {
            return m_luaHelper.CallFunctionEA(funcName, this, args);
        }

        public LuaHelper GetLuaHelper()
        {
            return m_luaHelper;
        }
    }
}
