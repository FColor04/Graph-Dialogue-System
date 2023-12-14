using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialogue_System;
using Dialogue_System.Editor;
using Dialogue_System.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NodeConnection = Dialogue_System.NodeConnection;

public class DialogueTreeViewer : GraphView
{
    public DialogueTreeEditor Window;
    public Action<NodeElement> OnNodeSelected;
    private DialogueTree _current;
    private CreateNodeSearchProvider _searchProvider;
    private TrueGraphMousePosition _mousePosition;
    
    public new class UxmlFactory : UxmlFactory<DialogueTreeViewer, UxmlTraits> {}
    public DialogueTreeViewer()
    {
        _searchProvider = ScriptableObject.CreateInstance<CreateNodeSearchProvider>();
        _searchProvider.CreateNodeCallback = CreateNode;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Dialogue System/Editor/DialogueTreeEditor.uss");
        styleSheets.Add(styleSheet);
        
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        _mousePosition = new TrueGraphMousePosition();
        this.AddManipulator(_mousePosition);

        canPasteSerializedData += _ => true;
        unserializeAndPaste += Paste;
        serializeGraphElements += Serialize;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        UpdateView();
        AssetDatabase.SaveAssets();
    }

    private string Serialize(IEnumerable<GraphElement> elements) => JsonUtility.ToJson(new DialogueNodeClipboard(elements));

    private void Paste(string operationName, string data)
    {
        var clipboard = JsonUtility.FromJson<DialogueNodeClipboard>(data);
        var nodeInstances = new List<DialogueNode>();
        
        Undo.RecordObject(_current, "Paste in Dialogue Node");
        
        foreach (var node in clipboard.nodes)
        {
            var nodeInstance = _current.CopyNode(node, node.position - clipboard.minPos + _mousePosition.LastMouseGraphPosition);
            
            foreach (var output in nodeInstance.Outputs) 
                output.children.Clear();
            
            CreateNodeView(nodeInstance);
            nodeInstance.OnNodeCreated();
            nodeInstances.Add(nodeInstance);
        }

        foreach (var connection in clipboard.connections)
        {
            var source = nodeInstances[connection.sourceNodeKey];
            var destination = nodeInstances[connection.destinationNodeIndex];
            source.Outputs[connection.outputIndex].children.Add(new NodeConnection(destination, connection.inputIndex));
            
            var sourceElement = GetNodeElement(source);
            var destinationElement = GetNodeElement(destination);
            var edge = sourceElement.Outputs[connection.outputIndex].ConnectTo(destinationElement.Inputs[connection.inputIndex]);
            AddElement(edge);
        }
        
        foreach (var connection in clipboard.externalConnections)
        {
            var sourceElement = GetNodeElement(connection.sourceNodeKey);
            var source = sourceElement.node;
            var destination = nodeInstances[connection.destinationNodeIndex];
            Undo.RecordObject(source, "Paste in Dialogue Node");
            source.Outputs[connection.outputIndex].children.Add(new NodeConnection(destination, connection.inputIndex));

            var destinationElement = GetNodeElement(destination);
            var edge = sourceElement.Outputs[connection.outputIndex].ConnectTo(destinationElement.Inputs[connection.inputIndex]);
            AddElement(edge);
        }
    }

    public void RefreshWindow() => EditorUtility.SetDirty(Window);

    // public DialogueNode CreateNodeOffsetPos(Type type, Vector2 position = default) => CreateNode(type, position - (Vector2)viewTransform.position);
    public DialogueNode CreateNode(Type type, Vector2 position = default)
    {
        if (_current == null || type == null) return null;
        var node = _current.CreateNode(type, position);
        CreateNodeView(node);
        node.OnNodeCreated();
        return node;
    }

    public NodeElement GetNodeElement(DialogueNode node) => GetNodeByGuid(node.guid) as NodeElement;
    public NodeElement GetNodeElement(string guid) => GetNodeByGuid(guid) as NodeElement;

    public void UpdateView() => UpdateView(_current);
    
    public void UpdateView(DialogueTree tree)
    {
        _current = tree;
        if (_current.entryNode == null)
        {
            _current.entryNode = _current.CreateNode(typeof(EntryNode));
            EditorUtility.SetDirty(_current);
            AssetDatabase.SaveAssets();
        }

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        
        foreach (var node in _current.nodes)
        {
            var parentElement = CreateNodeView(node);
        }

        foreach (var node in _current.nodes)
        {
            var parentElement = GetNodeElement(node);
            
            for (var i = 0; i < node.Outputs.Count; i++)
            {
                foreach (var child in node.Outputs[i].children)
                {
                    var childElement = GetNodeElement(child.node);

                    var edge = parentElement.Outputs[i].ConnectTo(childElement.Inputs[child.inputIndex]);
                    
                    AddElement(edge);
                }
            }
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node && endPort.portType.IsAssignableFrom(startPort.portType)).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        if (change.elementsToRemove != null)
        {
            foreach (var element in change.elementsToRemove)
            {
                if (element is NodeElement node)
                {
                    _current.RemoveNode(node.node);
                }else if (element is Edge edge)
                {
                    var parent = edge.output.node as NodeElement;
                    var child = edge.input.node as NodeElement;
                    var inputIndex = child.Inputs.IndexOf(edge.input);
                    var outputIndex = parent.Outputs.IndexOf(edge.output);
                    _current.RemoveChild(parent!.node, child!.node, inputIndex, outputIndex);
                }
            }
        }

        if (change.edgesToCreate != null)
        {
            foreach (var edge in change.edgesToCreate)
            {
                var parent = edge.output.node as NodeElement;
                var child = edge.input.node as NodeElement;
                var inputIndex = child.Inputs.IndexOf(edge.input);
                var outputIndex = parent.Outputs.IndexOf(edge.output);
                _current.AddChild(parent!.node, child!.node, inputIndex, outputIndex);
            }
        }
        
        return change;
    }

    public NodeElement CreateNodeView(DialogueNode node)
    {
        NodeElement element;
        if (node is SimpleSpeakNode speak)
        {
            element = new NodeElement(speak, this);
        }else
            element = new NodeElement(node, this);
        element.OnNodeSelected = OnNodeSelected;
        AddElement(element);
        return element;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.PreventDefault();
        evt.StopImmediatePropagation();
        _searchProvider.position = _mousePosition.LastMouseGraphPosition;
        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(_mousePosition.LastMousePosition) + Vector2.left * 40), _searchProvider);
    }

    public void RemoveNode(DialogueNode obj)
    {
        DeleteElements(new []{GetNodeElement(obj)});
        _current.RemoveNode(obj);
    }
}
