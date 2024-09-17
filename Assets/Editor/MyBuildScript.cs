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
            AddDefineSymbol();
            string[] scenes = { "Assets/Scenes/MainMenu.unity" };
            string buildPath = "Builds/teamCity.apk";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
        }

        public static void AddDefineSymbol()
        {
            // Get the current build target group
            BuildTargetGroup targetGroup = BuildTargetGroup.Android;

            // Define the symbol to add
            string defineSymbol = "UNITY_ANDROID";

            // Get the existing define symbols for the target group
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            // Check if the define symbol is already present
            if (!currentDefines.Contains(defineSymbol))
            {
                // Append the define symbol
                string newDefines = currentDefines + ";" + defineSymbol;

                // Set the new define symbols
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
                UnityEngine.Debug.Log($"Added define symbol '{defineSymbol}' for {targetGroup}.");
            }
            else
            {
                UnityEngine.Debug.Log($"Define symbol '{defineSymbol}' is already present for {targetGroup}.");
            }
        }
    }
}
