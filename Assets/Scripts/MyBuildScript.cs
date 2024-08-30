using UnityEditor;

namespace MyCompany.Build
{
    public static class MyBuildScript
    {
        public static void BuildGame()
        {
            string[] scenes = { "Assets/Scenes/MainMenu.unity" };
            string buildPath = "C:\\BuildAgent\\work\\17367024b001b829\\" + "Builds\\Windows\\MyGame.exe";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }
    }
}
