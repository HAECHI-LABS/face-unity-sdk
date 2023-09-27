using System;
using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Type;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class LoginOption
    {
        [JsonProperty("networks")]
        internal List<string> providers;

        private LoginOption(List<string> providers)
        {
            this.providers = providers;
        }

        public static LoginOption All()
        {
            return new LoginOption(EnumUtils.AllEnumAsList<LoginProviderType>()
                .ConvertAll(n => n.HostValue()));
        }
        
        public static LoginOption of(List<LoginProviderType> providers)
        {
            return new LoginOption(new List<LoginProviderType>(providers)
                .ConvertAll(n => n.HostValue()));
        }
    }
}