using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using haechi.face.unity.sdk.Runtime.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public class SafeWebviewController : MonoBehaviour, IURLHandler
    {
        private readonly Dictionary<string, Func<FaceRpcResponse, bool>> _handlerDictionary
            = new Dictionary<string, Func<FaceRpcResponse, bool>>();

        public event Action<SafeWebviewController, CloseWebviewArgs> OnCloseWebview;

        private void Awake()
        {
            Application.deepLinkActivated += this._onDeepLinkActivated;
            Application.focusChanged += this._onFocusChanged;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this._onDeepLinkActivated(Application.absoluteURL);
            }
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        extern static void launch_face_webview(string url, string redirectUri, string objectName);
#endif
        
        private void _launchUrl(string url, bool isAuth)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Application.OpenURL(url);
#elif UNITY_ANDROID
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var browserView = new AndroidJavaObject("xyz.facewallet.unity.android.BrowserView"))
            {
                browserView.CallStatic("launchUrl", activity, url);
            }
#elif UNITY_IOS
            launch_face_webview(url, SafeWebviewProtocol.Scheme, this.gameObject.name);
#endif
        }

        private void _onDeepLinkActivated(string url)
        {
            this._handleDeepLink(new Uri(url));
        }

        public void SendMessage(RpcRequestMessage message, Func<FaceRpcResponse, bool> callbackHandler, bool isAuth)
        {
            string redirectUri = "";
#if UNITY_EDITOR
            redirectUri = LocalTestWebServer.Start(this);
#endif
            this._handlerDictionary.Add(message.Id.ToString(), callbackHandler);
            
            string queryParams = SafeWebviewProtocol.EncodeQueryParams(new SafeWebviewProtocol.Parameters
            {
                Request = message,
                ApiKey = FaceSettings.Instance.ApiKey(),
                Env = FaceSettings.Instance.Environment(),
                Blockchain = FaceSettings.Instance.Blockchain(),
                Schema = redirectUri,
                Hostname = Application.identifier
            });
            UriBuilder uriBuilder = new UriBuilder(FaceSettings.Instance.WebviewHostURL())
            {
                Query = queryParams
            };
            Debug.Log($"URL: {uriBuilder}");

            // Launch browser
            string url = uriBuilder.ToString();
            this._launchUrl(url, isAuth);
            Debug.Log("URL Launched");
        }
        
        private void _onFocusChanged(bool isFocused)
        {
            // Return true when focus is changed into Unity App
            if (!isFocused)
            {
                return;
            }
#if UNITY_ANDROID
            this.OnCloseWebview?.Invoke(this, new CloseWebviewArgs
            {
                Response = FaceRpcResponse.WebviewClosed()
            });
#endif
        }

        private void _handleDeepLink(Uri uri)
        {
            Debug.Log($"URI Receive: {uri}");
            
            FaceRpcContext context = SafeWebviewProtocol.DecodeQueryParams(uri);
            Debug.Log($"Data received from webview: {context}");
            if (context.WebviewRequest())
            {
                WebviewRpcRequest request = context.Request;
                if (FaceRpcMethod.face_openBrowser.Is(request.Method))
                {
                    Debug.Log("!!!!!!!!!!!face_openBrowser!!!!!!!!!!!!!");
                    string[] arr = ((IEnumerable)request.RawParameters).Cast<object>()
                        .Select(x => x.ToString())
                        .ToArray();
                    Debug.Log(arr[0]);
                    Application.OpenURL(arr[0]);
                    return;
                }
                if (FaceRpcMethod.face_closeIframe.Is(request.Method))
                {
                    this.OnCloseWebview?.Invoke(this, new CloseWebviewArgs
                    {
                        Response = new FaceRpcResponse(request)
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
            
            if (String.IsNullOrEmpty(context.Response.Method) && String.IsNullOrEmpty(context.Response.Result.ToString()))
            {
                this._handlerDictionary.Remove(response.Id.ToString());
                return;
            }
            
            callback(response);
            this._handlerDictionary.Remove(response.Id.ToString());
        }

        public void HandleUrl(Uri url)
        {
            this._handleDeepLink(url);
        }
    }
    
    public class CloseWebviewArgs
    {
        public FaceRpcResponse Response;
    }
}