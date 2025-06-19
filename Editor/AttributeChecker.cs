using System; 
using System.IO; 
using System.Linq; 
using System.Text.RegularExpressions; 
using UnityEditor; 
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class AttributeCheckerEditor
{
    static AttributeCheckerEditor()
    {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private static void OnAfterAssemblyReload()
    {
        ProcessAttributes<EventAttribute>(isMethod: true);
        ProcessAttributes<RequestAttribute>(isMethod: false);
        ProcessAttributes<TempRequestAttribute>(isMethod: false);
    }

    private static void ProcessAttributes<T>(bool isMethod) where T : Attribute
    {
        if (isMethod)
        {
            foreach (var method in TypeCache.GetMethodsWithAttribute<T>())
                UpdateScriptForMember(method.DeclaringType, method.Name, isMethod);
        }
        else
        {
            foreach (var field in TypeCache.GetFieldsWithAttribute<T>())
                UpdateScriptForMember(field.DeclaringType, field.Name, isMethod);
        }

        foreach (var type in TypeCache.GetTypesWithAttribute<T>())
            UpdateScriptForMember(type, null, isMethod);
    }

    private static void UpdateScriptForMember(Type dt, string memberName, bool isMethod)
    {
        if (dt == null) return;

        var hasSubscriberDescendant = TypeCache.GetTypesDerivedFrom<Subscriber>()
            .Any(sub => sub != dt && sub.IsSubclassOf(dt));

        var guids = AssetDatabase.FindAssets($"t:MonoScript {dt.Name}");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null && script.GetClass() == dt)
            {
                ReplaceInFile(path, memberName, isMethod, hasSubscriberDescendant);
                break;
            }
        }
    }

    private static void ReplaceInFile(string path, string memberName, bool isMethod, bool shouldChange)
{
    string source = File.ReadAllText(path);
    string original = source;

    // Replace class inheritance
    source = Regex.Replace(source, @"(class\s+\w+\s*:\s*)MonoBehaviour", "$1Subscriber", RegexOptions.Multiline);

    if (!string.IsNullOrEmpty(memberName) && !isMethod)
    {
        // Replace field declarations
        source = Regex.Replace(source,
            $@"(\[\s*(?:Request|TempRequest)[^\]]*\][\s\r\n]*(?:\s*\[[^\]]*\][\s\r\n]*)*)([ \t]*)(private|protected)\s+([A-Za-z0-9_<>]+)\s+{memberName}\s*(?:=[^;]*;|;)",
            m =>
            {
                var attrs = m.Groups[1].Value;
                var indent = m.Groups[2].Value;
                var access = m.Groups[3].Value;
                var type = m.Groups[4].Value;
                bool hasSerialize = attrs.Contains("SerializeField");
                // Determine access
                var newAccess = shouldChange ? "protected" : "private";
                // Already wrapped?
                if (type.StartsWith("ObservableField<"))
                    return $"{attrs}{indent}{newAccess} {type} {memberName};";
                // Build declaration
                if (hasSerialize)
                    return $"{attrs}{indent}{newAccess} ObservableField<{type}> {memberName};";
                else
                    return $"{attrs}{indent}{newAccess} ObservableField<{type}> {memberName} = new();";
            },
            RegexOptions.Singleline);

        // Ensure initialization for existing ObservableField only when not marked [SerializeField]
        source = Regex.Replace(source,
            $@"ObservableField<[^>]+>\s+{memberName}\s*;",
            m =>
            {
                int idx = m.Index;
                int lineStart = source.LastIndexOf('\n', idx) + 1;
                int searchStart = Math.Max(0, lineStart - 200);
                string context = source.Substring(searchStart, lineStart - searchStart);
                if (context.Contains("[SerializeField"))
                    return m.Value; // leave unchanged
                return m.Value.TrimEnd(';') + " = new();";
            },
            RegexOptions.Singleline);

        // Replace usages outside declarations and attributes, skip lines with attribute or declaration
        var lines = source.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("//") || trimmed.Contains("[Request(") || trimmed.Contains("[TempRequest(")
                || trimmed.Contains("ObservableField<"))
            {
                continue;
            }
            lines[i] = Regex.Replace(line,
                $@"(?<![\.\w])({memberName})(?!\s*\(|\.Value)",
                "$1.Value");
        }
        source = string.Join("\n", lines);
    }
    else if (!string.IsNullOrEmpty(memberName) && isMethod)
    {
        // Replace method access modifier based on shouldChange
        source = Regex.Replace(source,
            $@"(\[\s*Event[^\]]*\][\s\r\n]*[^\S\r\n]*)(private|protected)(\s+[^\r\n]*?\s+{memberName}\s*\()",
            m =>
            {
                var attrsAndIndent = m.Groups[1].Value;
                var access = m.Groups[2].Value;
                var methodSignature = m.Groups[3].Value;
                var newAccess = shouldChange ? "protected" : "private";
                return $"{attrsAndIndent}{newAccess}{methodSignature}";
            },
            RegexOptions.Singleline);
    }

    if (source != original)
    {
        File.WriteAllText(path, source);
        Debug.Log($"[AttributeChecker] Updated file {path}");
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
}
#endif