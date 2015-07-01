using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace LuaUtility.LuaPureMVC
{
    public class LuaMacroCmd : MacroCommand
    {
        LuaHelper m_luaHelper;

        public LuaMacroCmd(string luaFile, LuaHelper luaHelper)
        {
            if (luaHelper == null)
                m_luaHelper = new LuaHelper(false);
            else
                m_luaHelper = luaHelper;
            m_luaHelper.DoFile(luaFile);
            CallLuaFunction("LLuaMacroCmd_Init");
        }

        public LuaMacroCmd(string luaFile)
            : this(luaFile, null)
        {
        }

        // open Facade for script use.
        public new IFacade Facade
        {
            get { return base.Facade; }
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
