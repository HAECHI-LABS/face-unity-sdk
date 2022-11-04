using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    public class TransactionRequestId
    {
        /// <value>
        /// Unique rpc request id.
        /// </value>
        [JsonProperty("requestId")]
        public string requestId;
        
        /// <value>
        /// Transaction hash string.
        /// </value>
        [JsonProperty("transactionId")]
        public string transactionId;
    }
}