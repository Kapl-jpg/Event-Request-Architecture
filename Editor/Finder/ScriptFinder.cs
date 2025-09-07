using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Event_Request_Architecture.Editor.Finder
{
    public class ScriptFinder : EditorWindow
    {
        private static readonly Dictionary<string, string> _triggers = new();
        private static readonly Dictionary<string, string> _subscribers = new();
        private static readonly List<string> _results = new();
        private static readonly List<Label> _labels = new();
        private static readonly Color _backgroundColor = new Color(0f, 0.18f, 0.29f);
        private static readonly Color _backlightColor = new Color(0.4f, 0.6f, 0.74f);
        private static readonly Color _textColor = new Color(0.99f, 0.94f, 0.84f);
        private static readonly Color _triggerFieldColor = new Color(0.47f, 0f, 0f);
        private static readonly Color _subscriberFieldColor = new Color(0.76f, 0.07f, 0.12f);

        private float _searchPanelHeight = 0.05f;

        [MenuItem("Tools/Event-Request/Show Dependencies")]
        public static void ShowWindow()
        {
            GetWindow<ScriptFinder>("Event Dependencies");
        }

        private static void ScanEvents()
        {
            string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

            string triggerPattern = @"EventManager\.Trigger\s*(<[^>]+>)?\s*\(([^;]+?)\);";
            string subscribePattern = @"EventManager\.Subscribe\s*(<[^>]+>)?\s*\(([^;]+?)\);";

            Regex triggerRegex = new Regex(triggerPattern);
            Regex subscribeRegex = new Regex(subscribePattern);

            foreach (var file in files)
            {
                string text = File.ReadAllText(file);

                foreach (Match match in triggerRegex.Matches(text))
                {
                    string args = match.Groups[2].Value.Trim();
                    string firstArg = args.Split(',')[0].Trim();
                    string eventName = ExtractEventName(firstArg);
                    string className = FindClassName(text, match.Index);
                    if(!_results.Contains(eventName))
                        _results.Add(eventName);
                    _triggers[eventName] = className;
                }

                foreach (Match match in subscribeRegex.Matches(text))
                {
                    string args = match.Groups[2].Value.Trim();
                    string firstArg = args.Split(',')[0].Trim();
                    string eventName = ExtractEventName(firstArg);
                    string className = FindClassName(text, match.Index);
                    if(!_results.Contains(eventName))
                        _results.Add(eventName);
                    _subscribers[eventName] = className;
                }
            }
        }

        private static string ExtractEventName(string arg)
        {
            if (arg.StartsWith("\"") && arg.EndsWith("\""))
            {
                return arg.Substring(1, arg.Length - 2);
            }

            if (arg.StartsWith("$\""))
            {
                string inner = arg.Substring(2).TrimEnd('"');
                int dotBeforeBrace = inner.IndexOf(".{");
                if (dotBeforeBrace > 0)
                {
                    return inner.Substring(0, dotBeforeBrace);
                }

                int closingBrace = inner.IndexOf('}');
                if (closingBrace >= 0 && closingBrace + 1 < inner.Length)
                {
                    string after = inner.Substring(closingBrace + 1);
                    return after.TrimStart('.').Trim();
                }

                return inner;
            }

            return arg;
        }

        private static string FindClassName(string text, int position)
        {
            string before = text.Substring(0, position);

            var m = Regex.Matches(before, @"class\s+([A-Za-z_][A-Za-z0-9_]*)");

            if (m.Count > 0)
            {
                return m[^1].Groups[1].Value;
            }

            return "<UnknownClass>";
        }

        private void CreateGUI()
        {
            ScanEvents();
            VisualElement root = rootVisualElement;
            root.style.backgroundColor = new StyleColor(_backgroundColor);
            var height = position.height * _searchPanelHeight;
            var splitView = new TwoPaneSplitView(0, height, TwoPaneSplitViewOrientation.Vertical);

            var searchPanel = new VisualElement();
            var searchTextField = new TextField("Search Event");
            var searchTextHeader = searchTextField.Q<Label>();
            searchTextHeader.style.color = _textColor;
            
            var viewPanel = new ScrollView(ScrollViewMode.Vertical);
            viewPanel.Add(CreateColumns());

            for (int i = 0; i < _results.Count; i++)
            {
                var eventFoldout = new Foldout
                {
                    text = _results[i]
                };
                var header = eventFoldout.Q<Label>();
                header.style.color = new StyleColor(_textColor);

                var color = Color.Lerp(_triggerFieldColor, _subscriberFieldColor, i / (_results.Count - 1f));
                eventFoldout.style.backgroundColor = new StyleColor(color);

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.FlexStart;

                var leftCol = new VisualElement();
                leftCol.style.flexDirection = FlexDirection.Column;
                leftCol.style.flexGrow = 1;
                leftCol.style.flexBasis = 0;

                var triggerText = new Label (_triggers[_results[i]]);
                triggerText.style.color = new StyleColor(_textColor);
                leftCol.Add(triggerText);

                var rightCol = new VisualElement();
                rightCol.style.flexDirection = FlexDirection.Column;
                rightCol.style.flexGrow = 1;
                rightCol.style.flexBasis = 0;

                var subscriberText = new Label(_subscribers[_results[i]]);
                subscriberText.style.color = new StyleColor(_textColor);
                rightCol.Add(subscriberText);

                row.Add(leftCol);
                row.Add(rightCol);

                eventFoldout.Add(row);
                
                _labels.Add(leftCol.Q<Label>());
                _labels.Add(rightCol.Q<Label>());
                _labels.Add(eventFoldout.Q<Label>());
                
                viewPanel.Add(eventFoldout);
            }
            
            var labels = new List<Label>();
            searchTextField.RegisterValueChangedCallback(evt =>
            {
                Regex eventNameRegex = new Regex(evt.newValue, RegexOptions.IgnoreCase);
                
                for (int i = 0; i < _labels.Count; i++)
                {
                    labels.Add(_labels[i]);
                    if (eventNameRegex.IsMatch(_labels[i].text))
                    {
                        if (evt.newValue != String.Empty)
                            _labels[i].style.backgroundColor = new StyleColor(_backgroundColor);
                        else
                        {
                            _labels[i].style.backgroundColor = new StyleColor(Color.clear);
                        }
                    }
                    else
                    {
                        _labels[i].style.backgroundColor = new StyleColor(Color.clear);
                    }
                }
            });
            
            searchPanel.Add(searchTextField);
            
            splitView.Add(searchPanel);
            splitView.Add(viewPanel);

            root.Add(splitView);
        }

        private VisualElement CreateColumns()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.FlexStart;

            var leftCol = new VisualElement();
            leftCol.style.flexDirection = FlexDirection.Column;
            leftCol.style.flexGrow = 1;
            leftCol.style.flexBasis = 0;
            leftCol.style.backgroundColor = new StyleColor(_backlightColor);
            
            var triggerText = new Label("Triggers");
            triggerText.style.unityTextAlign = TextAnchor.MiddleLeft;
            triggerText.style.color = _textColor;
                
            leftCol.Add(triggerText);

            var rightCol = new VisualElement();
            rightCol.style.flexDirection = FlexDirection.Column;
            rightCol.style.flexGrow = 1;
            rightCol.style.flexBasis = 0;
            rightCol.style.backgroundColor = new StyleColor(_backlightColor);
            
            var subscriberText = new Label("Subscribers");
            subscriberText.style.unityTextAlign = TextAnchor.MiddleLeft;
            subscriberText.style.color = _textColor;
            
            rightCol.Add(subscriberText);
            
            row.Add(leftCol);
            row.Add(rightCol);
            return row;
        }
    }
}