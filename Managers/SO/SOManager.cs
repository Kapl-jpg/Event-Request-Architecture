using System.Collections.Generic;
using UnityEngine;

public static class SOManager
{
    private static ScriptableObjectRegistry registry;
    private static readonly Dictionary<string, ScriptableObject> Registry = new();

    public static void Register(string key, ScriptableObject so)
    {
        if (Registry.TryAdd(key, so))
        {
            Debug.Log($"Registered SO with the key: {key}");
        }
        else
        {
            Debug.LogWarning($"The key {key} is already registered!");
        }
    }

    public static T Get<T>(string key) where T : ScriptableObject
    {
        if (!registry)
        {
            registry = Resources.Load<ScriptableObjectRegistry>("SORegistry");
            if (!registry)
            {
                Debug.LogError("ScriptableObjectRegistry not found in Resources!");
                return null;
            }
        }

        if (Registry.TryGetValue(key, out ScriptableObject so))
        {
            return so as T;
        }

        Debug.LogWarning($"Object with key '{key}' not found or has wrong type.");
        return null;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (!registry)
        {
            registry = Resources.Load<ScriptableObjectRegistry>("SORegistry");
        }
    }
}