using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SetupAndroidKeystore
    {
        private struct EnvFile
        {
            public string keystorePassword;
            public string keyPassword;
        }

        public static void Setup()
        {
            if (File.Exists("./env.json"))
            {
                Debug.Log("env.json exists. use unity-dev.keystore");
                var envFile = File.ReadAllText("./env.json");
                var env = JsonUtility.FromJson<EnvFile>(envFile);

                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = "./unity-dev.keystore";
                PlayerSettings.Android.keystorePass = env.keystorePassword;
                PlayerSettings.Android.keyaliasName = "unity-dev";
                PlayerSettings.Android.keyaliasPass = env.keyPassword;
            }
            else
            {
                Debug.LogWarning("env.json not found. You can't use Google log with id token");
                PlayerSettings.Android.useCustomKeystore = false;
            }
        }
    }
}