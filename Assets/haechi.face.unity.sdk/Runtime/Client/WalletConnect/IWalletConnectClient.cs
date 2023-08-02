using System.Collections.Generic;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Module;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public interface IWalletConnectClient
    {
        public Task<DappMetadata> RequestPair(string address, string wcUri,
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp, string dappName);
    }

    public class WalletConnectClientSupplier
    {
        private readonly Dictionary<WalletConnectVersion, IWalletConnectClient> _dictionary = new Dictionary<WalletConnectVersion, IWalletConnectClient>() {
            { WalletConnectVersion.V2, WalletConnectV2Client.GetInstance() }
        };

        public IWalletConnectClient Supply(WalletConnectVersion version)
        {
            this._dictionary.TryGetValue(version, out IWalletConnectClient client);
            return client;
        }
    }
}