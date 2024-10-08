using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using JetBrains.Annotations;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    internal static class SafeWebviewProtocol
    {
        public static string Scheme = FaceSettings.Instance.Scheme();
        
        public static string EncodeQueryParams(Parameters parameters)
        {
            byte[] requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parameters.Request,
                Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
            string encodedRequest = HttpUtility.UrlEncode(Encoding.UTF8.GetString(requestBytes));
            return $"request={encodedRequest}&" +
                   $"api_key={parameters.ApiKey}&" +
                   $"env={parameters.Env}&" +
                   $"network={parameters.Network.ToNetworkString()}&" +
                   $"hostname={parameters.Hostname}&" +
                   (!string.IsNullOrEmpty(parameters.RedirectUrl) ? $"redirectUrl={parameters.RedirectUrl}&" : "") +
                   $"webview_version={Face.WEBVIEW_VERSION}&" +
                   (!string.IsNullOrEmpty(parameters.Version) ? $"version={parameters.Version}&" : "") +
                   (!string.IsNullOrEmpty(parameters.Type) ? $"type={parameters.Type}" : "");
        }

        public static FaceRpcContext DecodeQueryParams(Uri uri)
        {
            Dictionary<string, string> queryParameters = _parseQuery(uri.Query);
            bool isResponse = queryParameters.ContainsKey("response");
           
            bool isRequest = queryParameters.ContainsKey("request");
            if ((!isResponse && !isRequest) || (isRequest && isResponse))
            {
                throw new InvalidWebviewMessageException();
            }

            return isResponse ? 
                new FaceRpcContext(_deserializeData<FaceRpcResponse>(queryParameters, "response")) : 
                new FaceRpcContext(_deserializeData<WebviewRpcRequest>(queryParameters, "request"));
        }

        private static T _deserializeData<T>(IReadOnlyDictionary<string, string> queryParameters, string key)
        {
            queryParameters.TryGetValue(key, out string encodedData);
            string data = HttpUtility.UrlDecode(encodedData);
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (JsonException e)
            {
                throw new InvalidRpcResponseException(e);
            }
        }

        private static Dictionary<string, string> _parseQuery(string text)
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(text);
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }

        public struct Parameters
        {
            public RpcRequestMessage Request;
            public string ApiKey;
            public Profile Env;
            public BlockchainNetwork Network;
            public string Hostname;
            [CanBeNull] public string RedirectUrl;
            [CanBeNull] public string Version;
            [CanBeNull] public string Type;
        }
    }
}