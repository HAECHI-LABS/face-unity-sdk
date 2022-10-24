using System;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    [JsonObject]
    public class FaceRpcRequest<T> : RpcRequestMessage
    {
        private FaceRpcRequest(FaceRpcMethod method, params T[] parameterList)
            : base(_generateId(), Enum.GetName(typeof(FaceRpcMethod), method),
                _parameterize(parameterList))
        {
            this.From = "FACE_NATIVE_SDK";
            this.To = "FACE_IFRAME";
        }

        [JsonProperty("from", Required = Required.Always)]
        public string From { get; private set; }

        [JsonProperty("to", Required = Required.Always)]
        public string To { get; private set; }

        private static int _generateId()
        {
            return Random.Range(1, 100000);
        }

        private static object[] _parameterize(params T[] parameterList)
        {
            object[] result = new object[parameterList.Length];
            parameterList.CopyTo(result, 0);
            return result;
        }

        public static FaceRpcRequest<T> Of(FaceRpcMethod method, params T[] parameterList)
        {
            return new FaceRpcRequest<T>(method, parameterList);
        }
    }
}