using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum Profile
    {
        Dev,
        StageTest,
        StageMainnet,
        Testnet,
        Mainnet
    }

    public static class Profiles
    {
        public static Profile ValueOf(string value)
        {
            return EnumUtils.FindEquals<Profile>(value);
        }
    }
}