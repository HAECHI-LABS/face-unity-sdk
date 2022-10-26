using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;

namespace haechi.face.unity.sdk.Runtime.Module
{
    /// <summary>
    /// WalletProxy 
    /// </summary>
    public class WalletProxy : IWallet
    {
        private IWallet _wallet;

        public WalletProxy()
        {
        }

        public void Register(IWallet wallet)
        {
            this._wallet = wallet;
        }
        
        public Task<FaceRpcResponse> GetBalance(string account = null)
        {
            return this._wallet.GetBalance(account);
        }
    }
}