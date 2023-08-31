using UnityBuilderAction;
using UnityEditor;
using UnityEngine;

public class FaceBuildMenu : MonoBehaviour
{
    [MenuItem("FaceWallet/Build Android Dev")]
    static void BuildAndroidDev()
    {
        var newVersion = VersionUpgrader.Upgrade(FaceDeployEnvironment.Dev);
        BuildScript.CustomizeForFace(FaceDeployEnvironment.Dev, newVersion);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.target = BuildTarget.Android;

        var buildAndroidAndroidAPK = "build/Android/Android.apk";
        buildPlayerOptions.locationPathName = buildAndroidAndroidAPK;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Build success " + buildAndroidAndroidAPK);
        EditorUtility.RevealInFinder(buildAndroidAndroidAPK);
    }

    [MenuItem("FaceWallet/Bump dev version")]
    static void BumpDevVersion()
    {
        VersionUpgrader.Upgrade(FaceDeployEnvironment.Dev);
    }
}