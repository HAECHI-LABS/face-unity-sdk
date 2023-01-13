using System;
using System.Threading;
using haechi.face.unity.sdk.Runtime.Type;
using JetBrains.Annotations;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

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
            this.From = "FACE_NATIVE_SDK";
            this.To = "FACE_IFRAME";
        }

        private static int _generateId()
        {
            System.Random rand = new System.Random();
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

        public FaceRpcRequest(Blockchain blockchain, FaceRpcMethod method, params T[] parameterList) 
            : base(_generateId(), Enum.GetName(typeof(FaceRpcMethod), method), 
                _parameterize(parameterList))
        {
            this.Blockchain = Enum.GetName(typeof(Blockchain), blockchain);
            this.From = "FACE_NATIVE_SDK";
            this.To = "FACE_IFRAME";
        }
        
        public FaceRpcRequest(Blockchain blockchain, string method, params T[] parameterList) 
            : base(_generateId(), method, _parameterize(parameterList))
        {
            this.Blockchain = Enum.GetName(typeof(Blockchain), blockchain);
            this.From = "FACE_NATIVE_SDK";
            this.To = "FACE_IFRAME";
        }  
        
        public FaceRpcRequest(Blockchain blockchain, RpcRequestMessage message) 
            : base(_generateId(), message.Method, _parameterize(message.RawParameters))
        {
            this.Blockchain = Enum.GetName(typeof(Blockchain), blockchain);
        }
        

        [JsonProperty("from", Required = Required.Always)]
        public string From { get; private set; }
        
        [JsonProperty("to", Required = Required.Always)]
        public string To { get; private set; }
        
        [JsonProperty("blockchain")]
        public string Blockchain { get; private set; }
    }
}