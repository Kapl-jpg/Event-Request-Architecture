using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

namespace Event_Request_Architecture.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;

    public class ScriptFinder : EditorWindow
    {
        private readonly List<string> _scripts = new();

        private float _searchPanelHeight = 0.05f;

        [MenuItem("Tools/Event-Request/Show Dependencies")]
        public static void ShowWindow()
        {
            GetWindow<ScriptFinder>("Find Scripts");
        }

        public static void ScanEvents()
        {
            string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            var results = new HashSet<string>();

            // паттерн для нахождения вызовов Trigger/Subscribe
            string pattern = @"EventManager\.(Trigger|Subscribe)(<[^>]+>)?\s*\(([^;]+?)\)";
            Regex regex = new Regex(pattern);

            foreach (var file in files)
            {
                string text = File.ReadAllText(file);
                foreach (Match match in regex.Matches(text))
                {
                    string args = match.Groups[3].Value.Trim();

                    // берём первый аргумент (до запятой)
                    string firstArg = args.Split(',')[0].Trim();

                    string eventName = ExtractEventName(firstArg);
                    if (!string.IsNullOrEmpty(eventName))
                        results.Add(eventName);
                }
            }

            Debug.Log("Найденные ивенты:\n" + string.Join("\n", results));
        }

        private static string ExtractEventName(string arg)
        {
            // 1. Строка в кавычках
            if (arg.StartsWith("\"") && arg.EndsWith("\""))
            {
                return arg.Substring(1, arg.Length - 2);
            }

            // 2. Интерполированная строка $"..."
            if (arg.StartsWith("$\""))
            {
                // убираем префикс $"
                string inner = arg.Substring(2).TrimEnd('"');

                // ищем первую закрывающую скобку интерполяции
                int closingBrace = inner.IndexOf('}');
                if (closingBrace >= 0)
                {
                    string before = inner.Substring(0, closingBrace + 1);
                    string after = inner.Substring(closingBrace + 1);

                    // если что-то есть после }
                    if (!string.IsNullOrEmpty(after))
                    {
                        return after.TrimStart('.'); // отбрасываем точку
                    }

                    // если что-то есть до интерполяции и оно не пустое
                    int dotIndex = inner.IndexOf(".{");
                    if (dotIndex > 0)
                    {
                        return inner.Substring(0, dotIndex);
                    }
                }

                // fallback — просто вернуть содержимое
                return inner;
            }

            // 3. Просто идентификатор или выражение
            return arg;
        }

        
        private void CreateGUI()
        {
            ScanEvents();
            VisualElement root = rootVisualElement;
            var height = position.height * _searchPanelHeight;
            var splitView = new TwoPaneSplitView(0, height, TwoPaneSplitViewOrientation.Vertical);

            var searchPanel = new VisualElement();
            searchPanel.Add(new Label { text = "Search Scripts" });

            var viewPanel = new ScrollView(ScrollViewMode.Vertical);

            var eventFoldout = new Foldout();
            eventFoldout.text = "Events";

            viewPanel.Add(eventFoldout);

            splitView.Add(searchPanel);
            splitView.Add(viewPanel);

            root.Add(splitView);
        }
    }
}