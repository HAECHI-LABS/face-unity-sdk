using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using haechi.face.unity.sdk.Runtime.Settings;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public class SafeWebviewController : MonoBehaviour, ISafeWebview
    {
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
        
        private readonly Dictionary<string, Func<RpcResponseMessage, bool>> _handlerDictionary
            = new Dictionary<string, Func<RpcResponseMessage, bool>>();
        
        private void Awake()
        {
            Application.deepLinkActivated += this.onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this.onDeepLinkActivated(Application.absoluteURL);   
            }
        }

        public void onDeepLinkActivated(string url)
        {
            this._handleDeepLink(new Uri(url));
        }
        
        // TODO: remove `id` parameter. Get ID from `message`
        public void SendMessage(string id, RpcRequestMessage message, Func<RpcResponseMessage, bool> callbackHandler)
        {
            Debug.Log($"Register Handler with ID: {id}");
            this._handlerDictionary.Add(id, callbackHandler);
            
            string queryParams = SafeWebviewProtocol.EncodeQueryParams(new SafeWebviewProtocol.Parameters
            {
                Request = message,
                ApiKey = FaceSettings.Instance.ApiKey,
                Env = FaceSettings.Instance.Environment(),
                Blockchain = FaceSettings.Instance.Blockchain(),
                
            });
            UriBuilder uriBuilder = new UriBuilder(FaceSettings.Instance.WebviewHostURL())
            {
                Query = queryParams
            };
            Debug.Log($"URL: {uriBuilder}");
            
            // Launch browser
            LaunchUrl(uriBuilder.ToString(), gameObject.name);
        }

        private void _handleDeepLink(Uri uri)
        {
            Debug.Log($"URI Receive: {uri}");
            RpcResponseMessage response = SafeWebviewProtocol.DecodeQueryParams(uri);

            if (!this._handlerDictionary.TryGetValue(response.Id.ToString(), out Func<RpcResponseMessage, bool> callback))
            {
                Debug.Log($"Cannot find handler by id: {response.Id}");
                return;
            }
            
            callback(response);
            this._handlerDictionary.Remove(response.Id.ToString());
        }
    }
}