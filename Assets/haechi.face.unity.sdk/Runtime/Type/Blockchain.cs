using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum Blockchain
    {
        ETHEREUM,
        POLYGON,
        BNB_SMART_CHAIN,
        KLAYTN
    }

    public static class Blockchains
    {
        public static Blockchain ValueOf(string value)
        {
            return EnumUtils.FindEquals<Blockchain>(value);
        }
        
        public static Dictionary<Blockchain, Dictionary<Profile, BlockchainNetwork>> Map =
            new Dictionary<Blockchain, Dictionary<Profile, BlockchainNetwork>>
            {
                {
                    Blockchain.ETHEREUM, new Dictionary<Profile, BlockchainNetwork>
                    {
                        { Profile.Dev, BlockchainNetwork.GOERLI },
                        { Profile.StageTest, BlockchainNetwork.GOERLI },
                        { Profile.StageMainnet, BlockchainNetwork.ETHEREUM },
                        { Profile.ProdTest, BlockchainNetwork.GOERLI },
                        { Profile.ProdMainnet, BlockchainNetwork.ETHEREUM }
                    }
                },
                {
                    Blockchain.KLAYTN, new Dictionary<Profile, BlockchainNetwork>
                    {
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
                        { Profile.Dev, BlockchainNetwork.MUMBAI },
                        { Profile.StageTest, BlockchainNetwork.MUMBAI },
                        { Profile.StageMainnet, BlockchainNetwork.POLYGON },
                        { Profile.ProdTest, BlockchainNetwork.MUMBAI },
                        { Profile.ProdMainnet, BlockchainNetwork.POLYGON }
                    }
                }
            };
    }
}