using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class SDKReleaseWindow : EditorWindow
{
    [MenuItem("FaceWallet/Release SDK")]
    public static void ShowWindow()
    {
        GetWindow<SDKReleaseWindow>("SDK Release");
    }

    private string newVersion = "";

    private List<string> logs = new List<string>();

    private void OnGUI()
    {
        GUILayout.Label("SDK Release", EditorStyles.boldLabel);

        GUILayout.Label("This window updates and pushes the version related files and tags the release.");
        GUILayout.Label("The tagging process is done in the tagging.yaml file of the github action.");

        string packageJsonPath = "./haechi.face.unity.sdk/package.json";
        if (!File.Exists(packageJsonPath))
        {
            EditorGUILayout.LabelField("package.json not found");
            // print current directory
            EditorGUILayout.LabelField("Current Directory", Directory.GetCurrentDirectory());
        } else {
            string packageJsonContent = File.ReadAllText(packageJsonPath);

            var packageJson = JObject.Parse(packageJsonContent);
            var packageJsonVersion = packageJson["version"];
            EditorGUILayout.LabelField("Current Version in package.json", packageJsonVersion.ToString());
        }

        string sdkInfoPath = "./haechi.face.unity.sdk/Runtime/Client/SdkInfo.cs";
        string sdkInfoContent = File.ReadAllText(sdkInfoPath);
        var sdkInfoRegex = new Regex("public static readonly string UNITY_SDK_VERSION = \"(.*?)\"");
        var sdkInfoVersion = sdkInfoRegex.Match(sdkInfoContent).Groups[1].Value;
        EditorGUILayout.LabelField("Current Version in SdkInfo.cs", sdkInfoVersion);

        if (String.IsNullOrEmpty(newVersion))
        {
            newVersion = sdkInfoVersion;
        }
        newVersion = EditorGUILayout.TextField("New Version", newVersion);

        if (GUILayout.Button("Change Version"))
        {
            // check version format
            if (!Regex.IsMatch(newVersion, @"^\d+\.\d+\.\d+$"))
            {
                EditorGUILayout.LabelField("Invalid version format");
                return;
            }

            string packageJsonContent = File.ReadAllText(packageJsonPath);
            var packageJson = JObject.Parse(packageJsonContent);
            packageJson["version"] = newVersion;
            File.WriteAllText(packageJsonPath, packageJson.ToString());

            sdkInfoContent = sdkInfoRegex.Replace(sdkInfoContent, $"public static readonly string UNITY_SDK_VERSION = \"{newVersion}\"");
            File.WriteAllText(sdkInfoPath, sdkInfoContent);

            logs.Add($"Version changed to {newVersion}");
        }

        if (GUILayout.Button("Add CHANGELOG.md"))
        {
            string changelogPath = "./haechi.face.unity.sdk/CHANGELOG.md";
            string changelogContent = File.ReadAllText(changelogPath);
            string aTag = $"<a name=\"{newVersion}\"></a>\n";
            string newChangelogContent = $"## Release: [{newVersion}](https://github.com/HAECHI-LABS/core/releases/tag/{newVersion})\n\n";
            File.WriteAllText(changelogPath, aTag + newChangelogContent + changelogContent);

            logs.Add($"CHANGELOG.md updated, please add features and bug fixes");
        }

        GUILayout.Label("Logs", EditorStyles.boldLabel);
        foreach (var log in logs)
        {
            EditorGUILayout.LabelField(log);
        }
    }
}