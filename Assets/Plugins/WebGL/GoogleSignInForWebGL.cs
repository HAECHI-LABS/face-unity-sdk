using System.Runtime.InteropServices;
using UnityEngine;

public class GoogleSignInForWebGL
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern void SignInWithGoogle(string objectName, string callback);
#endif

    public static void GoogleSignIn(GameObject gameObject, string callbackName)
    { 
#if UNITY_WEBGL
        SignInWithGoogle(gameObject.name, callbackName);
#endif
    }
}
