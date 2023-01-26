using System;
using Newtonsoft.Json;
using WalletConnectSharp.Sign.Models;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    [Serializable]
    [JsonObject]
    public class WcFaceMetadata
    {
        public WcFaceMetadata(string name, string url, string thumbnail)
        {
            this.AppName = name;
            this.AppUrl = url;
            this.Thumbnail = thumbnail;
        }

        public static WcFaceMetadata Converted(Metadata metadata)
        {
            return new WcFaceMetadata(metadata.Name, 
                metadata.Url,
                metadata.Icons.Length > 0 ? metadata.Icons[0] : null);
        }
        
        [JsonProperty("appName", Required = Required.Always)]
        public string AppName { get; private set; }
        [JsonProperty("appUrl", Required = Required.Always)]
        public string AppUrl { get; private set; }
        [JsonProperty("thumbnail", Required = Required.AllowNull)]
        public string Thumbnail { get; private set; }
    }
}