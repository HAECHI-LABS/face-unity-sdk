using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum BlockchainNetwork
    {
        ETHEREUM,
        GOERLI,
        POLYGON,
        MUMBAI,
        BNB_SMART_CHAIN,
        BNB_SMART_CHAIN_TESTNET,
        KLAYTN,
        BAOBAB,
        MEVERSE,
        MEVERSE_TESTNET,
        BORA,
        BORA_TESTNET,
    }

    public static class BlockchainNetworks
    {
        public static BlockchainNetwork ValueOf(string network)
        {
            return EnumUtils.FindEquals<BlockchainNetwork>(network);
        }

        public static BlockchainNetwork GetNetwork(Blockchain blockchain, Profile profile)
        {
            return GetNetwork(blockchain.ToString(), profile.ToString());
        }

        public static BlockchainNetwork GetNetwork(string blockchain, string profile)
        {
            if (!Blockchains.Map.TryGetValue(Blockchains.ValueOf(blockchain), out Dictionary<Profile, BlockchainNetwork> profileNetworkMap))
            {
                throw new ArgumentException($"Unknown blockchain {blockchain}");
            }

            if (!profileNetworkMap.TryGetValue(Profiles.ValueOf(profile), out BlockchainNetwork network))
            {
                throw new ArgumentException($"Unknown env {profile}");
            }

            return network;
        }

        public static string String(this BlockchainNetwork network)
        {
            return network.ToString().ToLower();
        }

        public static int GetChainId(this BlockchainNetwork blockchainNetwork)
        {
            switch (blockchainNetwork)
            {
                case BlockchainNetwork.ETHEREUM:
                    return 1;
                case BlockchainNetwork.GOERLI:
                    return 5;
                case BlockchainNetwork.POLYGON:
                    return 137;
                case BlockchainNetwork.MUMBAI:
                    return 80001;
                case BlockchainNetwork.BNB_SMART_CHAIN:
                    return 56;
                case BlockchainNetwork.BNB_SMART_CHAIN_TESTNET:
                    return 97;
                case BlockchainNetwork.KLAYTN:
                    return 8217;
                case BlockchainNetwork.BAOBAB:
                    return 1001;
                case BlockchainNetwork.MEVERSE:
                    return 7518;
                case BlockchainNetwork.MEVERSE_TESTNET:
                    return 4759;
                case BlockchainNetwork.BORA:
                    return 77001;
                case BlockchainNetwork.BORA_TESTNET:
                    return 99001;
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