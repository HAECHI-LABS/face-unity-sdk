using haechi.face.unity.sdk.Runtime.Type;
using NUnit.Framework;

namespace haechi.face.unity.sdk.Tests.Runtime.Type
{
    public class NeworkTests
    {
        [Test]
        public void GetNetworkTest()
        {
            Assert.AreEqual("ROPSTEN", NetworkResolver.GetNetwork("ETHEREUM", "StageTest"));
            Assert.AreEqual("ETHEREUM", NetworkResolver.GetNetwork("ETHEREUM", "StageMainnet"));
            Assert.AreEqual("ROPSTEN", NetworkResolver.GetNetwork("ETHEREUM", "ProdTest"));
            Assert.AreEqual("ETHEREUM", NetworkResolver.GetNetwork("ETHEREUM", "ProdMainnet"));
            
            Assert.AreEqual("MUMBAI", NetworkResolver.GetNetwork("POLYGON", "StageTest"));
            Assert.AreEqual("POLYGON", NetworkResolver.GetNetwork("POLYGON", "StageMainnet"));
            Assert.AreEqual("MUMBAI", NetworkResolver.GetNetwork("POLYGON", "ProdTest"));
            Assert.AreEqual("POLYGON", NetworkResolver.GetNetwork("POLYGON", "ProdMainnet"));
            
            Assert.AreEqual("BNB_SMART_CHAIN_TESTNET", NetworkResolver.GetNetwork("BNB_SMART_CHAIN", "StageTest"));
            Assert.AreEqual("BNB_SMART_CHAIN", NetworkResolver.GetNetwork("BNB_SMART_CHAIN", "StageMainnet"));
            Assert.AreEqual("BNB_SMART_CHAIN_TESTNET", NetworkResolver.GetNetwork("BNB_SMART_CHAIN", "ProdTest"));
            Assert.AreEqual("BNB_SMART_CHAIN", NetworkResolver.GetNetwork("BNB_SMART_CHAIN", "ProdMainnet"));
            
            Assert.AreEqual("BAOBAB", NetworkResolver.GetNetwork("KLAYTN", "StageTest"));
            Assert.AreEqual("KLAYTN", NetworkResolver.GetNetwork("KLAYTN", "StageMainnet"));
            Assert.AreEqual("BAOBAB", NetworkResolver.GetNetwork("KLAYTN", "ProdTest"));
            Assert.AreEqual("KLAYTN", NetworkResolver.GetNetwork("KLAYTN", "ProdMainnet"));
        }
    }
}