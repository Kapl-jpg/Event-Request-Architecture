using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateNamesScripts
{
    [MenuItem("Tools/Event-Request/Generate Names Scripts")]
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

        CreateEventScript(namesFolder);
        CreateRequestScript(namesFolder);
        CreateTempRequestScript(namesFolder);

        AssetDatabase.Refresh();
    }

    private static void CreateEventScript(string namesFolder)
    {
        string eventNamesPath = Path.Combine(namesFolder, "EventNames.cs");
        if (!File.Exists(eventNamesPath))
        {
            File.WriteAllText(eventNamesPath, EventContent());
            Debug.Log("The 'EventNames.cs' script has been created.");
        }
        else
        {
            Debug.Log("The 'EventNames.cs' script already exists.");
        }
    }
    
    private static string EventContent()
    {
        string eventContent =
            @"public struct EventNames
{
    public const string EventName = ""EventName"";
}";
        return eventContent;
    }
    
    private static void CreateRequestScript(string namesFolder)
    {
        string requestNamesPath = Path.Combine(namesFolder, "RequestNames.cs");
        if (!File.Exists(requestNamesPath))
        {
            File.WriteAllText(requestNamesPath, RequestContent());
            Debug.Log("The 'RequestNames.cs' script has been created.");
        }
        else
        {
            Debug.Log("The 'RequestNames.cs' script already exists.");
        }
    }

    private static string RequestContent()
    {
        string requestContent =
            @"public struct RequestNames
{
    public const string RequestName = ""RequestName"";
}";
        return requestContent;
    }
    
    private static void CreateTempRequestScript(string namesFolder)
    {
        string tempRequestNamesPath = Path.Combine(namesFolder, "TempRequestNames.cs");
        if (!File.Exists(tempRequestNamesPath))
        {
            File.WriteAllText(tempRequestNamesPath, TempRequestContent());
            Debug.Log("The 'TempRequestNames.cs' script has been created.");
        }
        else
        {
            Debug.Log("The 'TempRequestNames.cs' script already exists.");
        }
    }

    private static string TempRequestContent()
    {
        string tempRequestContent =
            @"public struct TempRequestNames
{
    public const string TempRequestName = ""TempRequestName"";
}";
        return tempRequestContent;
    }
}