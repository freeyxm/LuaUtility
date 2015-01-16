using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LuaInterface;

namespace LuaUtility
{
    public class LuaBinderWithUpdate : LuaBinder
    {
        private LuaFunction m_callbackUpdate;

        void Update()
        {
            if (m_callbackUpdate != null)
                m_callbackUpdate.Call(this);
        }

        public void SetCallbackUpdate(LuaFunction callback)
        {
            m_callbackUpdate = callback;
        }
    }
}