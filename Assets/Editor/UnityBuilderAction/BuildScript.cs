using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// copied from https://github.com/game-ci/documentation/blob/main/example/BuildScript.cs
namespace UnityBuilderAction
{
    public static class BuildScript
    {
        private static readonly string Eol = Environment.NewLine;

        private static readonly string[] Secrets =
            { "androidKeystorePass", "androidKeyaliasName", "androidKeyaliasPass" };

        private const string FaceDeploymentEnvArgName = "faceDeployEnvironment";

        public static void CustomizeForFace(FaceDeployEnvironment deployEnvironment)
        {
            EditorBuildSettingsScene[] scenes;
            switch (deployEnvironment)
            {
                case FaceDeployEnvironment.Dev:
                    PlayerSettings.applicationIdentifier = FaceDeployConstants.DevAppId;
                    scenes = new[] { new EditorBuildSettingsScene(FaceDeployConstants.DevSecnePath, true) };
                    break;
                case FaceDeployEnvironment.Stage:
                    PlayerSettings.applicationIdentifier = FaceDeployConstants.StageAppId;
                    scenes = new[] { new EditorBuildSettingsScene(FaceDeployConstants.StageSecnePath, true) };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deployEnvironment), deployEnvironment, null);
            }
            EditorBuildSettings.scenes = scenes;
            Debug.Log("Application identifier" + PlayerSettings.applicationIdentifier);
            Debug.Log("Scenes" + EditorBuildSettings.scenes);

            #if UNITY_2022_1_OR_NEWER
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Debug);
                PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.Android, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
            #elif UNITY_2020_3_OR_NEWER
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Debug);
                EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize;
            #endif
        }

        // called from CI
        public static void Build()
        {
            // Gather values from args
            Dictionary<string, string> options = GetValidatedOptions();
            Enum.TryParse(options[FaceDeploymentEnvArgName], out FaceDeployEnvironment deployEnvironment);
            CustomizeForFace(deployEnvironment);

            // Set version for this build
            PlayerSettings.bundleVersion = options["buildVersion"];
            PlayerSettings.macOS.buildNumber = options["buildVersion"];
            PlayerSettings.Android.bundleVersionCode = int.Parse(options["androidVersionCode"]);

            // Apply build target
            var buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), options["buildTarget"]);
            switch (buildTarget)
            {
                case BuildTarget.Android:
                {
                    EditorUserBuildSettings.buildAppBundle =
                        options["customBuildPath"].EndsWith(".aab");
                    if (options.TryGetValue("androidKeystoreName", out string keystoreName) &&
                        !string.IsNullOrEmpty(keystoreName))
                    {
                        PlayerSettings.Android.useCustomKeystore = true;
                        PlayerSettings.Android.keystoreName = keystoreName;
                    }

                    if (options.TryGetValue("androidKeystorePass", out string keystorePass) &&
                        !string.IsNullOrEmpty(keystorePass))
                        PlayerSettings.Android.keystorePass = keystorePass;
                    if (options.TryGetValue("androidKeyaliasName", out string keyaliasName) &&
                        !string.IsNullOrEmpty(keyaliasName))
                        PlayerSettings.Android.keyaliasName = keyaliasName;
                    if (options.TryGetValue("androidKeyaliasPass", out string keyaliasPass) &&
                        !string.IsNullOrEmpty(keyaliasPass))
                        PlayerSettings.Android.keyaliasPass = keyaliasPass;
                    if (options.TryGetValue("androidTargetSdkVersion",
                            out string androidTargetSdkVersion) &&
                        !string.IsNullOrEmpty(androidTargetSdkVersion))
                    {
                        var targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
                        try
                        {
                            targetSdkVersion =
                                (AndroidSdkVersions)Enum.Parse(typeof(AndroidSdkVersions),
                                    androidTargetSdkVersion);
                        }
                        catch
                        {
                            UnityEngine.Debug.Log(
                                "Failed to parse androidTargetSdkVersion! Fallback to AndroidApiLevelAuto");
                        }

                        PlayerSettings.Android.targetSdkVersion = targetSdkVersion;
                    }

                    break;
                }
                case BuildTarget.StandaloneOSX:
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone,
                        ScriptingImplementation.Mono2x);
                    break;
            }

            // Determine subtarget
            int buildSubtarget = 0;
#if UNITY_2021_2_OR_NEWER
            if (!options.TryGetValue("standaloneBuildSubtarget", out var subtargetValue) ||
                !Enum.TryParse(subtargetValue, out StandaloneBuildSubtarget buildSubtargetValue))
            {
                buildSubtargetValue = default;
            }

            buildSubtarget = (int)buildSubtargetValue;
