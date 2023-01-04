using UnityEditor;

class PerformBuild
{
    public static void Android ()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.scenes = new[] { "Assets/haechi.face.unity.sdk/Samples/SampleDapp/Scenes/SampleScene.unity" };
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.locationPathName = "face-unity-sample.apk";
        BuildPipeline.BuildPlayer(buildPlayerOptions); 
    }
    public static void IOS ()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/haechi.face.unity.sdk/Samples/SampleDapp/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = "face-unity-sample";
        buildPlayerOptions.target = BuildTarget.iOS;
        BuildPipeline.BuildPlayer(buildPlayerOptions); 
    }
}
 