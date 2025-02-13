using System;
using UnityEditor;
using UnityEngine;

namespace EventRequest.Managers
{
    public static class AttributeChecker
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnCompilationFinished()
        {
            var methods = TypeCache.GetMethodsWithAttribute<EventAttribute>();

            foreach (var method in methods)
            {
                if (method.DeclaringType == null) continue;
                
                if (!method.DeclaringType.IsSubclassOf(typeof(Subscriber)))
                {
                    Debug.LogException(new Exception(
                        $"Класс {method.DeclaringType.Name} имеет [Event('{method.Name}')] атрибут, но не наследуется от Subscriber!"));
                }
            }
        
            var fields = TypeCache.GetFieldsWithAttribute<RequestAttribute>();

            foreach (var field in fields)
            {
                if (field.DeclaringType == null) continue;
                {
                    if (!field.DeclaringType.IsSubclassOf(typeof(Subscriber)))
                    {
                        Debug.LogException(new Exception(
                            $"Класс {field.DeclaringType.Name} имеет [Request('{field.Name}')] атрибут, но не наследуется от Subscriber!"));
                    }
                }
            }
        }
    }
}