using System;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public interface ISafeWebview
    {
        void SendMessage(string id, RpcRequestMessage message, Func<RpcResponseMessage, bool> callbackHandler);

        void Request(string id, string path, Func<TestResponse, bool> callbackHandler);
    }
}