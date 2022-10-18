using System;
using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum Network
    {
        ETHEREUM,
        GOERLI,
        POLYGON,
        MUMBAI,
        BNB_SMART_CHAIN,
        BNB_SMART_CHAIN_TESTNET,
        KLAYTN,
        BAOBAB
    }

    public static class NetworkResolver
    {
        public static Network ValueOf(string network)
        {
            return EnumUtils.FindEquals<Network>(network);
        }

        public static string GetNetwork(string blockchain, string profile)
        {
            if (!Blockchains.Map.TryGetValue(EnumUtils.FindEquals<Blockchain>(blockchain),
                    out Dictionary<Profile, Network> profileNetworkMap))
            {
                throw new ArgumentException($"Unknown blockchain {blockchain}");
            }

            if (!profileNetworkMap.TryGetValue(EnumUtils.FindEquals<Profile>(profile), out Network network))
            {
                throw new ArgumentException($"Unknown env {profile}");
            }

            return network.ToString().ToLower();
        }

        public static int GetChainId(this Network network)
        {
            switch (network)
            {
                case Network.ETHEREUM:
                    return 1;
                case Network.GOERLI:
                    return 5;
                case Network.POLYGON:
                    return 137;
                case Network.MUMBAI:
                    return 80001;
                case Network.BNB_SMART_CHAIN:
                    return 56;
                case Network.BNB_SMART_CHAIN_TESTNET:
                    return 97;
                case Network.KLAYTN:
                    return 8217;
                case Network.BAOBAB:
                    return 1001;
                default:
                    return 1;
            }
        }

        public static int GetChainId(string network)
        {
            return ValueOf(network).GetChainId();
        }
    }
}