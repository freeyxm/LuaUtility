using System;
using System.Collections.Generic;
using UNetPkg;
using LuaInterface;

namespace LuaUtility
{
    class LuaNetMessage : NetMessageBase
    {
        private byte[] mData;
        private LuaFunction mCallbackFunc;

        public LuaNetMessage(string channel, int code, byte[] data, LuaFunction cbFunc)
        {
            Init(channel, code, ProcessResult, ProcessError);
            mData = data;
            mCallbackFunc = cbFunc;
        }

        public override byte[] GetSendData()
        {
            return mData;
        }

        void ProcessError(NetMessageBase nm)
        {
            throw new System.Exception("LuaNetMessage: errcode = " + nm.State);
        }

        void ProcessResult(byte[] data)
        {
            if (mCallbackFunc != null)
                mCallbackFunc.Call(data);
        }
    }
}
