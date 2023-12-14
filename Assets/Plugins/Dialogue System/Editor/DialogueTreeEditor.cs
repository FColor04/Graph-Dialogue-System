using System;
using System.Linq;
using Dialogue_System;
using Dialogue_System.Editor;
using Dialogue_System.Types;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class DialogueTreeEditor : EditorWindow
{
    private DialogueTreeViewer graph;
    private NodeInspector inspector;
    private Toolbar assetsToolbar;
    private DialogueSystemSettingsInspector settings;
    [SerializeField]
    private VisualTreeAsset visualTree;
    [SerializeField]
    private StyleSheet styleSheet;
    [SerializeField, HideInInspector]
    private DialogueTree _tree;
    private DialogueTree _currentlyDisplayedTree;
    private bool _updateSelection;
    private int _openTab;
    
    [MenuItem("Tools/Dialogue Tree Editor")]
    public static void ShowWindow()
    {
        DialogueTreeEditor wnd = GetWindow<DialogueTreeEditor>();
        wnd._updateSelection = true;
        wnd.titleContent = new GUIContent("Dialogue Tree Editor");
    }

    [OnOpenAsset]
    public static bool Open(int instanceID)
    {
        DialogueTree file = EditorUtility.InstanceIDToObject(instanceID) as DialogueTree;
        if (file != null)
        {
            OpenForEdit(file);
            return true;
        }
        return false;
    }
    
    public static void OpenForEdit(DialogueTree tree)
    {
        DialogueTreeEditor wnd = GetWindow<DialogueTreeEditor>();
        wnd.titleContent = new GUIContent($"{tree.name} — Dialogue Editor");
        wnd._updateSelection = false;
        wnd._tree = tree;
        wnd.OnSelectionChange();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        visualTree.CloneTree(root);
        root.styleSheets.Add(styleSheet);

        graph = root.Q<DialogueTreeViewer>();
        graph.Window = this;
        graph.OnNodeSelected = OnNodeSelected;
        inspector = root.Q<NodeInspector>();
        settings = root.Q<DialogueSystemSettingsInspector>();
        assetsToolbar = root.Q<Toolbar>();
        
        var graphInspectorPanel = root.Q<SinglePanelView>();
        var inspectorButton = root.Q<ToolbarButton>("InspectorButton");
        var settingsButton = root.Q<ToolbarButton>("SettingsButton");
        var buttonHolder = inspectorButton.parent;
        
        void UpdatePanelSelection(int index)
        {
            for (int i = 0; i < buttonHolder.childCount; i++)
            {
                buttonHolder[i].RemoveFromClassList("active");
            }
            buttonHolder[index].AddToClassList("active");
            graphInspectorPanel.UpdateSelection(index);
        }
        
        inspectorButton.clicked += () => UpdatePanelSelection(0);
        settingsButton.clicked += () => UpdatePanelSelection(1);
        graphInspectorPanel.UpdateSelection(0);
        
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        if (_updateSelection)
        {
            _tree = Selection.activeObject as DialogueTree;

            if (_tree == null && Selection.activeGameObject != null)
            {
                assetsToolbar.Clear();
                
                var assets = Selection.GetFiltered<GameObject>(SelectionMode.Unfiltered)
                    .SelectMany(go => 
                        go.GetComponents<IDialogueAssetProvider>()
                            .SelectMany(provider => provider.DialogueAssets))
                    .ToArray();
                
                foreach (var asset in assets)
                {
                    assetsToolbar.Add(new ToolbarButton(() =>
                    {
                        UpdateView(asset);
                    }){text = asset.name});
                }
                if(assets.Length > 0)
                    UpdateView(assets[0]);
            }
        }

        if (_tree != null && AssetDatabase.CanOpenAssetInEditor(_tree.GetInstanceID()))
        {
            UpdateView(_tree);
        }
    }

    private void UpdateView(DialogueTree tree)
    {
        _currentlyDisplayedTree = tree;
        graph.UpdateView(tree);
        settings.UpdateSettings(tree.settings);
    }

    private void OnNodeSelected(NodeElement node)
    {
        inspector.UpdateSelection(node.node);
        settings.UpdateSettings(_currentlyDisplayedTree.settings);
    }

    private void Update()
    {
        Repaint();
    }
}
