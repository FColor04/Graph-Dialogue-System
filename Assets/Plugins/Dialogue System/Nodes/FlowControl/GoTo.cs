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
    [NodeCategory("FlowControl")]
    public class GoTo : DialogueNode
    {
        public string nameOverride = "Other";
        public Color color = new Color32(160, 130, 20, 255);
        public DialogueNode target;


#if UNITY_EDITOR
        public override void ModifyVisualElement(VisualElement element)
        {
            element.Q("title").style.backgroundColor = new StyleColor(color);
            element.Q("title").Q<Label>().text = $"Go to {nameOverride}";
            element.TrackSerializedObjectValue(new SerializedObject(this), so =>
            {
                element.Q("title").Q<Label>().text = $"Go to {nameOverride}";
                element.Q("title").style.backgroundColor = new StyleColor(color);
                if (target == null)
                {
                    Debug.Log("Removing node");
                    EditorRemoveNode?.Invoke(this);
                }
            });
        }
        
        public override void OnNodeCreated()
        {
            if (target != null) return;
            target = EditorCreateNode?.Invoke(typeof(GoToExit), position + Vector2.right * 64);
            ((GoToExit)target)!.source = this;
            EditorRefreshWindow?.Invoke();
        }
#endif

        public override Output[] GetDefaultOutputs() => Array.Empty<Output>();

        public override bool OnUpdate(IDialogueController controller)
        {
            return true;
        }

        public override DialogueNode GetNextNode() => target;
    }
}