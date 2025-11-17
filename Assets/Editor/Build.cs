using UnityEditor;
using UnityEngine;

// Script for building application using CLI
public class Build
{
    static string[] scenes =
    {
        "Assets/Scenes/MenuScene.unity",
        "Assets/Scenes/GameplayScene.unity",
    };

    static string outputBasePath = "Builds/";

    // Builds application
    static void BuildApp(string outputPath, BuildTarget target)
    {
        // Switch build target
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, target);

        // Build
        BuildPipeline.BuildPlayer(scenes, outputPath, target, BuildOptions.None);
    }

    // Builds for windows
    public static void BuildWindows()
    {
        BuildApp($"{outputBasePath}/Windows/mlbreakout.exe", BuildTarget.StandaloneWindows64);
    }

    // Builds for macOS
    public static void BuildMac()
    {
        BuildApp($"{outputBasePath}/macOS/mlbreakout.app", BuildTarget.StandaloneOSX);
    }

    // Builds for Web
    public static void BuildWeb()
    {
        // Use gzip, it is easy to handle when deploying to Cloudflare R2
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        BuildApp($"{outputBasePath}/Web", BuildTarget.WebGL);
    }
}
