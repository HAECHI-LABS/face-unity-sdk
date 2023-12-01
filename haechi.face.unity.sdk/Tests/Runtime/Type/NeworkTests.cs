using haechi.face.unity.sdk.Runtime.Type;
using NUnit.Framework;

namespace haechi.face.unity.sdk.Tests.Runtime.Type
{
    public class NeworkTests
    {
        [Test]
        public void GetNetworkTest()
        {
            Assert.AreEqual(BlockchainNetwork.SEPOLIA, BlockchainNetworks.GetNetwork("ETHEREUM", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.ETHEREUM, BlockchainNetworks.GetNetwork("ETHEREUM", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.SEPOLIA, BlockchainNetworks.GetNetwork("ETHEREUM", "Testnet"));
            Assert.AreEqual(BlockchainNetwork.ETHEREUM, BlockchainNetworks.GetNetwork("ETHEREUM", "Mainnet"));
            
            Assert.AreEqual(BlockchainNetwork.MUMBAI, BlockchainNetworks.GetNetwork("POLYGON", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.POLYGON, BlockchainNetworks.GetNetwork("POLYGON", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.MUMBAI, BlockchainNetworks.GetNetwork("POLYGON", "Testnet"));
            Assert.AreEqual(BlockchainNetwork.POLYGON, BlockchainNetworks.GetNetwork("POLYGON", "Mainnet"));
            
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN_TESTNET, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN_TESTNET, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "Testnet"));
            Assert.AreEqual(BlockchainNetwork.BNB_SMART_CHAIN, BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "Mainnet"));
            
            Assert.AreEqual(BlockchainNetwork.BAOBAB, BlockchainNetworks.GetNetwork("KLAYTN", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.KLAYTN, BlockchainNetworks.GetNetwork("KLAYTN", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.BAOBAB, BlockchainNetworks.GetNetwork("KLAYTN", "Testnet"));
            Assert.AreEqual(BlockchainNetwork.KLAYTN, BlockchainNetworks.GetNetwork("KLAYTN", "Mainnet"));
            
            Assert.AreEqual(BlockchainNetwork.MEVERSE_TESTNET, BlockchainNetworks.GetNetwork("MEVERSE", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.MEVERSE, BlockchainNetworks.GetNetwork("MEVERSE", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.MEVERSE_TESTNET, BlockchainNetworks.GetNetwork("MEVERSE", "Testnet"));
            Assert.AreEqual(BlockchainNetwork.MEVERSE, BlockchainNetworks.GetNetwork("MEVERSE", "Mainnet"));
            
            Assert.AreEqual(BlockchainNetwork.BORA_TESTNET, BlockchainNetworks.GetNetwork("BORA", "StageTest"));
            Assert.AreEqual(BlockchainNetwork.BORA, BlockchainNetworks.GetNetwork("BORA", "StageMainnet"));
            Assert.AreEqual(BlockchainNetwork.BORA_TESTNET, BlockchainNetworks.GetNetwork("BORA", "Testnet"));
            Assert.AreEqual(BlockchainNetwork.BORA, BlockchainNetworks.GetNetwork("BORA", "Mainnet"));
        }
    }
}