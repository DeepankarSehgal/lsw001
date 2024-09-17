using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class onUnityLoads
{
    // Static constructor is called once when Unity loads
    static onUnityLoads()
    {
        // Your code here
        Debug.Log("Unity has loaded, and this script is running.");
        // Switch to Android build target
        if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
        AddDefineSymbol();

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
