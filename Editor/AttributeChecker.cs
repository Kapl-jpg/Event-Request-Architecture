using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
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
            foreach (var method in TypeCache.GetMethodsWithAttribute<T>())
                UpdateScriptForMember(method.DeclaringType, method.Name, isMethod);
        else
            foreach (var field in TypeCache.GetFieldsWithAttribute<T>())
                UpdateScriptForMember(field.DeclaringType, field.Name, isMethod);

        foreach (var type in TypeCache.GetTypesWithAttribute<T>())
            UpdateScriptForMember(type, null, isMethod);
    }

    private static void UpdateScriptForMember(Type dt, string memberName, bool isMethod)
    {
        if (dt == null) return;

        // Check if this class has any Subscriber descendants
        var subscriberDescendants = TypeCache.GetTypesDerivedFrom<Subscriber>();
        bool hasSubscriberDescendant = subscriberDescendants
            .Any(sub => sub != dt && sub.IsSubclassOf(dt));

        // We'll change access only if descendant exists or inheritance is changed
        bool shouldChange = hasSubscriberDescendant;

        var guids = AssetDatabase.FindAssets($"t:MonoScript {dt.Name}");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null && script.GetClass() == dt)
            {
                ReplaceInFile(path, dt.Name, memberName, isMethod, shouldChange);
                break;
            }
        }
    }

    private static void ReplaceInFile(string path, string className, string memberName, bool isMethod, bool shouldChange)
    {
        string source = File.ReadAllText(path);
        bool changed = false;

        // 1. Replace inheritance for MonoBehaviour classes
        string classPattern = $@"(class\s+{className}\s*:\s*)MonoBehaviour";
        string classReplacement = "$1Subscriber";
        var newSource = Regex.Replace(source, classPattern, classReplacement, RegexOptions.Multiline);
        if (newSource != source)
        {
            source = newSource;
            changed = true;
            shouldChange = true; // now part of chain
        }

        // 2. Change access modifiers only if flagged
        if (shouldChange && !string.IsNullOrEmpty(memberName))
        {
            if (isMethod)
            {
                string methodPattern = $@"\[\s*Event[^\]]*\][\s\r\n]*[^\S\r\n]*private(\s+[^\r\n]*?\s+{memberName}\s*\()";
                MatchEvaluator evaluator = m => m.Value.Replace("private", "protected");
                newSource = Regex.Replace(source, methodPattern, evaluator, RegexOptions.Singleline);
            }
            else
            {
                string fieldPattern = $@"\[\s*(Request|TempRequest)[^\]]*\][\s\r\n]*[^\S\r\n]*private(\s+[^\r\n]*?\s+{memberName}\s*(=|;))";
                MatchEvaluator evaluator = m => m.Value.Replace("private", "protected");
                newSource = Regex.Replace(source, fieldPattern, evaluator, RegexOptions.Singleline);
            }

            if (newSource != source)
            {
                source = newSource;
                changed = true;
            }
        }

        if (changed)
        {
            File.WriteAllText(path, source);
            Debug.Log($"[AttributeChecker] Updated file {path}");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
#endif
