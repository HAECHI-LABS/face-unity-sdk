using System.Collections.Generic;

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
        public static Dictionary<Blockchain, Dictionary<Profile, Network>> Map =
            new Dictionary<Blockchain, Dictionary<Profile, Network>>
            {
                {
                    Blockchain.ETHEREUM, new Dictionary<Profile, Network>
                    {
                        { Profile.StageTest, Network.ROPSTEN },
                        { Profile.StageMainnet, Network.ETHEREUM },
                        { Profile.ProdTest, Network.ROPSTEN },
                        { Profile.ProdMainnet, Network.ETHEREUM },
                    }
                },
                {
                    Blockchain.KLAYTN, new Dictionary<Profile, Network>
                    {
                        { Profile.StageTest, Network.BAOBAB },
                        { Profile.StageMainnet, Network.KLAYTN },
                        { Profile.ProdTest, Network.BAOBAB },
                        { Profile.ProdMainnet, Network.KLAYTN },
                    }
                },
                {
                    Blockchain.BNB_SMART_CHAIN, new Dictionary<Profile, Network>
                    {
                        { Profile.StageTest, Network.BNB_SMART_CHAIN_TESTNET },
                        { Profile.StageMainnet, Network.BNB_SMART_CHAIN },
                        { Profile.ProdTest, Network.BNB_SMART_CHAIN_TESTNET },
                        { Profile.ProdMainnet, Network.BNB_SMART_CHAIN },
                    }
                },
                {
                    Blockchain.POLYGON, new Dictionary<Profile, Network>
                    {
                        { Profile.StageTest, Network.MUMBAI },
                        { Profile.StageMainnet, Network.POLYGON },
                        { Profile.ProdTest, Network.MUMBAI },
                        { Profile.ProdMainnet, Network.POLYGON },
                    }
                },
            };
    }
}