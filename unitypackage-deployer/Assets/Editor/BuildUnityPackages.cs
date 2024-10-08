using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
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

        // read ../haechi.face.unity.sdk/package.json
        string packageJson = File.ReadAllText("../haechi.face.unity.sdk/package.json");
        // read depedencies
        // deserialize packagejson and read dependencies using SimpleJSONUnity
        var packageJsonObj = SimpleJSON.JSON.Parse(packageJson);
        var dependencies = packageJsonObj["dependencies"];
        Debug.Log("Dependencies: " + dependencies.AsObject + " " + dependencies.Count);

        foreach (var dependency in dependencies.AsObject)
        {
            var kv = (KeyValuePair<string, SimpleJSON.JSONNode>)dependency;
            Debug.Log("Dependency: " + kv.Key + " " + kv.Value);
        }

        // install dependencies
        foreach (var dependency in dependencies.AsObject)
        {
            var kv = (KeyValuePair<string, SimpleJSON.JSONNode>)dependency;
            Debug.Log("Install Dependency: " + kv.Key + " " + kv.Value.Value + " " + $"{kv.Key}@{kv.Value.Value}");
            AddRequest addRequest = Client.Add($"{kv.Key}@{kv.Value.Value}");
            while (!addRequest.IsCompleted)
            {
                Debug.Log("Waiting for add request to complete...");
                System.Threading.Thread.Sleep(1000);
            }
            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log("Installed " + kv.Key + " " + kv.Value);
            }
            else
            {
                Debug.LogError("Failed to install " + kv.Key + " " + kv.Value + " " + addRequest.Error.message);
                Debug.LogError(addRequest.Error);
            }
        }


        FileUtil.CopyFileOrDirectory("../haechi.face.unity.sdk", "./Assets/HaechiLabs/haechi.face.unity.sdk");
        Debug.Log("Copied ../haechi.face.unity.sdk to ./Assets/HaechiLabs/haechi.face.unity.sdk");

        AssetDatabase.ImportAsset("./Assets/HaechiLabs/haechi.face.unity.sdk", ImportAssetOptions.ForceUpdate);
        Debug.Log("Imported ../haechi.face.unity.sdk to ./Assets/HaechiLabs/haechi.face.unity.sdk");

        // Debug.Log("Exporting package...");
        UnityEditor.AssetDatabase.ExportPackage("Assets/HaechiLabs/haechi.face.unity.sdk", "../haechi.face.unity.sdk.unitypackage");
        Debug.Log("Exported package at ../haechi.face.unity.sdk.unitypackage");
    }

    [MenuItem("HaechiLabs/Export Package")]
    static void ExportPackage()
    {
        // Only works after the build
        UnityEditor.AssetDatabase.ExportPackage("Assets/HaechiLabs/haechi.face.unity.sdk", "../haechi.face.unity.sdk.unitypackage", ExportPackageOptions.Recurse);
        Debug.Log("Exported package at ../haechi.face.unity.sdk.unitypackage");
    }
}