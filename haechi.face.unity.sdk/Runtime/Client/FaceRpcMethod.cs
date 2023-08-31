using System;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public enum FaceRpcMethod
    {
        wallet_switchEthereumChain,
        face_logInSignUp,
        face_loginWithIdToken,
        face_directSocialLogin,
        face_logOut,
        face_currentUser,
        face_loggedIn,
        face_accounts,
        face_openWalletConnect,
        face_confirmWalletConnectDapp,
        face_closeIframe,
        face_switchNetwork,
        face_openHome,

        eth_getBalance,
        eth_sendTransaction,
        eth_call,
        eth_estimateGas,
        
        bora_connect,
        bora_isConnected,
        
        personal_sign
    }
    
    public static class FaceRpcMethods {
        public static bool Contains(string value)
        {
            return Enum.IsDefined(typeof(FaceRpcMethod), value);
        }
        
        public static FaceRpcMethod ValueOf(string value)
        {
            return EnumUtils.FindEquals<FaceRpcMethod>(value);
        }

        public static bool Is(this FaceRpcMethod self, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return ValueOf(value).Equals(self);
        }
    }
}