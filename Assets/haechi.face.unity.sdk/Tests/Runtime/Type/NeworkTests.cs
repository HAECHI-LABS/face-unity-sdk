using haechi.face.unity.sdk.Runtime.Type;
using NUnit.Framework;

namespace haechi.face.unity.sdk.Tests.Runtime.Type
{
    public class NeworkTests
    {
        [Test]
        public void GetNetworkTest()
        {
            Assert.AreEqual("ROPSTEN", BlockchainNetworks.GetNetwork("ETHEREUM", "StageTest"));
            Assert.AreEqual("ETHEREUM", BlockchainNetworks.GetNetwork("ETHEREUM", "StageMainnet"));
            Assert.AreEqual("ROPSTEN", BlockchainNetworks.GetNetwork("ETHEREUM", "ProdTest"));
            Assert.AreEqual("ETHEREUM", BlockchainNetworks.GetNetwork("ETHEREUM", "ProdMainnet"));
            
            Assert.AreEqual("MUMBAI", BlockchainNetworks.GetNetwork("POLYGON", "StageTest"));
            Assert.AreEqual("POLYGON", BlockchainNetworks.GetNetwork("POLYGON", "StageMainnet"));
            Assert.AreEqual("MUMBAI", BlockchainNetworks.GetNetwork("POLYGON", "ProdTest"));
            Assert.AreEqual("POLYGON", BlockchainNetworks.GetNetwork("POLYGON", "ProdMainnet"));
            
            Assert.AreEqual("BNB_SMART_CHAIN_TESTNET", BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "StageTest"));
            Assert.AreEqual("BNB_SMART_CHAIN", BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "StageMainnet"));
            Assert.AreEqual("BNB_SMART_CHAIN_TESTNET", BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "ProdTest"));
            Assert.AreEqual("BNB_SMART_CHAIN", BlockchainNetworks.GetNetwork("BNB_SMART_CHAIN", "ProdMainnet"));
            
            Assert.AreEqual("BAOBAB", BlockchainNetworks.GetNetwork("KLAYTN", "StageTest"));
            Assert.AreEqual("KLAYTN", BlockchainNetworks.GetNetwork("KLAYTN", "StageMainnet"));
            Assert.AreEqual("BAOBAB", BlockchainNetworks.GetNetwork("KLAYTN", "ProdTest"));
            Assert.AreEqual("KLAYTN", BlockchainNetworks.GetNetwork("KLAYTN", "ProdMainnet"));
        }
    }
}