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

    public class BlockchainNetworkProperty
    {
        public bool IsTestnet;
        public int ChainId;
        public Blockchain Blockchain;
    }

    public static class BlockchainNetworks
    {
        public static Dictionary<BlockchainNetwork, BlockchainNetworkProperty> Properties =
            new Dictionary<BlockchainNetwork, BlockchainNetworkProperty>
            {
                // Ethereum
                { BlockchainNetwork.ETHEREUM, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 1, Blockchain = Blockchain.ETHEREUM }},
                { BlockchainNetwork.GOERLI, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 5, Blockchain = Blockchain.ETHEREUM }},
                
                // Polygon
                { BlockchainNetwork.POLYGON, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 137, Blockchain = Blockchain.POLYGON }},
                { BlockchainNetwork.MUMBAI, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 80001, Blockchain = Blockchain.POLYGON }},
                
                // BSC
                { BlockchainNetwork.BNB_SMART_CHAIN, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 56, Blockchain = Blockchain.BNB_SMART_CHAIN }},
                { BlockchainNetwork.BNB_SMART_CHAIN_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 97, Blockchain = Blockchain.BNB_SMART_CHAIN }},
                
                // Klaytn
                { BlockchainNetwork.KLAYTN, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 8217, Blockchain = Blockchain.KLAYTN }},
                { BlockchainNetwork.BAOBAB, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 1001, Blockchain = Blockchain.KLAYTN }},
                
                // MEVerse
                { BlockchainNetwork.MEVERSE, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 7518, Blockchain = Blockchain.MEVERSE }},
                { BlockchainNetwork.MEVERSE_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 4759, Blockchain = Blockchain.MEVERSE }},
                
                // BORA
                { BlockchainNetwork.BORA, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 77001, Blockchain = Blockchain.BORA }},
                { BlockchainNetwork.BORA_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 99001, Blockchain = Blockchain.BORA }},
            };

        public static BlockchainNetwork ValueOf(string network)
        {
            return EnumUtils.FindEquals<BlockchainNetwork>(network.ToUpper());
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

        public static bool MatchWithProfile(this BlockchainNetwork blockchainNetwork, Profile profile)
        {
            return Properties[blockchainNetwork].IsTestnet == !profile.IsMainNet();
        }

        public static string String(this BlockchainNetwork network)
        {
            return network.ToString().ToLower();
        }

        public static int GetChainId(this BlockchainNetwork blockchainNetwork)
        {
            return Properties[blockchainNetwork].ChainId;
        }

        public static int GetChainId(string network)
        {
            return ValueOf(network).GetChainId();
        }
    }
}