#endif

            // Custom build
            Build(buildTarget, buildSubtarget, options["customBuildPath"]);
        }

        private static Dictionary<string, string> GetValidatedOptions()
        {
            ParseCommandLineArguments(out Dictionary<string, string> validatedOptions);

            if (!validatedOptions.TryGetValue(FaceDeploymentEnvArgName, out string _))
            {
                Console.WriteLine("Missing argument -" + FaceDeploymentEnvArgName);
                EditorApplication.Exit(100);
            }

            if (!validatedOptions.TryGetValue("projectPath", out string _))
            {
                Console.WriteLine("Missing argument -projectPath");
                EditorApplication.Exit(110);
            }

            if (!validatedOptions.TryGetValue("buildTarget", out string buildTarget))
            {
                Console.WriteLine("Missing argument -buildTarget");
                EditorApplication.Exit(120);
            }

            if (!Enum.IsDefined(typeof(BuildTarget), buildTarget ?? string.Empty))
            {
                Console.WriteLine($"{buildTarget} is not a defined {nameof(BuildTarget)}");
                EditorApplication.Exit(121);
            }

            if (!validatedOptions.TryGetValue("customBuildPath", out string _))
            {
                Console.WriteLine("Missing argument -customBuildPath");
                EditorApplication.Exit(130);
            }

            const string defaultCustomBuildName = "TestBuild";
            if (!validatedOptions.TryGetValue("customBuildName", out string customBuildName))
            {
                Console.WriteLine(
                    $"Missing argument -customBuildName, defaulting to {defaultCustomBuildName}.");
                validatedOptions.Add("customBuildName", defaultCustomBuildName);
            }
            else if (customBuildName == "")
            {
                Console.WriteLine(
                    $"Invalid argument -customBuildName, defaulting to {defaultCustomBuildName}.");
                validatedOptions.Add("customBuildName", defaultCustomBuildName);
            }

            return validatedOptions;
        }

        private static void ParseCommandLineArguments(
            out Dictionary<string, string> providedArguments)
        {
            providedArguments = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();

            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#    Parsing settings     #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}"
            );

            // Extract flags with optional values
            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse flag
                bool isFlag = args[current].StartsWith("-");
                if (!isFlag) continue;
                string flag = args[current].TrimStart('-');

                // Parse optional value
                bool flagHasValue = next < args.Length && !args[next].StartsWith("-");
                string value = flagHasValue ? args[next].TrimStart('-') : "";
                bool secret = Secrets.Contains(flag);
                string displayValue = secret ? "*HIDDEN*" : "\"" + value + "\"";

                // Assign
                Console.WriteLine($"Found flag \"{flag}\" with value {displayValue}.");
                providedArguments.Add(flag, value);
            }
        }

        private static void Build(BuildTarget buildTarget, int buildSubtarget, string filePath)
        {
            string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled)
                .Select(s => s.path).ToArray();
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                target = buildTarget,
//                targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget),
                locationPathName = filePath,
//                options = UnityEditor.BuildOptions.Development
#if UNITY_2021_2_OR_NEWER
                subtarget = buildSubtarget
#endif
            };

            BuildSummary buildSummary = BuildPipeline.BuildPlayer(buildPlayerOptions).summary;
            ReportSummary(buildSummary);
            ExitWithResult(buildSummary.result);
        }

        private static void ReportSummary(BuildSummary summary)
        {
            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#      Build results      #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}" +
                $"Duration: {summary.totalTime.ToString()}{Eol}" +
                $"Warnings: {summary.totalWarnings.ToString()}{Eol}" +
                $"Errors: {summary.totalErrors.ToString()}{Eol}" +
                $"Size: {summary.totalSize.ToString()} bytes{Eol}" +
                $"{Eol}"
            );
        }

        private static void ExitWithResult(BuildResult result)
        {
            switch (result)
            {
                case BuildResult.Succeeded:
                    Console.WriteLine("Build succeeded!");
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Console.WriteLine("Build failed!");
                    EditorApplication.Exit(101);
                    break;
                case BuildResult.Cancelled:
                    Console.WriteLine("Build cancelled!");
                    EditorApplication.Exit(102);
                    break;
                case BuildResult.Unknown:
                default:
                    Console.WriteLine("Build result is unknown!");
                    EditorApplication.Exit(103);
                    break;
            }
        }
    }
}