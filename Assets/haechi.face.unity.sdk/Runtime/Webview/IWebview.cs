using System;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    // NOTE: Not used
    public interface ISafeWebview
    {
        void SendMessage(string id, RpcRequestMessage message, Func<RpcResponseMessage, bool> callbackHandler);
    }
}