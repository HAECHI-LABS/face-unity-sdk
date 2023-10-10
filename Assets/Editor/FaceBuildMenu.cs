using Editor;
using UnityBuilderAction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class FaceBuildMenu : MonoBehaviour
{
    [MenuItem(("FaceWallet/Dev/Build Android APK"))]
    static void BuildAndroidDev()
    {
        BuildAndRunAndroidDevInner(false);
    }

    [MenuItem(("FaceWallet/Dev/Build and Run Android"))]
    static void BuildAndRunAndroidDev()
    {
        BuildAndRunAndroidDevInner(true);
    }

    [MenuItem("FaceWallet/Dev/Prepare Dev Android Build")]
    static void PrepareDevAndroidBuild()
    {
        BuildScript.CustomizeForFace(FaceDeployEnvironment.Dev);
        SetupAndroidKeystore.Setup();
        EditorSceneManager.OpenScene(FaceDeployConstants.DevSecnePath);
    }

    static void BuildAndRunAndroidDevInner(bool run)
    {
        BuildScript.CustomizeForFace(FaceDeployEnvironment.Dev);
        SetupAndroidKeystore.Setup();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.target = BuildTarget.Android;
        if (run)
        {
            buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
        }

        var buildAndroidAndroidAPK = "build/Android/Android.apk";
        buildPlayerOptions.locationPathName = buildAndroidAndroidAPK;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Build success " + buildAndroidAndroidAPK);
        EditorUtility.RevealInFinder(buildAndroidAndroidAPK);
    }
}