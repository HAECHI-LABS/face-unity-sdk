using System.IO;
using UnityEngine;

public class VersionUpgrader
{
    public class Version
    {
        public readonly int major;
        public readonly int minor;
        public readonly int patch;

        public override string ToString()
        {
            return $"{major}.{minor}.{patch}";
        }

        public Version(int major, int minor, int patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public Version BumpPatch()
        {
            return new Version(this.major, this.minor, this.patch + 1);
        }
    }

    public static Version Upgrade(FaceDeployEnvironment env)
    {
        Version version = ReadVersion(env);
        Debug.Log("prev version" + version);
        var newVersion = version.BumpPatch();
        Debug.Log("new version" + newVersion);
        WriteVersion(env, version);
        return newVersion;
    }

    private static void WriteVersion(FaceDeployEnvironment env, Version version)
    {
        string versionString = version.ToString();
        switch (env)
        {
            case FaceDeployEnvironment.Dev:
                File.WriteAllText("VERSION.dev", versionString);
                break;
            case FaceDeployEnvironment.Stage:
                File.WriteAllText("VERSION.stage", versionString);
                break;
            default:
                throw new System.Exception("Unknown environment");
        }
    }

    private static Version ReadVersion(FaceDeployEnvironment env)
    {
        // read file  in csharp
        // https://stackoverflow.com/questions/3681052/how-to-read-a-file-in-c

        string versionString;
        switch (env)
        {
            case FaceDeployEnvironment.Dev:
                versionString = File.ReadAllText("VERSION.dev");
                break;
            case FaceDeployEnvironment.Stage:
                versionString = File.ReadAllText("VERSION.stage");
                break;
            default:
                throw new System.Exception("Unknown environment");
        }

        versionString = versionString.Trim();
        string[] versionParts = versionString.Split('.');
        if (versionParts.Length != 3)
        {
            throw new System.Exception("Invalid version format");
        }

        int major = int.Parse(versionParts[0]);
        int minor = int.Parse(versionParts[1]);
        int patch = int.Parse(versionParts[2]);
        return new Version(major, minor, patch);
    }
}