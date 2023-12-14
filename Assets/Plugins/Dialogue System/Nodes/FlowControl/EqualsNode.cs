using Dialogue_System.Types;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Dialogue_System.Nodes.FlowControl
{
    [NodeCategory("FlowControl")]
    public class EqualsNode : DialogueNode
    {
        private object _a;
        private object _b;
        
        public override Output[] GetDefaultOutputs() =>
            new []
            {
                new Output("True"),
                new Output("False")
            };

        public override Input[] GetDefaultInputs() => new []
        {
            new Input(""),
            new Input("A", typeof(object)),
            new Input("B", typeof(object))
        };

        public override void OnEnter(IDialogueController controller)
        {
            // if (inputType == TypeEnum.Int)
            //     _value = (int) input;
        }
        
        public override bool OnUpdate(IDialogueController controller) => true;

        public override DialogueNode GetNextNode()
        {
            var @true = Outputs[0]?.children[0].node;
            var @false = Outputs[1]?.children[0].node;
            return Object.Equals(_a, _b) ? @true : @false;
        }

        public override void ModifyVisualElement(VisualElement element)
        {
#if UNITY_EDITOR
            // var preview = element.Q("input").Q(className:"typeObject").Q<Label>("type");
            // preview.text = $"{ToSymbol(comparison)} {compareTo}";
            //
            // preview.TrackSerializedObjectValue(new SerializedObject(this), so =>
            // {
            //     preview.text = $"{ToSymbol(comparison)} {compareTo}";
            // });
#endif
        }
    }
}