using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        [SerializeField] private string _serviceUrl = "http://localhost:3000/login";
        
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();
        private readonly Dictionary<string, Func<TestResponse, bool>> _handlerDictionary
            = new Dictionary<string, Func<TestResponse, bool>>();
        
        private Dictionary<string, object> _initParams = new Dictionary<string, object>();

        private void Awake()
        {
            this._initParams["redirectUrl"] = "facewebview://facewebview";
            Application.deepLinkActivated += this.onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this.onDeepLinkActivated(Application.absoluteURL);   
            }
        }

        public void onDeepLinkActivated(string url)
        {
            Debug.Log($"onDeepLinkActivated: {url}");
            this._setResultUrl(new Uri(url));
        }

        public void Request(string id, string path, Func<TestResponse, bool> callbackHandler)
        {
            Debug.Log($"Register Handler with ID: {id}");
            this._handlerDictionary.Add(id, callbackHandler);
            this._initParams["id"] = id;
            
#if UNITY_STANDALONE || UNITY_EDITOR
            this._initParams["redirectUrl"] = StartLocalWebServer();
#endif
            Dictionary<string, object> paramMap = new Dictionary<string, object>();
            paramMap["init"] = this._initParams;
            paramMap["params"] = (object)new Dictionary<string, object>();

            // if (extraParams != null && extraParams.Count > 0)
            //     foreach(KeyValuePair<string, object> item in extraParams)
            //     {
            //         (paramMap["params"] as Dictionary<string, object>) [item.Key] = item.Value;
            //     }

            string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(paramMap, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })));

            UriBuilder uriBuilder = new UriBuilder(_serviceUrl);
            uriBuilder.Path = path;
            uriBuilder.Fragment = hash;

            LaunchUrl(uriBuilder.ToString(), this._initParams["redirectUrl"].ToString(), gameObject.name);
        }
        
        public static void LaunchUrl(string url, string redirectUri = null, string objectName = null)
        {
            Debug.Log((new Uri(redirectUri)).Scheme);
            Debug.Log($"redirectUri: {redirectUri}, url: {url}");
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
    var uri = new Uri(redirectUri);
    web3auth_launch(url, uri.Scheme, objectName);
#endif
        }
        
