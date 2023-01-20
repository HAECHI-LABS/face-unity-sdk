using haechi.face.unity.sdk.Runtime.Module;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public class PairRequestEvent
    {
        public string address { get; set; }
        public string uri { get; set; }

        public Wallet faceWallet { get; set; }
    }
}