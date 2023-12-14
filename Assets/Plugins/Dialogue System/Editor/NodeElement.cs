using System;
using System.Collections.Generic;
using System.Linq;
using Dialogue_System.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue_System.Editor
{
    [Serializable]
    public class NodeElement : UnityEditor.Experimental.GraphView.Node
    {
        [NonSerialized]
        public DialogueTreeViewer Viewer;
        [NonSerialized]
        public Action<NodeElement> OnNodeSelected;
        public DialogueNode node;
        [NonSerialized]
        public List<Input> InputSchemas = new ();
        [NonSerialized]
        public List<Port> Inputs = new ();
        [NonSerialized]
        public List<Output> AlreadyAddedOutputs = new ();
        [NonSerialized]
        public List<Port> Outputs = new ();

        public NodeElement(DialogueNode node, DialogueTreeViewer dialogueTreeViewer) : base("Assets/Scripts/Dialogue System/Editor/Nodes/BasicNode.uxml")
        {
            this.node = node;
            Viewer = dialogueTreeViewer;
            CreateElement();
        }

        public NodeElement(SimpleSpeakNode node, DialogueTreeViewer dialogueTreeViewer) : base("Assets/Scripts/Dialogue System/Editor/Nodes/SpeakNode.uxml")
        {
            this.node = node;
            Viewer = dialogueTreeViewer;
            CreateElement();
        }

        public Vector2 GraphPosition => node.position;

        private void CreateElement()
        {
            title = node.EditorName;
            viewDataKey = node.guid;
            
            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();

            node.EditorCreateNode = Viewer.CreateNode;
            node.EditorRemoveNode = Viewer.RemoveNode;
            node.EditorRefreshWindow = Viewer.RefreshWindow;
            node.EditorRefresh = Refresh;
            node.ModifyVisualElement(this);
        }

        private void Refresh()
        {
            CreateInputPorts();
            CreateOutputPorts();
            Viewer.UpdateView();
        }

        private void CreateInputPorts()
        {
            var toRemove = InputSchemas.Except(node.Inputs).ToArray();

            foreach (var model in toRemove)
            {
                var index = InputSchemas.IndexOf(model);
                Inputs.RemoveAt(index);
                inputContainer.RemoveAt(index);
            }
            
            var toAdd = node.Inputs.Except(InputSchemas).ToArray();
            foreach (var model in toAdd)
            {
                var orientation = model.orientation == Orientation.Horizontal
                    ? UnityEditor.Experimental.GraphView.Orientation.Horizontal
                    : UnityEditor.Experimental.GraphView.Orientation.Vertical;
                var capacity = model.capacity == Capacity.Single ? Port.Capacity.Single : Port.Capacity.Multi;
                var t = ReadType(model.serializedType);
                var port = InstantiatePort(orientation, Direction.Input, capacity, t);
                port.portName = model.label;
                
                if (TryGetTypeColorOverride(t, out var c))
                {
                    port.portColor = c;
                }
                
                inputContainer.Add(port);
                Inputs.Add(port);
                InputSchemas.Add(model);
                port.pickingMode = PickingMode.Position;

                for (int i = 0; i < port.childCount; i++)
                {
                    port[i].pickingMode = PickingMode.Position;
                }
            }
        }

        private Type ReadType(string type)
        {
            Type t = null;
            if (!string.IsNullOrWhiteSpace(type))
                t = Type.GetType(type);
            if (t == null)
            {
                Debug.Log($"{type} couldn't be found.");
                t = typeof(FlowType);
            }
            return t;
        }

        private void CreateOutputPorts()
        {
            var toRemove = AlreadyAddedOutputs.Except(node.Outputs).Select(el => AlreadyAddedOutputs.IndexOf(el)).Where(i => i >= 0).ToArray();

            foreach (var index in toRemove)
            {
                if (Outputs.Count > index)
                {
                    Debug.Log($"Disconnecting {index}");
                    Viewer.DeleteElements(Outputs[index].connections);
                    Outputs.RemoveAt(index);
                }

                if(outputContainer.childCount > index)
                    outputContainer.RemoveAt(index);
                AlreadyAddedOutputs.RemoveAt(index);
            }
            
            var toAdd = node.Outputs.Except(AlreadyAddedOutputs).ToArray();
            foreach (var model in toAdd)
            {
                var i = node.Outputs.IndexOf(model);
                var orientation = model.orientation == Orientation.Horizontal
                    ? UnityEditor.Experimental.GraphView.Orientation.Horizontal
                    : UnityEditor.Experimental.GraphView.Orientation.Vertical;
                var capacity = model.capacity == Capacity.Single ? Port.Capacity.Single : Port.Capacity.Multi;
                var t = ReadType(model.serializedType);
                var port = InstantiatePort(orientation, Direction.Output, capacity, t);
                port.portName = model.label;
                port.userData = i;
                if (TryGetTypeColorOverride(t, out var c))
                {
                    port.portColor = c;
                }

                outputContainer.Add(port);
                Outputs.Add(port);
                AlreadyAddedOutputs.Add(model);
                
                port.pickingMode = PickingMode.Position;

                foreach (var child in port.Children())
                {
                    child.pickingMode = PickingMode.Position;
                }
            }
        }

        private bool TryGetTypeColorOverride(Type t, out Color32 c)
        {
            c = new Color32(255, 255, 255, 255);
            switch (t.Name)
            {
                case "Int32":
                    c = new Color32(197, 75, 222, 255);
                    return true;
                case "String":
                    c = new Color32(217, 181, 98, 255);
                    return true;
                case "Bool":
                    c = new Color32(204, 204, 204, 255);
                    return true;
                case "Object":
                    c = new Color32(204, 204, 204, 255);
                    return true;
                case "Single":
                    c = new Color32(222, 71, 149, 255);
                    return true;
                case "Double":
                    c = new Color32(92, 60, 222, 255);
                    return true;
                case "FlowType":
                    c = new Color32(66, 245, 188, 255);
                    return true;
                default:
                    #if DEBUG
                    Debug.Log($"Type {t.Name} was not found in color definitions");
                    #endif
                    c = Color.HSVToRGB((t.Name.Length % 14) / 14f, 0.8f, 0.6f);
                    return true;
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Dialogue Move node");
            node.position = new Vector2(newPos.xMin, newPos.yMin);
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
    }
}