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
        //Build  android message check. 1ssssssssssssdssasass
        [MenuItem("Build/Build Android")]
        public static void BuildGameAndroid()
        {
            print("check symbols!");
            CheckSymbols();
            CheckSymbols();
            string[] scenes = { "Assets/Scenes/MainMenu.unity" };
            string buildPath = "Builds/teamCity.apk";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
        }
        public static void CheckSymbols()
        {
            BuildTargetGroup targetGroup = BuildTargetGroup.Android;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            Debug.Log("Current define symbols for Android: " + defines);
        }
   
    }
}
