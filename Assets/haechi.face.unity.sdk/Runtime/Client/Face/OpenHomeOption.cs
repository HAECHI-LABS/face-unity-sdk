using System;
using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Type;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class OpenHomeOption
    {
        [JsonProperty("networks")]
        internal List<string> networks;

        private OpenHomeOption(List<string> networks)
        {
            this.networks = networks;
        }

        public static OpenHomeOption AllBlockchains()
        {
            return new OpenHomeOption(EnumUtils.AllEnumAsList<BlockchainNetwork>().ConvertAll(n => n.String()));
        }

        public static OpenHomeOption Of(List<BlockchainNetwork> networks)
        {
            return new OpenHomeOption(new List<BlockchainNetwork>(networks).ConvertAll(n => n.String()));
        }
    }
}