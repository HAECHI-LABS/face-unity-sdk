using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    internal static class SafeWebviewProtocol
    {
        public struct Parameters
        {
            public RpcRequestMessage Request;
            public string ApiKey;
            public Profile Env;
            public Blockchain Blockchain;

        }
        public static string Scheme = "facewebview";

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
                   $"blockchain={parameters.Blockchain}";
        }
        
        public static FaceRpcResponse DecodeQueryParams(Uri uri) 
        {
            Dictionary<string, string> queryParameters = _parseQuery(uri.Query);
            if (!queryParameters.TryGetValue("response", out string encodedResponse))
            {
                throw new InvalidWebviewMessageException();
            }
            string responseData = HttpUtility.UrlDecode(encodedResponse);
            try
            {
                return JsonConvert.DeserializeObject<FaceRpcResponse>(responseData);
            }
            catch (JsonException e)
            {
                throw new InvalidRpcResponse(e);
            }
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
    }
}