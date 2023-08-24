using System;
using haechi.face.unity.sdk.Runtime.Client.Face;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    [JsonObject]
    [Serializable]
    public class SendTransaction : RawTransaction
    {
      
        [SerializeField] [JsonProperty("from")]
        internal string from;

        [SerializeField] [JsonProperty("to")] 
        internal string to;

        [SerializeField] [JsonProperty("data")]
        internal string data;
        
        [SerializeField] [JsonProperty("value")]
        internal string value;
        
        [SerializeField] [JsonProperty("nonce")]
        internal string nonce;   
        
        [SerializeField] [JsonProperty("gasPrice")]
        internal string gasPrice;
        
        [SerializeField] [JsonProperty("gasLimit")]
        internal string gasLimit;
        

        /// <summary>
        /// Raw transaction data using for send transaction, contract call etc.
        /// </summary>
        /// <param name="from">From address.</param>
        /// <param name="to">To address.</param>
        /// <param name="value">Blockchain value. Should be hex format.</param>
        /// <param name="data">Data string.</param>
        public SendTransaction(string from, string to, string value, string data)
            :base(from, to, value, data)
        {
            this.from = from;
            this.to = to;
            this.value = value;
            this.data = data;
        }
    }
}