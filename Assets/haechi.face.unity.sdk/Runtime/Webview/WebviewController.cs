using System;
using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Client;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public class WebviewController
    {
        private readonly Dictionary<string, Func<FaceRpcResponse, bool>> _handlerDictionary
            = new Dictionary<string, Func<FaceRpcResponse, bool>>();

        private readonly string _webviewUri;
        private UniWebView _webView;
        private GameObject _webViewParent;

        public WebviewController(string webviewUri)
        {
            this._webviewUri = webviewUri;
            this.Init();
        }

        private void Init()
        {
            if (this._webView != null)
            {
                return;
            }
            UniWebView.SetJavaScriptEnabled(true);
            UniWebView.SetAllowJavaScriptOpenWindow(true);
            UniWebView.SetWebContentsDebuggingEnabled(true);
            this._webViewParent = new GameObject("UniWebView");
            this._webView = this._webViewParent.AddComponent<UniWebView>();
            this._webView.SetTransparencyClickingThroughEnabled(true);
            this._webView.BackgroundColor = Color.clear;
            this._webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            this._webView.AddUrlScheme("facewebview");
            this._webView.SetUserAgent(
                "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1");
            this._webView.SetSupportMultipleWindows(true, true);
            this._webView.SetTextZoom(100);

            this._webView.OnMultipleWindowOpened += (webView, id) =>
            {
                webView.SetTransparencyClickingThroughEnabled(false); 
                webView.SetShowSpinnerWhileLoading(true);
            };
            this._webView.OnMultipleWindowClosed += (webView, id) => { webView.SetTransparencyClickingThroughEnabled(true); };
            this._webView.OnShouldClose += webview =>
            {
                this._webView = null;
                UnityEngine.Object.Destroy(this._webViewParent);
                this.Init();
                return true;
            };
            
            this._webView.Load(this._webviewUri);
        }

        public void DispatchEvent(string id, string message, Func<FaceRpcResponse, bool> handler)
        {
            this._registerMessageHandler(id, handler);
            this._webView.Show();
            this._webView.EvaluateJavaScript(
                $"window.dispatchEvent(new MessageEvent('message', {{ 'data': '{message}' }}));");
            Debug.Log($"Sent message to Face SDK: {message}");
        }

        private void _registerMessageHandler(string id, Func<FaceRpcResponse, bool> handler)
        {
            this._handlerDictionary.Add(id, handler);

            void WrappedHandler(UniWebView webview, UniWebViewMessage message)
            {
                /*
                 * webview sends response in URI format.
                 * starts with "uniwebview://".
                 * message.Args returns uri query parameters.
                 */
                if (message.Args == null)
                {
                    return;
                }

                // NOTE: How about not using FaceRpcResponse type in WebviewController?
                Debug.Log($"Received Message: {message.RawMessage}");
                FaceRpcResponse response = FaceRpcResponse.OfDictionary(message.Args);

                Debug.Log($"Message Handled: {response.result}");
                Func<FaceRpcResponse, bool> handlerById;
                try
                {
                    handlerById = this._handlerDictionary[response.id];
                }
                catch (KeyNotFoundException e)
                {
                    Debug.LogWarning($"Handler with id {response.id} not found");
                    return;
                }

                if (handlerById == null)
                {
                    Debug.Log($"Cannot find handler by id: {response.id}");
                    return;
                }

                this._handlerDictionary.Remove(response.id);

                handlerById(response);

                this._webView.OnMessageReceived -= WrappedHandler;
            }

            this._webView.OnMessageReceived += WrappedHandler;
        }
    }
}