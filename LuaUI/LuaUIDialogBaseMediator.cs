using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaUtility
{
    public class LuaUIDialogBaseMediator : BaseCommMediator<LuaUIDialogBase>
    {
        public LuaUIDialogBaseMediator(LuaUIDialogBase ui)
            : base(ui.UIName + "Mediator", ui)
        {
            viewUI.CallLuaFunction("Mediator_Init", this);
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            //Debuger.Log(string.Format("HandleNotification: {0} -> {1}", notification.Name, notification.Body));
            viewUI.CallLuaFunction("Mediator_HandleNotification", notification.Name, notification.Body);
        }

        public void AddNotify(string notify)
        {
            //Debuger.Log(string.Format("AddNotify: {0}", notify));
            BaseCommMediator<LuaUIDialogBase>.AddNotify(notify);
        }
    }
}
