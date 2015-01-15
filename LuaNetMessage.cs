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
            Init(channel, code);
            mData = data;
            mCallbackFunc = cbFunc;
        }

        public override byte[] GetSendData()
        {
            return mData;
        }

        public override void ProcessErr()
        {
            throw new System.NotImplementedException("LuaProtocol: errcode = " + State);
        }

        protected override void ProcessResult(byte[] data)
        {
            mCallbackFunc.Call(data);
        }
    }
}
