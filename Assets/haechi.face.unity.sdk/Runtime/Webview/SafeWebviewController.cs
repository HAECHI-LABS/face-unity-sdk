using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using haechi.face.unity.sdk.Runtime.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Webview
{
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
        // TODO: fix method prefix with face~
#if UNITY_IOS
        [DllImport("__Internal")]
        extern static void launch_face_webview(string url, string redirectUri, string objectName);
#endif
        
        private static void LaunchUrl(string url, string objectName = null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Application.OpenURL(url);
#elif UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var browserView = new AndroidJavaObject("com.face.unity.android.BrowserView"))
        {
            browserView.CallStatic("launchUrl", activity, url);
        }

#elif UNITY_IOS
    var uri = new Uri(url);
    launch_face_webview(url, SafeWebviewProtocol.Scheme, objectName);
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
                Blockchain = FaceSettings.Instance.Blockchain(),
                Hostname = Application.identifier
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
            
            FaceRpcContext context = SafeWebviewProtocol.DecodeQueryParams(uri);
            Debug.Log($"Data received from webview: {context}");
            
            if (context.WebviewRequest())
            {
                if (FaceRpcMethod.face_closeIframe.Is(context.Request.Method))
                {
                    this.OnCloseWebview?.Invoke(this, new CloseWebviewArgs
                    {
                        Response = new FaceRpcResponse(context.Request)
                    });
                    return;
                }    
            }

            FaceRpcResponse response = context.Response;
            if (!this._handlerDictionary.TryGetValue(response.Id.ToString(), out Func<FaceRpcResponse, bool> callback))
            {
                Debug.Log($"Cannot find handler by id: {response.Id}");
                return;
            }
            
            callback(response);
            this._handlerDictionary.Remove(response.Id.ToString());
        }
    }
    
    public class CloseWebviewArgs
    {
        public FaceRpcResponse Response;
    }
}