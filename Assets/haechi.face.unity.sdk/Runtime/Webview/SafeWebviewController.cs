using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Settings;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Webview
{

    public class CloseWebviewArgs
    {
        public string RequestId;
    }
    
    public class SafeWebviewController : MonoBehaviour
    {
        private readonly Dictionary<string, Func<FaceRpcResponse, bool>> _handlerDictionary
            = new Dictionary<string, Func<FaceRpcResponse, bool>>();

        public event Action<SafeWebviewController, CloseWebviewArgs> OnCloseWebview;

        private void Awake()
        {
            Application.deepLinkActivated += this.onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this.onDeepLinkActivated(Application.absoluteURL);
            }
        }
#if UNITY_IOS
        [DllImport("__Internal")]
        extern static void web3auth_launch(string url, string redirectUri, string objectName);
#endif
        
        private static void LaunchUrl(string url, string objectName = null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Application.OpenURL(url);
#elif UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var browserView = new AndroidJavaObject("com.web3auth.unity.android.BrowserView"))
        {
            browserView.CallStatic("launchUrl", activity, url);
        }

#elif UNITY_IOS
    var uri = new Uri(url);
    web3auth_launch(url, SafeWebviewProtocol.Scheme, objectName);
#endif
        }

        public void onDeepLinkActivated(string url)
        {
            this._handleDeepLink(new Uri(url));
        }

        public void SendMessage(RpcRequestMessage message, Func<FaceRpcResponse, bool> callbackHandler)
        {
            Debug.Log($"Register Handler with ID: {message.Id}");
            this._handlerDictionary.Add(message.Id.ToString(), callbackHandler);
            
            string queryParams = SafeWebviewProtocol.EncodeQueryParams(new SafeWebviewProtocol.Parameters
            {
                Request = message,
                ApiKey = FaceSettings.Instance.ApiKey(),
                Env = FaceSettings.Instance.Environment(),
                Blockchain = FaceSettings.Instance.Blockchain()
            });
            UriBuilder uriBuilder = new UriBuilder(FaceSettings.Instance.WebviewHostURL())
            {
                Query = queryParams
            };
            Debug.Log($"URL: {uriBuilder}");

            // Launch browser
            LaunchUrl(uriBuilder.ToString(), this.gameObject.name);
        }

        private void _handleDeepLink(Uri uri)
        {
            Debug.Log($"URI Receive: {uri}");
            FaceRpcResponse response = SafeWebviewProtocol.DecodeQueryParams(uri);
            Debug.Log($"Response received: {response}");
            
            if (FaceRpcMethod.face_closeIframe.Is(response.Method))
            {
                this.OnCloseWebview?.Invoke(this, new CloseWebviewArgs
                {
                    RequestId = "TestID" // TODO: Fix me
                });
                return;
            }

            if (!this._handlerDictionary.TryGetValue(response.Id.ToString(), out Func<FaceRpcResponse, bool> callback))
            {
                Debug.Log($"Cannot find handler by id: {response.Id}");
                return;
            }

            callback(response); 
            this._handlerDictionary.Remove(response.Id.ToString());
        }
    }
}