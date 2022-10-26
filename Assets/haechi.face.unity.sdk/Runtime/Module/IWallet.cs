using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;

namespace haechi.face.unity.sdk.Runtime.Module
{
    // TODO: update interface
    public interface IWallet
    {
        Task<FaceRpcResponse> GetBalance(string account = null);
    }
}