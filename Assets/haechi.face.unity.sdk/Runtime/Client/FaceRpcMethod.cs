namespace haechi.face.unity.sdk.Runtime.Client
{
    public enum FaceRpcMethod
    {
        wallet_switchEthereumChain,
        wallet_initialize,
        face_logInSignUp,
        face_logOut,
        face_currentUser,
        face_loggedIn,
        face_accounts,
        face_openWalletConnect,

        eth_getBalance,
        eth_sendTransaction,
        eth_call,
        personal_sign
    }
}