using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Patterns;

namespace LuaUtility.LuaPureMVC
{
    public class LuaFacade : Facade
    {
        LuaHelper m_luaHelper;

        public LuaFacade(string key, string luaFile, LuaHelper luaHelper)
            : base(key)
        {
            if (luaHelper == null)
                m_luaHelper = new LuaHelper(false);
            else
                m_luaHelper = luaHelper;
            m_luaHelper.DoFile(luaFile);
            CallLuaFunction("LLuaFacade_Init");
            LuaFacadeMgr.Instance.AddObject(key, this);
        }

        public LuaFacade(string key, string luaFile)
            : this(key, luaFile, null)
        {
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
