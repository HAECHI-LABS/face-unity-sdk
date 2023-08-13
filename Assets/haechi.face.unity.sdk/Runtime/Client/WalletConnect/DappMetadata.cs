using System;
using Newtonsoft.Json;
using WalletConnectSharp.Core.Models.Pairing;
using WalletConnectSharp.Sign.Models;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    [Serializable]
    public class DappMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("icons")]
        public string[] Icons { get; set; }
        
        public DappMetadata(Metadata metadata)
        {
            this.Name = metadata.Name;
            this.Description = metadata.Description;
            this.Url = metadata.Url;
            this.Icons = metadata.Icons;
        }

        protected bool Equals(DappMetadata other)
        {
            return this.Name == other.Name && this.Description == other.Description && this.Url == other.Url && Equals(this.Icons, other.Icons);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((DappMetadata)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name, this.Description, this.Url, this.Icons);
        }
    }
}