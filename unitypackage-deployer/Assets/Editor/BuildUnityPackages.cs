using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildUnityPackages : MonoBehaviour
{
    [MenuItem("HaechiLabs/Build Unity Packages")]
    static void Build()
    {
        Debug.Log("Current Directory: " + Directory.GetCurrentDirectory());

        if (Directory.Exists("./Assets/HaechiLabs/HaechiFaceUnitySDK"))
        {
            Directory.Delete("./Assets/HaechiLabs/HaechiFaceUnitySDK", true);
            Debug.Log("Deleted ./Assets/HaechiLabs/HaechiFaceUnitySDK");
        }

        if (!Directory.Exists("./Assets/HaechiLabs"))
        {
            Directory.CreateDirectory("./Assets/HaechiLabs");
            Debug.Log("Created ./Assets/HaechiLabs");
        }

        FileUtil.CopyFileOrDirectory("../haechi.face.unity.sdk", "./Assets/HaechiLabs/haechi.face.unity.sdk");
        Debug.Log("Copied ../haechi.face.unity.sdk to ./Assets/HaechiLabs/haechi.face.unity.sdk");

        Debug.Log("Exporting package...");
        UnityEditor.AssetDatabase.ExportPackage("./Assets/HaechiLabs/haechi.face.unity.sdk", "../haechi.face.unity.sdk.unitypackage");
        Debug.Log("Exported package at ../haechi.face.unity.sdk.unitypackage");
    }
}
