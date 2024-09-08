using UnityEditor;
using UnityEngine;

namespace MyCompany.Build
{
    
    public class MyBuildScript:MonoBehaviour
    {
        [MenuItem("Build/Build Windows")]
        public static void BuildGame()
        {
            string[] scenes = { "Assets/Scenes/MainMenu.unity" };
            string buildPath = "Builds/Windows.exe";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }
        //Build nonssss android message check. 1
        [MenuItem("Build/Build Android")]
        public static void BuildGameAndroid()
        {
            string[] scenes = { "Assets/Scenes/MainMenu.unity" };
            string buildPath = "Builds/teamCity.apk";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
        }
    }
}
