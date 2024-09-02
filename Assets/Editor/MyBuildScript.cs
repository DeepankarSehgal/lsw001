using UnityEditor;

namespace MyCompany.Build
{
    public static class MyBuildScript
    {
        public static void BuildGame()
        {
            string[] scenes = { "Assets/Scenes/MainMenu.unity" };
            string buildPath = "Builds/Windows";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }
    }
}
