using System;
using JetBrains.Annotations;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    [JsonObject]
    public class FaceRpcRequest<T> : RpcRequestMessage
    {
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

        public FaceRpcRequest(FaceRpcMethod method, params T[] parameterList) 
            : base(_generateId(), Enum.GetName(typeof(FaceRpcMethod), method), 
                _parameterize(parameterList))
        {
            this.From = "FACE_NATIVE_SDK";
        }
        
        public FaceRpcRequest(string method, params T[] parameterList) 
            : base(_generateId(), method, _parameterize(parameterList))
        {
            this.From = "FACE_NATIVE_SDK";
        }
        
        [JsonProperty("from", Required = Required.Always)]
        public string From { get; private set; }
    }
}