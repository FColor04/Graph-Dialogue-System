using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dialogue_System.Editor
{
    public class NodeInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NodeInspector, UxmlTraits> {}

        public NodeInspector()
        {
            
        }

        public void UpdateSelection(DialogueNode node)
        {
            Clear();
            if (node != null)
                Add(new InspectorElement(node));
        }
    }
}