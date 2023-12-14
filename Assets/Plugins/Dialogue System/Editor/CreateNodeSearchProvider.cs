using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dialogue_System.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dialogue_System.Editor
{
    public class CreateNodeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public Vector2 position;
        public Func<Type, Vector2, DialogueNode> CreateNodeCallback = (_, _) => null;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var list = new List<SearchTreeEntry>();
            var categories = new Dictionary<string, List<SearchTreeEntry>>();
            list.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<DialogueNode>())
            {
                if (nodeType.GetCustomAttribute<NodeIgnoreAttribute>() != null) continue;
                var nodeName = ObjectNames.NicifyVariableName(nodeType.Name).Replace(" Node", "");
                var category = nodeType.GetCustomAttribute<NodeCategoryAttribute>()?.Category ?? "";
                if (string.IsNullOrWhiteSpace(category))
                {
                    list.Add(new SearchTreeEntry(new GUIContent(nodeName)){userData = nodeType, level = 1});
                    continue;
                }

                if (!categories.TryGetValue(category, out var categoryList))
                    categories.Add(category, new List<SearchTreeEntry>());
                categoryList ??= categories[category];
                categoryList.Add(new SearchTreeEntry(new GUIContent(nodeName)){userData = nodeType, level = 2});

            }

            foreach (var category in categories)
            {
                list.Add(new SearchTreeGroupEntry(new GUIContent(category.Key), 1));
                list.AddRange(category.Value);
            }
            
            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            CreateNodeCallback?.Invoke(SearchTreeEntry.userData as Type, position);
            return true;
        }
    }
}