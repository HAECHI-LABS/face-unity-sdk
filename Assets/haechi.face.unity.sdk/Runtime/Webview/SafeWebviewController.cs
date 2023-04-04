using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public class SafeWebviewController : MonoBehaviour, IURLHandler
    {
        private readonly Dictionary<string, Func<FaceRpcResponse, bool>> _handlerDictionary
            = new Dictionary<string, Func<FaceRpcResponse, bool>>();

        private static readonly ObservableDictionary<string, string> IframeResponse = new ObservableDictionary<string, string>();
        public event Action<SafeWebviewController, CloseWebviewArgs> OnCloseWebview;

        private void Awake()
        {
            IframeResponse.CollectionChanged += (_, args) =>
            {
                foreach (var argsNewItem in args.NewItems)
                {
                    KeyValuePair<string, string> response = (KeyValuePair<string, string>)argsNewItem;
                    if (!this._handlerDictionary.TryGetValue(response.Key, out Func<FaceRpcResponse, bool> callback))
                    {
                        Debug.Log($"Cannot find handler by id: {response.Key}");
                        return;
                    }
                    FaceRpcResponse rpcResponse = JsonConvert.DeserializeObject<FaceRpcResponse>(response.Value);
                    callback(rpcResponse);
                    this._handlerDictionary.Remove(response.Key);
                }
            };
            Application.deepLinkActivated += this.OnDeepLinkActivated;
            Application.focusChanged += this.onFocusChanged;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this.OnDeepLinkActivated(Application.absoluteURL);
            }
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        extern static void launch_face_webview(string url, string redirectUri, string objectName);
#endif
#if UNITY_IOS
        public void OnWebviewCanceled()
        {
            this.OnCloseWebview?.Invoke(this, new CloseWebviewArgs
            {
                Response = FaceRpcResponse.WebviewClosed()
            });
        }
#endif        


        private static void LaunchUrl(string url, string objectName = null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Application.OpenURL(url);
#elif UNITY_ANDROID
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var browserView = new AndroidJavaObject("xyz.facewallet.unity.android.BrowserView"))
            {
                browserView.CallStatic("launchUrl", activity, url);
            }

#elif UNITY_IOS
            var uri = new Uri(url);
            launch_face_webview(url, SafeWebviewProtocol.Scheme, objectName);
#endif
        }

        public void OnDeepLinkActivated(string url)
        {
            this._handleDeepLink(new Uri(url));
        }

        public void SendMessage(RpcRequestMessage message, Func<FaceRpcResponse, bool> callbackHandler)
        {
            this._handlerDictionary.Add(message.Id.ToString(), callbackHandler);
            
            string redirectUri = "";
#if UNITY_WEBGL
            Iframe.SendChildMessage(message);
            Iframe.WaitForResponse(message.Id.ToString(), ResponseCallback);
            return;
#elif UNITY_EDITOR || UNITY_STANDALONE
            redirectUri = LocalTestWebServer.Start(this);
#elif UNITY_ANDROID || UNITY_IOS
            redirectUri = $"{FaceSettings.Instance.Scheme()}://{FaceSettings.Instance.Scheme()}" ;
#endif
            
            string queryParams = SafeWebviewProtocol.EncodeQueryParams(new SafeWebviewProtocol.Parameters
            {
                Request = message,
                ApiKey = FaceSettings.Instance.ApiKey(),
                Env = FaceSettings.Instance.Environment(),
                Blockchain = FaceSettings.Instance.Blockchain(),
                RedirectUrl = redirectUri,
                Hostname = this._hostname()
            });
            UriBuilder uriBuilder = new UriBuilder(FaceSettings.Instance.WebviewHostURL())
            {
                Query = queryParams
            };
            Debug.Log($"URL: {uriBuilder}");

            // Launch browser
            LaunchUrl(uriBuilder.ToString(), this.gameObject.name);
        }
        
        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ResponseCallback(string id, string res)
        {
            IframeResponse.Add(id, res);
        }

        private string _hostname()
        {
            return Application.identifier;
        }

        private void onFocusChanged(bool isFocused)
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
#if !UNITY_WEBGL
            Debug.Log($"URI Receive: {uri}");
            FaceRpcContext context = SafeWebviewProtocol.DecodeQueryParams(uri);
            Debug.Log($"Data received from webview: {JsonConvert.SerializeObject(context)}");

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
#endif
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