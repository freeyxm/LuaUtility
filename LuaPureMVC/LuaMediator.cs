using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;

namespace LuaUtility.LuaPureMVC
{
    public class LuaMediator : Mediator
    {
        LuaHelper m_luaHelper;
        List<string> m_notifications = new List<string>();

        public LuaMediator(string mediatorName, object viewComponent, string luaFile, LuaHelper luaHelper)
            : base(mediatorName, viewComponent)
        {
            if (luaHelper == null)
                m_luaHelper = new LuaHelper(false);
            else
                m_luaHelper = luaHelper;
            m_luaHelper.DoFile(luaFile);
            CallLuaFunction("LLuaMediator_Init");
            LuaMediatorMgr.Instance.AddObject(mediatorName, this);
        }

        public LuaMediator(string mediatorName, object viewComponent, string luaFile)
            : this(mediatorName, viewComponent, luaFile, null)
        {
        }

        public override void HandleNotification(INotification notification)
        {
            CallLuaFunction("LLuaMediator_HandleNotification", notification);
        }

        public override void OnRegister()
        {
            CallLuaFunction("LLuaMediator_OnRegister");
        }

        public override void OnRemove()
        {
            CallLuaFunction("LLuaMediator_OnRemove");
        }

        public void AddNotification(params string[] notifications)
        {
            m_notifications.AddRange(notifications);
        }

        public override IList<string> ListNotificationInterests
        {
            get
            {
                IList<string> list = base.ListNotificationInterests;
                IEnumerator<string> e = m_notifications.GetEnumerator();
                while (e.MoveNext())
                {
                    list.Add(e.Current);
                }
                return list;
            }
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
