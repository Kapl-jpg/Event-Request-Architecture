using UnityEngine;
using UnityEditor;

public class ScriptableObjectRegistryWindow : EditorWindow {
    private ScriptableObjectRegistry _registry;

    [MenuItem("Tools/Event-Request/SO Registry")]
    public static void ShowWindow() {
        GetWindow<ScriptableObjectRegistryWindow>("SO Registry");
    }

    private void OnEnable() {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObjectRegistry");
        if (guids.Length > 0) {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _registry = AssetDatabase.LoadAssetAtPath<ScriptableObjectRegistry>(path);
        }
    }

    private void OnGUI() {
        EditorGUILayout.Space();

        if (!_registry) {
            EditorGUILayout.HelpBox("The registry was not found", MessageType.Warning);
            if (GUILayout.Button("Create new registry")) {
                CreateRegistry();
            }
            return;
        }

        EditorGUILayout.LabelField("Edit ScriptableObject Registry", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        SerializedObject so = new SerializedObject(_registry);
        SerializedProperty entriesProp = so.FindProperty("entries");
        EditorGUILayout.PropertyField(entriesProp, true);
        so.ApplyModifiedProperties();

        if (!GUILayout.Button("Save changes")) return;
        
        EditorUtility.SetDirty(_registry);
        AssetDatabase.SaveAssets();
    }

    private void CreateRegistry()
    {
        var path = RegistryPath();
        
        if (string.IsNullOrEmpty(path)) return;
        
        _registry = CreateInstance<ScriptableObjectRegistry>();
        AssetDatabase.CreateAsset(_registry, path);
        AssetDatabase.SaveAssets();
    }

    private string RegistryPath()
    {
        string path = "Assets/Resources";
        
        if (AssetDatabase.IsValidFolder(path)) return path + "/SORegistry.asset";
        
        AssetDatabase.CreateFolder("Assets", "Resources");
        Debug.Log("The 'Resources' folder has been created.");

        return path + "/SORegistry.asset";
    }
}