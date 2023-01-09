using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class Iframe
    {
        private static readonly Ready ready = new Ready();
        
        [DllImport("__Internal")]
        private static extern void createIframe(string url, Action readyCompleteCallback, Action readyCallback, Action showIframeCallback, Action hideIframeCallback);

        [DllImport("__Internal")]
        private static extern void sendChildMessage(string blockchain, string serializedMessage, Action readyCallback);

        [DllImport("__Internal")]
        private static extern void waitForResponse(string requestId, Action<string, string> responseCallback);

        [DllImport("__Internal")]
        private static extern void consoleLog(string log);
        
        [DllImport("__Internal")]
        private static extern void showOverlay(Action readyCallback);
        
        [DllImport("__Internal")]
        private static extern void hideOverlay(Action readyCallback);

        public static void CreateIframe()
        {
            if (FaceSettings.Instance == null)
            {
                return;
            }
            createIframe(FaceSettings.Instance.IframeURL(), ReadyCompleteCallback, ReadyCallback, ShowIframeCallback, HideIframeCallback);
        }

        public static void SendChildMessage(RpcRequestMessage message)
        {
            sendChildMessage(FaceSettings.Instance.Blockchain().ToString(), JsonConvert.SerializeObject(message), ReadyCallback);
        }

        public static void WaitForResponse(string requestId, Action<string, string> responseCallback)
        {
            waitForResponse(requestId, responseCallback);
        }

        public static void ConsoleLog(string log)
        {
            consoleLog(log);
        }
        
        [MonoPInvokeCallback(typeof(Action))]
        private static void ShowIframeCallback()
        {
            showOverlay(ReadyCallback);
        }
        
        [MonoPInvokeCallback(typeof(Action))]
        private static void HideIframeCallback()
        {
            hideOverlay(ReadyCallback);
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void ReadyCompleteCallback()
        {
            ready.Complete();
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static async void ReadyCallback()
        {
            Task task = new Task(() =>
            {
                if (ready.IsCompleted())
                {
                    return;
                }

                ready.Add(() => { });
            });
            await task;
        }

        private class Ready
        {
            private bool _isCompleted;
            private List<Action> _eventListeners = new List<Action>();

            public void Complete()
            {
                this._isCompleted = true;
                foreach (Action action in this._eventListeners)
                {
                    action();
                }
            }

            public void Add(Action action)
            {
                this._eventListeners.Add(action);
            }

            public bool IsCompleted()
            {
                return this._isCompleted;
            }
        }
    }
}