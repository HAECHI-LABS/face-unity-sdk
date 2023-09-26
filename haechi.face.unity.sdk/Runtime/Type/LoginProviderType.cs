using System;

namespace haechi.face.unity.sdk.Runtime.Type
{
    /// <summary>
    /// This defines social login provider types.
    /// </summary>
    public enum LoginProviderType
    {
        Google,
        Facebook,
        Apple,
        Twitter,
        Discord,
        Kakao
    }

    public static class LoginProviders
    {
        public static string String(this LoginProviderType provider)
        {
            return provider.ToString();
        }
        
        public static string HostValue(this LoginProviderType type)
        {
            switch (type)
            {
                case LoginProviderType.Google:
                    return "google.com";
                case LoginProviderType.Facebook:
                    return "facebook.com";
                case LoginProviderType.Apple:
                    return "apple.com";
                case LoginProviderType.Twitter:
                    return "twitter.com";
                case LoginProviderType.Discord:
                    return "discord.com";
                case LoginProviderType.Kakao:
                    return "kakao.com";
                default:
                    throw new ArgumentException($"Unknown login provider type: {type}");
            }
        }
    }
}