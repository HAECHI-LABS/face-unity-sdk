using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Module;
using WalletConnectSharp.Sign.Models;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public class PairRequestEvent
    {
        public string address { get; set; }
        public string uri { get; set; }
        public delegate Task<FaceRpcResponse> ConfirmWalletConnectDapp(DappMetadata dappMetadata);
        public ConfirmWalletConnectDapp confirmWalletConnectDapp { get; set; }
    }
}