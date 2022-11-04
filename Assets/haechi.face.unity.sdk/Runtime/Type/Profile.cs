using System.Collections.Generic;
using System.ComponentModel;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum Profile
    {
        Dev,
        StageTest,
        StageMainnet,
        ProdTest,
        ProdMainnet
    }

    public static class Profiles
    {
        private static Dictionary<string, Profile> _strProfileDictionary = new Dictionary<string, Profile>
        {
            {"Dev", Profile.Dev},
            {"StageTest", Profile.StageTest},
            {"StageMainnet", Profile.StageMainnet},
            {"Testnet", Profile.ProdTest},
            {"Mainnet", Profile.ProdMainnet}
        };
        public static Profile ValueOf(string value)
        {
            if (!_strProfileDictionary.TryGetValue(value, out Profile result))
            {
                throw new InvalidEnumArgumentException($"Enum name with {value} does not exist");
            }

            return result;
        }
        
        
    }
}