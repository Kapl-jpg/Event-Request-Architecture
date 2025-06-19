using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SOEntry {
    public string key;
    public ScriptableObject so;
}

public class ScriptableObjectRegistry : ScriptableObject {
    public List<SOEntry> entries = new();

    public void RegisterAll()
    {
        foreach (var entry in entries.Where(entry => !string.IsNullOrEmpty(entry.key) && entry.so))
        {
            SOManager.Register(entry.key, entry.so);
        }
    }
}