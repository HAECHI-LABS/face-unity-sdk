using System;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    public class UserCancelledException : Exception
    {
        public UserCancelledException(): base("User cancelled.") { }
    }
    
    public class UnKnownException : Exception
    {
        public UnKnownException(string error) : base("User cancelled.") { }
    }
}