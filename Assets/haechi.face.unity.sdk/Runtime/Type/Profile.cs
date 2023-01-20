using System.Collections.Generic;
using System.ComponentModel;

namespace haechi.face.unity.sdk.Runtime.Type
{
    public enum Profile
    {
        Local,
        Dev,
        StageTest,
        StageMainnet,
        ProdTest,
        ProdMainnet
    }

    public static class Profiles
    {
        public static bool IsMainNet(Profile profile)
        {
            return profile.Equals(Profile.ProdMainnet) || profile.Equals(Profile.StageMainnet);
        }
        
        private static Dictionary<string, Profile> _strProfileDictionary = new Dictionary<string, Profile>
        {
            {"Local", Profile.Local},
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