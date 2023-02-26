using System;
using Newtonsoft.Json;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharpV1.Core.Models;

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
        
        // TODO: Add 'deprecated' explicitly after 28th, June, 2023.
        public DappMetadata(ClientMeta clientMeta)
        {
            this.Name = clientMeta.Name;
            this.Description = clientMeta.Description;
            this.Url = clientMeta.URL;
            this.Icons = clientMeta.Icons;
        }
        
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