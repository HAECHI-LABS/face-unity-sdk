using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class BuildUnityPackages : MonoBehaviour
{
    // check https://www.notion.so/haechilabs/SDK-434b5866d95f497582aff125326033b0
    [MenuItem("HaechiLabs/Import Face SDK")]
    static void Build()
    {
        Debug.Log("Current Directory: " + Directory.GetCurrentDirectory());

        if (Directory.Exists("./Assets/haechi.face.unity.sdk"))
        {
            Directory.Delete("./Assets/haechi.face.unity.sdk", true);
            Debug.Log("Deleted ./Assets/haechi.face.unity.sdk");
        }

        string packageJson = File.ReadAllText("../haechi.face.unity.sdk/package.json");
        var packageJsonObj = SimpleJSON.JSON.Parse(packageJson);
        var dependencies = packageJsonObj["dependencies"];
        Debug.Log("Dependencies: " + dependencies.AsObject + " " + dependencies.Count);

        foreach (var dependency in dependencies.AsObject)
        {
            var kv = (KeyValuePair<string, SimpleJSON.JSONNode>)dependency;
            Debug.Log("Dependency: " + kv.Key + " " + kv.Value);
        }

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


        FileUtil.CopyFileOrDirectory("../haechi.face.unity.sdk", "./Assets/haechi.face.unity.sdk");
        Debug.Log("Copied ../haechi.face.unity.sdk to ./Assets/haechi.face.unity.sdk");

        AssetDatabase.ImportAsset("./Assets/haechi.face.unity.sdk", ImportAssetOptions.ForceUpdate);
        Debug.Log("Imported ../haechi.face.unity.sdk to ./Assets/haechi.face.unity.sdk");
    }

    [MenuItem("HaechiLabs/Export Package")]
    static void ExportPackage()
    {
        // Only works after the build
        UnityEditor.AssetDatabase.ExportPackage("Assets/haechi.face.unity.sdk", "../haechi.face.unity.sdk.unitypackage", ExportPackageOptions.Recurse);
        Debug.Log("Exported package at ../haechi.face.unity.sdk.unitypackage");
    }
}