using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace face_unity.haechi.face.unity.sdk.Runtime.Utils
{
    public class EnumUtils
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T FindEquals<T>(string value)
        {
            foreach (T item in GetValues<T>())
            {
                if (Equals(value, Enum.GetName(typeof(T), item)))
                {
                    return item;
                }
            }

            throw new InvalidEnumArgumentException($"Enum name with {value} does not exist");
        }
        
        public static List<TEnum> AllEnumAsList<TEnum>() where TEnum : Enum 
            => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();
        
        public static TEnum StringToEnum<TEnum>(string value) where TEnum : struct
        {
            TEnum result;
            if (Enum.TryParse<TEnum>(value, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"The value '{value}' is not a valid member of the enum {typeof(TEnum).Name}");
            }
        }
    }
}