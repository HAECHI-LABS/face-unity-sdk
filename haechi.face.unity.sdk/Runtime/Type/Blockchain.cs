using System;
using System.Collections.Generic;
using System.ComponentModel;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    [System.Obsolete("This enum is deprecated. you should use BlockchainNetwork instead.")]
    public enum Blockchain
    {
        ETHEREUM,
        POLYGON,
        BNB_SMART_CHAIN,
        KLAYTN,
        MEVERSE,
        DEFI_VERSE,
        BORA,
        DM2VERSE,
    }

    public static class Blockchains
    {
        public static Blockchain OfBlockchainNetwork(BlockchainNetwork network)
        {
            switch (network)
            {
                case BlockchainNetwork.ETHEREUM:
                case BlockchainNetwork.SEPOLIA:
                    return Blockchain.ETHEREUM;
                case BlockchainNetwork.POLYGON:
                case BlockchainNetwork.AMOY:
                    return Blockchain.POLYGON;
                case BlockchainNetwork.BNB_SMART_CHAIN:
                case BlockchainNetwork.BNB_SMART_CHAIN_TESTNET:
                    return Blockchain.BNB_SMART_CHAIN;
                case BlockchainNetwork.KLAYTN:
                case BlockchainNetwork.BAOBAB:
                    return Blockchain.KLAYTN;
                case BlockchainNetwork.MEVERSE:
                case BlockchainNetwork.MEVERSE_TESTNET:
                    return Blockchain.MEVERSE;
                case BlockchainNetwork.DEFI_VERSE:
                case BlockchainNetwork.DEFI_VERSE_TESTNET:
                    return Blockchain.DEFI_VERSE;
                case BlockchainNetwork.BORA:
                case BlockchainNetwork.BORA_TESTNET:
                    return Blockchain.BORA;
                case BlockchainNetwork.DM2VERSE:
                case BlockchainNetwork.DM2VERSE_TESTNET:
                    return Blockchain.DM2VERSE;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
        
        public static Blockchain OfChainId(int chainId)
        {
            foreach (var network in BlockchainNetworks.Properties)
            {
                if (network.Value.ChainId != chainId)
                {
                    continue;
                }
                return OfBlockchainNetwork(network.Key);
            }
    
            throw new ArgumentException($"Unknown chainId {chainId}");
        }

        public static Blockchain ValueOf(string value)
        {
            return EnumUtils.FindEquals<Blockchain>(value);
        }
        
        public static int GetPlatformCoinDecimals(this Blockchain blockchain)
        {
            switch (blockchain)
            {
                case Blockchain.ETHEREUM:
                case Blockchain.POLYGON:
                case Blockchain.BNB_SMART_CHAIN:
                case Blockchain.KLAYTN:
                case Blockchain.MEVERSE:
                case Blockchain.DEFI_VERSE:
                case Blockchain.BORA:
                case Blockchain.DM2VERSE:
                default:
                    return 18;
            }
        }

        [System.Obsolete("This method is deprecated. you should not rely on Blockchain type")]
        public static Dictionary<Blockchain, Dictionary<Profile, BlockchainNetwork>> Map =
            new Dictionary<Blockchain, Dictionary<Profile, BlockchainNetwork>>
            {
                {
                    Blockchain.ETHEREUM, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.SEPOLIA },
                        { Profile.Dev, BlockchainNetwork.SEPOLIA },
                        { Profile.StageTest, BlockchainNetwork.SEPOLIA },
                        { Profile.StageMainnet, BlockchainNetwork.ETHEREUM },
                        { Profile.ProdTest, BlockchainNetwork.SEPOLIA },
                        { Profile.ProdMainnet, BlockchainNetwork.ETHEREUM }
                    }
                },
                {
                    Blockchain.KLAYTN, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.BAOBAB },
                        { Profile.Dev, BlockchainNetwork.BAOBAB },
                        { Profile.StageTest, BlockchainNetwork.BAOBAB },
                        { Profile.StageMainnet, BlockchainNetwork.KLAYTN },
                        { Profile.ProdTest, BlockchainNetwork.BAOBAB },
                        { Profile.ProdMainnet, BlockchainNetwork.KLAYTN }
                    }
                },
                {
                    Blockchain.BNB_SMART_CHAIN, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.BNB_SMART_CHAIN_TESTNET },
                        { Profile.Dev, BlockchainNetwork.BNB_SMART_CHAIN_TESTNET },
                        { Profile.StageTest, BlockchainNetwork.BNB_SMART_CHAIN_TESTNET },
                        { Profile.StageMainnet, BlockchainNetwork.BNB_SMART_CHAIN },
                        { Profile.ProdTest, BlockchainNetwork.BNB_SMART_CHAIN_TESTNET },
                        { Profile.ProdMainnet, BlockchainNetwork.BNB_SMART_CHAIN }
                    }
                },
                {
                    Blockchain.POLYGON, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.AMOY },
                        { Profile.Dev, BlockchainNetwork.AMOY },
                        { Profile.StageTest, BlockchainNetwork.AMOY },
                        { Profile.StageMainnet, BlockchainNetwork.POLYGON },
                        { Profile.ProdTest, BlockchainNetwork.AMOY },
                        { Profile.ProdMainnet, BlockchainNetwork.POLYGON }
                    }
                },
                {
                    Blockchain.MEVERSE, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.MEVERSE_TESTNET },
                        { Profile.Dev, BlockchainNetwork.MEVERSE_TESTNET },
                        { Profile.StageTest, BlockchainNetwork.MEVERSE_TESTNET },
                        { Profile.StageMainnet, BlockchainNetwork.MEVERSE },
                        { Profile.ProdTest, BlockchainNetwork.MEVERSE_TESTNET },
                        { Profile.ProdMainnet, BlockchainNetwork.MEVERSE }
                    }
                },
                {
                    Blockchain.DEFI_VERSE, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.DEFI_VERSE_TESTNET },
                        { Profile.Dev, BlockchainNetwork.DEFI_VERSE_TESTNET },
                        { Profile.StageTest, BlockchainNetwork.DEFI_VERSE_TESTNET },
                        { Profile.StageMainnet, BlockchainNetwork.DEFI_VERSE },
                        { Profile.ProdTest, BlockchainNetwork.DEFI_VERSE_TESTNET },
                        { Profile.ProdMainnet, BlockchainNetwork.DEFI_VERSE }
                    }
                },
                {
                    Blockchain.BORA, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.BORA_TESTNET },
                        { Profile.Dev, BlockchainNetwork.BORA_TESTNET },
                        { Profile.StageTest, BlockchainNetwork.BORA_TESTNET },
                        { Profile.StageMainnet, BlockchainNetwork.BORA },
                        { Profile.ProdTest, BlockchainNetwork.BORA_TESTNET },
                        { Profile.ProdMainnet, BlockchainNetwork.BORA }
                    }
                },
                {
                    Blockchain.DM2VERSE, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Local, BlockchainNetwork.DM2VERSE_TESTNET },
                        { Profile.Dev, BlockchainNetwork.DM2VERSE_TESTNET },
                        { Profile.StageTest, BlockchainNetwork.DM2VERSE_TESTNET },
                        { Profile.StageMainnet, BlockchainNetwork.DM2VERSE },
                        { Profile.ProdTest, BlockchainNetwork.DM2VERSE_TESTNET },
                        { Profile.ProdMainnet, BlockchainNetwork.DM2VERSE }
                    }
                }
            };
    }
}