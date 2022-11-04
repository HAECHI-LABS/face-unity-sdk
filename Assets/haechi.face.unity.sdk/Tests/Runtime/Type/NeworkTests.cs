using haechi.face.unity.sdk.Runtime.Type;
using NUnit.Framework;

namespace haechi.face.unity.sdk.Tests.Runtime.Type
{
    public class NeworkTests
    {
        [Test]
        public void GetNetworkTest()
        {
            Assert.AreEqual(BlockchainNetwork.GOERLI, BlockchainNetworks.GetNetwork("ETHEREUM", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.ETHEREUM, BlockchainNetworks.GetNetwork("ETHEREUM", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.GOERLI, BlockchainNetworks.GetNetwork("ETHEREUM", "ProdTest"));
            Assert.AreEqual(BlockchainNetwork.ETHEREUM, BlockchainNetworks.GetNetwork("ETHEREUM", "ProdMainnet"));
            
            Assert.AreEqual(BlockchainNetwork.MUMBAI, BlockchainNetworks.GetNetwork("POLYGON", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.POLYGON, BlockchainNetworks.GetNetwork("POLYGON", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.MUMBAI, BlockchainNetworks.GetNetwork("POLYGON", "ProdTest"));
            Assert.AreEqual(BlockchainNetwork.POLYGON, BlockchainNetworks.GetNetwork("POLYGON", "ProdMainnet"));
            
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN_TESTNET, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN_TESTNET, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "ProdTest"));
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "ProdMainnet"));
            
            Assert.AreEqual(BlockchainNetwork.BAOBAB, BlockchainNetworks.GetNetwork("KLAYTN", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.KLAYTN, BlockchainNetworks.GetNetwork("KLAYTN", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.BAOBAB, BlockchainNetworks.GetNetwork("KLAYTN", "ProdTest"));
            Assert.AreEqual(BlockchainNetwork.KLAYTN, BlockchainNetworks.GetNetwork("KLAYTN", "ProdMainnet"));
        }
    }
}