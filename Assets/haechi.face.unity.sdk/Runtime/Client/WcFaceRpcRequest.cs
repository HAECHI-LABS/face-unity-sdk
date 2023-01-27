using System;
using haechi.face.unity.sdk.Runtime.Client.WalletConnect;
using haechi.face.unity.sdk.Runtime.Type;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    [JsonObject]
    public class WcFaceRpcRequest<T> : FaceRpcRequest<T>
    {

        public WcFaceRpcRequest(Blockchain blockchain, FaceRpcMethod method, WcFaceMetadata metadata, params T[] parameterList) :
            base(blockchain, method, parameterList)
        {
            this.Metadata = metadata;
        }

        
        [JsonProperty("metaData", NullValueHandling = NullValueHandling.Ignore)]
        public WcFaceMetadata Metadata { get; private set; }
    }
}