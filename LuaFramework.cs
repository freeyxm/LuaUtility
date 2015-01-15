using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LuaInterface;

namespace WPFLuaFramework
{
    /// <summary>
    /// Lua引擎
    /// </summary>
    class LuaFramework
    {
        private LuaState pLuaVM = new LuaState();//lua虚拟机

        /// <summary>
        /// 注册lua函数
        /// </summary>
        /// <param name="pLuaAPIClass">lua函数类</param>
        public void BindLuaApiClass(Object pLuaAPIClass)
        {
            foreach (MethodInfo mInfo in pLuaAPIClass.GetType().GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    string LuaFunctionName = (attr as LuaFunction).getFuncName();
                    pLuaVM.RegisterFunction(LuaFunctionName, pLuaAPIClass, mInfo);
                }
            }
        }

        /// <summary>
        /// 执行lua脚本文件
        /// </summary>
        /// <param name="luaFileName">脚本文件名</param>
        public void ExecuteFile(string luaFileName)
        {
            try
            {
                pLuaVM.DoFile(luaFileName);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.ToString());
            }
        }

        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="luaCommand">lua指令</param>
        public void ExecuteString(string luaCommand)
        {
            try
            {
                pLuaVM.DoString(luaCommand);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.ToString());
            }
        }
    }
}