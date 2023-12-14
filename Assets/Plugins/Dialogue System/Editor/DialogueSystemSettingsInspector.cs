using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dialogue_System.Editor
{
    public class DialogueSystemSettingsInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DialogueSystemSettingsInspector, UxmlTraits> {}

        public DialogueSystemSettingsInspector()
        {
            
        }

        public void UpdateSettings(DialogueSystemSettings settings)
        {
            Clear();
            Add(new InspectorElement(settings));
        }

    }
}