#if UNITY_STANDALONE || UNITY_EDITOR
        private string StartLocalWebServer()
        {
            HttpListener httpListener = new HttpListener();

            var redirectUrl = $"http://localhost:{_getRandomUnusedPort()}";

            httpListener.Prefixes.Add($"{redirectUrl}/complete/");
            httpListener.Prefixes.Add($"{redirectUrl}/auth/");
            httpListener.Start();
            httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest), httpListener);

            return redirectUrl + "/complete/";
        }
        
        private static int _getRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private void IncomingHttpRequest(IAsyncResult result) 
        {
            // get back the reference to our http listener
            HttpListener httpListener = (HttpListener)result.AsyncState;

            // fetch the context object
            HttpListenerContext httpContext = httpListener.EndGetContext(result);

            // if we'd like the HTTP listener to accept more incoming requests, we'd just restart the "get context" here:
            // httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest),httpListener);
            // however, since we only want/expect the one, single auth redirect, we don't need/want this, now.
            // but this is what you would do if you'd want to implement more (simple) "webserver" functionality
            // in your project.

            // the context object has the request object for us, that holds details about the incoming request
            HttpListenerRequest httpRequest = httpContext.Request;
            HttpListenerResponse httpResponse = httpContext.Response;

            if (httpRequest.Url.LocalPath == "/complete/")
            {

                httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest), httpListener);

                var responseString = @"
                    <!DOCTYPE html>
                    <html>
                    <head>
                      <meta charset=""utf-8"">
                      <meta name=""viewport"" content=""width=device-width"">
                      <title>Web3Auth</title>
                      <link href=""https://fonts.googleapis.com/css2?family=DM+Sans:wght@500&display=swap"" rel=""stylesheet"">
                    </head>
                    <body style=""padding:0;margin:0;font-size:10pt;font-family: 'DM Sans', sans-serif;"">
                      <div style=""display:flex;align-items:center;justify-content:center;height:100vh;display: none;"" id=""success"">
                        <div style=""text-align:center"">
                           <h2 style=""margin-bottom:0""> Authenticated successfully</h2>
                           <p> You can close this tab/window now </p>
                        </div>
                      </div>
                      <div style=""display:flex;align-items:center;justify-content:center;height:100vh;display: none;"" id=""error"">
                        <div style=""text-align:center"">
                           <h2 style=""margin-bottom:0""> Authentication failed</h2>
                           <p> Please try again </p>
                        </div>
                      </div>
                      <script>
                        if (window.location.hash.trim() == """") {
                            document.querySelector(""#error"").style.display=""flex"";
                        } else {
                            fetch(`http://${window.location.host}/auth/?code=${window.location.hash.slice(1,window.location.hash.length)}`).then(function(response) {
                              console.log(response);
                              document.querySelector(""#success"").style.display=""flex"";
                            }).catch(function(error) {
                              console.log(error);
                              document.querySelector(""#error"").style.display=""flex"";
                            });
                        }
                        
                      </script>
                    </body>
                    </html>
                ";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                httpResponse.ContentLength64 = buffer.Length;
                System.IO.Stream output = httpResponse.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

            }

            if (httpRequest.Url.LocalPath == "/auth/")
            {
                var responseString = @"ok";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                httpResponse.ContentLength64 = buffer.Length;
                System.IO.Stream output = httpResponse.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                string code = httpRequest.QueryString.Get("code");
                if (!string.IsNullOrEmpty(code))
                {
                    this._setResultUrl(new Uri($"http://localhost#{code}"));
                }

                httpListener.Close();
            }
        }
#endif
        private void _setResultUrl(Uri uri)
        {
            string hash = uri.Fragment;
            if (hash == null)
                throw new UserCancelledException();

            hash = hash.Remove(0, 1);
            Debug.Log($"result hash: {hash}");
            Dictionary<string, string> queryParameters = _parseQuery(uri.Query);

            if (queryParameters.Keys.Contains("error"))
                throw new UnKnownException(queryParameters["error"]);
            string responseData = Encoding.UTF8.GetString(_decodeBase64(hash));
            Debug.Log($"response: {responseData}");
            
            TestResponse response = JsonConvert.DeserializeObject<TestResponse>(responseData);
            Debug.Log($"response.Id: {response.Id}");

            if (!this._handlerDictionary.TryGetValue(response.Id, out Func<TestResponse, bool> callback))
            {
                Debug.Log($"Cannot find handler by id: {response.Id}");
                return;
            }
            
            callback(response);
            this._handlerDictionary.Remove(response.Id);
        }
        
        private static Dictionary<string, string> _parseQuery(string text)
        {
            if (text.Length > 0 && text[0] == '?')
                text = text.Remove(0, 1);

            var parts = text.Split('&').Where(x => !string.IsNullOrEmpty(x)).ToList();

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (parts.Count > 0)
            {
                result = parts.ToDictionary(
                    c => c.Split('=')[0],
                    c => Uri.UnescapeDataString(c.Split('=')[1])
                );
            }

            return result;
        }
        
        private  static byte[] _decodeBase64(string text)
        {
            var output = text;
            output = output.Replace('-', '+');
            output = output.Replace('_', '/');
            switch (output.Length % 4)
            {
                case 0: break;
                case 2: output += "=="; break;
                case 3: output += "="; break;
                default: throw new FormatException(text);
            }
            var converted = Convert.FromBase64String(output);
            return converted;
        }

        public void SendMessage(string id, RpcRequestMessage message, Func<RpcResponseMessage, bool> callbackHandler)
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    [JsonObject]
    public class TestResponse
    {
        [JsonConstructor]
        public TestResponse() {}
        
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }
        
        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }

        public override string ToString()
        {
            return "{Id: " + this.Id + ", Message: " + this.Message + "}";
        }
    }
    
    public class UserCancelledException : Exception
    {
        public UserCancelledException(): base("User cancelled.") { }
    }
    
    public class UnKnownException : Exception
    {
        public UnKnownException(string error) : base("User cancelled.") { }
    }
}