using System;
using Dialogue_System.Types;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue_System.Nodes.FlowControl
{
    [NodeIgnore]
    public class GoToExit : DialogueNode
    {
        public bool everHadASource;
        public GoTo source;

#if UNITY_EDITOR
        public override void ModifyVisualElement(VisualElement element)
        {
            if (source != null)
            {
                element.Q("title").style.backgroundColor = new StyleColor(source.color);
                element.Q("title").Q<Label>().text = $"{source.nameOverride} Exit";
                
                element.TrackSerializedObjectValue(new SerializedObject(source), so =>
                {
                    element.Q("title").Q<Label>().text = $"{source.nameOverride} Exit";
                    element.Q("title").style.backgroundColor = new StyleColor(source.color);
                });
            }
            else
            {
                element.TrackSerializedObjectValue(new SerializedObject(this), so =>
                {
                    if (source == null)
                    {
                        if (everHadASource)
                        {
                            EditorRemoveNode?.Invoke(this);
                        }
                        return;
                    }
                    element.Unbind();
                    everHadASource = true;
                    ModifyVisualElement(element);
                });
            }
        }
#endif

        public override Input[] GetDefaultInputs() => Array.Empty<Input>();

        public override bool OnUpdate(IDialogueController controller)
        {
            return true;
        }
    }
}