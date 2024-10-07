using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum BlockchainNetwork
    {
        ETHEREUM,
        SEPOLIA,
        POLYGON,
        AMOY,
        BNB_SMART_CHAIN,
        BNB_SMART_CHAIN_TESTNET,
        KLAYTN,
        BAOBAB,
        MEVERSE,
        MEVERSE_TESTNET,
        DEFI_VERSE,
        DEFI_VERSE_TESTNET,
        BORA,
        BORA_TESTNET,
        DM2VERSE,
        DM2VERSE_TESTNET,
    }

    public static class EnumExtensions {
        public static string ToNetworkString(this BlockchainNetwork blockchainNetwork)
        {
            return Enum.GetName(typeof(BlockchainNetwork), blockchainNetwork).ToLower();
        }
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
                { BlockchainNetwork.SEPOLIA, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 5, Blockchain = Blockchain.ETHEREUM }},
                
                // Polygon
                { BlockchainNetwork.POLYGON, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 137, Blockchain = Blockchain.POLYGON }},
                { BlockchainNetwork.AMOY, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 80002, Blockchain = Blockchain.POLYGON }},
                
                // BSC
                { BlockchainNetwork.BNB_SMART_CHAIN, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 56, Blockchain = Blockchain.BNB_SMART_CHAIN }},
                { BlockchainNetwork.BNB_SMART_CHAIN_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 97, Blockchain = Blockchain.BNB_SMART_CHAIN }},
                
                // Klaytn
                { BlockchainNetwork.KLAYTN, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 8217, Blockchain = Blockchain.KLAYTN }},
                { BlockchainNetwork.BAOBAB, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 1001, Blockchain = Blockchain.KLAYTN }},
                
                // MEVerse
                { BlockchainNetwork.MEVERSE, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 7518, Blockchain = Blockchain.MEVERSE }},
                { BlockchainNetwork.MEVERSE_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 4759, Blockchain = Blockchain.MEVERSE }},
                
                // DeFiVerse
                { BlockchainNetwork.DEFI_VERSE, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 16116, Blockchain = Blockchain.DEFI_VERSE }},
                { BlockchainNetwork.DEFI_VERSE_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 17117, Blockchain = Blockchain.DEFI_VERSE }},
                
                // BORA
                { BlockchainNetwork.BORA, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 77001, Blockchain = Blockchain.BORA }},
                { BlockchainNetwork.BORA_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 99001, Blockchain = Blockchain.BORA }},
                { BlockchainNetwork.DM2VERSE, new BlockchainNetworkProperty() { IsTestnet = false, ChainId = 68770, Blockchain = Blockchain.DM2VERSE }},
                { BlockchainNetwork.DM2VERSE_TESTNET, new BlockchainNetworkProperty() { IsTestnet = true, ChainId = 68775, Blockchain = Blockchain.DM2VERSE }},
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

        public static string String(this BlockchainNetwork network)
        {
            return network.ToNetworkString();
        }

        public static int GetChainId(this BlockchainNetwork blockchainNetwork)
        {
            return Properties[blockchainNetwork].ChainId;
        }

        public static int GetChainId(string network)
        {
            return ValueOf(network).GetChainId();
        }

        internal static List<BlockchainNetwork> GetAllNetworks()
        {
            return EnumUtils.AllEnumAsList<BlockchainNetwork>();
        }
    }
}