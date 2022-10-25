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
        
        [JsonProperty("createdAt")]
        public DateTime CreatedAt;
        
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt;
        
        [JsonProperty("deletedAt")]
        public DateTime DeletedAt;
    }
}