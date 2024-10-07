using System;
using haechi.face.unity.sdk.Runtime.Type;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    [JsonObject]
    public class FaceRpcRequest<T> : RpcRequestMessage
    {
        public FaceRpcRequest(FaceRpcMethod method, params T[] parameterList)
            : base(_generateId(), Enum.GetName(typeof(FaceRpcMethod), method),
                _parameterize(parameterList))
        {
#if UNITY_WEBGL
            this.From = "FACE_SDK";
#else
            this.From = "FACE_NATIVE_SDK";
#endif
            this.To = "FACE_IFRAME";
        }

        private static int _generateId()
        {
            Random rand = new Random();
            return rand.Next(1, 100000);
        }

        private static object[] _parameterize(params T[] parameterList)
        {
            object[] result = new object[parameterList.Length];
            parameterList.CopyTo(result, 0);
            return result;
        }
        
        private static object[] _parameterize(Object parameterList)
        {
            object[] convertList = (object[])parameterList;
            object[] result = new object[convertList.Length];
            convertList.CopyTo(result, 0);
            return result;
        }

        public FaceRpcRequest(BlockchainNetwork blockchainNetwork, FaceRpcMethod method, params T[] parameterList) 
            : base(_generateId(), Enum.GetName(typeof(FaceRpcMethod), method), 
                _parameterize(parameterList))
        {
            this.BlockchainNetwork = Enum.GetName(typeof(BlockchainNetwork), blockchainNetwork);
#if UNITY_WEBGL
            this.From = "FACE_SDK";
#else
            this.From = "FACE_NATIVE_SDK";
#endif
            this.To = "FACE_IFRAME";
        }
        
        public FaceRpcRequest(Blockchain blockchain, string method, params T[] parameterList) 
            : base(_generateId(), method, _parameterize(parameterList))
        {
            this.BlockchainNetwork = Enum.GetName(typeof(BlockchainNetwork), blockchain);
#if UNITY_WEBGL
            this.From = "FACE_SDK";
#else
            this.From = "FACE_NATIVE_SDK";
#endif
            this.To = "FACE_IFRAME";
        }  
        
        public FaceRpcRequest(Blockchain blockchain, RpcRequestMessage message) 
            : base(_generateId(), message.Method, _parameterize(message.RawParameters))
        {
            this.BlockchainNetwork = Enum.GetName(typeof(Blockchain), blockchain);
        }

        [JsonProperty("from", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string From { get; private set; }
        
        [JsonProperty("to", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; private set; }
        
        [JsonProperty("blockchainNetwork", NullValueHandling = NullValueHandling.Ignore)]
        public string BlockchainNetwork { get; private set; }
    }
}