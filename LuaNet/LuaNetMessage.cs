using System;
using System.Collections.Generic;
using UNetPkg;
using LuaInterface;

namespace LuaUtility
{
    class LuaNetMessage : NetMessageBase
    {
        private byte[] m_sendData;
        private LuaFunction m_callback;

        public LuaNetMessage(string channel, int code, byte[] data, LuaFunction callback)
        {
            Init(channel, code, ProcessResult, ProcessError);
            m_sendData = data;
            m_callback = callback;
        }

        public LuaNetMessage(int code, byte[] data, LuaFunction callback)
            : this(ProjectDefine.NET_CHANNEL_SHORTCONN, code, data, callback)
        {
        }

        public override byte[] GetSendData()
        {
            return m_sendData;
        }

        void ProcessError(NetMessageBase nm)
        {
            throw new System.Exception("LuaNetMessage: errcode = " + nm.State);
        }

        void ProcessResult(byte[] data)
        {
            if (m_callback != null)
                m_callback.Call(data);
        }
    }
}
