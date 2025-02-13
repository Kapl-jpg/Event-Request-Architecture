using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateNamesScripts
{
    [MenuItem("Tools/Generate Names Scripts")]
    public static void GenerateScripts()
    {
        string scriptsFolder = "Assets/Scripts";
        if (!AssetDatabase.IsValidFolder(scriptsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Scripts");
            Debug.Log("The 'Scripts' folder has been created.");
        }

        string namesFolder = Path.Combine(scriptsFolder, "Names");

        if (!AssetDatabase.IsValidFolder(namesFolder))
        {
            AssetDatabase.CreateFolder(scriptsFolder, "Names");
            Debug.Log("The 'Names' folder has been created.");
        }
        else
        {
            Debug.Log("The 'Names' folder already exists.");
        }

        string eventNamesPath = Path.Combine(namesFolder, "EventNames.cs");
        string requestNamesPath = Path.Combine(namesFolder, "RequestNames.cs");

        string eventNamesContent =
            @"public struct EventNames
{
    public const string EventName = ""EventName"";
}";

        string requestNamesContent =
            @"public struct RequestNames
{
    public const string RequestName = ""RequestName"";
}";
        if (!File.Exists(eventNamesPath))
        {
            File.WriteAllText(eventNamesPath, eventNamesContent);
            Debug.Log("The 'EventNames.cs' script has been created.");
        }
        else
        {
            Debug.Log("The 'EventNames.cs' script already exists.");
        }

        if (!File.Exists(requestNamesPath))
        {
            File.WriteAllText(requestNamesPath, requestNamesContent);
            Debug.Log("The 'RequestNames.cs' script has been created.");
        }
        else
        {
            Debug.Log("The 'RequestNames.cs' script already exists.");
        }

        AssetDatabase.Refresh();
    }
}