using System;
using System.Threading.Tasks;
using WalletConnectSharp.Sign.Models;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public class PairRequestEvent
    {
        public delegate Task<Boolean> WalletConnectPairEvent(Metadata metadata);

        public string address { get; set; }
        public string uri { get; set; }

        public WalletConnectPairEvent confirmWalletConnectDapp { get; set; }
    }
}