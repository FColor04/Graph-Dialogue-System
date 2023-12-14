using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dialogue_System.Editor
{
    [Serializable]
    public class DialogueNodeClipboard
    {
        public Vector2 minPos;
        public List<DialogueNode> nodes;
        public List<ClipboardNodeConnection<int>> connections = new ();
        public List<ClipboardNodeConnection<string>> externalConnections = new ();

        public DialogueNodeClipboard(IEnumerable<GraphElement> elements)
        {
            var nodesArray = elements.OfType<NodeElement>().ToArray();
            if (nodesArray.Length > 0 && nodesArray[0] is NodeElement e)
                minPos = e.GraphPosition;
            nodes = nodesArray.Select(n => n.node).ToList();
            for (var nodeIndex = 0; nodeIndex < nodesArray.Length; nodeIndex++)
            {
                var currentNodeElement = nodesArray[nodeIndex];
                var min = currentNodeElement.GraphPosition;
                if (min.x < minPos.x)
                    minPos.x = min.x;
                if (min.y < minPos.y)
                    minPos.y = min.y;

                for (var outputIndex = 0; outputIndex < currentNodeElement.node.Outputs.Count; outputIndex++)
                {
                    var output = currentNodeElement.node.Outputs[outputIndex];
                    foreach (var child in output.children)
                    {
                        var connectsTo = nodes.IndexOf(child.node);
                        if (connectsTo >= 0)
                        {
                            connections.Add(new (nodeIndex, connectsTo, outputIndex, child.inputIndex));
                        }
                    }
                }

                foreach (var connection in currentNodeElement.node.GetInputNodeConnections())
                {
                    if (nodes.Contains(connection.source)) continue;
                    if (connection.source.Outputs[connection.outputIndex].capacity == Capacity.Single) continue;
                    externalConnections.Add(new ClipboardNodeConnection<string>(connection.source.guid, nodeIndex, connection.outputIndex, connection.inputIndex));
                }
            }
        }
    }
}