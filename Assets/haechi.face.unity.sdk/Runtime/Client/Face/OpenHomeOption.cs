using System;
using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Exception;
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

        public static OpenHomeOption AllBlockchains(Profile profile)
        {
            return new OpenHomeOption(EnumUtils.AllEnumAsList<BlockchainNetwork>()
                .FindAll(n => n.MatchWithProfile(profile))
                .ConvertAll(n => n.String()));
        }

        public static OpenHomeOption Of(Profile profile, List<BlockchainNetwork> networks)
        {
            CheckNetworkValidity(profile, networks);
            
            EnumUtils.AllEnumAsList<BlockchainNetwork>()
                .ConvertAll(n => n.String());
            return new OpenHomeOption(new List<BlockchainNetwork>(networks).ConvertAll(n => n.String()));
        }

        private static void CheckNetworkValidity(Profile profile, List<BlockchainNetwork> networks)
        {
            if (networks.Count == 0)
            {
                throw new InvalidOpenHomeArguments("The 'networks' should select at least one network.");
            }
            
            string currentInitializedNetwork = profile.IsMainNet() ? "Mainnet" : "Testnet";
            foreach (BlockchainNetwork blockchainNetwork in networks)
            {
                if (blockchainNetwork.MatchWithProfile(profile))
                {
                    continue;
                }
                
                throw new InvalidOpenHomeArguments($"You initialized the Face SDK with {currentInitializedNetwork}." +
                                                   $"Please open the wallet home in the same environment as the initialized network.");
            }
        }
    }
}