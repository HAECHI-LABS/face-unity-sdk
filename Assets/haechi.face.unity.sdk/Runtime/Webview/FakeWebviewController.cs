using System;
using Nethereum.JsonRpc.Client.RpcMessages;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public class FakeWebviewController : MonoBehaviour, ISafeWebview
    {
        public void SendMessage(string id, RpcRequestMessage message, Func<RpcResponseMessage, bool> callbackHandler)
        {
            // NO-OP
        }
    }
}