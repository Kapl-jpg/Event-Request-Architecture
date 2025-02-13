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
                        $"The {method.DeclaringType.Name} class has the [Event('{method.Name}')] attribute, but it does not inherit from Subscriber!"));
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
                            $"The {field.DeclaringType.Name} class has the [Request('{field.Name}')] attribute, but it does not inherit from Subscriber!"));
                    }
                }
            }
        }
    }
}