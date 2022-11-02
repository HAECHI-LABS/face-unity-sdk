using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    public class TransactionRequestId
    {
        [JsonProperty("requestId")]
        public string RequestId;
        
        [JsonProperty("transactionId")]
        public string TransactionId;
    }
}