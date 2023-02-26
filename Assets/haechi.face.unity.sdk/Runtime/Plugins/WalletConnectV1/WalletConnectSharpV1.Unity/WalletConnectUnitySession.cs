using WalletConnectSharpV1.Core;
using WalletConnectSharpV1.Core.Events;
using WalletConnectSharpV1.Core.Models;
using WalletConnectSharpV1.Core.Network;
using WalletConnectSharpV1.Unity.Network;

namespace WalletConnectSharpV1.Unity
{
    public class WalletConnectUnitySession : WalletConnectSession
    {
        static WalletConnectUnitySession()
        {
            TransportFactory.Instance.RegisterDefaultTransport((eventDelegator) =>
            {
                NativeWebSocketTransport transport = NativeWebSocketTransport.GetInstance();
                transport.AttachEventDelegator(eventDelegator);
                return transport;
            });
        }

        public WalletConnectUnitySession(SavedSession savedSession, ITransport transport = null, ICipher cipher = null,
            EventDelegator eventDelegator = null) : base(savedSession, transport, cipher, eventDelegator)
        {
        }
    }
}